package com.uno;
import com.uno.UnoObject;

public class FloatArray extends UnoObject
{
    public FloatArray(int length) {
        super(com.foreign.ExternedBlockHost.newFloatArrayPtr(length));
    }

    private FloatArray(long ptr) {
        super(ptr);
    }

    public FloatArray(float[] arr) {
        super(com.foreign.ExternedBlockHost.floatArrayToUnoArrayPtr(arr));
    }

    public static FloatArray InitFromUnoPtr(long ptr)
    {
        return new FloatArray(ptr);
    }

    public float get(int index) {
        return com.foreign.ExternedBlockHost.getFloat(this, index);
    }

    public float set(int index, float val) {
        return com.foreign.ExternedBlockHost.setFloat(this, index, val);
    }

    public float[] copyArray()
    {
        return (float[])com.foreign.ExternedBlockHost.copyFloatArray(this);
    }
}
