package com.uno;
import com.uno.UnoObject;

public class DoubleArray extends UnoObject
{
    public DoubleArray(int length) {
        super(com.Bindings.ExternedBlockHost.newDoubleArrayPtr(length));
    }

    private DoubleArray(long ptr) {
        super(ptr);
    }

    public DoubleArray(double[] arr) {
        super(com.Bindings.ExternedBlockHost.doubleArrayToUnoArrayPtr(arr));
    }

    public static DoubleArray InitFromUnoPtr(long ptr)
    {
        return new DoubleArray(ptr);
    }

    public double get(int index) {
        return com.Bindings.ExternedBlockHost.getDouble(this, index);
    }

    public double set(int index, double val) {
        return com.Bindings.ExternedBlockHost.setDouble(this, index, val);
    }

    public double[] copyArray()
    {
        return (double[])com.Bindings.ExternedBlockHost.copyDoubleArray(this);
    }
}
