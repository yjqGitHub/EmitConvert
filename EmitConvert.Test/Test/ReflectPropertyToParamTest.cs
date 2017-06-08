using JQ.EmitConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace EmitConvert.Test.Test
{
    /// <summary>
    /// Copyright (C) 2015 备胎 版权所有。
    /// 类名：ReflectPropertyToParamTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：
    /// 创建标识：yjq 2017/6/8 15:51:17
    /// </summary>
    public sealed class ReflectPropertyToParamTest : IAction
    {
        public void Action()
        {
            User user = new EmitConvert.Test.User { Id = 1, Name = "234", Product = new EmitConvert.Test.Product { ProductAddTime = DateTime.Now, ProductId = 8, ProductName = "ddf", ProductPrice = (decimal)25.6 }, Sex = false, Time = DateTime.Now, Uid = Guid.NewGuid() };
            var paramList1 = PropertyUtil.ObjToSqlParameter(user, "w_"); 
        }
    }
}
