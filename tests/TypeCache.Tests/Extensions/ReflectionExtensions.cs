// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using TypeCache.Adapters;
using TypeCache.Extensions;
using TypeCache.Reflection;
using TypeCache.Utilities;
using Xunit;

namespace TypeCache.Tests.Extensions;

public class ReflectionExtensions
{
	private class Contact
	{
		private static Guid _ID;
		private string _FirstName;
		private string _MiddleName;
		private string _LastName;
		private int _Age;
		private string _Email;
		private string _Phone;
		private bool _Deleted;

		public static Guid ID { get => _ID; set => _ID = value; }
		public string FirstName { get => this._FirstName; set => this._FirstName = value; }
		public string MiddleName { get => this._MiddleName; set => this._MiddleName = value; }
		public string LastName { get => this._LastName; set => this._LastName = value; }
		public int Age { get => this._Age; set => this._Age = value; }
		public string Email { get => this._Email; set => this._Email = value; }
		public string Phone { get => this._Phone; set => this._Phone = value; }
		public bool Deleted { get => this._Deleted; set => this._Deleted = value; }

		public long WriteOutput(int i, string text1, string text2, string text3, TimeOnly timeOnly, DateTime dateTime, DateTimeOffset dateTimeOffset, Guid guid, int age)
		{
			if (i % 10000 is 0)
				Console.WriteLine($"{i}: {text1}, {text2}, {text3}, {timeOnly}, {dateTime}, {dateTimeOffset}, {guid}, {age}");
			return i % 10000;
		}
	}

	[Fact]
	public void FieldInfo_GetValueFunc()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNameField = type.StaticFields()['_' + nameof(Contact.ID)];
		var firstNameField = type.Fields()['_' + nameof(Contact.FirstName)];
		var middleNameField = type.Fields()['_' + nameof(Contact.MiddleName)];
		var lastNameField = type.Fields()['_' + nameof(Contact.LastName)];
		var ageField = type.Fields()['_' + nameof(Contact.Age)];
		var emailField = type.Fields()['_' + nameof(Contact.Email)];
		var phoneField = type.Fields()['_' + nameof(Contact.Phone)];
		var deletedField = type.Fields()['_' + nameof(Contact.Deleted)];

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameField.GetValue();
			firstNameField.GetValue(contact);
			middleNameField.GetValue(contact);
			lastNameField.GetValue(contact);
			ageField.GetValue(contact);
			emailField.GetValue(contact);
			phoneField.GetValue(contact);
			deletedField.GetValue(contact);
		}
		stopwatch.Stop();
		var elapsedGetValue = stopwatch.Elapsed;

		Assert.True(elapsedGetValue > TimeSpan.Zero);
	}

	[Fact]
	public void FieldInfo_SetValueAction()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNameField = type.StaticFields()['_' + nameof(Contact.ID)];
		var firstNameField = type.Fields()['_' + nameof(Contact.FirstName)];
		var middleNameField = type.Fields()['_' + nameof(Contact.MiddleName)];
		var lastNameField = type.Fields()['_' + nameof(Contact.LastName)];
		var ageField = type.Fields()['_' + nameof(Contact.Age)];
		var emailField = type.Fields()['_' + nameof(Contact.Email)];
		var phoneField = type.Fields()['_' + nameof(Contact.Phone)];
		var deletedField = type.Fields()['_' + nameof(Contact.Deleted)];

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameField.SetValue(Guid.NewGuid());
			firstNameField.SetValue(contact, "FirstName");
			middleNameField.SetValue(contact, "MiddleName");
			lastNameField.SetValue(contact, "LastName");
			ageField.SetValue(contact, 30);
			emailField.SetValue(contact, "Email");
			phoneField.SetValue(contact, "Phone");
			deletedField.SetValue(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetValue = stopwatch.Elapsed;

		Assert.True(elapsedSetValue > TimeSpan.Zero);
	}

	[Fact]
	public void Type_Constructors_Create()
	{
		Assert.NotNull(typeof(Contact).Create());
		Assert.NotNull(typeof(object).Create());
		Assert.NotNull(typeof(IndexOutOfRangeException).Create());
	}

	[Fact]
	public void Type_Methods_Invoke()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();

		var method = contact.GetType().Methods()[nameof(Contact.WriteOutput)].Find([0, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 31]);
		var call = ((Func<Contact, int, string, string, string, TimeOnly, DateTime, DateTimeOffset, Guid, int, long>)method.Delegate);

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			_ = call.Invoke(contact, i, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 31);
		}
		stopwatch.Stop();
		var elapsed1 = stopwatch.Elapsed;

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			_ = method.Invoke(contact, [i, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 31]);
		}
		stopwatch.Stop();
		var elapsed2 = stopwatch.Elapsed;

		Assert.True(elapsed1 < elapsed2);
	}

	[Fact]
	public void Type_Properties_GetValue()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var properties = contact.GetType().Properties();

		var idNameProperty = properties[nameof(Contact.ID)];
		var firstNameProperty = properties[nameof(Contact.FirstName)];
		var middleNameProperty = properties[nameof(Contact.MiddleName)];
		var lastNameProperty = properties[nameof(Contact.LastName)];
		var ageProperty = properties[nameof(Contact.Age)];
		var emailProperty = properties[nameof(Contact.Email)];
		var phoneProperty = properties[nameof(Contact.Phone)];
		var deletedProperty = properties[nameof(Contact.Deleted)];

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameProperty.GetStaticValue();
			firstNameProperty.GetValue(contact);
			middleNameProperty.GetValue(contact);
			lastNameProperty.GetValue(contact);
			ageProperty.GetValue(contact);
			emailProperty.GetValue(contact);
			phoneProperty.GetValue(contact);
			deletedProperty.GetValue(contact);
		}
		stopwatch.Stop();
		var elapsed = stopwatch.Elapsed;

		Assert.True(elapsed > TimeSpan.Zero);
	}

	[Fact]
	public void Type_Properties_SetValue()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var properties = contact.GetType().Properties();

		var idNameProperty = properties[nameof(Contact.ID)];
		var firstNameProperty = properties[nameof(Contact.FirstName)];
		var middleNameProperty = properties[nameof(Contact.MiddleName)];
		var lastNameProperty = properties[nameof(Contact.LastName)];
		var ageProperty = properties[nameof(Contact.Age)];
		var emailProperty = properties[nameof(Contact.Email)];
		var phoneProperty = properties[nameof(Contact.Phone)];
		var deletedProperty = properties[nameof(Contact.Deleted)];

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameProperty.SetStaticValue(Guid.NewGuid());
			firstNameProperty.SetValue(contact, "FirstName");
			middleNameProperty.SetValue(contact, "MiddleName");
			lastNameProperty.SetValue(contact, "LastName");
			ageProperty.SetValue(contact, 30);
			emailProperty.SetValue(contact, "Email");
			phoneProperty.SetValue(contact, "Phone");
			deletedProperty.SetValue(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsed = stopwatch.Elapsed;

		Assert.True(elapsed > TimeSpan.Zero);
	}

	[Theory]
	[InlineData("UInt64", typeof(ulong))]
	[InlineData("String", typeof(string))]
	[InlineData("Char*", typeof(char*))]
	[InlineData("Char***", typeof(char***))]
	[InlineData("Int32[]", typeof(int[]))]
	[InlineData("Int32[,,]", typeof(int[,,]))]
	[InlineData("Int32[][][]", typeof(int[][][]))]
	[InlineData("IList<Boolean>", typeof(IList<bool>))]
	[InlineData("IList<>", typeof(IList<>))]
	[InlineData("IDictionary<String, List<Int32>>", typeof(IDictionary<string, List<int>>))]
	[InlineData("IDictionary<,>", typeof(IDictionary<,>))]
	public void Type_CodeName(string expected, Type type)
	{
		var actual = type.CodeName();

		Assert.Equal(expected, actual);

		if (type.IsGenericType)
		{
			type.GenericTypeArguments.ForEach(_ => expected = expected.Replace(_.CodeName(), string.Empty));
			expected = expected.Replace(" ", string.Empty);
			actual = type.GetGenericTypeDefinition().CodeName();

			Assert.Equal(expected, actual);
		}
	}

	[Theory]
	[InlineData(CollectionType.Array, typeof(string[]))]
	[InlineData(CollectionType.ArrayList, typeof(ArrayList))]
	[InlineData(CollectionType.BitArray, typeof(BitArray))]
	[InlineData(CollectionType.BlockingCollection, typeof(BlockingCollection<string>))]
	[InlineData(CollectionType.ConcurrentBag, typeof(ConcurrentBag<string>))]
	[InlineData(CollectionType.ConcurrentDictionary, typeof(ConcurrentDictionary<int, string>))]
	[InlineData(CollectionType.ConcurrentQueue, typeof(ConcurrentQueue<string>))]
	[InlineData(CollectionType.ConcurrentStack, typeof(ConcurrentStack<string>))]
	[InlineData(CollectionType.FrozenDictionary, typeof(FrozenDictionary<int, string>))]
	[InlineData(CollectionType.FrozenSet, typeof(FrozenSet<string>))]
	[InlineData(CollectionType.Hashtable, typeof(Hashtable))]
	[InlineData(CollectionType.HybridDictionary, typeof(HybridDictionary))]
	[InlineData(CollectionType.ImmutableArray, typeof(ImmutableArray<string>))]
	[InlineData(CollectionType.ImmutableSortedDictionary, typeof(ImmutableSortedDictionary<int, string>))]
	[InlineData(CollectionType.ImmutableDictionary, typeof(IImmutableDictionary<int, string>))]
	[InlineData(CollectionType.ImmutableSortedSet, typeof(ImmutableSortedSet<string>))]
	[InlineData(CollectionType.ImmutableSet, typeof(IImmutableSet<string>))]
	[InlineData(CollectionType.ImmutableList, typeof(IImmutableList<string>))]
	[InlineData(CollectionType.ImmutableQueue, typeof(IImmutableQueue<string>))]
	[InlineData(CollectionType.ImmutableStack, typeof(IImmutableStack<string>))]
	[InlineData(CollectionType.KeyedCollection, typeof(KeyedCollection<int, string>))]
	[InlineData(CollectionType.LinkedList, typeof(LinkedList<string>))]
	[InlineData(CollectionType.ListDictionary, typeof(ListDictionary))]
	[InlineData(CollectionType.NameObjectCollection, typeof(NameObjectCollectionBase))]
	[InlineData(CollectionType.ObservableCollection, typeof(ObservableCollection<object>))]
	[InlineData(CollectionType.OrderedDictionary, typeof(IOrderedDictionary))]
	[InlineData(CollectionType.PriorityQueue, typeof(PriorityQueue<int, string>))]
	[InlineData(CollectionType.Queue, typeof(Queue))]
	[InlineData(CollectionType.Queue, typeof(Queue<string>))]
	[InlineData(CollectionType.ReadOnlyObservableCollection, typeof(ReadOnlyObservableCollection<object>))]
	[InlineData(CollectionType.ReadOnlyCollection, typeof(ReadOnlyCollection<object>))]
	[InlineData(CollectionType.ReadOnlyCollection, typeof(ReadOnlyCollectionBase))]
	[InlineData(CollectionType.SortedDictionary, typeof(SortedDictionary<int, string>))]
	[InlineData(CollectionType.SortedList, typeof(SortedList<int, string>))]
	[InlineData(CollectionType.SortedList, typeof(SortedList))]
	[InlineData(CollectionType.SortedSet, typeof(SortedSet<string>))]
	[InlineData(CollectionType.Stack, typeof(Stack<string>))]
	[InlineData(CollectionType.Stack, typeof(Stack))]
	[InlineData(CollectionType.StringCollection, typeof(StringCollection))]
	[InlineData(CollectionType.Collection, typeof(Collection<string>))]
	[InlineData(CollectionType.Collection, typeof(CollectionBase))]
	[InlineData(CollectionType.ReadOnlySet, typeof(IReadOnlySet<string>))]
	[InlineData(CollectionType.Set, typeof(ISet<string>))]
	[InlineData(CollectionType.Dictionary, typeof(IDictionary<int, string>))]
	[InlineData(CollectionType.ReadOnlyDictionary, typeof(IReadOnlyDictionary<int, string>))]
	[InlineData(CollectionType.List, typeof(IList<string>))]
	[InlineData(CollectionType.ReadOnlyList, typeof(IReadOnlyList<string>))]
	[InlineData(CollectionType.ReadOnlyCollection, typeof(IReadOnlyCollection<string>))]
	[InlineData(CollectionType.Collection, typeof(ICollection<string>))]
	[InlineData(CollectionType.None, typeof(string))]
	[InlineData(CollectionType.None, typeof(object))]
	public void Type_CollectionType(CollectionType expected, Type type)
	{
		Assert.Equal(expected, type.CollectionType());
	}

	[Theory]
	[InlineData(ObjectType.Action, typeof(Action))]
	[InlineData(ObjectType.Action, typeof(Action<bool>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string, bool?>))]
	[InlineData(ObjectType.Action, typeof(Action<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string, bool?, nuint>))]
	[InlineData(ObjectType.Action, typeof(Action<,,,,,,,,,,,,,,,>))]
	[InlineData(ObjectType.AsyncEnumerable, typeof(IAsyncEnumerable<string>))]
	[InlineData(ObjectType.AsyncEnumerator, typeof(IAsyncEnumerator<string>))]
	[InlineData(ObjectType.AsyncResult, typeof(IAsyncResult))]
	[InlineData(ObjectType.Attribute, typeof(JsonIgnoreAttribute))]
	[InlineData(ObjectType.DataColumn, typeof(DataColumn))]
	[InlineData(ObjectType.DataRow, typeof(DataRow))]
	[InlineData(ObjectType.DataRowView, typeof(DataRowView))]
	[InlineData(ObjectType.DataSet, typeof(DataSet))]
	[InlineData(ObjectType.DataTable, typeof(DataTable))]
	[InlineData(ObjectType.Enumerable, typeof(List<string>))]
	[InlineData(ObjectType.Enumerator, typeof(List<string>.Enumerator))]
	[InlineData(ObjectType.Exception, typeof(ArgumentException))]
	[InlineData(ObjectType.Func, typeof(Func<bool>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string, bool?>))]
	[InlineData(ObjectType.Func, typeof(Func<bool, int, object, string, Guid, DataSet, DataTable, DataColumn, DataRow, DataRowView, DBNull, int?, string, bool?, nuint>))]
	[InlineData(ObjectType.Func, typeof(Func<,,,,,,,,,,,,,,,>))]
	[InlineData(ObjectType.Func, typeof(Func<,,,,,,,,,,,,,,,,>))]
	[InlineData(ObjectType.Delegate, typeof(Predicate<object>))]
	[InlineData(ObjectType.JsonArray, typeof(JsonArray))]
	[InlineData(ObjectType.JsonDocument, typeof(JsonDocument))]
	[InlineData(ObjectType.JsonElement, typeof(JsonElement))]
	[InlineData(ObjectType.JsonObject, typeof(JsonObject))]
	[InlineData(ObjectType.JsonValue, typeof(JsonValue))]
	[InlineData(ObjectType.Lazy, typeof(Lazy<object>))]
	[InlineData(ObjectType.Lazy, typeof(Lazy<object, string>))]
	[InlineData(ObjectType.Memory, typeof(Memory<object>))]
	[InlineData(ObjectType.Nullable, typeof(decimal?))]
	[InlineData(ObjectType.Object, typeof(object))]
	[InlineData(ObjectType.Observable, typeof(CustomObservable<object>))]
	[InlineData(ObjectType.Observer, typeof(ObserverAdapter<object>))]
	[InlineData(ObjectType.Pointer, typeof(char*))]
	[InlineData(ObjectType.Range, typeof(Range))]
	[InlineData(ObjectType.ReadOnlyMemory, typeof(ReadOnlyMemory<byte>))]
	[InlineData(ObjectType.ReadOnlySpan, typeof(ReadOnlySpan<byte>))]
	[InlineData(ObjectType.ScalarType, typeof(char))]
	[InlineData(ObjectType.ScalarType, typeof(string))]
	[InlineData(ObjectType.Span, typeof(Span<byte>))]
	[InlineData(ObjectType.Stream, typeof(FileStream))]
	[InlineData(ObjectType.StringBuilder, typeof(StringBuilder))]
	[InlineData(ObjectType.Task, typeof(Task))]
	[InlineData(ObjectType.Task, typeof(Task<object>))]
	[InlineData(ObjectType.Tuple, typeof(Tuple<int, string>))]
	[InlineData(ObjectType.Type, typeof(Type))]
	[InlineData(ObjectType.ValueTask, typeof(ValueTask))]
	[InlineData(ObjectType.ValueTask, typeof(ValueTask<object>))]
	[InlineData(ObjectType.ValueTuple, typeof((int, string)))]
	[InlineData(ObjectType.Void, typeof(void))]
	[InlineData(ObjectType.WeakReference, typeof(WeakReference))]
	[InlineData(ObjectType.WeakReference, typeof(WeakReference<object>))]
	[InlineData(ObjectType.Unknown, typeof(HashMaker))]
	public void Type_ObjectType(ObjectType expected, Type type)
	{
		Assert.Equal(expected, type.ObjectType());
	}

	[Theory]
	[InlineData(ScalarType.BigInteger, typeof(BigInteger))]
	[InlineData(ScalarType.Boolean, typeof(bool))]
	[InlineData(ScalarType.Byte, typeof(byte))]
	[InlineData(ScalarType.Char, typeof(char))]
	[InlineData(ScalarType.DateOnly, typeof(DateOnly))]
	[InlineData(ScalarType.DateTime, typeof(DateTime))]
	[InlineData(ScalarType.DateTimeOffset, typeof(DateTimeOffset))]
	[InlineData(ScalarType.Decimal, typeof(decimal))]
	[InlineData(ScalarType.Double, typeof(double))]
	[InlineData(ScalarType.Enum, typeof(Enum))]
	[InlineData(ScalarType.Enum, typeof(StringComparison))]
	[InlineData(ScalarType.Guid, typeof(Guid))]
	[InlineData(ScalarType.Half, typeof(Half))]
	[InlineData(ScalarType.Index, typeof(Index))]
	[InlineData(ScalarType.Int128, typeof(Int128))]
	[InlineData(ScalarType.Int16, typeof(short))]
	[InlineData(ScalarType.Int32, typeof(int))]
	[InlineData(ScalarType.Int64, typeof(long))]
	[InlineData(ScalarType.IntPtr, typeof(nint))]
	[InlineData(ScalarType.None, typeof(object))]
	[InlineData(ScalarType.SByte, typeof(sbyte))]
	[InlineData(ScalarType.Single, typeof(float))]
	[InlineData(ScalarType.String, typeof(string))]
	[InlineData(ScalarType.TimeOnly, typeof(TimeOnly))]
	[InlineData(ScalarType.TimeSpan, typeof(TimeSpan))]
	[InlineData(ScalarType.UInt128, typeof(UInt128))]
	[InlineData(ScalarType.UInt16, typeof(ushort))]
	[InlineData(ScalarType.UInt32, typeof(uint))]
	[InlineData(ScalarType.UInt64, typeof(ulong))]
	[InlineData(ScalarType.UIntPtr, typeof(nuint))]
	[InlineData(ScalarType.Uri, typeof(Uri))]
	public void Type_ScalarType(ScalarType expected, Type type)
	{
		Assert.Equal(expected, type.ScalarType());
	}
}
