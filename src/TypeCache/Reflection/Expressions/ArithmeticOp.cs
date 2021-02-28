// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Expressions
{
	public enum ArithmeticOp
	{
		/// <summary><code>a + b</code></summary>
		Add,
		/// <summary><code>checked(a + b)</code></summary>
		AddChecked,
		/// <summary><code>a / b</code></summary>
		Divide,
		/// <summary><code>a % 4</code></summary>
		Modulus,
		/// <summary><code>a * b</code></summary>
		Multiply,
		/// <summary><code>checked(a * b)</code></summary>
		MultiplyChecked,
		/// <summary><code>a ^ b</code></summary>
		Power,
		/// <summary><code>a - b</code></summary>
		Subtract,
		/// <summary><code>checked(a - b)</code></summary>
		SubtractChecked
	}
}
