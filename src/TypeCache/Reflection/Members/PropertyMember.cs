// Copyright (c) 2021 Samuel Abraham

using System.Reflection;
using static TypeCache.Extensions.IEnumerableExtensions;
using static TypeCache.Extensions.ObjectExtensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class PropertyMember : Member, IPropertyMember
	{
		public PropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", false);

			this.GetMethod = propertyInfo.GetMethod != null ? new MethodMember(propertyInfo.GetMethod) : null;
			this.SetMethod = propertyInfo.SetMethod != null ? new MethodMember(propertyInfo.SetMethod) : null;
		}

		public object? this[object instance]
		{
			get => this.GetMethod?.Invoke(instance, null);
			set => this.SetMethod?.Invoke(instance, new[] { value });
		}

		public IMethodMember? GetMethod { get; }

		public IMethodMember? SetMethod { get; }
	}
}
