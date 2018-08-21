using Uno.Collections;
using Uno.IO;

namespace Uno.Diagnostics
{
    [Obsolete]
    public static class ProfileSerializer
    {
        public static void Serialize(ProfileData data, MemoryStream stream)
        {
            var writer = new BinaryWriter(stream);
            writer.Write(data.FunctionIds.Count);
            foreach (var functionId in data.FunctionIds)
            {
                writer.Write(functionId);
            }
            writer.Write(data.ProfileEvents.Count);
            foreach (var profileEvent in data.ProfileEvents)
            {
                switch (profileEvent.Type)
                {
                    case EventType.Enter:
                        Serialize(writer, (profileEvent as EnterEvent));
                        break;
                    case EventType.Exit:
                        Serialize(writer, (profileEvent as ExitEvent));
                        break;
                    case EventType.Allocate:
                        Serialize(writer, (profileEvent as AllocateEvent));
                        break;
                    case EventType.Free:
                        Serialize(writer, (profileEvent as FreeEvent));
                        break;
                    default:
                        throw new Exception("Internal error: Could not serialize event of type " + profileEvent.Type);
                }
            }
        }

        public static void Serialize(BinaryWriter writer, ExitEvent exit)
        {
            writer.Write((byte)exit.Type);
            writer.Write(exit.TimeStamp);
        }

        public static void Serialize(BinaryWriter writer, EnterEvent enter)
        {
            writer.Write((byte)enter.Type);
            writer.Write(enter.TimeStamp);
            writer.Write(enter.Id);
        }

        public static void Serialize(BinaryWriter writer, AllocateEvent allocate)
        {
            writer.Write((byte)allocate.Type);
            writer.Write(allocate.TimeStamp);
            writer.Write(allocate.Class);
            writer.Write(allocate.Id);
            writer.Write(allocate.Weight);
        }

        public static void Serialize(BinaryWriter writer, FreeEvent free)
        {
            writer.Write((byte)free.Type);
            writer.Write(free.TimeStamp);
            writer.Write(free.Class);
            writer.Write(free.Id);
        }
    }
}
