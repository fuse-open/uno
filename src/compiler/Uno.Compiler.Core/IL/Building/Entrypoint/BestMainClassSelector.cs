using System;
using System.Collections.Generic;
using System.Linq;
using Uno.Compiler.API.Domain.IL;
using Uno.Logging;

namespace Uno.Compiler.Core.IL.Building.Entrypoint
{
    public class BestMainClassSelector
    {
        readonly List<DataType> _foundMainClasses;
        readonly Log _log;
        readonly SourcePackage _mainProject;
        readonly DataType _mainClassAttribute;

        BestMainClassSelector(List<DataType> foundMainClasses, Log log, SourcePackage mainProject, DataType mainClassAttribute)
        {
            _foundMainClasses = foundMainClasses;
            _log = log;
            _mainProject = mainProject;
            _mainClassAttribute = mainClassAttribute;
        }

        public static DataType GetBestMainClass(List<DataType> foundMainClasses, Log log, SourcePackage mainProject, DataType mainClassAttribute)
        {
            return new BestMainClassSelector(foundMainClasses, log, mainProject, mainClassAttribute).GetBestMainClass();
        }

        DataType GetBestMainClass()
        {
            var attributedMainClass = TryGetAttributedMainClass();
            if (attributedMainClass != null)
                return attributedMainClass;

            var mainClass = TryGetMainClass();
            if (mainClass != null)
                return mainClass;

            ReportNoUsableMainClass();
            return null;
        }

        DataType TryGetAttributedMainClass()
        {
            return PickWithPriorityToStartupProject(
                _foundMainClasses.Where(c => c.HasAttribute(_mainClassAttribute)).ToList(),
                ReportMultipleMainClassAttributedClasses);
        }

        DataType TryGetMainClass()
        {
            return PickWithPriorityToStartupProject(
                _foundMainClasses.Where(HasPublicDefaultConstructor).ToList(),
                ReportMultipleNonAbstractApplicationClasses);
        }

        DataType PickWithPriorityToStartupProject(List<DataType> candidates, Action<DataType> warningFunction)
        {
            if (candidates.Count == 1)
            {
                return candidates[0];
            }
            if (candidates.Count > 1)
            {
                var inStartupProject = candidates.Where(c => Equals(c.Source.Package, _mainProject)).ToList();
                if (inStartupProject.Count == 0)
                {
                    warningFunction(candidates[0]);
                    return candidates[0];
                }
                if (inStartupProject.Count == 1)
                {
                    return inStartupProject[0];
                }
                if (inStartupProject.Count > 1)
                {
                    warningFunction(candidates[0]);
                    return inStartupProject[0];
                }
            }
            return null;
        }

        static bool HasPublicDefaultConstructor(DataType dt)
        {
            var ctor = dt.TryGetDefaultConstructor();
            return ctor != null && ctor.IsPublic && ctor.IsConstructor;
        }

        void ReportMultipleMainClassAttributedClasses(DataType e)
        {
            _log.Warning(
                _mainProject.Source,
                ErrorCode.W0000,
                "Multiple classes with the '[MainClass]' attribute was found. Defaulting to '" + e + "'");
        }

        void ReportMultipleNonAbstractApplicationClasses(DataType e)
        {
            _log.Warning(
                _mainProject.Source,
                ErrorCode.W0000,
                "Multiple non-abstract application classes was found in project. Defaulting to '" + e + "'");
        }
        void ReportNoUsableMainClass()
        {
            _log.Error(
                _mainProject.Source,
                ErrorCode.E3504,
                "Multiple non-abstract application classes was found in project, but none with a public parameterless constructor");
        }
    }
}