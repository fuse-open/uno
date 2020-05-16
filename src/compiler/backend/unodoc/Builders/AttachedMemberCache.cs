using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.Backends.UnoDoc.Builders.Extensions;

namespace Uno.Compiler.Backends.UnoDoc.Builders
{
    public class AttachedMemberCache
    {
        private readonly Dictionary<DataType, List<AttachedUxProperty>> _attachedProperties = new Dictionary<DataType, List<AttachedUxProperty>>();
        private readonly Dictionary<DataType, List<AttachedUxEvent>> _attachedEvents = new Dictionary<DataType, List<AttachedUxEvent>>();

        public AttachedMemberCache(IEnumerable<DataType> types)
        {
            foreach (var type in types)
            {
                var props = FindAttachedProperties(type);
                foreach (var pair in props)
                {
                    if (!_attachedProperties.ContainsKey(pair.Key))
                    {
                        _attachedProperties.Add(pair.Key, new List<AttachedUxProperty>());
                    }
                    _attachedProperties[pair.Key].AddRange(pair.Value);
                }

                var evts = FindAttachedEvents(type);
                foreach (var pair in evts)
                {
                    if (!_attachedEvents.ContainsKey(pair.Key))
                    {
                        _attachedEvents.Add(pair.Key, new List<AttachedUxEvent>());
                    }
                    _attachedEvents[pair.Key].AddRange(pair.Value);
                }
            }
        }

        public List<AttachedUxProperty> GetAttachedUxProperties(List<DataType> tree, DataType currentDataType)
        {
            var result = new HashSet<AttachedUxProperty>(new AttachedUxPropertyEqualityComparer());
            foreach (var props in tree.Select(e => _attachedProperties.ContainsKey(e) ? _attachedProperties[e] : new List<AttachedUxProperty>()))
            {
                result.AddRange(props.Where(e => e.UnderlyingMethod.DeclaringType.GetUri() != currentDataType.GetUri()));
            }
            return result.ToList();
        }

        public List<AttachedUxEvent> GetAttachedUxEvents(List<DataType> tree, DataType currentDataType)
        {
            var result = new HashSet<AttachedUxEvent>(new AttachedUxEventEqualityComparer());
            foreach (var props in tree.Select(e => _attachedEvents.ContainsKey(e) ? _attachedEvents[e] : new List<AttachedUxEvent>()))
            {
                result.AddRange(props.Where(e => e.UnderlyingMethod.DeclaringType.GetUri() != currentDataType.GetUri()));
            }
            return result.ToList();
        } 

        private static Dictionary<DataType, List<AttachedUxProperty>> FindAttachedProperties(DataType dataType)
        {
            var result = new Dictionary<DataType, List<AttachedUxProperty>>();

            foreach (var method in dataType.Methods)
            {
                var attribute = method.Attributes.FirstOrDefault(e => e.ReturnType.QualifiedName == ExportConstants.UxAttachedPropertySetterAttributeName &&
                                                                       e.Arguments.Length > 0);
                if (attribute == null || method.Parameters.Length < 2)
                {
                    continue;
                }

                var propertyName = ((string) attribute.Arguments[0].ConstantValue).Split('.').Last();
                var attachedTo = method.Parameters[0].Type;
                var returnType = method.Parameters[1].Type;

                if (!result.ContainsKey(attachedTo))
                {
                    result.Add(attachedTo, new List<AttachedUxProperty>());
                }
                result[attachedTo].Add(new AttachedUxProperty(propertyName, method, returnType));
            }

            return result;
        } 

        private static Dictionary<DataType, List<AttachedUxEvent>> FindAttachedEvents(DataType dataType)
        {
            var result = new Dictionary<DataType, List<AttachedUxEvent>>();

            foreach (var method in dataType.Methods)
            {
                var attribute = method.Attributes.FirstOrDefault(e => e.ReturnType.QualifiedName == ExportConstants.UxAttachedEventAdderAttributeName &&
                                                                                   e.Arguments.Length > 0);
                if (attribute == null || method.Parameters.Length < 2)
                {
                    continue;
                }

                var eventName = ((string) attribute.Arguments[0].ConstantValue).Split('.').Last();
                var attachedTo = method.Parameters[0].Type;
                var returnType = method.Parameters[1].Type;

                if (!result.ContainsKey(attachedTo))
                {
                    result.Add(attachedTo, new List<AttachedUxEvent>());
                }
                result[attachedTo].Add(new AttachedUxEvent(eventName, method, returnType));

            }
            return result;
        } 
    }
}