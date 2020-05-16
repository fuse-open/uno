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
    public class ApiReferenceJsonRenderer : Renderer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings
        {
            DefaultValueHandling = DefaultValueHandling.Ignore,
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private readonly List<DocumentViewModel> _uxNamespaces = new List<DocumentViewModel>();
        private readonly Dictionary<string, List<DataTypeViewModel>> _uxClassesByNamespace = new Dictionary<string, List<DataTypeViewModel>>();

        public ApiReferenceJsonRenderer(Log log, string outputPath, HashSet<DocumentViewModel> viewModels) : base(log, outputPath, viewModels)
        {
            LogMessage("Building UX classes by namespace cache");
            var sw = Stopwatch.StartNew();
            foreach (var viewModel in ViewModels.OfType<DataTypeViewModel>().Where(e => e.UxProperties != null))
            {
                if (!_uxClassesByNamespace.ContainsKey(viewModel.UxProperties.UxNamespaceUri))
                {
                    _uxClassesByNamespace.Add(viewModel.UxProperties.UxNamespaceUri, new List<DataTypeViewModel>());
                }
                _uxClassesByNamespace[viewModel.UxProperties.UxNamespaceUri].Add(viewModel);
            }
            sw.Stop();
            LogMessage("Built UX classes by namespace cache in " + sw.Elapsed);

            LogMessage("Building UX namespace cache");
            sw = Stopwatch.StartNew();
            foreach (var viewModel in ViewModels.OfType<NamespaceViewModel>().Where(viewModel => _uxClassesByNamespace.ContainsKey(viewModel.Uri.Href)))
            {
                _uxNamespaces.Add(viewModel);
            }
            sw.Stop();
            LogMessage("Built UX namespace cache in " + sw.Elapsed);
        }

        protected override string FileExtension => "json";
        protected override bool HasIndexDocument => true;
        protected override string RendererName => "api-reference-json";

        protected override HashSet<string> GetExpectedOutputPaths()
        {
            var paths = new HashSet<string>(ViewModels.Select(e => e.Uri.IdUri + "." + FileExtension));
            return paths;
        }

        protected override int AddAndUpdateDocuments()
        {
            var count = 0;
            var lockObject = new object();

            LogMessage("Adding/updating " + ViewModels.Count + " documents");
            Parallel.ForEach(ViewModels,
                             viewModel =>
                             {
                                 var directory = Path.GetDirectoryName(viewModel.Uri.IdUri);
                                 var filename = Path.GetFileName(viewModel.Uri.IdUri) + "." + FileExtension;
                                 var targetDirectory = string.IsNullOrWhiteSpace(directory) ? OutputPath : Path.Combine(OutputPath, directory);
                                 var targetPath = Path.Combine(targetDirectory, filename);

                                 Directory.CreateDirectory(targetDirectory);
                                 File.WriteAllText(targetPath, GetRenderedDocumentBody(viewModel));

                                 lock (lockObject)
                                 {
                                     count = count + 1;
                                     WriteAddAndUpdateProgress(count, ViewModels.Count);
                                 }
                             });
            return count;
        }

        private void WriteAddAndUpdateProgress(int current, int total)
        {
            if (current % 1000 == 0)
            {
                LogMessage(" - Written " + current + " out of " + total + " documents...");
            }
        }

        protected override int AddOrUpdateIndexDocument()
        {
            var path = Path.Combine(OutputPath, "index." + FileExtension);
            var viewModel = new RootViewModel(new TitlesViewModel("Reference", "Reference", "Reference", "Reference", "Reference"));
            File.WriteAllText(path, GetRenderedIndexBody(viewModel));
            return 1;
        }

        private string GetRenderedDocumentBody(DocumentViewModel viewModel)
        {
            var tocBuilder = new TableOfContentsBuilder(viewModel, ViewModelsByParent, ViewModelsById);
            var toc = tocBuilder.Build();
            var page = new PageViewModel(viewModel, toc);

            return JsonConvert.SerializeObject(page, _jsonSerializerSettings);
        }

        private string GetRenderedIndexBody(DocumentViewModel viewModel)
        {
            var tocBuilder = new TableOfContentsBuilder(viewModel, ViewModelsByParent, ViewModelsById);
            var toc = tocBuilder.Build();                       
            var uxNamespaces = _uxNamespaces.Select(e =>
            {
                var entries = _uxClassesByNamespace.ContainsKey(e.Uri.Href) ? _uxClassesByNamespace[e.Uri.IdUri] : new List<DataTypeViewModel>();
                var result = entries.OrderBy(x => x.UxProperties.UxName)
                                    .Select(x => new UxNamespaceEntryViewModel(x.Uri.Href, x.UxProperties.UxName))
                                    .ToList();
                return new UxNamespaceViewModel(e.Uri.Href, e.Titles.FullTitle, result);
            }).OrderBy(e => e.Title).ToList();

            var page = new RootPageViewModel(viewModel, toc, uxNamespaces);
            return JsonConvert.SerializeObject(page, _jsonSerializerSettings);
        }
    }
}