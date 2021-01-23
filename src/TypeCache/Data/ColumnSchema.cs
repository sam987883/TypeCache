﻿// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Data
{
	public class ColumnSchema
	{
		public int Id { get; set; }

		public string Name { get; set; } = string.Empty;

		public string Type { get; set; } = string.Empty;

		public bool Hidden { get; set; }

		public bool Identity { get; set; }

		public bool Nullable { get; set; }

		public bool PrimaryKey { get; set; }

		public bool Readonly { get; set; }

		public int Length { get; set; }

		public RuntimeTypeHandle TypeHandle { get; set; }
	}
}
