namespace Sam987883.Reflection.Mappers
{
	public class MapperSetting
	{
		public MapperSetting(string to, string from, bool ignoreNullValue)
		{
			this.From = from;
			this.IgnoreNullValue = ignoreNullValue;
			this.To = to;
		}

		public string From { get; set; }

		public bool IgnoreNullValue { get; set; }

		public string To { get; set; }
	}
}
