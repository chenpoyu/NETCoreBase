# 專案筆記

這個專案是早期整理出來的 ASP.NET Core Web API 底層。它不是完整產品，比較像一套後端起手式：帳號登入、JWT、權限、角色、功能選單、Repository、Excel 匯出、Swagger 都先放進來，之後的業務 API 可以接在這個結構上。

## 分層

`NETCoreBase.API` 是入口。Controller 都放在這裡，Startup 設定 Swagger、JWT、權限 Filter、Cors、Serilog，以及 Autofac 模組。

`NETCoreBase.Core` 放應用流程。這層主要是 Request、Handler、Service、Validator。Controller 收到資料後會丟給 MediatR，再進到對應的 Handler 和 Service。

`NETCoreBase.Common` 放共用工具。像是 GenericRepository、JWT 管理、權限 Handler、例外 Filter、Excel Formatter、AES/SHA1 helper 都在這裡。

`NETCoreBase.Database` 是 DB First 產生的資料層。Model 和 `NETCoreBaseContext` 都是從 SQL Server schema 來的，所以這層看起來會比較像資料表的原貌。

## 請求流程

一般 API 會先進 Controller，再由 Controller 呼叫 `_mediator.Send(req)`。Request 會對到 Core 裡面的 QueryHandler 或 CommandHandler。Handler 通常不寫太多邏輯，真正的資料操作會在 Service 或 Repository。

登入和註冊例外，因為這兩支 API 有 `[AllowAnonymous]`，不需要先帶 JWT。登入成功後會回傳 token，後續 API 走 JWT 與權限檢查。

## 權限

這套權限分成兩段：

1. JWT 是否有效。
2. 使用者是否有功能權限。

Startup 裡面有全域掛上 `JwtAuthPolicy` 和 `PermissionPolicy`。如果某支 API 不需要登入，要像 Login、Register 一樣明確加 `[AllowAnonymous]`。

## 目前保留的老味道

這次把底層升到 .NET 10，沒有把專案改成 Minimal API，也沒有重寫成新版 Program.cs 寫法。原因很簡單：這個專案原本就是 Startup + Autofac 模組的結構，硬改會讓舊專案的脈絡不見。
