# API 筆記

API 路由大致照 Controller 名稱走。Account 相關路由比較特別，檔案叫 `LoginController.cs`，但 Controller class 是 `AccountController`。

## Account

`POST /api/Account/Login`

登入用。Body 帶 `UserName` 和 `UserPass`。成功後會回傳 token。

`POST /api/Account/Register`

註冊用。Body 帶帳號、密碼和姓名。註冊成功後也會回 token。

`GET /api/Account/GetFeatureTree`

取得目前登入者可看的功能樹。這支會從 token claim 取 `userId`。

## Users

`GET /api/Users`

查使用者清單。查詢條件放 query string。

`GET /api/Users/{id}`

查單一使用者。

`POST /api/Users`

建立使用者。

`PUT /api/Users/{id}`

修改使用者。Route id 必須和 body 裡的 id 一樣。

`DELETE /api/Users`

刪除使用者。這裡沿用原本寫法，刪除條件從 request body 或模型繫結進來。

## Roles

角色 API 的模式跟 Users 差不多：清單、單筆、建立、修改、刪除。

## Features

功能 API 用來維護系統選單和功能權限。角色能看到哪些功能，會透過資料表關聯處理。

## Swagger

開發環境會啟用 Swagger。先呼叫 Login 拿 token，再按 Swagger 右上角 Authorize，輸入 token 後就能測需要授權的 API。
