using System.Collections.Generic;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Members;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class FlatteningExtensions
    {
        public static IEnumerable<Event> EnumerateFlattenedEvents(this DataType dataType)
        {
            return EnumerateFlattenedEvents(dataType, new FlatteningState(dataType));
        }

        private static IEnumerable<Event> EnumerateFlattenedEvents(DataType dataType, FlatteningState state)
        {
            foreach (var ev in EnumerateFlattenedMembers(dataType.Events, state))
            {
                yield return ev;
            }

            if (dataType.Base != null)
            {
                foreach (var ev in EnumerateFlattenedEvents(dataType.Base, state))
                {
                    yield return ev;
                }
            }
        }

        public static IEnumerable<Field> EnumerateFlattenedFields(this DataType dataType)
        {
            return EnumerateFlattenedFields(dataType, new FlatteningState(dataType));
        }

        private static IEnumerable<Field> EnumerateFlattenedFields(DataType dataType, FlatteningState state)
        {
            foreach (var field in EnumerateFlattenedMembers(dataType.Fields, state))
            {
                yield return field;
            }

            if (dataType.Base != null)
            {
                foreach (var ev in EnumerateFlattenedFields(dataType.Base, state))
                {
                    yield return ev;
                }
            }
        }

        public static IEnumerable<Method> EnumerateFlattenedMethods(this DataType dataType)
        {
            return EnumerateFlattenedMethods(dataType, new FlatteningState(dataType));
        }

        private static IEnumerable<Method> EnumerateFlattenedMethods(DataType dataType, FlatteningState state)
        {
            foreach (var method in EnumerateFlattenedMembers(dataType.Methods, state))
            {
                yield return method;
            }

            if (dataType.Base != null)
            {
                foreach (var method in EnumerateFlattenedMethods(dataType.Base, state))
                {
                    yield return method;
                }
            }
        }

        public static IEnumerable<Property> EnumerateFlattenedProperties(this DataType dataType)
        {
            return EnumerateFlattenedProperties(dataType, new FlatteningState(dataType));
        }

        private static IEnumerable<Property> EnumerateFlattenedProperties(DataType dataType, FlatteningState state)
        {
            foreach (var property in EnumerateFlattenedMembers(dataType.Properties, state))
            {
                yield return property;
            }

            if (dataType.Base != null)
            {
                foreach (var property in EnumerateFlattenedProperties(dataType.Base, state))
                {
                    yield return property;
                }
            }
        }

        private static IEnumerable<T> EnumerateFlattenedMembers<T>(IEnumerable<T> members, FlatteningState state) where T : Member
        {
            foreach (var member in members)
            {
                if (state.ShouldBeVisible(member))
                {
                    state.Add(member);
                    yield return member;
                }
            }
        }

        private class FlatteningState
        {
            private readonly IList<string> _seenSignatures = new List<string>();
            private readonly DataType _originatingFrom;

            public FlatteningState(DataType originatingFrom)
            {
                _originatingFrom = originatingFrom;
            }

            public void Add(Member member)
            {
                _seenSignatures.Add(member.GetSignature());
            }

            public bool ShouldBeVisible(Member member)
            {
                var isUnseen = !_seenSignatures.Contains(member.GetSignature());
                var shouldIncludeStatics = member.DeclaringType == _originatingFrom;
                return isUnseen && (!member.IsStatic || shouldIncludeStatics);
            }
        }
    }
}
