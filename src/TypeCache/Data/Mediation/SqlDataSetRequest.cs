﻿// Copyright (c) 2021 Samuel Abraham

using System.Data;
using TypeCache.Mediation;

namespace TypeCache.Data.Mediation;

public sealed class SqlDataSetRequest : IRequest<DataSet>
{
	public required SqlCommand Command { get; set; }
}
