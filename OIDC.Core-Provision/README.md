# OIDC.Core Provisioning Tool

This project contains a standard C# console app that can be used to provision a new project with the following;

- Initial superuser
- Initial first party application
- A set of basic roles
- A set of basic scopes

Please note - the dotnet tool references OIDC.Core-API which must be present. You will also need to configure the 
default connection string in the API's `appsettings.Development.json` before provisioning.

From the root directory (with the individual API, APITest and Provision directories);
```
cd OIDC.Core-Provision
dotnet run <admin-username> <initial-app-name> <absolute-path-to-API-dir>
```