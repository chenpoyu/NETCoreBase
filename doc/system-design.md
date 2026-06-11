# SD：系統設計

這份文件接在 SA 後面，說明目前程式怎麼切、請求怎麼走、哪些設計是刻意保留的。這不是重新設計一套新框架，而是把這份底層的設計脈絡寫清楚。

## 架構總覽

專案分成四個主要組件：

```text
NETCoreBase.API
  -> NETCoreBase.Core
      -> NETCoreBase.Common
          -> NETCoreBase.Database
```

實際依賴有一點交錯：Core 會參考 Common 和 Database，Common 也會參考 Database。這是早期底層常見寫法，優點是開發快，缺點是界線沒有 Clean Architecture 那麼乾淨。

## 執行視角

本機開發時，整體執行關係大致是：

```text
Browser / API Client
  -> NETCoreBase.API container or local process
      -> SQL Server
      -> Seq
```

前端不在這個專案裡。Swagger 可以暫時當 API client，用來登入、貼 token、測授權 API。

## 分層責任

`NETCoreBase.API`

- ASP.NET Core Web API 入口。
- Controller 和路由。
- Swagger 設定。
- CORS、Serilog、JWT、權限 Policy 設定。
- Autofac module 掛載。

`NETCoreBase.Core`

- Request / Response DTO。
- MediatR Handler。
- FluentValidation Validator。
- Service 介面與實作。
- AutoMapper Profile。

`NETCoreBase.Common`

- Repository 基底。
- JWT 產生與驗證。
- 權限 handler。
- Excel formatter。
- 例外處理 filter。
- 加解密與雜湊 helper。
- 查詢分頁模型。

`NETCoreBase.Database`

- DB First Model。
- `NETCoreBaseContext`。
- `Schema.sql`。
- scaffold 操作筆記。

## Request / Handler / Service 設計

Controller 不直接操作 DbContext。它只負責接 request，然後呼叫 `_mediator.Send(req)`。

Request 會進到對應 Handler。Handler 再呼叫 Service。Service 裡處理資料查詢、資料異動、token 建立、權限資料組合等事情。

這樣做的好處是 Controller 不會很肥，也比較容易把驗證和流程拆開。缺點是對小功能來說檔案會變多，新人一開始要花一點時間對路徑。

典型查詢 API 的呼叫順序：

```text
Controller
  -> MediatR Request
      -> QueryHandler
          -> Service
              -> Repository / DbContext
                  -> SQL Server
```

典型異動 API 的呼叫順序：

```text
Controller
  -> MediatR Request
      -> CommandHandler
          -> Service
              -> Repository / DbContext
                  -> SaveChangesProcess
                  -> SQL Server
```

## DI 設計

專案使用 Autofac，不是只用 ASP.NET Core 內建 DI。

Startup 裡的 `ConfigureContainer` 會註冊：

- `ServiceModule`
- `EFModule`
- `AutoMapperModule`

MediatR 14 已改由 `ConfigureServices` 透過 `services.AddMediatR(...)` 註冊。`MediatorModule` 現在只保留為 assembly marker，讓 MediatR 和 FluentValidation 可以找到 Core 專案裡的 Handler 與 Validator。

Service 用命名慣例註冊：公開 class、非 abstract、名稱以 `Service` 結尾，就註冊成 implemented interfaces。

早期版本曾用 Autofac 手動註冊 MediatR 的 `ServiceFactory`。新版 MediatR 已不適合沿用那段註冊，所以這次升級順手把 MediatR 註冊移到 `IServiceCollection`。

## 驗證設計

輸入驗證使用 FluentValidation。Validator 放在各 Request 附近，例如 Login、Register、CreateUser。

MVC 預設 ModelState invalid filter 被關掉，讓驗證行為走專案自己的 pipeline 和例外處理方式。

## 授權設計

授權分成兩層：

1. JWT token 檢查。
2. 功能權限檢查。

Startup 目前對 Controller 全域掛上：

- `JwtAuthPolicy`
- `PermissionPolicy`

需要匿名使用的 API 要明確加 `[AllowAnonymous]`。目前 Login 和 Register 就是這樣。

JWT 設定來自 `JwtTokenConfig`。Secret、Issuer、Audience 都可以透過 appsettings 或環境變數設定。

授權失敗時，請先分辨是哪一層失敗：

- token 格式錯、過期、簽章錯，通常是 JWT 驗證問題。
- token 正確但不能使用某功能，通常是功能權限資料或 `PermissionHandler` 問題。
- Login、Register 這類匿名 API 若被擋，先看是否忘了 `[AllowAnonymous]`。

## 資料存取設計

資料存取透過 EF Core 和 GenericRepository。查詢預設使用 `AsNoTracking()`。新增、修改、刪除透過 EF Core change tracking 設定 entity state。

`DbContextExtensions.SaveChangesProcess` 會在儲存前處理建立者、建立時間、更新者、更新時間這類欄位。這讓共用欄位不用每個 Service 自己塞，但也代表資料表欄位命名要配合。

## API 設計

目前 API 以 REST 風格為主，但不是嚴格 REST。像刪除使用者仍沿用 request object 的寫法，不完全是 `DELETE /api/Users/{id}`。

這是舊底層保留下來的風格。若要對外提供正式 API，建議先整理路由規則、錯誤格式和 HTTP status code，再開始擴新業務 API。

## Log 設計

Serilog 在 Program 啟動時建立 logger。Console 和 Seq 都會寫。

原本 Seq URL 寫死 `http://localhost:5341`，這次改成讀 `SeqUrl`，讓 Docker Compose 裡可以指到 `http://seq:80`。沒設定時仍回到 localhost。

## Docker 設計

Dockerfile 採 multi-stage build：

1. SDK image restore 和 publish。
2. ASP.NET runtime image 執行 API。

Compose 啟動三個服務：

- `api`
- `db`
- `seq`

Compose 不自動套用 `Schema.sql`。這是刻意保留，避免 API 容器啟動時偷偷改資料庫。正式專案若要自動化，建議另外做 migration job 或 db-init container。

部署到正式環境時，不建議直接拿本機 compose 當正式架構。比較合理的切法是：

- API image 交給正式的 container runtime 或平台管理。
- SQL Server 使用受控資料庫或獨立資料庫主機。
- Seq 或其他 log server 獨立管理保存時間和權限。
- secrets 由部署平台注入，不放在 image 裡。

## 設定設計

公開版 appsettings 只放占位值。正式值應由下面來源覆蓋：

- Docker `.env`
- Docker Compose environment
- 部署平台 secret
- 本機未 commit 的 appsettings Local 檔

目前 `.gitignore` 已排除常見本機設定和憑證檔。

## 設計取捨

保留 Startup：

這樣比較接近原本專案，不會因為升級 .NET 10 就把啟動流程重寫掉。

保留 DB First：

這份底層原本就以既有資料庫為中心。若改成 Code First，資料層會整個變味。

保留 Autofac：

Service 和 module 註冊已經用 Autofac 寫好。短期內移除 Autofac 的價值不高，風險反而比較大。

只局部調整 Docker：

Docker 的目標是讓本機環境能起來，不是一次做到正式站台部署。正式部署仍要另外處理 image registry、secret、資料庫備份、監控和網路。
