// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Collections;

public abstract class CustomEquatable<T> : IEquatable<T>
	where T : class
{
	private readonly Func<T?, bool> _Equals;
	private readonly int _HashCode;

	/// <summary>
	/// <code>
	/// {<br/>
	/// <see langword="    "/><paramref name="customEquatableEquals"/>.AssertNotNull();<br/>
	/// <see langword="    "/><paramref name="customEqualityFactors"/>.AssertNotEmpty();<br/>
	/// <br/>
	/// <see langword="    this"/>._Equals = <paramref name="customEquatableEquals"/>;<br/>
	/// <br/>
	/// <see langword="    var"/> hashCode = <see langword="new"/> <see cref="HashCode"/>();<br/>
	/// <see langword="    "/><paramref name="customEqualityFactors"/>.ForEach(hashCode.Add);<br/>
	/// <see langword="    this"/>._HashCode = hashCode.ToHashCode();<br/>
	/// }
	/// </code>
	/// </summary>
	/// <param name="customEquatableEquals">Implementation for <see cref="IEquatable{T}.Equals(T?)"/>.</param>
	/// <param name="customEqualityFactors">Values to use for generating hashCode for <see cref="object.GetHashCode"/>.</param>
	public CustomEquatable(Func<T?, bool> customEquatableEquals, params object[] customEqualityFactors)
	{
		customEquatableEquals.AssertNotNull();
		customEqualityFactors.AssertNotEmpty();

		this._Equals = customEquatableEquals;

		var hashCode = new HashCode();
		customEqualityFactors.ForEach(hashCode.Add);
		this._HashCode = hashCode.ToHashCode();
	}

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(T? other)
		=> this._Equals(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals(object? other)
		=> this._Equals(other as T);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override int GetHashCode()
		=> this._HashCode;
}
