// Copyright (c) 2021 Samuel Abraham

using System;
using System.Linq.Expressions;

namespace TypeCache.Business
{
	public class Validator
	{
		public bool Validate<T>(T value, Expression<Func<T, bool>> test, out string? message)
		{
			if (test.Compile()(value))
			{
				message = null;
				return true;
			}
			else
			{
				message = $"Fail: [{value}] in: {test}";
				return false;
			}
		}
	}
}
