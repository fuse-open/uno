using System;

namespace Uno.Compiler.API
{
    public sealed class ExpandInterceptor
    {
        readonly Func<string,string,object,Macros.MacroCall,string> _interceptEntity;
        readonly Func<string, object, string> _interceptParam;
        readonly Func<string, object, string> _interceptReturn;

		public ExpandInterceptor(Func<string,string,object,Macros.MacroCall,string> interceptEntity = null,
            Func<string, object, string> interceptParam = null,
            Func<string, object, string> interceptReturn = null)
        {
            _interceptEntity = interceptEntity;
            _interceptParam = interceptParam;
            _interceptReturn = interceptReturn;
        }

		public string InterceptEntity(string macroText, string expansionResult, object context, Macros.MacroCall macroCall)
        {
            if (_interceptEntity != null)
                return _interceptEntity(macroText, expansionResult, context, macroCall);
            else
                return expansionResult;
        }

        public string InterceptParameter(string expansionResult, object context)
        {
            if (_interceptParam != null)
                return _interceptParam(expansionResult, context);
            else
                return expansionResult;
        }

        public string InterceptReturn(string expansionResult, object context)
        {
            if (_interceptReturn != null)
                return _interceptReturn(expansionResult, context);
            else
                return expansionResult;
        }
    }
}

