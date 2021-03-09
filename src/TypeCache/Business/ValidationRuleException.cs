// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Business
{
	public class ValidationRuleException : Exception
	{
		public ValidationRuleException(Exception exception) : base(exception.Message, exception)
		{
		}
	}
}
