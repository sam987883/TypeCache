// Copyright(c) 2020 Samuel Abraham

namespace TypeCache.Data
{
	public interface IDataRequest
	{
		/// <summary>
		/// The data source name that contains the connection string and database provider to use.
		/// </summary>
		public string DataSource { get; set; }
	}
}
