using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Uno.Build.Stuff;
using Uno.IO;
using Uno.Logging;
using Uno.ProjectFormat;

namespace Uno.Build
{
    public class ProjectCleaner : DiskObject
    {
        readonly HashSet<string> _cleanedProjects = new HashSet<string>();

        public ProjectCleaner(Log log)
            : base(log)
        {
        }

        public void Clean(IEnumerable<string> files)
        {
            foreach (var f in files)
                Clean(f);
        }

        public void Clean(string filename)
        {
            Log.Verbose("Cleaning " + filename.ToRelativePath());

            try
            {
                Clean(Project.Load(filename));
            }
            catch (Exception e)
            {
                Log.Trace(e);
                Log.Error(e.Message);
            }
        }

        public void Clean(Project project)
        {
            if (project == null)
                throw new ArgumentNullException(nameof(project));

            if (_cleanedProjects.Contains(project.FullPath))
                return;

            _cleanedProjects.Add(project.FullPath);

            Disk.DeleteDirectory(project.BuildDirectory);
            Disk.DeleteDirectory(project.CacheDirectory);

            // Remove files installed by stuff
            Installer.CleanAll(Log,
                project.StuffFiles.Select(
                    x => Path.Combine(project.RootDirectory, x.NativePath)));

            foreach (var pref in project.ProjectReferences)
                Clean(Project.Load(Path.Combine(project.RootDirectory, pref.ProjectPath)));
        }
    }
}
