using System.Collections.Generic;

namespace Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData
{
    public class TableOfContentsViewModel
    {
        public List<TableOfContentsEntryGroupViewModel> UxProperties { get; }
        public List<TableOfContentsEntryGroupViewModel> AttachedUxProperties { get; }
        public List<TableOfContentsEntryGroupViewModel> UxEvents { get; }
        public List<TableOfContentsEntryGroupViewModel> AttachedUxEvents { get; }

        public List<TableOfContentsEntryGroupViewModel> JsModules { get; }
        public List<TableOfContentsEntryGroupViewModel> JsEvents { get; }
        public List<TableOfContentsEntryGroupViewModel> JsProperties { get; }
        public List<TableOfContentsEntryGroupViewModel> JsMethods { get; }

        public List<TableOfContentsEntryGroupViewModel> Namespaces { get; }

        public List<TableOfContentsEntryGroupViewModel> UxClasses { get; }
        public List<TableOfContentsEntryGroupViewModel> Classes { get; }
        public List<TableOfContentsEntryGroupViewModel> Delegates { get; }
        public List<TableOfContentsEntryGroupViewModel> Enums { get; }
        public List<TableOfContentsEntryGroupViewModel> Interfaces { get; }
        public List<TableOfContentsEntryGroupViewModel> Structs { get; }

        public List<TableOfContentsEntryGroupViewModel> Constructors { get; }
        public List<TableOfContentsEntryGroupViewModel> Properties { get; }
        public List<TableOfContentsEntryGroupViewModel> Methods { get; }
        public List<TableOfContentsEntryGroupViewModel> Events { get; }
        public List<TableOfContentsEntryGroupViewModel> Fields { get; }
        public List<TableOfContentsEntryGroupViewModel> Casts { get; }
        public List<TableOfContentsEntryGroupViewModel> Operators { get; }
        public List<TableOfContentsEntryGroupViewModel> Literals { get; }

        public List<TableOfContentsEntryGroupViewModel> SwizzlerTypes { get; }

        public TableOfContentsViewModel(List<TableOfContentsEntryGroupViewModel> uxProperties,
                                        List<TableOfContentsEntryGroupViewModel> attachedUxProperties,
                                        List<TableOfContentsEntryGroupViewModel> uxEvents,
                                        List<TableOfContentsEntryGroupViewModel> attachedUxEvents,
                                        List<TableOfContentsEntryGroupViewModel> jsModules,
                                        List<TableOfContentsEntryGroupViewModel> jsEvents,
                                        List<TableOfContentsEntryGroupViewModel> jsProperties,
                                        List<TableOfContentsEntryGroupViewModel> jsMethods,
                                        List<TableOfContentsEntryGroupViewModel> namespaces,
                                        List<TableOfContentsEntryGroupViewModel> uxClasses,
                                        List<TableOfContentsEntryGroupViewModel> classes,
                                        List<TableOfContentsEntryGroupViewModel> delegates,
                                        List<TableOfContentsEntryGroupViewModel> enums,
                                        List<TableOfContentsEntryGroupViewModel> interfaces,
                                        List<TableOfContentsEntryGroupViewModel> structs,
                                        List<TableOfContentsEntryGroupViewModel> constructors,
                                        List<TableOfContentsEntryGroupViewModel> properties,
                                        List<TableOfContentsEntryGroupViewModel> methods,
                                        List<TableOfContentsEntryGroupViewModel> events,
                                        List<TableOfContentsEntryGroupViewModel> fields,
                                        List<TableOfContentsEntryGroupViewModel> casts,
                                        List<TableOfContentsEntryGroupViewModel> operators,
                                        List<TableOfContentsEntryGroupViewModel> literals,
                                        List<TableOfContentsEntryGroupViewModel> swizzlerTypes)
        {
            UxProperties = uxProperties;
            AttachedUxProperties = attachedUxProperties;
            UxEvents = uxEvents;
            AttachedUxEvents = attachedUxEvents;

            JsModules = jsModules;
            JsEvents = jsEvents;
            JsProperties = jsProperties;
            JsMethods = jsMethods;

            Namespaces = namespaces;

            UxClasses = uxClasses;
            Classes = classes;
            Delegates = delegates;
            Enums = enums;
            Interfaces = interfaces;
            Structs = structs;

            Constructors = constructors;
            Properties = properties;
            Methods = methods;
            Events = events;
            Fields = fields;
            Casts = casts;
            Operators = operators;
            Literals = literals;

            SwizzlerTypes = swizzlerTypes;
        }

        public bool ShouldSerializeUxProperties() => UxProperties.Count > 0;
        public bool ShouldSerializeAttachedUxProperties() => AttachedUxProperties.Count > 0;
        public bool ShouldSerializeUxEvents() => UxEvents.Count > 0;
        public bool ShouldSerializeAttachedUxEvents() => AttachedUxEvents.Count > 0;

        public bool ShouldSerializeJsModules() => JsModules.Count > 0;
        public bool ShouldSerializeJsEvents() => JsEvents.Count > 0;
        public bool ShouldSerializeJsProperties() => JsProperties.Count > 0;
        public bool ShouldSerializeJsMethods() => JsMethods.Count > 0;

        public bool ShouldSerializeNamespaces() => Namespaces.Count > 0;

        public bool ShouldSerializeUxClasses() => UxClasses.Count > 0;
        public bool ShouldSerializeClasses() => Classes.Count > 0;
        public bool ShouldSerializeDelegates() => Delegates.Count > 0;
        public bool ShouldSerializeEnums() => Enums.Count > 0;
        public bool ShouldSerializeInterfaces() => Interfaces.Count > 0;
        public bool ShouldSerializeStructs() => Structs.Count > 0;

        public bool ShouldSerializeConstructors() => Constructors.Count > 0;
        public bool ShouldSerializeProperties() => Properties.Count > 0;
        public bool ShouldSerializeMethods() => Methods.Count > 0;
        public bool ShouldSerializeEvents() => Events.Count > 0;
        public bool ShouldSerializeFields() => Fields.Count > 0;
        public bool ShouldSerializeCasts() => Casts.Count > 0;
        public bool ShouldSerializeOperators() => Operators.Count > 0;
        public bool ShouldSerializeLiterals() => Literals.Count > 0;

        public bool ShouldSerializeSwizzlerTypes() => SwizzlerTypes.Count > 0;
    }
}