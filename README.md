# UserTaskAssement
User Task Assesment

## To get it running on your machine
* clone repo down to your machine
* for the purpose of this assesment, appsettings are detailed below, which is NOT standard practice
* add appsettings.Development.json to the api project with the following settings:

```
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AppSettings": {
    "Environment": "Development",
    "BuildNumber": "0.0.0"
  },
  "ConnectionStrings": {
    "dbconnectionstring": "Data Source=Data/Database/DataBalkTask.db"
  },
  "Jwt": {
    "Key": "baa689d3-0bd2-4eaf-8e53-7d990a6cd48d",
    "Issuer": "https://DataBalk.Task.Api",
    "Audience": "DataBalk.Task.Api"
  }
}
```
* Ensure the above configured connection string points to a database containing the following schema objects:
  * [Task table]()
  * [User table]()
* You can drop and recreate the tables to start fresh, using the scripts above
* From inside the `api` folder:
  * run `dotnet build` to restore nuget packages and build api project.
  * run `dotnet run` to start api project.
  * open https://localhost:7124/swagger/index.html in a browser to view swagger documentation.