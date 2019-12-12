using Uno.Compiler.ExportTargetInterop;
using Android.Base.Types;
using Android.Base.Wrappers;
using Android.Base.Primitives;

namespace Uno.Compiler.ExportTargetInterop
{
    [ForeignTypeName("::NSData*")]
    [ForeignInclude(Language.ObjC, "Foundation/Foundation.h")]
    public extern(FOREIGN_OBJC_SUPPORTED) class ForeignDataView : ObjC.Object
    {
        ForeignDataView() { }

        ForeignDataView(byte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Handle = CreateNSDataFromByteArray(extern<IntPtr>(data) "$0").Handle;
        }

        ForeignDataView(sbyte[] data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            Handle = CreateNSDataFromByteArray(extern<IntPtr>(data) "$0").Handle;
        }

        public static ForeignDataView Create(byte[] unoArray)
        {
            return new ForeignDataView(unoArray);
        }

        public static ForeignDataView Create(sbyte[] unoArray)
        {
            return new ForeignDataView(unoArray);
        }

        [Foreign(Language.ObjC)]
        static ObjC.Object CreateNSDataFromByteArray(IntPtr rawUnoArray)
        @{
            auto unoArray = (\@{byte[]})rawUnoArray;
            ::uRetain(unoArray);
            return [[::NSData alloc]
                initWithBytesNoCopy:unoArray->Ptr()
                length:(NSUInteger)unoArray->Length()
                deallocator:^(void* bytes, NSUInteger length)
                {
                    ::uRelease(unoArray);
                }];
         @}
    }

    public extern(Android) static class ForeignDataView
    {
        // The following methods data uno data and return a direct Java ByteBuffer
        // The advantage of using these methods is that Java will hold a reference to the
        // original Uno data so that its freed whilst java is using it.

        public static Java.Object Create(byte[] unoArray)
        {
            if (unoArray == null)
                throw new ArgumentNullException(nameof(unoArray));
            return CreateInner(ByteBuffer.NewDirectByteBuffer(unoArray), unoArray);
        }

        public static Java.Object Create(sbyte[] unoArray)
        {
            if (unoArray == null)
                throw new ArgumentNullException(nameof(unoArray));
            return CreateInner(ByteBuffer.NewDirectByteBuffer(unoArray), unoArray);
        }

        static Java.Object CreateInner(ujobject directBuffer, object unoObj)
        {
            if (!ByteBuffer.ValidDirectByteBuffer(directBuffer))
                throw new Exception("ForeignDataView failed to create a direct buffer");
            var wrappedBuffer = JWrapper.Wrap(directBuffer);
            var result = CreateInnerJava(wrappedBuffer, unoObj);
            if (result == null)
                throw new Exception("ForeignDataView was unable to create a Java view to the data");
            return result;
        }

        [Foreign(Language.Java)]
        static Java.Object CreateInnerJava(Java.Object directBuffer, object unoObj)
        @{
            java.nio.ByteBuffer buf = (java.nio.ByteBuffer)directBuffer;
            try
            {
                return new com.uno.UnoBackedByteBuffer(buf, unoObj);
            }
            catch (Exception e)
            {
                return null; // this is unfortunate but we need to handle java's checked exceptions
                             // before we can fix this properly
            }
        @}
    }
}
