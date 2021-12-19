# .NET Core WebApi with authentication, DDD and CQRS

### Current framework version: .NET 5 

--------------

# Features
### Optimized data schemas
* [EF core](https://github.com/dotnet/efcore) - 
- [DataContext]() for writes, DDD support (Value Objects, domain model rules validation, persistence ignorance)
- [IReadOnlyDatabaseContext]() for reads, only read collections exposed

### Separation of concerns
* Segregating the read and write sides mean maintainable and flexible models. Most of the complex business logic goes into the write model. The read model can be relatively simple.

### Security
* It's easier to ensure that only the right domain entities are performing writes on the data.

### Independent scaling 
* Easly to split into WRITE and READ databases

--------------

## API Documentation
- [Swagger](https://appcore.azurewebsites.net/swagger/index.html)
