# .NET 8 升級筆記

這份文件是前一次升級到 .NET 8 的歷史紀錄。專案目前已再升級到 .NET 10，現況請看 `net10-upgrade.md`。

這次升級以 2024/02 為時間點。當時可用的 .NET 8 發行版是 .NET 8.0.2，SDK 是 8.0.200。

## 動到的地方

四個 project 都從 `net5.0` 改成 `net8.0`：

- `NETCoreBase.API`
- `NETCoreBase.Core`
- `NETCoreBase.Common`
- `NETCoreBase.Database`

Microsoft.AspNetCore 和 Entity Framework Core 相關套件升到 `8.0.2`。其他套件以當時可用、且不大改原架構為原則調整。

## 沒有改的地方

沒有把 Startup 拆掉，也沒有改成 Minimal API。

沒有重做 Repository。

沒有把 DB First 改成 Code First。

沒有重寫權限模型。

這些都不是不能改，而是改了之後就不像原本那套底層了。這次主要是讓它站到 .NET 8，其他地方保持原本的骨架。

## MediatR

MediatR 沒有升到 12.x。原因是專案目前用 Autofac 手動註冊 `ServiceFactory`，12.x 之後這段會牽涉 breaking change。為了不要在這次升級裡順手重寫 DI 流程，先停在 11.x。

## Serilog / Seq

原本 Seq URL 寫死在 `Program.cs`。為了讓 Docker Compose 裡的 API 可以打到 `seq` 服務，現在改成讀 `SeqUrl`，沒設定時仍然回到 `http://localhost:5341`。
