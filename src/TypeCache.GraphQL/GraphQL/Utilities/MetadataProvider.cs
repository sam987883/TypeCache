using System.Collections.Generic;
using GraphQL.Types;

namespace GraphQL.Utilities;

/// <summary>
/// Default implementation of <see cref="IProvideMetadata"/>. This is the base class for numerous
/// descendants like <see cref="GraphType"/>, <see cref="FieldType"/>, <see cref="Schema"/> and others.
/// </summary>
public class MetadataProvider : IProvideMetadata
{
	private Dictionary<string, object?>? _metadata;

	/// <inheritdoc />
	public Dictionary<string, object?> Metadata
		=> _metadata ??= new();

	/// <inheritdoc />
	public TType GetMetadata<TType>(string key, TType defaultValue = default!)
		=> this._metadata?.TryGetValue(key, out object? item) is true ? (TType)item! : defaultValue;

	/// <inheritdoc />
	public TType GetMetadata<TType>(string key, Func<TType> defaultValueFactory)
		=> this._metadata?.TryGetValue(key, out object? item) is true ? (TType)item! : defaultValueFactory();

	/// <inheritdoc />
	public bool HasMetadata(string key)
		=> this._metadata?.ContainsKey(key) is true;

	/// <summary>
	/// Copies metadata to the specified target.
	/// </summary>
	/// <param name="target">Target for copying metadata.</param>
	public void CopyMetadataTo(IProvideMetadata target)
	{
		if (this._metadata?.Count > 0)
		{
			foreach (var pair in this._metadata)
				target.Metadata[pair.Key] = pair.Value;
		}
	}
}
