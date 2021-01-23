﻿// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TypeCache.Data;

namespace TypeCache.Extensions
{
	public static class DbDataReaderExtensions
	{
		public static string[] GetColumns(this IDataReader @this)
			=> 0.Range(@this.FieldCount).To(@this.GetName).ToArray(@this.FieldCount);

		public static async IAsyncEnumerable<object[]> ReadRowsAsync(this DbDataReader @this, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			var columnCount = @this.FieldCount;
			while (await @this.ReadAsync(cancellationToken))
			{
				var values = new object[columnCount];
				@this.GetValues(values);
				yield return values;
			}
		}

		public static async ValueTask<RowSet> ReadRowSetAsync(this DbDataReader @this, CancellationToken cancellationToken = default)
			=> new RowSet
			{
				Columns = @this.GetColumns(),
				Rows = (await @this.ReadRowsAsync(cancellationToken).ToListAsync()).ToArray()
			};

		public static async IAsyncEnumerable<RowSet> ReadRowSetsAsync(this DbDataReader @this, [EnumeratorCancellation] CancellationToken cancellationToken = default)
		{
			do
			{
				yield return await @this.ReadRowSetAsync();
			} while (await @this.NextResultAsync(cancellationToken));
		}
	}
}
