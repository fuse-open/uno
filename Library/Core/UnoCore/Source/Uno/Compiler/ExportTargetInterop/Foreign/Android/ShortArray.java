package com.uno;
import com.uno.UnoObject;

public class ShortArray extends UnoObject
{
    public ShortArray(int length) {
        super(com.Bindings.ExternedBlockHost.newShortArrayPtr(length));
    }

    private ShortArray(long ptr) {
        super(ptr);
    }

    private ShortArray(short[] arr) {
        super(com.Bindings.ExternedBlockHost.shortArrayToUnoArrayPtr(arr));
    }

    public static ShortArray InitFromUnoPtr(long ptr)
    {
        return new ShortArray(ptr);
    }

    public short get(int index) {
        return com.Bindings.ExternedBlockHost.getShort(this, index);
    }

    public short set(int index, short val) {
        return com.Bindings.ExternedBlockHost.setShort(this, index, val);
    }

    public short[] copyArray()
    {
        return (short[])com.Bindings.ExternedBlockHost.copyShortArray(this);
    }
}
