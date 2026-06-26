# 交接確認清單

這份清單給新接手這份程式碼的維護者使用。請從上到下逐項確認，再開始對系統進行任何修改。

---

## 執行環境

- [ ] 確認 .NET SDK 版本 — 查看 `global.json`（目前固定在 10.0.301）
- [ ] 確認應用程式進入點 — `NETCoreBase.API/Program.cs` 啟動 Serilog 和 Host；`Startup.cs` 負責服務註冊和 Middleware Pipeline
- [ ] 確認環境設定檔 — `appsettings.json`（佔位值）、`appsettings.Development.json`（本地覆蓋，不 commit）
- [ ] 確認本地已有 `.env` 檔案，且依照 `.env.example` 填入正確的值
- [ ] 確認本地啟動方式 — 完整 Stack 用 `docker compose up --build`；僅 API 用 `dotnet run --project NETCoreBase.API`

---

## 資料庫

- [ ] 確認資料庫引擎 — SQL Server 2022（Docker 映像 `mcr.microsoft.com/mssql/server:2022-latest`）
- [ ] 確認連線字串來源 — `appsettings.json` 的 `ConnectionStrings:DefaultConnection`，或 Docker 環境變數
- [ ] 確認 Schema 腳本位置 — `NETCoreBase.Database/Schema.sql`（執行前會 DROP 並重建資料庫，不可對正式環境使用）
- [ ] 確認 DB First Scaffold 指令 — 詳見 `doc/database-notes.md`
- [ ] 確認 API 可用前是否需要種子資料或基礎資料
- [ ] 確認 Schema 變更流程 — 目前無自動 Migration；Schema 變更需手動更新腳本並重新 Scaffold

---

## 認證

- [ ] 確認 JWT Issuer — 設定在 `JwtTokenConfig:Issuer`
- [ ] 確認 JWT Audience — 設定在 `JwtTokenConfig:Audience`
- [ ] 確認 JWT 簽名金鑰來源 — `JwtTokenConfig:Secret` 透過環境變數注入；正式環境必須使用夠長的隨機字串
- [ ] 確認 Access Token 過期時間 — `JwtTokenConfig:AccessTokenExpiration`（預設 720 分鐘）
- [ ] 確認哪些端點是公開的 — 目前 `AccountController` 的 Login、Register、RefreshToken 使用 `[AllowAnonymous]`；其他所有 Controller 都需要 JWT
- [ ] 確認授權模型 — `PermissionPolicy` 全域套用；`PermissionHandler` 根據使用者角色評估功能存取權

---

## 維運

- [ ] 確認日誌輸出目的地 — Serilog 輸出至 Console 和 Seq；Docker Compose 中 Seq 在 http://localhost:5341
- [ ] 確認 Seq 設定 — 透過 Docker Compose 啟動（`datalust/seq:2024` 映像）；資料持久化在 `seq-data` Volume
- [ ] 確認 Docker Compose 服務 — `api`、`db`、`seq`
- [ ] 確認 API 健康狀態 — `docker compose up` 後驗證 http://localhost:8080/swagger 可正常載入
- [ ] 確認 Swagger 授權流程 — 呼叫 Login 取得 Token，複製後貼入 Swagger Authorize 對話框

---

## 安全性

- [ ] 確認沒有正式密鑰被 commit — 檢查 `appsettings.json` 是否有 `CHANGE_ME_` 佔位值；確認 `.gitignore` 排除了 `.env` 和本地設定檔
- [ ] 確認 CORS 政策 — 目前使用 `SetIsOriginAllowed(origin => true)` 搭配 `AllowCredentials()`，允許所有來源；正式環境前必須改為明確的允許清單
- [ ] 確認 HTTPS 設定 — 已啟用 `UseHttpsRedirection()`；本地 Docker Compose 僅使用 HTTP
- [ ] 確認 Swagger 可見性 — Swagger 只在 `Development` 環境啟用；確認正式環境的 `ASPNETCORE_ENVIRONMENT` 不是 `Development`
- [ ] 確認密碼雜湊方式 — 目前透過 `CryptHelper` 使用 SHA1；這是已知限制，正式環境前應替換（詳見 `doc/production-considerations.md`）
- [ ] 確認 Token 過期設定 — 依風險容忍度審查正式環境的 `AccessTokenExpiration` 值
- [ ] 確認錯誤回應格式 — `HttpResponseExceptionFilter` 處理例外；確認正式環境的錯誤回應不會洩漏 Stack Trace、內部例外訊息或資料庫細節

---

## 人員

- [ ] 確認主要維護者與聯絡資訊
- [ ] 確認部署負責人（誰管理容器執行環境或主機環境）
- [ ] 確認資料庫負責人（誰管理 Schema 變更與備份）
- [ ] 確認認證失敗或資料存取問題的緊急聯絡人
- [ ] 確認哪些團隊或系統使用這個 API
