// Copyright (c) 2021 Samuel Abraham

using System;
using System.Collections.Generic;
using System.Diagnostics;
using TypeCache.Extensions;
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
	[InlineData("Int32[]", typeof(int[]))]
	[InlineData("Int32[][][]", typeof(int[][][]))]
	[InlineData("IList<Boolean>", typeof(IList<bool>))]
	[InlineData("IDictionary<String, List<Int32>>", typeof(IDictionary<string, List<int>>))]
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
}
