// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Extensions;

public enum UnaryOperator
{
	/// <summary><c>i <see langword="is true"/></c></summary>
	IsTrue,
	/// <summary><c>i <see langword="is false"/></c></summary>
	IsFalse,
	/// <summary><c>++i</c></summary>
	PreIncrement,
	/// <summary><c>i + 1</c></summary>
	Increment,
	/// <summary><c>i++</c></summary>
	PostIncrement,
	/// <summary><c>--i</c></summary>
	PreDecrement,
	/// <summary><c>i - 1</c></summary>
	Decrement,
	/// <summary><c>i--</c></summary>
	PostDecrement,
	/// <summary><c>-i</c></summary>
	Negate,
	/// <summary><c><see langword="checked"/>(-i)</c></summary>
	NegateChecked,
	/// <summary><c>~i</c></summary>
	Complement
}
