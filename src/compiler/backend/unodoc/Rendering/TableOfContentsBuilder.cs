using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.Backends.UnoDoc.ViewModels;
using Uno.Compiler.Backends.UnoDoc.ViewModels.MetaData;

namespace Uno.Compiler.Backends.UnoDoc.Rendering
{
    public class TableOfContentsBuilder
    {
        private readonly DocumentViewModel _viewModel;
        private readonly Dictionary<string, HashSet<DocumentViewModel>> _viewModelsByParent;
        private readonly Dictionary<string, DocumentViewModel> _viewModelsById;

        public TableOfContentsBuilder(DocumentViewModel viewModel,
                                      Dictionary<string, HashSet<DocumentViewModel>> viewModelsByParent,
                                      Dictionary<string, DocumentViewModel> viewModelsById)
        {
            _viewModel = viewModel;
            _viewModelsByParent = viewModelsByParent;
            _viewModelsById = viewModelsById;
        }

        public TableOfContentsViewModel Build()
        {
            var toc = new TableOfContentsViewModel(GetGroupedChildren("UxProperty", false, IsUxMember),
                                                   GetGroupedChildren("AttachedUxProperty", true, IsAttachedMember),
                                                   GetGroupedChildren("UxEvent", false, IsUxMember),
                                                   GetGroupedChildren("AttachedUxEvent", true, IsAttachedMember),
                                                   GetGroupedChildren("JsModule", false),
                                                   GetGroupedChildren("JsEvent", false),
                                                   GetGroupedChildren("JsProperty", false),
                                                   GetGroupedChildren("JsMethod", false),
                                                   GetGroupedChildren("Namespace", false),
                                                   GetGroupedChildren("UxClass", false, IsUxClass),
                                                   GetGroupedChildren("Class", false, e => !IsUxClass(e)),
                                                   GetGroupedChildren("Delegate", false),
                                                   GetGroupedChildren("Enum", false),
                                                   GetGroupedChildren("Interface", false),
                                                   GetGroupedChildren("Struct", false),
                                                   GetGroupedChildren("Constructor", false),
                                                   GetGroupedChildren("Property", false, e => !IsUxMember(e) && !IsAttachedMember(e)),
                                                   GetGroupedChildren("Method", false),
                                                   GetGroupedChildren("Event", false, e => !IsUxMember(e) && !IsAttachedMember(e)),
                                                   GetGroupedChildren("Field", false),
                                                   GetGroupedChildren("Cast", false),
                                                   GetGroupedChildren("Operator", false),
                                                   GetGroupedChildren("Literal", false),
                                                   GetGroupedChildren("SwizzlerType", false));
            return toc;
        }

        private static bool IsUxClass(DocumentViewModel model)
        {
            var dataType = model as DataTypeViewModel;
            if (dataType == null || (dataType.Comment?.Attributes?.Advanced ?? false))
            {
                return false;
            }

            return dataType.UxProperties != null;
        }

        private static bool IsUxMember(DocumentViewModel model)
        {
            var member = model as MemberViewModel;
            if (member == null || (member.Comment?.Attributes?.Advanced ?? false))
            {
                return false;
            }

            return member.UxProperties != null;
        }

        private static bool IsAttachedMember(DocumentViewModel model)
        {
            return model is AttachedMemberViewModel;
        }

        private List<TableOfContentsEntryGroupViewModel> GetGroupedChildren(string typeId, bool isAttached, Func<DocumentViewModel, bool> filter = null)
        {
            var result = GroupByAncestry(GetChildren(typeId, filter), isAttached);
            return result;
        }

        private List<TableOfContentsEntryViewModel> GetChildren(string typeId, Func<DocumentViewModel, bool> filter = null)
        {
            var children = _viewModelsByParent.ContainsKey(_viewModel.Id.Id)
                                   ? _viewModelsByParent[_viewModel.Id.Id].Where(e => e.Id.Type == typeId)
                                                                          .OrderBy(e => e.Titles.IndexTitle)
                                                                          .ToList()
                                   : new List<DocumentViewModel>();
            if (filter != null)
            {
                children = children.Where(filter).ToList();
            }

            var models = children.Select(e =>
            {
                var member = e as MemberViewModel;
                var returns = e as IReturnEnabledViewModel;
                var parameters = e as IParameterEnabledViewModel;
                var titles = new IndexTitlesViewModel(e.Titles.IndexTitle, e.Titles.FullyQualifiedIndexTitle);
                return new TableOfContentsEntryViewModel(e.Id,
                                                         e.Uri,
                                                         titles,
                                                         e.Comment?.ToBasicComment(),
                                                         returns?.Returns,
                                                         parameters?.Parameters,
                                                         member?.Flags,
                                                         e.DeclaredIn);
            }).ToList();
            return models;
        }

        private List<TableOfContentsEntryGroupViewModel> GroupByAncestry(List<TableOfContentsEntryViewModel> entries, bool isAttached)
        {
            var dataType = _viewModel as DataTypeViewModel;
            if (dataType != null)
            {
                return GroupByAncestry(dataType.Inheritance, entries, isAttached);
            }

            var member = _viewModel as MemberViewModel;
            if (member != null)
            {
                // Find the data type the member is declared in
                dataType = _viewModelsById[member.Id.ParentId] as DataTypeViewModel;
                if (dataType == null)
                {
                    throw new Exception("Parent " + member.Id.ParentId + " for member " + member.Id + " was not a data type, bug?");
                }
                return GroupByAncestry(dataType.Inheritance, entries, isAttached);
            }

            // If it was neither a data type or member, it's likely a namespace and we don't group those.
            return entries.Count == 0
                           ? new List<TableOfContentsEntryGroupViewModel>()
                           : new List<TableOfContentsEntryGroupViewModel> { new TableOfContentsEntryGroupViewModel(null, entries) };
        }

        private List<TableOfContentsEntryGroupViewModel> GroupByAncestry(InheritanceViewModel inheritance, List<TableOfContentsEntryViewModel> entries, bool isAttached)
        {
            // If there is no inheritance, just return an empty group
            if (inheritance == null)
            {
                return entries.Count == 0
                               ? new List<TableOfContentsEntryGroupViewModel>()
                               : new List<TableOfContentsEntryGroupViewModel> { new TableOfContentsEntryGroupViewModel(null, entries) };
            }

            var rootGroupItems = new List<TableOfContentsEntryViewModel>();

            // Build a cache
            var entriesByDeclaringType = new Dictionary<string, Tuple<DocumentReferenceViewModel, List<TableOfContentsEntryViewModel>>>();
            foreach (var entry in entries)
            {
                if (isAttached)
                {
                    if (entry.Parameters == null || entry.Parameters.Count < 1)
                    {
                        throw new ArgumentException($"Found attached member without required amount of parameters: {entry.Id.Id}");
                    }
                    rootGroupItems.Add(entry);
                }
                else
                {
                    if (entry.DeclaredIn == null)
                    {
                        rootGroupItems.Add(entry);
                    }
                    else
                    {
                        if (!entriesByDeclaringType.ContainsKey(entry.DeclaredIn.Id.Id))
                        {
                            entriesByDeclaringType.Add(entry.DeclaredIn.Id.Id,
                                                       new Tuple<DocumentReferenceViewModel, List<TableOfContentsEntryViewModel>>(entry.DeclaredIn,
                                                                                                                                  new List<TableOfContentsEntryViewModel>()));
                        }
                        entriesByDeclaringType[entry.DeclaredIn.Id.Id].Item2.Add(entry);
                    }
                }
            }

            var groups = new List<TableOfContentsEntryGroupViewModel>();
            BuildTocGroupsFrom(inheritance.Root, entriesByDeclaringType, groups);
            groups.Reverse(); // Get the closest ancestors first in the list

            // If we have root items, add them first
            if (rootGroupItems.Any())
            {
                groups.Insert(0, new TableOfContentsEntryGroupViewModel(null, rootGroupItems));
            }

            return groups;
        }

        private void BuildTocGroupsFrom(InheritanceNodeViewModel ancestor, Dictionary<string, Tuple<DocumentReferenceViewModel, List<TableOfContentsEntryViewModel>>> entriesByDeclaringType, List<TableOfContentsEntryGroupViewModel> target)
        {
            var key = ancestor.Uri;
            if (entriesByDeclaringType.ContainsKey(key))
            {
                var entryInfo = entriesByDeclaringType[key];
                var group = new TableOfContentsEntryGroupViewModel(entryInfo.Item1, entryInfo.Item2);
                target.Add(group);
            }

            // Add groups from descendants
            foreach (var child in ancestor.Children)
            {
                BuildTocGroupsFrom(child, entriesByDeclaringType, target);
            }
        }
    }
}