// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.GraphQL.Attributes;
using TypeCache.Reflection;

namespace TypeCache.GraphQL.TestApp.Tables;

public class Person
{
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
	[GraphQLType(ScalarType.Guid)]
	[Name("rowguid")]
	public Guid Rowguid { get; set; }
	[GraphQLType(ScalarType.NotNullString)]
	public DateTime ModifiedDate { get; set; }
}
