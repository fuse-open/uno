using System;
using System.Collections.Generic;
using Uno.Compiler.API.Backends;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Statements;
using Uno.IO;
using Uno.Logging;

namespace Uno.Compiler.API.Domain.IL
{
    public abstract class Pass : LogObject
    {
        protected readonly Disk Disk;
        protected readonly IBuildData Data;
        protected readonly IEnvironment Environment;
        protected readonly IEssentials Essentials;
        protected readonly IILFactory ILFactory;

        public virtual bool Condition => true;
        public Namespace Namespace { get; internal set; }
        public Function Function { get; protected internal set; }
        public DataType Type { get; protected internal set; }
        public BlockBase Block { get; internal set; }
        public Shader Shader { get; protected internal set; }
        public MetaProperty MetaProperty { get; internal set; }
        public Namescope Namescope => (
            MetaProperty != null
                ? MetaProperty.Parent :
                Function != null
                    ? Function.DeclaringType
                    : (Namescope)Block ?? Type
            ) ?? DataType.Invalid;

        protected Pass(
            Disk disk,
            IBuildData data,
            IEnvironment environment,
            IILFactory ilf)
            : base(disk)
        {
            Disk = disk;
            Data = data;
            Environment = environment;
            Essentials = ilf.Essentials;
            ILFactory = ilf;
        }

        protected Pass(Pass parent)
            : base(parent.Log)
        {
            Disk = parent.Disk;
            Data = parent.Data;
            Environment = parent.Environment;
            Essentials = parent.Essentials;
            ILFactory = parent.ILFactory;
        }

        protected Pass(Backend backend)
            : base(backend.Log)
        {
            Disk = backend.Disk;
            Data = backend.Data;
            Environment = backend.Environment;
            Essentials = backend.Essentials;
            ILFactory = backend.ILFactory;
        }

        protected Pass(ShaderBackend backend)
            : base(backend.Log)
        {
            Disk = backend.Disk;
            Data = backend.Data;
            Environment = backend.Environment;
            Essentials = backend.Essentials;
            ILFactory = backend.ILFactory;
        }

        public virtual void Run()
        {
            if (!Condition || !Begin())
                return;

            Data.IL.Visit(this);

            if (Data.MainClass != null)
            {
                var dt = Type = Data.MainClass;
                new Method(dt.Source, dt, null, 0, ".startup", DataType.Void, ParameterList.Empty, Data.StartupCode).Visit(this);
            }

            End();
        }

        public void Traverse(Action<Namespace> ns, Action<DataType> dt = null, Action<Function> f = null)
        {
            Visitor.Traverse(new Visitor(ns, dt, f), Data.IL);
        }

        public void Traverse(Action<DataType> dt, Action<Function> f = null)
        {
            Visitor.Traverse(new Visitor(null, dt, f), Data.IL);
        }

        public void Traverse(Action<Function> f)
        {
            Visitor.Traverse(new Visitor(null, null, f), Data.IL);
        }

        public void Traverse(IVisitor visitor)
        {
            Visitor.Traverse(visitor, Data.IL);
        }

        public virtual bool Begin()
        {
            return true;
        }

        public virtual void End()
        {
        }

        public virtual bool Begin(Namespace ns)
        {
            return true;
        }

        public virtual void End(Namespace ns)
        {
        }

        public virtual bool Begin(DataType dt)
        {
            return true;
        }

        public virtual void End(DataType dt)
        {
        }

        public virtual bool Begin(BlockBase b)
        {
            return !Environment.IsGeneratingCode;
        }

        public virtual void End(BlockBase b)
        {
        }

        public virtual void OnApply(Apply apply)
        {
        }

        public virtual bool Begin(MetaProperty mp)
        {
            return true;
        }

        public virtual void End(MetaProperty mp)
        {
        }

        public virtual bool Begin(Shader s)
        {
            return !Environment.IsGeneratingCode;
        }

        public virtual void End(Shader s)
        {
        }

        public virtual bool Begin(Function f)
        {
            return true;
        }

        public virtual void End(Function f)
        {
        }

        public virtual void BeginScope(Scope s)
        {
        }

        public virtual void EndScope(Scope s)
        {
        }

        public virtual void Begin(ref Statement e)
        {
        }

        public virtual void Next(Statement parent)
        {
        }

        public virtual void End(ref Statement e)
        {
        }

        public virtual void Begin(ref Expression e, ExpressionUsage u = ExpressionUsage.Argument)
        {
        }

        public virtual void End(ref Expression e, ExpressionUsage u = ExpressionUsage.Argument)
        {
        }
    }
}
