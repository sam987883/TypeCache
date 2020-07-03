// Copyright (c) 2020 Samuel Abraham

using sam987883.Common;
using sam987883.Database.Commands;
using sam987883.Extensions;
using sam987883.Reflection;
using System;

namespace sam987883.Database
{
	public class SchemaValidator
	{
		private readonly TableSchema _TableSchema;

		public SchemaValidator(TableSchema tableSchema) =>
			this._TableSchema = tableSchema;

		public void Validate(BatchDelete batch)
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.Output.Columns);
		}

		public void Validate<T>(BatchDelete<T> batch) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateColumnsReadable($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.PropertyCache, batch.Input.Columns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.Output.Columns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.PropertyCache, batch.Output.Columns);
		}

		public void Validate(BatchInsert batch)
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.Input.Columns, batch.InsertColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.Output.Columns);
		}

		public void Validate<T>(BatchInsert<T> batch) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.Input.Columns, batch.InsertColumns);
			this.ValidateColumnsReadable($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.PropertyCache, batch.InsertColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.Output.Columns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Columns)}", batch.PropertyCache, batch.Output.Columns);
		}

		public void Validate(BatchUpdate batch)
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.Input.Columns, batch.UpdateColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.Output.Inserted.Columns);
		}

		public void Validate<T>(BatchUpdate<T> batch) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.Input.Columns, batch.UpdateColumns);
			this.ValidateColumnsReadable($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.PropertyCache, batch.UpdateColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.Output.Deleted.Columns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.PropertyCache, batch.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.Output.Inserted.Columns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.PropertyCache, batch.Output.Inserted.Columns);
		}

		public void Validate(BatchUpsert batch)
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.Input.Columns, batch.InsertColumns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.Input.Columns, batch.UpdateColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.Output.Inserted.Columns);
		}

		public void Validate<T>(BatchUpsert<T> batch) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(batch)}.{nameof(batch.Table)}");
			this.ValidateInputColumnsHavePrimaryKeys($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Columns)}", batch.Input.Columns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.Input.Columns, batch.InsertColumns);
			this.ValidateColumnsReadable($"{nameof(batch)}.{nameof(batch.InsertColumns)}", batch.PropertyCache, batch.InsertColumns);
			this.ValidateInputDataColumns($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.Input.Columns, batch.UpdateColumns);
			this.ValidateColumnsReadable($"{nameof(batch)}.{nameof(batch.UpdateColumns)}", batch.PropertyCache, batch.UpdateColumns);
			this.ValidateInputRows($"{nameof(batch)}.{nameof(batch.Input)}.{nameof(batch.Input.Rows)}", batch.Input);
			this.ValidateOnColumns($"{nameof(batch)}.{nameof(batch.OnColumns)}", batch.OnColumns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.PropertyCache, batch.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Deleted)}.{nameof(batch.Output.Deleted.Columns)}", batch.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.Output.Inserted.Columns);
			this.ValidateColumnsWritable($"{nameof(batch)}.{nameof(batch.Output)}.{nameof(batch.Output.Inserted)}.{nameof(batch.Output.Inserted.Columns)}", batch.PropertyCache, batch.Output.Inserted.Columns);
		}

		public void Validate(Delete delete)
		{
			this.ValidateIsTable($"{nameof(delete)}.{nameof(delete.Table)}");
			this.ValidateExpression($"{nameof(delete)}.{nameof(delete.Where)}", delete.Where);
			this.ValidateOutputColumns($"{nameof(delete)}.{nameof(delete.Output)}.{nameof(delete.Output.Columns)}", delete.Output.Columns);
		}

		public void Validate<T>(Delete<T> delete) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(delete)}.{nameof(delete.Table)}");
			this.ValidateExpression($"{nameof(delete)}.{nameof(delete.Where)}", delete.Where);
			this.ValidateOutputColumns($"{nameof(delete)}.{nameof(delete.Output)}.{nameof(delete.Output.Columns)}", delete.Output.Columns);
			this.ValidateColumnsWritable($"{nameof(delete)}.{nameof(delete.Output)}.{nameof(delete.Output.Columns)}", delete.PropertyCache, delete.Output.Columns);
		}

		public void Validate(Update update)
		{
			this.ValidateIsTable($"{nameof(update)}.{nameof(update.Table)}");
			this.ValidateColumnsWritable($"{nameof(update)}.{nameof(update.Set)}", update.Set.To(set => set.Column).ToArray());
			this.ValidateExpression($"{nameof(update)}.{nameof(update.Where)}", update.Where);
			this.ValidateOutputColumns($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Deleted)}.{nameof(update.Output.Deleted.Columns)}", update.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Inserted)}.{nameof(update.Output.Inserted.Columns)}", update.Output.Inserted.Columns);
		}

		public void Validate<T>(Update<T> update) where T : class, new()
		{
			this.ValidateIsTable($"{nameof(update)}.{nameof(update.Table)}");
			this.ValidateColumnsWritable($"{nameof(update)}.{nameof(update.Set)}", update.PropertyCache, update.Set.To(set => set.Column).ToArray());
			this.ValidateExpression($"{nameof(update)}.{nameof(update.Where)}", update.Where);
			this.ValidateOutputColumns($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Deleted)}.{nameof(update.Output.Deleted.Columns)}", update.Output.Deleted.Columns);
			this.ValidateColumnsWritable($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Deleted)}.{nameof(update.Output.Deleted.Columns)}", update.PropertyCache, update.Output.Deleted.Columns);
			this.ValidateOutputColumns($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Inserted)}.{nameof(update.Output.Inserted.Columns)}", update.Output.Inserted.Columns);
			this.ValidateColumnsWritable($"{nameof(update)}.{nameof(update.Output)}.{nameof(update.Output.Inserted)}.{nameof(update.Output.Inserted.Columns)}", update.PropertyCache, update.Output.Inserted.Columns);
		}

		public void Validate(Select select)
		{
			this.ValidateColumnsExist($"{nameof(select)}.{nameof(select.Output.Columns)}", select.Output.Columns);
			this.ValidateExpression($"{nameof(select)}.{select.Where}", select.Where);
			this.ValidateExpression($"{nameof(select)}.{select.Having}", select.Having);
			this.ValidateColumnsExist($"{nameof(select)}.{nameof(select.OrderBy)}", select.OrderBy.To(orderBy => orderBy.Column).ToArray());
		}

		private void ValidateExpression(string parameter, ExpressionSet? expressionSet)
		{
			if (expressionSet == null)
				return;

			if (expressionSet.Expressions.Any())
			{
				var fields = expressionSet.Expressions.To(expression => expression.Field);
				var columns = this._TableSchema.Columns.To(column => column.Name);
				var invalidFieldCsv = fields.Without(columns).ToCsv();
				if (!invalidFieldCsv.IsBlank())
					throw new ArgumentException($"{this._TableSchema.Name} does not have the following columns: {invalidFieldCsv}.",
						parameter);

				var stringFields = expressionSet.Expressions
					.If(expression => expression.Operator == ComparisonOperator.Like
						|| expression.Operator == ComparisonOperator.NotLike
						|| expression.Operator == ComparisonOperator.StartWith
						|| expression.Operator == ComparisonOperator.NotStartWith
						|| expression.Operator == ComparisonOperator.EndWith
						|| expression.Operator == ComparisonOperator.NotEndWith)
					.To(expression => expression.Field);
				var stringColumns = this._TableSchema.Columns.If(column => column.Type.Contains("char", StringComparison.OrdinalIgnoreCase)).To(column => column.Name);
				invalidFieldCsv = stringFields.Without(stringColumns).ToCsv();
				if (!invalidFieldCsv.IsBlank())
					throw new ArgumentException($"The following columns in {this._TableSchema.Name} cannot use string operators: {invalidFieldCsv}.",
						parameter);
			}

			expressionSet.ExpressionSets.Do(item => this.ValidateExpression(parameter, item));
		}

		private void ValidateInputColumnsHavePrimaryKeys(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var primaryKeys = this._TableSchema.Columns.If(column => column.PrimaryKey).To(column => column.Name).ToArray();
				if (primaryKeys.Any())
				{
					if (!columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
						throw new ArgumentException("Input columns must contain all primary keys for batch UPDATE.", parameter);
				}
				else
					throw new ArgumentException($"Table {this._TableSchema.Name} must have a primary key for batch UPDATE.", "Table");
			}
			else
				throw new ArgumentException("No input columns for batch UPDATE.", parameter);
		}

		private void ValidateInputDataColumns(string parameter, string[] inputColumns, string[] dataColumns)
		{
			if (dataColumns.Any())
			{
				var invalidColumnCsv = dataColumns.Without(inputColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Data columns contain columns that are not available from the input: {invalidColumnCsv}", parameter);

				var writableColumns = this._TableSchema.Columns.If(column => !column.Readonly).To(column => column.Name);
				invalidColumnCsv = dataColumns.Without(writableColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Data columns for table {this._TableSchema.Name} contain non-writable columns: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns are required.", parameter);
		}

		private void ValidateColumnsExist(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var writableColumns = this._TableSchema.Columns.To(column => column.Name);
				var invalidColumnCsv = columns.Without(writableColumns).ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns do not exist: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns are required.", parameter);
		}

		private void ValidateColumnsReadable<T>(string parameter, IPropertyCache<T> propertyCache, string[] columns) where T : class, new()
		{
			if (columns.Any())
			{
				var readableProperties = propertyCache.Properties
					.GetValues(columns)
					.If(property => property.GetMethod != null)
					.To(property => property.Name);
				var invalidColumnCsv = columns
					.Without(readableProperties)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns must have readable properties from type '{typeof(T).Name}': {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns for readable properties are required.", parameter);
		}

		private void ValidateColumnsWritable(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var writableColumns = this._TableSchema.Columns
					.If(column => !column.Readonly)
					.To(column => column.Name);
				var invalidColumnCsv = columns
					.Without(writableColumns)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Must not include non-writable columns: {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Writable columns are required.", parameter);
		}

		private void ValidateColumnsWritable<T>(string parameter, IPropertyCache<T> propertyCache, string[] columns) where T : class, new()
		{
			if (columns.Any())
			{
				var writableProperties = propertyCache.Properties
					.GetValues(columns)
					.If(property => property.SetMethod != null)
					.To(property => property.Name);
				var invalidColumnCsv = columns
					.Without(writableProperties)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Columns must have writable properties from type '{typeof(T).Name}': {invalidColumnCsv}", parameter);
			}
			else
				throw new ArgumentException("Columns for writable properties are required.", parameter);
		}

		private void ValidateInputRows(string parameter, RowSet input)
		{
			if (input.Rows.Any())
			{
				var invalidRowCount = input.Rows
					.If(row => row.Length != input.Columns.Length)
					.Count();
				if (invalidRowCount > 0)
					throw new ArgumentException($"{invalidRowCount} input rows have a different number of values than the {input.Columns.Length} columns.", parameter);
			}
			else
				throw new ArgumentException("Input rows are required.", parameter);
		}

		private void ValidateInputRows<T>(string parameter, RowSet<T> input) where T : class, new()
		{
			if (!input.Rows.Any())
				throw new ArgumentException("Input rows are required.", parameter);
		}

		private void ValidateIsTable(string parameter)
		{
			if (this._TableSchema.Type != TableType.Table)
				throw new ArgumentException($"{this._TableSchema.Name} is a {this._TableSchema.Type.Name()} and not a {TableType.Table.Name()}.",
					parameter);
		}

		private void ValidateOnColumns(string parameter, string[] columns)
		{
			if (columns.Any())
			{
				var primaryKeys = this._TableSchema.Columns
					.If(column => column.PrimaryKey)
					.To(column => column.Name)
					.ToArray();
				if (primaryKeys.Any() && !columns.Has(primaryKeys, StringComparer.OrdinalIgnoreCase))
					throw new ArgumentException("ON columns must contain all primary keys for SQL MERGE.", parameter);
			}
			else
				throw new ArgumentException("SQL MERGE requires ON columns.", parameter);
		}

		private void ValidateOutputColumns(string parameter, string[] outputColumns)
		{
			if (outputColumns.Any())
			{
				var columns = this._TableSchema.Columns.To(column => column.Name);
				var invalidColumnCsv = outputColumns
					.Without(columns)
					.ToCsv(column => $"[{column}]");
				if (!invalidColumnCsv.IsBlank())
					throw new ArgumentException($"Invalid OUTPUT columns for table {this._TableSchema.Name}: {invalidColumnCsv}", parameter);
			}
		}
	}
}
