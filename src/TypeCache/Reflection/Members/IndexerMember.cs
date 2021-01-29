// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Reflection;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Reflection.Members
{
	internal sealed class IndexerMember : Member, IIndexerMember
	{
		public IndexerMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", true);

			this.GetMethod = propertyInfo.GetMethod != null ? new MethodMember(propertyInfo.GetMethod) : null;
			this.SetMethod = propertyInfo.SetMethod != null ? new MethodMember(propertyInfo.SetMethod) : null;
		}

		public object? this[object instance, params object[] indexParameters]
		{
			get => this.GetMethod?.Invoke(instance, indexParameters);
			set
			{
				if (this.SetMethod != null)
				{
					var list = new List<object?>(indexParameters.Length + 1) { value };
					list.AddRange(indexParameters);
					this.SetMethod.Invoke(instance, list.ToArray());
				}
			}
		}

		public IMethodMember? GetMethod { get; }

		public IMethodMember? SetMethod { get; }
	}
}
