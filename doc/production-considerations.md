# 正式環境注意事項

這個 repository 是底層基礎專案。以下項目應在正式環境上線前審查並處理完畢。

---

## 安全性

### 密鑰

- 替換 `appsettings.json` 中所有 `CHANGE_ME_...` 佔位值
- 不得將真實密鑰 commit 到 repository — 請使用環境變數、Secret Manager 或部署平台的密鑰注入機制
- 曾以明文 commit 到 git 歷史的值必須全部換掉

### JWT

- 使用足夠長且隨機的 JWT 簽名 Secret（最少 32 個字元，建議 64 個以上）
- 審查 `AccessTokenExpiration` — 720 分鐘對高安全性系統而言過長，請依風險容忍度調整
- 確認 issuer 和 audience 的值與實際部署 URL 一致
- 加入啟動驗證，若 Secret 是已知佔位值則拒絕應用程式啟動

### 密碼雜湊

- 目前 `CryptHelper` 使用 SHA1 進行密碼雜湊
- 無加鹽的 SHA1 不適合正式環境使用
- 替換為 ASP.NET Core Identity `PasswordHasher` 或其他現代方案（bcrypt、Argon2）
- 更換雜湊方案前，需規劃現有使用者密碼的遷移策略

### CORS

- 目前政策使用 `SetIsOriginAllowed(origin => true)` 搭配 `AllowCredentials()`，允許所有來源
- 正式環境替換為明確的來源允許清單
- 審查是否真的需要 `AllowCredentials()`

### Swagger

- Swagger 目前只在 `ASPNETCORE_ENVIRONMENT == Development` 時啟用
- 確認正式部署的 `ASPNETCORE_ENVIRONMENT` 不是 `Development`
- 若正式環境必須提供 API 文件，請加入認證保護

### 錯誤回應

- `HttpResponseExceptionFilter` 在 API 邊界處理例外
- 確認正式環境的錯誤回應不會洩漏 Stack Trace、內部例外訊息或資料庫細節
- 考慮回傳標準化的 `ProblemDetails` 格式

### AES 加密

- `AesIV` 和 `AesKey` 供 `CryptHelper` 使用；任何真實資料加密前必須替換佔位值
- 加入啟動驗證，若偵測到已知佔位值則拒絕啟動

---

## 資料庫

- 不要對正式環境執行 `Schema.sql`，此腳本會 DROP 並重建整個資料庫
- 正式環境的 Schema 變更需有明確流程：審查腳本、在 Staging 測試、於排定的維護窗口或使用線上遷移方式套用
- 使用擁有最小必要權限的資料庫帳號 — 正式環境避免使用 `SA`
- 依預期負載設定適當的連線池（Connection Pool）參數
- 審查 Service 中的交易邊界，確保需要原子性的操作使用明確的 Transaction
- 正式上線前定義並測試備份與還原策略

---

## 可觀測性

- 依正式環境需求設定 Serilog 最低日誌層級（正式環境避免使用 `Debug` 或 `Verbose`）
- 在 Seq 或正式日誌儲存設定日誌保留政策
- 加入請求 Correlation ID 以支援分散式追蹤
- 加入健康檢查端點，考慮以 `app.MapHealthChecks("/health")` 提供就緒探測
- 針對錯誤率、延遲尖峰和認證失敗次數定義告警規則

---

## 部署

- 不要將本地 Docker Compose 架構作為正式環境架構使用
- 開發、Staging、正式環境各自獨立設定
- 建立不可變的工件（Artifact）— 同一個映像應先部署到 Staging，確認後再晉升到正式
- 發布前審查 Dockerfile 基礎映像是否有已知漏洞
- 定義部署失敗時的回滾策略
- 使用正式等級的容器調度平台（Kubernetes、Azure Container Apps 等），而非 Compose

---

## 可維護性

- 隨系統演進持續更新 `doc/handover-checklist.md`
- 以整個團隊都能理解的格式記錄授權規則 — 哪些角色可以存取哪些功能
- 保持 API 合約在版本間穩定；如果無法避免破壞性變更，加入 API 版本控制
- 避免將業務規則寫進 Controller — 保留在 Service 或 Command Handler
- 在重大變更前，為關鍵流程（登入、Token 驗證、權限執行）加入整合測試
