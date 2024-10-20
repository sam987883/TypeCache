using System.Collections.Generic;

namespace GraphQL.Validation;

/// <inheritdoc cref="IValidationResult"/>
public readonly struct ValidationResult : IValidationResult
{
	/// <summary>
	/// Initializes a new instance with the specified set of validation errors.
	/// </summary>
	/// <param name="errors">Set of validation errors.</param>
	public ValidationResult(IEnumerable<ValidationError> errors)
	{
		foreach (var error in errors)
			this.Errors.Add(error);
	}

	/// <inheritdoc/>
	public bool IsValid => this.Errors.Count == 0;

	/// <inheritdoc/>
	public IList<ExecutionError> Errors { get; } = new List<ExecutionError>();
}

/// <summary>
/// A validation result that indicates no errors were found during validation of the document.
/// </summary>
public readonly struct SuccessfullyValidatedResult : IValidationResult
{
	public SuccessfullyValidatedResult()
	{
	}

	/// <summary>
	/// Returns <see langword="true"/> indicating that the document was successfully validated.
	/// </summary>
	public bool IsValid => true;

	/// <summary>
	/// Returns an empty list of execution errors.
	/// </summary>
	public IList<ExecutionError> Errors { get; } = new List<ExecutionError>();
}
