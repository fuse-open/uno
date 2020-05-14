using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Uno.Logging;

namespace Uno.Diagnostics
{
    public static class ProcessExtensions
    {
        private static string FindIndexedProcessName(int pid)
        {
            try
            {
                var processName = Process.GetProcessById(pid).ProcessName;
                var processesByName = Process.GetProcessesByName(processName);

                for (var index = 0; index < processesByName.Length; index++)
                {
                    var indexedName = index == 0 ? processName : processName + "#" + index;
                    var processId = new PerformanceCounter("Process", "ID Process", indexedName);
                    if ((int)processId.NextSample().RawValue == pid)
                        return indexedName;
                }
            }
            catch (InvalidOperationException e)
            {
                Log.Default.Warning(
                    "Exception in FindIndexedProcessName(): " + e.Message + "\n\n" + 
                    "This may be resolved when resetting performance counters in Windows.\n" + 
                    "To do this type the following in cmd.exe:\n" +
                    "    lodctr /R\n" +
                    "    lodctr \"C:\\Windows\\Microsoft.NET\\Framework64\\v4.0.20506\\corperfmonsymbols.ini\"\n" +
                    "(Source: http://stackoverflow.com/questions/1540777/performancecounters-on-net-4-0-windows-7 )\n");
            }

            return null;
        }

        public static Process GetParent(this Process process)
        {
            var indexedProcessName = FindIndexedProcessName(process.Id);

            if (indexedProcessName == null)
                return null;

            var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
            var pid = parentId.NextSample().RawValue;
            return Process
                .GetProcesses()
                .FirstOrDefault(
                    p =>
                    {
                        try
                        {
                            return p.Id == pid && p.StartTime <= process.StartTime;
                        }
                        catch
                        {
                            return false;
                        }
                    });
        }

        public static List<Process> GetParents(this Process process)
        {
            var parents = new List<Process>();
            var parent = process.GetParent();

            while (parent != null)
            {
                parents.Add(parent);
                parent = parent.GetParent();
            }

            return parents;
        }

        public static void KillTree(this Process process)
        {
            ProcessTreeKiller.KillTree(process.Id);
        }
    }
}
