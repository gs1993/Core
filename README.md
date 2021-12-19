# .NET Core WebApi with authentication, DDD and CQRS

### Current framework version: .NET 5 

--------------

# Features
### Optimized data schemas
* [DataContext](https://github.com/gs1993/Core/blob/master/src/domain/Domain/Shared/DatabaseContext/DataContext.cs) for writes, DDD support (Value Objects, domain model rules validation, persistence ignorance)
* [IReadOnlyDatabaseContext](https://github.com/gs1993/Core/blob/master/src/domain/Domain/Shared/DatabaseContext/ReadonlyDataContext.cs) for reads, only read collections exposed

### CQRS
* Segregating the read and write sides mean maintainable and flexible models. Most of the complex business logic goes into the write model. The read model can be relatively simple.

### Security
* It's easier to ensure that only the right domain entities are performing writes on the data.

### Independent scaling 
* Easly to split into WRITE and READ databases

### External services integration
* [Payment gateway service](https://github.com/gs1993/Core/blob/master/src/serviceAccess/PayuGateway/PayuGatewayPaymentService.cs)
* [Currency conversion service]()

--------------

## API Documentation
- [Swagger](https://appcore.azurewebsites.net/swagger/index.html)
