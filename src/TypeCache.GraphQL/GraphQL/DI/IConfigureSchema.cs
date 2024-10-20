using GraphQL.Types;

namespace GraphQL.DI;

/// <summary>
/// Allows configuration of a schema prior to the code in its constructor.
/// <br/><br/>
/// Typically executes during the <see cref="Schema"/> constructor,
/// which executes prior to any descendant classes' constructors.
/// </summary>
public interface IConfigureSchema
{
	/// <summary>
	/// Configures a schema prior to the code in its constructor.
	/// <br/><br/>
	/// Specifically, typically executes during the <see cref="Schema"/> constructor,
	/// which executes prior to any descendant classes' constructors.
	/// </summary>
	void Configure(ISchema schema, IServiceProvider serviceProvider);
}
