># TypeCache {$}

###### sam987883@gmail.com  

[**Source Code**](https://github.com/sam987883/TypeCache/tree/master/src/TypeCache.GraphQL)

[Request Features (or report a bug) (if any)](https://github.com/sam987883/TypeCache/issues)

---
### `TypeCache.Extensions` - Faster Reflection

	using TypeCache.Extensions;

	// Create an instance of Senator using the default constructor
	var intance = typeof(Senator).Create();

	// Create an instance of Customer using a matching constructor
	var intance = typeof(Senator).Create(["Bob Dole", 98]); // Passing an array of values
	var intance = typeof(Senator).Create(("Bob Dole", 98)); // Passing a tuple of values

	// Find the matching ConstructorInfo object based on argument values
	var comstructorInfo = typeof(Senator).FindConstructor(["Bob Dole", 98]) // Passing an array of values
	var comstructorInfo = typeof(Senator).FindConstructor(("Bob Dole", 98)) // Passing a tuple of values

	// Find a matching MethodInfo object based on argument values
	var methodInfo = typeof(Senator).FindMethod("RunForPresident", [typeof(int), typeof(bool)]); // Find method by argument types
	var methodInfo = typeof(Senator).FindMethod("RunForPresident", [1996, true]); // Passing an array of values
	var methodInfo = typeof(Senator).FindMethod("RunForPresident", (1996, true)); // Passing a tuple of values

	// Get a field value
	var fieldValue = typeof(Senator).GetFieldValue(instance, "_Bills");

	var fieldInfo = typeof(Senator).GetFieldInfo(instance, "_Bills");
	var fieldValue = fieldInfo.GetValueEx("_Bills");

	// Set a field value
	var fieldValue = typeof(Senator).SetFieldValue(instance, "_Bills", 47);

	var fieldInfo = typeof(Senator).GetFieldInfo(instance, "_Bills");
	var fieldValue = fieldInfo.SetValueEx("_Bills");

	// Get a property value
	var propertyValue = typeof(Senator).GetPropertyValue(instance, "DoleWhip");

	var propertyInfo = typeof(Senator).GetPropertyInfo(instance, "DoleWhip");
	var propertyValue = propertyInfo.GetValueEx("DoleWhip");

	// Set a property value
	var propertyValue = typeof(Senator).SetPropertyValue(instance, "DoleWhip", Fruits.Pineapple);

	var propertyInfo = typeof(Senator).GetPropertyInfo(instance, "DoleWhip");
	var propertyValue = propertyInfo.SetValueEx("DoleWhip", Fruits.Pineapple);

	// Invoke a method
	typeof(Senator).InvokeAction("StopHillaryCare", ["Oh noes", Action.Veto, false]); // Passing an array of values
	typeof(Senator).InvokeAction("StopHillaryCare", ("Oh noes", Action.Veto, false)); // Passing a tuple of values

	var state = typeof(Senator).InvokeFunc("GetState");

---
### `TypeCache.Extensions` - Better Object Mapping

	using TypeCache.Extensions;

	var dictionary = new Dictionary<string, object>(2)
	{
		{ "Name", "Bob Dole" },
		{ "Age", 98 }
	};
	var bobDole1 = new Representative();
	var bobDole2 = new Senator();
	var bobDole3 = null as President;

	dictionary.MapTo(bobDole1); // Automatically maps dictionary entries to properties
	bobDole1.MapTo(bobDole2); // Automatically maps model properties with same names

	// To further customize mappings, use MapAttribute and MapIgnoreAttribute

---
### `TypeCache.Mediation` - Mediator that supports named rules

	using TypeCache.Mediation;

	IMediator mediator; // Injected

	var bobDole1 = new Representative(); // Implements IRequest<Senator>
	Senator bobDole2 = mediator.Map(bobDole1); // Implements IRequest<President>

	mediator.Execute("Presidential Campaign", bobDole2);

	President? bobDole3 = mediator.Map(nameof(President), bobDole2); // Map using named Rule

	bobDole3.AssertNotNull(); // Unhandled exception

---
### `TypeCache.Data` - Simple Robust Database CRUD Access



---
### `TypeCache.Extensions` - Helpful Extensions

	using TypeCache.Extensions;

	// Assertions
	instance.AssertNotNull(); // Throws ArgumentNullException if instance is null

	text.AssertNotBlank(); // Throws ArgumentException if text is null, empty or whitespaces

	success.AssertTrue(); // Throws ArgumentException is success is not true (false or null)

	// Action/Func Retry on fail
	Action runForPresident;

	runForPresident.Retry([TimeSpan.FromYears(4)]);

	var timeSpan = runForPresident.Timed(); // Involes the Action and returns how long it took to run

	// Comparable Extensions
	IComparable<Senator> gingrich, bobDole;
	var success = bobDole.CompareTo(gingrich) > 0; // Cryptic
	success = bobDole.GreaterThan(gingrich); // More readable

	// DateTime extensions
	var bobDoleDOB = new DateOnly(1923, 7, 22);

	// The following does not throw an exception as Between is inclusive of its endpoints.
	bobDoleDOB.Between(new DateOnly(1923, 7, 22), new DateOnly(2021, 12, 5)).AssertTrue();

	// The following throws an exception as InBetween is exclusive of its endpoints.
	bobDoleDOB.InBetween(new DateOnly(1923, 7, 22), new DateOnly(2021, 12, 5)).AssertTrue();

	var dateOnly = DateTime.UtcNow.ToDateOnly();
	var dateTimeOffset = DateTime.UtcNow.ToDateTimeOffset();
	var dateForUI = DateTime.UtcNow.ToISO8601();

	var utcDateTime = DateTIme.Now.ToTimeZone(TimeZoneInfo.Utc)
	utcDateTime = DateTIme.Now.ToUTC();

	// JSON
	JsonNode json;
	// Access JSON nodes using JSON Path expression
	var nodes = json.GetNodes("$.Politicians.Senators[1].*");
