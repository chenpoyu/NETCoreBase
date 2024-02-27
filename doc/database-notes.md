# 資料庫筆記

資料庫用 SQL Server。Schema 腳本放在 `NETCoreBase.Database/Schema.sql`，裡面建立的表主要有使用者、角色、功能、角色功能、使用者角色、登入紀錄和 Banner。

## DB First

這個專案是 DB First。`NETCoreBase.Database/Models` 底下的 Model 和 `NETCoreBaseContext` 是 scaffold 出來的，不是手寫 Entity 設計。改資料表時，原本做法會是更新 SQL，再重新 scaffold。

`NETCoreBase.Database/Readme.md` 保留當時的操作筆記，包括 Docker 跑 SQL Server、把 Schema.sql 複製進容器，以及 `dotnet ef dbcontext scaffold`。

## 連線字串

公開版不放真實密碼。程式裡看到的 `CHANGE_ME...` 都只是占位字。本機開發請用自己的設定覆蓋：

- `ConnectionStrings__DefaultConnection`
- `MSSQL_SA_PASSWORD`

Docker Compose 已經把 API 連到 `db` 服務。若不用 Docker，連線字串就改成自己的 SQL Server 位置。

## 初始化資料庫

Compose 只負責把 SQL Server 跑起來，不會自動套 Schema。資料庫容器起來後，可以照舊把 `Schema.sql` 放進容器再執行。

範例：

```bash
docker cp NETCoreBase.Database/Schema.sql netcorebase-db-1:/home/Schema.sql
docker exec -it netcorebase-db-1 /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P "$MSSQL_SA_PASSWORD" -i /home/Schema.sql
```

不同版本 SQL Server image 裡的 `sqlcmd` 位置可能不同，如果找不到，先進容器確認 `/opt` 底下的工具路徑。
