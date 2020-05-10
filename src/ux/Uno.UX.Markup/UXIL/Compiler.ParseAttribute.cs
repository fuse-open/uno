using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.IO;
using Uno.UX.Markup.UXIL.Expressions;

namespace Uno.UX.Markup.UXIL
{
    class ValueBinding
    {
        public Node Owner { get; }
        public Property Property { get; }
        public string TypeName { get; }
        public IEnumerable<string> Arguments { get; }

        public Reflection.IDataType DeferredDataType { get; set; }

        public ValueBinding(Node owner, Property prop, string type, IEnumerable<string> arguments)
        {
            Owner = owner;
            Property = prop;
            TypeName = type;
            Arguments = arguments;
        }
    }

    public partial class Compiler
    {
        internal readonly HashSet<UXPropertyClass> _uxProperties = new HashSet<UXPropertyClass>();
        internal readonly HashSet<UXPropertyAccessorSource> _uxPropertyAccessors = new HashSet<UXPropertyAccessorSource>();

        Reflection.IDataType DataBindingType
        {
            get
            {
                var t = ResolveType(null, Configuration.DefaultNamespace, "Fuse.Reactive.DataBinding");
                if (t == null) throw new Exception("Data binding type not found. Are you missing a package reference to Fuse.Reactive.Bindings ?");
                return t;
            }
        }

        Reflection.IDataType FindType(string name)
        {
            var res = ResolveType(null, Configuration.DefaultNamespace, name);
            if (res == null) throw new Exception("Type not found: " + name);
            return res;
        }

        Reflection.IDataType _fileSourceType;
        Reflection.IDataType FileSourceType
        {
            get
            {
                return _fileSourceType ?? (_fileSourceType = _dataTypeProvider.DataTypes.First(x => x.FullName == "Uno.UX.FileSource"));
            }
        }

        Reflection.IDataType _expressionType;
        Reflection.IDataType ExpressionType
        {
            get
            {
                if (_expressionType == null) _expressionType = ResolveType(null, Configuration.DefaultNamespace, "Fuse.Reactive.IExpression");
                return _expressionType;
            }
        }

        /**
            High-level entry point for parsing a UX attribute value.
        */
        void ParseAttribute(Node owner, Property targetProp, Property contextProp, string value, FileSourceInfo src)
        {
            var isStringLike = targetProp.Facet.DataType.IsString || targetProp.Facet.DataType.Implements(FileSourceType);

            try
            {
                var e = Parser.Parse(new Uno.Compiler.SourceFile(src.FileName, value, src.LineNumber), value, isStringLike);
                ParseAttribute(owner, targetProp, contextProp, e, src, targetProp.Facet.UXIdentifierScope);
            }
            catch (Exception e)
            {
                ReportError(src, e.Message);
            }
        }

        void ParseAttribute(Node owner, Property targetProp, Property contextProp, Expression e, FileSourceInfo src, Reflection.IdentifierScope identifierScope)
        {
            if (ExpressionType != null && targetProp.Facet.DataType.Implements(ExpressionType))
            {
                // This supports e.g. <DataBinding Target="foo.Prop" Key="foo" />
                ((BindableProperty)targetProp).Bind(TransformExpression(e, contextProp, src, owner, identifierScope, true));
            }
            else if (e.IsTrivial && !(targetProp is ReferenceProperty && (e is StringLiteral) && !(targetProp.Facet.DataType.Implements(FileSourceType))))
            {
                ParseTrivialExpression(e.ToString().Trim('\"'), owner, targetProp, contextProp, src);
            }
            else
            {
                var binding = new NewObjectNode(owner.Source, null, DataBindingType, InstanceType.Local);
                _nodes.Add(e, binding);
                binding.BindableProperties.First(x => x.Facet.Name == "Target").Bind(owner, targetProp);
                _uxProperties.Add(new UXPropertyClass(targetProp, owner));

                var argumentProp = binding.BindableProperties.First(x => x.Facet.Name == "Key");

                var mode = "Default";
                if (e is Binding)
                {
                    var b = (Binding)e;
                    if (b.Key is ModeExpression)
                    {
                        var mod = b.Key as ModeExpression;
                        e = new Binding(mod.Expression);
                        mode = mod.Mode.ToString();
                    }
                }

                binding.AtomicProperties.First(x => x.Facet.Name == "Mode")
                    .Value = new EnumValue((Reflection.IEnum)FindType("Fuse.Reactive.BindingMode"), mode, owner.Source);

                var exp = TransformExpression(e, contextProp, src, owner, identifierScope, true);
                argumentProp.Bind(exp);
                TryFindSuitableParent(owner, binding);
            }
        }

        NewObjectNode TransformExpression(Expression e, Property contextProp, FileSourceInfo src, Node owner, Reflection.IdentifierScope identifierScope, bool rootLevel = false)
        {
            if (e is Binding)
            {
                var binding = e as Binding;
                return TransformExpression(binding.Key, contextProp, src, owner, Reflection.IdentifierScope.Data, rootLevel);
            }

            if (e is ModeExpression)
            {
                var mod = (ModeExpression)e;

                if (!rootLevel)
                {
                    if (mod.Mode.HasFlag(Modifier.Write))
                        ReportError(owner.Source, "Write-modifiers only allowed on root-level expressions");

                    if (mod.Mode.HasFlag(Modifier.Clear))
                        ReportError(owner.Source, "Clear-modifiers only allowed on root-level expressions");
                }

                return TransformExpression(mod.Expression, contextProp, src, owner, Reflection.IdentifierScope.Data);
            }

            if (e is RawExpression)
            {
                var re = e as RawExpression;
                return TransformExpression(re.Expression, contextProp, src, owner, Reflection.IdentifierScope.Globals);
            }

            if (e is ThisExpression)
            {
                e = new Identifier("this");
                identifierScope = Reflection.IdentifierScope.Globals;
            }

            if (e is Identifier)
            {
                var id = e as Identifier;

                if (identifierScope == Reflection.IdentifierScope.Data || identifierScope == Reflection.IdentifierScope.Names)
                {
                    var idd = new NewObjectNode(src, null, FindType("Fuse.Reactive.Data"), InstanceType.Local);
                    idd.CreateMembers();
                    owner.AddChild(idd);
                    _nodes.Add(new object(), idd);
                    idd.AtomicProperties.First(x => x.Facet.Name == "Key").Value = new String(id.Name, owner.Source);
                    return idd;
                }
                if (identifierScope == Reflection.IdentifierScope.Globals)
                {
                    var literal = new NewObjectNode(src, null, FindType("Fuse.Reactive.Constant"), InstanceType.Local);
                    owner.AddChild(literal);
                    literal.CreateMembers();
                    _nodes.Add(new object(), literal);
                    var valueProp = literal.BindableProperties.OfType<ReferenceProperty>().First(x => x.Facet.Name == "Value");
                    ParseTrivialExpression(id.Name, literal, valueProp, contextProp, owner.Source);
                    
                    return literal;
                }
            }

            if (e is Literal)
            {
                var li = e as Literal;
                var literal = new NewObjectNode(src, null, FindType("Fuse.Reactive.Constant"), InstanceType.Local);
                literal.CreateMembers();
                _nodes.Add(new object(), literal);
                var v = li is StringLiteral ? new String(li.Value, owner.Source) : ParseArbitraryValue(li.Value, contextProp.Facet.DataType, owner.Source);
                var bv = new BoxedValueNode(src, null, FindType("object"), v, InstanceType.Local);
                bv.CreateMembers();
                owner.AddChild(bv);
                literal.BindableProperties.First(x => x.Facet.Name == "Value").Bind(bv);
                owner.AddChild(literal);
                return literal;
            }

            if (e is MemberExpression)
            {
                var me = e as MemberExpression;
                var obj = TransformExpression(me.Object, contextProp, src, owner, identifierScope);
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive.Member"), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                op.BindableProperties.First(x => x.Facet.Name == "Object").Bind(obj);
                op.AtomicProperties.First(x => x.Facet.Name == "Name").Value = new String(me.Member, owner.Source);
                return op;
            }

            if (e is NameValuePairExpression)
            {
                var me = e as NameValuePairExpression;
                var nameExp = me.Name;
                if (nameExp is Expressions.Identifier) nameExp = new Expressions.StringLiteral(((Expressions.Identifier)nameExp).Name); // Interpret {foo: bar} as {'foo':bar}
                var name = TransformExpression(nameExp, contextProp, src, owner, Reflection.IdentifierScope.Globals); // Left hand of : is in global context, to allow {foo: bar}
                var obj = TransformExpression(me.Value, contextProp, src, owner, identifierScope);
                
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive.NameValuePair"), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                op.BindableProperties.First(x => x.Facet.Name == "Value").Bind(obj);
                op.BindableProperties.First(x => x.Facet.Name == "Name").Bind(name);
                return op;
            }

            if (e is LookUpExpression)
            {
                var me = e as LookUpExpression;
                var collection = TransformExpression(me.Collection, contextProp, src, owner, identifierScope);
                var index = TransformExpression(me.Index, contextProp, src, owner, identifierScope);
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive.LookUp"), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                op.BindableProperties.First(x => x.Facet.Name == "Collection").Bind(collection);
                op.BindableProperties.First(x => x.Facet.Name == "Index").Bind(index);
                return op;
            }

            if (e is BinaryExpression)
            {
                var be = e as BinaryExpression;
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive." + be.Name), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "Left"), contextProp, be.Left, src, identifierScope);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "Right"), contextProp, be.Right, src, identifierScope);
                return op;
            }

            if (e is ConditionalExpression)
            {
                var be = e as ConditionalExpression;
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive.Conditional"), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "Condition"), contextProp, be.Condition, src, identifierScope);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "TrueValue"), contextProp, be.TrueCase, src, identifierScope);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "FalseValue"), contextProp, be.FalseCase, src, identifierScope);
                return op;
            }

            if (e is UnaryExpression)
            {
                var ue = e as UnaryExpression;
                var op = new NewObjectNode(src, null, FindType("Fuse.Reactive." + ue.Name), InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);
                ParseAttribute(op, op.BindableProperties.First(x => x.Facet.Name == "Operand"), contextProp, ue.Operand, src, identifierScope);
                return op;
            }

            if (e is UserDefinedUnaryOperator)
            {
                var ud = e as UserDefinedUnaryOperator;

                var operatorType = Types.FirstOrDefault(x => x.UXUnaryOperatorName == ud.UxUnaryOperatorName);
                if (operatorType == null)
                {
                    ReportError(owner.Source, "Could not resolve UX unary operator type: " + ud.UxUnaryOperatorName);
                    return null;
                }

                var op = new NewObjectNode(src, null, operatorType, InstanceType.Local);
                op.CreateMembers();
                owner.AddChild(op);
                _nodes.Add(new object(), op);

                // TODO: Remove fuselibs-specific type reference https://github.com/fusetools/uno/issues/1424
                if (operatorType.FullName == "Fuse.Reactive.Property")
                {
                    var objProp = op.BindableProperties.First(x => x.Facet.Name == "Object");
                    var accessorProp = op.BindableProperties.First(x => x.Facet.Name == "Accessor");

                    var mem = ud.Argument as MemberExpression;
                    if (mem != null)
                    {
                        ParsePropertyAccessor(owner, op, objProp, accessorProp, mem.Object, mem.Member);
                    }
                    else
                    {
                        var id = ud.Argument as Identifier;

                        if (id != null)
                        {
                            ParsePropertyAccessor(owner, op, objProp, accessorProp, new ThisExpression(), id.Name);
                        }
                        else
                        {
                            ReportError(owner.Source, "Argument to property-binding must be on the form 'named_object.Property' or 'Property' (to refer to properties on 'this' implicitly)");
                            return null;
                        }
                    }

                    
                }
                else
                {
                    ParseAttribute(op, op.Properties.First(x => x.Facet.IsConstructorArgument), contextProp, ud.Argument, src, identifierScope);
                }
                

                return op;
            }

            if (e is FunctionCallExpression)
            {
                var fce = (FunctionCallExpression)e;

                Reflection.IDataType func;
                if (identifierScope == Reflection.IdentifierScope.Names)
                {
                    func = FindType("Fuse.Reactive.NamedFunctionCall");
                }
                else
                {
                    if (!_uxFunctions.TryGetValue(fce.FuncName, out func))
                        ReportError(owner.Source, "UX function not found: '" + fce.FuncName + "'");
                }

                var fo = new NewObjectNode(src, null, func, InstanceType.Local);
                fo.CreateMembers();
                owner.AddChild(fo);
                _nodes.Add(new object(), fo);

                if (identifierScope == Reflection.IdentifierScope.Names)
                {
                    fo.AtomicProperties.First(x => x.Facet.Name == "Name").Value = new String(fce.FuncName, fo.Source);
                    identifierScope = Reflection.IdentifierScope.Globals;
                }

                if (func.Implements(VarArgFunction))
                {
                    var args = fo.BindableProperties.First(x => x.Facet.Name == "Arguments");
                    for (int i = 0; i < fce.Args.Length; i++)
                    {
                        var arg = TransformExpression(fce.Args[i], contextProp, src, owner, identifierScope);
                        args.Bind(arg);
                    }
                }
                else
                {
                    var index = 0;
                    foreach (var dep in fo.Properties.Where(x => x.Facet.IsConstructorArgument))
                        ParseAttribute(fo, dep, contextProp, fce.Args[index++], src, identifierScope);
                }

                return fo;
            }

            if (e is VectorExpression)
            {
                var ve = (VectorExpression)e;

                var vectorFunc = FindType("Fuse.Reactive.Vector");

                if (ve.Comps.All(x => x is Expressions.NameValuePairExpression))
                    vectorFunc = FindType("Fuse.Reactive.Object");

                var vo = new NewObjectNode(src, null, vectorFunc, InstanceType.Local);
                vo.CreateMembers();
                owner.AddChild(vo);
                _nodes.Add(new object(), vo);

                var args = vo.BindableProperties.First(x => x.Facet.Name == "Arguments");
                for (int i = 0; i < ve.Comps.Length; i++)
                {
                    var arg = TransformExpression(ve.Comps[i], contextProp, src, owner, identifierScope);
                    args.Bind(arg);
                }

                return vo;
            }
            

            throw new Exception("Unsupported expression type: " + e.GetType());
        }

        Reflection.IDataType _varArgFunction;
        Reflection.IDataType VarArgFunction
        {
            get
            {
                if (_varArgFunction == null) _varArgFunction = FindType("Fuse.Reactive.VarArgFunction");
                return _varArgFunction;
            }
        }

        void ParseTrivialExpression(string value, Node owner, Property p, Property contextProperty, FileSourceInfo src)
        {
            if (p is AtomicProperty) ParseGenericAtomic(owner, (AtomicProperty)p, value, src, null);
            else if (p is ReferenceProperty) BindReference(src, owner, (ReferenceProperty)p, contextProperty, value);
            else if (p is ListProperty) ParseListProperty(src, owner, (ListProperty)p, value);
            else throw new Exception();
        }

        void ParseGenericAtomic(Node owner, AtomicProperty p, string value, FileSourceInfo src, Reflection.IDataType optionalType)
        {
            var dt = optionalType ?? p.Facet.DataType;

            if (dt.IsValueType || dt.IsString || dt.FullName == "Uno.UX.Value")
            {
                p.Value = Parse(value, dt, src);
            }
            else
            {
                p.Value = new ReferenceValue(src, new NodeSource(ResolveNode(src, owner, value, dt)));
            }
        }

        void ParsePropertyAccessor(Node owner, Node propertyExp, BindableProperty objProp, BindableProperty accessorProp, Expression objEx, string propName)
        {
            Node node;
            var prop = FindProperty(owner.Source, owner, objEx.ToString() + "." + propName, out node);
            if (prop == null) return;

            objProp.Bind(TransformExpression(objEx, objProp, owner.Source, propertyExp, Reflection.IdentifierScope.Globals));
            _uxPropertyAccessors.Add(accessorProp.Bind(prop));
        }

        void BindReference(FileSourceInfo src, Node owner, BindableProperty p, Property contextProperty, string value)
        {
            if (p.Facet.ListItemType.FullName.StartsWith("Uno.UX.Property"))
            {
                ParsePropertyReference(src, owner, p, value);
                return;
            }

            if (p.Facet.ListItemType.FullName == "Uno.UX.FileSource")
            {
                ParseBundleFileProperty(src, owner, p, value.Trim('\''));
                return;
            }

            // Prioritize enum literals if the context property is of matching enum type
            if (TryParseEnum(src, owner, p, contextProperty, value))
                return;

            var n = ResolveNode(src, owner, value, p.Facet.ListItemType, x => x.Implements(p.Facet.ListItemType), 
                "The type must be compatible with '" + p.Facet.ListItemType.FullName + "'", x => x.Implements(contextProperty.Facet.ListItemType));

            if (n != null) p.Bind(n);
        }

        EnumValue TryParseEnum(FileSourceInfo src, Property contextProperty, string value)
        {
            if (contextProperty.Facet.DataType is Reflection.IEnum)
            {
                // Prioritize enum literals if the context property is of matching enum type
                var e = (Reflection.IEnum)contextProperty.Facet.DataType;

                if (e.Literals.Any(x => x.Name == value))
                    return new EnumValue(e, value, src);
            }
            return null;
        }

        bool TryParseEnum(FileSourceInfo src, Node owner, Property p, Property contextProperty, string value)
        {
            var v = TryParseEnum(src, contextProperty, value);
            if (v != null)
            {
                if (p is BindableProperty)
                {
                    var bp = (BindableProperty)p;
                    var en = new BoxedValueNode(owner.Source, null, v.Enum, v, InstanceType.Local);
                    owner.AddChild(en);
                    bp.Bind(en);
                }
                else if (p is AtomicProperty)
                {
                    var ap = (AtomicProperty)p;
                    ap.Value = v;
                }
                return true;
            }
            return false;
        }

        void ParseBundleFileProperty(FileSourceInfo src, Node owner, BindableProperty p, string value)
        {
            var path = value.UnixToNative().ToFullPath(Path.GetDirectoryName(owner.Source.FileName));

            if (!File.Exists(path))
            {
                ReportError(src, "File does not exist: " + path);
            }

            p.BindToBundleFile(path);
        }

        void ParsePropertyReference(FileSourceInfo src, Node owner, BindableProperty p, string value)
        {
            Node node;
            var prop = FindProperty(src, owner, value, out node);
            if (prop == null) return;

            _uxProperties.Add(new UXPropertyClass(prop, node));
            p.Bind(node, prop);
        }

        Property FindProperty(FileSourceInfo src, Node owner, string value, out Node node)
        {
            var prop = TryFindProperty(src, owner, value, out node);
            if (prop == null)
            {
                value = value.Contains(".") ? value : "this." + value;
                var parts = value.Split('.');
                ReportError(src, "'" + parts[0] + "' does not contain property '" + parts[1] + "'");
                return null;
            }

            return prop;
        }

        Property TryFindProperty(FileSourceInfo src, Node owner, string path, out Node node)
        {
            if (!path.Contains(".")) path = "this." + path;

            var parts = path.Split('.');

            node = ResolveNode(src, owner, parts[0], null, x => !x.IsValueType, "The type must be a reference type");
            if (node == null) return null;

            return node.TryFindPropertyOrEvent(src, this, parts[1]) as Property;
        }

        void ParseListProperty(FileSourceInfo src, Node owner, ListProperty p, string value)
        {
            var parts = value.Split(',').Select(x => x.Trim());

            foreach (var np in parts)
            {
                BindReference(src, owner, p, p, np);
            }
        }

        static string NodeToPath(Node x)
        {
            if (x is ResourceRefNode)
                return ((ResourceRefNode)x).StaticRefId;
            return x.ContainingClass.GeneratedClassName.FullName + "." + x.Name;
        }

        internal Node ResolveNode(FileSourceInfo src, Node owner, string name, Reflection.IDataType targetType, Predicate<Reflection.IDataType> acceptFunction = null, string notAcceptedMessage = null, Predicate<Reflection.IDataType> prioritizeFunction = null)
        {
            if (targetType != null)
            {
                var special = TryResolveSpecialTypes(owner, name, targetType);
                if (special != null) return special;
            }

            // First priority - nodes in the scope with the given name
            var n = owner.Scope.FindNode(name);
            if (n != null)
            {
                if (acceptFunction == null || acceptFunction(n.DeclaredType))
                    return n;

                // Ugly hack for allowing circular dependencies between UX and code-behind (needed by fuselibs' ManualTestApp -- remove once we've cleaned it up)
                if (targetType.FullName == n.DeclaredType.FullName)
                    return n;

                ReportError(src, "The node '" + name + "' can not be used here: " + notAcceptedMessage);
                return null;
            }

            // Second priority - global resources
            var resources = _globalResourceCache.TryFindNode(name);

            var acceptable = new List<Node>();



            if (resources != null)
            {
                foreach (var r in resources)
                {
                    if (acceptFunction == null || acceptFunction(r.ResultingType)) acceptable.Add(r);
                }

                // Prioritized project-local resources
                // to avoid aliasing with symbols from generated code
                if (acceptable.Count > 1)
                {
                    var filtered = acceptable.Where(x => x is BoxedValueNode).ToArray();
                    if (filtered.Count() > 0)
                    {
                        acceptable.Clear();
                        acceptable.AddRange(filtered);
                    }
                }

                if (acceptable.Count > 1)
                {
                    var filtered = acceptable.Where(x => x is NewObjectNode).ToArray();
                    if (filtered.Count() > 0)
                    {
                        acceptable.Clear();
                        acceptable.AddRange(filtered);
                    }
                }

                if (acceptable.Count == 0)
                {
                    if (!resources.Any())
                    {
                        var m = "";
                        foreach (var g in _nodes.Values)
                        {
                            if (g.Name == name)
                            {
                                if (g.ContainingClass.GeneratedClassName.FullName != owner.ContainingClass.GeneratedClassName.FullName)
                                {
                                    ReportError(src, "'" + name + "' declared in " + Path.GetFileName(g.Source.FileName) + "(" + g.Source.LineNumber + ") is a member of '"  + g.ContainingClass.GeneratedClassName.FullName +  "' and cannot be accessed from '"+ owner.ContainingClass.GeneratedClassName.FullName + "'. To make this work, consider making '" + owner.ContainingClass.GeneratedClassName.FullName + "' an ux:InnerClass of '" + g.ContainingClass.GeneratedClassName.FullName +"'.");
                                    return null;
                                }
                                else
                                {
                                    ReportError(src, "'" + name + "' declared in " + Path.GetFileName(g.Source.FileName) + "(" + g.Source.LineNumber + ") cannot be accessed from this scope.");
                                }
                            }

                        }

                        ReportError(src, "There is nothing named '" + name + "' in this scope, and no global resources with that alias. " + m);
                        return null;
                    }

                    var msg = "";
                    foreach (var r in resources)
                    {
                        msg += "'" + NodeToPath(r) + "' is of type '" + r.ResultingType.FullName + "'. ";

                        if (acceptFunction == null || acceptFunction(r.ResultingType)) acceptable.Add(r);
                    }

                    ReportError(src, "There is nothing named '" + name + "' in this scope, and none of the global resources with alias '" + name + "' can be used here: " + notAcceptedMessage + ". " + msg);
                    return null;
                }
                else if (acceptable.Count > 1)
                {
                    if (prioritizeFunction != null)
                    {
                        var prioritized = acceptable.Where(x => prioritizeFunction(x.DeclaredType)).ToArray();
                        if (prioritized.Length == 1) return prioritized[0];
                    }

                    ReportError(src, "Multiple global resources with alias '" + name + "' can be used here, please qualify with full name: " + acceptable.Select(NodeToPath).Aggregate((x, y) => x + ", " + y));
                    return null;
                }
                else
                {
                    return acceptable[0];
                }


            }

            ReportError(src, "'" + name + "' could not be found. It is not an ux:Name in this scope nor a global resource");
            return null;
        }
    }
}
