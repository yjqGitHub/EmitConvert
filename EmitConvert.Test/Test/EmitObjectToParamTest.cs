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
            int? id = 9;
            var obj = new { Id = id, Name = 2 };
            var paramList = DbParamUtil.ToDbParam<SqlParameter>(obj, "@", "w_");
            foreach (var item in paramList)
            {
                Console.WriteLine($"{item.ParameterName},{item.DbType},{item.Value}");
            }
            var paramList1 = DbParamUtil.ToDbParam<SqlParameter>(new User(), "@", "w_");
            foreach (var item in paramList1)
            {
                Console.WriteLine($"{item.ParameterName},{item.DbType},{item.Value}");
            }
            var paramList2 = DbParamUtil.ToDbParam<SqlParameter>(new TestClaa(), "@", "w_");
            //foreach (var item in paramList2)
            //{
            //    Console.WriteLine($"{item.ParameterName},{item.DbType},{item.Value}");
            //}
            var paramList3 = DbParamUtil.ToDbParam<SqlParameter>(new TestParam2(22), "@", "w_");
            //foreach (var item in paramList3)
            //{
            //    Console.WriteLine($"{item.ParameterName},{item.DbType},{item.Value}");
            //}
        }
    }

    public struct TestClaa
    {
        public int MyProperty { get; set; }

        public string aaa { get; set; }

        public int? asdsad { get; set; }
    }

    public struct TestParam2
    {
        public TestParam2(int a)
        {
            MyProperty = a;
        }

        public int MyProperty { get; set; }
    }
}