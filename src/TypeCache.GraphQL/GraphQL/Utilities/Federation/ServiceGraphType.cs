using GraphQL.Types;
using TypeCache.GraphQL.Types;

namespace GraphQL.Utilities.Federation;

public class ServiceGraphType : ObjectGraphType
{
	public ServiceGraphType()
	{
		this.Name = "_Service";
		this.Field<GraphQLStringType>("sdl").Resolve(context =>
		{
			var printer = new FederatedSchemaPrinter(context.Schema);
			return printer.PrintFederatedSchema();
		});
	}
}
