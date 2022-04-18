// Copyright (c) 2021 Samuel Abraham

using System;
using System.Runtime.CompilerServices;
using GraphQL.Types;
using GraphQLParser.AST;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.Security;
using static TypeCache.Default;

namespace TypeCache.GraphQL.Types;

/// <summary>
/// Requires call to either one of:
/// <list type="bullet">
/// <item><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, byte[], byte[])"/></item>
/// <item><see cref="TypeCache.Extensions.ServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, decimal, decimal)"/></item>
/// </list>
/// </summary>
public class GraphQLHashIdType : ScalarGraphType
{
	private readonly IHashMaker _HashMaker;

	/// <exception cref="ArgumentNullException"/>
	public GraphQLHashIdType(IHashMaker hashMaker)
	{
		hashMaker.AssertNotNull();

		this._HashMaker = hashMaker;
		this.Name = "HashID";
		this.Description = "A hashed ID.";
	}

	public override bool CanParseLiteral(GraphQLValue value)
		=> value is GraphQLNullValue || value is GraphQLIntValue || value is GraphQLStringValue;

	public override bool CanParseValue(object? value)
		=> value is null || value is int || value is long || value is Guid || value is string;

	[MethodImpl(METHOD_IMPL_OPTIONS)]
	public override object? ParseLiteral(GraphQLValue value)
		=> this.ParseValue(value);

	public override object? ParseValue(object? value)
		=> value switch
		{
			Guid hashId => this._HashMaker.Decrypt(hashId.ToByteArray()),
			string hashId => this._HashMaker.Decrypt(hashId),
			_ => value
		};

	public override object? Serialize(object? value)
		=> value switch
		{
			int id => this._HashMaker.Encrypt(id),
			long id => this._HashMaker.Encrypt(id),
			_ => value
		};
}
