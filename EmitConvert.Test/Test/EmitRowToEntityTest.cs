using JQ.EmitConvert;
using System;

namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：EmitRowToEntityTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：EmitRowToEntityTest
    /// 创建标识：yjq 2017/6/5 20:33:40
    /// </summary>
    public sealed class EmitRowToEntityTest : IAction
    {
        public void Action()
        {
            Console.WriteLine(User.GetUserList(1).Rows[0].ToEntity<User>());
        }
    }
}