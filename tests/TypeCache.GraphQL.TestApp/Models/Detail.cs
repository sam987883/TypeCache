// Copyright (c) 2021 Samuel Abraham

using TypeCache.GraphQL.Attributes;

namespace TypeCache.GraphQL.TestApp.Models;

public class Detail
{
	public string? Alias { get; set; }

	public string? FirstName { get; set; }

	public int SomeValue { get; set; }

	[GraphQLName("detail")]
	public static ValueTask<Detail> GetDetail()
		=> ValueTask.FromResult(new Detail() { Alias = "A1", FirstName = "John", SomeValue = 111 });

	[GraphQLName("details")]
	public static ValueTask<IEnumerable<Detail>> GetDetails(IEnumerable<string> firstNames)
		=> ValueTask.FromResult((IEnumerable<Detail>)new Detail[]
		{
			new () { Alias = "A1", FirstName = "John", SomeValue = 111 },
			new () { Alias = "A2", FirstName = "Samuel", SomeValue = 222 },
			new () { Alias = "A3", FirstName = "Samuel", SomeValue = 333 },
		});
}
