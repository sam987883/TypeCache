// Copyright (c) 2020 Samuel Abraham

using System.Collections.Generic;
using System.Reflection;
using static Sam987883.Common.Extensions.IEnumerableExtensions;
using static Sam987883.Common.Extensions.ObjectExtensions;

namespace Sam987883.Reflection.Members
{
	internal sealed class IndexerMember<T> : Member, IIndexerMember<T>
		where T : class
	{
		public IndexerMember(PropertyInfo propertyInfo) : base(propertyInfo)
		{
			propertyInfo.GetIndexParameters().Any().Assert($"{nameof(propertyInfo)}.{nameof(propertyInfo.GetIndexParameters)}().Any()", true);

			this.GetMethod = propertyInfo.GetMethod != null ? new MethodMember<T>(propertyInfo.GetMethod) : null;
			this.SetMethod = propertyInfo.SetMethod != null ? new MethodMember<T>(propertyInfo.SetMethod) : null;
		}

		public object? this[T instance, params object[] indexParameters]
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

		public IMethodMember<T>? GetMethod { get; }

		public IMethodMember<T>? SetMethod { get; }
	}
}
