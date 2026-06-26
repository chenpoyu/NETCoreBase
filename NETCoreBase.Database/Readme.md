## 拉取 Docker 映像

```bash
docker pull mcr.microsoft.com/mssql/server:2022-latest
docker pull datalust/seq:latest
```

## 啟動容器

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<密碼>" \
   -p 1433:1433 --name mssql -h mssql \
   -d mcr.microsoft.com/mssql/server:2022-latest

docker run --name seq -d --restart unless-stopped -e ACCEPT_EULA=Y -p 5341:80 datalust/seq:latest
```

## 複製 Schema 腳本到容器

```bash
docker cp Schema.sql mssql:/home/
```

## 執行 Schema 腳本

```bash
docker exec -it mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U SA -P <密碼> -i /home/Schema.sql -C
```

## DB First 重新 Scaffold

```bash
dotnet ef dbcontext scaffold "Server=localhost,1433;Database=NETCoreBase;Trusted_Connection=False;User=SA;Password=<密碼>" Microsoft.EntityFrameworkCore.SqlServer -o Models --force
```
