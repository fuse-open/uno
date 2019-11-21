package com.uno;
import com.uno.UnoObject;

public class DoubleArray extends UnoObject
{
    public DoubleArray(int length) {
        super(com.foreign.ExternedBlockHost.newDoubleArrayPtr(length));
    }

    private DoubleArray(long ptr) {
        super(ptr);
    }

    public DoubleArray(double[] arr) {
        super(com.foreign.ExternedBlockHost.doubleArrayToUnoArrayPtr(arr));
    }

    public static DoubleArray InitFromUnoPtr(long ptr)
    {
        return new DoubleArray(ptr);
    }

    public double get(int index) {
        return com.foreign.ExternedBlockHost.getDouble(this, index);
    }

    public double set(int index, double val) {
        return com.foreign.ExternedBlockHost.setDouble(this, index, val);
    }

    public double[] copyArray()
    {
        return (double[])com.foreign.ExternedBlockHost.copyDoubleArray(this);
    }
}
