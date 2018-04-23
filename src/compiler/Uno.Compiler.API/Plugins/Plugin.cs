using System;
using System.IO;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API.Plugins
{
    public abstract class Plugin
    {
        public abstract string Class { get; }

        protected IILFactory ILFactory { get; private set; }
        protected IBundle Bundle { get; private set; }
        protected Log Log { get; private set; }
        protected string CacheDirectory { get; private set; }

        protected string GetCacheName(string arg)
        {
            if (CacheDirectory == null)
                throw new InvalidOperationException("Plugin is not initialized");

            return Path.Combine(CacheDirectory, arg.GetNormalizedBasename());
        }

        public void Initialize(
            Log log, 
            IBundle bundle, 
            IILFactory ilf)
        {
            Log = log;
            Bundle = bundle;
            ILFactory = ilf;
            CacheDirectory = Path.Combine(
                                Bundle.Directory, 
                                GetType().Name.ToIdentifier().ToLowerInvariant());
            Initialize();
        }

        protected virtual void Initialize()
        {
        }
    }
}