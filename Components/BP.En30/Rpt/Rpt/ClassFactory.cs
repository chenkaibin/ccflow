using System;
using System.Reflection;
using System.Collections;


namespace BP.Report
{
	public class ClassFactory
	{
		/// <summary>
		/// 设置对象实例上指定属性的值
		/// </summary>
		/// <param name="obj">对象实例</param>
		/// <param name="propertyName">属性名，属性为非静态特性</param>
		/// <param name="val">值</param>
		public static void SetValue(object obj ,string propertyName ,object val)
		{
			Type tp = obj.GetType();
			PropertyInfo p = tp.GetProperty( propertyName);
			if( p==null)
				throw new Exception( "设置属性值失败！类型["+tp+"]没有属性["+propertyName+"]");
			p.SetValue( obj ,val ,null);
		}
		/// <summary>
		/// 获取对象实例上指定属性的值
		/// </summary>
		/// <param name="obj">对象实例</param>
		/// <param name="propertyName">属性名</param>
		/// <returns>值</returns>
		public static object GetValue(object obj ,string propertyName)
		{
			Type tp = obj.GetType();
			PropertyInfo p = tp.GetProperty( propertyName);
			if( p==null)
				throw new Exception( "获取属性值失败！类型["+tp+"]没有属性["+propertyName+"]");
			object val = p.GetValue( obj ,null);
			return val;
		}
		/// <summary>
		/// 获取对象实例上指定属性的值，转换为string
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="propertyName"></param>
		/// <returns>值</returns>
		public static string GetValueToStr(object obj ,string propertyName)
		{
			object val = GetValue(obj,propertyName);
			if(val==null)
				return "";
			else
				return val.ToString();
		}

	}
}