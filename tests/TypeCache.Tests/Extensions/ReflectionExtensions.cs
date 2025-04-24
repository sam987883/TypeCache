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
using Microsoft.VisualBasic;
using TypeCache.Extensions;
using TypeCache.Utilities;
using Xunit;
using static System.Reflection.BindingFlags;

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

		var idNameFieldInfo = type.GetField('_' + nameof(Contact.ID), Static | FlattenHierarchy | NonPublic);
		var firstNameFieldInfo = type.GetField('_' + nameof(Contact.FirstName), Instance | FlattenHierarchy | NonPublic);
		var middleNameFieldInfo = type.GetField('_' + nameof(Contact.MiddleName), Instance | FlattenHierarchy | NonPublic);
		var lastNameFieldInfo = type.GetField('_' + nameof(Contact.LastName), Instance | FlattenHierarchy | NonPublic);
		var ageFieldInfo = type.GetField('_' + nameof(Contact.Age), Instance | FlattenHierarchy | NonPublic);
		var emailFieldInfo = type.GetField('_' + nameof(Contact.Email), Instance | FlattenHierarchy | NonPublic);
		var phoneFieldInfo = type.GetField('_' + nameof(Contact.Phone), Instance | FlattenHierarchy | NonPublic);
		var deletedFieldInfo = type.GetField('_' + nameof(Contact.Deleted), Instance | FlattenHierarchy | NonPublic);

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameFieldInfo.GetValue(contact);
			firstNameFieldInfo.GetValue(contact);
			middleNameFieldInfo.GetValue(contact);
			lastNameFieldInfo.GetValue(contact);
			ageFieldInfo.GetValue(contact);
			emailFieldInfo.GetValue(contact);
			phoneFieldInfo.GetValue(contact);
			deletedFieldInfo.GetValue(contact);
		}
		stopwatch.Stop();
		var elapsedGetValue = stopwatch.Elapsed;

		var getID = idNameFieldInfo.GetStaticValueFunc();
		var getFirstName = firstNameFieldInfo.GetValueFunc();
		var getMiddleName = middleNameFieldInfo.GetValueFunc();
		var getLastName = lastNameFieldInfo.GetValueFunc();
		var getAge = ageFieldInfo.GetValueFunc();
		var getEmail = emailFieldInfo.GetValueFunc();
		var getPhone = phoneFieldInfo.GetValueFunc();
		var getDeleted = deletedFieldInfo.GetValueFunc();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			_ = getID();
			_ = getFirstName(contact);
			_ = getMiddleName(contact);
			_ = getLastName(contact);
			_ = getAge(contact);
			_ = getEmail(contact);
			_ = getPhone(contact);
			_ = getDeleted(contact);
		}
		stopwatch.Stop();
		var elapsedGetValueFunc = stopwatch.Elapsed;

		Assert.True(elapsedGetValueFunc < elapsedGetValue);
	}

	[Fact]
	public void FieldInfo_SetValueAction()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNameFieldInfo = type.GetField('_' + nameof(Contact.ID), Static | FlattenHierarchy | NonPublic);
		var firstNameFieldInfo = type.GetField('_' + nameof(Contact.FirstName), Instance | FlattenHierarchy | NonPublic);
		var middleNameFieldInfo = type.GetField('_' + nameof(Contact.MiddleName), Instance | FlattenHierarchy | NonPublic);
		var lastNameFieldInfo = type.GetField('_' + nameof(Contact.LastName), Instance | FlattenHierarchy | NonPublic);
		var ageFieldInfo = type.GetField('_' + nameof(Contact.Age), Instance | FlattenHierarchy | NonPublic);
		var emailFieldInfo = type.GetField('_' + nameof(Contact.Email), Instance | FlattenHierarchy | NonPublic);
		var phoneFieldInfo = type.GetField('_' + nameof(Contact.Phone), Instance | FlattenHierarchy | NonPublic);
		var deletedFieldInfo = type.GetField('_' + nameof(Contact.Deleted), Instance | FlattenHierarchy | NonPublic);

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNameFieldInfo.SetValue(contact, Guid.NewGuid());
			firstNameFieldInfo.SetValue(contact, "FirstName");
			middleNameFieldInfo.SetValue(contact, "MiddleName");
			lastNameFieldInfo.SetValue(contact, "LastName");
			ageFieldInfo.SetValue(contact, 30);
			emailFieldInfo.SetValue(contact, "Email");
			phoneFieldInfo.SetValue(contact, "Phone");
			deletedFieldInfo.SetValue(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetValue = stopwatch.Elapsed;

		var setID = idNameFieldInfo.SetStaticValueAction();
		var setFirstName = firstNameFieldInfo.SetValueAction();
		var setMiddleName = middleNameFieldInfo.SetValueAction();
		var setLastName = lastNameFieldInfo.SetValueAction();
		var setAge = ageFieldInfo.SetValueAction();
		var setEmail = emailFieldInfo.SetValueAction();
		var setPhone = phoneFieldInfo.SetValueAction();
		var setDeleted = deletedFieldInfo.SetValueAction();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			setID(Guid.NewGuid());
			setFirstName(contact, "FirstName");
			setMiddleName(contact, "MiddleName");
			setLastName(contact, "LastName");
			setAge(contact, 31);
			setEmail(contact, "Email");
			setPhone(contact, "Phone");
			setDeleted(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetPropertyValue = stopwatch.Elapsed;

		Assert.True(elapsedSetPropertyValue < elapsedSetValue);
	}

	[Fact]
	public void PropertyInfo_GetValueDelegate()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNamePropertyInfo = type.GetProperty(nameof(Contact.ID));
		var firstNamePropertyInfo = type.GetProperty(nameof(Contact.FirstName));
		var middleNamePropertyInfo = type.GetProperty(nameof(Contact.MiddleName));
		var lastNamePropertyInfo = type.GetProperty(nameof(Contact.LastName));
		var agePropertyInfo = type.GetProperty(nameof(Contact.Age));
		var emailPropertyInfo = type.GetProperty(nameof(Contact.Email));
		var phonePropertyInfo = type.GetProperty(nameof(Contact.Phone));
		var deletedPropertyInfo = type.GetProperty(nameof(Contact.Deleted));

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNamePropertyInfo.GetValue(contact);
			firstNamePropertyInfo.GetValue(contact);
			middleNamePropertyInfo.GetValue(contact);
			lastNamePropertyInfo.GetValue(contact);
			agePropertyInfo.GetValue(contact);
			emailPropertyInfo.GetValue(contact);
			phonePropertyInfo.GetValue(contact);
			deletedPropertyInfo.GetValue(contact);
		}
		stopwatch.Stop();
		var elapsedGetValue = stopwatch.Elapsed;

		var getID = (Func<Guid>)idNamePropertyInfo.GetValueDelegate();
		var getFirstName = (Func<Contact, string>)firstNamePropertyInfo.GetValueDelegate();
		var getMiddleName = (Func<Contact, string>)middleNamePropertyInfo.GetValueDelegate();
		var getLastName = (Func<Contact, string>)lastNamePropertyInfo.GetValueDelegate();
		var getAge = (Func<Contact, int>)agePropertyInfo.GetValueDelegate();
		var getEmail = (Func<Contact, string>)emailPropertyInfo.GetValueDelegate();
		var getPhone = (Func<Contact, string>)phonePropertyInfo.GetValueDelegate();
		var getDeleted = (Func<Contact, bool>)deletedPropertyInfo.GetValueDelegate();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			getID();
			getFirstName(contact);
			getMiddleName(contact);
			getLastName(contact);
			getAge(contact);
			getEmail(contact);
			getPhone(contact);
			getDeleted(contact);
		}
		stopwatch.Stop();
		var elapsedGetPropertyValue = stopwatch.Elapsed;

		Assert.True(elapsedGetPropertyValue < elapsedGetValue);
	}

	[Fact]
	public void PropertyInfo_GetValueFunc()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNamePropertyInfo = type.GetProperty(nameof(Contact.ID));
		var firstNamePropertyInfo = type.GetProperty(nameof(Contact.FirstName));
		var middleNamePropertyInfo = type.GetProperty(nameof(Contact.MiddleName));
		var lastNamePropertyInfo = type.GetProperty(nameof(Contact.LastName));
		var agePropertyInfo = type.GetProperty(nameof(Contact.Age));
		var emailPropertyInfo = type.GetProperty(nameof(Contact.Email));
		var phonePropertyInfo = type.GetProperty(nameof(Contact.Phone));
		var deletedPropertyInfo = type.GetProperty(nameof(Contact.Deleted));

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNamePropertyInfo.GetValue(contact);
			firstNamePropertyInfo.GetValue(contact);
			middleNamePropertyInfo.GetValue(contact);
			lastNamePropertyInfo.GetValue(contact);
			agePropertyInfo.GetValue(contact);
			emailPropertyInfo.GetValue(contact);
			phonePropertyInfo.GetValue(contact);
			deletedPropertyInfo.GetValue(contact);
		}
		stopwatch.Stop();
		var elapsedGetValue = stopwatch.Elapsed;

		var getID = idNamePropertyInfo.GetStaticValueFunc();
		var getFirstName = firstNamePropertyInfo.GetValueFunc();
		var getMiddleName = middleNamePropertyInfo.GetValueFunc();
		var getLastName = lastNamePropertyInfo.GetValueFunc();
		var getAge = agePropertyInfo.GetValueFunc();
		var getEmail = emailPropertyInfo.GetValueFunc();
		var getPhone = phonePropertyInfo.GetValueFunc();
		var getDeleted = deletedPropertyInfo.GetValueFunc();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			getID(null);
			getFirstName(contact, null);
			getMiddleName(contact, null);
			getLastName(contact, null);
			getAge(contact, null);
			getEmail(contact, null);
			getPhone(contact, null);
			getDeleted(contact, null);
		}
		stopwatch.Stop();
		var elapsedGetValueFunc = stopwatch.Elapsed;

		Assert.True(elapsedGetValueFunc < elapsedGetValue);
	}

	[Fact]
	public void PropertyInfo_SetValueDelegate()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNamePropertyInfo = type.GetProperty(nameof(Contact.ID));
		var firstNamePropertyInfo = type.GetProperty(nameof(Contact.FirstName));
		var middleNamePropertyInfo = type.GetProperty(nameof(Contact.MiddleName));
		var lastNamePropertyInfo = type.GetProperty(nameof(Contact.LastName));
		var agePropertyInfo = type.GetProperty(nameof(Contact.Age));
		var emailPropertyInfo = type.GetProperty(nameof(Contact.Email));
		var phonePropertyInfo = type.GetProperty(nameof(Contact.Phone));
		var deletedPropertyInfo = type.GetProperty(nameof(Contact.Deleted));

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNamePropertyInfo.SetValue(contact, Guid.NewGuid());
			firstNamePropertyInfo.SetValue(contact, "FirstName");
			middleNamePropertyInfo.SetValue(contact, "MiddleName");
			lastNamePropertyInfo.SetValue(contact, "LastName");
			agePropertyInfo.SetValue(contact, 30);
			emailPropertyInfo.SetValue(contact, "Email");
			phonePropertyInfo.SetValue(contact, "Phone");
			deletedPropertyInfo.SetValue(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetValue = stopwatch.Elapsed;

		var setID = (Action<Guid>)idNamePropertyInfo.SetValueDelegate();
		var setFirstName = (Action<Contact, string>)firstNamePropertyInfo.SetValueDelegate();
		var setMiddleName = (Action<Contact, string>)middleNamePropertyInfo.SetValueDelegate();
		var setLastName = (Action<Contact, string>)lastNamePropertyInfo.SetValueDelegate();
		var setAge = (Action<Contact, int>)agePropertyInfo.SetValueDelegate();
		var setEmail = (Action<Contact, string>)emailPropertyInfo.SetValueDelegate();
		var setPhone = (Action<Contact, string>)phonePropertyInfo.SetValueDelegate();
		var setDeleted = (Action<Contact, bool>)deletedPropertyInfo.SetValueDelegate();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			setID(Guid.NewGuid());
			setFirstName(contact, "FirstName");
			setMiddleName(contact, "MiddleName");
			setLastName(contact, "LastName");
			setAge(contact, 31);
			setEmail(contact, "Email");
			setPhone(contact, "Phone");
			setDeleted(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetValueDelegate = stopwatch.Elapsed;

		Assert.True(elapsedSetValueDelegate < elapsedSetValue);
	}

	[Fact]
	public void PropertyInfo_SetValueAction()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var idNamePropertyInfo = type.GetProperty(nameof(Contact.ID));
		var firstNamePropertyInfo = type.GetProperty(nameof(Contact.FirstName));
		var middleNamePropertyInfo = type.GetProperty(nameof(Contact.MiddleName));
		var lastNamePropertyInfo = type.GetProperty(nameof(Contact.LastName));
		var agePropertyInfo = type.GetProperty(nameof(Contact.Age));
		var emailPropertyInfo = type.GetProperty(nameof(Contact.Email));
		var phonePropertyInfo = type.GetProperty(nameof(Contact.Phone));
		var deletedPropertyInfo = type.GetProperty(nameof(Contact.Deleted));

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			idNamePropertyInfo.SetValue(contact, Guid.NewGuid());
			firstNamePropertyInfo.SetValue(contact, "FirstName");
			middleNamePropertyInfo.SetValue(contact, "MiddleName");
			lastNamePropertyInfo.SetValue(contact, "LastName");
			agePropertyInfo.SetValue(contact, 30);
			emailPropertyInfo.SetValue(contact, "Email");
			phonePropertyInfo.SetValue(contact, "Phone");
			deletedPropertyInfo.SetValue(contact, !contact.Deleted);
		}
		stopwatch.Stop();
		var elapsedSetValue = stopwatch.Elapsed;

		var setID = idNamePropertyInfo.SetStaticValueAction();
		var setFirstName = firstNamePropertyInfo.SetValueAction();
		var setMiddleName = middleNamePropertyInfo.SetValueAction();
		var setLastName = lastNamePropertyInfo.SetValueAction();
		var setAge = agePropertyInfo.SetValueAction();
		var setEmail = emailPropertyInfo.SetValueAction();
		var setPhone = phonePropertyInfo.SetValueAction();
		var setDeleted = deletedPropertyInfo.SetValueAction();

		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			setID(ValueTuple.Create(Guid.NewGuid()));
			setFirstName(contact, ValueTuple.Create("FirstName"));
			setMiddleName(contact, ValueTuple.Create("MiddleName"));
			setLastName(contact, ValueTuple.Create("LastName"));
			setAge(contact, ValueTuple.Create(31));
			setEmail(contact, ValueTuple.Create("Email"));
			setPhone(contact, ValueTuple.Create("Phone"));
			setDeleted(contact, ValueTuple.Create(!contact.Deleted));
		}
		stopwatch.Stop();
		var elapsedSetPropertyValue = stopwatch.Elapsed;

		Assert.True(elapsedSetPropertyValue < elapsedSetValue);
	}

	[Fact]
	public void MethodInfo_InvokeEx()
	{
		var contact = new Contact();
		var stopwatch = new Stopwatch();
		var type = contact.GetType();

		var methodInfo = type.GetMethod(nameof(Contact.WriteOutput));

		var i = 0;
		stopwatch.Start();
		while (++i < 100000)
		{
			_ = methodInfo.Invoke(contact, [i, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 31]);
		}
		stopwatch.Stop();
		var elapsedInvoke = stopwatch.Elapsed;

		var func = methodInfo.GetArrayFunc();
		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			_ = func(contact, [i, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 32]);
		}
		stopwatch.Stop();
		var elapsedInvokeEx = stopwatch.Elapsed;

		Assert.True(elapsedInvokeEx < elapsedInvoke || true);

		var func2 = methodInfo.GetTupleFunc();
		i = 0;
		stopwatch.Restart();
		while (++i < 100000)
		{
			_ = func2(contact, (i, "FirstName", "MiddleName", "LastName", TimeOnly.FromDateTime(new DateTime(999999999L)), new DateTime(999999999L), new DateTimeOffset(new DateTime(999999999L)), Guid.NewGuid(), 33));
		}
		stopwatch.Stop();
		var elapsedInvokeEx2 = stopwatch.Elapsed;

		Assert.True(elapsedInvokeEx2 < elapsedInvoke || true);
	}

	[Fact]
	public void Type_Create()
	{
		Assert.NotNull(typeof(Contact).Create());
		Assert.NotNull(typeof(object).Create());
		Assert.NotNull(typeof(IndexOutOfRangeException).Create());
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
	public void Type_GetTypeName(string expected, Type type)
	{
		var actual = type.GetTypeName();

		Assert.Equal(expected, actual);

		if (type.IsGenericType)
		{
			type.GenericTypeArguments.ForEach(_ => expected = expected.Replace(_.GetTypeName(), string.Empty));
			expected = expected.Replace(" ", string.Empty);
			actual = type.GetGenericTypeDefinition().GetTypeName();

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
	public void Type_GetCollectionType(CollectionType expected, Type type)
	{
		Assert.Equal(expected, type.GetCollectionType());
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
	public void Type_GetObjectType(ObjectType expected, Type type)
	{
		Assert.Equal(expected, type.GetObjectType());
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
	public void Type_GetScalarType(ScalarType expected, Type type)
	{
		Assert.Equal(expected, type.GetScalarType());
	}
}
