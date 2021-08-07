# TrueLayerChallenge


## Getting Started

Clone the repository.
```shell
git clone https://github.com/gatolim/TrueLayerChallenge.git
```

### Web API

1. Install latest [.NET core](https://www.microsoft.com/net/core). 
If latest is incompatible with the current version of .net core for this project (2.0)
download .NET core SDK 2.0 from [release archives](https://github.com/dotnet/core/blob/master/release-notes/download-archives/2.0.0-download.md).

2. Restore and run the web api
```shell
cd TrueLayerChallenge.WebApi
dotnet restore
dotnet run
```

### Swagger

In order to acces the api swagger documentation, first setup and run the api locally then navigate to swagger UI at http://localhost:5000/swagger/.



## Production Consideration
The following would be consider when it comes to productionising the existing codebase.

1. Implementing retry policy when dealing with external APIs
2. Use a proper database, rather then an inmemory store.
3. Errors handling and loggings
  a. Apply more granular errors handling and loggings, especially around the external API to cover more edge cases. 
  b. Implement logic to handle any custom exceptions within the API's Global Error Handler, on top of the existing logic which only handled "Unhandled" exceptions.
  c. Consider implementing with a logging framework (eg serilog), for better logs management and troubleshooting experience.
6. Adopt Correlationmanager and Correlation ID for exceptions and logs, for troubleshooting pupose.
7. Apply authentication if needed.
