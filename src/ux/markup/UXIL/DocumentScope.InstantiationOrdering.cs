using System.Collections.Generic;
using System.Linq;

namespace Uno.UX.Markup.UXIL
{

    public abstract partial class DocumentScope
    {
        Compiler _compiler;

        readonly HashSet<Node> _instantiationSet = new HashSet<Node>();

        internal void OrderInstantiations(Compiler compiler)
        {
            _compiler = compiler;

            foreach (var c in Children)
                OrderInstantiations(c, new Stack<Node>(), true);

            foreach (var c in Children)
                ScheduleRoots(c);
        }
        internal void ScheduleInstantiation(Node n)
        {
            _instantiationSet.Add(n);
            _instantiationOrder.Add(new NodeSource(n));
        }

        void OrderInstantiations(Node n, Stack<Node> dependencies, bool includeUXProperties)
        {
            ScheduleDependencies(n, dependencies, includeUXProperties);

            if (!(n is DocumentScope))
                foreach (var c in n.Children)
                {
                    OrderInstantiations(c, dependencies, includeUXProperties);
                }
        }

        void ScheduleRoots(Node n)
        {
            if (n != this && !_instantiationSet.Contains(n))
            {
                _instantiationSet.Add(n);
                _instantiationOrder.Add(new NodeSource(n));
            }

            if (!(n is DocumentScope))
                foreach (var c in n.Children)
                    ScheduleRoots(c);
        }

        readonly List<ReferenceSource> _instantiationOrder = new List<ReferenceSource>();
        public IEnumerable<ReferenceSource> InstantiationOrder => _instantiationOrder;

        readonly HashSet<Node> _scheduledSet = new HashSet<Node>();

        void ScheduleDependencies(Node n, Stack<Node> dependencies, bool includeUXProperties)
        {
            if (_scheduledSet.Contains(n)) return;
            _scheduledSet.Add(n);

            dependencies.Push(n);
            if (n is ObjectNode)
            {
                var on = (ObjectNode)n;

                //https://github.com/fusetools/Uno/issues/329
                on.CreateMembers(); // just to be sure, shouldn't strictly be neccessary though

                foreach (var d in on.ConstructorDependencies)
                {
                    if (d.Node == null) continue;

                    if (dependencies.Contains(d.Node))
                    {
                        _compiler.ReportError(n.Source, "'" + n.ResultingType.FullName + "' and '" + d.Node.ResultingType.FullName + "' at line " + d.Node.Source.LineNumber
                            + " are involved in a circular reference in their constructor arguments.");
                        return;
                    }

                    ScheduleNode(d.Node, dependencies, includeUXProperties);
                    ScheduleReferenceSource(d, dependencies, includeUXProperties);
                }

                foreach (var d in on.ReferencePropertiesWithValues)
                {
                    ScheduleReferenceSource(d.Source, dependencies, includeUXProperties);
                }

                foreach (var d in on.ListPropertiesWithValues)
                {
                    foreach (var p in d.Sources) ScheduleReferenceSource(p, dependencies, includeUXProperties);
                }
            }

            dependencies.Pop();
        }

        void ScheduleNode(Node n, Stack<Node> dependencies, bool includeUXProperties)
        {
            if (n.Scope == this)
            {
                ScheduleDependencies(n, dependencies, includeUXProperties);

                if (!_instantiationSet.Contains(n))
                {
                    _instantiationSet.Add(n);
                    _instantiationOrder.Add(new NodeSource(n));
                }
            }
        }

        void ScheduleReferenceSource(ReferenceSource d, Stack<Node> dependencies, bool includeUXProperties)
        {
            if (includeUXProperties)
            {
                var uxps = d as UXPropertySource;
                if (uxps != null) ScheduleProperySource(uxps, dependencies);
            }

            var ns = d as NodeSource;
            if (ns != null) ScheduleNodeSource(ns, dependencies);
        }

        void ScheduleProperySource(UXPropertySource uxps, Stack<Node> dependencies)
        {
            if (!_instantiationOrder.Any(x => x is UXPropertySource && ((UXPropertySource)x).Node == uxps.Node && ((UXPropertySource)x).Property.Property.Facet.Name == uxps.Property.Property.Facet.Name))
            {
                ScheduleNode(uxps.Node, dependencies, true);
                _instantiationOrder.Add(uxps);
            }
        }

        void ScheduleNodeSource(NodeSource ns,Stack<Node> dependencies)
        {
            if (ns.Node.DeclaredType.IsInnerClass)
            {
                var cr = (ClassNode)ns.Node.DeclaredType;
                ScheduleDependencies(cr, dependencies, false);
            }
        }

    }
}
