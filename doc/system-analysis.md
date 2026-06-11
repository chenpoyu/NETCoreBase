# SA：系統分析

這份文件先把這個底層當成一個要交給下一位工程師接手的專案來看。它不是完整業務系統，而是一個後端基底：把登入、JWT、權限、角色、功能選單、資料存取、Excel 匯出和 Swagger 先整理好，後面要做正式系統時，可以從這裡接業務功能。

## 系統定位

NETCoreBase 的定位是「管理型 Web API 底層」。它適合拿來做後台、內部系統、權限控管比較明確的 API 服務。前端不在這個 repository 裡，這裡只處理後端 API、資料庫存取和共用基礎能力。

這個專案不追求一開始就變成大型框架。比較務實的方向是：先把每個管理系統都會遇到的重複工作收進來，讓後續開發不用每次從登入、權限、Repository 和 Swagger 重新開始。

## 使用者與角色

系統使用者大致分成三種：

- 一般登入使用者：透過帳號密碼登入，取得 token 後呼叫 API。
- 系統管理者：維護使用者、角色、功能選單與權限。
- 開發者：在這個底層上新增業務 Controller、Request、Handler、Service 和資料表。

目前程式沒有把「系統管理者」寫成固定角色名稱，而是透過角色、功能、權限資料表來組合。這樣比較彈性，但也代表初始資料要準備好，否則登入後不會自動有完整選單。

## 系統邊界

這個 repository 負責：

- API 入口與路由。
- JWT 登入驗證。
- 功能權限判斷。
- 使用者、角色、功能的基本 CRUD。
- SQL Server 存取。
- Excel 匯出。
- Swagger 文件。
- Docker 本機啟動環境。

這個 repository 不負責：

- 前端畫面。
- 第三方登入。
- 正式寄信服務。
- CI/CD pipeline。
- 正式環境的 secrets 管理。
- 多租戶或組織層級權限。

## 功能需求

帳號功能：

- 使用者可以註冊。
- 使用者可以登入。
- 登入成功後取得 JWT。
- 系統可以查詢、建立、修改、刪除使用者。

角色功能：

- 系統可以查詢角色清單。
- 系統可以建立、修改、刪除角色。
- 角色可以和功能建立關聯。

功能與權限：

- 系統可以維護功能資料。
- 功能可以有父子層級，支援選單樹。
- 登入者可以查詢自己可看的功能樹。
- API 可透過授權 policy 做權限檢查。

資料輸出：

- 回傳資料可以透過 Excel formatter 輸出。
- 欄位顯示名稱可透過 attribute 控制。

系統文件：

- 開發環境提供 Swagger。
- README 和 doc 需能讓新接手的人知道怎麼看專案。

## 使用案例摘要

UC-01 使用者登入：

使用者輸入帳號密碼，系統驗證帳密和帳號狀態。驗證成功後回傳 JWT；驗證失敗時回傳錯誤。這是後續所有授權 API 的入口。

UC-02 使用者維護：

系統管理者查詢使用者清單，建立新使用者，或修改既有使用者資料。這裡主要處理帳號資料，不處理前端畫面。

UC-03 角色維護：

系統管理者建立角色，調整角色名稱與狀態。角色本身不代表權限，權限要透過角色和功能關聯決定。

UC-04 功能維護：

系統管理者維護功能選單和功能路徑。功能可有父子階層，讓前端可以組選單。

UC-05 使用者取得功能樹：

使用者登入後，前端呼叫功能樹 API。系統依照使用者角色找到可用功能，再回傳給前端顯示。

UC-06 匯出 Excel：

API 回傳資料時，可透過 Excel formatter 輸出檔案。這個能力適合用在清單型資料，不適合處理大量報表或複雜樣板報表。

## 主要流程

登入流程：

1. 使用者呼叫 `POST /api/Account/Login`。
2. 系統用帳號和密碼雜湊查使用者。
3. 查到啟用中的使用者後，組 claims。
4. `JwtAuthManager` 產生 JWT。
5. API 回傳 token。

授權流程：

1. 使用者呼叫需要授權的 API。
2. ASP.NET Core Authentication 驗證 Bearer token。
3. `JwtAuthHandler` 檢查 token 是否可解。
4. `PermissionHandler` 檢查使用者和功能權限。
5. 通過後才進入 Controller action。

功能樹流程：

1. 前端帶 token 呼叫 `GET /api/Account/GetFeatureTree`。
2. Controller 從 claims 取 `userId`。
3. Core 查詢使用者角色與角色功能。
4. 回傳可用功能清單或樹狀資料。

## 資料需求

系統目前用 SQL Server，資料表集中在幾個主題：

- `Users`：帳號、密碼雜湊、姓名、聯絡資訊、登入狀態。
- `UserLogins`：登入紀錄。
- `Roles`：角色。
- `UserRoles`：使用者和角色關聯。
- `Features`：功能與選單。
- `FeaturePermissions`：功能可用的權限項目。
- `RoleFeatures`：角色可使用的功能。
- `Banners`：Banner 資料，屬於早期底層一起放進來的範例表。

目前 schema 只建立結構，沒有放正式 seed data。若要完整跑一套後台，至少要補第一個管理者、角色、功能和角色功能關聯。

## 非功能需求

安全：

- 不把真實密碼、金鑰、JWT secret 放進 repository。
- JWT secret、AES key、SQL 密碼由環境變數或部署環境提供。
- 公開 repository 前要清 Git history，不能只改目前檔案。

可維護性：

- Controller 保持薄，流程交給 MediatR 和 Service。
- 共用能力放在 Common，不讓每個業務模組各寫一套。
- DB First 模式下，資料表異動要同步更新 Model。

可觀測性：

- 使用 Serilog 記錄 request log。
- 本機可以透過 Seq 看 log。

部署：

- API 可用 Dockerfile build。
- 本機可用 docker compose 起 API、SQL Server、Seq。
- Schema 目前維持手動套用，不在 API 啟動時自動 migration。

## 驗收觀點

以底層專案來看，驗收不應只看某支 API 能不能回資料。比較合理的驗收點是：

- 新人能照 README 和 doc 找到專案入口、資料庫腳本和 Docker 啟動方式。
- appsettings 裡沒有真實密碼和金鑰。
- 登入、JWT、權限檢查的責任位置清楚。
- 新增一個管理功能時，有明確資料夾和命名規則可依循。
- 資料庫是 DB First，後續異動方式有寫清楚。
- 哪些地方不能直接拿去正式環境，文件有明確提醒。

## 限制與風險

這份底層保留早期寫法，所以有些地方不是 2026 新專案常見的樣子。例如仍保留 Startup、Autofac module、DB First scaffold。MediatR 已改用新版註冊方式，但整體專案仍維持舊底層的骨架。這不是不能用，但接手的人要知道這些選擇是為了延續既有專案，而不是要展示最新寫法。

密碼雜湊目前用 SHA1 helper，這在正式產品不建議沿用。若這套底層要拿去做真正服務，帳密安全會是第一批要調整的項目。

權限模型已有雛形，但還沒有完整管理畫面和 seed data。後續若要讓它變成可交付的管理後台，需要補初始化資料、管理流程和測試。
