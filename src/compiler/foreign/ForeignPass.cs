using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.CPlusPlus;
using System.Collections.Generic;

namespace Uno.Compiler.Foreign
{
	abstract class ForeignPass : Pass
	{
		readonly protected ForeignHelpers Helpers;

		protected readonly CppBackend Backend;

		protected ForeignPass(CppBackend backend) : base(backend)
		{
			Backend = backend;
			Helpers = new ForeignHelpers(Environment, Essentials, backend, Data);
		}

		public override bool Begin(Function f)
		{
			var langString = Helpers.GetForeignAttribute(f);
			if (langString != null && Language == langString)
				OnForeignFunction(f, Helpers.GetForeignAnnotations(f, langString));
			return false;
		}

		protected abstract string Language { get; }

		protected abstract void OnForeignFunction(Function f, List<string> annotations);
	}
}
