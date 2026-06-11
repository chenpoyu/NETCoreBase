# 展望與整理路線

這份 roadmap 不是承諾表，比較像接手後可以怎麼整理的順序。原則是先把公開和可接手做好，再處理安全和品質，最後才談架構升級。

## 第一階段：公開前整理

目標：讓 repository 可以被人看懂，而且不要帶出敏感資訊。

已完成或正在做的事：

- 移除目前檔案中的實值密碼和金鑰。
- 加入中文 README。
- 補 Dockerfile、docker-compose 和 `.env.example`。
- 升到今天的 .NET 10 版本。
- 補 SA、SD、底層規劃和展望文件。

公開前仍要注意：

- Git history 裡曾出現過舊密碼和金鑰，不能直接把既有 repo 改 public。
- 若那些值曾在真實環境使用過，要先輪替。
- 新 public repository 最好用乾淨 history 建立。

## 第二階段：可執行環境

目標：讓新接手的人可以用固定步驟跑起來。

建議項目：

- 補 database init script 或 db-init container。
- 補 health check endpoint。
- 補 Docker Compose healthcheck。
- 補一份本機啟動檢查清單。
- 補 Swagger 登入示範 request。

這階段不要急著大改架構。先讓環境穩定，比什麼都重要。

## 第三階段：安全補強

目標：把真正不能進正式環境的安全點補起來。

建議項目：

- 密碼雜湊從 SHA1 改成正式 password hasher。
- 設計舊密碼轉換策略。
- JWT secret 長度和來源做啟動檢查。
- AES key/iv 做啟動檢查。
- CORS 不再使用完全開放規則。
- Swagger 在正式環境預設關閉。
- 權限不足、token 無效、驗證錯誤回應格式統一。

## 第四階段：測試和品質

目標：讓後續升級和擴功能比較不怕。

建議項目：

- 新增測試專案。
- 補 Login、Register、PermissionHandler 測試。
- 補 Service 層查詢和異動測試。
- 補 API smoke test。
- 補 `dotnet format` 或 editorconfig。
- 補 package vulnerability 檢查流程。

## 第五階段：架構整理

目標：在不破壞既有功能的前提下，把底層整理得更清楚。

可以考慮：

- 把 Common 對 Database 的依賴收斂。
- 把 GenericRepository 的責任切清楚。
- 把錯誤處理改成統一 Result 或 ProblemDetails。
- 整理 Controller 路由，讓 DELETE / PUT 更一致。
- 持續觀察 MediatR 14.x 之後的註冊方式和授權模式。
- 評估是否仍需要 Autofac。

這些改動都會碰到既有風格，不建議和安全修正混在同一批做。

## 長期方向

如果這套底層要繼續使用，可以走兩條路。

第一條是「維持舊專案升級底座」。這條路保留 Startup、DB First、Autofac，重點是安全、文件、Docker、測試和可維護性。改動小，適合拿來公開展示當年專案整理成果。

第二條是「演進成新專案樣板」。這條路會逐步改成比較新的 .NET 寫法，重新整理 Program.cs、DI、設定、授權、資料存取和測試。改動大，適合真的要拿去開新產品時再做。

目前比較建議先走第一條。把底層收乾淨、文件補完整、敏感資訊處理好，這樣公開後看起來會比較誠實，也比較像一個可以接手的老專案。
