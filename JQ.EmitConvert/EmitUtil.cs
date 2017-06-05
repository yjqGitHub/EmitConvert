using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace JQ.EmitConvert
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：EmitUtil.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：EmitUtil
    /// 创建标识：yjq 2017/6/5 19:46:52
    /// </summary>
    public static class EmitUtil
    {
        #region 根据类型创建Table转List的方法

        /// <summary>
        /// 根据类型创建Table转List的方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static DynamicMethod CreateTableToListMethod<T>()
        {
            var listType = typeof(List<T>);
            var instanceType = typeof(T);
            DynamicMethod convertMethod = new DynamicMethod("ConvertListMethod" + instanceType.Name, listType, new Type[] { typeof(DataTable) });

            ILGenerator il = convertMethod.GetILGenerator();

            LocalBuilder listBuilder = il.DeclareLocal(listType);//List<T> 存储对象

            LocalBuilder currentRowBuilder = il.DeclareLocal(typeof(DataRow));// T 存储对象
            LocalBuilder totalCountBuilder = il.DeclareLocal(typeof(int));// table 总记录数
            LocalBuilder currentIndexBuilder = il.DeclareLocal(typeof(int));// 当前table.Rows的索引

            Label exit = il.DefineLabel();//退出循环
            Label loop = il.DefineLabel();//循环体

            il.Emit(OpCodes.Newobj, listType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, listBuilder);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowMethod);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowsCountMethod);
            il.Emit(OpCodes.Stloc_S, totalCountBuilder);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_S, currentIndexBuilder);

            il.MarkLabel(loop);//开始循环
            LocalBuilder instanceBuilder = il.DeclareLocal(instanceType);// T 存储对象
            il.Emit(OpCodes.Newobj, instanceType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, instanceBuilder);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowMethod);
            il.Emit(OpCodes.Ldloc, currentIndexBuilder);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowItemMethod);//table.Rows[i]
            il.Emit(OpCodes.Stloc_S, currentRowBuilder);

            SetDataRowIL(instanceType, il, currentRowBuilder, instanceBuilder);

            il.Emit(OpCodes.Ldloc, listBuilder);
            il.Emit(OpCodes.Ldloc, instanceBuilder);
            il.Emit(OpCodes.Call, listType.GetMethod("Add"));

            //i++
            il.Emit(OpCodes.Ldloc, currentIndexBuilder);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_S, currentIndexBuilder);
            //i<table.Rows.Count
            il.Emit(OpCodes.Ldloc, currentIndexBuilder);
            il.Emit(OpCodes.Ldloc, totalCountBuilder);
            il.Emit(OpCodes.Clt);
            il.Emit(OpCodes.Brtrue, loop);

            il.Emit(OpCodes.Ldloc, listBuilder);
            il.Emit(OpCodes.Ret);

            return convertMethod;
        }

        #endregion 根据类型创建Table转List的方法

        #region 根据类型创建Datarow转Model的方法

        /// <summary>
        /// 根据类型创建Datarow转Model的方法
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        public static DynamicMethod CreateDataRowToEntityMethod<T>()
        {
            var instanceType = typeof(T);
            DynamicMethod convertMethod = new DynamicMethod("ConvertMethod" + instanceType.Name, instanceType, new Type[] { typeof(DataRow) }, true);

            ILGenerator il = convertMethod.GetILGenerator();
            LocalBuilder instanceBuilder = il.DeclareLocal(instanceType);// T 存储对象
            LocalBuilder currentRowBuilder = il.DeclareLocal(typeof(DataRow));// DataRow 存储对象

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_S, currentRowBuilder);

            il.Emit(OpCodes.Newobj, instanceType.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_S, instanceBuilder);

            SetDataRowIL(instanceType, il, currentRowBuilder, instanceBuilder);
            il.Emit(OpCodes.Ldloc, instanceBuilder);
            il.Emit(OpCodes.Ret);
            return convertMethod;
        }

        #endregion 根据类型创建Datarow转Model的方法

        #region SetDataRowIL

        private static void SetDataRowIL(Type instanceType, ILGenerator il, LocalBuilder currentRowBuilder, LocalBuilder instanceBuilder)
        {
            List<PropertyInfo> properties = PropertyUtil.GetTypeProperties(instanceType);

            foreach (var item in properties.Where(m => TypeUtil._BaseTypes.Contains(m.PropertyType) || m.PropertyType.IsEnum))
            {
                Label endIfLabel = il.DefineLabel();

                //判断DataRow是否包含该属性

                il.Emit(OpCodes.Ldloc, currentRowBuilder);
                il.Emit(OpCodes.Ldstr, item.Name);
                il.Emit(OpCodes.Call, DataTableUtil.IsContainColumnMethod);
                il.Emit(OpCodes.Brfalse, endIfLabel);

                //获取该属性在datarow的值
                il.Emit(OpCodes.Ldloc, instanceBuilder);

                il.Emit(OpCodes.Ldloc, currentRowBuilder);
                il.Emit(OpCodes.Ldstr, item.Name);
                il.Emit(OpCodes.Call, DataTableUtil.GetColumnValueMethod);

                //设置值
                if (item.PropertyType.IsValueType)
                {
                    var convertMethod = ConvertUtil.GetMethodInfo(item.PropertyType);
                    if (convertMethod == null)
                    {
                        il.Emit(OpCodes.Unbox_Any, item.PropertyType);//如果是值类型就拆箱
                    }
                    else
                    {
                        il.Emit(OpCodes.Call, convertMethod);
                    }
                }
                else
                {
                    il.Emit(OpCodes.Castclass, item.PropertyType);
                }
                il.Emit(OpCodes.Callvirt, item.GetSetMethod(true));

                il.MarkLabel(endIfLabel);
            }

            foreach (var item in properties.Where(m => !(TypeUtil._BaseTypes.Contains(m.PropertyType) || m.PropertyType.IsEnum)))
            {
                if (item.PropertyType.IsArray || item.PropertyType.IsGenericType)
                {
                }
                else
                {
                    LocalBuilder instanceProperty = il.DeclareLocal(item.PropertyType);
                    il.Emit(OpCodes.Newobj, item.PropertyType.GetConstructor(Type.EmptyTypes));
                    il.Emit(OpCodes.Stloc, instanceProperty);
                    SetDataRowIL(item.PropertyType, il, currentRowBuilder, instanceProperty);

                    il.Emit(OpCodes.Ldloc, instanceBuilder);
                    il.Emit(OpCodes.Ldloc, instanceProperty);
                    il.Emit(OpCodes.Castclass, item.PropertyType);
                    il.Emit(OpCodes.Callvirt, item.GetSetMethod(true));
                }
            }
        }

        #endregion SetDataRowIL

        /// <summary>
        /// 设置整数值的emit代码
        /// </summary>
        /// <param name="il"></param>
        /// <param name="value"></param>
        private static void Ldc(ILGenerator il, int value)
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
    }
}