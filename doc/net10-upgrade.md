# .NET 10 升級筆記

這次升級以 2026-06-11 為時間點。專案從 `net8.0` 升到 `net10.0`，SDK 固定在 `10.0.301`，Docker runtime 固定在 `10.0.9`。

## 動到的地方

四個 project 都改成 `net10.0`：

- `NETCoreBase.API`
- `NETCoreBase.Core`
- `NETCoreBase.Common`
- `NETCoreBase.Database`

Microsoft.AspNetCore、Microsoft.Extensions 和 Entity Framework Core 相關套件升到目前對應的 .NET 10 穩定版本。第三方套件也同步更新到目前 NuGet 上的穩定版。

`Microsoft.EntityFrameworkCore.SqlServer` 依照目前 NuGet 穩定版停在 10.0.3；其他 EF Core 基礎、Design、Tools 套件升到 10.0.9。

## MediatR

MediatR 從 11.x 升到 14.x。這不是單純改版本號，因為舊版手動註冊 `ServiceFactory` 的方式已經不適合沿用。

目前做法是：

- `Startup.ConfigureServices` 使用 `services.AddMediatR(...)` 註冊 handler。
- `ValidatorBehavior<,>` 透過 `AddOpenBehavior` 加進 pipeline。
- `MediatorModule` 保留為 assembly marker，不再繼承 Autofac Module。

## AutoMapper

AutoMapper 升到 16.x。`MapperConfiguration` 現在建立時補上 `NullLoggerFactory.Instance`，避免沿用舊 constructor 寫法。

這套底層仍然維持原本的 AutoMapper module 設計，沒有改成 `services.AddAutoMapper(...)`。

AutoMapper 16.x 和 MediatR 14.x 都有 license key 的設定方式。這份公開底層沒有內建任何 license key；如果後續要正式使用，要由使用單位自行評估授權和設定方式。

## Docker

Dockerfile 改用：

- `mcr.microsoft.com/dotnet/sdk:10.0.301`
- `mcr.microsoft.com/dotnet/aspnet:10.0.9`

docker compose 的 API image 名稱改成 `netcorebase-api:net10`。

## 沒有改的地方

沒有改成 Minimal API。

沒有移除 Startup。

沒有把 DB First 改成 migration。

沒有重寫 Repository。

沒有重做權限模型。

這次升級的重點是把底層推到 .NET 10，並處理必要的套件相容性。架構整理仍然建議另外排時間做，不要和版本升級混在同一批。
