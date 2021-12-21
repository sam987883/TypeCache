// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Extensions;

public enum UnaryOperator
{
	/// <summary><code>i is true</code></summary>
	IsTrue,
	/// <summary><code>i is false</code></summary>
	IsFalse,
	/// <summary><code>++i</code></summary>
	PreIncrement,
	/// <summary><code>i + 1</code></summary>
	Increment,
	/// <summary><code>i++</code></summary>
	PostIncrement,
	/// <summary><code>--i</code></summary>
	PreDecrement,
	/// <summary><code>i - 1</code></summary>
	Decrement,
	/// <summary><code>i--</code></summary>
	PostDecrement,
	/// <summary><code>-i</code></summary>
	Negate,
	/// <summary><code>checked(-i)</code></summary>
	NegateChecked,
	/// <summary><code>~i</code></summary>
	Complement
}
