using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：TypeUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：TypeUtil
    /// 创建标识：yjq 2017/6/5 19:37:20
    /// </summary>
    public sealed class TypeUtil
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, FieldInfo[]> _typeFieldCache = new ConcurrentDictionary<RuntimeTypeHandle, FieldInfo[]>();

        /// <summary>
        /// 根据类型获取字段属性与数据的访问权
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>字段属性与数据的访问权</returns>
        public static FieldInfo[] GetTypeFields(Type type)
        {
            if (type == null) return new FieldInfo[0];

            var typeHandle = type.TypeHandle;
            return _typeFieldCache.GetValue(typeHandle, () =>
            {
                return type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            });
        }

        public static readonly Type[] _BaseTypes = new Type[] {typeof(byte) ,
                          typeof(sbyte),
                          typeof(short),
                          typeof(ushort),
                          typeof(int),
                          typeof(uint),
                          typeof(long),
                          typeof(ulong),
                          typeof(float),
                          typeof(double),
                          typeof(decimal),
                          typeof(bool),
                          typeof(string),
                          typeof(char),
                          typeof(Guid),
                          typeof(DateTime),
                          typeof(DateTimeOffset),
                          typeof(TimeSpan),
                          typeof(byte[]),
                          typeof(byte?),
                          typeof(sbyte?),
                          typeof(short?),
                          typeof(ushort?),
                          typeof(int?),
                          typeof(uint?),
                          typeof(long?),
                          typeof(ulong?),
                          typeof(float?),
                          typeof(double?),
                          typeof(decimal?),
                          typeof(bool?),
                          typeof(char?),
                          typeof(Guid?),
                          typeof(DateTime?),
                          typeof(DateTimeOffset?),
                          typeof(TimeSpan?),
                          typeof(object) };
    }
}