# Projectlabor Backend - RakLap 
This is the backend service for our application, RakLap, a system designed to manage, monitor, and optimize warehouse operations.  
It provides APIs for handling inventory, shipments, products, and warehouse data.

## Used technologies
- ASP .NET 9
- MSSQL
- Smtp4Dev

## Used Nuget Packages
- **Automapper** for entity to dto mapping
- **EPPlus** for excel import/export
- **FluentEmail** for email sending
- **Argon2** for password encryption
- **EntityFrameworkCore** for database conncetion
- **SwashBuckle** for swaggeer

## Running the server
1. Install **Smtp4Dev** in the Commandline
```
dotnet tool install -g Rnwood.Smtp4dev
```
2. Run **Update-Database** in the Nuget package manager console
```
Update-Database
```
3. Run smtp4dev in the Commandline
```
smtp4dev
```
4. Start the application

### Server should run on https://localhost:7116/swagger/index.html
### Smtp4Dev can be viewed on https://localhost:5000
