package com.fuse;
import java.lang.reflect.Field;

public final class R
{
    public static int get(String path)
    {
        try {
            int lastDot = path.lastIndexOf(".", path.length()-1);
            Class<?> cls = Class.forName(@(Activity.Package).R.class.getName()+"$"+(((String)path.subSequence(0, lastDot)).replace('.','$')));
            Field f = cls.getField((String)path.subSequence(lastDot+1, path.length()));
            return f.getInt(null);
        } catch (Exception e) {
            return -1;
        }
    }
}
