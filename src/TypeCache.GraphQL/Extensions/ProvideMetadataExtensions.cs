using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Types;
using GraphQLParser.AST;

namespace TypeCache.GraphQL.Extensions;

internal static class ProvideMetadataExtensions
{
	private const string AST_METAFIELD = "__AST_MetaField__"; // TODO: possible remove
	private const string DIRECTIVES_KEY = "__APPLIED__DIRECTIVES__";
	private const string EXTENSION_AST_METAFIELD = "__EXTENSION_AST_MetaField__"; // TODO: possible remove

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
		where TMetadataProvider : IProvideMetadata => provider.ApplyDirective(name, _ => { });

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
		if (configure == null)
			throw new ArgumentNullException(nameof(configure));

		var directive = new AppliedDirective(name);
		configure(directive);

		var directives = provider.GetAppliedDirectives() ?? new List<AppliedDirective>(0);
		directives.Add(directive);

		provider.Metadata[DIRECTIVES_KEY] = directives;

		return provider;
	}

	public static bool IsExtensionType(this IProvideMetadata @this)
		=> @this.HasExtensionAstTypes() && !@this.AstTypeHasFields();

	public static bool AstTypeHasFields(this IProvideMetadata @this)
		=> (GetAstType<ASTNode>(@this) as IHasFieldsDefinitionNode)?.Fields?.Any() is true;

	public static T? GetAstType<T>(this IProvideMetadata @this)
		=> @this.GetMetadata<T>(AST_METAFIELD);

	public static TMetadataProvider SetAstType<TMetadataProvider>(this TMetadataProvider @this, ASTNode node) // TODO: possible remove
		where TMetadataProvider : IProvideMetadata

	{
		@this.WithMetadata(AST_METAFIELD, node); //TODO: remove?

		if (node is IHasDirectivesNode ast)
			@this.CopyDirectivesFrom(ast);

		return @this;
	}

	public static TMetadataProvider CopyDirectivesFrom<TMetadataProvider>(this TMetadataProvider @this, IHasDirectivesNode node)
		where TMetadataProvider : IProvideMetadata
	{
		if (node.Directives?.Count > 0)
		{
			foreach (var directive in node.Directives)
			{
				@this.ApplyDirective(directive!.Name.StringValue, //ISSUE:allocation
					_ =>
					{
						if (directive.Arguments?.Count > 0)
						{
							foreach (var arg in directive.Arguments)
								_.AddArgument(new DirectiveArgument(arg.Name.StringValue) { Value = arg.Value.ParseAnyLiteral() }); //ISSUE:allocation
						}
					});
			}
		}

		return @this;
	}

	public static bool HasExtensionAstTypes(this IProvideMetadata @this)
		=> @this.HasMetadata(EXTENSION_AST_METAFIELD) && GetExtensionAstTypes(@this).Count > 0;

	public static void AddExtensionAstType<T>(this IProvideMetadata type, T astType)
		where T : ASTNode
	{
		var types = GetExtensionAstTypes(type);
		types.Add(astType);
		type.Metadata[EXTENSION_AST_METAFIELD] = types;

		if (astType is IHasDirectivesNode ast)
			type.CopyDirectivesFrom(ast);
	}

	public static List<ASTNode> GetExtensionAstTypes(this IProvideMetadata type)
		=> type.GetMetadata(EXTENSION_AST_METAFIELD, () => new List<ASTNode>(0))!;

	public static IEnumerable<GraphQLDirective> GetExtensionDirectives<T>(this IProvideMetadata @this)
		where T : ASTNode
		=> @this.GetExtensionAstTypes()
			.OfType<IHasDirectivesNode>()
			.Where(_ => _.Directives is not null)
			.SelectMany(_ => _.Directives!);
}
