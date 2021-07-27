// *************************************************
// @author : rontian
// @email  : i@ronpad.com
// @date   : 2021/07/23
// *************************************************/

using System;
using System.Reflection;
namespace Com.RonPad.Entities.Tools
{
    public static class TypeUtils
    {
        /**
         * 通过反射创建普通类型实例
         */
        public static object CreateInstance(this Type type, object[] args = null)
        {
            return Activator.CreateInstance(type, args);
        }
        /**
         * 通过反射创建 (确定) 泛型类型实例
         */
        public static object CreateInstance(this Type type, Type[] types, object[] args = null)
        {
            type = type.MakeGenericType(types);
            return Activator.CreateInstance(type, args);
            //return type.InvokeMember(construct, BindingFlags.Default | BindingFlags.InvokeMethod, null, o, args);
        }
        public static bool SetTypeValue(this Type type, object instance, string name, object value)
        {
            return !type.SetPropertyValue(instance, name, value) && type.SetFieldValue(instance, name, value);
        }
        public static bool SetPropertyValue(this Type type, object instance, string name, object value)
        {
            var propertyInfo = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo != null)
            {
                propertyInfo.SetValue(instance, value);
                return true;
            }
            return false;
        }
        public static bool SetFieldValue(this Type type, object instance, string name, object value)
        {
            var fieldInfo = type.GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (fieldInfo != null)
            {
                fieldInfo.SetValue(instance, value);
                return true;
            }
            return false;
        }
    }
}
