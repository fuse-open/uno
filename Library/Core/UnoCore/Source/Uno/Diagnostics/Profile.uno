using Uno.Compiler.ExportTargetInterop;
using Uno.Runtime.Implementation;
using Uno.Collections;
using Uno.IO;
using Uno.Platform;

namespace Uno.Diagnostics
{
    [Obsolete]
    public sealed class AlwaysProfileAttribute: Attribute {}
    [Obsolete]
    public sealed class NeverProfileAttribute: Attribute {}

    [Obsolete]
    public static class Profile
    {
        private static bool _insideProfile;
        private static ProfileData _profileData;
        private static string _appName;
        private static bool _registeredEvents;
        private static bool _isProfiling;
        private static string _timeStampString;
        public static double StartTimeStamp;

        public static void Init()
        {
            _profileData = new ProfileData();
            _isProfiling = true;
            var now = Uno.DateTime.Now;
            _timeStampString = String.Format("{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}",
                                             now.Year, now.Month, now.Day,
                                             now.Hour, now.Minute);
            StartTimeStamp = Clock.GetSeconds();
        }

        public static void AddFunctionId(string name)
        {
            _profileData.FunctionIds.Add(name);
        }

        public static void Enter(int functionId)
        {
            if (!_isProfiling || _insideProfile)
                return;
            _insideProfile = true;
            _profileData.ProfileEvents.Add(new EnterEvent(functionId));
            _insideProfile = false;
        }

        public static void Exit()
        {
            if (!_isProfiling || _insideProfile)
                return;
            _insideProfile = true;
            _profileData.ProfileEvents.Add(new ExitEvent());
            _insideProfile = false;
        }

        public static void Allocate(string type, object obj, int weight)
        {
            _profileData.ProfileEvents.Add(
                new AllocateEvent(_profileData.TypeMap.IdFor(type), 0, weight));
        }

        public static void Free(string type, object obj)
        {
            _profileData.ProfileEvents.Add(
                new FreeEvent(_profileData.TypeMap.IdFor(type), 0));
        }

        public static void Serialize(string filename)
        {
            _isProfiling = false;
            var stream = new MemoryStream();
            ProfileSerializer.Serialize(_profileData, stream);

            filename = filename + "." + _timeStampString + ".data";
            Write(filename, stream.GetBuffer(), 0, (int)stream.Length);
        }

        [extern(CPLUSPLUS) Require("Source.Include", "uBase/Path.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "uBase/Disk.h")]
        [extern(CPLUSPLUS) Require("Source.Include", "Uno/Support.h")]
        public static void Write(string fileBaseName, byte[] bytes, int offset, int count)
        {
            if defined(CPLUSPLUS)
            @{
                uBase::String baseName = uStringToXliString($0);
                uBase::String fileName = uBase::Path::Combine("/sdcard", baseName);
                uBase::File file(fileName, uBase::FileModeWrite);
                void* ptr = $1->Ptr();
                int byteCount = $3;
                file.Write(ptr, sizeof(unsigned char), byteCount);
                file.Close();
            @}
            else if defined(DOTNET)
            {
                var myDocuments = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                var profileDir = Path.Combine(myDocuments, "Realtime Studio", "ProfileData");
                Directory.CreateDirectory(profileDir);
                using (var fs = File.Open(Path.Combine(profileDir, fileBaseName), FileMode.Create))
                {
                    fs.Write(bytes, offset, count);
                }
            }
            else if defined(JAVASCRIPT)
            @{
                /* TODO */
            @}
            else
                build_error;
        }

        public static void AppTerminating(ApplicationState newState)
        {
            Serialize(_appName);
        }

        public static void RegisterClosingEvent(object ignored, string appName)
        {
            if (_registeredEvents)
                return;
            _appName = appName;
            CoreApp.Terminating += AppTerminating;
            _registeredEvents = true;
        }
    }
}
