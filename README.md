in the package manager run the following commands to set up your SQLite database 

dotnet ef migrations add InitialCreate  --startup-project ./HappyCompanyProject/HappyCompanyProject.csproj

dotnet ef database update --startup-project ./HappyCompanyProject/HappyCompanyProject.csproj
