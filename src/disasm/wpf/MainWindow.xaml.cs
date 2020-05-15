using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using Uno.Build.Targets;
using Uno.Disasm.ILView;

namespace Uno.Disasm
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IBuildLog, IILView, IDisposable
    {
        const string DefaultTitle = "Uno Disassembler";

        CompletionWindow _completionWindow;
        public readonly BuildService BuildService;
        public ObservableCollection<ILItem> Items { get; } = new ObservableCollection<ILItem>();
        public VisibilityMode[] VisibilityModes => VisibilityMode.Items;
        public Disassembler[] Disassemblers => Disassembler.Items;
        readonly Dictionary<Syntax, IHighlightingDefinition> _definitions = new Dictionary<Syntax, IHighlightingDefinition>();
        readonly SynchronizedWriter _writer = new SynchronizedWriter();
        readonly StringBuilder _output = new StringBuilder();
        readonly AppState State;
        bool _isRestoring;

        public Command File_Open { get; }
        public Command File_Close { get; }
        public Command Build_BuildTarget { get; }
        public Command Build_CleanProject { get; }
        public Command Build_Cancel { get; }

        public MainWindow()
        {
            InitializeComponent();
            State = new AppState(this);
            BuildService = new BuildService(this, this);
            Title = DefaultTitle;
            DataContext = this;
            NonActiveWindowTitleBrush = WindowTitleBrush;
            TextEditor.TextArea.TextView.LinkTextForegroundBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#99DAF9"));

            TreeView.SelectedItemChanged += UpdateSelection;
            DisasmBox.SelectionChanged += UpdateSelection;
            DisasmBox.SelectedItem = Disassemblers.First();
            VisibilityBox.SelectionChanged += UpdateSelection;
            VisibilityBox.SelectedItem = VisibilityModes[1];
            SizeChanged += (s, e) => State.UpdateSize();
            Closing += (s, e) => State.UpdateSize();
            Closed += (s, e) => Dispose();

            TreeView.ContextMenuOpening +=
                (s, e) => TreeView.ContextMenu = GetContextMenu(TreeView.GetContainerAtPoint<TreeViewItem>(Mouse.GetPosition(TreeView))?.DataContext as ILItem);
            TreeView.MouseDoubleClick +=
                (s, e) => DoubleClick(TreeView.SelectedItem as ILItem);
            TextEditor.ContextMenuOpening +=
                (s, e) => TextEditor.ContextMenu = GetContextMenu(TreeView.SelectedItem as ILItem);

            MainMenu.Visibility = Visibility.Collapsed;
            KeyUp +=
                (s, e) =>
                {
                    if (e.Key == Key.System)
                        MainMenu.Visibility = MainMenu.Visibility == Visibility.Collapsed
                            ? Visibility.Visible
                            : Visibility.Collapsed;
                };

            File_Open = new Command(
                x =>
                {
                    string filename;
                    if (TryOpenProject(out filename))
                        BuildService.StartBuild(new[] { filename });
                });
            File_Close = new Command(
                x => Close());

            Build_BuildTarget = new Command(
                x => BuildService.StartBuild());
            Build_CleanProject = new Command(
                x => BuildService.StartClean());
            Build_Cancel = new Command(
                x => BuildService.Cancel(),
                x => BuildService.IsBuilding);

            foreach (var target in BuildTargets.Enumerate())
            {
                var targetItem = new MenuItem {Header = target.Identifier};
                targetItem.Click += (s, e) => BuildService.StartBuild(target);
                Build_BuildItem.Items.Add(targetItem);
            }

            State.Load();
            new DispatcherTimer(
                    TimeSpan.FromSeconds(1.0 / 12.0),
                    DispatcherPriority.ApplicationIdle,
                    FlushLog,
                    Application.Current.Dispatcher)
                .Start();

            FindBox.TextChanged += (s, e) =>
            {
                if (_completionWindow == null)
                {
                    _completionWindow = new CompletionWindow(TextEditor.TextArea);
                    _completionWindow.Show();
                    _completionWindow.Closed += delegate {
                        _completionWindow = null;
                    };
                }
            };
        }

        public void Dispose()
        {
            State.Save();
            BuildService.Dispose();
        }

        public bool TryOpenProject(out string filename)
        {
            var ofd = new OpenFileDialog
            {
                Title = "Open Project",
                Filter = "Uno Projects (*.unoproj)|*.unoproj",
            };

            var retval = ofd.ShowDialog(this).Value;
            filename = ofd.FileName;
            Focus();
            return retval;
        }

        void UpdateSelection(object s, object e)
        {
            foreach (var item in Items)
                item.UpdateVisibility(((VisibilityMode)VisibilityBox.SelectedItem).Flags);

            if (!BuildService.IsBuilding && TreeView.SelectedItem is ILItem)
            {
                TextEditor.SyntaxHighlighting = GetHighlightingDefinition(((ILItem)TreeView.SelectedItem).Syntax);
                TextEditor.Text = ((ILItem)TreeView.SelectedItem).GetText(
                    (Disassembler)DisasmBox.SelectedItem,
                    ((VisibilityMode)VisibilityBox.SelectedItem).Flags);

                if (!_isRestoring)
                    TextEditor.ScrollToLine(0);
            }
        }

        void FlushLog(object s = null, object e = null)
        {
            if (_writer.Buffer.Length > 0)
            {
                lock (_writer.Buffer)
                {
                    var str = _writer.Buffer.ToString();
                    TextEditor.AppendText(str);
                    TextEditor.ScrollToEnd();
                    _writer.Buffer.Clear();
                    _output.Append(str);
                }
            }
        }

        TextWriter IBuildLog.Writer => _writer;

        void IBuildLog.Clear()
        {
            State.UpdateView();
            FlushLog();
            lock (_writer.Buffer)
            {
                _output.Clear();
                TextEditor.Clear();
                TextEditor.SyntaxHighlighting = null;
            }
        }

        void IILView.OnBuildStarting(BuildItem build)
        {
            Items.Add(build);
            build.Select();
            Build_Cancel.OnCanExecuteChanged();
            Activate();
            TextEditor.Focus();
        }

        void IILView.OnBuildFinished(BuildItem build)
        {
            _isRestoring = true;

            try
            {
                FlushLog();
                build.BuildLog = _output.ToString();

                foreach (var e in build.Folders)
                    build.AddChild(e);

                if (build.Packages.Count > 0)
                {
                    Items.Clear();
                    Items.Add(build);
                }

                foreach (var e in build.Packages)
                {
                    e.UpdateVisibility(((VisibilityMode) VisibilityBox.SelectedItem).Flags);
                    Items.Add(e);
                }

                build.Select();
                Build_Cancel.OnCanExecuteChanged();

                if (build.Packages.Count == 0 || !State.RestoreView())
                {
                    ((TreeViewItem)TreeView.ItemContainerGenerator.ContainerFromItem(build))
                        .BringIntoView();
                    TreeView.Focus();
                }
            }
            finally
            {
                // Wait for rendering to finish
                Dispatcher.BeginInvoke(
                    (Action)(() => _isRestoring = false),
                    DispatcherPriority.ContextIdle,
                    null);
            }
        }

        void IILView.OnProjectChanged(string project)
        {
            Title = (!string.IsNullOrEmpty(project) ? project + " - " : "") + DefaultTitle;
        }

        void IILView.BeginInvoke<T>(Action<T> method, T arg)
        {
            Dispatcher.BeginInvoke(method, arg);
        }

        IHighlightingDefinition GetHighlightingDefinition(Syntax syntax)
        {
            IHighlightingDefinition result;
            if (!_definitions.TryGetValue(syntax, out result) && syntax != Syntax.None)
            {
                var type = typeof(MainWindow);
                var fullName = type.Namespace + ".Syntax." + syntax + ".xshd";
                using (var stream = type.Assembly.GetManifestResourceStream(fullName))
                using (var reader = new XmlTextReader(stream))
                    result = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                _definitions.Add(syntax, result);
            }

            return result;
        }

        static ContextMenu GetContextMenu(ILItem ilItem)
        {
            var menu = new ContextMenu();

            if (ilItem == null)
                return menu;

            foreach (var command in ILItem.Commands)
            {
                if (command == null)
                {
                    if (menu.Items.Count > 0 &&
                        !(menu.Items[menu.Items.Count - 1] is Separator))
                        menu.Items.Add(new Separator());
                }
                else if (command.CanShow(ilItem))
                {
                    var menuItem = new MenuItem
                    {
                        Header = command.Header,
                        Icon = new Image
                        {
                            Source = ILIconConverter.GetBitmapImage(command.GetIcon(ilItem)),
                            Stretch = Stretch.Fill,
                            Width = 16,
                            Height = 16
                        },
                        IsEnabled = command.CanExecute(ilItem),
                        FontWeight = command.IsDefault(ilItem) 
                                ? FontWeights.Bold 
                                : FontWeights.Normal
                    };

                    menuItem.Click += (sender, args) => command.Execute(ilItem);
                    menu.Items.Add(menuItem);
                }
            }

            if (menu.Items.Count > 0 &&
                menu.Items[menu.Items.Count - 1] is Separator)
                menu.Items.RemoveAt(menu.Items.Count - 1);

            if (menu.Items.Count == 0)
                menu.Items.Add(new MenuItem { Header = "(no commands)", IsEnabled = false });

            return menu;
        }

        static void DoubleClick(ILItem ilItem)
        {
            if (ilItem == null)
                return;

            foreach (var command in ILItem.Commands)
            {
                if (command != null &&
                    command.CanShow(ilItem) &&
                    command.IsDefault(ilItem) &&
                    command.CanExecute(ilItem))
                {
                    command.Execute(ilItem);
                    return;
                }
            }
        }
    }
}
