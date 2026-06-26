# 架構決策紀錄

這份文件記錄這個 repository 的主要架構決策及其原因。目的是讓新接手的維護者了解為何程式碼這樣組織，而不是把這些傳統友善的選擇當成偶然。

---

## ADR 001：保留 DB First 相容性

### 背景

企業系統通常依賴既有的資料庫 Schema。Schema 可能與其他系統共用、由 DBA 團隊管理，或受報表工具和舊系統的限制。在這種情況下強制改用 Code First Migration，會引入升級風險並與既有的管理流程衝突。

### 決策

這套底層保留 DB First 相容性。`NETCoreBase.Database/Models/` 中的 EF Core 實體類別由 `dotnet ef dbcontext scaffold` 指令從資料庫產生。Schema 才是唯一的真相來源，而非實體類別本身。

### 後果

- 更容易與既有企業資料庫整合
- 降低現代化改造時的遷移風險
- 需要紀律確保產生的實體模型與應用程式邏輯分開
- 業務規則不應寫進產生的模型類別
- Schema 變更後需重新執行 scaffold 指令並審閱產出結果
- 對全新的專案或以 Schema 為主的開發，不如 Code First 方便

---

## ADR 002：保留 Startup-style 設定方式

### 背景

.NET 6 以後的 Minimal Hosting 對全新專案更簡潔。然而許多企業 ASP.NET Core 專案仍沿用 `Startup` 類別的寫法，維護這些系統的團隊也熟悉這個模式。在版本升級時額外改成 Minimal Hosting 只會增加異動範圍，沒有實質功能效益。

### 決策

這套底層保留 `Startup.cs`，明確分為 `ConfigureServices` 和 `Configure` 兩個方法。`Program.cs` 只做最基本的事：啟動 Serilog 並交給 `Startup` 類別處理。

### 後果

- 對維護既有企業 ASP.NET Core 系統的團隊而言很熟悉
- 方便從 .NET Core 3.x 或 .NET 5/6 Startup-style 專案轉換過來的開發者
- .NET 版本升級時的風險較低，因為啟動結構不需要改
- 比較新的範本更簡潔；從頭開始的新成員可能會覺得冗長

---

## ADR 003：使用 Autofac 作為 DI 容器

### 背景

ASP.NET Core 內建的 DI 容器對大多數現代專案已足夠。Autofac 在這份程式碼早期版本就已引入，主要用於模組化組織和慣例式註冊。重新改成內建容器需要改寫所有模組註冊，僅憑功能效益並不值得。

### 決策

這套底層保留 Autofac。DI 模組分為 `ServiceModule`、`EFModule` 和 `AutoMapperModule`。服務依慣例自動註冊：名稱以 `Service` 結尾的公開非抽象類別，會自動對應到其實作的介面。

### 後果

- 與既有程式碼和企業開發模式一致
- 模組層級的註冊清晰且容易稽核
- 不熟悉 Autofac 的開發者需要短期適應
- .NET 版本升級時必須一起維護 Autofac 相依性
- 是否還需要 Autofac 的評估已列入長期 roadmap

---

## ADR 004：使用 JWT Bearer 認證

### 背景

內部系統和 API 通常需要整合 SSO、IAM 或 Token 發行者。Token-based 的方式讓 API 保持無狀態，並可與身份驗證層分離。

### 決策

這套底層使用 JWT Bearer Token 作為主要 API 認證機制。Token 由 `JwtAuthManager` 在登入成功後自行發行。JWT 設定（issuer、audience、secret、過期時間）從 `JwtTokenConfig` 讀取，可透過環境變數覆蓋。

### 後果

- API 完全無狀態，伺服器不保留 Session
- 只要正確設定 issuer 和簽名金鑰，可與常見的 Identity Provider 搭配
- 目前實作是自行發行 Token 而非委外給外部 Identity Provider，適合自成體系的系統，但限制了 SSO 整合的彈性
- JWT Secret 必須妥善管理；在正式環境使用弱密鑰或佔位值是嚴重的安全風險

---

## ADR 005：使用 MediatR 進行請求分派

### 背景

Controller 直接呼叫 Service 方法容易造成肥大的 Controller 或 Service 類別累積不相關的責任。MediatR 提供 Request/Handler 模式，將 HTTP 邊界與應用程式邏輯分離，也更容易加入跨切面行為。

### 決策

Controller 透過 `IMediator.Send()` 分派請求。`NETCoreBase.Core/Commands/` 中的 Handler 以 Use Case 為單位組織。FluentValidation 透過 `ValidatorBehavior` 接入 MediatR Pipeline，讓驗證在每個 Handler 執行前自動觸發，無需在 Controller 重複寫驗證邏輯。

### 後果

- Controller 保持精簡：只負責將 HTTP 輸入轉成 Request 物件並呼叫 `Send()`
- 驗證、日誌和其他跨切面關注點可透過 Pipeline Behavior 統一加入
- 以 Use Case 為單位的資料夾結構（Request、Handler、Validator、Response）增加了檔案數量，但提升了導覽性
- 新開發者需要學習 MediatR Request/Handler 模式才能有效導覽程式碼
- 功能越多檔案越多；對非常小的功能可能會覺得過度設計

---

## ADR 006：雙層授權模型

### 背景

企業內部系統通常同時需要身份驗證（這個使用者是本人嗎？）和功能存取控制（這個使用者有權限執行這個操作嗎？）。單一 JWT 驗證不足以做到功能層級的權限控管。

### 決策

授權分兩層，均透過 `Startup.cs` 的 Action Filter 全域套用：

1. `JwtAuthPolicy` — 驗證 JWT Token 是否有效
2. `PermissionPolicy` — 根據使用者角色檢查功能層級的存取權

需要公開存取的端點必須明確加上 `[AllowAnonymous]`。

### 後果

- 在 API 邊界進行清晰、可稽核的授權
- 所有端點都會一致地進行功能權限檢查
- 新增公開端點需要明確宣告排除，比預設公開更安全
- 權限模型依賴資料庫中的角色和功能資料；授權要正常運作必須先建立好種子資料
- 權限規則由資料驅動，彈性高，但需要文件說明哪些角色有哪些功能
