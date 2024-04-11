# 底層規劃說明

這個底層的目標不是做一個很炫的框架，而是整理一套常見後台 API 會反覆需要的基礎能力。好的底層應該讓開發者少寫重複程式，但不要讓人為了加一支 API 還要先理解一堆抽象。

## 核心目標

第一個目標是「可以快速開新功能」。新增一個管理功能時，理想流程是新增 Request、Validator、Handler、Service 方法，必要時補資料表和 Model，不需要重寫登入、授權、分頁、例外處理。

第二個目標是「權限可以集中管理」。功能選單、角色、權限不要散在程式各處。程式可以保留 attribute 或 policy，但實際使用者能做什麼，應該能從資料表組出來。

第三個目標是「本機環境容易重建」。Docker Compose 提供 SQL Server 和 Seq，讓新接手的人不用先猜資料庫和 log server 怎麼準備。

## 底層能力

目前已經有的能力：

- JWT 登入和驗證。
- 使用者、角色、功能的基本 CRUD。
- 功能選單資料結構。
- 權限 policy 和 handler。
- GenericRepository。
- AutoMapper profile。
- FluentValidation。
- Excel 輸出。
- Swagger。
- Serilog 和 Seq。
- Dockerfile 和 docker compose。

## 建議補強的能力

初始化資料：

目前 schema 沒有 seed data。後續應補一份可重複執行的初始化腳本，至少建立管理者、管理角色、基本功能選單和角色功能關聯。

統一回應格式：

現在大多直接 `Ok(...)` 或 `NoContent()`。如果要做正式底層，可以定義成功、失敗、驗證錯誤、系統錯誤的固定格式，前端會比較好接。

錯誤代碼：

現在例外處理偏簡單。後續可以補錯誤代碼，例如帳密錯誤、權限不足、資料不存在、資料重複，避免前端只靠文字判斷。

查詢規格：

目前已有 `QueryOption` 和分頁模型。可以再把排序、關鍵字查詢、日期區間和狀態篩選整理成一致規格。

審計欄位：

CreateUser、CreateDate、UpdateUser、UpdateDate 已有處理痕跡。後續可以把軟刪除、狀態碼、異動紀錄再標準化。

安全：

密碼雜湊應從 SHA1 換成 ASP.NET Core Identity PasswordHasher 或其他合適方案。這件事若要做，要搭配既有密碼資料的轉換策略。

測試：

目前沒有測試專案。底層若要長期使用，至少要補 Service 層單元測試、授權 handler 測試、Repository 查詢測試和 API smoke test。

## 開發規範建議

新增功能時，檔案位置照既有規則：

- Request / Response 放在 `NETCoreBase.Core/Commands/{Domain}/{Action}`。
- Validator 和 Handler 放在同一個 action 資料夾。
- Service interface 放在 `NETCoreBase.Core/Interfaces`。
- Service implementation 放在 `NETCoreBase.Core/Services`。
- Controller 放在 `NETCoreBase.API/Controllers`。

命名保持直覺。查詢用 `QueryHandler`，異動用 `CommandHandler`。如果某個功能沒有複雜流程，不要硬切太多層；但資料存取和授權邏輯不要塞進 Controller。

## 設定規劃

公開 repository 只保留安全占位值。

本機用 `.env` 或未 commit 的 local appsettings。

Docker Compose 用環境變數覆蓋 appsettings。

正式環境用部署平台 secret 或 secret manager，不把值放進 image。

## 資料庫規劃

短期仍沿用 DB First。原因是這份專案的 Model 已經跟 schema 綁在一起，現在改 Code First 會多一層風險。

中期可以整理 scaffold 流程，把 DB First 指令、輸出目錄、覆蓋規則和注意事項寫清楚。

長期如果要變成新系統底層，可以評估是否改成 migration 管理。但這要看團隊習慣，不是一定要改。

## 部署規劃

目前 Dockerfile 可產出 API image。Compose 適合本機，不等於正式部署。

正式部署還需要補：

- image tag 規則。
- registry。
- 環境變數和 secrets 管理。
- SQL Server 備份。
- health check。
- log retention。
- rollback 方式。

這些可以獨立補，不必塞進應用程式碼。
