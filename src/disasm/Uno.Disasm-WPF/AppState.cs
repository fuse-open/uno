using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;
using Uno.IO;
using Uno.Logging;

namespace Uno.Disasm
{
    class AppState
    {
        static readonly string Filename = 
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "Uno", "disasm-window.json");

        readonly MainWindow _window;
        AppData _data = new AppData();

        public AppState(MainWindow window)
        {
            _window = window;
        }

        public void UpdateView()
        {
            if (_window.TreeView.Items.Count == 0)
                return;

            Save();
        }

        public bool RestoreView()
        {
            if (_data.Tree.Restore(_window.TreeView))
            {
                _window.TextEditor.ScrollToVerticalOffset(_data.VerticalOffset);
                _window.TextEditor.Focus();
                return true;
            }

            return false;
        }

        public void UpdateSize()
        {
            _data.Window = new WindowData(
                (int)_window.Left, 
                (int)_window.Top, 
                (int)_window.Width, 
                (int)_window.Height, 
                _window.WindowState);
        }

        public void Save()
        {
            try
            {
                _data.Tree = new TreeState(_window.TreeView);
                _data.VerticalOffset = _window.TextEditor.VerticalOffset;
                _data.Window.State = _window.WindowState;
                _data.Disassembler = _window.DisasmBox.SelectedIndex;
                _data.VisibilityMode = _window.VisibilityBox.SelectedIndex;
                Disk.Default.CreateDirectory(Path.GetDirectoryName(Filename));
                File.WriteAllText(Filename, JsonConvert.SerializeObject(_data, Formatting.Indented));
            }
            catch (Exception e)
            {
                Log.Default.Error("Failed to save '" + Filename + "': " + e.Message);
            }
        }

        public void Load()
        {
            if (!File.Exists(Filename))
            {
                UpdateSize();
                return;
            }

            try
            {
                _data = JsonConvert.DeserializeObject<AppData>(File.ReadAllText(Filename));
                _window.Left = _data.Window.Left;
                _window.Top = _data.Window.Top;
                _window.Width = _data.Window.Width;
                _window.Height = _data.Window.Height;
                _window.WindowState = _data.Window.State;
                _window.DisasmBox.SelectedItem = Disassembler.Items[ _data.Disassembler];
                _window.VisibilityBox.SelectedItem = VisibilityMode.Items[_data.VisibilityMode];
            }
            catch (Exception e)
            {
                Log.Default.Error("Failed to load '" + Filename + "': " + e.Message);
                Disk.Default.DeleteFile(Filename);
            }
        }

        public struct WindowData
        {
            public int Left;
            public int Top;
            public int Width;
            public int Height;
            public WindowState State;

            public WindowData(int left, int top, int width, int height, WindowState state)
            {
                Left = left;
                Top = top;
                Width = width;
                Height = height;
                State = state;
            }
        }

        public class AppData
        {
            public WindowData Window = new WindowData(0, 0, 1280, 720, WindowState.Normal);
            public TreeState Tree = new TreeState();
            public int Disassembler;
            public int VisibilityMode = 1;
            public double VerticalOffset;
        }
    }
}
