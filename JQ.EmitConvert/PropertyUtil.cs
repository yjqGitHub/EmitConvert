using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：PropertyUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：PropertyUtil
    /// 创建标识：yjq 2017/6/5 19:42:00
    /// </summary>
    public static class PropertyUtil
    {
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>> _propertyCache = new ConcurrentDictionary<RuntimeTypeHandle, List<PropertyInfo>>();

        public static List<PropertyInfo> GetPropertyInfos(object obj)
        {
            if (obj == null)
            {
                return new List<PropertyInfo>();
            }
            return GetTypeProperties(obj.GetType());
        }

        #region 获取实例的属性列表

        /// <summary>
        /// 根据类型获取类型的属性信息
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>类型的</returns>
        public static List<PropertyInfo> GetTypeProperties(Type type)
        {
            if (type == null) return new List<PropertyInfo>();
            var typeHandle = type.TypeHandle;
            return _propertyCache.GetValue(typeHandle, () =>
            {
                return type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();//BindingFlags.GetProperty |
            });
        }

        #endregion 获取实例的属性列表

        public static SqlParameter[] ObjToSqlParameter(object obj, string prefix = null)
        {
            List<PropertyInfo> proList = GetPropertyInfos(obj);
            List<SqlParameter> paraList = new List<SqlParameter>();
            foreach (PropertyInfo propertInfo in proList)
            {
                var tuple = ProToSqlParameter(obj, propertInfo, prefix);
                paraList.Add(tuple.Item2);
            }
            return paraList.ToArray();
        }

        /// <summary>
        /// 获取属性对应的sql参数
        /// </summary>
        /// <param name="obj">实例值</param>
        /// <param name="propertInfo">属性信息</param>
        /// <param name="prefix">参数名字前缀</param>
        /// <returns>item1：属性名字，item2：SqlParameter</returns>
        public static Tuple<string, SqlParameter> ProToSqlParameter(object obj, PropertyInfo propertInfo, string prefix = null)
        {
            string proName = propertInfo.Name;
            var proValue = propertInfo.GetValue(obj, null);
            bool isNullable = false;
            Type propertyType = propertInfo.PropertyType;
            string propertyTypeName = string.Empty;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                propertyTypeName = propertyType.GetGenericArguments()[0].Name;
                isNullable = true;
            }
            else
            {
                propertyTypeName = propertyType.Name;
            }
            object paraValue = null;
            DbType dbType;
            if (propertyType.IsEnum)
            {
                paraValue = Convert.ToInt32(proValue).ToString();
                dbType = DbType.Int32;
            }
            else
            {
                paraValue = GetParameterValue(proValue, propertyTypeName, isNullable);
                dbType = TypeUtil.Type2DbType(propertyType);
            }
            SqlParameter parameter = new SqlParameter()
            {
                ParameterName = "@" + (prefix ?? string.Empty) + proName,
                DbType = dbType,
                Value = paraValue
            };
            return Tuple.Create(proName, parameter);
        }

        /// <summary>
        /// 获取sqlparametr参数值
        /// </summary>
        /// <param name="value">实际值</param>
        /// <param name="typeName">类型名字</param>
        /// <param name="isNullable">是否为可空类型</param>
        /// <returns>sqlparametr参数值</returns>
        private static object GetParameterValue(object value, string typeName, bool isNullable)
        {
            object parameterValue = null;
            if (value == null)
            {
                if (isNullable || string.Equals("string", typeName, StringComparison.OrdinalIgnoreCase))
                {
                    parameterValue = DBNull.Value;
                }
            }
            if (parameterValue == null)
            {
                parameterValue = value;
            }
            return parameterValue;
        }
    }
}