package com.uno;

import java.io.Closeable;
import java.nio.ByteOrder;
import java.nio.Buffer;
import java.nio.ByteBuffer;
import java.nio.CharBuffer;
import java.nio.DoubleBuffer;
import java.nio.FloatBuffer;
import java.nio.IntBuffer;
import java.nio.LongBuffer;
import java.nio.ShortBuffer;

// Original Goal: Use data from uno arrays in java without copying
//
// Original Solution: Use jni's NewDirectByteBuffer
//
// Solution's Problem: Must keep reference to uno array until done with the direct ByteBuffer.
//                     Maintaining that relationship can become complex.
//
// This Solution: Hold onto the uno array from the java side
//
// Issues:
//
// 1. Even though ByteBuffer is abstract is is not possible to extend it:
//    https://stackoverflow.com/questions/624458/extending-bytebuffer-class
//    https://stackoverflow.com/questions/26577827/java-unable-to-extend-bytebuffer-class
//
// 2. Whilst you can implement Buffer in java it has no 'get' method so you have to add that
//    yourself. At that point you have a non-standard class again. Due to this we chose to
//    match ByteBuffer 'method for method' even though we could actually subclass it.
//
// 3. Due to 2, we have to implement our own InputStream that takes a UnoBackedByteBuffer. The
//    good news is that we dont have to jump through so many hoops for InputStream and thus it
//    can be used with the rest of the standard library without issue.

public class UnoBackedByteBuffer implements Closeable, Comparable<ByteBuffer>
{
    private Object _unoRef;
    private ByteBuffer _buf;
    public UnoBackedByteBuffer(ByteBuffer buf, Object unoRef) throws Exception {
        if (!buf.isDirect())
            throw new Exception("UnoBackedByteBuffer: Buffer provided was not direct");
        _unoRef = unoRef;
        _buf = buf;
    }
    // Buffer
    //
    public Buffer clear() { return _buf.clear(); }
    public Buffer flip() { return _buf.flip(); }
    public Buffer limit(int newLimit) { return _buf.limit(newLimit); }
    public Buffer mark() { return _buf.mark(); }
    public Buffer position(int newPosition) { return _buf.position(newPosition); }
    public Buffer reset() { return _buf.reset(); }
    public Buffer rewind() { return _buf.rewind(); }
    public ByteBuffer asReadOnlyBuffer() { return _buf.asReadOnlyBuffer(); }
    public ByteBuffer compact() { return _buf.compact(); }
    public ByteBuffer duplicate() { return _buf.duplicate(); }
    public ByteBuffer get(byte[] dst) { return _buf.get(dst); }
    public ByteBuffer get(byte[] dst, int offset, int length) { return _buf.get(dst, offset, length); }
    public ByteBuffer order(ByteOrder bo) { return _buf.order(bo); }
    public ByteBuffer put(ByteBuffer src) { return _buf.put(src); }
    public ByteBuffer put(byte b) { return _buf.put(b); }
    public ByteBuffer put(byte[] src) { return _buf.put(src); }
    public ByteBuffer put(byte[] src, int offset, int length) { return _buf.put(src, offset, length); }
    public ByteBuffer put(int index, byte b) { return _buf.put(index, b); }
    public ByteBuffer putChar(char value) { return _buf.putChar(value); }
    public ByteBuffer putChar(int index, char value) { return _buf.putChar(index, value); }
    public ByteBuffer putDouble(double value) { return _buf.putDouble(value); }
    public ByteBuffer putDouble(int index, double value) { return _buf.putDouble(index, value); }
    public ByteBuffer putFloat(float value) { return _buf.putFloat(value); }
    public ByteBuffer putFloat(int index, float value) { return _buf.putFloat(index, value); }
    public ByteBuffer putInt(int index, int value) { return _buf.putInt(index, value); }
    public ByteBuffer putInt(int value) { return _buf.putInt(value); }
    public ByteBuffer putLong(int index, long value) { return _buf.putLong(index, value); }
    public ByteBuffer putLong(long value) { return _buf.putLong(value); }
    public ByteBuffer putShort(int index, short value) { return _buf.putShort(index, value); }
    public ByteBuffer putShort(short value) { return _buf.putShort(value); }
    public ByteBuffer slice() { return _buf.slice(); }
    public ByteOrder order() { return _buf.order(); }
    public CharBuffer asCharBuffer() { return _buf.asCharBuffer(); }
    public DoubleBuffer asDoubleBuffer() { return _buf.asDoubleBuffer(); }
    public FloatBuffer asFloatBuffer() { return _buf.asFloatBuffer(); }
    public IntBuffer asIntBuffer() { return _buf.asIntBuffer(); }
    public LongBuffer asLongBuffer() { return _buf.asLongBuffer(); }
    public ShortBuffer asShortBuffer() { return _buf.asShortBuffer(); }
    public String toString() { return _buf.toString(); }
    public boolean equals(Object ob) { return _buf.equals(ob); }
    public boolean hasArray() { return _buf.hasArray(); }
    public boolean hasRemaining() { return _buf.hasRemaining(); }
    public boolean isDirect() { return _buf.isDirect(); }
    public boolean isReadOnly() { return _buf.isReadOnly(); }
    public byte get() { return _buf.get(); }
    public byte get(int index) { return _buf.get(index); }
    public byte[] array() { return _buf.array(); }
    public char getChar() { return _buf.getChar(); }
    public char getChar(int index) { return _buf.getChar(index); }
    public double getDouble() { return _buf.getDouble(); }
    public double getDouble(int index) { return _buf.getDouble(index); }
    public float getFloat() { return _buf.getFloat(); }
    public float getFloat(int index) { return _buf.getFloat(index); }
    public int arrayOffset() { return _buf.arrayOffset(); }
    public int capacity() { return _buf.capacity(); }
    public int compareTo(ByteBuffer that) { return _buf.compareTo(that); }
    public int getInt() { return _buf.getInt(); }
    public int getInt(int index) { return _buf.getInt(index); }
    public int hashCode() { return _buf.hashCode(); }
    public int limit() { return _buf.limit(); }
    public int position() { return _buf.position(); }
    public int remaining() { return _buf.remaining(); }
    public long getLong() { return _buf.getLong(); }
    public long getLong(int index) { return _buf.getLong(index); }
    public short getShort() { return _buf.getShort(); }
    public short getShort(int index) { return _buf.getShort(index); }

    // Closable
    public void close ()
    {
        _unoRef = null;
        _buf = null;
    }
}
