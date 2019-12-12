package com.uno;
import com.uno.UnoObject;

public class ByteArray extends UnoObject
{
    boolean _unoIsUnsigned = false;

    public ByteArray(int length) {
        this(length, false);
    }

    public ByteArray(int length, boolean unoIsUnsigned) {
        super(com.foreign.ExternedBlockHost.newByteArrayPtr(length, unoIsUnsigned));
        _unoIsUnsigned = unoIsUnsigned;
    }

    private ByteArray(long ptr, boolean unoIsUnsigned) {
        super(ptr);
        _unoIsUnsigned = unoIsUnsigned;
    }

    public ByteArray(byte[] arr) {
        super(com.foreign.ExternedBlockHost.byteArrayToUnoArrayPtr(arr));
    }

    public static ByteArray InitFromUnoPtr(long ptr, boolean unoIsUnsigned)
    {
        return new ByteArray(ptr, unoIsUnsigned);
    }

    public byte get(int index) {
        if (_unoIsUnsigned)
            return com.foreign.ExternedBlockHost.getUByte(this, index);
        else
            return com.foreign.ExternedBlockHost.getByte(this, index);
    }

    public byte set(int index, byte val) {
        if (_unoIsUnsigned)
            return com.foreign.ExternedBlockHost.setUByte(this, index, val);
        else
            return com.foreign.ExternedBlockHost.setByte(this, index, val);
    }

    public byte[] copyArray()
    {
        if (_unoIsUnsigned)
            return (byte[])com.foreign.ExternedBlockHost.copyUByteArray(this);
        else
            return (byte[])com.foreign.ExternedBlockHost.copyByteArray(this);
    }
}
