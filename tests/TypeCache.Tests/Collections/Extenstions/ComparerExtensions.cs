// Copyright (c) 2021 Samuel Abraham

using System.Collections;
using System.Collections.Generic;
using TypeCache.Collections.Extensions;
using Xunit;

namespace TypeCache.Tests.Collections.Extensions
{
	public class ComparerExtensions
	{
		[Fact]
		public void EqualTo()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.False(comparer.EqualTo(2, 1));
			Assert.True(comparer.EqualTo(2, 2));
			Assert.False(comparer.EqualTo(2, 3));
			Assert.False(comparerInt.EqualTo(2, 1));
			Assert.True(comparerInt.EqualTo(2, 2));
			Assert.False(comparerInt.EqualTo(2, 3));
		}

		[Fact]
		public void LessThan()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.False(comparer.LessThan(2, 1));
			Assert.False(comparer.LessThan(2, 2));
			Assert.True(comparer.LessThan(2, 3));
			Assert.False(comparerInt.LessThan(2, 1));
			Assert.False(comparerInt.LessThan(2, 2));
			Assert.True(comparerInt.LessThan(2, 3));
		}

		[Fact]
		public void LessThanOrEqualTo()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.False(comparer.LessThanOrEqualTo(2, 1));
			Assert.True(comparer.LessThanOrEqualTo(2, 2));
			Assert.True(comparer.LessThanOrEqualTo(2, 3));
			Assert.False(comparerInt.LessThanOrEqualTo(2, 1));
			Assert.True(comparerInt.LessThanOrEqualTo(2, 2));
			Assert.True(comparerInt.LessThanOrEqualTo(2, 3));
		}

		[Fact]
		public void MoreThan()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.True(comparer.MoreThan(2, 1));
			Assert.False(comparer.MoreThan(2, 2));
			Assert.False(comparer.MoreThan(2, 3));
			Assert.True(comparerInt.MoreThan(2, 1));
			Assert.False(comparerInt.MoreThan(2, 2));
			Assert.False(comparerInt.MoreThan(2, 3));
		}

		[Fact]
		public void MoreThanOrEqualTo()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.True(comparer.MoreThanOrEqualTo(2, 1));
			Assert.True(comparer.MoreThanOrEqualTo(2, 2));
			Assert.False(comparer.MoreThanOrEqualTo(2, 3));
			Assert.True(comparerInt.MoreThanOrEqualTo(2, 1));
			Assert.True(comparerInt.MoreThanOrEqualTo(2, 2));
			Assert.False(comparerInt.MoreThanOrEqualTo(2, 3));
		}

		[Fact]
		public void Maximum()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.Equal(3, comparer.Maximum(1, 3));
			Assert.Equal(3, comparer.Maximum(3, 1));
			Assert.Equal(3, comparerInt.Maximum(1, 3));
			Assert.Equal(3, comparerInt.Maximum(3, 1));
		}

		[Fact]
		public void Minimum()
		{
			var comparer = (IComparer)Comparer<int>.Default;
			var comparerInt = (IComparer<int>)Comparer<int>.Default;

			Assert.Equal(1, comparer.Minimum(1, 3));
			Assert.Equal(1, comparer.Minimum(3, 1));
			Assert.Equal(1, comparerInt.Minimum(1, 3));
			Assert.Equal(1, comparerInt.Minimum(3, 1));
		}
	}
}
