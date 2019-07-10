using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Uno.IO;
using Uno.UX.Markup.Reflection;

namespace Uno.UX.Markup.UXIL
{
    public class UXPropertyClass
    {
        public Property Property { get; }
        public Node Owner { get; private set; }
        public IDataType UsageType { get; }

        public UXPropertyClass(Property prop, Node owner)
        {
            Property = prop;
            Owner = owner;
            UsageType = owner.DeclaredType;
        }

        public override int GetHashCode()
        {
            return Property.GetHashCode() ^ UsageType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var u = obj as UXPropertyClass;
            if (u == null) return false;
            return Property.Equals(u.Property) && UsageType.Equals(u.UsageType);
        }
    }

    public class Document
    {
        public List<Node> NodesInDocumentOrder { get; }

        internal Document(List<Node> nodesInDocumentOrder)
        {
            NodesInDocumentOrder = nodesInDocumentOrder;
        }
    }

    public class Project
    {
        public Dictionary<string, Document> Documents { get; private set; }
        public List<ClassNode> RootClasses { get; private set; }

        public string ProjectName { get; private set; }
        public string GeneratedPath { get; private set; }

        public IEnumerable<UXPropertyAccessorSource> UXPropertyAccessors { get; private set; }
        public IEnumerable<UXPropertyClass> UXProperties { get; private set; }

        public IEnumerable<KeyValuePair<string, List<Node>>> GlobalResources { get; private set; }

        internal Project(string projectName, string generatedPath, 
            Dictionary<string, Document> documents,
            List<ClassNode> rootClasses, 
            IEnumerable<UXPropertyAccessorSource> uxPropertyAccessors,
            IEnumerable<UXPropertyClass> uxProperties,
            IEnumerable<KeyValuePair<string, List<Node>>> globalResources)
        {
            Documents = documents;
            ProjectName = projectName;
            GeneratedPath = generatedPath;
            RootClasses = rootClasses;
            UXPropertyAccessors = uxPropertyAccessors;
            UXProperties = uxProperties;
            GlobalResources = globalResources;
        }
    }

    public partial class Compiler
    {
        readonly Dictionary<object, UXIL.Node> _nodes = new Dictionary<object, UXIL.Node>();

        internal void AddNode(object handle, Node n)
        {
            _nodes.Add(handle, n);
        }

        readonly IDataTypeProvider _dataTypeProvider;
        readonly GlobalResourceCache _globalResourceCache;
        readonly Common.IMarkupErrorLog _log;

        IDataType TemplateType => _dataTypeProvider.TryGetTypeByName("Uno.UX.Template");

        IDataType Float4 => _dataTypeProvider.TryGetTypeByName("float4");

        public class UXSource
        {
            public readonly string FilePath;
            public readonly string OptionalContent;

            public UXSource(string filePath, string optionalContent = null)
            {
                FilePath = filePath;
                OptionalContent = optionalContent;
            }
        }

        public static Project Compile(IDataTypeProvider dataTypeProvider, IEnumerable<UXSource> uxPaths, string projectDirectory, string projectName, string generatedPath, Common.IMarkupErrorLog log)
        {
            Dictionary<string, XDocument> xdocs = new Dictionary<string, XDocument>();

            var roots = new Dictionary<XDocument, AST.Element>();

            foreach (var p in uxPaths)
            {
                System.IO.Stream stream;
                if (p.OptionalContent != null) stream = new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(p.OptionalContent));
                else stream = File.OpenRead(p.FilePath);

                try
                {
                    var xdoc = XmlHelpers.ReadAllXml(stream, LoadOptions.SetLineInfo | LoadOptions.PreserveWhitespace, true);

                    var ast = AST.Parser.Parse(projectName, p.FilePath, xdoc, log);
                    if (ast.Generator is AST.UnspecifiedGenerator)
                        continue; // Skip documents/roots without generators

                    xdocs.Add(p.FilePath, xdoc);

                    if (ast.Generator is AST.TemplateGenerator)
                        continue;

                    var cg = ast.Generator as AST.ClassGenerator;
                    if (cg != null && cg.IsInnerClass)
                        continue;

                    roots.Add(xdoc, ast);
                }
                catch (System.Xml.XmlException e)
                {
                    log.ReportError(p.FilePath, e.LineNumber, e.Message);
                }
                finally
                {
                    stream.Dispose();
                }
            }

            var root = new AST.Element(null, null, null, null, AST.ElementType.Object, null, null, null, null, null, null, null, roots.Values.ToArray(), false, new FileSourceInfo(projectName, 0));

            var comp = new Compiler(dataTypeProvider, projectDirectory, root, log);

            var docs = xdocs.Where(kvp => roots.ContainsKey(kvp.Value)).ToDictionary(kvp => kvp.Key, kvp =>
            {
                var xdoc = kvp.Value;
                var astRoot = roots[xdoc];

                var nodesInDocumentOrder = astRoot
                    .DescendantNodesAndSelf()
                    .OfType<AST.Element>()
                    .Select(astNode => comp._astNodeToNode[astNode])
                    .ToList();

                return new Document(nodesInDocumentOrder);
            });

            return new Project(projectName, generatedPath, docs, comp._rootClasses, comp._uxPropertyAccessors, comp._uxProperties, comp._globalResourceCache.Handles);
        }

        internal static Compiler CreateTest(IDataTypeProvider il, string projectDirectory, string projectName, string generatedPath, Common.IMarkupErrorLog log)
        {
            var root = new AST.Element(null, null, null, null, AST.ElementType.Object, null, null, null, null, null, null, null, new AST.Element[0], false, new FileSourceInfo(projectName, 0));

            return new Compiler(il, projectDirectory, root, log);
        }

        public IEnumerable<IDataType> Types {  get { return _dataTypeProvider.DataTypes; } }

        IDataType[] _unaryOps;
        public IEnumerable<IDataType> UnaryOperators
        {
            get
            {
                return _unaryOps ?? (_unaryOps = Types.Where(x => x.UXUnaryOperatorName != null).ToArray());
            }
        }

        readonly Dictionary<string, IDataType> _uxFunctions = new Dictionary<string, IDataType>();

        readonly string _projectDirectory;

        readonly Dictionary<AST.Node, Node> _astNodeToNode = new Dictionary<AST.Node, Node>();

        Compiler(IDataTypeProvider dataTypeProvider, string projectDirectory, AST.Element astRoot, Common.IMarkupErrorLog log)
        {
            _projectDirectory = projectDirectory;
            _log = log;
            if (astRoot == null) return;

            _dataTypeProvider = dataTypeProvider;
            _globalResourceCache = new GlobalResourceCache(_dataTypeProvider);

            foreach (var f in _dataTypeProvider.DataTypes.Where(x => x.UXFunctionName != null))
            {
                if (_uxFunctions.ContainsKey(f.UXFunctionName))
                {
                    ReportError(astRoot.Children.First().Source, "Multiple implementations of the UX function '" + f.UXFunctionName + "' found in project: " + f.FullName + " and " + _uxFunctions[f.UXFunctionName].FullName + ". Please delete or rename one of them.");
                    return;
                }

                _uxFunctions.Add(f.UXFunctionName, f);
            }

            foreach (var c in astRoot.Children.OfType<AST.Element>())
                FindInnerClasses(c);

            CreateInnerClassNodes();

            //foreach (var e in astRoot.Children.OfType<AST.Element>())
            //    TransformBindingExpressions(e);

            if (_errorCount > 0) return;

            foreach (var r in astRoot.Children)
                CreateNodes(null, r, ContentMode.Default, InstanceType.Local);

            foreach (var n in _nodes.Values.OfType<DocumentScope>())
                n.VerifyNamesUnique(this);

            foreach (var n in _nodes)
            {
                n.Value.CreateMembers();
            }

            ResolveContentStrings();

            ResolveNameProperties();

            TransformShorthandTargetValueSyntax();

            //PrepareValueBindings();

            ResolveDeferredGenericTypes();

            //ResolveValueBindings();

            ResolveProperties();
            foreach (var kvp in _nodes)
            {
                ResolveBindings(kvp.Value);
                var e = kvp.Key as AST.Element;
                if (e != null)
                    ResolveElementBindings(e, kvp.Value);
            }

            ResolveAutoProperties();

            ResolveAuxNameTables();

            ResolveGlobalReferenceValues();

            FindAndRegisterGlobalModules();

            foreach (var n in _rootClasses)
                foreach (var k in n.DocumentScopes)
                    k.OrderInstantiations(this);

            VerifyClassesUnique();

            VerifyClassesUsedInValidScopes();

            VerifyConstructorArguments();
        }

        void ResolveContentStrings()
        {
            foreach (var n in _nodes.Values.Where(x => x.ContentString != null).ToArray())
            {
                var cp = n.AtomicProperties.FirstOrDefault(x => x.Facet.AutoBindingType == AutoBindingType.Content);

                if (cp == null)
                {
                    ReportError(n.Source, "Node of type '" + n.DeclaredType + "' does not support string content");
                    continue;
                }

                if (cp.Facet.IsUXVerbatim)
                {
                    cp.Value = new String(n.ContentString, n.Source);
                }
                else
                {
                    ParseAttribute(n, cp, cp, n.ContentString.Trim(), n.Source);
                }
            }
        }

        void TransformShorthandTargetValueSyntax()
        {
            _nodes.ToArray().Where(x => x.Key is AST.Element).ToList().ForEach(x => TransformShorthandTargetValueSyntax(x.Value, (AST.Element)x.Key));
        }

        void TransformShorthandTargetValueSyntax(Node n, AST.Element e)
        {
            // Transform <Change foo.bar="123" to <Change Target="foo.bar" Value="123"
            foreach (var p in e.Properties.ToArray())
            {
                if (n.ResultingType is DeferredGenericType)
                {
                    var dgt = n.ResultingType as DeferredGenericType;
                    if (dgt.ParameterProperty == p.Name)
                    {
                        e.RemoveProperty(p);
                        e.AddProperty(new AST.Property("Target", p.Name, p.Source));
                        e.AddProperty(new AST.Property("Value", p.Value, p.Source));
                    }
                }
            }
        }

        void VerifyConstructorArguments()
        {
            foreach (var n in _nodes.Values.Where(x => x.InstanceType == InstanceType.Local || x.InstanceType == InstanceType.Global))
            {
                if (n is DependencyNode) continue;
                if (n is PropertyNode) continue;

                foreach (var p in n.Properties.Where(x => 
                    x.Facet.IsConstructorArgument 
                    && x.Facet.DeclaringType == n.DeclaredType
                    && !x.HasValue
                    && !(n is ClassNode && x.Facet is DeclaredUXDependency && ((ClassNode)n).DeclaredDependencies.Any(y => y.Name == x.Facet.Name))))
                {
                    var ap = p as AtomicProperty;
                    var ca = p.Facet as IConstructorArgument;
                    if (ca.DefaultValue != null)
                        ap.Value = Parse(ca.DefaultValue, p.Facet.DataType, n.Source);
                    else
                        ReportError(n.Source, "'" + n.DeclaredType.FullName + "' must specify required attribute '" + p.Facet.Name + "'");
                }
            }
        }

        void FindAndRegisterGlobalModules()
        {
            var appClass = _rootClasses.Where(x => x.IsAppClass);
            var testClasses = _rootClasses.Where(x => x.IsTest);
            var rootClasses = appClass.Union(testClasses);

            foreach (var cl in _dataTypeProvider.DataTypes.Where(x => x.IsGlobalModule))
            {
                var module = new NewObjectNode(FileSourceInfo.Unknown, cl.FullName, cl, InstanceType.Local);

                foreach (var rootClass in rootClasses)
                {
                    rootClass.AddChild(module);
                    rootClass.ScheduleInstantiation(module);
                }
            }
        }

        void ResolveGlobalReferenceValues()
        {
            foreach (var k in _nodes)
            {
                var bvn = k.Value as BoxedValueNode;
                if (bvn != null)
                {
                    var grv = bvn.Value as GlobalReferenceValue;
                    if (grv != null)
                    {
                        ResolveGlobalReference(k.Value, grv, new Stack<GlobalReferenceValue>());
                    }
                    continue;
                }

                foreach (var p in k.Value.AtomicProperties.Select(x => x.Value).OfType<GlobalReferenceValue>())
                {
                    ResolveGlobalReference(k.Value, p, new Stack<GlobalReferenceValue>());
                }
            }
        }

        void ResolveGlobalReference(Node owner, GlobalReferenceValue p, Stack<GlobalReferenceValue> stack)
        {
            if (stack.Contains(p))
            {
                stack.Push(p);
                ReportError(owner.Source, "Circular global reference detected: " + stack.Select(x => x.Identifier).Aggregate((x, y) => x + " -> " + y));
                return;
            }

            Predicate<IDataType> acceptFunc = x => x.Implements(p.DataType);
            var n = ResolveNode(p.Source, owner, p.Identifier, p.DataType, acceptFunc, "Unable to convert '" + p.Identifier + "' to the required type '" + p.DataType.FullName + "'");

            if (n is ResourceRefNode)
            {
                p.ResolvedPath = ((ResourceRefNode)n).StaticRefId;
            }
            else if (n is BoxedValueNode)
            {
                var v = (n as BoxedValueNode).Value;

                var grv = v as GlobalReferenceValue;
                if (grv != null)
                {
                    stack.Push(p);
                    ResolveGlobalReference(owner, grv, stack);
                    stack.Pop();
                    v = grv.ResolvedValue;
                }
                 
                p.ResolvedValue = v;
            }
        }

        readonly List<ClassNode> _rootClasses = new List<ClassNode>();

        string RemoveCommonLeadString(string s, string other)
        {
            int i = 0;
            for (; i < Math.Min(s.Length, other.Length); i++)
            {
                if (s[i] != other[i]) break;
            }
            return s.Substring(i);
        }

        void VerifyClassesUnique()
        {
            var classNames = new Dictionary<string, ClassNode>();
            foreach (var c in _rootClasses)
            {
                if (classNames.ContainsKey(c.GeneratedClassName.FullName))
                {
                    var x = classNames[c.GeneratedClassName.FullName];
                    var path = RemoveCommonLeadString(x.Source.FileName, c.Source.FileName);
                    if (path.Length == 0) path = "on line " + x.Source.LineNumber;
                    else path = " in " + path + " (line " + c.Source.LineNumber + ")";
                    ReportError(c.Source, "Multiple definitions of class '" + c.GeneratedClassName + "' in the project. There is another one " + path);
                    continue;

                }
                classNames.Add(c.GeneratedClassName.FullName, c);
            }
        }

        void VerifyClassesUsedInValidScopes()
        {
            foreach (var n in _nodes.Values.Where(x => x.DeclaredType.IsInnerClass))
            {
                var cr = (ClassNode)n.DeclaredType;
                var scope = cr.ParentScope;

                var k = n.Scope;
                bool scopeOk = false;
                while (k != null)
                {
                    if (k == scope)
                    {
                        scopeOk = true;
                        break;
                    }
                    k = k.ParentScope;
                }

                if (scopeOk) continue;

                ReportError(n.Source, "The class '" + n.DeclaredType.FullName + "' can only be used in '" + PathToNode(scope) + "'");
            }
        }

        string PathToNode(Node n)
        {
            var s = "";
            if (n.ParentScope != null)
            {
                s = PathToNode(n.ParentScope) + ".";
            }

            if (n.Scope is ClassNode)
            {
                return s + ((ClassNode)n.Scope).GeneratedClassName;
            }
            else if (n.Scope is TemplateNode)
            {
                var tn = (TemplateNode)n.Scope;
                return s + new TypeNameHelper(tn.ProducedType.FullName).Surname + "(line " + tn.Source.LineNumber + ")";
            }
            else
            {
                throw new Exception();
            }
        }

        readonly Dictionary<AST.Element, ClassNode> _innerClasses = new Dictionary<AST.Element, ClassNode>();
        void FindInnerClasses(AST.Element root)
        {
            var cg = root.Generator as AST.ClassGenerator;
            if (cg != null)
            {
                _innerClasses.Add(root, null);
            }
            foreach (var c in root.Children.OfType<AST.Element>())
                FindInnerClasses(c);
        }

        void CreateInnerClassNodes()
        {
            foreach (var n in _innerClasses.Keys.ToArray())
            {
                CreateInnerClassNode(n);
            }
        }

        ClassNode CreateInnerClassNode(AST.Element n)
        {
            if (_innerClasses[n] != null) return _innerClasses[n];

            var dt = ResolveType(n);
            if (dt == null)
            {
                ReportError(n.Source, "Could not resolve type '" + n.TypeName + "'");
                return null;
            }

            var e = n;
            var generator = (AST.ClassGenerator)AST.Generator.Resolve(n.Source, e, ContentMode.Default, InstanceType.None, _log);

            return _innerClasses[n] = (ClassNode)GeneratorToNode(n, dt, generator, InstanceType.None, generator.IsInnerClass);
        }

        int _errorCount = 0;

        internal void ReportError(FileSourceInfo node, string message)
        {
            _errorCount++;
            _log.ReportError(node.FileName, node.LineNumber, message);
        }

        void ResolveNameProperties()
        {
            _nodes.ToArray().Where(x => x.Key is AST.Element).ToList().ForEach(x => ResolveNameProperties((AST.Element)x.Key, x.Value));
        }

        void ResolveNameProperties(AST.Element e, UXIL.Node n)
        {
            if (n.InstanceType == InstanceType.Global && n.Name != null)
            {
                var globalKey = n.TryFindPropertyOrEvent(e.Source, this, "Resource.GlobalKey") as AtomicProperty;
                globalKey.Value = new String(n.Name, e.Source);
            }

            var fnp = n.FileNameProperty;
            if (fnp != null)
            {
                fnp.Value = new String(e.Source.FileName, e.Source);
            }

            var np = n.NameProperty;
            if (np != null)
            {
                if (e.UXName != null)
                {
                    if (np.Facet.DataType.FullName == "Uno.UX.Selector")
                    {
                        np.FallbackValue = new Selector(e.UXName, e.Source);
                    }
                    else
                    {
                        np.FallbackValue = new String(e.UXName, e.Source);
                    }
                }
                else
                {
                    var na = e.Properties.FirstOrDefault(x => x.Name == np.Facet.Name);
                    if (na != null)
                    {
                        n.Name = na.Value;
                    }
                }
            }
        }

        void ResolveAutoProperties()
        {
            _nodes.Values.ToList().ForEach(ResolveAutoProperties);
        }

        IDataType _nameTableType;
        internal IDataType NameTableType => _nameTableType ??
                                            (_nameTableType = ResolveType(null, Configuration.DefaultNamespace, "Uno.UX.NameTable"));

        void ResolveAutoProperties(UXIL.Node n)
        {
            foreach (var p in n.Properties.OfType<ReferenceProperty>().Where(x => x.Facet.IsUXAutoNameTableProperty))
            {
                if (n.InstanceType == InstanceType.Global)
                {
                    p.Bind(new ResourceRefNode(n.Source, "Uno.UX.NameTable.Empty", null, NameTableType, InstanceType.Global));
                }
                else if (n.InstanceType == InstanceType.Local)
                {
                    p.Bind(n.Scope.GetNameTable(this));
                }
            }

            foreach (var p in n.Properties.OfType<AtomicProperty>().Where(x => x.Facet.IsUXAutoClassNameProperty))
            {
                if (n.InstanceType == InstanceType.Global)
                {
                    ReportError(n.Source, "Cannot have global instances that of type '" + n.DeclaredType.FullName + "', they must be marked 'ux:Class' or specify 'ClassName'");
                }
                else if (n.InstanceType == InstanceType.Local)
                {
                    p.Value = new String(n.ContainingClass.FullName, n.Source);
                }
            }
        }

        void ResolveAuxNameTables()
        {
            foreach (var n in _nodes.Values.ToList())
            {
                var atomicAndReferencePropertiesWithValues = Enumerable.Concat<Property>(
                    n.AtomicPropertiesWithValues,
                    n.ReferencePropertiesWithValues);
                
                foreach (var p in atomicAndReferencePropertiesWithValues.Where(x => x.Facet.UXAuxNameTable != null))
                {
                    var aux = n.Properties.OfType<ReferenceProperty>().First(x => x.Facet.Name == p.Facet.UXAuxNameTable);
                    aux.Bind(n.Scope.GetNameTable(this));
                }
            }
        }


        void ResolveProperties()
        {
            _nodes.ToArray().Where(x => x.Key is AST.Element).ToList().ForEach(x => ResolveProperties((AST.Element)x.Key, x.Value));
        }

        int _eventBindingCount;


        void ResolveProperties(AST.Element e, UXIL.Node n)
        {
            foreach (var p in e.Properties)
            {
				// Deal with properties in `dep:` xmlns
				if (p.Namespace == Configuration.DependencyNamespace)
				{
					var depProp = n.TryFindPropertyOrEvent(p.Source, this, "Dependencies") as ListProperty;
					
					if (depProp == null)
					{
						ReportError(p.Source, "'dep:' namespace only supported on classes that have a 'Dependencies' list property");
						continue;
					}

					var exp = p.Value;
					var ex = Expressions.Parser.Parse(new Uno.Compiler.SourceFile(n.Source.FileName, exp, n.Source.LineNumber), exp, false);
					var eo = TransformExpression(ex, depProp, n.Source, n, IdentifierScope.Globals, false);

					var namedExp = new NewObjectNode(n.Source, null, depProp.BindableType, InstanceType.Local);
					namedExp.CreateMembers();
					n.AddChild(namedExp);

					namedExp.AtomicProperties.First(x => x.Facet.Name == "Name").Value = new String(p.Name, n.Source);
					namedExp.Properties.OfType<ReferenceProperty>().First(x => x.Facet.Name == "Expression").Bind(eo);

					depProp.Bind(namedExp);
					continue;
				}

                var prop = n.TryFindPropertyOrEvent(p.Source, this, p.Name);

                if (prop is DelegateProperty)
                {
                    var dp = (DelegateProperty)prop;

                    dp.Method = p.Value; // TODO: Validate

                    continue;
                }

                if (prop is Property)
                {
                    ParseAttribute(n, prop as Property, prop as Property, p.Value, p.Source);
                    continue;
                }

                var ev = prop as Event;

                if (ev != null)
                {
                    var eventExp = Expressions.Parser.Parse(new Uno.Compiler.SourceFile(n.Source.FileName, p.Value, n.Source.LineNumber), p.Value, false);

                    // This is to support the legacy case of Uno code behinds with event handler methods,
                    // <Button Clicked="btn_clicked" />
                    if (eventExp is Expressions.Identifier)
                    {
                        ev.Handler = new EventMethod(p.Value);
                        continue;
                    }

                    // TODO: make Fuse.Reactive.EventBinding configurable
                    var t = ResolveType(p.Source, Configuration.DefaultNamespace, "Fuse.Reactive.EventBinding");
                    var binding = new NewObjectNode(p.Source, "temp_eb" + (_eventBindingCount++), t, InstanceType.Local);
                    _nodes.Add(p, binding);
                    binding.CreateMembers();

                    var keyProp = binding.BindableProperties.First(x => x.Facet.IsConstructorArgument);

                    if (!TryFindSuitableParent(n, binding))
                    {
                        ReportError(p.Source, "Unable to find suitable host node for the event binding");
                    }

                    var exp = TransformExpression(eventExp, keyProp, p.Source, n, IdentifierScope.Globals);

                    keyProp.Bind(exp);
                        
                    ev.Handler = new EventBinding(binding);
                    continue;
                }

                if (n.ResultingType.ImplicitPropertySetter != null)
                {
                    Node srcNode;
                    var srcProp = TryFindProperty(n.Source, n, p.Name, out srcNode);

                    if (srcProp != null)
                    {
                        var setterType = ResolveType(n.Source, Configuration.DefaultNamespace, n.ResultingType.ImplicitPropertySetter);

                        var resolvedSetterType = new DeferredGenericType(setterType, "Target");
                        resolvedSetterType.SetResolvedGenericArgument(srcProp.Facet.DataType);

                        var setter = new NewObjectNode(n.Source, null, resolvedSetterType, n.InstanceType);

                        var targetProp = setter.TryFindBindableProperty(n.ContainingClass, "Target");

                        _uxProperties.Add(new UXPropertyClass(srcProp, srcNode));

                        if (targetProp == null)
                            ReportError(p.Source, "The implicit setter type '" + n.ResultingType.ImplicitPropertySetter + "' does not have a required 'Target' property");

                        var targetConstructorArg = targetProp.Facet as IConstructorArgument;

                        if (targetConstructorArg == null)
                            ReportError(p.Source, "The 'Target' property of implicit setter type '" + n.ResultingType.ImplicitPropertySetter + "' must be an immutable constructor argument");

                        targetProp.Bind(srcNode, srcProp);

                        var valueProp = setter.AtomicProperties.FirstOrDefault(x => x.Facet.Name == "Value");

                        if (valueProp == null)
                            ReportError(p.Source, "The implicit setter type '" + n.ResultingType.ImplicitPropertySetter + "' does not have a required atomic 'Value' property");

                        ParseGenericAtomic(srcNode, valueProp, p.Value, n.Source, srcProp.Facet.DataType);

                        n.AddChild(setter);

                        _nodes.Add(new object(), setter);
                        if (!TryAutoBind(n, setter))
                            ReportError(p.Source, "The implicit setter type '" + n.ResultingType.ImplicitPropertySetter + "' does not auto-bind to any properties on '" + e.TypeName + "'");

                        continue;
                    }
                }

                var missingHint = n.ResultingType.GetMissingPropertyHint(p.Name);

                if (p.Name.Contains('.'))
                {
                    ReportError(p.Source, "'" + e.TypeName + "' does not have an attached property called '" + p.Name + "', nor does it support implicit set-properties. " + missingHint);
                }
                else
                {
                    ReportError(p.Source, "'" + e.TypeName + "' does not have a property called '" + p.Name + "'. " + missingHint);
                }

            }
        }

        void ResolveBindings(Node n)
        {
            AutoBindLineNumber(n);
            AutoBindSourceFileName(n);
        }

        void ResolveElementBindings(AST.Element elm, Node n)
        {
            foreach (var b in elm.Children)
            {
                Node child;
                var isUXTest = false;

                if (b is AST.Element)
                {
                    var e = (AST.Element)b;
                    if (!_nodes.ContainsKey(b)) return;

                    child = _nodes[b];

                    if (e.UXKey != null)
                    {
                        child = child.Parent;
                    }

                    isUXTest = e.Generator is AST.TestGenerator;
                }
                else if (b is AST.ReferenceNode)
                {
                    var r = (AST.ReferenceNode)b;

                    var type = ResolveType(r);

                    child = ResolveNode(n.Source, n, r.RefId, type, x => x.Implements(type), "The reference does not match the specified type '" + type.FullName + "'");
                }
                else if (b is AST.Text)
                {
                    if (!_nodes.ContainsKey(b)) return;

                    child = _nodes[b];
                }
                else throw new Exception("Unknown AST node");

                if (child == null) continue;

                if (child is ClassNode) continue;
                if (isUXTest & child.Parent != n) continue; // The case for ux:Test nodes

                if (b.Binding != null)
                {
                    var prop = n.TryFindBindableProperty(n.ContainingClass, b.Binding);
                    if (prop == null)
                    {
                        ReportError(b.Source, "'" + n.MemberSource.FullName + " does not expose a bindable property called '" + b.Binding + "'");
                    }
                    else
                    {
                        prop.Bind(child);
                    }
                }
                else if (!b.ExplicitAutoBindFalse)
                {
                    if (!(child is PropertyNode || child is DependencyNode))
                    {
                        if (TryAutoBind(n, child)) continue;
                        else
                        {
                            if (child is TemplateNode)
                            {
                                ReportError(b.Source, "'" + b.TypeName + "' template doesn't fit here. '" + n.DeclaredType.FullName + "' does not have any content properties that accepts ux:Template");
                            }
                            else
                            {
                                ReportError(b.Source, "'" + b.TypeName + "' doesn't fit here. '" + n.DeclaredType.FullName + "' does not have any content properties that accepts '" + child.ResultingType.FullName + "'");
                            }
                        }
                    }
                }
            }
        }

        bool _triedInitTypes;
        IDataType _staticSolidColor;
        IDataType _solidColor;

        Node TryResolveSpecialTypes(Node owner, string refId, IDataType type)
        {
            if (!_triedInitTypes)
            {
                _staticSolidColor = _dataTypeProvider.TryGetTypeByName("Fuse.Drawing.StaticSolidColor");
                _solidColor = _dataTypeProvider.TryGetTypeByName("Fuse.Drawing.SolidColor");
                _triedInitTypes = true;
            }

            if ("#0123456789".Contains(refId.First()))
            {
                if (_staticSolidColor != null && _solidColor != null)
                {
                    IDataType t = null;

                    if (_staticSolidColor.Implements(type)) t = _staticSolidColor;
                    else if (_solidColor.Implements(type)) t = _solidColor;

                    if (t != null)
                    {
                        var sc = new NewObjectNode(owner.Source, null, t, InstanceType.Local);
                        sc.CreateMembers();
                        var colorProp = sc.AtomicProperties.First(x => x.Facet.Name == "Color");
                        colorProp.Value = Parse(refId, colorProp.Facet.DataType, sc.Source);
                        _nodes.Add(new object(), sc);
                        owner.AddChild(sc);
                        return sc;
                    }
                }
            }
            return null;
        }

        static bool TryAutoBind(Node parent, Node child)
        {
            if (child.ResultingType.IsFreestanding) return true;

            return
                TryAutoBind(parent, child, AutoBindingType.Primary) ||
                TryAutoBind(parent, child, AutoBindingType.Components) ||
                TryAutoBind(parent, child, AutoBindingType.Content);
        }

        static bool TryAutoBind(Node parent, Node child, AutoBindingType type)
        {
            switch (type)
            {
                case AutoBindingType.Primary:
                    foreach (var p in parent.PrimaryProperties)
                        if (p.Facet.Accepts(child.ResultingType))
                        {
                            p.Bind(child);
                            return true;
                        }
                    break;
                case AutoBindingType.Components:
                    foreach (var p in parent.ComponentProperties)
                        if (p.Facet.Accepts(child.ResultingType))
                        {
                            p.Bind(child);
                            return true;
                        }
                    break;

                case AutoBindingType.Content:
                    foreach (var p in parent.ContentProperties)
                        if (p.Facet.Accepts(child.ResultingType))
                        {
                            p.Bind(child);
                            return true;
                        }
                    break;

                

                default:
                    foreach (var p in parent.BindableProperties.Where(x => x.Facet.AutoBindingType == type))
                    {
                        if (p.Facet.Accepts(child.ResultingType))
                        {
                            p.Bind(child);
                            return true;
                        }
                    }
                    break;
            }

            

            if (child is BoxedValueNode)
            {
                var bvn = child as BoxedValueNode;
                foreach (var p in parent.AtomicProperties.Where(x => x.Facet.AutoBindingType == type))
                {
                    if (p.Facet.Accepts(child.ResultingType))
                    {
                        p.Value = bvn.Value;
                        return true;
                    }
                }
            }

            return false;
        }

        void AutoBindLineNumber(Node node)
        {
            var intType = _dataTypeProvider.TryGetTypeByName("int");
            foreach (var p in node.AtomicProperties.Where(x => x.Facet.AutoBindingType == AutoBindingType.LineNumber
                                                            && x.Facet.Accepts(intType)))
            {
                p.Value = new Uno.UX.Markup.Scalar<int>(node.Source.LineNumber, node.Source);
            }
        }

        void AutoBindSourceFileName(Node node)
        {
            var stringType = _dataTypeProvider.TryGetTypeByName("string");
            foreach (var p in node.AtomicProperties.Where(x => x.Facet.AutoBindingType == AutoBindingType.SourceFileName
                                                            && x.Facet.Accepts(stringType)))
            {
                var p1 = System.IO.Path.GetFullPath(_projectDirectory).NativeToUnix();
                var p2 = System.IO.Path.GetFullPath(node.Source.FileName).NativeToUnix();
                p.Value = new Uno.UX.Markup.String(p2.Substring(p1.Length+1), node.Source);
            }
        }

        bool TryFindSuitableParent(Node owner, Node bindingNode)
        {
            if (bindingNode.DeclaredType.IsFreestanding) return true;

            var parent = owner;

            var nodeType = ResolveType(bindingNode.Source, "Fuse", "Node");

            while (parent != null)
            {
                // HACK for now, this should be done with an attribute
                if (parent.DeclaredType.Implements(nodeType))
                {
                    if (!TryAutoBind(parent, bindingNode)) throw new Exception();
                    parent.AddChild(bindingNode);
                    return true;
                }

                if (parent is DocumentScope)
                {
                    if (parent.Parent != null && parent.Parent.ResultingType.FullName == "Fuse.Reactive.Each")
                    {
                        ReportError(bindingNode.Source, "Cannot use " + bindingNode.ResultingType.FullName + " here, because the parent node does not support behaviors. Consider placing a <Panel> inside the encapsulating <Each>.");
                    }
                    else
                    {
                        ReportError(bindingNode.Source, bindingNode.ResultingType.FullName + " cannot be used here, because there are no suitable parent nodes in this scope.");
                    }
                }

                parent = parent.Parent;
            }

            return false;
        }

        void CreateNodes(Node parent, AST.Node n, ContentMode contentMode, InstanceType instanceMode)
        {
            var dt = ResolveType(n);
            if (dt == null)
            {
                ReportError(n.Source, "Could not resolve type '" + n.TypeName + "'");
                return;
            }

            // Don't generate template for nodes that are already templates
            if (contentMode == ContentMode.Template)
            {
                if (dt.Implements(TemplateType))
                {
                    contentMode = ContentMode.Default;
                }
            }

            var e = n as AST.Element;
            if (e != null)
            {
                var rawProperties = e.Properties.Select(x => new RawProperty(x)).ToArray();

                Node node;
                if (e.ElementType == AST.ElementType.Property)
                {
                    node = new PropertyNode(e.Source, e.UXName, ResolveType(e), rawProperties);
                    parent.ContainingClass.RegisterUXProperty((PropertyNode)node);
                }
				else if (e.ElementType == AST.ElementType.Dependency)
				{
					node = new DependencyNode(e.Source, e.UXName, ResolveType(e), rawProperties);

                    if (!(parent is ClassNode))
                        ReportError(e.Source, "ux:Dependency can only be used in nodes marked ux:Class");

					parent.ContainingClass.RegisterDependency((DependencyNode)node);
				}
                else if (_innerClasses.ContainsKey(e))
                {
                    node = _innerClasses[e];
                    if (node == null) throw new Exception();
                }
                else
                {
                    var generator = AST.Generator.Resolve(n.Source, e, contentMode, instanceMode, _log);
                    node = GeneratorToNode(n, dt, generator, instanceMode, false);
                }

                if (node == null) return;

                if (e.UXPath != null)
                {
                    // Resolve full path, TODO: Verify sanity and emit warning
                    node.Path = e.UXPath.UnixToNative().ToFullPath(Path.GetDirectoryName(e.Source.FileName));
                }

                _nodes.Add(e, node);

                _astNodeToNode.Add(e, node);

                var childContenMode = ContentMode.Default;
                switch (dt.ContentMode)
                {
                    case ContentMode.Template: childContenMode = ContentMode.Template; break;
                    case ContentMode.TemplateIfClass: if (node is ClassNode) childContenMode = ContentMode.Template; break;
                }

                var childInstanceType = node.InstanceType;
                if (node.InstanceType == InstanceType.None) childInstanceType = InstanceType.Local;

                if (e.UXKey != null)
                {
                    var resourceType = _dataTypeProvider.TryGetTypeByName("Uno.UX.Resource");
                    var resourceObject = new NewObjectNode(n.Source, null, resourceType, instanceMode, rawProperties);

                    resourceObject.CreateMembers();

                    resourceObject.AtomicProperties.First(x => x.Facet.Name == "Key").Value = new String(e.UXKey, e.Source);
                    resourceObject.BindableProperties.First(x => x.Facet.Name == "Value").Bind(node);

                    resourceObject.AddChild(node);
                    parent.AddChild(resourceObject);
                }
                else
                {
                    if (node.Parent == null && parent != null)
                        parent.AddChild(node);
                }

                if (node.InstanceType == InstanceType.Global)
                    _globalResourceCache.AddGlobalNode(node);

                foreach (var c in ((AST.Element)n).Children)
                {
                    if (c is AST.Element || c is AST.ReferenceNode || c is AST.Text)
                    {
                        CreateNodes(node, c, childContenMode, childInstanceType);
                    }
                }

                return;
            }
            else if (n is AST.Text)
            {
                var t = n as AST.Text;
                parent.ContentString = t.UXValue;
                return;
            }
            else if (n is AST.ReferenceNode)
            {
                return;
            }

            ReportError(n.Source, "Unknown element type");
        }


        Node GeneratorToNode(AST.Node e, IDataType dt, AST.Generator generator, InstanceType instanceType, bool isInnerClass)
        {
            IEnumerable<RawProperty> rawProperties = null;

            var clearColor = (Vector<float>)null;

            var elm = e as AST.Element;
            if (elm != null)
            {
                rawProperties = elm.Properties.Select(x => new RawProperty(x)).ToArray();

                if (elm.ClearColor != null)
                {
                    clearColor = (Vector<float>)Parse(elm.ClearColor, Float4, elm.Source);
                }
            }

            if (generator is AST.InstanceGenerator)
            {
                var ig = (AST.InstanceGenerator)generator;

                var name = e.UXName;

                if (ig is AST.GlobalInstanceGenerator)
                {
                    instanceType = InstanceType.Global;
                    name = ((AST.GlobalInstanceGenerator)ig).Name ?? name;
                }

				if (generator is AST.TemplateGenerator)
                {
                    var fg = (AST.TemplateGenerator)generator;
                    return new UXIL.TemplateNode(e.Source, name, fg.Case, fg.IsDefaultCase, dt, TemplateType, new TypeNameHelper(fg.ClassName), clearColor, instanceType, rawProperties);
                }
                else if (dt.IsValueType || dt.IsString )
                {
                    AtomicValue value = null;
                    if (e.UXValue != null)
                    {
                        value = Parse(e.UXValue, dt, e.Source);
                    }
                    else ReportError(e.Source, "Value type node must specify 'ux:Value' attribute");

                    return new UXIL.BoxedValueNode(e.Source, name, dt, value, instanceType, rawProperties);
                }
                else
                {
                    return new UXIL.NewObjectNode(e.Source, name, dt, instanceType, rawProperties);
                }
            }
            else if (generator is AST.ClassGenerator)
            {
                var cg = (AST.ClassGenerator)generator;
                var name = e.UXName ?? "this";
                var cl = new UXIL.ClassNode(e.Source, cg.IsInnerClass, name, dt, new TypeNameHelper(cg.ClassName), clearColor, cg.AutoCtor, cg.Simulate, false, rawProperties);

                if (!isInnerClass)
                    _rootClasses.Add(cl);
                return cl;
            }
            else if (generator is AST.TestGenerator)
            {
                var tg = (AST.TestGenerator)generator;

                var testBootstrapper = dt.UXTestBootstrapper;
                if (testBootstrapper == null)
                {
                    ReportError(e.Source, "The type '" + dt.FullName + "' cannot be used with ux:Test because no test bootstrapper for the type was found. Could you be missing a package reference?");
                    return null;
                }

                var testClassName = new TypeNameHelper(tg.TestName);

                var testClass = new UXIL.ClassNode(e.Source, false, "this", testBootstrapper, testClassName, clearColor, true, true, true, rawProperties);
                _rootClasses.Add(testClass);
                _nodes.Add(testClassName, testClass);

                var testSubject = GeneratorToNode(e, dt, new AST.InstanceGenerator(), InstanceType.Local, false);
                TryAutoBind(testClass, testSubject);
                testClass.AddChild(testSubject);

                return testSubject;
            }

            ReportError(e.Source, "Unknown generator: " + generator);
            return null;
        }

        abstract class PropertyIdentifier
        {
        }

        sealed class PropertyPath: PropertyIdentifier
        {
            public string Path { get; }
            public AST.Node Owner { get; }
            public PropertyPath(string path, AST.Node owner) { Path = path; Owner = owner; }

            public override bool Equals(object obj)
            {
                if (!(obj is PropertyPath)) return false;
                return Path == ((PropertyPath)obj).Path;
            }

            public override int GetHashCode()
            {
                return Path.GetHashCode();
            }
        }

        sealed class ActualProperty: PropertyIdentifier
        {
            public Property Property { get; }

            public ActualProperty(Property prop)
            {
                Property = prop;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is ActualProperty)) return false;
                var np = (ActualProperty)obj;

                return Property == np.Property;
            }

            public override int GetHashCode()
            {
                return Property.GetHashCode();
            }
        }

        readonly Dictionary<Tuple<IDataType, PropertyIdentifier>, DeferredGenericType> _deferredGenericTypes = new Dictionary<Tuple<IDataType, PropertyIdentifier>, DeferredGenericType>();

        void ResolveDeferredGenericTypes()
        {
            var totalTypesResolved = 0;

            while (totalTypesResolved < _deferredGenericTypes.Count)
            {
                var typesResolved = 0;

                foreach (var e in _deferredGenericTypes)
                {
                    Property prop;

                    if (e.Key.Item2 is ActualProperty)
                    {
                        prop = ((ActualProperty)e.Key.Item2).Property;
                    }
                    else if (e.Key.Item2 is PropertyPath)
                    {
                        Node node;
                        var pp = (PropertyPath)e.Key.Item2;
                        var path = pp.Path;
                        var owner = _nodes[pp.Owner];
                        prop = TryFindProperty(owner.Source, owner, path, out node);
                        if (prop == null) ReportError(node.Source, $"{path} is not a valid property path");
                    }
                    else throw new Exception();

                    if (prop.Facet.IsActualDataTypeAvailable)
                    {
                        e.Value.SetResolvedGenericArgument(prop.Facet.DataType);
                        typesResolved++;
                        totalTypesResolved++;
                    }
                }

                if (typesResolved == 0)
                {
                    ReportError(null, "Unable to resolve all generic types because the document contains circular implied type references");
                }
            }
        }

        IDataType ResolveType(AST.Node e)
        {
            var gt = ResolveType(e.Source, e.Namespace, e.TypeName);

            if (gt == null)
            {
                ReportError(e.Source, "Data type not found: " + e.TypeName);
                return null;
            }

            if (!(gt is ClassNode) && gt.AutoGenericInfo != null && e is AST.Element)
            {
                var elm = (AST.Element) e;

                string genericArgProp = null;

                var prop = elm.Properties.Where(x => x.Name == gt.AutoGenericInfo.ArgumentProperty).FirstOrDefault();

                if (prop == null)
                {
                    var implProp = elm.Properties.Where(x => x.Name.Contains('.')).FirstOrDefault();

                    if (implProp != null)
                    {
                        genericArgProp = implProp.Name;
                    }
                    else
                    {
                        ReportError(e.Source, "Property '" + gt.AutoGenericInfo.ArgumentProperty + "' is required for creating generic type '" + gt.AutoGenericInfo.Alias + "'");
                        return gt;
                    }
                }
                else
                {
                    genericArgProp = prop.Value;
                }


                return DeferResolveGenericType(gt, new PropertyPath(genericArgProp, e), genericArgProp);
            }


            return gt;
        }

        class TypeHandle
        {
            public readonly string Namespace;
            public readonly string TypeName;
            public TypeHandle(string ns, string name)
            {
                Namespace = ns;
                TypeName = name;
            }
            public override int GetHashCode()
            {
                return TypeName.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                var th = obj as TypeHandle;
                if (obj == null) return false;
                return TypeName == th.TypeName && Namespace == th.Namespace;
            }
        }

        Dictionary<TypeHandle, IDataType> _typeCache = new Dictionary<TypeHandle, IDataType>();

        IDataType ResolveType(FileSourceInfo src, string ns, string typeName)
        {
            var th = new TypeHandle(ns, typeName);
            IDataType res;
            if (!_typeCache.TryGetValue(th, out res))
            {
                res = ResolveTypeInner(src, ns, typeName);
                _typeCache.Add(th, res);
            }
            return res;
        }

        IDataType ResolveTypeInner(FileSourceInfo src, string ns, string typeName)
        {
            var nss = ns.Split(',').Select(x => x.Trim());

            if (ns == Configuration.DefaultNamespace)
            {
                var dt = LookForType(typeName);
                if (dt != null) return dt;
            }

            foreach (var n in nss)
            {
                var dtName = n + "." + typeName;

                var dt = LookForType(dtName);

                if (dt != null) return dt;
            }

            return _dataTypeProvider.GetTypeByGenericAlias(typeName);
        }

        Dictionary<string, AST.Element> _innerClassNameToElementCache;

        IDataType LookForType(string typeName)
        {
            if (_innerClassNameToElementCache == null)
            {
                _innerClassNameToElementCache = new Dictionary<string, AST.Element>();
                foreach (var c in _innerClasses)
                    _innerClassNameToElementCache.Add(((AST.ClassGenerator)c.Key.Generator).ClassName, c.Key);
            }

            AST.Element elm;
            if (_innerClassNameToElementCache.TryGetValue(typeName, out elm))
                return CreateInnerClassNode(elm);

            return _dataTypeProvider.TryGetTypeByName(typeName);
        }

        DeferredGenericType DeferResolveGenericType(IDataType gt, PropertyIdentifier propertyIdentifier, string propertyPath)
        {
            var tuple = Tuple.Create<IDataType, PropertyIdentifier>(gt, propertyIdentifier);
            DeferredGenericType dg;
            if (!_deferredGenericTypes.TryGetValue(tuple, out dg))
            {
                dg = new DeferredGenericType(gt, propertyPath);
                _deferredGenericTypes.Add(tuple, dg);
            }

            return dg;
        }


    }
}
