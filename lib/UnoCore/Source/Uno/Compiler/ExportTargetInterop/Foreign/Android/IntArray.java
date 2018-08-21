package com.uno;
import com.uno.UnoObject;

public class IntArray extends UnoObject
{
    public IntArray(int length) {
        super(com.Bindings.ExternedBlockHost.newIntArrayPtr(length));
    }

    private IntArray(long ptr) {
        super(ptr);
    }

    public IntArray(int[] arr) {
        super(com.Bindings.ExternedBlockHost.intArrayToUnoArrayPtr(arr));
    }

    public static IntArray InitFromUnoPtr(long ptr)
    {
        return new IntArray(ptr);
    }

    public int get(int index) {
        return com.Bindings.ExternedBlockHost.getInt(this, index);
    }

    public int set(int index, int val) {
        return com.Bindings.ExternedBlockHost.setInt(this, index, val);
    }

    public int[] copyArray()
    {
        return (int[])com.Bindings.ExternedBlockHost.copyIntArray(this);
    }
}
