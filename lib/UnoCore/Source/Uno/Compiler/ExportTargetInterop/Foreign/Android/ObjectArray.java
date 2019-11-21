package com.uno;
import com.uno.UnoObject;

public class ObjectArray extends UnoObject
{
    public ObjectArray(int length) {
        super(com.foreign.ExternedBlockHost.newObjectArrayPtr(length));
    }

    private ObjectArray(long ptr) {
        super(ptr);
    }

    public ObjectArray(UnoObject[] arr)
    {
        super(com.foreign.ExternedBlockHost.newObjectArrayPtr(arr.length));
        int len = arr.length;
        for (int i=0; i<len; i++)
            set(i, arr[i]);
    }

    public static ObjectArray InitFromUnoPtr(long ptr)
    {
        return new ObjectArray(ptr);
    }

    public UnoObject get(int index) {
        return (UnoObject)com.foreign.ExternedBlockHost.getObject(this, index);
    }

    public UnoObject set(int index, UnoObject val) {
        return (UnoObject)com.foreign.ExternedBlockHost.setObject(this, index, val);
    }

    public UnoObject[] copyArray()
    {
        return (UnoObject[])com.foreign.ExternedBlockHost.copyObjectArray(this);
    }
}
