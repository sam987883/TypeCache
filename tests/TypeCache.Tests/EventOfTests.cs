// Copyright (c) 2021 Samuel Abraham

using System.IO;
using Xunit;

namespace TypeCache.Tests
{
	public class EventOfTests
	{
		[Fact]
		public void EventOfFileSystemWatcher()
		{
			Assert.Equal(6, EventOf<FileSystemWatcher>.Events.Count);
		}
	}
}
