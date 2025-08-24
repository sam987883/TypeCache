// Copyright (c) 2021 Samuel Abraham

namespace TypeCache.Mapping;

public interface IMapper
{
	void Map<T>(IEnumerable<KeyValuePair<string, object?>> source, T target)
		where T : class;

	void Map<T>(IDictionary<string, object?> source, T target)
		where T : class;

	void Map<S>(S source, IDictionary<string, object?> target)
		where S : class;

	void Map<S, T>(S source, T target)
		where S : class
		where T : class;

	void Map(object source, object target);
}
