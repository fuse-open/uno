using System;
using Uno.Compiler.API.Domain.Graphics;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.Backends.UnoDoc.Builders.Naming;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class EntityNaming : IEntityNaming
    {
        public string GetPageTitle(IEntity entity)
        {
            return GetNamingImplementationFor(entity).GetPageTitle(entity);
        }

        public string GetIndexTitle(IEntity entity)
        {
            return GetNamingImplementationFor(entity).GetIndexTitle(entity);
        }

        public string GetFullIndexTitle(IEntity entity)
        {
            return GetNamingImplementationFor(entity).GetFullIndexTitle(entity);
        }

        public string GetNavigationTitle(IEntity entity)
        {
            return GetNamingImplementationFor(entity).GetNavigationTitle(entity);
        }

        private IEntityNaming GetNamingImplementationFor(IEntity entity)
        {
            if (entity is Block)
            {
                return new BlockNaming();
            }
            if (entity is DataType)
            {
                return new DataTypeNaming();
            }
            if (entity is Member)
            {
                return new MemberNaming();
            }
            if (entity is Namespace)
            {
                return new NamespaceNaming();
            }

            throw new ArgumentException("Unable to determine naming implementation for " + entity.GetType().FullName);
        }
    }
}