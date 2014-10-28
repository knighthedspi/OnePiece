using UnityEngine;
using System.Collections;
using System.Reflection;
using System;

public class ModelObject<T> {

	public static void SetField(T obj, string key, object data) {
		FieldInfo fieldInfo = obj.GetType().GetField(key, BindingFlags.NonPublic | BindingFlags.Instance);
		SetField(obj, fieldInfo, data);
	}
	
	public static void SetField(T obj, FieldInfo fieldInfo, object data) {
		if (fieldInfo.FieldType == typeof(int)) {
			fieldInfo.SetValue(obj, (int)data);
		} else if (fieldInfo.FieldType == typeof(Nullable<Int32>)) {
			fieldInfo.SetValue(obj, (int?)data);
		} else if (fieldInfo.FieldType == typeof(string)) {
			fieldInfo.SetValue(obj, (string)data);
		} else if (fieldInfo.FieldType == typeof(double)) {
			fieldInfo.SetValue(obj, (double)data);
		} else if (fieldInfo.FieldType == typeof(byte[])) {
			fieldInfo.SetValue(obj, (byte[])data);
		}
	}
}
