// Copyright (c) 2021 Samuel Abraham

using System;

namespace TypeCache.Data
{
	public readonly struct ColumnSchema
	{
		public int Id { get; init; }

		public string Name { get; init; }

		public string Type { get; init; }

		public bool Hidden { get; init; }

		public bool Identity { get; init; }

		public bool Nullable { get; init; }

		public bool PrimaryKey { get; init; }

		public bool Readonly { get; init; }

		public int Length { get; init; }

		public RuntimeTypeHandle TypeHandle => (this.Type switch
		{
			"BIGINT" when this.Nullable => typeof(long?),
			"BIGINT" => typeof(long),
			"BINARY" when this.Length > 1 => typeof(byte[]),
			"BINARY" when this.Nullable => typeof(byte?),
			"BINARY" => typeof(byte),
			"BIT" when this.Nullable => typeof(bool?),
			"BIT" => typeof(bool),
			"CHAR" when this.Length > 1 => typeof(string),
			"CHAR" when this.Nullable => typeof(char?),
			"CHAR" => typeof(char),
			"DATE" when this.Nullable => typeof(DateTime?),
			"DATE" => typeof(DateTime),
			"DATETIME" when this.Nullable => typeof(DateTime?),
			"DATETIME" => typeof(DateTime),
			"DATETIME2" when this.Nullable => typeof(DateTime?),
			"DATETIME2" => typeof(DateTime),
			"DATETIMEOFFSET" when this.Nullable => typeof(DateTimeOffset?),
			"DATETIMEOFFSET" => typeof(DateTimeOffset),
			"DECIMAL" when this.Nullable => typeof(decimal?),
			"DECIMAL" => typeof(decimal),
			"FLOAT" when this.Nullable => typeof(double?),
			"FLOAT" => typeof(double),
			"IMAGE" => typeof(byte[]),
			"INT" when this.Nullable => typeof(int?),
			"INT" => typeof(int),
			"MONEY" when this.Nullable => typeof(decimal?),
			"MONEY" => typeof(decimal),
			"NCHAR" when this.Length > 1 => typeof(string),
			"NCHAR" when this.Nullable => typeof(char?),
			"NCHAR" => typeof(char),
			"NTEXT" => typeof(string),
			"NUMERIC" when this.Nullable => typeof(decimal?),
			"NUMERIC" => typeof(decimal),
			"NVARCHAR" => typeof(string),
			"REAL" when this.Nullable => typeof(float?),
			"REAL" => typeof(float),
			"ROWVERSION" => typeof(byte[]),
			"SMALLDATETIME" when this.Nullable => typeof(DateTime?),
			"SMALLDATETIME" => typeof(DateTime),
			"SMALLINT" when this.Nullable => typeof(short?),
			"SMALLINT" => typeof(short),
			"SMALLMONEY" when this.Nullable => typeof(decimal?),
			"SMALLMONEY" => typeof(decimal),
			"SYSNAME" => typeof(string),
			"TEXT" => typeof(string),
			"TIME" when this.Nullable => typeof(TimeSpan?),
			"TIME" => typeof(TimeSpan),
			"TIMESTAMP" => typeof(byte[]),
			"TINYINT" => typeof(sbyte),
			"UNIQUEIDENTIFIER" when this.Nullable => typeof(Guid?),
			"UNIQUEIDENTIFIER" => typeof(Guid),
			"VARBINARY" => typeof(byte[]),
			"VARCHAR" => typeof(string),
			_ => typeof(object)
		}).TypeHandle;
	}
}
