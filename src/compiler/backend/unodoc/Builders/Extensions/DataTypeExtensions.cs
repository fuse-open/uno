using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Compiler.API.Domain.IL.Expressions;
using Uno.Compiler.API.Domain.IL.Members;
using Uno.Compiler.API.Domain.IL.Types;

namespace Uno.Compiler.Backends.UnoDoc.Builders.Extensions
{
    public static class DataTypeExtensions
    {
        private static readonly HashSet<string> WhitelistedUxNamespaces = new HashSet<string>(new[]
        {
            "Fuse",
            "Fuse.Reactive",
            "Fuse.Animations",
            "Fuse.Drawing",
            "Fuse.Entities",
            "Fuse.Controls",
            "Fuse.Layouts",
            "Fuse.Shapes",
            "Fuse.Elements",
            "Fuse.Effects",
            "Fuse.Triggers",
            "Fuse.Navigation",
            "Fuse.Triggers.Actions",
            "Fuse.Gestures",
            "Fuse.Resources",
            "Fuse.Native"
        });

        private static readonly List<string> ExcludedUxBaseNamespaces = new List<string>
        {
            "Fuse.Elements"
        };

        private static readonly List<string> UxClassNameExclusionFilters = new List<string>
        {
            "Internal",
            "Exception",
            "Dummy",
            "Designer",
            "Preview",
            "Scripting"
        };

        public static Namespace FindNamespace(this DataType dataType)
        {
            if (dataType.ParentNamespace != null)
            {
                return dataType.ParentNamespace;
            }

            var parent = dataType.Parent;
            while (parent != null)
            {
                var ns = parent as Namespace;
                if (ns != null)
                {
                    return ns;
                }
                parent = parent.Parent;
            }

            throw new ArgumentException("Unable to find parent namespace for data type " + dataType.FullName);
        }

        public static List<string> GetGenericParameterNamesOrNull(this DataType dataType, bool fullyQualified)
        {
            if (dataType.IsGenericParameterization)
            {
                return dataType.GenericArguments.Select(e => e.IsArray
                                                                     ? FormatGenericParameterName(e.ElementType, fullyQualified) + "[]"
                                                                     : FormatGenericParameterName(e, fullyQualified))
                               .ToList();
            }
            if (dataType.IsGenericDefinition)
            {
                return dataType.GenericParameters.Select(e => e.IsArray
                                                                      ? FormatGenericParameterName(e.ElementType, fullyQualified) + "[]"
                                                                      : FormatGenericParameterName(e, fullyQualified))
                               .ToList();
            }

            return null;
        }

        private static string FormatGenericParameterName(DataType dataType, bool fullyQualified)
        {
            if (dataType.IsGenericParameter)
            {
                return dataType.Name;
            }

            var result = fullyQualified
                                 ? new EntityNaming().GetFullIndexTitle(dataType)
                                 : new EntityNaming().GetIndexTitle(dataType);
            return result;
        }

        public static List<DataType> GetParameterTypesOrNull(this DataType dataType)
        {
            var delegateType = dataType as DelegateType;
            return delegateType?.Parameters.Select(e => e.Type).ToList();
        }

        public static List<Parameter> GetParametersOrNull(this DataType dataType)
        {
            var delegateType = dataType as DelegateType;
            return delegateType?.Parameters.ToList();
        }

        public static List<string> GetModifierNames(this DataType dataType)
        {
            return dataType.Modifiers.GetModifierNames();
        }

        public static UxClassProperties GetUxClassProperties(this DataType dataType)
        {
            var ns = dataType.FindNamespace();

            //
            // Must be in or beneath a whitelisted namespace, but not contain a string from the filter list
            //
            string uxNamespace;
            if (!IsWhitelistedUxNamespace(ns.FullName, out uxNamespace))
            {
                return null;
            }
            var relativeName = GetNameRelativeToWhitelistedNamespace(ns.FullName + "." + dataType.Name);
            if (UxClassNameExclusionFilters.Any(e => relativeName.Contains(e)))
            {
                return null;
            }

            //
            // Must be a public and non-abstract class
            //
            if (!dataType.IsClass || !dataType.IsPublic || dataType.IsAbstract)
            {
                return null;
            }

            //
            // Must have a parameterless constructor, or a constructor with the UXConstructor attribute
            //
            var publicConstructors = dataType.Constructors.Where(e => e.IsPublic).ToList();
            var validConstructors = publicConstructors.Where(e => (e.Parameters == null || e.Parameters.Length == 0) ||
                                                                  (e.Attributes ?? new NewObject[0]).Any(x => x.ReturnType.QualifiedName == ExportConstants.UxConstructorAttributeName));
            if (!validConstructors.Any())
            {
                return null;
            }

            return new UxClassProperties(uxNamespace, uxNamespace.ToLowerInvariant().Replace('.', '/'), relativeName);
        }


        private static bool IsWhitelistedUxNamespace(string ns, out string matchedNs)
        {
            string bestMatch = null;
            foreach (var whitelistedNs in WhitelistedUxNamespaces)
            {
                // Either exact match, or input ns being sub-ns
                if (whitelistedNs == ns || ns.StartsWith(whitelistedNs + "."))
                {
                    if (bestMatch == null || bestMatch.Length < whitelistedNs.Length)
                    {
                        bestMatch = whitelistedNs;
                    }
                }
            }

            if (bestMatch == null || ExcludedUxBaseNamespaces.Any(e => e == bestMatch))
            {
                matchedNs = null;
                return false;
            }

            matchedNs = bestMatch;
            return true;
        }

        public static string GetNameRelativeToWhitelistedNamespace(string name)
        {
            string longestPrefix = null;
            foreach (var ns in WhitelistedUxNamespaces)
            {
                if (name.StartsWith(ns + "."))
                {
                    if (longestPrefix == null || longestPrefix.Length < ns.Length)
                    {
                        longestPrefix = ns;
                    }
                }
            }

            return longestPrefix == null
                           ? name
                           : name.Substring(longestPrefix.Length + 1);
        }

        public static bool IsVirtualType(this DataType dataType)
        {
            return dataType.IsGenericParameter;
        }

        public class UxClassProperties
        {
            public string Namespace { get; private set; }
            public string NamespaceUri { get; private set; }
            public string Name { get; private set; }

            public UxClassProperties(string ns, string nsUri, string name)
            {
                Namespace = ns;
                NamespaceUri = nsUri;
                Name = name;
            }
        }
    }
}
