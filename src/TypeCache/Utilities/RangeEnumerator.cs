// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;

namespace TypeCache.Utilities;

public struct RangeEnumerator
{
	private int _Current;
	private readonly int _End;

	public RangeEnumerator(Range range)
	{
		this._Current = range.IsReverse() ? range.Start.Value + 1 : range.Start.Value - 1;
		this._End = range.End.Value;
	}

	public int Current => this._Current;

	public bool MoveNext()
		=> this._Current switch
		{
			_ when this._Current < this._End => ++this._Current < this._End,
			_ when this._Current > this._End => --this._Current > this._End,
			_ => false
		};
}
