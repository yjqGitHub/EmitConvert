using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2015 备胎 版权所有。
    /// 类名：EmitBuilderUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：
    /// 创建标识：yjq 2017/6/7 20:20:17
    /// </summary>
    public static class EmitBuilderUtil
    {
        /// <summary>
        /// 执行根据类型创建默认的对象值
        /// </summary>
        /// <param name="il"></param>
        /// <param name="instanceType">对象类型</param>
        public static void SetDefaultObjectValue(this ILGenerator il, Type instanceType, LocalBuilder instanceBuilder)
        {
            if (instanceType.IsClass)
            {
                il.Emit(OpCodes.Newobj, instanceType.GetConstructor(Type.EmptyTypes));
                il.Emit(OpCodes.Stloc_S, instanceBuilder);
            }
            else
            {
                il.Emit(OpCodes.Ldloca_S, instanceBuilder);
                il.Emit(OpCodes.Initobj, instanceType);
            }
        }

        /// <summary>
        /// 执行加载类型IL
        /// </summary>
        /// <param name="il"></param>
        /// <param name="instanceType">对象类型</param>
        /// <param name="instanceBuilder">本地变量</param>
        public static void LoadDefaultObject(this ILGenerator il, Type instanceType, LocalBuilder instanceBuilder)
        {
            if (instanceType.IsClass)
            {
                il.Emit(OpCodes.Ldloc, instanceBuilder);
            }
            else//structure 需要加载的是对象的地址
            {
                il.Emit(OpCodes.Ldloca, instanceBuilder);
            }
        }

        /// <summary>
        /// 执行设置值的IL
        /// </summary>
        /// <param name="il"></param>
        /// <param name="instanceType">对象类型</param>
        /// <param name="setMethod">方法</param>
        public static void SetObjectValue(this ILGenerator il, Type instanceType, MethodInfo setMethod)
        {
            if (instanceType.IsValueType)
            {
                il.Emit(OpCodes.Call, setMethod);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, setMethod);
            }
        }

        /// <summary>
        /// 执行转换值的方法IL
        /// </summary>
        /// <param name="il"></param>
        /// <param name="propertyType">属性类型</param>
        public static void ConvertValue(this ILGenerator il, Type propertyType)
        {
            var convertMethod = ConvertUtil.GetMethodInfo(propertyType);
            if (convertMethod == null)
            {
                if (propertyType.IsValueType)
                {
                    il.Emit(OpCodes.Unbox_Any, propertyType);//如果是值类型就拆箱
                }
                else
                {
                    il.Emit(OpCodes.Castclass, propertyType);
                }
            }
            else
            {
                il.Emit(OpCodes.Call, convertMethod);
            }
        }

        /// <summary>
        /// 设置DataRow转Entity的IL
        /// </summary>
        /// <param name="instanceType">实例类型</param>
        /// <param name="il"></param>
        /// <param name="currentRowBuilder">当前DataRow</param>
        /// <param name="instanceBuilder">实例的本地变量</param>
        public static void SetDataRowToEntityIL(this ILGenerator il, Type instanceType, LocalBuilder currentRowBuilder, LocalBuilder instanceBuilder)
        {
            List<PropertyInfo> properties = PropertyUtil.GetTypeProperties(instanceType);
            foreach (var property in properties)
            {
                var setMethod = property.GetSetMethod(true);
                if (setMethod != null)
                {
                    if (TypeUtil._BaseTypes.Contains(property.PropertyType) || property.PropertyType.IsEnum)
                    {
                        Label endIfLabel = il.DefineLabel();

                        //判断是否包含此属性,不区分大小写，不包含则直接调到结束位置
                        il.Emit(OpCodes.Ldloc, currentRowBuilder);
                        il.Emit(OpCodes.Ldstr, property.Name);
                        il.Emit(OpCodes.Call, DataTableUtil.IsContainColumnMethod);
                        il.Emit(OpCodes.Brfalse, endIfLabel);

                        //获取此属性在dataRow中的值，并赋值
                        LoadDefaultObject(il, instanceType, instanceBuilder);
                        il.Emit(OpCodes.Ldloc, currentRowBuilder);
                        il.Emit(OpCodes.Ldstr, property.Name);
                        il.Emit(OpCodes.Call, DataTableUtil.GetColumnValueMethod);
                        //值进行转换
                        ConvertValue(il, property.PropertyType);
                        SetObjectValue(il, instanceType, setMethod);

                        //结束
                        il.MarkLabel(endIfLabel);
                    }
                    else if (property.PropertyType.IsArrayOrCollection())//数组类型
                    {
                    }
                    else//对象
                    {
                        LocalBuilder instanceProperty = il.DeclareLocal(property.PropertyType);
                        SetDefaultObjectValue(il, property.PropertyType, instanceProperty);//实例对象
                        SetDataRowToEntityIL(il, property.PropertyType, currentRowBuilder, instanceProperty);
                        LoadDefaultObject(il, instanceType, instanceBuilder);
                        il.Emit(OpCodes.Ldloc, instanceProperty);
                        SetObjectValue(il, instanceType, setMethod);
                    }
                }
            }
        }

        /// <summary>
        /// 设置DbNull的IL
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public static LocalBuilder SetDbNullIL(this ILGenerator il)
        {
            LocalBuilder dbNullBuilder = il.DeclareLocal(TypeUtil._ObjectType);
            il.Emit(OpCodes.Ldsfld, typeof(DBNull).GetField("Value"));
            il.Emit(OpCodes.Stloc, dbNullBuilder);
            return dbNullBuilder;
        }

        /// <summary>
        /// 执行设置整数值的emit代码
        /// </summary>
        /// <param name="il"></param>
        /// <param name="value"></param>
        public static void LdcInt(this ILGenerator il, int value)
        {
            switch (value)
            {
                case -1:
                    il.Emit(OpCodes.Ldc_I4_M1);
                    return;

                case 0:
                    il.Emit(OpCodes.Ldc_I4_0);
                    return;

                case 1:
                    il.Emit(OpCodes.Ldc_I4_1);
                    return;

                case 2:
                    il.Emit(OpCodes.Ldc_I4_2);
                    return;

                case 3:
                    il.Emit(OpCodes.Ldc_I4_3);
                    return;

                case 4:
                    il.Emit(OpCodes.Ldc_I4_4);
                    return;

                case 5:
                    il.Emit(OpCodes.Ldc_I4_5);
                    return;

                case 6:
                    il.Emit(OpCodes.Ldc_I4_6);
                    return;

                case 7:
                    il.Emit(OpCodes.Ldc_I4_7);
                    return;

                case 8:
                    il.Emit(OpCodes.Ldc_I4_8);
                    return;
            }

            if (value > -129 && value < 128)
                il.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
            else
                il.Emit(OpCodes.Ldc_I4, value);
        }

        /// <summary>
        /// 执行设置类型转换的IL
        /// </summary>
        /// <param name="il"></param>
        /// <param name="objType"></param>
        public static void SetObjectCastIL(this ILGenerator il, Type objType)
        {
            if (objType.IsValueType)
            {
                il.Emit(OpCodes.Unbox_Any, objType);
            }
            else
            {
                il.Emit(OpCodes.Castclass, objType);
            }
        }

        public static void SetGetValueIL(this ILGenerator il, PropertyInfo property, MethodInfo getMethod)
        {
            if (property.DeclaringType.IsValueType && Nullable.GetUnderlyingType(property.DeclaringType) == null)
            {
                var t = il.DeclareLocal(property.DeclaringType);
                il.Emit(OpCodes.Stloc, t);
                il.Emit(OpCodes.Ldloca_S, t);
            }
            if (property.DeclaringType.IsValueType)
            {
                il.Emit(OpCodes.Call, getMethod);
            }
            else
            {
                il.Emit(OpCodes.Callvirt, getMethod);
            }
        }
    }
}