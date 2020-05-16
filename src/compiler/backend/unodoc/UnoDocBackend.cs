using System.Diagnostics;
using System.IO;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.Rendering;

namespace Uno.Compiler.Backends.UnoDoc
{
    public class UnoDocBackend : Backend
    {
        public override string Name => "UnoDoc";

        public UnoDocBackend()
        {
            Options = BackendOptions.ExportDontExports;
        }

        public override bool CanLink(Function f)
        {
            return true;
        }

        public override BackendResult Build()
        {
            var renderToPath = Environment.ExpandSingleLine("@(ReferenceOutputPath:NativePath)");
            var skipDeleteDeprecated = Environment.ExpandSingleLine("@(ReferenceSkipDeleteDeprecated)") == "true";

            var apiPath = Path.Combine(renderToPath, "api");
            var indexPath = Path.Combine(renderToPath, "indicies");

            Log.Message("Starting view model generation");
            var sw = Stopwatch.StartNew();
            var viewModels = new ViewModelExporter(Data.IL, Utilities).BuildExport();
            sw.Stop();
            Log.Message("Generated " + viewModels.Count + " in " + sw.Elapsed);

            Log.Message("Rendering JSON api reference export to " + apiPath);
            sw = Stopwatch.StartNew();
            var count = new ApiReferenceJsonRenderer(Log, apiPath, viewModels).Render(skipDeleteDeprecated);
            sw.Stop();
            Log.Message("Rendered JSON api reference export (" + count + " documents) to " + apiPath + " in " + sw.Elapsed);

            Log.Message("Rendering JSON index export to " + indexPath);
            sw = Stopwatch.StartNew();
            count = new IndexJsonRenderer(Log, indexPath, viewModels).Render(skipDeleteDeprecated);
            sw.Stop();
            Log.Message("Rendered JSON index export (" + count + " documents) to " + indexPath + " in " + sw.Elapsed);
            return null;
        }
    }
}
