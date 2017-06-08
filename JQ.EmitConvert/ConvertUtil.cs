using System;
using System.Collections.Generic;
using System.Reflection;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：ConvertUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：ConvertUtil
    /// 创建标识：yjq 2017/6/5 19:40:04
    /// </summary>
    public static class ConvertUtil
    {
        private static Dictionary<RuntimeTypeHandle, MethodInfo> _ConvertMethodCache = null;

        static ConvertUtil()
        {
            _ConvertMethodCache = new Dictionary<RuntimeTypeHandle, MethodInfo>()
            {
                [typeof(byte).TypeHandle] = typeof(ConvertUtil).GetMethod("ToByte", new Type[] { typeof(object) }),
                [typeof(sbyte).TypeHandle] = typeof(ConvertUtil).GetMethod("ToSByte", new Type[] { typeof(object) }),
                [typeof(short).TypeHandle] = typeof(ConvertUtil).GetMethod("ToInt16", new Type[] { typeof(object) }),
                [typeof(ushort).TypeHandle] = typeof(ConvertUtil).GetMethod("ToUInt16", new Type[] { typeof(object) }),
                [typeof(int).TypeHandle] = typeof(ConvertUtil).GetMethod("ToInt32", new Type[] { typeof(object) }),
                [typeof(uint).TypeHandle] = typeof(ConvertUtil).GetMethod("ToUInt32", new Type[] { typeof(object) }),
                [typeof(long).TypeHandle] = typeof(ConvertUtil).GetMethod("ToInt64", new Type[] { typeof(object) }),
                [typeof(ulong).TypeHandle] = typeof(ConvertUtil).GetMethod("ToUInt64", new Type[] { typeof(object) }),
                [typeof(bool).TypeHandle] = typeof(ConvertUtil).GetMethod("ToBoolean", new Type[] { typeof(object) }),
                [typeof(decimal).TypeHandle] = typeof(ConvertUtil).GetMethod("ToDecimal", new Type[] { typeof(object) }),
                [typeof(double).TypeHandle] = typeof(ConvertUtil).GetMethod("ToDouble", new Type[] { typeof(object) }),
                [typeof(float).TypeHandle] = typeof(ConvertUtil).GetMethod("ToFloat", new Type[] { typeof(object) }),
                [typeof(char).TypeHandle] = typeof(ConvertUtil).GetMethod("ToChar", new Type[] { typeof(object) }),
                [typeof(Guid).TypeHandle] = typeof(ConvertUtil).GetMethod("ToGuid", new Type[] { typeof(object) }),
                [typeof(DateTime).TypeHandle] = typeof(ConvertUtil).GetMethod("ToDateTime", new Type[] { typeof(object) }),
                [typeof(byte?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNByte", new Type[] { typeof(object) }),
                [typeof(sbyte?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNSByte", new Type[] { typeof(object) }),
                [typeof(short?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNInt16", new Type[] { typeof(object) }),
                [typeof(ushort?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNUInt16", new Type[] { typeof(object) }),
                [typeof(int?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNInt32", new Type[] { typeof(object) }),
                [typeof(uint?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNUInt32", new Type[] { typeof(object) }),
                [typeof(long?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNInt64", new Type[] { typeof(object) }),
                [typeof(ulong?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNUInt64", new Type[] { typeof(object) }),
                [typeof(bool?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNBoolean", new Type[] { typeof(object) }),
                [typeof(decimal?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNDecimal", new Type[] { typeof(object) }),
                [typeof(double?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNDouble", new Type[] { typeof(object) }),
                [typeof(float?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNFloat", new Type[] { typeof(object) }),
                [typeof(char?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNChar", new Type[] { typeof(object) }),
                [typeof(Guid?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNGuid", new Type[] { typeof(object) }),
                [typeof(DateTime?).TypeHandle] = typeof(ConvertUtil).GetMethod("ToNDateTime", new Type[] { typeof(object) }),
                [typeof(string).TypeHandle] = typeof(ConvertUtil).GetMethod("ToString", new Type[] { typeof(object) })
            };
        }

        public static void Add(RuntimeTypeHandle typeHandle, MethodInfo methodInfo)
        {
            if (!_ConvertMethodCache.ContainsKey(typeHandle))
            {
                _ConvertMethodCache[typeHandle] = methodInfo;
            }
        }

        private static MethodInfo GetMethodInfo(RuntimeTypeHandle typeHandle)
        {
            if (_ConvertMethodCache.ContainsKey(typeHandle))
            {
                return _ConvertMethodCache[typeHandle];
            }
            return null;
        }

        public static MethodInfo GetMethodInfo(Type type)
        {
            if (_ConvertMethodCache.ContainsKey(type.TypeHandle))
            {
                return _ConvertMethodCache[type.TypeHandle];
            }
            return null;
        }

        public static byte ToByte(object value)
        {
            if (value is byte)
            {
                return (byte)value;
            }
            return Convert.ToByte(value);
        }

        public static sbyte ToSByte(object value)
        {
            if (value is sbyte)
            {
                return (sbyte)value;
            }
            return Convert.ToSByte(value);
        }

        public static short ToInt16(object value)
        {
            if (value is short)
            {
                return (short)value;
            }
            return Convert.ToInt16(value);
        }

        public static ushort ToUInt16(object value)
        {
            if (value is ushort)
            {
                return (ushort)value;
            }
            return Convert.ToUInt16(value);
        }

        public static int ToInt32(object value)
        {
            if (value is int)
            {
                return (int)value;
            }
            if (value is Enum)
            {
                return Convert.ToInt32(((Enum)value).ToString("d"));
            }
            return Convert.ToInt32(value);
        }

        public static uint ToUInt32(object value)
        {
            if (value is uint)
            {
                return (uint)value;
            }
            return Convert.ToUInt32(value);
        }

        public static long ToInt64(object value)
        {
            if (value is long)
            {
                return (long)value;
            }
            return Convert.ToInt64(value);
        }

        public static ulong ToUInt64(object value)
        {
            if (value is ulong)
            {
                return (ulong)value;
            }
            return Convert.ToUInt64(value);
        }

        public static bool ToBoolean(object value)
        {
            if (value == null)
            {
                return false;
            }
            if (value is bool)
            {
                return (bool)value;
            }
            if (value.Equals("1") || value.Equals("-1"))
            {
                value = "true";
            }
            else if (value.Equals("0"))
            {
                value = "false";
            }

            return Convert.ToBoolean(value);
        }

        public static decimal ToDecimal(object value)
        {
            if (value is decimal)
            {
                return (decimal)value;
            }

            return Convert.ToDecimal(value);
        }

        public static double ToDouble(object value)
        {
            if (value is double)
            {
                return (double)value;
            }

            return Convert.ToDouble(value);
        }

        public static float ToFloat(object value)
        {
            if (value is float)
            {
                return (float)value;
            }

            return Convert.ToSingle(value);
        }

        public static char ToChar(object value)
        {
            if (value is char)
            {
                return (char)value;
            }

            return Convert.ToChar(value);
        }

        public static Guid ToGuid(object value)
        {
            if (value is Guid)
            {
                return (Guid)value;
            }
            return Guid.Parse(value.ToString());
        }

        public static DateTime ToDateTime(object value)
        {
            if (value is DateTime)
            {
                return (DateTime)value;
            }
            return Convert.ToDateTime(value);
        }

        public static byte? ToNByte(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is byte?)
            {
                return (byte?)value;
            }
            return Convert.ToByte(value);
        }

        public static sbyte? ToNSByte(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is sbyte?)
            {
                return (sbyte?)value;
            }
            return Convert.ToSByte(value);
        }

        public static short? ToNInt16(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is short?)
            {
                return (short?)value;
            }
            return Convert.ToInt16(value);
        }

        public static ushort? ToNUInt16(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is ushort)
            {
                return (ushort)value;
            }

            return Convert.ToUInt16(value);
        }

        public static int? ToNInt32(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is int)
            {
                return (int)value;
            }
            if (value is Enum)
            {
                return Convert.ToInt32(((Enum)value).ToString("d"));
            }
            return Convert.ToInt32(value);
        }

        public static uint? ToNUInt32(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is uint)
            {
                return (uint)value;
            }
            return Convert.ToUInt32(value);
        }

        public static long? ToNInt64(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is long)
            {
                return (long)value;
            }

            return Convert.ToInt64(value);
        }

        public static ulong? ToNUInt64(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is ulong?)
            {
                return (ulong?)value;
            }
            return Convert.ToUInt64(value);
        }

        public static bool? ToNBoolean(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is bool)
            {
                return (bool)value;
            }
            if (value.Equals("1") || value.Equals("-1"))
            {
                value = "true";
            }
            else if (value.Equals("0"))
            {
                value = "false";
            }

            return Convert.ToBoolean(value);
        }

        public static decimal? ToNDecimal(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is decimal)
            {
                return (decimal)value;
            }

            return Convert.ToDecimal(value);
        }

        public static double? ToNDouble(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is double)
            {
                return (double)value;
            }

            return Convert.ToDouble(value);
        }

        public static float? ToNFloat(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is float)
            {
                return (float)value;
            }

            return Convert.ToSingle(value);
        }

        public static char? ToNChar(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is char)
            {
                return (char)value;
            }

            return Convert.ToChar(value);
        }

        public static Guid? ToNGuid(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is Guid)
            {
                return (Guid)value;
            }
            return Guid.Parse(value.ToString());
        }

        public static DateTime? ToNDateTime(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is DateTime)
            {
                return (DateTime)value;
            }
            return Convert.ToDateTime(value);
        }

        public static string ToString(object value)
        {
            if (value == null)
            {
                return null;
            }
            return value.ToString();
        }
    }
}