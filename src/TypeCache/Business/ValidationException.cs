// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;

namespace TypeCache.Business;

public sealed class ValidationException : Exception
{
	public string[] ValidationMessages { get; }

	public ValidationException(params string[] validationMessages) : base(validationMessages.ToCSV())
	{
		this.ValidationMessages = validationMessages;
	}

	public ValidationException(Validator validator) : base(validator.Fails.ToCSV())
	{
		this.ValidationMessages = validator.Fails.ToArray();
	}
}
