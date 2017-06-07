using JQ.EmitConvert;
using System;
using System.Linq;

namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：EmitTableToListTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：EmitTableToListTest
    /// 创建标识：yjq 2017/6/5 20:17:29
    /// </summary>
    public sealed class EmitTableToListTest : IAction
    {
        public void Action()
        {
            var userList =User.GetUserList(1).ToList<User>();
            foreach (var item in userList)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}