using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Reflection.Emit;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：DataTableUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：DataTableUtil
    /// 创建标识：yjq 2017/6/5 19:43:07
    /// </summary>
    public static class DataTableUtil
    {
        public static readonly MethodInfo IsContainColumnMethod = typeof(DataTableUtil).GetMethod("IsContainColumn", new Type[] { typeof(DataRow), typeof(string) });

        public static readonly MethodInfo GetColumnValueMethod = typeof(DataTableUtil).GetMethod("GetColumnValue", new Type[] { typeof(DataRow), typeof(string) });

        public static readonly MethodInfo GetRowMethod = typeof(DataTable).GetMethod("get_Rows", Type.EmptyTypes);

        public static readonly MethodInfo GetRowItemMethod = typeof(DataRowCollection).GetMethod("get_Item", new Type[] { typeof(int) });

        public static readonly MethodInfo GetRowsCountMethod = typeof(DataRowCollection).GetMethod("get_Count", Type.EmptyTypes);

        private delegate List<T> TableToListDelegate<T>(DataTable table);//将Table转为List的委托

        public static List<T> ToList<T>(this DataTable @table)
        {
            if (@table == null || @table.Rows.Count <= 0)
            {
                return new List<T>();
            }
            DynamicMethod method = DynamicMethodUtil.GetConvertToListMethod<T>();
            var action = (TableToListDelegate<T>)method.CreateDelegate(typeof(TableToListDelegate<T>));
            return action(@table);
        }

        private delegate T RowToEntityDelegate<T>(DataRow row);//将DataRow转为Entity的委托

        public static T ToEntity<T>(this DataRow @row)
        {
            if (@row == null)
            {
                return default(T);
            }
            DynamicMethod method = DynamicMethodUtil.GetConvertToEntityMethod<T>();
            var action = (RowToEntityDelegate<T>)method.CreateDelegate(typeof(RowToEntityDelegate<T>));
            return action(@row);
        }

        public static bool IsContainColumn(DataRow row, string columnName)
        {
            if (row == null) return false;
            if (string.IsNullOrWhiteSpace(columnName)) return false;
            return row.Table.Columns.IndexOf(columnName) >= 0;
        }

        public static object GetColumnValue(DataRow row, string columnName)
        {
            if (IsContainColumn(row, columnName))
            {
                var value = row[columnName];
                if (value == null || value == DBNull.Value)
                {
                    value = null;
                }
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}