// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Reflection.Members
{
	internal sealed class PropertyMember : Member, IPropertyMember
	{
		public PropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			this.Getter = propertyInfo.GetMethod != null ? new MethodMember(propertyInfo.GetMethod) : null;
			this.Setter = propertyInfo.SetMethod != null ? new MethodMember(propertyInfo.SetMethod) : null;
		}

		public object? this[object instance]
		{
			get => this.Getter?.Invoke(instance, null);
			set => this.Setter?.Invoke(instance, new[] { value });
		}

		public IMethodMember? Getter { get; }

		public IMethodMember? Setter { get; }
	}
}
