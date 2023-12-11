// Copyright (c) 2021 Samuel Abraham

using System.IO;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Utilities;

public class EventOfTests
{
	[Fact]
	public void EventOfFileSystemWatcher()
	{
		Assert.Equal(6, EventHandler<FileSystemWatcher>.Events.Length);
	}
}
