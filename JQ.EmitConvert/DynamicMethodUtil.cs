using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Reflection.Emit;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：DynamicMethodUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：DynamicMethodUtil
    /// 创建标识：yjq 2017/6/5 20:04:09
    /// </summary>
    public static class DynamicMethodUtil
    {
        private static ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod> _ConvertToListMethodCache = new ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod>();

        public static DynamicMethod GetConvertToListMethod<T>()
        {
            return _ConvertToListMethodCache.GetValue(typeof(T).TypeHandle, () =>
            {
                return EmitUtil.CreateTableToListMethod<T>();
            });
        }

        private static ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod> _ConvertDataRowToEntityMethodCache = new ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod>();

        public static DynamicMethod GetConvertToEntityMethod<T>()
        {
            return _ConvertDataRowToEntityMethodCache.GetValue(typeof(T).TypeHandle, () =>
            {
                return EmitUtil.CreateDataRowToEntityMethod<T>();
            });
        }

        private static ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod>> _ObjectToParamListMethodCache = new ConcurrentDictionary<RuntimeTypeHandle, ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod>>();

        public static DynamicMethod GetObjectToParamListMethod<TParam>(Type objType) where TParam : DbParameter
        {
            ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod> objectToParamListMethod;
            _ObjectToParamListMethodCache.TryGetValue(objType.TypeHandle, out objectToParamListMethod);
            if (objectToParamListMethod != null)
            {
                DynamicMethod method;
                objectToParamListMethod.TryGetValue(typeof(TParam).TypeHandle, out method);
                if (method == null)
                {
                    method = EmitUtil.CreateObjectToParamListMethod<TParam>(objType);
                    objectToParamListMethod.TryAdd(typeof(TParam).TypeHandle, method);
                    _ObjectToParamListMethodCache[objType.TypeHandle] = objectToParamListMethod;
                }
                return method;
            }
            else
            {
                objectToParamListMethod = new ConcurrentDictionary<RuntimeTypeHandle, DynamicMethod>();
                DynamicMethod method = EmitUtil.CreateObjectToParamListMethod<TParam>(objType);
                objectToParamListMethod.TryAdd(typeof(TParam).TypeHandle, method);
                _ObjectToParamListMethodCache[objType.TypeHandle] = objectToParamListMethod;
                return method;
            }

        }
    }
}