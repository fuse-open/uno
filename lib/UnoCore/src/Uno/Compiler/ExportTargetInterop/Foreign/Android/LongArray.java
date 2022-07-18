package com.uno;
import com.uno.UnoObject;

public class LongArray extends UnoObject
{
    public LongArray(int length) {
        super(com.foreign.ExternedBlockHost.newLongArrayPtr(length));
    }

    private LongArray(long ptr) {
        super(ptr);
    }

    public LongArray(long[] arr) {
        super(com.foreign.ExternedBlockHost.longArrayToUnoArrayPtr(arr));
    }

    public static LongArray InitFromUnoPtr(long ptr)
    {
        return new LongArray(ptr);
    }

    public long get(int index) {
        return com.foreign.ExternedBlockHost.getLong(this, index);
    }

    public long set(int index, long val) {
        return com.foreign.ExternedBlockHost.setLong(this, index, val);
    }

    public long[] copyArray()
    {
        return (long[])com.foreign.ExternedBlockHost.copyLongArray(this);
    }
}
