# NETCoreBase

NETCoreBase 是一個 ASP.NET Core Web API 底層專案。這版已依照 2024/02 的時間點升到 .NET 8，主要目標是保留原本的 Startup、Autofac、MediatR、DB First 寫法，同時讓專案可以用 .NET 8 的工具鏈建置。

## 專案內容

- `NETCoreBase.API`：Web API 入口、Swagger、JWT 驗證與 Controller。
- `NETCoreBase.Core`：Request/Handler、Service、FluentValidation、AutoMapper Profile。
- `NETCoreBase.Common`：Repository、JWT、權限、Excel、例外處理與共用工具。
- `NETCoreBase.Database`：Entity Framework Core DB First 產生的 Model、DbContext 與 SQL Server schema。
- `doc`：專案交接文件，寫法偏工程筆記，不把舊專案重新包裝。

## 主要技術

- .NET 8.0.2 / SDK 8.0.200
- ASP.NET Core Web API
- Entity Framework Core 8 / SQL Server
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

公開版本中的 `NETCoreBase.API/appsettings.json` 已將連線密碼、AES Key/IV、PushDecode 與 JWT Secret 改為占位值。若要在本機執行，請用環境變數或自己的本機設定覆蓋，不要把真實密碼或金鑰 commit 進 repository。

常見要調整的欄位：

- `ConnectionStrings:DefaultConnection`
- `AesIV`
- `AesKey`
- `PushDecode`
- `JwtTokenConfig:Secret`
- `MailConfig:UserName`
- `MailConfig:UserPass`
- `SeqUrl`

## Docker

先複製 `.env.example` 成 `.env`，再改成本機值。

```bash
docker compose up --build
```

服務預設位置：

- API：`http://localhost:8080`
- Seq：`http://localhost:5341`
- SQL Server：`localhost,1433`

Compose 只會把 SQL Server 跑起來，不會自動建立資料表。資料庫腳本仍放在 `NETCoreBase.Database/Schema.sql`，可以照 `doc/database-notes.md` 的方式手動套用。

## 文件

程式文件放在 `doc`：

- `doc/system-analysis.md`
- `doc/system-design.md`
- `doc/base-framework-plan.md`
- `doc/roadmap.md`
- `doc/operation-and-security.md`
- `doc/project-notes.md`
- `doc/api-notes.md`
- `doc/database-notes.md`
- `doc/docker-notes.md`
- `doc/net8-upgrade.md`

## API 文件

開發環境啟動後可透過 Swagger 查看 API：

```text
/swagger
```

Swagger 的使用方式維持原本做法：先透過 Login API 取得 token，再把 token 貼到 Authorize。

## 公開注意事項

目前檔案中的實值密碼與金鑰已改成占位值，也加入了常見本機設定與憑證檔案的 ignore 規則。若要把既有 repository 直接改成 public，仍要先處理 Git 歷史；只改現在的檔案，不會讓舊 commit 裡曾出現過的密碼消失。
