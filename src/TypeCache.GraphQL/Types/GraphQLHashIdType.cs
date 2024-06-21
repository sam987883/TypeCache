// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using GraphQLParser.AST;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.Utilities;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Requires call to either one of:
/// <list type="bullet">
/// <item><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddHashMaker(IServiceCollection, byte[], byte[])"/></item>
/// <item><see cref="TypeCache.Extensions.ServiceCollectionExtensions.AddHashMaker(IServiceCollection, decimal, decimal)"/></item>
/// </list>
/// </summary>
public sealed class GraphQLHashIdType : ScalarGraphType
{
	private readonly IHashMaker _HashMaker;

	/// <exception cref="ArgumentNullException"/>
	public GraphQLHashIdType(IHashMaker hashMaker)
	{
		hashMaker.ThrowIfNull();

		this._HashMaker = hashMaker;
		this.Name = "HashID";
		this.Description = "A hashed ID.";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value is GraphQLNullValue || value is GraphQLStringValue;

	public override bool CanParseValue(object? value)
		=> value is null || value is Guid || value is string;

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override object? ParseLiteral(GraphQLValue value)
		=> this.ParseValue(value);

	public override object? ParseValue(object? value)
		=> value switch
		{
			GraphQLNullValue => null,
			GraphQLStringValue text => this._HashMaker.Decrypt((string)text.Value),
			_ => value
		};

	public override object? Serialize(object? value)
		=> value switch
		{
			int id => this._HashMaker.Encrypt(id).ToString(),
			long id => this._HashMaker.Encrypt(id).ToString(),
			_ => value
		};
}
