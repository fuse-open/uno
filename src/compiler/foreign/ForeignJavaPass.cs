using System.Collections.Generic;
using Uno.Compiler.API;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.CPlusPlus;

namespace Uno.Compiler.Foreign.Java
{
    partial class ForeignJavaPass : ForeignPass
    {		
        bool _initialized;
		readonly Dictionary<DataType, JavaClass> TopLevelJavaClasses = new Dictionary<DataType, JavaClass>();
        internal static DataType BoxedJavaObject;
        internal static DataType UnoToJavaBoxingClass;
		ExpandInterceptor ExpandInterceptor;
		Entrypoints BlockHost;
		public Converters.Converter Convert;

		protected override string Language => "Java";

        public ForeignJavaPass(CppBackend backend) : base(backend) {}

		void EnsureInitialized()
		{
			if (!_initialized)
			{
				ExpandInterceptor = new ExpandInterceptor(InterceptEntity);
				BoxedJavaObject = ILFactory.GetType("Java.Object");
				_initialized = true;
				Convert = new Converters.Converter(BoxedJavaObject, Essentials, ILFactory, Helpers);
				BlockHost = new Entrypoints(Environment, Disk, ILFactory, Convert);
				UnoToJavaBoxingClass = ILFactory.GetType("Uno.Compiler.ExportTargetInterop.Foreign.Android.JavaUnoObject");
			}
		}

		protected override void OnForeignFunction(Function f, List<string> annotations)
		{
			// Setup
			EnsureInitialized();
			Helpers.CacheContext(null, f.Source);

			var fm = new ForeignMethod(f, Essentials, Convert, Helpers, annotations);
			GenDelegatePlumbing(f);

			// Generate new java code
			var jcls = GetJavaClass(f.DeclaringType, Environment);
			jcls.AddForeignMethod(fm, ExpandInterceptor);

			// replace the extern body string with the jni calls
			Helpers.ReplaceBody(f, fm.GetCallFromUno());

			// add c++ headers to type
			Helpers.SourceInclude(fm.CppHeadersForDeclaringType, ((Method)f).DeclaringType);
		}

        public override void End()
        {
            EnsureInitialized();
			BlockHost.WriteJava();
			WriteJavaClasses();
			WriteDelegateTypeJavaFiles();
			BlockHost.WriteCpp(Helpers);
        }

		void WriteJavaClasses()
		{
			foreach (var jcls in TopLevelJavaClasses.Values)
				jcls.WriteJavaClass(Disk);
		}

		void WriteDelegateTypeJavaFiles()
        {
            foreach (var d in DelegatesSeen.Keys)
            {
                var filePath = Convert.Name.JavaDelegateName(d, true).Replace(".", "/") + ".java";
                var path = Environment.Combine(Environment.GetString("Java.SourceDirectory"), filePath);
                Disk.WriteAllText(path, DelegatesSeen[d]);
            }
        }

		// java classes
		JavaClass GetJavaClass(DataType dt, IEnvironment env)
		{
			if (!TopLevelJavaClasses.ContainsKey(dt))
			{
				if (dt.IsNestedType)
				{
					var parent = GetJavaClass(dt.ParentType, env);
					if (parent.StaticNestedClasses.ContainsKey(dt))
					{
						return parent.StaticNestedClasses[dt];
					}
					else
					{
						var newNested = new JavaClass(dt, Helpers, Convert, BlockHost, env);
						parent.StaticNestedClasses[dt] = newNested;
						return newNested;
					}
				}
				else
				{
					TopLevelJavaClasses[dt] = new JavaClass(dt, Helpers, Convert, BlockHost, env);
				}
			}
			return TopLevelJavaClasses[dt];
		}
    }
}
