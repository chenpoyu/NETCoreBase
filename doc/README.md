# 文件目錄

這裡放的是專案交接用的筆記。內容照著程式目前的樣子寫，不把它包裝成全新的框架，也不補沒有做的功能。

如果要快速理解這個底層，建議照下面順序看：

1. `system-analysis.md`
2. `system-design.md`
3. `architecture-decisions.md`
4. `project-notes.md`
5. `handover-checklist.md`

這裡的 SA 指 System Analysis，偏需求、角色、流程、資料範圍；SD 指 System Design，偏架構、模組、介面、部署與技術決策。

- `project-notes.md`：專案分層、主要流程、幾個容易找錯位置的地方。
- `system-analysis.md`：SA 文件，說明系統定位、角色、功能需求、流程、資料需求與限制。
- `system-design.md`：SD 文件，說明分層設計、模組責任、授權流程、部署方式與設計取捨。
- `architecture-decisions.md`：主要架構決策與理由（DB First、Startup、Autofac、MediatR、JWT、授權模型）。
- `base-framework-plan.md`：底層規劃，說明這套底層想解決什麼、保留什麼、後續怎麼擴。
- `roadmap.md`：後續展望和分階段整理方向。
- `operation-and-security.md`：公開、設定、部署與維運上的注意事項。
- `handover-checklist.md`：新接手人員的逐項確認清單。
- `production-considerations.md`：上正式環境前需要處理的項目。
- `api-notes.md`：Controller 和 API 使用方式。
- `database-notes.md`：資料庫、DbContext、Schema 腳本的說明。
- `net10-upgrade.md`：這次升到 .NET 10 時動到的地方。
- `net8-upgrade.md`：前一次升到 .NET 8 的歷史紀錄。
