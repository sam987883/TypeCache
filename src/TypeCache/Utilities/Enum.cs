// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Immutable;
using TypeCache.Collections;
using TypeCache.Extensions;

namespace TypeCache.Utilities;

public static class Enum<T>
	where T : struct, Enum
{
	static Enum()
	{
		var attributes = typeof(T).GetCustomAttributes(false);
		Attributes = attributes.Length > 0 ? attributes.Cast<Attribute>().ToImmutableArray() : ImmutableArray<Attribute>.Empty;
		Flags = attributes.Any<FlagsAttribute>();
		Name = typeof(T).Name;
		Tokens = new TokenCollection();
	}

	[DebuggerHidden]
	public static IReadOnlyList<Attribute> Attributes { get; }

	[DebuggerHidden]
	public static bool Flags { get; }

	[DebuggerHidden]
	public static string Name { get; }

	[DebuggerHidden]
	public static TokenCollection Tokens { get; }

	[DebuggerHidden]
	public static Type UnderlyingType => typeof(T).GetEnumUnderlyingType();

	public class TokenCollection : IReadOnlyList<Token<T>>
	{
		private readonly IReadOnlyList<Token<T>> _Tokens;

		internal TokenCollection()
		{
			this._Tokens = Enum.GetValues<T>().Select(value => new Token<T>(value)).ToImmutableArray();
		}

		public Token<T> this[int index] => this._Tokens[index];

		public Token<T>? this[T value]
		{
			get
			{
				var comparer = new EnumComparer<T>();
				return this._Tokens.FirstOrDefault(token => comparer.EqualTo(token.Value, value));
			}
		}

		public Token<T>? this[string name, StringComparison comparison = StringComparison.OrdinalIgnoreCase]
			=> this._Tokens.FirstOrDefault(token => token.Name.Equals(name, comparison));

		public int Count => this._Tokens.Count;

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		public IEnumerator<Token<T>> GetEnumerator()
			=> this._Tokens.GetEnumerator();

		[MethodImpl(AggressiveInlining), DebuggerHidden]
		IEnumerator IEnumerable.GetEnumerator()
			=> ((IEnumerable)this._Tokens).GetEnumerator();
	}
}
