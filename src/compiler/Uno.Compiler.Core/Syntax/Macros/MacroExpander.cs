using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Uno.Compiler.API;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Backends.Decompilers;
using Uno.Compiler.API.Domain.Extensions;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;
using Uno.Compiler.Core.IL.Optimizing;
using Uno.Compiler.Core.IL.Utilities;
using Uno.Compiler.Frontend.Preprocessor;
using Uno.IO;
using Uno.Logging;
using Uno.Macros;

namespace Uno.Compiler.Core.Syntax.Macros
{
    class MacroExpander : LogObject
    {
        const string Invalid = "0";

        readonly Backend _backend;
        readonly BuildEnvironment _env;
        readonly ExtensionRoot _extensions;
        readonly ILFactory _ilf;
        readonly Compiler _compiler;
        readonly Disk _disk;

        Decompiler Decompiler => _backend.Decompiler;
        ILStripper ILStripper => _compiler.ILStripper;

        public MacroExpander(
            Backend backend,
            BuildEnvironment env,
            ExtensionRoot extensions,
            ILFactory ilf,
            Compiler compiler)
            : base(compiler)
        {
            _backend = backend;
            _env = env;
            _extensions = extensions;
            _ilf = ilf;
            _compiler = compiler;
            _disk = compiler.Disk;
        }

        internal string ExpandMacros(Source src, string text, bool escape, Function context, Namescope[] usings)
        {
            return ExpandMacros(src, text, escape, new MacroContext(context, usings));
        }

        internal string ExpandMacrosAndDirectives(Source src, string text, bool escape, Function func, Namescope[] usings,
            ExpandInterceptor interceptor = null)
        {
            try
            {
                return Preprocessor.Process(
                    src,
                    text,
                    escape,
                    new MacroContext(func, usings, interceptor),
                    ExpandMacros,
                    Decompiler.AllowErrors);
            }
            catch (SourceException e)
            {
                throw new FatalException(e.Source, ErrorCode.E6100, e.Message, e);
            }
            catch (Exception e)
            {
                throw new FatalException(src, ErrorCode.E6101, e.Message, e);
            }
        }

        string ExpandMacros(Source src, string text, bool escape, object context)
        {
            // HACK: Disable package reference checking on Android, because they need to reference
            // members from higher-level packages.
            if (_env.IsDefined("ANDROID") && (
                    text.Contains(" JNICALL ") ||
                    text.EndsWith(":Include}")))
                src = Source.Unknown;

            text = MacroParser.Expand(src, text, escape, context, ExpandConfigMacro, "@(", ')');
            return MacroParser.Expand(src, text, escape, context, ExpandEntityMacro, "@{", '}');
        }

        string ExpandConfigMacro(Source src, string name, object context)
        {
            var calls = new List<MacroCall>();
            MacroParser.GetCalls(src, name, calls);
            return ExpandConfigMacro(src, calls, calls.Count - 1, context).String;
        }

        SourceValue ExpandConfigMacro(Source src, List<MacroCall> calls, int index, object context)
        {
            var call = calls[index];

            if (index == 0)
            {
                switch (call.Method)
                {
                    case null:
                    {
                        return ExpandConfigRoot(src, calls, index, context);
                    }
                    case "IsRequired":
                    {
                        return Bool(src, _extensions.Requirements.ContainsKey(call.Root));
                    }
                    case "IsSet":
                    {
                        SourceValue root;
                        return Bool(src, _env.TryGetValue(call.Root, out root, true) && !string.IsNullOrEmpty(root.String));
                    }
                    case "Defined":
                    {
                        return call.Arguments == null
                            ? Bool(src, _env.IsDefined(calls[0].Root))
                            : InvalidArgumentsError(src, call, "DEFINE", null);
                    }
                    case "Env":
                    {
                        return call.Arguments == null
                            ? Environment.ExpandEnvironmentVariables(Environment.GetEnvironmentVariable(calls[0].Root) ?? "")
                            : InvalidArgumentsError(src, call, "ENV_VAR", null);
                    }
                }
            }

            switch (call.Method)
            {
                case "Or":
                {
                    if (call.Arguments != null && call.Arguments.Count == 1)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return string.IsNullOrEmpty(root.String)
                            ? new SourceValue(src, GetArgumentOrNull(src, call, 0, context))
                            : root;
                    }

                    return InvalidArgumentsError(src, call, "VALUE", "defaultValue");
                }
                case "SplitAndJoin":
                {
                    if (call.Arguments != null && call.Arguments.Count <= 4)
                    {
                        var jsonArray = ExpandConfigRoot(src, calls, index, context);
                        var separator = GetArgumentOrNull(src, call, 0, context) ?? "\n";
                        var prefix = GetArgumentOrNull(src, call, 1, context);
                        var suffix = GetArgumentOrNull(src, call, 2, context);
                        var splitOn = GetArgumentOrNull(src, call, 3, context) ?? "\n";
                        var array = jsonArray.String.Split (new[] { splitOn }, StringSplitOptions.None);

                        if (prefix != null || suffix != null)
                            for (int i = 0; i < array.Length; i++)
                                array[i] = prefix + array[i] + suffix;

                        return new SourceValue(src, string.Join(separator, array));
                    }

                    return InvalidArgumentsError(src, call, "SET", "[separator]", "[prefix]", "[suffix]");
                }
                case "Join":
                {
                    if (call.Arguments != null && call.Arguments.Count <= 3)
                    {
                        var array = ExpandConfigArray(src, calls, index, context);
                        var separator = GetArgumentOrNull(src, call, 0, context) ?? "\n";
                        var prefix = GetArgumentOrNull(src, call, 1, context);
                        var suffix = GetArgumentOrNull(src, call, 2, context);

                        if (prefix != null || suffix != null)
                            for (int i = 0; i < array.Length; i++)
                                array[i] = prefix + array[i] + suffix;

                        return new SourceValue(src, string.Join(separator, array));
                    }

                    return InvalidArgumentsError(src, call, "SET", "[separator]", "[prefix]", "[suffix]");
                }
                case "JoinSorted":
                {
                    if (call.Arguments != null && call.Arguments.Count <= 3)
                    {
                        var array = ExpandConfigArray(src, calls, index, context);
                        var separator = GetArgumentOrNull(src, call, 0, context) ?? "\n";
                        var prefix = GetArgumentOrNull(src, call, 1, context);
                        var suffix = GetArgumentOrNull(src, call, 2, context);

                        if (prefix != null || suffix != null)
                            for (int i = 0; i < array.Length; i++)
                                array[i] = prefix + array[i] + suffix;

                        Array.Sort(array);
                        return new SourceValue(src, string.Join(separator, array));
                    }

                    return InvalidArgumentsError(src, call, "SET", "[separator]", "[prefix]", "[suffix]");
                }
                case "StringArray":
                {
                    if (call.Arguments == null)
                    {
                        var array = ExpandConfigArray(src, calls, index, context);

                        for (int i = 0; i < array.Length; i++)
                            array[i] = array[i].ToLiteral();

                        return new SourceValue(src, string.Join(", ", array));
                    }

                    return InvalidArgumentsError(src, call, "SET", null);
                }
                case "Equals":
                {
                    if (call.Arguments != null && call.Arguments.Count == 1)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context).String;
                        return Bool(src, root.Equals(GetArgumentOrNull(src, call, 0, context)));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", "otherValue");
                }
                case "Replace":
                {
                    if (call.Arguments != null && call.Arguments.Count == 2)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        var oldValue = GetArgumentOrNull(src, call, 0, context);
                        var newValue = GetArgumentOrNull(src, call, 1, context);
                        return new SourceValue(src, root.String.Replace(oldValue, newValue));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", "oldValue", "newValue");
                }
                case "Indent":
                {
                    if (call.Arguments != null && call.Arguments.Count == 2)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        var value = GetArgumentOrNull(src, call, 0, context);
                        var count = int.Parse(GetArgumentOrNull(src, call, 1, context) ?? Invalid);

                        var indent = "";
                        for (int i = 0; i < count; i++)
                            indent += value;

                        return new SourceValue(src, root.String.Trim().Replace("\n", "\n" + indent));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", "space", "count");
                }
                case "Trim":
                {
                    if (call.Arguments != null && call.Arguments.Count == 0)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(src, root.String.Trim());
                    }

                    return InvalidArgumentsError(src, call, "VALUE");
                }
                case "Path":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        var filename = root.String;
                        if (!string.IsNullOrEmpty(filename))
                            _disk.GetFullPath(root.Source, ref filename, PathFlags.AllowAbsolutePath | PathFlags.AllowNonExistingPath);
                        return new SourceValue(src, filename.NativeToUnix());
                    }

                    return InvalidArgumentsError(src, call, "PATH", null);
                }
                case "NativePath":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        var filename = root.String;
                        if (!string.IsNullOrEmpty(filename))
                            _disk.GetFullPath(root.Source, ref filename, PathFlags.AllowAbsolutePath | PathFlags.AllowNonExistingPath);
                        return new SourceValue(src, filename);
                    }

                    return InvalidArgumentsError(src, call, "PATH", null);
                }
                case "Base":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(src, Path.GetFileName(root.String));
                    }

                    return InvalidArgumentsError(src, call, "PATH", null);
                }
                case "EscapeCommand":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.EscapeCommand());
                    }

                    return InvalidArgumentsError(src, call, "COMMAND", null);
                }
                case "EscapeSpace":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.EscapeSpace());
                    }

                    return InvalidArgumentsError(src, call, "PATH", null);
                }
                case "EscapeXml":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, SecurityElement.Escape(root.String));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "QuoteSpace":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.QuoteSpace());
                    }

                    return InvalidArgumentsError(src, call, "PATH", null);
                }
                case "Identifier":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.ToIdentifier());
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "QIdentifier":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.ToIdentifier(true));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "ToLower":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.ToLowerInvariant());
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "ToUpper":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.String.ToUpperInvariant());
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "Bool":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, GetBool(src, root.String).ToLiteral());
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "Test":
                {
                    if (call.Arguments != null && call.Arguments.Count == 2)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(src, !string.IsNullOrEmpty(root.String) && GetBool(src, root.String) ?
                            GetArgumentOrNull(src, call, 0, context) :
                            GetArgumentOrNull(src, call, 1, context));
                    }

                    return InvalidArgumentsError(src, call, "VALUE", "whenTrue", "whenFalse");
                }
                case "Int":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);

                        int result;
                        if (int.TryParse(root.String, out result))
                            return new SourceValue(root.Source, result.ToLiteral());

                        Log.Error(src, ErrorCode.E0000, "Unable to parse int " + root.String.Quote());
                        return new SourceValue(root.Source, Invalid);
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "Number":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);

                        double result;
                        if (double.TryParse(root.String, out result))
                            return new SourceValue(root.Source, result.ToLiteral());

                        Log.Error(src, ErrorCode.E0000, "Unable to parse number " + root.String.Quote());
                        return new SourceValue(root.Source, Invalid);
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                case "String":
                {
                    if (call.Arguments == null)
                    {
                        var root = ExpandConfigRoot(src, calls, index, context);
                        return new SourceValue(root.Source, root.ToLiteral());
                    }

                    return InvalidArgumentsError(src, call, "VALUE", null);
                }
                default:
                {
                    var root = ExpandConfigRoot(src, calls, index, context);
                    Log.Error(src, ErrorCode.E0000, call.Quote() + " is not a valid macro");
                    return new SourceValue(root.Source, Invalid);
                }
            }
        }

        SourceValue ExpandConfigRoot(Source src, List<MacroCall> calls, int index, object context)
        {
            var root = calls[index].Root;

            switch (index)
            {
                case 0:
                {
                    if (MacroArguments.IsLiteral(root) || root.StartsWith('@'))
                        return new SourceValue(src, ExpandMacros(src, MacroArguments.Parse(root), true, context));

                    if (root == null || !_extensions.Properties.ContainsKey(root))
                    {
                        switch (root)
                        {
                            case "FILE":
                            case "FILENAME":
                                return new SourceValue(src, src.FullPath);
                            case "DIRNAME":
                                return new SourceValue(src, Path.GetDirectoryName(src.FullPath));
                            case "LINE":
                                return new SourceValue(src, src.Line.ToString());
                            case "PACKAGE":
                                return new SourceValue(src, src.Package.Name);
                            case "PACKAGE_VERSION":
                                return new SourceValue(src, src.Package.Version);
                            case "PACKAGE_DIR":
                                return new SourceValue(src, src.Package.SourceDirectory);
                            case "EXE_DIR":
                                return new SourceValue(src, Path.GetDirectoryName(typeof(MacroExpander).Assembly.Location));
                            case "MSG_ORIGIN":
                                return new SourceValue(src, "This file was generated based on " + src.File.ToString().Replace('\\', '/').Quote() + ".");
                            case "MSG_EDIT_WARNING":
                                return new SourceValue(src, "WARNING: Changes might be lost if you edit this file directly.");
                        }

                        //Log.Error(src, ErrorCode.E0000, root.Quote() + " is not a valid property for this backend.");
                        return new SourceValue(src, "");
                    }

                    SourceValue result;
                    return _env.TryGetValue(root, out result, true)
                        ? new SourceValue(result.Source, result.String ?? "")
                        : new SourceValue(src, "");
                }
                default:
                {
                    if (string.IsNullOrEmpty(root))
                        return ExpandConfigMacro(src, calls, index - 1, context);

                    Log.Error(src, ErrorCode.E0000, "'<base>." + root + "' is not valid in this context.");
                    return new SourceValue(src, Invalid);
                }
            }
        }

        string[] ExpandConfigArray(Source src, List<MacroCall> calls, int index, object context)
        {
            var root = calls[0].Root;
            if (root == null || !_extensions.ElementDefinitions.Contains(root))
            {
                SourceValue result;
                if (root != null && _env.TryGetValue(root, out result, true))
                    return (result.String ?? "").Split('\n');

                Log.Error(src, ErrorCode.E0000, root.Quote() + " is not a valid element for this backend.");
                return new string[0];
            }

            var array = _env.GetSet(root, true).ToArray();

            if (index > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    calls[0].Root = MacroArguments.GetLiteral(array[i]);
                    array[i] = ExpandConfigMacro(src, calls, index - 1, context).String;
                }

                calls[0].Root = root;
            }

            return array;
        }

        string GetArgumentOrNull(Source src, MacroCall macroCall, int index, object context)
        {
            return macroCall.Arguments.Count > index && macroCall.Arguments[index] != null
                ? ExpandMacros(src, MacroArguments.Parse(macroCall.Arguments[index]), true, context)
                : null;
        }

        bool GetBool(Source src, string root)
        {
            bool result;
            if (Preprocessor.TryParseCondition(root, out result))
                return result;

            Log.Error(src, ErrorCode.E0000, "Unable to parse bool " + root.Quote());
            return false;
        }

        string ExpandEntityMacro(Source src, string macro, object context_)
        {
            try
            {
                var calls = new List<MacroCall>();
                MacroParser.GetCalls(src, macro, calls);
                
                var context = (MacroContext)context_;
                var scopes = new Namescope[context.Usings.Length + 2];
                scopes[0] = DataType.Invalid;
                scopes[1] = context.Function != null ? context.Function.DeclaringType : DataType.Invalid;
                Array.Copy(context.Usings, 0, scopes, 2, context.Usings.Length);
                var root = _ilf.GetEntity(src, calls[0].Root, context.Usings);

                // See if generic parameter is flattened to specific type
                if (root is GenericParameterType)
                    root = GetGenericRoot(root as GenericParameterType, context);

                if (root.IsStripped && root is DataType)
                {
                    // Backend may not support generic types, see if flattened parameterization exists
                    var type = (DataType)root;

                    switch (type.Parent.NamescopeType)
                    {
                        case NamescopeType.DataType:
                        {
                            for (int i = 0; i < type.ParentType.NestedTypes.Count; i++)
                            {
                                if (type.ParentType.NestedTypes[i].Prototype == type.Prototype)
                                {
                                    root = type.ParentType.NestedTypes[i];
                                    break;
                                }
                            }
                            break;
                        }
                        case NamescopeType.Namespace:
                        {
                            for (int i = 0; i < type.ParentNamespace.Types.Count; i++)
                            {
                                if (type.ParentNamespace.Types[i].Prototype == type.Prototype)
                                {
                                    root = type.ParentNamespace.Types[i];
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }

                DataType dt = null;
                while (calls.Count > 1)
                {
                    var obj = ExpandEntityMacro_inner(src, calls[0], context, root, out dt);

                    if (dt == null)
                    {
                        Log.Error(src, ErrorCode.E6117, calls[0].Quote() + " cannot be chained because it does not return anything");
                        return Invalid;
                    }

                    calls.RemoveAt(0);
                    scopes[0] = dt;
                    root = !string.IsNullOrEmpty(calls[0].Root)
                        ? _ilf.GetEntity(src, calls[0].Root, scopes)
                        : dt;

                    var args = new List<string> { obj };

                    if (calls[0].Arguments != null)
                        args.AddRange(calls[0].Arguments);

                    calls[0].Arguments = MacroArguments.Concat(obj, calls[0].Arguments);
                }

                var result = ExpandEntityMacro(src, calls[0], context, root);

                if (context.Interceptor!=null)
                    result = context.Interceptor.InterceptEntity(macro, result, root, calls[0]);
                return result;
            }
            catch (Exception e)
            {
                Log.Error(src, ErrorCode.E6118, macro.Quote() + ": " + e.Message);
                return Invalid;
            }
        }

        string StringArgument(MacroContext context, DataType type, string str)
        {
            return (context.Interceptor != null) ? context.Interceptor.InterceptParameter(str, type) : str;
        }


        StringExpression StringExpressionArgument(MacroContext context, Source src, DataType type, string str)
        {
            return new StringExpression(
                src,
                type,
                StringArgument(context, type, str)
            );
        }

        string StringReturn(MacroContext context, DataType type, string str)
        {
            return context.Interceptor != null
                ? context.Interceptor.InterceptReturn(str, type)
                : str;
        }

        DataType GetGenericRoot(GenericParameterType gt, MacroContext context)
        {
            if (context.Function != null)
            {
                var m = context.Function.Prototype as Method;

                if (m != null && m.IsGenericParameterization)
                {
                    var def = m.GenericDefinition;

                    for (int i = 0; i < def.GenericParameters.Length; i++)
                        if (def.GenericParameters[i] == gt)
                            return m.GenericArguments[i];
                }

                var dt = (context.Function as Method)?.GenericType ?? context.Function.DeclaringType;

                do
                {
                    var proto = dt.Prototype;

                    if (proto.IsGenericParameterization)
                    {
                        var def = proto.GenericDefinition;

                        for (int i = 0; i < def.GenericParameters.Length; i++)
                            if (def.GenericParameters[i] == gt)
                                return proto.GenericArguments[i];
                    }
                } while ((dt = dt.ParentType) != null);
            }

            return gt;
        }

        string ExpandEntityMacro(Source src, MacroCall macroCall, MacroContext context, IEntity entity)
        {
            switch (macroCall.Method)
            {
                case "IsSealed":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsSealed).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsVirtual":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsVirtual).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsVirtualBase":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsVirtualBase).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsVirtualOverride":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsVirtualOverride).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsAbstract":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsAbstract).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsStatic":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsStatic).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "IsStripped":
                {
                    return macroCall.Arguments == null
                        ? Bool(src, entity.IsStripped).String
                        : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "Include":
                {
                    return macroCall.Arguments == null && entity is DataType
                        ? Have(src, macroCall, entity)
                            ? Decompiler.GetInclude(src, entity as DataType)
                            : Invalid
                        : InvalidArgumentsError(src, macroCall, "TYPE", null).String;
                }
                case "IncludeDirective":
                {
                    return macroCall.Arguments == null && entity is DataType
                        ? entity.IsStripped
                            ? "/* " + entity + " was stripped */"
                            : "#include <" + Decompiler.GetInclude(src, entity as DataType) + ">"
                        : InvalidArgumentsError(src, macroCall, "TYPE", null).String;
                }
                case "ForwardDeclaration":
                {
                    return macroCall.Arguments == null && entity is DataType
                        ? entity.IsStripped
                            ? "/* " + entity + " was stripped */"
                            : Decompiler.GetForwardDeclaration(src, entity as DataType)
                        : InvalidArgumentsError(src, macroCall, "TYPE", null).String;
                }
                case "Function":
                {
                    return macroCall.Arguments == null && entity is Function
                        ? Have(src, macroCall, entity)
                            ? Decompiler.GetFunction(src, context.Function, entity as Function)
                            : Invalid
                        : InvalidArgumentsError(src, macroCall, "FUNCTION", null).String;
                }
                default:
                {
                    DataType dt;
                    return ExpandEntityMacro_inner(src, macroCall, context, entity, out dt);
                }
            }
        }

        string ExpandEntityMacro_inner(Source src, MacroCall macroCall, MacroContext context, IEntity entity, out DataType dt)
        {
            dt = null;

            if (!Have(src, macroCall, entity) || entity is InvalidType)
                return Invalid;

            switch (macroCall.Method)
            {
                case null:
                {
                    return macroCall.Arguments == null || macroCall.Arguments.Count == 1
                            ? ExpandEntityImplicitGet(src, macroCall, context, entity, out dt)
                            : InvalidArgumentsError(src, macroCall, "ENTITY", null).String;
                }
                case "Of":
                {
                    dt = entity as DataType;
                    return macroCall.Arguments != null && macroCall.Arguments.Count == 1 && dt != null
                            ? macroCall.Arguments[0]
                            : InvalidArgumentsError(src, macroCall, "TYPE").String;
                }
                case "TypeOf":
                {
                    dt = _ilf.Essentials.Type;
                    return macroCall.Arguments == null && entity is DataType
                            ? Decompiler.GetTypeOf(src, context.Function, entity as DataType)
                            : InvalidArgumentsError(src, macroCall, "TYPE", null).String;
                }
                case "Array":
                {
                    return entity is DataType
                            ? ExpandEntityArray(src, macroCall, context, entity as DataType, out dt)
                            : InvalidArgumentsError(src, macroCall, "ELEMENT_TYPE", "[elements]").String;
                }
                case "New":
                {
                    return entity is ArrayType
                            ? ExpandEntityNew(src, macroCall, context, entity as ArrayType, out dt) :
                        entity is Constructor
                            ? ExpandEntityNew(src, macroCall, context, entity as Constructor, out dt) :
                        entity is Method && entity.UnoName == "New"
                            ? ExpandEntityCall(src, macroCall, context, entity as Method, out dt)
                            : InvalidArgumentsError(src, macroCall, "CONSTRUCTOR|ARRAY_TYPE", "[args]").String;
                }
                case "Call":
                {
                    if ((entity as Event)?.ImplicitField != null)
                        entity = (entity as Event).ImplicitField;

                    if (entity is Field && macroCall.Arguments != null)
                    {
                        // Convert field to delegate object
                        var obj = ExpandEntity(src, macroCall, context, entity, out dt);

                        if (entity.IsStatic)
                            macroCall.Arguments = MacroArguments.Concat(obj, macroCall.Arguments);
                        else if (macroCall.Arguments.Count > 0)
                            macroCall.Arguments[0] = obj;

                        entity = entity.ReturnType;
                    }

                    return entity is Method
                        ? ExpandEntityCall(src, macroCall, context, entity as Method, out dt) :
                        entity is DelegateType
                        ? ExpandEntityCall(src, macroCall, context, entity as DelegateType, out dt)
                            : InvalidArgumentsError(src, macroCall, "FUNCTION|DELEGATE", "[args]").String;
                }
                case "Get":
                {
                    return entity is Property
                            ? ExpandEntityGet(src, macroCall, context, entity as Property, out dt) :
                        (entity as Event)?.ImplicitField != null
                            ? ExpandEntityGet(src, macroCall, context, (entity as Event).ImplicitField, out dt) :
                        entity is Field
                            ? ExpandEntityGet(src, macroCall, context, entity as Field, out dt) :
                        entity is ArrayType
                            ? ExpandEntityGet(src, macroCall, context, entity as ArrayType, out dt)
                            : InvalidArgumentsError(src, macroCall, "FIELD|PROPERTY", "[args]").String;
                }
                case "Set":
                {
                    return entity is Property
                            ? ExpandEntitySet(src, macroCall, context, entity as Property, out dt) :
                        (entity as Event)?.ImplicitField != null
                            ? ExpandEntitySet(src, macroCall, context, (entity as Event).ImplicitField, out dt) :
                        entity is Field
                            ? ExpandEntitySet(src, macroCall, context, entity as Field, out dt) :
                        entity is ArrayType
                            ? ExpandEntitySet(src, macroCall, context, entity as ArrayType, out dt)
                            : InvalidArgumentsError(src, macroCall, "FIELD|PROPERTY", "[args]").String;
                }
                default:
                {
                    Log.Error(src, ErrorCode.E6116, macroCall.Quote() + " is not a valid macro");
                    return Invalid;
                }
            }
        }

        string ExpandEntity(Source src, MacroCall macroCall, MacroContext context, IEntity entity, out DataType dt)
        {
            switch (entity.EntityType)
            {
                case EntityType.Namescope:
                    switch (entity.NamescopeType)
                    {
                        case NamescopeType.DataType:
                            dt = (DataType) entity;
                            return Decompiler.GetType(src, context.Function, dt);
                        default:
                            dt = null;
                            break;
                    }
                    break;
                case EntityType.Member:
                    dt = ((Member) entity).ReturnType;
                    switch (entity.MemberType)
                    {
                        case MemberType.Literal:
                            return Decompiler.GetLiteral(src, context.Function, (Literal) entity);
                        case MemberType.Field:
                            return Decompiler.GetLoadField(src, context.Function, (Field) entity,
                                CompileObject(src, context, macroCall, (Member) entity));
                        case MemberType.Property:
                            if (((Property) entity).ImplicitField != null)
                                return Decompiler.GetLoadField(src, context.Function, ((Property) entity).ImplicitField,
                                    CompileObject(src, context, macroCall, (Member) entity));
                            break;
                        case MemberType.Event:
                            if (((Event) entity).ImplicitField != null)
                                return Decompiler.GetLoadField(src, context.Function, ((Event) entity).ImplicitField,
                                    CompileObject(src, context, macroCall, (Member) entity));
                            break;
                    }
                    break;
                default:
                    dt = null;
                    break;
            }

            return Decompiler.GetEntity(src, context.Function, entity);
        }

        string ExpandEntityImplicitGet(Source src, MacroCall macroCall, MacroContext context, IEntity entity, out DataType dt)
        {
            var result = ExpandEntity(src, macroCall, context, entity, out dt);
            // The interceptor (foreign code) doesn't care about type names or literals like enum values
            if (entity.EntityType == EntityType.Namescope ||
                entity.EntityType == EntityType.Member && entity.MemberType == MemberType.Literal)
                return result;
            return StringReturn(context, dt, result);
        }

        string ExpandEntityGet(Source src, MacroCall macroCall, MacroContext context, ArrayType type, out DataType dt)
        {
            dt = type.ElementType;
            return StringReturn(context, dt,
                Decompiler.GetLoadElement(src, context.Function, type.ElementType,
                    CompileObject(src, context, macroCall, type),
                    CompileIndex(src, context, macroCall, type)));
        }

        string ExpandEntitySet(Source src, MacroCall macroCall, MacroContext context, ArrayType type, out DataType dt)
        {
            dt = type.ElementType;
            return Decompiler.GetStoreElement(src, context.Function, type.ElementType,
                    CompileObject(src, context, macroCall, type),
                    CompileIndex(src, context, macroCall, type),
                    CompileValue(src, context, macroCall, type));
        }

        string ExpandEntityGet(Source src, MacroCall macroCall, MacroContext context, Field field, out DataType dt)
        {
            dt = field.ReturnType;
            return StringReturn(context, dt,
                Decompiler.GetLoadField(src, context.Function, field,
                    CompileObject(src, context, macroCall, field)));
        }

        string ExpandEntitySet(Source src, MacroCall macroCall, MacroContext context, Field field, out DataType dt)
        {
            dt = field.ReturnType;
            return Decompiler.GetStoreField(src, context.Function, field,
                    CompileObject(src, context, macroCall, field),
                    CompileValue(src, context, macroCall, field));
        }

        string ExpandEntityGet(Source src, MacroCall macroCall, MacroContext context, Property property, out DataType dt)
        {
            dt = property.ReturnType;
            return Have(src, macroCall, property, property.GetMethod)
                ? StringReturn(context, dt,
                    Decompiler.GetGetProperty(src, context.Function, property,
                        CompileObject(src, context, macroCall, property),
                        CompileArguments(src, context, macroCall, property)))
                : Invalid;
        }

        string ExpandEntitySet(Source src, MacroCall macroCall, MacroContext context, Property property, out DataType dt)
        {
            dt = property.ReturnType;
            return Have(src, macroCall, property, property.SetMethod)
                ? Decompiler.GetSetProperty(src, context.Function, property,
                    CompileObject(src, context, macroCall, property),
                    CompileArguments(src, context, macroCall, property),
                    CompileValue(src, context, macroCall, property))
                : Invalid;
        }

        string ExpandEntityCall(Source src, MacroCall macroCall, MacroContext context, Method method, out DataType dt)
        {
            dt = method.ReturnType;
            return StringReturn(context, dt,
                Decompiler.GetCallMethod(src, context.Function, method,
                    CompileObject(src, context, macroCall, method),
                    CompileArguments(src, context, macroCall, method)));
        }

        string ExpandEntityCall(Source src, MacroCall macroCall, MacroContext context, DelegateType type, out DataType dt)
        {
            dt = type.ReturnType;
            return StringReturn(context, dt,
                Decompiler.GetCallDelegate(src, context.Function,
                    CompileObject(src, context, macroCall, type),
                    CompileArguments(src, context, macroCall, type)));
        }

        string ExpandEntityArray(Source src, MacroCall macroCall, MacroContext context, DataType type, out DataType dt)
        {
            var arrayType = _compiler.TypeBuilder.GetArray(type);
            dt = arrayType;
            return StringReturn(context, dt,
                Decompiler.GetNewArray(src, context.Function, arrayType,
                    CompileElements(src, context, macroCall, type)));
        }

        string ExpandEntityNew(Source src, MacroCall macroCall, MacroContext context, ArrayType arrayType, out DataType dt)
        {
            dt = arrayType;
            return StringReturn(context, dt,
                Decompiler.GetNewArray(src, context.Function, arrayType,
                    CompileValue(src, context, macroCall, _ilf.Essentials.Int)));
        }

        string ExpandEntityNew(Source src, MacroCall macroCall, MacroContext context, Constructor ctor, out DataType dt)
        {
            dt = ctor.ReturnType;
            return StringReturn(context, dt,
                Decompiler.GetNewObject(src, context.Function, ctor,
                    CompileArguments(src, context, macroCall, ctor)));
        }

        Expression CompileValue(Source src, MacroContext context, MacroCall macroCall, Field field)
        {
            if (macroCall.Arguments != null && (
                    field.IsStatic && macroCall.Arguments.Count == 1 ||
                    !field.IsStatic && macroCall.Arguments.Count == 2
                ))
                return StringExpressionArgument(context, src, field.ReturnType, macroCall.Arguments[field.IsStatic ? 0 : 1]);

            InvalidArgumentsError(src, macroCall, field);
            return Expression.Invalid;
        }

        Expression CompileValue(Source src, MacroContext context, MacroCall macroCall, DataType type)
        {
            if (macroCall.Arguments != null && macroCall.Arguments.Count == 1)
                return StringExpressionArgument(context, src, type, macroCall.Arguments[0]);

            InvalidArgumentsError(src, macroCall, type);
            return Expression.Invalid;
        }

        Expression CompileIndex(Source src, MacroContext context, MacroCall macroCall, ArrayType type)
        {
            if (macroCall.Arguments != null && macroCall.Arguments.Count >= 2)
                return StringExpressionArgument(context, src, _ilf.Essentials.Int, macroCall.Arguments[1]);

            InvalidArgumentsError(src, macroCall, type);
            return Expression.Invalid;
        }

        Expression CompileValue(Source src, MacroContext context, MacroCall macroCall, ArrayType type)
        {
            if (macroCall.Arguments != null && macroCall.Arguments.Count == 3)
                return StringExpressionArgument(context, src, type.ElementType, macroCall.Arguments[2]);

            InvalidArgumentsError(src, macroCall, type);
            return Expression.Invalid;
        }

        Expression CompileValue(Source src, MacroContext context, MacroCall macroCall, IParametersEntity entity)
        {
            var offset = entity.IsStatic ? 0 : 1;

            if (macroCall.Arguments != null && macroCall.Arguments.Count - 1 == entity.Parameters.Length + offset)
                return StringExpressionArgument(context, src, entity.ReturnType, macroCall.Arguments[macroCall.Arguments.Count - 1]);

            InvalidArgumentsError(src, macroCall, entity);
            return Expression.Invalid;
        }

        Expression[] CompileElements(Source src, MacroContext context, MacroCall macroCall, DataType type)
        {
            if (macroCall.Arguments != null)
            {
                var args = new Expression[macroCall.Arguments.Count];
                for (int i = 0; i < args.Length; i++)
                    args[i] = StringExpressionArgument(context, src, type, macroCall.Arguments[i]);
                return args;
            }

            // Optional args on array initializer
            return Expressions.Empty;
        }

        Expression[] CompileArguments(Source src, MacroContext context, MacroCall macroCall, IParametersEntity entity)
        {
            var offset = entity.IsStatic ? 0 : 1;

            if (macroCall.Arguments != null && macroCall.Arguments.Count >= entity.Parameters.Length + offset)
            {
                var args = new Expression[entity.Parameters.Length];
                for (int i = 0; i < args.Length; i++)
                    args[i] = StringExpressionArgument(context, src, entity.Parameters[i].Type, macroCall.Arguments[i + offset]);
                return args;
            }

            InvalidArgumentsError(src, macroCall, entity);
            return Expressions.Empty;
        }

        Expression CompileObject(Source src, MacroContext context, MacroCall macroCall, Member member)
        {
            return member.IsStatic
                ? null
                : CompileObject(src, context, macroCall, member.DeclaringType);
        }

        Expression CompileObject(Source src, MacroContext context, MacroCall macroCall, DataType type)
        {
            return macroCall.Arguments != null && macroCall.Arguments.Count > 0
                    ? StringExpressionArgument(context, src, type, macroCall.Arguments[0]).Address
                    : InvalidArgumentsError(src, macroCall, type);
        }

        Expression InvalidArgumentsError(Source src, MacroCall macroCall, IEntity root)
        {
            var args = new List<string>();
            if (!root.IsStatic && !(root is DataType))
                args.Add("object");
            if (root is ParametersMember || root is DelegateType)
                foreach (var p in ((IParametersEntity) root).Parameters)
                    args.Add(p.Name);
            if (root is ArrayType)
                args.Add("index");
            if (macroCall.Method == "Set")
                args.Add("value");

            InvalidArgumentsError(src, macroCall, root.FullName, args.ToArray());
            return Expression.Invalid;
        }

        SourceValue InvalidArgumentsError(Source src, MacroCall macroCall, string root, params string[] args)
        {
            macroCall.Root = root;
            macroCall.Arguments = new List<string>(args);
            Log.Error(src, ErrorCode.E0000, macroCall.Quote() + " has some invalid arguments");
            return new SourceValue(src, Invalid);
        }

        SourceValue Bool(Source src, bool value)
        {
            return new SourceValue(src, value ? "1" : "0");
        }

        bool Have(Source src, MacroCall macroCall, IParametersEntity member, Method accessor)
        {
            if (accessor == null || accessor.IsStripped)
            {
                macroCall.Arguments = null;
                macroCall.Root = member.FullName;
                Log.Error(src, ErrorCode.E0000, macroCall.Quote() + " cannot be expanded because the accessor has been stripped");
                return false;
            }

            ILStripper.Keep(accessor);
            return true;
        }

        bool Have(Source src, MacroCall macroCall, IEntity entity)
        {
            if (entity == null || entity.IsStripped)
            {
                macroCall.Arguments = null;
                macroCall.Root = entity == null ? "<null>" : entity.FullName;
                Log.Error(src, ErrorCode.E0000, macroCall.Quote() + " cannot be expanded because the entity has been stripped");
                return false;
            }

            ILStripper.Keep(entity);
            return true;
        }
    }
}
