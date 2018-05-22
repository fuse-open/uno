package com.uno;
import com.uno.UnoObject;

public class LongArray extends UnoObject
{
    public LongArray(int length) {
        super(com.Bindings.ExternedBlockHost.newLongArrayPtr(length));
    }

    private LongArray(long ptr) {
        super(ptr);
    }

    public LongArray(long[] arr) {
        super(com.Bindings.ExternedBlockHost.longArrayToUnoArrayPtr(arr));
    }

    public static LongArray InitFromUnoPtr(long ptr)
    {
        return new LongArray(ptr);
    }

    public long get(int index) {
        return com.Bindings.ExternedBlockHost.getLong(this, index);
    }

    public long set(int index, long val) {
        return com.Bindings.ExternedBlockHost.setLong(this, index, val);
    }

    public long[] copyArray()
    {
        return (long[])com.Bindings.ExternedBlockHost.copyLongArray(this);
    }
}
