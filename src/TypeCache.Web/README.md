># TypeCache - Web
##### sam987883@gmail.com  

![image](../../TypeCash.png)

[**Source Code**](https://github.com/sam987883/TypeCache/tree/master/src/TypeCache.GraphQL)

[Request Features (or report a bug) (if any)](https://github.com/sam987883/TypeCache/issues)

---
### SQL Web API Extensions
- Configure SQL API:
  - `Microsoft.Extensions.DependencyInjection.IServiceCollection.`__`ConfigureSqlApi(...)`__
- API Endpoints:
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApi(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiCallProcedure(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiDelete(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiDeleteBatch(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiInsert(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiInsertBatch(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiSchemaGet(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiSelectTable(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiSelectView(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiUpdate(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiUpdateBatch(...)`__
- Get SQL Endpoints:
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlGetSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetDeleteSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetDeleteBatchSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetInsertSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetInsertBatchSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetSelectTableSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetSelectViewSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetUpdateSQL(...)`__
  - `Microsoft.AspNetCore.Routing.IEndpointRouteBuilder.`__`MapSqlApiGetUpdateBatchSQL(...)`__
---
### Helpful Classes (ideas welcome for more!)

- `TypeCache.Web.Attributes.`__`RequireClaimAttribute`__
- `TypeCache.Web.Attributes.`__`RequireHeaderAttribute`__
- `TypeCache.Web.Handlers.`__`ClaimAuthorizationHandler`__
- `TypeCache.Web.Handlers.`__`HeaderAuthorizationHandler`__
- `TypeCache.Web.Middleware.`__`WebMiddleware`__
- `TypeCache.Web.Requirements.`__`ClaimAuthorizationRequirement`__
- `TypeCache.Web.Requirements.`__`HeaderAuthorizationRequirement`__
