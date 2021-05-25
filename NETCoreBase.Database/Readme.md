## Pull Docker image
docker pull mcr.microsoft.com/mssql/server:2019-latest
docker pull datalust/seq:latest

## Docker run
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<Password>" \
   -p 1433:1433 --name mssql -h mssql \
   -d mcr.microsoft.com/mssql/server:2019-latest
docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest

## Copy File to Docker image
docker cp Schema.sql mssql:/home/

## Excute SQL File
docker exec -it mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P <Password> -i Schema.sql

## DBFirst
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=NETCoreBase;Trusted_Connection=False;User=SA;Password=<Password>" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
