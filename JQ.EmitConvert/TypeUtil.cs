using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
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
    public static class TypeUtil
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, FieldInfo[]> _typeFieldCache = new ConcurrentDictionary<RuntimeTypeHandle, FieldInfo[]>();

        private static Dictionary<RuntimeTypeHandle, DbType> _TypeMap;

        static TypeUtil()
        {
            #region TypeMap

            _TypeMap = new Dictionary<RuntimeTypeHandle, DbType>
            {
                [typeof(byte).TypeHandle] = DbType.Byte,
                [typeof(sbyte).TypeHandle] = DbType.SByte,
                [typeof(short).TypeHandle] = DbType.Int16,
                [typeof(ushort).TypeHandle] = DbType.UInt16,
                [typeof(int).TypeHandle] = DbType.Int32,
                [typeof(uint).TypeHandle] = DbType.UInt32,
                [typeof(long).TypeHandle] = DbType.Int64,
                [typeof(ulong).TypeHandle] = DbType.UInt64,
                [typeof(float).TypeHandle] = DbType.Single,
                [typeof(double).TypeHandle] = DbType.Double,
                [typeof(decimal).TypeHandle] = DbType.Decimal,
                [typeof(bool).TypeHandle] = DbType.Boolean,
                [typeof(string).TypeHandle] = DbType.String,
                [typeof(char).TypeHandle] = DbType.StringFixedLength,
                [typeof(Guid).TypeHandle] = DbType.Guid,
                [typeof(DateTime).TypeHandle] = DbType.DateTime,
                [typeof(DateTimeOffset).TypeHandle] = DbType.DateTimeOffset,
                [typeof(TimeSpan).TypeHandle] = DbType.Time,
                [typeof(byte[]).TypeHandle] = DbType.Binary,
                [typeof(byte?).TypeHandle] = DbType.Byte,
                [typeof(sbyte?).TypeHandle] = DbType.SByte,
                [typeof(short?).TypeHandle] = DbType.Int16,
                [typeof(ushort?).TypeHandle] = DbType.UInt16,
                [typeof(int?).TypeHandle] = DbType.Int32,
                [typeof(uint?).TypeHandle] = DbType.UInt32,
                [typeof(long?).TypeHandle] = DbType.Int64,
                [typeof(ulong?).TypeHandle] = DbType.UInt64,
                [typeof(float?).TypeHandle] = DbType.Single,
                [typeof(double?).TypeHandle] = DbType.Double,
                [typeof(decimal?).TypeHandle] = DbType.Decimal,
                [typeof(bool?).TypeHandle] = DbType.Boolean,
                [typeof(char?).TypeHandle] = DbType.StringFixedLength,
                [typeof(Guid?).TypeHandle] = DbType.Guid,
                [typeof(DateTime?).TypeHandle] = DbType.DateTime,
                [typeof(DateTimeOffset?).TypeHandle] = DbType.DateTimeOffset,
                [typeof(TimeSpan?).TypeHandle] = DbType.Time,
                [typeof(object).TypeHandle] = DbType.Object
            };

            #endregion TypeMap
        }

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

        #region 根据类型对应类型获取对应数据库对应的类型

        /// <summary>
        /// 根据类型对应类型获取对应数据库对应的类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>数据库对应的类型</returns>
        public static DbType Type2DbType(Type type)
        {
            if (type.IsEnum)
            {
                return _TypeMap[typeof(int).TypeHandle];
            }
            if (_TypeMap.ContainsKey(type.TypeHandle))
            {
                return _TypeMap[type.TypeHandle];
            }
            else
            {
                return DbType.Object;
            }
        }

        #endregion 根据类型对应类型获取对应数据库对应的类型

        /// <summary>
        /// 判断类型是否为集合或者数组
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsArrayOrCollection(this Type type)
        {
            if (type == null) return false;
            return type.IsArray || type.IsGenericType;
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