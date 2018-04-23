using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Uno.Compiler.Backends.UnoDoc.ViewModels;
using Uno.Logging;

namespace Uno.Compiler.Backends.UnoDoc.Rendering
{
    public abstract class Renderer
    {
        protected abstract string FileExtension { get; }
        protected abstract bool HasIndexDocument { get; }
        protected abstract string RendererName { get; }

        protected Log Log { get; }
        protected string OutputPath { get; set; }
        protected HashSet<DocumentViewModel> ViewModels { get; }
        protected Dictionary<string, HashSet<DocumentViewModel>> ViewModelsByParent { get; private set; }
        protected Dictionary<string, DocumentViewModel> ViewModelsById { get; private set; }
        
        protected Renderer(Log log, string outputPath, HashSet<DocumentViewModel> viewModels)
        {
            Log = log;
            OutputPath = outputPath;

            // Strip away namespaces that contain no children,
            ViewModels = StripEmptyNamespaces(viewModels);

            // Build a list of all view models by their parent view model
            ViewModelsByParent = BuildParentCache(ViewModels);
            ViewModelsById = BuildViewModelCache(viewModels);

            // Only grab non-virtual view models
            ViewModels = new HashSet<DocumentViewModel>(ViewModels.Where(e => !e.Uri.IsVirtual));
        }

        private HashSet<DocumentViewModel> StripEmptyNamespaces(HashSet<DocumentViewModel> viewModels)
        {
            var parentLookup = BuildParentCache(viewModels);
            var localViewModels = new HashSet<DocumentViewModel>(viewModels);

            while (true)
            {
                var emptyNs = localViewModels.OfType<NamespaceViewModel>()
                                             .Where(e => !parentLookup.ContainsKey(e.Id.Id) || parentLookup[e.Id.Id].Count == 0)
                                             .ToList();
                if (emptyNs.Count == 0) return localViewModels;
                emptyNs.ForEach(ns =>
                {
                    localViewModels.Remove(ns);
                    if (ns.Id.ParentId != null && parentLookup.ContainsKey(ns.Id.ParentId))
                    {
                        parentLookup[ns.Id.ParentId].Remove(ns);
                    }
                });
            }
        }

        protected abstract HashSet<string> GetExpectedOutputPaths();
        protected abstract int AddAndUpdateDocuments();
        protected virtual int AddOrUpdateIndexDocument() => 0;

        protected void LogMessage(string message)
        {
            Log.Message($"[{RendererName}] {message}");
        }

        public int Render(bool skipDeleteDeprecated)
        {
            var count = 0;
            Stopwatch sw;

            Directory.CreateDirectory(OutputPath);

            if (!skipDeleteDeprecated)
            {
                LogMessage("Removing deprecated documents from " + OutputPath);
                sw = Stopwatch.StartNew();
                RemoveDeprecatedDocuments();
                sw.Stop();
                LogMessage("Removed deprecated documents from " + OutputPath + " in " + sw.Elapsed);
            }

            LogMessage("Adding/updating new/existing documents in " + OutputPath);
            sw = Stopwatch.StartNew();
            count += AddAndUpdateDocuments();
            sw.Stop();
            LogMessage("Added/updated new/existing documents in " + OutputPath + " in " + sw.Elapsed);

            if (HasIndexDocument)
            {
                LogMessage("Adding/updating index document in " + OutputPath);
                sw = Stopwatch.StartNew();
                count += AddOrUpdateIndexDocument();
                sw.Stop();
                LogMessage("Added/updated index document in " + OutputPath + " in " + sw.Elapsed);
            }

            return count;
        }

        private void RemoveDeprecatedDocuments()
        {
            var newPaths = GetExpectedOutputPaths();
            if (HasIndexDocument)
            {
                newPaths.Add($"index.{FileExtension}");
            }

            var existingFiles = Directory.EnumerateFiles(OutputPath, "*." + FileExtension, SearchOption.AllDirectories)
                                         .Select(path => path.Substring(OutputPath.Length + 1).Replace("\\", "/"))
                                         .ToList();
            var removedFiles = existingFiles.Where(path => !newPaths.Contains(path)).ToList();
            if (removedFiles.Count == 0) return;

            LogMessage("Removing " + removedFiles.Count + " deprecated documents:");
            Parallel.ForEach(removedFiles,
                             file =>
                             {
                                 LogMessage(" - " + file);
                                 File.Delete(Path.Combine(OutputPath, file));
                             });
        }

        private Dictionary<string, HashSet<DocumentViewModel>> BuildParentCache(IEnumerable<DocumentViewModel> viewModels)
        {
            var result = new Dictionary<string, HashSet<DocumentViewModel>>();

            foreach (var viewModel in viewModels)
            {
                var parent = viewModel.Id.ParentId ?? "__root__";
                if (!result.ContainsKey(parent))
                {
                    result.Add(parent, new HashSet<DocumentViewModel>(new DocumentViewModelEqualityComparer()));
                }
                result[parent].Add(viewModel);
            }

            return result;
        }

        private Dictionary<string, DocumentViewModel> BuildViewModelCache(IEnumerable<DocumentViewModel> viewModels)
        {
            var dict = new Dictionary<string, DocumentViewModel>();
            var hasDuplicates = false;

            foreach (var model in viewModels)
            {
                if (dict.ContainsKey(model.Id.Id))
                {
                    Log.Error($"View model with id {model.Id.Id} already exists in view model cache (originally declared at {dict[model.Id.Id].UnderlyingEntity.Source.FullPath} line {dict[model.Id.Id].UnderlyingEntity.Source.Line} - redeclared at {model.UnderlyingEntity.Source.FullPath} line {model.UnderlyingEntity.Source.Line}");
                    hasDuplicates = true;
                }
                else
                {
                    dict.Add(model.Id.Id, model);
                }
            }

            if (hasDuplicates)
            {
                throw new ArgumentException($"Duplicates in IL detected, this smells like a compiler bug");
            }
            return dict;
        }
    }
}
