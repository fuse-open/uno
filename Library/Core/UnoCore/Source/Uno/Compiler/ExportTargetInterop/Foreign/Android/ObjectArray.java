package com.uno;
import com.uno.UnoObject;

public class ObjectArray extends UnoObject
{
    public ObjectArray(int length) {
        super(com.Bindings.ExternedBlockHost.newObjectArrayPtr(length));
    }

    private ObjectArray(long ptr) {
        super(ptr);
    }

    public ObjectArray(UnoObject[] arr)
    {
        super(com.Bindings.ExternedBlockHost.newObjectArrayPtr(arr.length));
        int len = arr.length;
        for (int i=0; i<len; i++)
            set(i, arr[i]);
    }

    public static ObjectArray InitFromUnoPtr(long ptr)
    {
        return new ObjectArray(ptr);
    }

    public UnoObject get(int index) {
        return (UnoObject)com.Bindings.ExternedBlockHost.getObject(this, index);
    }

    public UnoObject set(int index, UnoObject val) {
        return (UnoObject)com.Bindings.ExternedBlockHost.setObject(this, index, val);
    }

    public UnoObject[] copyArray()
    {
        return (UnoObject[])com.Bindings.ExternedBlockHost.copyObjectArray(this);
    }
}
