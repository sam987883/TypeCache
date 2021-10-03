using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using TypeCache.Reflection.Extensions;
using static TypeCache.Default;

namespace TypeCache.Reflection
{
	public static class LambdaExpressionFactory
	{
		public static LambdaExpression Create(ConstructorInfo constructorInfo)
		{
			var parameterInfos = constructorInfo.GetParameters();
			if (parameterInfos.Any())
			{
				parameterInfos.Sort(Default.ParameterPositionComparer);

				var parameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray();
				var lambdaExpression = constructorInfo.New(parameters).Lambda(parameters);
				return lambdaExpression;
			}
			else
			{
				var lambdaExpression = constructorInfo.New().Lambda();
				return lambdaExpression;
			}
		}

		public static LambdaExpression Create(MethodInfo methodInfo)
		{
			ParameterExpression instance = nameof(instance).Parameter(methodInfo.DeclaringType!);
			var parameterInfos = methodInfo.GetParameters();
			if (parameterInfos.Any())
			{
				parameterInfos.Sort(Default.ParameterPositionComparer);

				var parameters = parameterInfos.To(parameterInfo => parameterInfo!.Parameter()).ToArray();
				var lambdaExpression = methodInfo.IsStatic
					? methodInfo.CallStatic(parameters).Lambda(parameters)
					: instance.Call(methodInfo, parameters).Lambda(new[] { instance }.And(parameters));
				return lambdaExpression;
			}
			else
			{
				var lambdaExpression = methodInfo.IsStatic
					? methodInfo.CallStatic().Lambda()
					: instance.Call(methodInfo).Lambda(instance);
				return lambdaExpression;
			}
		}

		public static LambdaExpression Create(Type[] parameterTypes, Func<ParameterExpression[], Expression> bodyFactory)
		{
			var parameters = parameterTypes.To((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
			return bodyFactory(parameters).Lambda(parameters);
		}

		public static LambdaExpression CreateAction(Type[] parameterTypes, Func<ParameterExpression[], Expression> bodyFactory)
		{
			var parameters = parameterTypes.To((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
			return bodyFactory(parameters).LambdaAction(parameters);
		}

		public static LambdaExpression CreateFunc(Type[] parameterTypes, Type returnType, Func<ParameterExpression[], Expression> bodyFactory)
		{
			var parameters = parameterTypes.To((type, i) => $"parameter{i + 1}".Parameter(type)).ToArray();
			return bodyFactory(parameters).LambdaFunc(returnType, parameters);
		}

		public static LambdaExpression CreateGetter(FieldInfo fieldInfo)
			=> !fieldInfo.IsStatic
				? LambdaExpressionFactory.Create(new[] { fieldInfo.DeclaringType! }, parameters => parameters[0].Field(fieldInfo))
				: fieldInfo.StaticField().Lambda();

		public static LambdaExpression CreateSetter(FieldInfo fieldInfo)
		{
			fieldInfo.IsInitOnly.Assert($"{nameof(FieldInfo)}.{nameof(fieldInfo.IsInitOnly)}", false);
			fieldInfo.IsLiteral.Assert($"{nameof(FieldInfo)}.{nameof(fieldInfo.IsLiteral)}", false);

			return !fieldInfo.IsStatic
				? LambdaExpressionFactory.CreateAction(new[] { fieldInfo.DeclaringType!, fieldInfo.FieldType }, parameters => parameters[0].Field(fieldInfo).Assign(parameters[1]))
				: LambdaExpressionFactory.CreateAction(new[] { fieldInfo.FieldType }, parameters => fieldInfo.StaticField().Assign(parameters[0]));
		}
	}
}
