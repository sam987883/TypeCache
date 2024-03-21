># TypeCache - GraphQL
###### sam987883@gmail.com  

[**Source Code**](https://github.com/sam987883/TypeCache/tree/master/src/TypeCache.GraphQL)

[Request Features (or report a bug) (if any)](https://github.com/sam987883/TypeCache/issues)

---
### `TypeCache.GraphQL.Types` - GraphQL Type Objects

- `GraphQLEnumType`
- `GraphQLHashIdType`
- `GraphQLInputType`
- `GraphQLInterfaceType`
- `GraphQLObjectType`
- `GraphQLScalarType`
- `GraphQLUnionType`
- `GraphQLUriType`
---
### `TypeCache.GraphQL.Attributes` - GraphQL Attributes

- `GraphQLDeprecationReasonAttribute`
- `GraphQLDescriptionAttribute`
- `GraphQLIgnoreAttribute`
- `GraphQLInputNameAttribute`
- `GraphQLMutationAttribute`
- `GraphQLNameAttribute`
- `GraphQLQueryAttribute`
- `GraphQLSubscriptionAttribute`
- `GraphQLTypeAttribute`
---
### `GraphQL.Types.ISchema` - GraphQL ISchema Extensions

- `AddVersion(...)`
- `AddDatabaseSchemaQueries(...)`
- `AddDatabaseSchemaQuery(...)`
- `AddDatabaseEndpoints(...)`
- `AddEndpoints<>(...)`
- `AddQuery(...)`
- `AddQueries<>(...)`
- `AddMutation(...)`
- `AddMutations<>(...)`
- `AddSubscription(...)`
- `AddSubscriptions<>(...)`
- `AddSqlApiEndpoints<>(...)`
- `AddSqlApiCallProcedureEndpoint<>(...)`
- `AddSqlApiDeleteDataEndpoint<>(...)`
- `AddSqlApiDeleteEndpoint<>(...)`
- `AddSqlApiInsertDataEndpoint<>(...)`
- `AddSqlApiInsertEndpoint<>(...)`
- `AddSqlApiSelectEndpoint<>(...)`
- `AddSqlApiUpdateDataEndpoint<>(...)`
- `AddSqlApiUpdateEndpoint<>(...)`
---
### `TypeCache.GraphQL.Types.GraphQLObject<T>` - GraphQL GraphQLObject<T> Extensions

- `AddField(MethodInfo)`
- `AddQueryItem<CHILD, MATCH>(MethodInfo, Func<T, MATCH>, Func<CHILD, MATCH>)`
- `AddQueryCollection<CHILD, MATCH>(MethodInfo, Func<T, MATCH>, Func<CHILD, MATCH>)`
