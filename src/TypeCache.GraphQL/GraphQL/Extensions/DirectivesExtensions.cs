using GraphQL.Introspection;
using GraphQL.Types;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace GraphQL;

/// <summary>
/// Extension methods to configure directives applied to GraphQL elements: types, fields, arguments, etc.
/// </summary>
public static class DirectivesExtensions
{
	private const string DIRECTIVES_KEY = "__APPLIED__DIRECTIVES__";

	/// <summary>
	/// Indicates whether provider has any applied directives.
	/// Note that built-in @deprecated directive is not taken into account and ignored.
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// </summary>
	public static bool HasAppliedDirectives(this IProvideMetadata provider)
		=> provider.GetAppliedDirectives()?.Count > 0;

	/// <summary>
	/// Provides all directives applied to this provider if any. Otherwise returns <see langword="null"/>.
	/// Note that built-in @deprecated directive is not taken into account and ignored.
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// </summary>
	public static IList<AppliedDirective>? GetAppliedDirectives(this IProvideMetadata provider)
		=> provider.GetMetadata<IList<AppliedDirective>>(DIRECTIVES_KEY);

	/// <summary>
	/// Finds applied directive by its name from the specified provider if any. Otherwise returns <see langword="null"/>.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	public static AppliedDirective? FindAppliedDirective(this IProvideMetadata provider, string name)
		=> provider.GetAppliedDirectives()?.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(name));

	/// <summary>
	/// Apply directive without specifying arguments. If the directive declaration has arguments,
	/// then their default values (if any) will be used.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider ApplyDirective<TMetadataProvider>(this TMetadataProvider provider, string name)
		where TMetadataProvider : IProvideMetadata
		=> provider.ApplyDirective(name, _ => { });

	/// <summary>
	/// Apply directive specifying one argument. If the directive declaration has other arguments,
	/// then their default values (if any) will be used.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	/// <param name="argumentName">Argument name.</param>
	/// <param name="argumentValue">Argument value.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider ApplyDirective<TMetadataProvider>(this TMetadataProvider provider, string name, string argumentName, object? argumentValue)
		where TMetadataProvider : IProvideMetadata
		=> provider.ApplyDirective(name, directive => directive.AddArgument(new DirectiveArgument(argumentName) { Value = argumentValue }));

	/// <summary>
	/// Apply directive specifying two arguments. If the directive declaration has other arguments,
	/// then their default values (if any) will be used.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	/// <param name="argument1Name">First argument name.</param>
	/// <param name="argument1Value">First argument value.</param>
	/// <param name="argument2Name">Second argument name.</param>
	/// <param name="argument2Value">Second argument value.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider ApplyDirective<TMetadataProvider>(this TMetadataProvider provider, string name, string argument1Name, object? argument1Value, string argument2Name, object? argument2Value)
		where TMetadataProvider : IProvideMetadata
		=> provider.ApplyDirective(name, directive => directive
			.AddArgument(new DirectiveArgument(argument1Name) { Value = argument1Value })
			.AddArgument(new DirectiveArgument(argument2Name) { Value = argument2Value }));

	/// <summary>
	/// Apply directive with configuration delegate.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	/// <param name="configure">Configuration delegate.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider ApplyDirective<TMetadataProvider>(this TMetadataProvider provider, string name, Action<AppliedDirective> configure)
		where TMetadataProvider : IProvideMetadata
	{
		configure.ThrowIfNull();

		var directive = new AppliedDirective(name);
		configure(directive);

		var directives = provider.GetAppliedDirectives() ?? new List<AppliedDirective>(0);
		directives.Add(directive);

		provider.Metadata[DIRECTIVES_KEY] = directives;

		return provider;
	}

	/// <summary>
	/// Remove applied directive by its name.
	/// </summary>
	/// <param name="provider">
	/// Metadata provider. This can be an instance of <see cref="GraphType"/>,
	/// <see cref="FieldType"/>, <see cref="Schema"/> or others.
	/// </param>
	/// <param name="name">Directive name.</param>
	/// <returns>The reference to the specified <paramref name="provider"/>.</returns>
	public static TMetadataProvider RemoveAppliedDirective<TMetadataProvider>(this TMetadataProvider provider, string name)
		 where TMetadataProvider : IProvideMetadata
	{
		var directive = provider.GetAppliedDirectives()?.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(name));
		if (directive is not null)
			provider.GetAppliedDirectives()!.Remove(directive);

		return provider;
	}

	internal static string? GetDeprecationReason(this IProvideMetadata provider)
		=> provider.FindAppliedDirective("deprecated")?.FindArgument("reason")?.Value is string reason ? reason : null;

	internal static void SetDeprecationReason(this IProvideMetadata provider, string? reason)
	{
		if (reason is null)
			provider.RemoveAppliedDirective("deprecated");
		else
		{
			var deprecated = provider.FindAppliedDirective("deprecated");
			if (deprecated is null)
				provider.ApplyDirective("deprecated", "reason", reason);
			else
			{
				var arg = deprecated.FindArgument("reason");
				if (arg is not null)
					arg.Value = reason;
				else
					deprecated.AddArgument(new("reason") { Value = reason });
			}
		}
	}

	internal static void AddAppliedDirectivesField<TSourceType>(this ObjectGraphType<TSourceType> type, string element)
		where TSourceType : IProvideMetadata
	{
		type.Field<NonNullGraphType<ListGraphType<NonNullGraphType<__AppliedDirective>>>>("appliedDirectives")
			.Description($"Directives applied to the {element}")
			.Resolve(context =>
			{
				if (!context.Source!.HasAppliedDirectives())
					return Array<AppliedDirective>.Empty;

				return context.Source!.GetAppliedDirectives()!
					.Where(_ =>
					{
						var schemaDirective = context.Schema.Directives[_.Name];
						return schemaDirective is not null && context.Schema.Filter.AllowDirective(schemaDirective);
					})
					.OrderBy(_ => _.Name)
					.ToArray();
			});
	}
}
