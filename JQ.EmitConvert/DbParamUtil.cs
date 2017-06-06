using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：ParamUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：ParamUtil
    /// 创建标识：yjq 2017/6/6 19:43:32
    /// </summary>
    public static class DbParamUtil
    {
        private delegate List<TParam> ObjectToParamListDelegate<TParam>(object obj, string sign, string prefix);

        public static List<TParam> ToDbParam<TParam>(object obj, string sign, string prefix) where TParam : DbParameter
        {
            var convertMethod = DynamicMethodUtil.GetObjectToParamListMethod<TParam>(obj.GetType());
            var action = (ObjectToParamListDelegate<TParam>)convertMethod.CreateDelegate(typeof(ObjectToParamListDelegate<TParam>));
            return action(obj, sign, prefix);
        }
    }
}
