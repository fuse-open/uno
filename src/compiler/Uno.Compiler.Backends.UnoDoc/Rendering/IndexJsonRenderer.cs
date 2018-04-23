using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Uno.Compiler.Backends.UnoDoc.ViewModels;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;
using Uno.Logging;

namespace Uno.Compiler.Backends.UnoDoc.Rendering
{
    public class IndexJsonRenderer : Renderer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };
        private readonly Dictionary<string, Tuple<DocumentReferenceViewModel, List<DataTypeViewModel>>> _classesByBaseType;

        public IndexJsonRenderer(Log log, string outputPath, HashSet<DocumentViewModel> viewModels) : base(log, outputPath, viewModels)
        {
            LogMessage("Building indexable class cache");
            var sw = Stopwatch.StartNew();
            var classes = ViewModels.OfType<DataTypeViewModel>()
                                 .Where(IsIndexableClass)
                                 .ToList();
            sw.Stop();
            LogMessage("Built indexable class cache in " + sw.Elapsed);

            LogMessage("Building indexable class descendant cache");
            sw = Stopwatch.StartNew();
            _classesByBaseType = new Dictionary<string, Tuple<DocumentReferenceViewModel, List<DataTypeViewModel>>>();
            foreach (var c in classes)
            {
                if (c.Base == null)
                {
                    continue;
                }

                if (!_classesByBaseType.ContainsKey(c.Base.Id.Id))
                {
                    _classesByBaseType.Add(c.Base.Id.Id,
                                           new Tuple<DocumentReferenceViewModel, List<DataTypeViewModel>>(c.Base,
                                                                                                          new List<DataTypeViewModel>()));
                }
                _classesByBaseType[c.Base.Id.Id].Item2.Add(c);
            }
            sw.Stop();
            LogMessage("Built indexable class descendant cache in " + sw.Elapsed);
        }

        private static bool IsIndexableClass(DataTypeViewModel viewModel)
        {
            if (viewModel.Id.Type != "Class" && viewModel.Id.Type != "UxClass")
            {
                return false;
            }

            if (!viewModel.Id.Modifiers.Contains("public"))
            {
                return false;
            }

            return true;
        }

        protected override string FileExtension => "json";
        protected override bool HasIndexDocument => false;
        protected override string RendererName => "index-json";

        protected override HashSet<string> GetExpectedOutputPaths()
        {
            var result = new HashSet<string>(_classesByBaseType.Select(e => e.Value.Item1.Uri.IdUri + "." + FileExtension));
            return result;
        }

        protected override int AddAndUpdateDocuments()
        {
            var count = 0;
            var lockObject = new object();

            LogMessage("Adding/updating " + _classesByBaseType.Count + " documents");
            Parallel.ForEach(_classesByBaseType,
                             root =>
                             {
                                 var baseType = root.Value.Item1;

                                 lock (lockObject)
                                 {
                                     var directory = Path.GetDirectoryName(baseType.Uri.IdUri);
                                     var filename = Path.GetFileName(baseType.Uri.IdUri) + "." + FileExtension;
                                     var targetDirectory = string.IsNullOrWhiteSpace(directory) ? OutputPath : Path.Combine(OutputPath, directory);
                                     var targetPath = Path.Combine(targetDirectory, filename);

                                     Directory.CreateDirectory(targetDirectory);
                                     File.WriteAllText(targetPath, GetRenderedDocumentBody(baseType));
                                     count = count + 1;
                                     WriteAddAndUpdateProgress(count, _classesByBaseType.Count);
                                 }
                             });

            return count;
        }

        private void WriteAddAndUpdateProgress(int current, int total)
        {
            if (current % 100 == 0)
            {
                LogMessage(" - Written " + current + " out of " + total + " documents...");
            }
        }

        private string GetRenderedDocumentBody(DocumentReferenceViewModel baseType)
        {
            var descendants = FindDescendantsOf(baseType.Id.Id);
            var items = descendants.OrderBy(e => e.Titles.FullyQualifiedIndexTitle)
                                   .Select(e => new TableOfContentsEntryViewModel(e.Id,
                                                                                  e.Uri,
                                                                                  new IndexTitlesViewModel(e.Titles.IndexTitle, e.Titles.FullyQualifiedIndexTitle), 
                                                                                  e.Comment?.ToBasicComment(),
                                                                                  e.Returns,
                                                                                  e.Parameters,
                                                                                  null,
                                                                                  e.DeclaredIn))
                                   .ToList();
            var page = new SubclassIndexPageViewModel(baseType, items);
            return JsonConvert.SerializeObject(page, _jsonSerializerSettings);
        }

        private List<DataTypeViewModel> FindDescendantsOf(string id)
        {
            if (!_classesByBaseType.ContainsKey(id))
            {
                return new List<DataTypeViewModel>();
            }

            var descendants = new List<DataTypeViewModel>(_classesByBaseType[id].Item2);
            foreach (var descendant in descendants.ToArray())
            {
                descendants.AddRange(FindDescendantsOf(descendant.Id.Id));
            }
            return descendants;
        }
    }
}