using Uno.Logging;

namespace Uno.IO
{
    public class DiskObject : LogObject
    {
        protected readonly Disk Disk;

        public DiskObject(Log log)
            : this(new Disk(log ?? Log.Default))
        {
        }

        public DiskObject(Disk disk)
            : base(disk?.Log)
        {
            Disk = disk ?? Disk.Default;
        }

        public DiskObject(DiskObject obj)
            : base(obj?.Log)
        {
            Disk = obj?.Disk ?? Disk.Default;
        }
    }
}