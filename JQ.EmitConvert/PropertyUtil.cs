using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：PropertyUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：PropertyUtil
    /// 创建标识：yjq 2017/6/5 19:42:00
    /// </summary>
    public static class PropertyUtil
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>> _propertyCache = new ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>>();

        #region 获取实例的属性列表

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型的</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type)
        {
            if (type == null) return new List<PropertyInfo>();
            var typeHandle = type.TypeHandle;
            return _propertyCache.GetValue(typeHandle, () =>
            {
                return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();//BindingFlags.GetProperty |
            });
        }

        #endregion 获取实例的属性列表
    }
}