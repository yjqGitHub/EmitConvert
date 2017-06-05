using JQ.EmitConvert;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：PropertyReflectUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：PropertyReflectUtil
    /// 创建标识：yjq 2017/6/5 20:39:00
    /// </summary>
    public static class PropertyReflectUtil
    {
        #region 将DataRow转换成对应的实体。

        /// <summary>
        ///  将DataRow转换成对应的实体。
        ///  创建标识：yjq 2016年7月21日15:07:01
        /// </summary>
        /// <typeparam name="T">要转换的实体类型。</typeparam>
        /// <param name="this">被转换的DataRow</param>
        /// <returns>转换的实体结果</returns>
        public static T ToEntity<T>(DataRow @this) where T : new()
        {
            Type type = typeof(T);
            List<PropertyInfo> properties = PropertyUtil.GetTypeProperties(type);
            FieldInfo[] fields = TypeUtil.GetTypeFields(type);

            var entity = new T();

            foreach (PropertyInfo property in properties)
            {
                if (@this.Table.Columns.Contains(property.Name))
                {
                    Type valueType = property.PropertyType;
                    property.SetValue(entity, @this[property.Name].To(valueType), null);
                }
            }

            foreach (FieldInfo field in fields)
            {
                if (@this.Table.Columns.Contains(field.Name))
                {
                    Type valueType = field.FieldType;
                    field.SetValue(entity, @this[field.Name].To(valueType));
                }
            }

            return entity;
        }

        #endregion 将DataRow转换成对应的实体。

        public static object To(this Object @this, Type type)
        {
            if (@this != null)
            {
                Type targetType = type;

                if (@this.GetType() == targetType)
                {
                    return @this;
                }

                TypeConverter converter = TypeDescriptor.GetConverter(@this);
                if (converter != null)
                {
                    if (converter.CanConvertTo(targetType))
                    {
                        return converter.ConvertTo(@this, targetType);
                    }
                }

                converter = TypeDescriptor.GetConverter(targetType);
                if (converter != null)
                {
                    if (converter.CanConvertFrom(@this.GetType()))
                    {
                        return converter.ConvertFrom(@this);
                    }
                }

                if (@this == DBNull.Value)
                {
                    return null;
                }
            }

            return @this;
        }

        /// <summary>
        ///     Enumerates to entities in this collection.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as an IEnumerable&lt;T&gt;</returns>
        public static IEnumerable<T> ToEntities<T>(DataTable @this) where T : new()
        {
            Type type = typeof(T);
            List<PropertyInfo> properties = PropertyUtil.GetTypeProperties(type);
            FieldInfo[] fields = TypeUtil.GetTypeFields(type);

            var list = new List<T>();

            foreach (DataRow dr in @this.Rows)
            {
                var entity = new T();

                foreach (PropertyInfo property in properties)
                {
                    if (@this.Columns.Contains(property.Name))
                    {
                        Type valueType = property.PropertyType;
                        property.SetValue(entity, dr[property.Name].To(valueType), null);
                    }
                }

                foreach (FieldInfo field in fields)
                {
                    if (@this.Columns.Contains(field.Name))
                    {
                        Type valueType = field.FieldType;
                        field.SetValue(entity, dr[field.Name].To(valueType));
                    }
                }
                list.Add(entity);
            }

            return list;
        }
    }
}