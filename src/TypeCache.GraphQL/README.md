># TypeCache - GraphQL
###### sam987883@gmail.com  

[**Source Code**](https://github.com/sam987883/TypeCache/tree/master/src/TypeCache.GraphQL)

[Request Features (or report a bug) (if any)](https://github.com/sam987883/TypeCache/issues)

---
### GraphQL Type Objects

- `TypeCache.GraphQL.Types.`__`GraphQLEnumType`__
- `TypeCache.GraphQL.Types.`__`GraphQLHashIdType`__
- `TypeCache.GraphQL.Types.`__`GraphQLInputType`__
- `TypeCache.GraphQL.Types.`__`GraphQLInterfaceType`__
- `TypeCache.GraphQL.Types.`__`GraphQLObjectType`__
- `TypeCache.GraphQL.Types.`__`GraphQLUnionType`__
---
### GraphQL Attributes

- `TypeCache.GraphQL.Attributes.`__`GraphQLDeprecationReasonAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLDescriptionAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLIgnoreAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLInputNameAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLMutationAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLNameAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLQueryAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLSubscriptionAttribute`__
- `TypeCache.GraphQL.Attributes.`__`GraphQLTypeAttribute`__
---
### GraphQL ISchema Extensions

- `GraphQL.Types.ISchema.`__`AddVersion(...)`__
- `GraphQL.Types.ISchema.`__`AddDatabaseSchemaQueries(...)`__
- `GraphQL.Types.ISchema.`__`AddDatabaseSchemaQuery(...)`__
- `GraphQL.Types.ISchema.`__`AddDatabaseEndpoints(...)`__
- `GraphQL.Types.ISchema.`__`AddEndpoints<>(...)`__
- `GraphQL.Types.ISchema.`__`AddQuery(...)`__
- `GraphQL.Types.ISchema.`__`AddQueries<>(...)`__
- `GraphQL.Types.ISchema.`__`AddMutation(...)`__
- `GraphQL.Types.ISchema.`__`AddMutations<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSubscription(...)`__
- `GraphQL.Types.ISchema.`__`AddSubscriptions<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiEndpoints<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiCallProcedureEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiDeleteDataEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiDeleteEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiInsertDataEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiInsertEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiSelectEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiUpdateDataEndpoint<>(...)`__
- `GraphQL.Types.ISchema.`__`AddSqlApiUpdateEndpoint<>(...)`__
---
### GraphQL GraphQLObject<T> Extensions

- `TypeCache.GraphQL.Types.GraphQLObject<T>.`__`AddField(MethodInfo)`__
- `TypeCache.GraphQL.Types.GraphQLObject<T>.`__`AddQueryItem<CHILD, MATCH>(MethodInfo, Func<T, MATCH>, Func<CHILD, MATCH>)`__
- `TypeCache.GraphQL.Types.GraphQLObject<T>.`__`AddQueryCollection<CHILD, MATCH>(MethodInfo, Func<T, MATCH>, Func<CHILD, MATCH>)`__
