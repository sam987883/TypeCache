// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Business
{
	public class ValidationRuleException : Exception
	{
		public ValidationRuleException(params string[] messages)
			=> this.Messages = messages;

		public string[] Messages { get; }
	}
}
