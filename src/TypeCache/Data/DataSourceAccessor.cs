// Copyright (c) 2021 Samuel Abraham

using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TypeCache.Collections.Extensions;
using TypeCache.Extensions;
using static TypeCache.Default;

namespace TypeCache.Data;

internal class DataSourceAccessor : IAccessor<DataSource>
{
	private readonly IReadOnlyDictionary<string, DataSource> _DataSources;

	public DataSourceAccessor(DataSource[] dataSources)
	{
		this._DataSources = dataSources.ToDictionary(dataSource => dataSource.Name, dataSource => dataSource)
			.ToImmutableDictionary();
	}

	public DataSource this[string dataSource] => this._DataSources[dataSource];

	[MethodImpl(METHOD_IMPL_OPTIONS), DebuggerHidden]
	public bool Has(string name)
		=> this._DataSources.ContainsKey(name);
}
