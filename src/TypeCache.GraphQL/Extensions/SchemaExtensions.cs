// Copyright (c) 2021 Samuel Abraham

using System.Data;
using GraphQL;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Extensions;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.Extensions;

public static partial class SchemaExtensions
{
	extension(ISchema @this)
	{
		/// <summary>
		/// Adds GraphQL endpoints based on <typeparamref name="T"/> controller public methods decorated with the following attributes:
		/// <list type="bullet">
		/// <item><see cref="GraphQLQueryAttribute"/></item>
		/// <item><see cref="GraphQLMutationAttribute"/></item>
		/// <item><see cref="GraphQLSubscriptionAttribute"/></item>
		/// </list>
		/// </summary>
		/// <typeparam name="T">The class containing the decorated public methods that will be converted into GraphQL endpoints.</typeparam>
		/// <returns>The added <see cref="FieldType"/>(s).</returns>
		public FieldType[] AddEndpoints<T>()
			where T : notnull
		{
			var methods = Type<T>.Methods.SelectMany(_ => _.Value).Where(_ => _.IsPublic).ToArray();
			var fieldTypes = new List<FieldType>();
			fieldTypes.AddRange(methods.Where(_ => _.Attributes.GraphQLQuery).Select(@this.AddQuery));
			fieldTypes.AddRange(methods.Where(_ => _.Attributes.GraphQLMutation).Select(@this.AddMutation));
			fieldTypes.AddRange(methods.Where(_ => _.Attributes.GraphQLSubscription).Select(@this.AddSubscription));

			var staticMethods = Type<T>.StaticMethods.SelectMany(_ => _.Value).Where(_ => _.IsPublic).ToArray();
			fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.GraphQLQuery).Select(@this.AddQuery));
			fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.GraphQLMutation).Select(@this.AddMutation));
			fieldTypes.AddRange(staticMethods.Where(_ => _.Attributes.GraphQLSubscription).Select(@this.AddSubscription));

			return fieldTypes.ToArray();
		}
	}

	private static string FixName(string name)
	{
		if (!name.Contains('_'))
			return name;

		return name.ToLowerInvariant().Split('_').Select(_ => _.ToPascalCase()).Concat();
	}
}
