># TypeCache - Web
###### sam987883@gmail.com  

[**Source Code**](https://github.com/sam987883/TypeCache/tree/master/src/TypeCache.GraphQL)

[Request Features (or report a bug) (if any)](https://github.com/sam987883/TypeCache/issues)

---
# SQL Web API Extensions
---
### `Microsoft.Extensions.DependencyInjection.IServiceCollection` - Configure SQL API:
- `ConfigureSqlApi(...)`
---
### `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder` - API Endpoints:
- `MapSqlApi(...)`
- `MapSqlApiCallProcedure(...)`
- `MapSqlApiDelete(...)`
- `MapSqlApiDeleteBatch(...)`
- `MapSqlApiInsert(...)`
- `MapSqlApiInsertBatch(...)`
- `MapSqlApiSchemaGet(...)`
- `MapSqlApiSelectTable(...)`
- `MapSqlApiSelectView(...)`
- `MapSqlApiUpdate(...)`
- `MapSqlApiUpdateBatch(...)`
---
### `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder` - Get SQL Endpoints:
- `MapSqlGetSQL(...)`
- `MapSqlApiGetDeleteSQL(...)`
- `MapSqlApiGetDeleteBatchSQL(...)`
- `MapSqlApiGetInsertSQL(...)`
- `MapSqlApiGetInsertBatchSQL(...)`
- `MapSqlApiGetSelectTableSQL(...)`
- `MapSqlApiGetSelectViewSQL(...)`
- `MapSqlApiGetUpdateSQL(...)`
- `MapSqlApiGetUpdateBatchSQL(...)`
---
## Helpful Classes (ideas welcome for more!)

- `TypeCache.Web.Attributes.RequireClaimAttribute`
- `TypeCache.Web.Attributes.RequireHeaderAttribute`
- `TypeCache.Web.Handlers.ClaimAuthorizationHandler`
- `TypeCache.Web.Handlers.HeaderAuthorizationHandler`
- `TypeCache.Web.Middleware.WebMiddleware`
- `TypeCache.Web.Requirements.ClaimAuthorizationRequirement`
- `TypeCache.Web.Requirements.HeaderAuthorizationRequirement`
