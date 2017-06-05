using System;
using System.Collections.Concurrent;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：ConcurrentDicExtension.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：ConcurrentDicExtension
    /// 创建标识：yjq 2017/6/5 19:38:51
    /// </summary>
    public static partial class Extension
    {
        /// <summary>
        /// ConcurrentDictionary 获取值的扩展方法
        /// </summary>
        /// <typeparam name="TKey">键类型</typeparam>
        /// <typeparam name="TValue">值类型</typeparam>
        /// <param name="conDic">字典集合</param>
        /// <param name="key">键值</param>
        /// <param name="action">获取值的方法(使用时需要执行注意方法闭包)</param>
        /// <returns>指定键的值</returns>
        public static TValue GetValue<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> conDic, TKey key, Func<TValue> action)
        {
            TValue value;
            if (conDic.TryGetValue(key, out value)) return value;
            value = action();
            conDic[key] = value;
            return value;
        }
    }
}