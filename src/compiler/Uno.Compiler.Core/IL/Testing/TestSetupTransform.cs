using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Uno.Compiler.API.Domain;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Core.IL.Testing
{
    internal class TestSetupTransform : CompilerPass
    {
        private readonly List<Method> _methods = new List<Method>();

        private readonly Source _source;
        private readonly DataType _testAttributeType;
        private readonly DataType _ignoreAttributeType;
        private readonly DataType _testSetupType;
        private readonly DelegateType _actionType;
        private readonly DataType _testRegistryType;
        private readonly DataType _mainClass;
        private readonly Constructor _mainConstructor;
        private readonly Field _testSetupField;
        private TestOptions _testOptions;
        private readonly DataType _appClass;

        public TestSetupTransform(CompilerPass parent, bool headless)
            : base(parent)
        {
            if (Environment.Options.TestOptions == null)
                throw new InvalidOperationException("Internal error, could not get test options");

            _testOptions = Environment.Options.TestOptions.Value;
            _testAttributeType = ILFactory.GetType("Uno.Testing.TestAttribute");
            _ignoreAttributeType = ILFactory.GetType("Uno.Testing.IgnoreAttribute");

            if (_testAttributeType is InvalidType)
                throw new InvalidOperationException("Did you forget to reference the Uno.Testing package?");

            _actionType = (DelegateType)ILFactory.GetType("Uno.Action");
            _testSetupType = ILFactory.GetType("Uno.Testing.TestSetup");

            _appClass = headless ? ILFactory.GetType("Uno.Testing.TestingApp") : Essentials.Application;

            _source = Package.Source;
            _testRegistryType = ILFactory.GetType("Uno.Testing.Registry");
            _mainClass = new ClassType(_source, Data.IL, null, Modifiers.Generated | Modifiers.Public, "MainClass");
            _mainClass.SetBase(_appClass);
            _mainConstructor = new Constructor(_source, _mainClass, null, Modifiers.Public, ParameterList.Empty);
            _mainClass.Constructors.Add(_mainConstructor);

            _testSetupField = new Field(_source, _mainClass, "_testSetup", null, Modifiers.Private, FieldModifiers.ReadOnly, _testSetupType);
            _mainClass.Fields.Add(_testSetupField);

            Data.IL.Types.Add(_mainClass);
        }

        public override bool Begin(DataType dt)
        {
            foreach (var method in dt.Methods)
            {
                if (method.HasAttribute(_testAttributeType) && Environment.CanExport(method))
                    _methods.Add(method);
            }
            return true;
        }

        public override void End()
        {
            CreateTestSetUpClass();
        }

        public void CreateTestSetUpClass()
        {
            try
            {
                RegisterTestMethods();
                Data.SetMainClass(_mainClass);
            }
            catch (SourceException e)
            {
                Log.Error(e.Source, ErrorCode.E0000, e.ToString());
            }
        }

        Method CreateInvoker(Method method)
        {
            var invokeMethod = new Method(_source,
                                          _mainClass,
                                          null,
                                          Modifiers.Generated | Modifiers.Public | Modifiers.Static,
                                          InvokeName(method),
                                          DataType.Void,
                                          ParameterList.Empty);
            var testVar = new Variable(_source, invokeMethod, "obj", method.DeclaringType,
                                       VariableType.Default, InstantiateTestFixture(method));
            invokeMethod.SetBody(new Scope(_source, 
                new VariableDeclaration(testVar),
                new CallMethod(_source, new LoadLocal(_source, testVar), method)));

            var dispose = method.DeclaringType.TryGetMethod("Dispose", true);
            if (dispose != null)
                invokeMethod.Body.Statements.Add(new CallMethod(_source, new LoadLocal(_source, testVar), dispose));

            _mainClass.Methods.Add(invokeMethod);
            return invokeMethod;
        }

        private Expression InstantiateTestFixture(Method method)
        {
            if (method.IsStatic)
                return null;

            var ctor = (Constructor)method.DeclaringType.TryGetDefaultConstructor();
            return ctor != null ? new NewObject(_source, ctor, Expressions.Empty) : null;
        }

        bool ShouldIgnoreForThisPlatform(Method method, Expression[] args)
        {
            var condition = args.Length > 1 ? args[1].ToString().Trim('"') : null;
            if (string.IsNullOrEmpty(condition))
                return true;
            return Environment.Test(method.Source, condition);
        }

        public static string TestName(Method method)
        {
            return method.FullName;
        }

        public TestMethod CreateTestMethod(Method method)
        {
            var name = TestName(method);

            var ignoreAttribs = method.Attributes.Where(x => x.ReturnType == _ignoreAttributeType).ToList();
            if (ignoreAttribs.Count == 0)
                return new TestMethod(name, false, "");

            if (ignoreAttribs.Count == 1)
            {
                var args = ignoreAttribs[0].Arguments;
                var ignoreReason = args.Length > 0 ? args[0].ToString().Trim('"') : null;
                bool ignoreOnPlatform = ShouldIgnoreForThisPlatform(method, args);
                return new TestMethod(name, ignoreOnPlatform, ignoreReason);
            }

            throw new SourceException(method.Source, "Can not specify more than one Ignore attribute.");
        }

        private void RegisterTestMethods()
        {
            var registryObject = ILFactory.NewObject(_source, _testRegistryType);
            var registryVariable = new Variable(_source, _mainConstructor, "registry", _testRegistryType, VariableType.Default, registryObject);
            var body = new Scope(_source);
            body.Statements.Add(new CallConstructor(_source, _appClass.Constructors[0]));
            body.Statements.Add(new VariableDeclaration(registryVariable));

            var methods = _methods;
            if (_testOptions.Filter != null)
            {
                var regex = new Regex(_testOptions.Filter, RegexOptions.CultureInvariant);
                methods = methods.Where(m => regex.IsMatch(m.FullName)).ToList();
            }

            methods.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.InvariantCultureIgnoreCase));

            var ret = new List<TestMethod>();
            foreach (var method in methods)
            {
                var testMethod = CreateTestMethod(method);
                var testExpression = RegisterTest(testMethod,
                             new LoadLocal(_source, registryVariable),
                             new NewDelegate(_source, _actionType, null, CreateInvoker(method)));

                body.Statements.Add(testExpression);
                ret.Add(testMethod);
            }

            var registryExpr = new LoadLocal(_source, registryVariable);
            var testSetupConstruction = ILFactory.NewObject(_source, _testSetupType, registryExpr, new Constant(_source, Essentials.String, _testOptions.TestServerUrl));

            body.Statements.Add(new StoreField(_source, new This(_source, _mainClass), _testSetupField, testSetupConstruction));

            _mainConstructor.SetBody(body);
        }

        private Expression RegisterTest(TestMethod testMethod, Expression registryArgument, NewDelegate invokeDelegate)
        {
            var testNameExpr = new Constant(_source, Essentials.String, testMethod.Name);
            var ignoreExpr = new Constant(_source, Essentials.Bool, testMethod.Ignored);
            var ignoreReasonExpr = new Constant(_source, Essentials.String, testMethod.IgnoreReason);
            return ILFactory.CallMethod(_source, registryArgument, "Add", invokeDelegate,
                                        testNameExpr, ignoreExpr, ignoreReasonExpr);
        }

        private static string InvokeName(Method method)
        {
            return "Invoke___" + method.FullName.Replace('.', '_').ToIdentifier();
        }
    }
}
