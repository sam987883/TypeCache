// Copyright (c) 2021 Samuel Abraham

using System.Runtime.CompilerServices;
using GraphQL.Language.AST;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using TypeCache.Extensions;
using TypeCache.Security;
using TypeCache.Security.Extensions;

namespace TypeCache.GraphQL.Types
{
	/// <summary>
	/// Requires call to either one of:
	/// <list type="bullet">
	/// <item><see cref="IServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, byte[], byte[])"/></item>
	/// <item><see cref="IServiceCollectionExtensions.RegisterHashMaker(IServiceCollection, decimal, decimal)"/></item>
	/// </list>
	/// </summary>
	public class GraphHashIdType : ScalarGraphType
	{
		private readonly IHashMaker _HashMaker;

		public GraphHashIdType(IHashMaker hashMaker)
		{
			hashMaker.AssertNotNull(nameof(hashMaker));

			this._HashMaker = hashMaker;
			this.Name = "HashID";
			this.Description = "A hashed ID.";
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override object? ParseLiteral(IValue value)
			=> this.ParseValue(value.Value);

		public override object? ParseValue(object? value)
			=> value switch
			{
				int id => id,
				long id => id,
				string hashId => this._HashMaker.Decrypt(hashId),
				_ => value
			};

		public override object? Serialize(object? value)
			=> value switch
			{
				int id => this._HashMaker.Encrypt(id),
				long id => this._HashMaker.Encrypt(id),
				string hashId => this._HashMaker.Decrypt(hashId),
				_ => value
			};
	}
}
