using System;
using System.Diagnostics;
using System.IO;
using System.Management;

namespace Uno.Diagnostics
{
    public static class ProcessTreeKiller
    {
        public static void KillTree(int pid)
        {
            if (PlatformDetection.IsWindows)
            {
                KillWindowsTree(pid);
            }
            else if (PlatformDetection.IsMac)
            {
                KillOsxTree(pid);
            }
            else
            {
                throw new Exception("Killing process trees is only supported on Windows and macOS");
            }
        }

        //http://stackoverflow.com/a/10402906/7084
        private static void KillWindowsTree(int pid)
        {
            var searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            foreach (ManagementObject mo in searcher.Get())
            {
                KillWindowsTree(Convert.ToInt32(mo["ProcessID"]));
            }
            try
            {
                // This is unsafe: the PID of either the child or the parent may have been reused by
                // the system. A better approach would be to put the child-processes in job-objects:
                //
                // https://msdn.microsoft.com/en-us/library/windows/desktop/ms684161(v=vs.85).aspx

                Process.GetProcessById(pid).Kill();
            }
            catch
            {
                // Process already exited.
            }
        }

        private static void KillOsxTree(int pid)
        {
            const string scriptPath = "/tmp/uno_kill_script.sh";
            File.WriteAllText(scriptPath, (OsxKillScript.Replace("PID_GOES_HERE", pid.ToString())).Replace("\r", ""));
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    FileName = "sh",
                    Arguments = scriptPath,
                }
            };
            p.Start();
        }

        private const string OsxKillScript = @"#!/bin/sh

kill_tree() {
    ps -o pid,ppid | while read line; do
        pid=`echo $line | awk '{print $1}'`
        ppid=`echo $line | awk '{print $2}'`
        if [ $ppid == $1 ]; then
            kill_tree $pid
        fi
    done
    kill -9 $1 2>/dev/null
}

kill_orphan_uno_process()
{
    RUN_SH_PID=`ps -o ppid,pid,command | grep -e '^\ *1.*run.sh$' | awk '{print $2}'`
    if [ -z ""$RUN_SH_PID"" ]; then exit; fi
    UNO_PROCESS_PID=`ps -o pid,ppid | grep -e ""$RUN_SH_PID$"" | awk '{print $1}'`
    echo 'Uno process gone. Killing '$UNO_PROCESS_PID
    sleep 10
    kill -9 $UNO_PROCESS_PID 2>/dev/null
}

kill_tree PID_GOES_HERE

if [ $? != 0 ]; then
    kill_orphan_uno_process
fi
";
    }
}
