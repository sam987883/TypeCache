// Copyright (c) 2020 Samuel Abraham

namespace sam987883.Database
{
	public class ColumnSchema
	{
		public string Name { get; set; }
		public bool Hidden { get; set; }
		public bool Identity { get; set; }
		public bool Nullable { get; set; }
		public bool PrimaryKey { get; set; }
		public bool Readonly { get; set; }
		public string Type { get; set; }
		public int Length { get; set; }
	}
}
