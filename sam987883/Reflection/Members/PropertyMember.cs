// Copyright (c) 2020 Samuel Abraham

using System.Reflection;
using static sam987883.Extensions.IEnumerableExtensions;
using static sam987883.Extensions.ObjectExtensions;

namespace sam987883.Reflection.Members
{
	internal sealed class PropertyMember<T> : Member, IPropertyMember<T>
	{
		public PropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", false);

			this.GetMethod = propertyInfo.GetMethod != null ? new MethodMember<T>(propertyInfo.GetMethod) : null;
			this.SetMethod = propertyInfo.SetMethod != null ? new MethodMember<T>(propertyInfo.SetMethod) : null;
		}

		public object? this[T instance]
		{
			get => this.GetMethod?.Invoke(instance);
			set => this.SetMethod?.Invoke(instance, value);
		}

		public IMethodMember<T>? GetMethod { get; }

		public IMethodMember<T>? SetMethod { get; }
	}
}
