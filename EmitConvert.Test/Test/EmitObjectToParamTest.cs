using JQ.EmitConvert;
using System;
using System.Data.SqlClient;

namespace EmitConvert.Test.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：EmitObjectToParamTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：EmitObjectToParamTest
    /// 创建标识：yjq 2017/6/6 19:09:24
    /// </summary>
    public sealed class EmitObjectToParamTest : IAction
    {
        public void Action()
        {
            var paramList = DbParamUtil.ToDbParam<SqlParameter>(new User(), "@", "w_");
        }
    }
}