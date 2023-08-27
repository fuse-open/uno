using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.AST;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Core.IL;
using Uno.Compiler.Core.IL.Building.Functions;
using Uno.Compiler.Core.IL.Building.Functions.Lambdas;
using Uno.Compiler.Core.IL.Building.Types;
using Uno.Compiler.Core.IL.Bytecode;
using Uno.Compiler.Core.IL.Optimizing;
using Uno.Compiler.Core.IL.Testing;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Core.IL.Validation;
using Uno.Compiler.Core.IL.Validation.ControlFlow;
using Uno.Compiler.Core.Syntax;
using Uno.Compiler.Core.Syntax.Binding;
using Uno.Compiler.Core.Syntax.Builders;
using Uno.Compiler.Frontend;
using Uno.Compiler.Frontend.Analysis;
using Uno.Diagnostics;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.Core
{
    public class Compiler : LogObject, ICompiler, IScheduler
    {
        public Backend Backend { get; }
        public Disk Disk { get; }
        public Shell Shell { get; }

        public readonly BuildData Data;
        public readonly BuildEnvironment Environment;
        public readonly Essentials Essentials;
        public readonly ILFactory ILFactory;
        public readonly Utilities Utilities;
        public readonly CompilerPass Pass;

        // Frontend
        public readonly SourceReader Input;

        // Compiling
        public readonly NameResolver NameResolver;

        // Building
        public readonly BuildQueue Queue;
        public readonly BlockBuilder BlockBuilder;
        public readonly BundleBuilder BundleBuilder;
        public readonly TypeBuilder TypeBuilder;
        public readonly AstProcessor AstProcessor;
        public readonly UxlProcessor UxlProcessor;

        // Optimizing
        internal readonly ConstantFolder ConstantFolder;
        internal readonly ILStripper ILStripper;

        // Validation
        internal readonly ILVerifier ILVerifier;

        // Process
        readonly List<Task> _tasks = new List<Task>();
        readonly List<Pass> _generators = new List<Pass>();
        readonly List<Pass> _transforms = new List<Pass>();

        // ICompiler
        Log ICompiler.Log => Log;
        IBuildInput ICompiler.Input => Input;
        IBuildData ICompiler.Data => Data;
        IEnvironment ICompiler.Environment => Environment;
        IILFactory ICompiler.ILFactory => ILFactory;
        IUtilities ICompiler.Utilities => Utilities;
        IScheduler ICompiler.Scheduler => this;

        public Compiler(Log log, Backend backend, SourceBundle bundle, CompilerOptions options)
            : base(log)
        {
            // This is a block of dependency injection to initialize the Compiler
            var il = new Namespace();
            var extensions = new ExtensionRoot();
            Backend = backend;
            var disk = Disk = new Disk(log, true);
            Shell = new Shell(log);
            var essentials = Essentials = new Essentials();
            var resolver = NameResolver = new NameResolver(this);
            var ilf = ILFactory = new ILFactory(backend, il, essentials, resolver, this);
            var data = Data = new BuildData(il, extensions, ilf);
            var environment = Environment = new BuildEnvironment(backend, bundle, options, extensions, ilf, this);
            var input = Input = new SourceReader(log, bundle, environment);
            var queue = Queue = new BuildQueue(log, environment, backend, this);
            var blockBuilder = BlockBuilder = new BlockBuilder(backend, il, ilf, resolver, this, queue);
            var typeBuilder = TypeBuilder = new TypeBuilder(environment, ilf, resolver, this, queue);
            BundleBuilder = new BundleBuilder(backend, environment, ilf, this);
            AstProcessor = new AstProcessor(il, blockBuilder, typeBuilder, resolver, environment);
            UxlProcessor = new UxlProcessor(disk, backend.Name, il, extensions, environment, ilf);
            var pass = Pass = new CompilerPass(disk, data, environment, ilf, backend, input.Bundle, typeBuilder, resolver);
            Utilities = new Utilities(il, pass);
            ILVerifier = new ILVerifier(pass);
            ConstantFolder = new ConstantFolder(pass);
            ILStripper = new ILStripper(pass);
        }

#region Top-level building methods

        public void Load()
        {
            using (Log.StartProfiler(Input))
            {
                AstProcessor.AddRange(Input.ReadSourceFiles());
                UxlProcessor.AddRange(Input.ReadExtensionsFiles(Backend.Name));
            }

            if (Log.HasErrors)
                return;

            using (Log.StartProfiler(GetType()))
            {
                LoadCache();
                InitializeIL();
            }
        }

        public void Compile()
        {
            Progress(BuildStep.Compiling);
            Backend.Begin(this);

            foreach (var bundle in Input.Bundles)
                if (Backend.CanLink(bundle))
                    bundle.Flags |= SourceBundleFlags.CanLink;

            // Make sure all references of linkable bundles are also linkable
            foreach (var bundle in Input.Bundles)
            {
                if (bundle.CanLink)
                {
                    foreach (var reference in bundle.References)
                    {
                        if (!reference.CanLink)
                        {
                            Log.Verbose(bundle.Name + ": Removing CanLink flag because " + reference.Name + " can't link");
                            bundle.Flags &= ~SourceBundleFlags.CanLink;
                            break;
                        }
                    }
                }
            }

            using (Log.StartProfiler(Queue))
                Queue.BuildTypesAndFunctions();

            if (Log.HasErrors)
                return;

            using (Log.StartProfiler(UxlProcessor))
                UxlProcessor.CompileDocuments();

            if (Log.HasErrors)
                return;

            Backend.ShaderBackend.Initialize(this, BundleBuilder);

            using (Log.StartProfiler(Queue))
                Queue.BuildEverything();

            if (Log.HasErrors)
                return;

            Run(new ILAnalyzer(Pass));
            Run(ILVerifier);
        }

        public BackendResult Generate(Action<ICompiler> callback)
        {
            Progress(BuildStep.Generating);
            Backend.Configure();

            if (Environment.Options.TestOptions.HasValue)
                Run(new TestSetupTransform(Pass));

            if (!Environment.IsConsole && !Environment.IsLibrary)
                Data.ResolveMainClass(Pass, Environment);

            if (Environment.IsConsole && Environment.HasCustomEntrypoint)
                Data.SetEntrypoint(ILFactory.GetEntity(Environment.Options.MainClass + ".Main") as Method);

            if (Environment.IsConsole && Data.Entrypoint == null)
                Log.Error(Input.Bundle.Source, null, "No suitable entrypoint was found");

            Run(new ExtensionTransform(Pass));
            Run(new FixedArrayTransform(Pass));

            if (Backend.Has(FunctionOptions.ClosureConvert))
                Run(new ClosureConversionTransform(Pass));

            Run(_generators);
            BundleBuilder.Build();

            if (!Environment.IsConsole && !Environment.IsLibrary)
                Data.CreateEntrypoint();

            Run(ConstantFolder);

            Run(new ControlFlowVerifier(Pass));
            Run(new BackendTransform(Pass));
            Run(new MemberTransform(Pass));

            if (Backend.Has(TypeOptions.MakeUniqueNames))
                Run(new NameTransform(Pass));
            if (Backend.Has(FunctionOptions.MakeNativeCode))
                Run(new NativeTransform(Pass));

            if (Backend.Has(FunctionOptions.Analyze))
            {
                Run(new A1(Pass));
                Run(new A2(Pass));
            }

            Run(new IndirectionTransform(Pass));
            Run(new ExternTransform(Pass));

            UxlProcessor.CompileRequirements();
            ILStripper.Begin();
            Run(_transforms);

            using (Log.StartProfiler(ILStripper))
                ILStripper.End();

            UxlProcessor.FlattenExtensions();
            Run(ILVerifier);

            if (Log.HasErrors)
                return null;

            var buildProfiler = Log.StartProfiler(Backend.GetType());

            try
            {
                Backend.BeginBuild();
                return Backend.Build();
            }
            finally
            {
                Backend.EndBuild();
                buildProfiler.Dispose();
                UxlProcessor.WriteTypedFiles();
                callback(this);
                Environment.ClearPropertyCache();
                UxlProcessor.WriteUntypedFiles();
                Wait();
                Backend.End();
                StoreCache();
            }
        }

#endregion
#region Lower-level methods needed by Ninja etc

        public void ParseSourceFiles()
        {
            AstProcessor.AddRange(Input.ReadSourceFiles());
        }

        public void ParseSourceCode(SourceBundle bundle, string filename, string text)
        {
            var ast = new List<AstDocument>();
            new Parser(Log, bundle, filename, text).Parse(ast);
            AstProcessor.AddRange(ast);
        }

        public void InitializeIL()
        {
            UxlProcessor.CompileDefines();
            AstProcessor.Process();
            Essentials.Resolve(ILFactory);
        }

#endregion
#region IScheduler

        public void AddGenerator(Pass pass)
        {
            _generators.Add(pass);
        }

        public void AddTransform(Pass pass)
        {
            _transforms.Add(pass);
        }

#endregion
#region Task API

        public void StartAsync(Action work)
        {
            lock (_tasks)
                _tasks.Add(Task.Factory.StartNew(work));
        }

        public void Wait()
        {
            if (_tasks.Count == 0)
                return;

            Log.Verbose("Waiting for tasks to finish ...");
            Task[] taskArray;

            lock (_tasks)
            {
                taskArray = _tasks.ToArray();
                _tasks.Clear();
            }

            Task.WaitAll(taskArray);
        }

#endregion
#region Private utility methods

        void Progress(BuildStep step)
        {
            Environment.Step = step;
        }

        void Run(IEnumerable<Pass> passes)
        {
            foreach (var pass in passes)
                using (Log.StartProfiler(pass.GetType()))
                    pass.Run();
        }

        void Run(Pass pass)
        {
            using (Log.StartProfiler(pass.GetType()))
                pass.Run();
        }

#endregion
#region Shared IL cache

        void StoreCache()
        {
            if (!Environment.CanCacheIL)
                return;

            // Store IL cache for faster `uno doctor`
            foreach (var bundle in Input.Bundles)
                bundle.Cache[SourceBundle.ILKey] = Data.IL;
        }

        void LoadCache()
        {
            if (!Environment.CanCacheIL)
                return;

            foreach (var bundle in Input.Bundles)
            {
                Namespace il;
                if (!bundle.TryGetCache(SourceBundle.ILKey, out il))
                    continue;

                // Load IL cached in memory for faster `uno doctor`
                LoadNamespace(bundle, il, Data.IL);
            }

            // Clean NameResolver cache because we've introduced new stuff
            NameResolver.ClearCache();
        }

        void LoadNamespace(SourceBundle bundle, Namespace cache, Namespace parent)
        {
            if (!cache.Bundles.Contains(bundle))
                return;

            parent = GetNamespace(cache, parent);
            parent.Bundles.Add(bundle);

            foreach (var block in cache.Blocks)
            {
                if (block.Source.Bundle != bundle)
                    continue;

                block.SetParent(parent);
                parent.Blocks.Add(block);
            }

            foreach (var dt in cache.Types)
            {
                if (dt.Source.Bundle != bundle)
                    continue;

                dt.SetParent(parent);
                parent.Types.Add(dt);
            }

            foreach (var ns in cache.Namespaces)
                LoadNamespace(bundle, ns, parent);
        }

        Namespace GetNamespace(Namespace cache, Namespace parent)
        {
            if (cache.IsRoot)
                return parent;

            foreach (var ns in parent.Namespaces)
                if (ns.Name == cache.Name)
                    return ns;

            var result = new Namespace(parent, cache.Name);
            parent.Namespaces.Add(result);
            return result;
        }

#endregion
#region Global compiler factory for backends

        static Compiler()
        {
            CompilerFactory.Initialize(f => new BytecodeCompiler(f));
        }

#endregion
    }
}
