using System;
using System.Data;

namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：Port.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：Port
    /// 创建标识：yjq 2017/6/5 20:18:11
    /// </summary>
    public class User
    {
        public double? Id { get; set; }
        public string Name { get; set; }
        public bool Sex { get; set; }
        public Guid Uid { private get; set; }
        public Port SourcePort { get; private set; }
        public DateTime? Time { get; set; }

        public string SexText
        {
            private get
            {
                return Sex ? "男" : "女";
            }
            set
            {
                Sex = (value == "男");
            }
        }

        public Product Product { get; set; }

        public override string ToString()
        {
            return $"{Id},{Name},{Sex},{Uid},{Time},{SexText},{SourcePort},{Product.ToString()}";
        }

        static public DataTable GetUserList(int count)
        {
            DataTable table = new DataTable("User");
            table.Columns.Add("Id");
            table.Columns.Add("Name");
            table.Columns.Add("Sex");
            table.Columns.Add("Uid");
            table.Columns.Add("Time");
            table.Columns.Add("SourcePort", typeof(object));
            table.Columns.Add("ProductId");
            table.Columns.Add("ProductName");
            table.Columns.Add("ProductPrice");
            table.Columns.Add("ProductAddTime");
            for (int i = 0; i < count; i++)
            {
                table.Rows.Add(i, "blqw" + i, true, Guid.NewGuid(), DateTime.Now, i < 10 ? Port.APP : Port.PC, i, "product" + i.ToString(), i, DateTime.Now);
            }
            return table;
        }
    }

    public enum Port
    {
        PC,
        APP
    }

    public class Product
    {
        public int? ProductId { get; set; }
        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public DateTime ProductAddTime { get; set; }

        public override string ToString()
        {
            return $"{ProductId},{ProductName},{ProductPrice},{ProductAddTime}";
        }
    }
}