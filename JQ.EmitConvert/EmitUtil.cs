using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;

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
        public static DynamicMethod CreateObjectToParamListMethod<TParam>(Type objType) where TParam : DbParameter
        {
            var listType = typeof(List<TParam>);
            var instanceType = typeof(TParam);
            //三个参数，第一个参数是object，第二个参数是参数符号（@,:,?），参数前缀
            DynamicMethod convertMethod = new DynamicMethod("ConvertObjectToParamList" + objType.Name, listType, new Type[] { TypeUtil._ObjectType, TypeUtil._StringType, TypeUtil._StringType }, true);

            ILGenerator il = convertMethod.GetILGenerator();
            LocalBuilder listBuilder = il.DeclareLocal(listType);//List<T> 存储对象
            il.SetDefaultObjectValue(listType, listBuilder);

            LocalBuilder objBuilder = il.DeclareLocal(objType);
            il.Emit(OpCodes.Ldarg_0);
            il.SetObjectCastIL(objType);
            il.Emit(OpCodes.Stloc, objBuilder);

            LocalBuilder dbNullBuilder = il.SetDbNullIL();

            List<PropertyInfo> properties = PropertyUtil.GetTypeProperties(objType);
            foreach (var item in properties)
            {
                Label setDbNullLable = il.DefineLabel();//设置dbnull
                Label setParamLable = il.DefineLabel();//给param赋值
                Label exitLable = il.DefineLabel();//退出

                LocalBuilder paramNameBuilder = il.DeclareLocal(TypeUtil._StringType);//参数变量名字
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Ldarg_2);
                il.Emit(OpCodes.Ldstr, item.Name);
                il.Emit(OpCodes.Call, TypeUtil._StringType.GetMethod("Concat", new Type[] { TypeUtil._StringType, TypeUtil._StringType, TypeUtil._StringType }));
                il.Emit(OpCodes.Stloc, paramNameBuilder);

                LocalBuilder instanceBuilder = il.DeclareLocal(instanceType);// T 存储对象
                il.SetDefaultObjectValue(instanceType, instanceBuilder);

                LocalBuilder dbTypeBuilder = il.DeclareLocal(TypeUtil._DbTypeType);
                il.LdcInt((int)TypeUtil.Type2DbType(item.PropertyType));//数据库类型
                il.Emit(OpCodes.Stloc, dbTypeBuilder);

                //设置dbType
                il.Emit(OpCodes.Ldloc, instanceBuilder);
                il.Emit(OpCodes.Ldloc, dbTypeBuilder);
                il.Emit(OpCodes.Callvirt, instanceType.GetMethod("set_DbType", new Type[] { TypeUtil._DbTypeType }));
                //设置ParameterName
                il.Emit(OpCodes.Ldloc, instanceBuilder);
                il.Emit(OpCodes.Ldloc, paramNameBuilder);
                il.Emit(OpCodes.Callvirt, instanceType.GetMethod("set_ParameterName", new Type[] { TypeUtil._StringType }));

                il.Emit(OpCodes.Nop);

                LocalBuilder dbValueBuilder = il.DeclareLocal(TypeUtil._ObjectType);
                var getMethod = item.GetGetMethod(true);
                if (getMethod == null)
                {
                    il.Emit(OpCodes.Br_S, setDbNullLable);
                }
                else
                {
                    il.Emit(OpCodes.Ldloc, objBuilder);
                    if (item.DeclaringType.IsValueType && Nullable.GetUnderlyingType(item.DeclaringType) == null)
                    {
                        var t = il.DeclareLocal(item.DeclaringType);
                        il.Emit(OpCodes.Stloc, t);
                        il.Emit(OpCodes.Ldloca_S, t);
                    }
                    if (item.DeclaringType.IsValueType)
                    {
                        il.Emit(OpCodes.Call, getMethod);
                    }
                    else
                    {
                        il.Emit(OpCodes.Callvirt, getMethod);
                    }

                    if (item.PropertyType.IsValueType)
                    {
                        il.Emit(OpCodes.Box, item.PropertyType);
                        if (item.PropertyType.IsEnum)
                        {
                            il.Emit(OpCodes.Ldstr, "d");
                            il.Emit(OpCodes.Call, typeof(Enum).GetMethod("ToString", new Type[] { TypeUtil._StringType }));
                        }
                    }
                    il.Emit(OpCodes.Stloc, dbValueBuilder);
                    il.Emit(OpCodes.Ldloc, dbValueBuilder);
                    il.Emit(OpCodes.Ldnull);
                    il.Emit(OpCodes.Beq_S, setDbNullLable);
                    il.Emit(OpCodes.Br_S, setParamLable);
                }
                il.MarkLabel(setDbNullLable);
                il.Emit(OpCodes.Ldloc, dbNullBuilder);
                il.Emit(OpCodes.Stloc, dbValueBuilder);
                il.Emit(OpCodes.Br_S, setParamLable);

                il.MarkLabel(setParamLable);
                il.Emit(OpCodes.Ldloc, instanceBuilder);
                il.Emit(OpCodes.Ldloc, dbValueBuilder);
                il.Emit(OpCodes.Callvirt, instanceType.GetMethod("set_Value", new Type[] { TypeUtil._ObjectType }));

                il.Emit(OpCodes.Ldloc, listBuilder);
                il.Emit(OpCodes.Ldloc, instanceBuilder);
                il.Emit(OpCodes.Call, listType.GetMethod("Add"));
            }
            il.Emit(OpCodes.Ldloc, listBuilder);
            il.Emit(OpCodes.Ret);
            return convertMethod;
        }

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
            DynamicMethod convertMethod = new DynamicMethod("ConvertListMethod" + instanceType.Name, listType, new Type[] { typeof(DataTable) }, true);

            ILGenerator il = convertMethod.GetILGenerator();

            LocalBuilder listBuilder = il.DeclareLocal(listType);//List<T> 存储对象

            LocalBuilder currentRowBuilder = il.DeclareLocal(typeof(DataRow));// T 存储对象
            LocalBuilder totalCountBuilder = il.DeclareLocal(TypeUtil._IntType);// table 总记录数
            LocalBuilder currentIndexBuilder = il.DeclareLocal(TypeUtil._IntType);// 当前table.Rows的索引

            Label exit = il.DefineLabel();//退出循环
            Label loop = il.DefineLabel();//循环体

            il.SetDefaultObjectValue(listType, listBuilder);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowMethod);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowsCountMethod);
            il.Emit(OpCodes.Stloc_S, totalCountBuilder);

            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_S, currentIndexBuilder);

            il.MarkLabel(loop);//开始循环
            LocalBuilder instanceBuilder = il.DeclareLocal(instanceType);
            il.SetDefaultObjectValue(instanceType, instanceBuilder);//default(T);

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowMethod);
            il.Emit(OpCodes.Ldloc, currentIndexBuilder);
            il.Emit(OpCodes.Callvirt, DataTableUtil.GetRowItemMethod);//table.Rows[i]
            il.Emit(OpCodes.Stloc_S, currentRowBuilder);

            il.SetDataRowToEntityIL(instanceType, currentRowBuilder, instanceBuilder);

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
            il.SetDefaultObjectValue(instanceType, instanceBuilder);//default(T);

            LocalBuilder currentRowBuilder = il.DeclareLocal(typeof(DataRow));// DataRow 存储对象

            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Stloc_S, currentRowBuilder);

            il.SetDataRowToEntityIL(instanceType, currentRowBuilder, instanceBuilder);
            il.LoadDefaultObject(instanceType, instanceBuilder);
            il.Emit(OpCodes.Ret);
            return convertMethod;
        }

        #endregion 根据类型创建Datarow转Model的方法

        public static DynamicMethod CreateClassCastMethod<TFrom, TTartget>()
        {
            var typeFrom = typeof(TFrom);
            var typeTarget = typeof(TTartget);
            DynamicMethod method = new DynamicMethod("ClassCast" + typeFrom.Name, typeTarget, new Type[] { typeFrom }, true);

            ILGenerator il = method.GetILGenerator();

            var targetLocalBuilder = il.DeclareLocal(typeTarget);
            il.SetDefaultObjectValue(typeTarget, targetLocalBuilder);

            var fromPropertyList = PropertyUtil.GetTypeProperties(typeFrom);
            var targetPropertyList = PropertyUtil.GetTypeProperties(typeTarget);

            foreach (var targetProperty in targetPropertyList)
            {
                var formProperty = fromPropertyList.Where(m => m.Name.Equals(targetProperty.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                if (formProperty != null)
                {
                    var getMethod = formProperty.GetGetMethod(true);
                    if (getMethod != null)
                    {
                        var setMehtod = targetProperty.GetSetMethod(true);
                        if (setMehtod != null)
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.SetGetValueIL(formProperty, getMethod);
                        }
                    }
                }
            }

            return method;
        }
    }
}