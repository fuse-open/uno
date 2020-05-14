using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Uno.ProjectFormat
{
    static class UnoprojSerializer
    {
        public static void Serialize(UnoprojDocument project, string filename)
        {
            using (var f = File.Create(filename))
                Serialize(project, f);
        }

        public static void Serialize(UnoprojDocument project, Stream stream)
        {
            var json = new Dictionary<string, object>();

            foreach (var e in project.Properties)
            {
                var parts = e.Key.Split('.');
                var key = parts.Last();
                var parent = json;

                for (int i = 0; i < parts.Length - 1; i++)
                {
                    object obj;
                    if (!parent.TryGetValue(parts[i], out obj))
                    {
                        obj = new Dictionary<string, object>();
                        parent.Add(parts[i], obj);
                    }

                    parent = (Dictionary<string, object>) obj;
                }

                bool boolValue;
                int intValue;
                if (e.Value.String == null)
                    parent[key] = null;
                else if (bool.TryParse(e.Value.String, out boolValue))
                    parent[key] = boolValue;
                else if (int.TryParse(e.Value.String, out intValue))
                    parent[key] = intValue;
                else if (parent != json && e.Value.String.IndexOf('\n') != -1)
                    parent[key] = e.Value.String.Split('\n');
                else
                    parent[key] = e.Value.String;
            }

            if (project.OptionalInternalsVisibleTo != null)
            {
                var internals = new List<string>();

                foreach (var e in project.OptionalInternalsVisibleTo)
                    internals.Add(e.String);

                json["InternalsVisibleTo"] = internals;
            }

            if (project.OptionalPackages != null)
            {
                var packages = new List<string>();

                foreach (var e in project.OptionalPackages)
                    packages.Add(e.ToString());

                json["Packages"] = packages;
            }

            if (project.OptionalProjects != null)
            {
                var projects = new List<string>();

                foreach (var e in project.OptionalProjects)
                    projects.Add(e.ToString());

                json["Projects"] = projects;
            }

            if (project.Includes != null)
            {
                var includes = new List<string>();

                foreach (var e in project.Includes)
                    includes.Add(e.ToString());

                json["Includes"] = includes;
            }

            if (project.OptionalExcludes != null)
            {
                var excludes = new List<string>();

                foreach (var e in project.OptionalExcludes)
                    excludes.Add(e.String.NativeToUnix());

                json["Excludes"] = excludes;
            }

            var w = new StreamWriter(stream);

            try
            {
                new JsonSerializer() { Formatting = Formatting.Indented }
                    .Serialize(w, json);
            }
            finally
            {
                w.Flush();
            }
        }
    }
}
