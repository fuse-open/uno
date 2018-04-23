using System;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.API.Domain.IL
{
    public class Visitor : IVisitor
    {
        readonly Action<Namespace> _ns;
        readonly Action<DataType> _dt;
        readonly Action<Function> _f;
        readonly bool _nsr, _dtr;

        public Visitor(Action<Namespace> ns = null, Action<DataType> dt = null, Action<Function> f = null)
        {
            _ns = ns ?? (_ => { });
            _dt = dt ?? (_ => { });
            _f = f ?? (_ => { });
            _dtr = f != null;
            _nsr = _dtr || _dt != null;
        }

        public bool Visit(Namespace ns)
        {
            _ns(ns);
            return _nsr;
        }

        public bool Visit(DataType dt)
        {
            _dt(dt);
            return _dtr;
        }

        public void Visit(Function f)
        {
            _f(f);
        }

        public static void Traverse(IVisitor visitor, Namespace ns)
        {
            if (visitor.Visit(ns))
                foreach (var e in ns.Types)
                    Traverse(visitor, e);

            foreach (var e in ns.Namespaces)
                Traverse(visitor, e);
        }

        public static void Traverse(IVisitor visitor, DataType dt)
        {
            if (visitor.Visit(dt))
            {
                foreach (var f in dt.EnumerateFunctions())
                {
                    visitor.Visit(f);

                    if (f.IsGenericMethod)
                    {
                        var m = (Method)f;
                        foreach (var p in m.GenericParameterizations)
                            visitor.Visit(p);
                    }
                }
            }

            foreach (var e in dt.NestedTypes)
                Traverse(visitor, e);

            if (dt.IsGenericDefinition)
                foreach (var e in dt.GenericParameterizations)
                    Traverse(visitor, e);
        }
    }
}