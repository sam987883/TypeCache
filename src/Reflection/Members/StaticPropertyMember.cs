// Copyright (c) 2020 Samuel Abraham

using System.Reflection;

namespace Sam987883.Reflection.Members
{
	internal sealed class StaticPropertyMember : Member, IStaticPropertyMember
	{
		public StaticPropertyMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			this.GetMethod = propertyInfo.GetMethod != null ? new StaticMethodMember(propertyInfo.GetMethod) : null;
			this.SetMethod = propertyInfo.SetMethod != null ? new StaticMethodMember(propertyInfo.SetMethod) : null;
		}

		public IStaticMethodMember? GetMethod { get; }

		public IStaticMethodMember? SetMethod { get; }

		public object? Value
		{
			get => this.GetMethod?.Invoke();
			set => this.SetMethod?.Invoke(value);
		}
	}
}
