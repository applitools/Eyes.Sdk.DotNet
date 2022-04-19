using System;
using System.Reflection;

namespace Applitools.Utils
{
    public static class ReflectionUtils
    {
        public static T GetPrivateFieldValue<T>(this object instance, string fieldName)
        {
            return (T)GetPrivateFieldValue(instance, fieldName);
        }

        public static TV GetBaseTypePrivateFieldValue<TI, TV>(this TI instance, string fieldName)
        {
            return (TV)GetBaseTypePrivateFieldValue(instance, fieldName);
        }

        private static object GetPrivateFieldValue<T>(T instance, string fieldName)
        {
            var instanceType = instance.GetType();
            FieldInfo fieldInfo = instanceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new Exception($"The type {instanceType} does not contain {fieldName}");
            }

            return fieldInfo.GetValue(instance);
        }

        private static object GetBaseTypePrivateFieldValue<T>(T instance, string fieldName)
        {
            var instanceType = typeof(T);
            FieldInfo fieldInfo = instanceType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new Exception($"The type {instanceType} does not contain {fieldName}");
            }

            return fieldInfo.GetValue(instance);
        }
    }
}
