using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.IO;
using Uno.UX.Markup.UXIL;

namespace Uno.UX.Markup.CodeGeneration
{
    class CodeGenerator
    {

        public static void GenerateCode(UXIL.Project doc, TextFormatter tw, Common.IMarkupErrorLog log, Func<string, TextFormatter> textFormatterFactory)
        {
            var cg = new CodeGenerator(doc, tw, log, textFormatterFactory);
            cg.GenerateCode();
        }

        readonly UXIL.Project _doc;
        TextFormatter _tw;
        readonly Common.IMarkupErrorLog _log;
        readonly Func<string, TextFormatter> _textFormatterFactory;

        void ReportError(UXIL.Node node, string message)
        {
            _log.ReportError(node.Source.FileName, node.Source.LineNumber, message);
        }

        void ReportWarning(UXIL.Node node, string message)
        {
            _log.ReportWarning(node.Source.FileName, node.Source.LineNumber, message);
        }


        CodeGenerator(UXIL.Project doc, TextFormatter tw, Common.IMarkupErrorLog log, Func<string, TextFormatter> textFormatterFactory)
        {
            _doc = doc;
            _tw = tw;
            _log = log;
            _textFormatterFactory = textFormatterFactory;
        }

        void GenerateCode()
        {
            EmitUXPropertyClasses();

            foreach (var s in _doc.RootClasses)
            {
                _tw = _textFormatterFactory(s.GeneratedClassName.FullName);
                GenerateCode(new Scope(s, null));
            }
        }

        void GenerateCode(Scope scope)
        {
            BeginNamespace(scope);

            BeginClass(scope);

            EmitTest(scope);

            EmitDeclaredProperties(scope);

            EmitInnerFactories(scope);
            EmitInnerClasses(scope);

            FindAndRegisterUXPropertySources(scope);
			EmitUXPropertyFields(scope);

            EmitFields(scope);

            BeginStaticInitMethod(scope);
            EmitGlobalResourceRegistrations(scope);
            EmitInstantiations(scope, InstanceType.Global);
            EmitResourceProperties(scope);
            EndStaticInitMethod(scope);

            BeginInitMethod(scope);
            EmitInstantiations(scope, InstanceType.Local);
            EmitProperties(scope);
            EndInitMethod(scope);

            EndClass(scope);

            EndNamespace(scope);
        }

        void EmitTest(Scope scope)
        {
            var cl = scope.DocumentScope as ClassNode;
            if (cl == null || !cl.IsTest) return;

            Emit("[global::Uno.Testing.Test]");
            Emit("public void Run()");
            PushScope();
            Emit("RunTest();");
            PopScope();
        }

        void EmitGlobalResourceRegistrations(Scope scope)
        {
            var cn = scope.DocumentScope as ClassNode;
            if (cn == null || !cn.IsAppClass) return;

            foreach (var n in _doc.GlobalResources)
            {
                foreach (var v in n.Value.OfType<ResourceRefNode>())
                    Emit("global::Uno.UX.Resource.SetGlobalKey(global::" + v.StaticRefId + ", \"" + n.Key + "\");");
            }
        }

        void EmitDeclaredProperties(Scope scope)
        {
            var cl = scope.DocumentScope as ClassNode;

            if (cl == null) return;

            foreach (var c in cl.DeclaredUXProperties)
            {
                Emit(c.ResultingType.FullName + " _field_" + c.Name + ";");

                Emit("[global::Uno.UX.UXOriginSetter(\"Set"+ c.Name + "\")]");
                Emit("public " + c.ResultingType.FullName + " " + c.Name);
                PushScope();
                Emit("get { return _field_" + c.Name + "; }");
                Emit("set { Set" + c.Name + "(value, null); }");
                PopScope();

                Emit("public void Set" + c.Name + "(" + c.ResultingType.FullName + " value, global::Uno.UX.IPropertyListener origin)");
                PushScope();
                Emit("if (value != _field_" + c.Name + ")");
                PushScope();
                Emit("_field_" + c.Name + " = value;");
                Emit("OnPropertyChanged(" + c.Name.ToLiteral() + ", origin);");
                PopScope();
                PopScope();
            }

			foreach (var c in cl.DeclaredDependencies)
			{
				Emit("readonly " + c.ResultingType.FullName + " " + c.Name + ";");
			}
		}

        void EmitInnerFactories(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesExcludingRoot.OfType<UXIL.TemplateNode>())
            {
                if (n.GeneratedClassName.FullName == null)
                {
                    n.GeneratedClassName = scope.GetUniqueTemplateName(n);
                }

                GenerateCode(new Scope(n, scope));
            }
        }

        void EmitInnerClasses(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesExcludingRoot.OfType<UXIL.ClassNode>().Where(x => x.IsInnerClass))
            {
                GenerateCode(new Scope(n, scope));
            }
        }


        void BeginNamespace(Scope scope)
		{
            if (!string.IsNullOrEmpty(scope.DocumentScope.GeneratedClassName.Namespace))
            {
                Emit("namespace " + scope.DocumentScope.GeneratedClassName.Namespace);
                PushScope();
            }
        }

        void EndNamespace(Scope scope)
        {
            if (!string.IsNullOrEmpty(scope.DocumentScope.GeneratedClassName.Namespace))
            {
                PopScope();
            }
        }

        void BeginClass(Scope scope)
        {
            if (scope.DocumentScope is UXIL.ClassNode)
            {
                var t = scope.DocumentScope as UXIL.ClassNode;
                Emit("[Uno.Compiler.UxGenerated]");
                Emit("public partial class " + t.GeneratedClassName.Surname + ": " + t.BaseType.FullName);
                PushScope();
            }
            else if (scope.DocumentScope is UXIL.TemplateNode)
            {
                var t = scope.DocumentScope as UXIL.TemplateNode;
                Emit("[Uno.Compiler.UxGenerated]");
                Emit("public partial class " + t.GeneratedClassName.Surname + ": Uno.UX.Template");
                PushScope();
                EmitParentScopeClosureConstructor(scope);
            }
            else
            {
                ReportError(scope.DocumentScope, "Unsupported root node");
            }
        }

		string GetDependenciesArgString(Reflection.IDataType cn, bool prefixes)
		{
			var deps = cn.Properties.Where(x => x.IsConstructorArgument && x.DeclaringType == cn).ToArray();

			var s = "";
			for (int i = 0; i < deps.Length; i++)
			{
				if (i > 0) s += ", ";
                if (prefixes) s += "\n\t\t[global::Uno.UX.UXParameter(\"" + deps[i].Name + "\")] " + deps[i].DataType.FullName + " ";
                s += deps[i].Name;
			}
			return s;
		}

        private void EmitParentScopeClosureConstructor(Scope scope, bool initUX = false)
        {
            if (scope.ParentScope != null)
            {
                var parentType = scope.ParentScope.DocumentScope.GeneratedClassName;
                
                Emit("[Uno.WeakReference] internal readonly " + parentType + " __parent;");
				
                var fn = scope.DocumentScope as TemplateNode;
                if (fn != null)
                {
                    var parentInstanceType = new TypeNameHelper(fn.ParentScope.DeclaredType.FullName);
                    Emit("[Uno.WeakReference] internal readonly " + parentInstanceType + " __parentInstance;");
                    Emit("public " + scope.DocumentScope.GeneratedClassName.Surname + "(" + parentType + " parent, " + parentInstanceType + " parentInstance): base("+ (fn.Case != null ? fn.Case.ToLiteral() : "null") +", " + (fn.IsDefaultCase ? "true" : "false")  + ")");
                }
                else
                {
                    Emit("public " + scope.DocumentScope.GeneratedClassName.Surname + "(" + parentType + " parent)");
                }
                
                PushScope();
                Emit("__parent = parent;");
                if (fn != null) Emit("__parentInstance = parentInstance;");
                if (initUX) Emit("InitializeUX();");
                PopScope();
            }
        }

        void EndClass(Scope scope)
        {
            foreach (var s in scope.Selectors)
            {
                Emit("static global::Uno.UX.Selector " + scope.SelectorToName(s.Key) + " = " + s.Key.ToLiteral() + ";");
            }

            PopScope();
        }

        string UniqueName(UXIL.UXPropertyClass p)
        {
            var t = p.Property.Facet.DeclaringType;
            var iap = p.Property.Facet as Reflection.IAttachedProperty;
            if (iap != null) t = iap.OwnerType;
            else if (t.GenericParameterCount > 0 || t.IsGenericParametrization) t = p.Owner.DeclaredType;
            return _doc.ProjectName.ToIdentifier() + "_" + t.QualifiedName.ToIdentifier() + "_" + p.Property.Facet.Name.ToIdentifier() + "_Property";
        }

        string PropClassName(UXPropertySource p)
        {
            return "global::Uno.UX.Property<" + p.Property.Property.Facet.DataType.FullName + ">"; 
        }

        void EmitUXPropertyFields(Scope scope)
        {
            foreach (var p in scope.UXNodeProperties)
            {
                var fieldName = scope.PropertySourceIdentifier(p);
                Emit(PropClassName(p) + " " + fieldName + ";");
            }
        }


        void EmitUXPropertyClasses()
        {
            var processedClasses = new HashSet<string>();

            foreach (var p in _doc.UXPropertyAccessors)
            {
                var name = _doc.ProjectName.ToIdentifier() + "_accessor_" + p.ClassName;
                if (processedClasses.Contains(name)) continue;

                var facet = p.Property.Facet as Markup.Reflection.IMutableProperty;

                if (facet == null)
                {
                    ReportError(_doc.RootClasses.First(), "Animated properties must be mutable. '" + p.Property.Facet.Name + "' is a constructor argument");
                    continue;
                }

                processedClasses.Add(name);

                Emit("sealed class " + name + ": global::Uno.UX.PropertyAccessor");
                PushScope();

                var iap = p.Property.Facet as Reflection.IAttachedProperty;

                var ownerDt = p.Property.Facet.DeclaringType;
                if (iap != null) ownerDt = iap.OwnerType;

                var ownerDtName = !ownerDt.IsInnerClass ? ("global::" + ownerDt.FullName) : ownerDt.QualifiedName;
                var propDt = p.Property.Facet.DataType.FullName;

                Emit("public static global::Uno.UX.PropertyAccessor Singleton = new " + name + "();");

                Emit("public override global::Uno.UX.Selector Name { get { return _name; } }");
                Emit("static global::Uno.UX.Selector _name = \"" + p.Property.Facet.Name + "\";");

                Emit("public override global::Uno.Type PropertyType { get { return typeof(" + p.Property.Facet.DataType.FullName + "); } }");

                if (facet.CanGet)
                {
                    if (iap != null)
                    {
                        Emit("public override object GetAsObject(global::Uno.UX.PropertyObject obj) { return global::" + iap.DeclaringType.FullName + "." + iap.GetMethodName + "((" + ownerDtName + ")obj); }");
                    }
                    else
                    {
                        Emit("public override object GetAsObject(global::Uno.UX.PropertyObject obj) { return ((" + ownerDtName + ")obj)." + p.Property.Facet.Name + "; }");
                    }
                }
                if (facet.CanSet)
                {
                    if (facet.OriginSetterName != null)
                    {
                        if (iap != null)
                        {
                            throw new Exception("Cannot use origin setters on attached properties");
                        }
                        else
                        {
                            Emit("public override void SetAsObject(global::Uno.UX.PropertyObject obj, object v, global::Uno.UX.IPropertyListener origin) { ((" + ownerDtName + ")obj)." + facet.OriginSetterName + "((" + propDt +")v, origin); }");
                            Emit("public override bool SupportsOriginSetter { get { return true; } }");
                        }

                    }
                    else
                    {
                        if (iap != null)
                        {
                            Emit("public override void SetAsObject(global::Uno.UX.PropertyObject obj, object v, global::Uno.UX.IPropertyListener origin) { global::" + iap.DeclaringType.FullName + "." + iap.SetMethodName + "((" + ownerDtName + ")obj, (" + propDt + ")v); }");
                        }
                        else
                        {
                            Emit("public override void SetAsObject(global::Uno.UX.PropertyObject obj, object v, global::Uno.UX.IPropertyListener origin) { ((" + ownerDtName + ")obj)." + p.Property.Facet.Name + " = (" + propDt + ")v; }");
                        }
                    }

                }

                PopScope();
            }

            foreach (var p in _doc.UXProperties)
            {
                var name = UniqueName(p);
                if (processedClasses.Contains(name)) continue;

                var facet = p.Property.Facet as Markup.Reflection.IMutableProperty;

				if (facet == null)
				{
					ReportError(_doc.RootClasses.First(), "Animated properties must be mutable. '" + p.Property.Facet.Name + "' is a constructor argument");
					continue;
				}

                processedClasses.Add(name);

                Emit("sealed class " + name + ": Uno.UX.Property<" + p.Property.Facet.DataType.FullName + ">");
                PushScope();

                var iap = p.Property.Facet as Reflection.IAttachedProperty;

                var ownerDt = p.Property.Facet.DeclaringType;
                if (iap != null) ownerDt = iap.OwnerType;

                var ownerDtName = ownerDt.QualifiedName;
                if (iap == null && ownerDt.GenericParameterCount > 0 || ownerDt.IsGenericParametrization) ownerDtName = p.Owner.DeclaredType.QualifiedName;
                var propDt = p.Property.Facet.DataType.FullName;

                Emit((p.Owner.DeclaredType.IsFreestanding ? "" : "[Uno.WeakReference] ") + "readonly " + ownerDtName + " _obj;");

                Emit("public " + name + "(" + ownerDtName + " obj, global::Uno.UX.Selector name) : base(name) { _obj = obj; }");

                Emit("public override global::Uno.UX.PropertyObject Object { get { return _obj; } }");
                
                if (facet.CanGet)
                {
                    if (iap != null)
                    {
                        Emit("public override " + propDt + " Get(global::Uno.UX.PropertyObject obj) { return global::" + iap.DeclaringType.FullName + "." + iap.GetMethodName + "((" + ownerDtName + ")obj); }");
                    }
                    else
                    {
                        Emit("public override " + propDt + " Get(global::Uno.UX.PropertyObject obj) { return ((" + ownerDtName + ")obj)." + p.Property.Facet.Name + "; }");
                    }
                }
                if (facet.CanSet)
                {
                    if (facet.OriginSetterName != null)
                    {
                        if (iap != null)
                        {
                            throw new Exception("Cannot use origin setters on attached properties");
                        }
                        else
                        {
                            Emit("public override void Set(global::Uno.UX.PropertyObject obj, " + propDt + " v, global::Uno.UX.IPropertyListener origin) { ((" + ownerDtName + ")obj)." + facet.OriginSetterName + "(v, origin); }");
                            Emit("public override bool SupportsOriginSetter { get { return true; } }");
                        }
                        
                    }
                    else
                    {
                        
                        if (iap != null)
                        {
                            Emit("public override void Set(global::Uno.UX.PropertyObject obj, " + propDt + " v, global::Uno.UX.IPropertyListener origin) { global::" + iap.DeclaringType.FullName + "." + iap.SetMethodName + "((" + ownerDtName + ")obj, v); }");
                        }
                        else
                        {
                            Emit("public override void Set(global::Uno.UX.PropertyObject obj, " + propDt + " v, global::Uno.UX.IPropertyListener origin) { ((" + ownerDtName + ")obj)." + p.Property.Facet.Name + " = v; }");
                        }
                    }
                    
                }

                PopScope();
            }
        }

        void EmitFields(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesExcludingRoot)
            {
                if (n == scope.DocumentScope) continue;

                if (n is UXIL.NameTableNode)
                {
                    EmitStaticNameTable(n as NameTableNode);
                    continue;
                }
                else if (n.InstanceType == InstanceType.Global)
                {
                    if (!string.IsNullOrEmpty(n.Name))
                    {
                        var name = scope.GetUniqueIdentifier(n);
                        var prefix = "[global::Uno.UX.UXGlobalResource(\"" + n.Name + "\")] ";
                        Emit(prefix + "public static readonly " + n.ResultingType.QualifiedName + " " + name + ";");
                    }
                }
                else if (n is UXIL.ObjectNode)
                {
                    if (!string.IsNullOrEmpty(n.Name))
                        Emit("internal global::" + n.ResultingType.QualifiedName + " " + scope.GetUniqueIdentifier(n) + ";");
                }

            }
        }

        bool IsTempVariable(Scope s, Node n)
        {
            var on = n as PropertyNode;
            if (on?.Name != null)
            {
                return false;
            }

            if (!(n is UXIL.TemplateNode))
            {
                if (((s.DocumentScope is UXIL.ClassNode) || (n is UXIL.ObjectNode)) && n.Name != null)
                    return false; // No, it's a field.
            }

            return true;
        }


        void EmitResourceProperties(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesIncludingRoot)
            {
                if (n.InstanceType == InstanceType.Global)
                {
                    EmitProperties(scope, n);
                }
            }
        }

        void EmitInstantiations(Scope scope, InstanceType instanceType)
        {
            foreach (var rs in scope.DocumentScope.InstantiationOrder)
            {
                if (rs is UXPropertySource)
                {
                    var p = (UXPropertySource)rs;

                    if (p.Node == scope.DocumentScope && instanceType == InstanceType.Global) continue;

                    var it = p.Node.InstanceType;
                    if (it == InstanceType.None) it = InstanceType.Local;

                    if (p.Node != scope.DocumentScope && it != instanceType) continue;

					var propClassName = UniqueName(p.Property);
					var fieldName = scope.PropertySourceIdentifier(p);
					var arg = scope.GetUniqueIdentifier(p.Node, scope.DocumentScope);

                    var s = new Selector(p.Property.Property.Facet.Name, p.Node.Source);
                    scope.Register(s);

                    Emit(fieldName + " = new " + propClassName + "(" + arg + ", " + scope.SelectorToName(s.Value) + ");");
                }
                else if (rs is NodeSource)
                {
                    

                    if (rs.Node.InstanceType != instanceType) continue;

                    if (rs.Node == scope.DocumentScope) continue;
                    var n = rs.Node;

                    if (n is UXIL.BoxedValueNode)
                    {
                        var on = (UXIL.BoxedValueNode)n;
                        Emit((IsTempVariable(scope, n) ? "var " : "") + scope.GetUniqueIdentifier(n) + " = " + Instantiation(scope, on) + ";");
                    }
                    else if (n is UXIL.NewObjectNode)
                    {
                        var on = (UXIL.NewObjectNode)n;
                        Emit((IsTempVariable(scope, n) ? "var " : "") + scope.GetUniqueIdentifier(n) + " = " + Instantiation(scope, on) + ";");
                    }
                    else if (n is UXIL.DependencyNode)
                    {
                        // already declared as a field
                    }
                    else if (n is UXIL.TemplateNode)
                    {
                        var tn = (UXIL.TemplateNode)n;

                        Emit((IsTempVariable(scope, n) ? "var " : "") + scope.GetUniqueIdentifier(n) + " = new " + tn.GeneratedClassName + "(this, " + scope.Self + ");");
                    }
                    else if (n is UXIL.NameTableNode)
                    {
                        var nt = (UXIL.NameTableNode)n;
                        var pt = (nt.ParentTable != null ? "__parent.__g_nametable" : "null");
                        Emit(n.Name + " = new global::Uno.UX.NameTable(" + pt + ", __g_static_nametable);");
                    }
                    else if (n is UXIL.ResourceRefNode)
                    {
                        // Silent ignore, staticRefId can be used directly
                    }
                    else
                    {
                        throw new Exception("Unhandled node type in UX code generator: " + n.GetType().FullName);
                    }
                }
            }

            if (instanceType == InstanceType.Local)
            {
                EmitAtomicPropertyValues(scope, scope.DocumentScope);
                EmitEventHandlers(scope, scope.DocumentScope);
            }
        }

        void EmitStaticNameTable(UXIL.NameTableNode n)
        {
            Emit("global::Uno.UX.NameTable __g_nametable;");

            Emit("static string[] __g_static_nametable = new string[] {");
            _tw.Indent();
            for (int i = 0; i < n.Entries.Length; i++)
            {
                var e = n.Entries[i];
                var dt = e.Node.DeclaredType.FullName;
                Emit("\"" + e.Name + "\"" + (i < n.Entries.Length-1 ? "," : ""));
            }
            _tw.Unindent("};");
        }


        string EventHandlerToString(Scope scope, UXIL.EventHandler eh)
        {
            if (eh is UXIL.EventMethod)
            {
                var em = (EventMethod)eh;
                return em.Name;
            }
            else
            {
                var eb = (EventBinding)eh;
                return scope.GetUniqueIdentifier(eb.Binding) + ".OnEvent";
            }
        }
     

        void EmitEventHandlers(Scope scope, UXIL.Node node)
        {
            foreach (var e in node.EventsWithHandler)
            {
                if (e.Facet is Reflection.IRegularEvent)
                {
                    Emit(scope.GetUniqueIdentifier(node) + "." + e.Facet.Name + " += " + EventHandlerToString(scope, e.Handler) + ";");
                }
                else if (e.Facet is Reflection.IAttachedEvent)
                {
                    var iae = (Reflection.IAttachedEvent)e.Facet;

                    Emit("global::" + iae.DeclaringType.FullName + "." + iae.AddMethodName + "(" + scope.GetUniqueIdentifier(node) + ", " + EventHandlerToString(scope, e.Handler) + ");");
                }
            }
        }

        void EmitProperties(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesIncludingRoot)
            {
                if (n != scope.DocumentScope && n.InstanceType != InstanceType.Local) continue;
                if (n is UXIL.DocumentScope && n != scope.DocumentScope) continue;

                EmitProperties(scope, n);
            }
        }



        void FindAndRegisterUXPropertySources(Scope scope)
        {
            foreach (var n in scope.DocumentScope.NodesIncludingRoot)
            {
                foreach (var rp in n.ReferencePropertiesWithValues.Select(x => x.Source)
                    .Union(n.ListPropertiesWithValues.SelectMany(x => x.Sources)))
                {
                    if (rp is UXIL.UXPropertySource)
                    {
                        var uxps = (UXIL.UXPropertySource)rp;
                        scope.Register(uxps);
                    }
                }
            }
        }

        void EmitProperties(Scope scope, UXIL.Node n)
        {
            if (n is UXIL.BoxedValueNode || n is UXIL.NewObjectNode)
            { 
                EmitAtomicPropertyValues(scope, n);
                EmitEventHandlers(scope, n);
            }


            foreach (var dp in n.DelegatePropertiesWithValues)
            {
                EmitAssign(scope, n, (Reflection.IMutableProperty)dp.Facet, dp.Method);
            }


            foreach (var rp in n.MutableReferencePropertiesWithValues)
            {
                var s = rp.Source as ReferenceSource;
                if (s != null)
                {
                    var value = ReferenceSourceToString(scope, s);
                    EmitAssign(scope, n, (Reflection.IMutableProperty)rp.Facet, value);
                }
            }

            foreach (var lp in n.ListPropertiesWithValues)
            {
                foreach (var s in lp.Sources.OfType<ReferenceSource>()) 
                {
                    EmitAddToList(scope, n, lp, ReferenceSourceToString(scope, s));
                }
            }
        }

        void EmitAddToList(Scope scope, UXIL.Node parent, UXIL.ListProperty prop, string value)
        {
            var facet = prop.Facet as Markup.Reflection.IMutableProperty;
			Emit(scope.GetUniqueIdentifier(parent) + "." + prop.Facet.Name + ".Add(" + value + ");");
        }

        string ReferenceSourceToString(Scope scope, UXIL.ReferenceSource rs)
        {
            if (rs is UXIL.UXPropertySource)
            {
                return scope.PropertySourceIdentifier((UXIL.UXPropertySource)rs);
            }
            else if (rs is UXIL.UXPropertyAccessorSource)
            {
                return _doc.ProjectName.ToIdentifier() + "_accessor_" + ((UXIL.UXPropertyAccessorSource)rs).Singleton;
            }
            else if (rs is UXIL.NodeSource)
            {
                var ns = (UXIL.NodeSource)rs;

                if (ns.Source.InstanceType == InstanceType.Global && ns.Source.Name != null)
                {
                    return "global::" + ns.Source.Scope.GeneratedClassName + "." + (ns.Source.Name).ToIdentifier();
                }
                else
                {
                    return scope.GetUniqueIdentifier(ns.Source, scope.DocumentScope);
                }
            }
            else if (rs is UXIL.BundleFileSource)
            {
                var bfs = (UXIL.BundleFileSource)rs;
                return "new global::Uno.UX.BundleFileSource(import(" + bfs.Path.ToRelativePath(Path.GetDirectoryName(_doc.GeneratedPath)).NativeToUnix().ToLiteral() + "))";
            }
            else throw new Exception();
        }

        void EmitAtomicPropertyValues(Scope scope, UXIL.Node on)
        {
            foreach (var p in on.MutableAtomicPropertiesWithValues)
            {
                if (p.Value is ReferenceValue)
                {
                    EmitAssign(scope, on, (Reflection.IMutableProperty)p.Facet, ReferenceSourceToString(scope, (p.Value as ReferenceValue).Value));
                }
                else
                {
                    if (p.Value is Selector)
                    {
                        var handle = scope.Register((Selector)p.Value);
                        EmitAssign(scope, on, (Reflection.IMutableProperty)p.Facet, handle);
                    }
                    else
                    {
                        EmitAssign(scope, on, (Reflection.IMutableProperty)p.Facet, p.Value.ToLiteral());
                    }
                }
            }
        }

        void EmitAssign(Scope scope, UXIL.Node on, Reflection.IMutableProperty p, string value)
        {
			if (p is Reflection.IAttachedProperty)
            {
                var iap = (Reflection.IAttachedProperty)p;

                if (iap.CanSet)
                {
                    Emit("global::" + iap.DeclaringType.FullName + "." + iap.SetMethodName + "(" + scope.GetUniqueIdentifier(on) + ", " + value + ");");
                }
                else
                {
                    ReportError(on, "Property '" + iap.Name + "' is read-only");
                }
            }
            else
            {
                Emit(scope.GetUniqueIdentifier(on) + "." + p.Name + " = " + value + ";");
            }
        }

        string Instantiation(Scope scope, UXIL.ObjectNode on)
        {
            if (on.Path != null)
            {
                return on.DataType.QualifiedName + ".Load(import(" + on.Path.ToRelativePath(Path.GetDirectoryName(_doc.GeneratedPath)).NativeToUnix().ToLiteral() + "))";
            }
            else if (on is UXIL.BoxedValueNode)
            {
                var bvn = (UXIL.BoxedValueNode)on;
                return bvn.Value.ToLiteral();
            }

            return Instantiation(scope, on, on.DataType, on.Properties);
        }

        string Instantiation(Scope scope, Node src, Reflection.IDataType dataType, IEnumerable<Property> properties)
        {
			var args = dataType.Properties.Where(x => x.IsConstructorArgument);

			if (args.Any())
            {
                var mp = new List<string>();
                foreach (var arg in args)
                {
                    var p = properties.FirstOrDefault(x => x.Facet == arg);

                    if (p is AtomicProperty)
                    {
                        var ap = (AtomicProperty)p;
                        if (ap.Value != null) // Assume error message is already generated. If null, this is a ux:Dependency
                        {
                            if (ap.Value is ReferenceValue) mp.Add(ReferenceSourceToString(scope, (ap.Value as ReferenceValue).Value));
                            else mp.Add(ap.Value.ToLiteral());
                        }
                    }
                    else if (p is ReferenceProperty)
                    {
                        var rp = (ReferenceProperty)p;
                        if (rp.Source != null)// Assume error message is already generated. If null, this is a ux:Dependency
                            mp.Add(ReferenceSourceToString(scope, rp.Source));
                    }
                    else throw new Exception("Unhandled constructor argument type: " + p.GetType().FullName);
                }

                if (mp.Count > 0)
                    return "new global::" + dataType.QualifiedName + "(" + mp.Aggregate((x, y) => x + ", " + y) + ")";
                else
                    return "new global::" + dataType.QualifiedName + "()";
            }
            else
            {
                if (dataType.IsInnerClass)
                {
                    var path = FindParentScopePath(dataType as ClassNode, scope);

                    return "new global::" + dataType.QualifiedName + "(" + path + ")";
                }
                else
                {
                    return "new global::" + dataType.QualifiedName + "()";
                }
                        
            }
        }

        string FindParentScopePath(ClassNode cn, Scope scope)
        {
            var path = "this";
            var s = scope;
            while (s != null)
            {
                if (s.DocumentScope == cn.ParentScope) return path;
                path += ".__parent";
                s = s.ParentScope;
            }

            throw new Exception("'" + cn.GeneratedClassName + "' cannot be used here, wrong scope for inner class.");
        }

        void BeginInitMethod(Scope scope)
        {
            if (scope.DocumentScope is UXIL.ClassNode)
            {
                var cs = (UXIL.ClassNode)scope.DocumentScope;

                if (cs.IsInnerClass)
                {
                    EmitParentScopeClosureConstructor(scope, cs.AutoCtor);
                }
                else if (cs.AutoCtor)
                {
					Emit("[global::Uno.UX.UXConstructor]");
					Emit("public " + cs.GeneratedClassName.Surname + "("+ GetDependenciesArgString(cs, true) + ")" +
                        (cs.BaseType.Properties.Any(x => x.IsConstructorArgument) ? ": base(" + GetDependenciesArgString(cs.BaseType, false) + ")" : ""));
                    PushScope();

					foreach (var dep in cs.DeclaredDependencies)
					{
						Emit("this." + dep.Name + " = " + dep.Name + ";");
					}

                    Emit("InitializeUX();");
                    PopScope();
                }

                Emit("void InitializeUX()");
                PushScope();
            }
            else if (scope.DocumentScope is UXIL.TemplateNode)
            {
                var t = (UXIL.TemplateNode)scope.DocumentScope;

                Emit("public override object New()");
                PushScope();

                if (t.ProducedType.IsInnerClass)
                {
                    var path = FindParentScopePath((ClassNode)t.ProducedType, scope);
                    Emit("var __self = new " + t.ProducedType.FullName + "(" + path +");");
                }
                else 
                {
                    Emit("var __self = " + Instantiation(scope, t, t.ProducedType, t.Properties) + ";");
                }
            }
            else
            {
                ReportError(scope.DocumentScope, "Document scope not supported: " + scope.DocumentScope.GetType().FullName);
            }
        }

        

        void EndInitMethod(Scope scope)
        {
            if (scope.DocumentScope is TemplateNode)
            {
                Emit("return __self;");
            }

            PopScope();
        }

        void BeginStaticInitMethod(Scope scope)
        {
            Emit("static " + scope.DocumentScope.GeneratedClassName.Surname + "()");
            PushScope();
        }

        void EndStaticInitMethod(Scope scope)
        {
            PopScope();
        }

        void Emit(string s)
        {
            _tw.WriteLine(s);
        }

        void PushScope()
        {
            _tw.Indent("{");
        }

        void PopScope()
        {
            _tw.Unindent("}");
        }
    }
}
