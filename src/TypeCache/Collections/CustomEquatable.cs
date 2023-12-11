// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using static System.Reflection.BindingFlags;

namespace TypeCache.Collections;

public abstract class CustomEquatable<T>(Func<T?, bool> equals)
	: IEquatable<T>
	where T : class
{
	private readonly Func<T?, bool> _Equals = equals ?? equals.ThrowArgumentNullException();

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public bool Equals(T? other)
		=> this._Equals(other);

	[MethodImpl(AggressiveInlining), DebuggerHidden]
	public override bool Equals(object? other)
		=> this._Equals(other as T);

	public override int GetHashCode()
	{
		var fieldInfos = this.GetType().GetFields(DeclaredOnly | Instance | NonPublic | Public);
		if (fieldInfos.Length == 0)
			return base.GetHashCode();

		var values = fieldInfos.Select(fieldInfo => fieldInfo.GetFieldValue(this)).ToArray();
		var hashCode = new HashCode();
		values.ForEach(hashCode.Add);
		return hashCode.ToHashCode();
	}
}
