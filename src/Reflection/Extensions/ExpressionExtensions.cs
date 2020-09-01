// Copyright (c) 2020 Samuel Abraham

using Sam987883.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Sam987883.Reflection.Extensions
{
	public static class ExpressionExtensions
	{
		public static Expression SystemConvert(this Expression @this, Type type) => type.ToNativeType() switch
		{
			NativeType.Boolean => Expression.Call(typeof(Convert), nameof(Convert.ToBoolean), null, @this),
			NativeType.Char => Expression.Call(typeof(Convert), nameof(Convert.ToChar), null, @this),
			NativeType.SByte => Expression.Call(typeof(Convert), nameof(Convert.ToSByte), null, @this),
			NativeType.Byte => Expression.Call(typeof(Convert), nameof(Convert.ToByte), null, @this),
			NativeType.Int16 => Expression.Call(typeof(Convert), nameof(Convert.ToInt16), null, @this),
			NativeType.UInt16 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt16), null, @this),
			NativeType.Int32 => Expression.Call(typeof(Convert), nameof(Convert.ToInt32), null, @this),
			NativeType.UInt32 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt32), null, @this),
			NativeType.Int64 => Expression.Call(typeof(Convert), nameof(Convert.ToInt64), null, @this),
			NativeType.UInt64 => Expression.Call(typeof(Convert), nameof(Convert.ToUInt64), null, @this),
			NativeType.Single => Expression.Call(typeof(Convert), nameof(Convert.ToSingle), null, @this),
			NativeType.Double => Expression.Call(typeof(Convert), nameof(Convert.ToDouble), null, @this),
			NativeType.Decimal => Expression.Call(typeof(Convert), nameof(Convert.ToDecimal), null, @this),
			NativeType.DateTime => Expression.Call(typeof(Convert), nameof(Convert.ToDateTime), null, @this),
			NativeType.String => Expression.Call(typeof(Convert), nameof(Convert.ToString), null, @this),
			_ => @this.Cast(type)
		};

		public static IEnumerable<Expression> ToParameterArray(this ParameterExpression @this, params ParameterInfo[] parameterInfos) =>
			parameterInfos.To(parameterInfo => @this.ArrayAccess(parameterInfo.Position).SystemConvert(parameterInfo.ParameterType));
	}
}
