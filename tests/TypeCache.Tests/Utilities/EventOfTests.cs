// Copyright (c) 2021 Samuel Abraham

using System.IO;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class EventOfTests
{
	[Fact]
	public void EventOfFileSystemWatcher()
	{
		Assert.Equal(6, TypeCache.Utilities.EventHandler<FileSystemWatcher>.Events.Length);
	}
}
