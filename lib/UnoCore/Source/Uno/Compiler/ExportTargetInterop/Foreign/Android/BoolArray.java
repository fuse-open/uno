package com.uno;
import com.uno.UnoObject;

public class BoolArray extends UnoObject
{
    public BoolArray(int length) {
        super(com.Bindings.ExternedBlockHost.newBoolArrayPtr(length));
    }

    private BoolArray(long ptr) {
        super(ptr);
    }

    public BoolArray(boolean[] arr) {
        super(com.Bindings.ExternedBlockHost.boolArrayToUnoArrayPtr(arr));
    }

    public static BoolArray InitFromUnoPtr(long ptr)
    {
        return new BoolArray(ptr);
    }

    public boolean get(int index) {
        return com.Bindings.ExternedBlockHost.getBool(this, index);
    }

    public boolean set(int index, boolean val) {
        return com.Bindings.ExternedBlockHost.setBool(this, index, val);
    }

    public boolean[] copyArray()
    {
        return (boolean[])com.Bindings.ExternedBlockHost.copyBoolArray(this);
    }
}
