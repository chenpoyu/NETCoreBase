# NETCoreBase

NETCoreBase 是一個以 ASP.NET Core 5 建立的 Web API 範例專案，保留當年開發時的分層架構與套件版本，作為 .NET Core 後端基底專案參考。

## 專案內容

- `NETCoreBase.API`：Web API 入口、Swagger、JWT 驗證與 Controller。
- `NETCoreBase.Core`：應用服務、MediatR Request/Handler、FluentValidation 驗證與 AutoMapper Profile。
- `NETCoreBase.Common`：共用模組、Repository、JWT、權限、Excel、例外處理與工具類別。
- `NETCoreBase.Database`：Entity Framework Core DB First 產生的 Model、DbContext 與 SQL Server schema。

## 主要技術

- .NET 5 / ASP.NET Core Web API
- Entity Framework Core 5 / SQL Server
- Autofac
- MediatR
- FluentValidation
- AutoMapper
- JWT Bearer Authentication
- Swagger / Swashbuckle
- Serilog / Seq
- EPPlus

## 功能範圍

- 使用者註冊、登入、查詢、建立、修改與刪除
- 角色查詢、建立、修改與刪除
- 功能選單與權限資料維護
- JWT 驗證與自訂授權 Policy
- Excel 輸出 Formatter
- SQL Server schema 建立腳本

## 專案設定

公開版本中的 `NETCoreBase.API/appsettings.json` 已將連線密碼、AES Key/IV、PushDecode 與 JWT Secret 改為占位值。若要在本機執行，請在本機環境改成自己的設定，不要把真實密碼或金鑰 commit 進 repository。

需要調整的欄位包含：

- `ConnectionStrings:DefaultConnection`
- `AesIV`
- `AesKey`
- `PushDecode`
- `JwtTokenConfig:Secret`
- `MailConfig:UserName`
- `MailConfig:UserPass`

## 資料庫

資料庫腳本位於 `NETCoreBase.Database/Schema.sql`。當年的資料庫啟動方式可參考 `NETCoreBase.Database/Readme.md`，包含 SQL Server Docker、Seq Docker 與 DB First scaffold 指令。

範例：

```bash
docker pull mcr.microsoft.com/mssql/server:2019-latest
docker pull datalust/seq:latest
```

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=<YourStrongPassword>" \
  -p 1433:1433 --name mssql -h mssql \
  -d mcr.microsoft.com/mssql/server:2019-latest
```

```bash
docker cp NETCoreBase.Database/Schema.sql mssql:/home/
docker exec -it mssql /opt/mssql-tools/bin/sqlcmd -S localhost -U SA -P <YourStrongPassword> -i /home/Schema.sql
```

## API 文件

開發環境啟動後可透過 Swagger 查看 API：

```text
/swagger
```

Swagger 說明中保留當時的使用方式：先透過 Login API 取得 token，再將 token 貼到 Authorize 進行授權。

## 公開注意事項

此 repository 已移除目前檔案中的實值密碼與金鑰，並在 `.gitignore` 加入常見本機設定與憑證檔案規則。若要將既有 repository 直接公開，仍建議先確認 Git 歷史紀錄是否曾包含真實密碼或金鑰；如果曾經 commit 過真實秘密，應先輪替該秘密並清理 Git history。

## 備註

這是一個保留當年實作風格的專案，未進行框架升級、套件升級或架構重構。
