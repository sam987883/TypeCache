// Copyright (c) 2021 Samuel Abraham

using System.Reflection;

namespace TypeCache.Reflection.Members
{
	internal sealed class StaticPropertyMember : Member, IStaticPropertyMember
	{
		public StaticPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			this.Getter = propertyInfo.GetMethod != null ? new StaticMethodMember(propertyInfo.GetMethod) : null;
			this.Setter = propertyInfo.SetMethod != null ? new StaticMethodMember(propertyInfo.SetMethod) : null;
		}

		public IStaticMethodMember? Getter { get; }

		public IStaticMethodMember? Setter { get; }

		public object? Value
		{
			get => this.Getter?.Invoke();
			set => this.Setter?.Invoke(value);
		}
	}
}
