namespace EmitConvert.Test
{
    /// <summary>
    /// Copyright (C) 2017 yjq 版权所有。
    /// 类名：PropertyReflectUtilRowToEntityTest.cs
    /// 类属性：公共类（非静态）
    /// 类功能描述：PropertyReflectUtilRowToEntityTest
    /// 创建标识：yjq 2017/6/5 20:41:22
    /// </summary>
    public sealed class PropertyReflectUtilRowToEntityTest : IAction
    {
        public void Action()
        {
            PropertyReflectUtil.ToEntity<User>(User.GetUserList(1).Rows[0]);
        }
    }
}