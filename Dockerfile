FROM mcr.microsoft.com/dotnet/sdk:10.0.301 AS build
WORKDIR /src

COPY NETCoreBase.sln ./
COPY NETCoreBase.API/NETCoreBase.API.csproj NETCoreBase.API/
COPY NETCoreBase.Common/NETCoreBase.Common.csproj NETCoreBase.Common/
COPY NETCoreBase.Core/NETCoreBase.Core.csproj NETCoreBase.Core/
COPY NETCoreBase.Database/NETCoreBase.Database.csproj NETCoreBase.Database/

RUN dotnet restore NETCoreBase.API/NETCoreBase.API.csproj

COPY . .
RUN dotnet publish NETCoreBase.API/NETCoreBase.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0.9 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "NETCoreBase.API.dll"]
