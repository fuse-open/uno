package com.uno;
import com.uno.UnoObject;

public class BoolArray extends UnoObject
{
    public BoolArray(int length) {
        super(com.foreign.ExternedBlockHost.newBoolArrayPtr(length));
    }

    private BoolArray(long ptr) {
        super(ptr);
    }

    public BoolArray(boolean[] arr) {
        super(com.foreign.ExternedBlockHost.boolArrayToUnoArrayPtr(arr));
    }

    public static BoolArray InitFromUnoPtr(long ptr)
    {
        return new BoolArray(ptr);
    }

    public boolean get(int index) {
        return com.foreign.ExternedBlockHost.getBool(this, index);
    }

    public boolean set(int index, boolean val) {
        return com.foreign.ExternedBlockHost.setBool(this, index, val);
    }

    public boolean[] copyArray()
    {
        return (boolean[])com.foreign.ExternedBlockHost.copyBoolArray(this);
    }
}
