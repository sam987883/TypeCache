// Copyright (c) 2020 Samuel Abraham


namespace sam987883.Database.Commands
{
	public class Output
	{
		public RowSet Deleted { get; set; } = new RowSet();

		public RowSet Inserted { get; set; } = new RowSet();
	}

	public class Output<T> where T : class, new()
	{
		public RowSet<T> Deleted { get; set; } = new RowSet<T>();

		public RowSet<T> Inserted { get; set; } = new RowSet<T>();
	}
}
