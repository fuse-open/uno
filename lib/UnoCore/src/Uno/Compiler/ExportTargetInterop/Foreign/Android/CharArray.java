package com.uno;
import com.uno.UnoObject;

public class CharArray extends UnoObject
{
    public CharArray(int length) {
        super(com.foreign.ExternedBlockHost.newCharArrayPtr(length));
    }

    private CharArray(long ptr) {
        super(ptr);
    }

    public CharArray(char[] arr) {
        super(com.foreign.ExternedBlockHost.charArrayToUnoArrayPtr(arr));
    }

    public static CharArray InitFromUnoPtr(long ptr)
    {
        return new CharArray(ptr);
    }

    public char get(int index) {
        return com.foreign.ExternedBlockHost.getChar(this, index);
    }

    public char set(int index, char val) {
        return com.foreign.ExternedBlockHost.setChar(this, index, val);
    }

    public char[] copyArray()
    {
        return (char[])com.foreign.ExternedBlockHost.copyCharArray(this);
    }
}
