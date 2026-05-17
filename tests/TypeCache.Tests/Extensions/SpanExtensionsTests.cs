// Copyright (c) 2021 Samuel Abraham

using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class SpanExtensionsTests
{
	[Fact]
	public void AsReadOnly()
	{
		Span<int> span = new[] { 1, 2, 3 };
		ReadOnlySpan<int> readOnly = span.AsReadOnly();

		Assert.Equal(3, readOnly.Length);
		Assert.Equal(1, readOnly[0]);
	}

	[Fact]
	public void AsReadOnly_Empty()
	{
		Span<int> span = [];
		ReadOnlySpan<int> readOnly = span.AsReadOnly();

		Assert.Equal(0, readOnly.Length);
	}

	[Fact]
	public void AsBytes()
	{
		Span<int> span = new[] { 1, 2 };
		Span<byte> bytes = span.AsBytes();

		Assert.Equal(span.Length * sizeof(int), bytes.Length);
	}
}
