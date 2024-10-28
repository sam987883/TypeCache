// Copyright (c) 2021 Samuel Abraham

using GraphQL.Types;
using TypeCache.Attributes;
using TypeCache.GraphQL.Attributes;
using TypeCache.GraphQL.Types;

namespace TypeCache.GraphQL.TestApp.Tables;

[SqlApi]
public class Person
{
	[GraphQLType<HashIdGraphType>()]
	public int BusinessEntityID { get; set; }
	public string? PersonType { get; set; }
	public bool NameStyle { get; set; }
	public string? Title { get; set; }
	public string? FirstName { get; set; }
	public string? MiddleName { get; set; }
	public string? LastName { get; set; }
	public string? Suffix { get; set; }
	public int EmailPromotion { get; set; }
	public string? AdditionalContactInfo { get; set; }
	public string? Demographics { get; set; }
	[GraphQLType<StringGraphType>()]
	[GraphQLName("rowguid")]
	public Guid Rowguid { get; set; }
	[GraphQLType<NonNullGraphType<StringGraphType>>()]
	public DateTime ModifiedDate { get; set; }

	public IEnumerable<Person> GetPersons()
		=> new Person[]
		{
			new() { FirstName = "John", LastName = "Smith" }
		};
}
