// Copyright (c) 2021 Samuel Abraham

using TypeCache.Data.Domain;

namespace TypeCache.Data.Extensions
{
	public static class RequestExtensions
	{
		public static CountCommand ToCountCommand(this DeleteCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				InputParameters = @this.InputParameters,
				Table = @this.Table,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this InsertCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				InputParameters = @this.InputParameters,
				Table = @this.From,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this SelectCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				InputParameters = @this.InputParameters,
				Table = @this.From,
				Where = @this.Where
			};

		public static CountCommand ToCountCommand(this UpdateCommand @this)
			=> new()
			{
				DataSource = @this.DataSource,
				InputParameters = @this.InputParameters,
				Table = @this.Table,
				Where = @this.Where
			};
	}
}
