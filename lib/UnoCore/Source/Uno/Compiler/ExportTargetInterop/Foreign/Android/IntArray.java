package com.uno;
import com.uno.UnoObject;

public class IntArray extends UnoObject
{
    public IntArray(int length) {
        super(com.foreign.ExternedBlockHost.newIntArrayPtr(length));
    }

    private IntArray(long ptr) {
        super(ptr);
    }

    public IntArray(int[] arr) {
        super(com.foreign.ExternedBlockHost.intArrayToUnoArrayPtr(arr));
    }

    public static IntArray InitFromUnoPtr(long ptr)
    {
        return new IntArray(ptr);
    }

    public int get(int index) {
        return com.foreign.ExternedBlockHost.getInt(this, index);
    }

    public int set(int index, int val) {
        return com.foreign.ExternedBlockHost.setInt(this, index, val);
    }

    public int[] copyArray()
    {
        return (int[])com.foreign.ExternedBlockHost.copyIntArray(this);
    }
}
