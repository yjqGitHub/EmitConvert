using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：PropertyReflectUtilTableToListTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：PropertyReflectUtilTableToListTest
    /// 创建标识：yjq 2017/6/5 20:40:27
    /// </summary>
    public sealed class PropertyReflectUtilTableToListTest : IAction
    {
        public void Action()
        {
            PropertyReflectUtil.ToEntities<User>(User.GetUserList(100));
        }
    }
}
