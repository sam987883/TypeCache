using TypeCache.Extensions;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class GlobalExtensions
{
	[Fact]
	public void Between()
	{
		Assert.False((-1).Between(1, 3));
		Assert.True(1.Between(1, 3));
		Assert.True(2U.Between(1U, 3U));
		Assert.True(3L.Between(1L, 3L));
		Assert.False(4UL.Between(1UL, 3UL));
	}

	[Fact]
	public void InBetween()
	{
		Assert.False((-1).InBetween(1, 3));
		Assert.False(1.InBetween(1, 3));
		Assert.True(2U.InBetween(1U, 3U));
		Assert.False(3L.InBetween(1L, 3L));
		Assert.False(4UL.InBetween(1UL, 3UL));
	}

	[Fact]
	public void Swap()
	{
		decimal a = 1M;
		decimal b = 2M;

		a.Swap(ref b);

		Assert.Equal(1M, b);
		Assert.Equal(2M, a);

		var swap = (a, b).Swap();

		Assert.Equal(1M, swap.Item1);
		Assert.Equal(2M, swap.Item2);
	}
}
