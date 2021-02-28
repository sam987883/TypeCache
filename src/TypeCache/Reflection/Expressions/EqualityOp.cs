// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Reflection.Expressions
{
	public enum EqualityOp
	{
		/// <summary><code>a == b</code></summary>
		EqualTo,
		/// <summary><code>a === b</code></summary>
		ReferenceEqualTo,
		/// <summary><code>a != b</code></summary>
		NotEqualTo,
		/// <summary><code>a !== b</code></summary>
		ReferenceNotEqualTo,
		/// <summary><code>a &gt; b</code></summary>
		MoreThan,
		/// <summary><code>a &gt;= b</code></summary>
		MoreThanOrEqualTo,
		/// <summary><code>a &lt; b</code></summary>
		LessThan,
		/// <summary><code>a &lt;= b</code></summary>
		LessThanOrEqualTo,
	}
}
