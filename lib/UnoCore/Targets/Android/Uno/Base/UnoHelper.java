package com.foreign;

import java.util.HashMap;
import java.util.Observable;
import java.util.Observer;
import java.lang.reflect.Field;
import java.lang.reflect.Modifier;

import android.os.Build;
import android.util.LongSparseArray;
import android.webkit.JavascriptInterface;
import android.annotation.TargetApi;

@TargetApi(Build.VERSION_CODES.JELLY_BEAN)
public class UnoHelper {

	private static HashMap<String,Long> unoTypes = new HashMap<String,Long>();
	private static LongSparseArray<Class<?>> unoFallbacks = new LongSparseArray<Class<?>>();
	private static HashMap<Class<?>,Long> unoFallbacksClassToPtr = new HashMap<Class<?>,Long>();
	private static LongSparseArray<Class<?>> unoToJavaClass = new LongSparseArray<Class<?>>();

	public static native long AnnounceInstantiation(Object jobj, long unoPtr);
	public static native void Finalize(long arg0);

	public static void RegisterUnoType(String javaTypeName, long unoTypePtr)
	{
		Long ptr = unoTypePtr;
		Class<?> cls = null;
		try {
			cls = Class.forName(javaTypeName);
		} catch (ClassNotFoundException ignore) {
            ptr = null;
        }
		unoTypes.put(javaTypeName, ptr);
		unoToJavaClass.put(unoTypePtr, cls);
	}
	public static void RegisterUnoFallback(String javaTypeName, long unoTypePtr)
	{
		Class<?> cls = null;
		try {
			cls = Class.forName(javaTypeName);
		} catch (ClassNotFoundException ignore) {}
		unoFallbacks.put(Long.valueOf(unoTypePtr), cls);
		unoFallbacksClassToPtr.put(cls, Long.valueOf(unoTypePtr));
	}

	public static Class<?> UnoToJavaType(long unoTypePtr) throws ClassNotFoundException
	{
		Class<?> cls = unoToJavaClass.get(unoTypePtr);
		if (cls == null) {
			throw new ClassNotFoundException("BINDING CLASS NOT FOUND (UnoToJavaType): Not found for unoTypePtr:"+unoTypePtr);
		}
		return cls;
	}

	public static long NonSearchingJavaToUnoType(Object obj, boolean errorIfMissing) throws Throwable
	{
		Class<?> firstCls = obj.getClass();
		String clsName = firstCls.getName();
		Long unoTypePtr = unoTypes.get(clsName);
		if (unoTypePtr==null) {
			if (errorIfMissing) {
				String msg = "NonSearchingJavaToUnoType: Could not find uno type for " + clsName;
				throw new Exception(msg);
			} else {
				return 0L;
			}
		}
		return (long)unoTypePtr;
	}
	public static long NonSearchingJavaToUnoType(Class<?> cls, boolean errorIfMissing) throws Throwable
	{
		String clsName = cls.getName();
		Long unoTypePtr = unoTypes.get(clsName);
		if (unoTypePtr==null) {
			if (errorIfMissing) {
				String msg = "NonSearchingJavaToUnoType: Could not find uno type for " + clsName;
				throw new Exception(msg);
			} else {
				return 0L;
			}
		}
		return (long)unoTypePtr;
	}

	public static long JavaToUnoType(Object obj, long fallbackTypePtr, boolean typeHasFallbackClass) throws ClassNotFoundException
	{
		Class<?> firstCls = obj.getClass();
		Long unoTypePtr = unoTypes.get(firstCls.getName());
		if (unoTypePtr != null) {
			return (long)unoTypePtr;
		} else {
			if (typeHasFallbackClass) {
				Class<?> itf = unoFallbacks.get(fallbackTypePtr);
				if (itf == null) {
					throw new ClassNotFoundException("BINDING CLASS NOT FOUND (unoFallbacks): Not found for unoTypePtr:"+fallbackTypePtr);
				}
				Class<?> currentCls = firstCls;
				while (true) {
					if ((!itf.equals(currentCls)) && itf.isAssignableFrom(currentCls)) {
						Long potential = unoTypes.get(currentCls.getName());
						if (potential != null) {
							unoTypes.put(firstCls.getName(), potential);
							return (long)potential;
						}
					} else {
						unoTypes.put(firstCls.getName(), fallbackTypePtr);
						return fallbackTypePtr;
					}
					currentCls = currentCls.getSuperclass();
					if (currentCls==null) {
						unoTypes.put(firstCls.getName(), fallbackTypePtr);
						return fallbackTypePtr;
					}
				}
			} else {
				Class<?> currentCls = firstCls;
                while (true) {
					currentCls = currentCls.getSuperclass();
					if (currentCls == null) {
						unoTypes.put(firstCls.getName(), fallbackTypePtr);
						return fallbackTypePtr;
					} else {
						Long potential = unoTypes.get(currentCls.getName());
						if (potential != null) {
							if (Modifier.isAbstract(currentCls.getModifiers())) {
								Long fallbackClassPtr = unoFallbacksClassToPtr.get(currentCls);
								if (fallbackClassPtr!=null) {
									unoTypes.put(firstCls.getName(), fallbackClassPtr);
									return fallbackClassPtr;
								}
							} else {
								unoTypes.put(firstCls.getName(), potential);
								return (long)potential;
							}
						}
					}
				}
			}
		}
	}

	public static boolean ClassHasField(Class<?> cls, String fieldName){
		Field[] fields = cls.getFields();
		for (int i = 0; i < fields.length; i++) {
			if(fields[i].getName().equals(fieldName)) { return true; }
		}
		return false;
	}
	public static long GetUnoRef(Class<?> cls, Object obj)
	{
		if (obj!=null) {
			if (com.foreign.UnoWrapped.class.isAssignableFrom(cls)) {
				return ((com.foreign.UnoWrapped)obj)._GetUnoPtr();
			} else {
				return -1;
			}
		} else {
			return 0;
		}
	}
	public static long GetUnoObjectRef(Object obj)
	{
		if (obj!=null) {
			return GetUnoRef(obj.getClass(), obj);
		} else {
			return 0;
		}
	}

    public static Observable MakeJSObservable(Observer obs)
	{
		Observable result = new JSObservableImpl();
		result.addObserver(obs);
		return result;
	}

	private static class JSObservableImpl extends Observable {
		@JavascriptInterface
		public void forceNotifyObservers()
		{
            setChanged();
			super.notifyObservers();
		}
		@JavascriptInterface
		public void forceNotifyObservers(String data)
		{
            setChanged();
			super.notifyObservers(data);
		}
	}
}
