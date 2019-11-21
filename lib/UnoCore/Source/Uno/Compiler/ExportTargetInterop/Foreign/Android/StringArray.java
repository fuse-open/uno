package com.uno;
import com.uno.UnoObject;

public class StringArray extends UnoObject
{
    public StringArray(int length) {
        super(com.foreign.ExternedBlockHost.newStringArrayPtr(length));
    }

    private StringArray(long ptr) {
        super(ptr);
    }

    public StringArray(String[] arr)
    {
        super(com.foreign.ExternedBlockHost.newStringArrayPtr(arr.length));
        int len = arr.length;
        for (int i=0; i<len; i++)
            set(i, arr[i]);
    }

    public static StringArray InitFromUnoPtr(long ptr)
    {
        return new StringArray(ptr);
    }

    public String get(int index) {
        return com.foreign.ExternedBlockHost.getString(this, index);
    }

    public String set(int index, String val) {
        return com.foreign.ExternedBlockHost.setString(this, index, val);
    }

    public String[] copyArray()
    {
        return (String[])com.foreign.ExternedBlockHost.copyStringArray(this);
    }
}
