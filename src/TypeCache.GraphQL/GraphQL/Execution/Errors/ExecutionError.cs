using System.Collections;
using GraphQL.Execution;
using GraphQLParser;
using GraphQLParser.AST;

namespace GraphQL;

/// <summary>
/// Represents an error generated while processing a document and
/// intended to be returned within an <see cref="ExecutionResult"/>.
/// </summary>
[Serializable]
public class ExecutionError : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionError"/> class with a specified error message.
	/// </summary>
	public ExecutionError(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionError"/>
	/// class with a specified error message and exception data.
	/// </summary>
	public ExecutionError(string message, IDictionary data)
		: base(message)
	{
		if (data is not null)
		{
			foreach (DictionaryEntry pair in data)
			{
				this.Data[pair.Key] = pair.Value;
			}
		}
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ExecutionError"/> class with a specified
	/// error message and inner exception. Sets the <see cref="Code"/> property based on the
	/// inner exception. Loads any exception data from the inner exception into this instance.
	/// </summary>
	public ExecutionError(string message, Exception? innerException)
		: base(message, innerException)
	{
		if (innerException is not null)
			Code = ErrorInfoProvider.GetErrorCode(innerException);

		if (innerException?.Data is not null)
		{
			foreach (DictionaryEntry pair in innerException.Data)
			{
				this.Data[pair.Key] = pair.Value;
			}
		}
	}

	/// <summary>
	/// Returns a list of locations (if any) within the document that this error applies to.
	/// </summary>
	public List<Location> Locations { get; } = new();

	/// <summary>
	/// Gets or sets a code for this error. Code is typically used to write the 'code'
	/// property to the execution result 'extensions' property.
	/// </summary>
	public string? Code { get; set; }

	/// <summary>
	/// Gets or sets the path within the GraphQL document where this error applies to.
	/// </summary>
	public IEnumerable<object>? Path { get; set; }

	/// <summary>
	/// Gets or sets additional information about error.
	/// </summary>
	public Dictionary<string, object?> Extensions { get; set; } = new();
}

/// <summary>
/// Provides extension methods for <see cref="ExecutionError"/> instances.
/// </summary>
public static class ExecutionErrorExtensions
{
	/// <summary>
	/// Adds a location to an <see cref="ExecutionError"/> based on a <see cref="ASTNode"/> within a <see cref="GraphQLDocument"/>.
	/// </summary>
	public static TError AddLocation<TError>(this TError error, ASTNode? abstractNode, GraphQLDocument? document)
		where TError : ExecutionError
	{
		if (abstractNode is null || abstractNode.Location == default || document is null || document.Source.IsEmpty)
			return error;

		error.Locations.Add(Location.FromLinearPosition(document.Source, abstractNode.Location.Start));
		return error;
	}
}
