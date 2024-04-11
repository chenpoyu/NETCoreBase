# 維運與安全筆記

這份文件放一些公開和部署前要注意的事。它不取代正式資安文件，只是把這個專案目前最容易踩到的點先寫出來。

## Secrets

repository 裡不應該放：

- SQL Server 密碼。
- JWT Secret。
- AES Key / IV。
- SMTP 密碼。
- private key、certificate、token。
- 正式環境 appsettings。

目前公開檔案只保留 `CHANGE_ME...` 這類占位字。真正的值請放在本機 `.env`、部署平台 secret 或其他 secret manager。

## Git history

只改目前檔案不代表 Git history 乾淨。這個 repository 過去 commit 曾出現過舊密碼和金鑰，所以如果要公開，建議做法是：

1. 先輪替曾經出現過的值。
2. 用乾淨 history 建立新的 public repository。
3. 或使用 history rewrite 工具清掉舊值，再確認遠端也已處理。

如果只是把 GitHub 上既有 private repository 直接切成 public，舊 commit 仍然會被看見。

## Docker

Compose 會從 `.env` 讀設定。缺少必要值時，compose 會直接報錯，不讓服務用空字串啟動。

SQL Server 的密碼要符合 image 的密碼規則。`.env.example` 只是範例，不是正式密碼。

Seq 是本機觀察 log 用。正式環境要另外決定 log 保存時間、權限和儲存位置。

## Swagger

目前只有 Development 環境會啟用 Swagger。正式部署時請確認 `ASPNETCORE_ENVIRONMENT` 不要設成 Development。

Swagger 的 Authorize 使用方式是貼 JWT token。若未來要對外開放 API 文件，應另外處理文件權限，不要把內部管理 API 完整暴露出去。

## CORS

目前 CORS 使用 `SetIsOriginAllowed(orign => true)`，等於允許所有 origin。這對本機開發很方便，但正式環境不建議這樣用。

正式環境應改成白名單，並且確認是否真的需要 `AllowCredentials()`。

## 密碼

目前密碼雜湊 helper 使用 SHA1。這是舊專案留下來的寫法，不建議拿去正式產品。

如果要正式使用，建議改成 ASP.NET Core Identity 的 PasswordHasher，或其他帶 salt、work factor 的密碼雜湊方案。改之前要先想好舊密碼怎麼轉，不能只改新密碼。

## Token

JWT Secret 不應使用短字串。正式環境請使用足夠長度且不可猜的值。

Access token 有效時間目前由 `AccessTokenExpiration` 控制。正式系統要依照風險調整，也要決定 refresh token 是否真的要實作完整流程。

## 資料庫

Schema 腳本會 drop database，使用前要看清楚。這份腳本適合本機或初始化環境，不應直接拿去跑正式資料庫。

正式環境至少要有：

- 備份策略。
- 還原演練。
- 權限最小化的 DB 帳號。
- migration 或變更腳本審查流程。
