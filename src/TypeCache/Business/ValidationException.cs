using System;
using TypeCache.Collections.Extensions;

namespace TypeCache.Business;

public class ValidationException : Exception
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
