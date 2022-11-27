// Copyright (c) 2021 Samuel Abraham

using System;
using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests;

public class DelegateOfTests
{
	[Fact]
	public void DelegateOfAction()
	{
		Assert.Empty(DelegateOf<Action<string, int, DateTime?>>.Attributes);
		Assert.Equal(typeof(Action<string, int, DateTime?>).TypeHandle, DelegateOf<Action<string, int, DateTime?>>.Handle);
		Assert.False(DelegateOf<Action<string, int, DateTime?>>.Internal);
		Assert.NotNull(DelegateOf<Action<string, int, DateTime?>>.Method);
		Assert.Contains(DelegateOf<Action<string, int, DateTime?>>.Name, typeof(Action<string, int, DateTime?>).Name);
		Assert.Equal(3, DelegateOf<Action<string, int, DateTime?>>.Parameters.Count);
		Assert.True(DelegateOf<Action<string, int, DateTime?>>.Public);
		Assert.Equal(typeof(void).GetTypeMember(), DelegateOf<Action<string, int, DateTime?>>.Return.Type);
	}

	[Fact]
	public void DelegateOfFunc()
	{
		Assert.Empty(DelegateOf<Func<string, int, DateTime?, bool>>.Attributes);
		Assert.Equal(typeof(Func<string, int, DateTime?, bool>).TypeHandle, DelegateOf<Func<string, int, DateTime?, bool>>.Handle);
		Assert.False(DelegateOf<Func<string, int, DateTime?, bool>>.Internal);
		Assert.NotNull(DelegateOf<Func<string, int, DateTime?, bool>>.Method);
		Assert.Contains(DelegateOf<Func<string, int, DateTime?, bool>>.Name, typeof(Func<string, int, DateTime?, bool>).Name);
		Assert.Equal(3, DelegateOf<Func<string, int, DateTime?, bool>>.Parameters.Count);
		Assert.True(DelegateOf<Func<string, int, DateTime?, bool>>.Public);
		Assert.Equal(TypeOf<bool>.Member, DelegateOf<Func<string, int, DateTime?, bool>>.Return.Type);
	}

	[Fact]
	public void DelegateOfPredicate()
	{
		Assert.Empty(DelegateOf<Predicate<string>>.Attributes);
		Assert.Equal(typeof(Predicate<string>).TypeHandle, DelegateOf<Predicate<string>>.Handle);
		Assert.False(DelegateOf<Predicate<string>>.Internal);
		Assert.NotNull(DelegateOf<Predicate<string>>.Method);
		Assert.Contains(DelegateOf<Predicate<string>>.Name, typeof(Predicate<string>).Name);
		Assert.Equal(1, DelegateOf<Predicate<string>>.Parameters.Count);
		Assert.True(DelegateOf<Predicate<string>>.Public);
		Assert.Equal(TypeOf<bool>.Member, DelegateOf<Predicate<string>>.Return.Type);
	}
}
