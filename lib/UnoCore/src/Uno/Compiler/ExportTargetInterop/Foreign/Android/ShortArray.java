package com.uno;
import com.uno.UnoObject;

public class ShortArray extends UnoObject
{
    public ShortArray(int length) {
        super(com.foreign.ExternedBlockHost.newShortArrayPtr(length));
    }

    private ShortArray(long ptr) {
        super(ptr);
    }

    private ShortArray(short[] arr) {
        super(com.foreign.ExternedBlockHost.shortArrayToUnoArrayPtr(arr));
    }

    public static ShortArray InitFromUnoPtr(long ptr)
    {
        return new ShortArray(ptr);
    }

    public short get(int index) {
        return com.foreign.ExternedBlockHost.getShort(this, index);
    }

    public short set(int index, short val) {
        return com.foreign.ExternedBlockHost.setShort(this, index, val);
    }

    public short[] copyArray()
    {
        return (short[])com.foreign.ExternedBlockHost.copyShortArray(this);
    }
}
