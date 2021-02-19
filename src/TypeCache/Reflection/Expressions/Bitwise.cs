// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Expressions
{
	public enum Bitwise
	{
		/// <summary><code>a &amp; b</code></summary>
		And,
		/// <summary><code>a | b</code></summary>
		Or,
		/// <summary><code>a ^ b</code></summary>
		ExclusiveOr,
		/// <summary><code>a &lt;&lt; 4</code></summary>
		LeftShift,
		/// <summary><code>a &gt;&gt; 4</code></summary>
		RightShift
	}
}
