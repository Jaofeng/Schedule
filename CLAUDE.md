# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 專案概述

這是一個 .NET 8.0 排程工作器 (Schedule Plan Worker) 程式庫，提供了強大的排程功能，支援多種排程類型：
- 一次性執行 (Once)
- 每日執行 (Day)  
- 每週執行 (Week)
- 每月執行 (Month)
- 每月週執行 (MonthWeek)
- 程式啟動/停止時執行 (Startup/Stoped)

## 核心架構

### 主要元件
- **PlanWorker**: 主要的背景服務，繼承自 `BackgroundService`，負責執行排程邏輯
- **SchedulePlan**: 實作 `ISchedulePlan` 介面，代表單一排程項目
- **PlanCollection**: 實作 `IPlanCollection` 介面，管理排程項目集合  
- **TimePlan**: 實作 `IPlanTime` 介面，處理時間計算邏輯
- **PlanAttribute**: 裝飾器屬性，用於自動綁定靜態方法為排程項目

### 核心流程
1. **初始化**: `PlanWorker` 啟動時自動掃描所有帶有 `PlanAttribute` 的靜態方法
2. **排程執行**: 定期檢查所有排程項目的執行時間 (預設每30秒檢查一次)
3. **生命週期管理**: 支援啟動和停止時的特殊排程執行

## 常用指令

### 建置專案
```bash
dotnet build Schedule.sln
```

### 執行測試
```bash
# 執行所有測試
dotnet test

# 執行特定測試類別
dotnet test --filter "ClassName=BasicFunctionalTests"

# 產生測試報告
dotnet test --logger "trx;LogFileName=TestResults.trx"

# 程式碼覆蓋率
dotnet test --collect:"XPlat Code Coverage"
```

### 封裝 NuGet 套件
```bash
dotnet pack CJF.Schedule/Schedule.csproj -o ./bin
```

## 程式碼慣例

### 專案結構
- **CJF.Schedule/**: 主要程式庫專案
  - **Interfaces/**: 介面定義
  - **PlanWorker.cs**: 主要工作器實作
  - **SchedulePlan.cs**: 排程項目實作
  - **TimePlan.cs**: 時間計畫實作
  - **_Enums.cs**: 列舉定義
  - **_Extensions.cs**: 擴充方法
- **CJF.Schedule.Test/**: 測試專案，使用 xUnit 框架

### 命名慣例
- 使用 PascalCase 命名公開成員
- 私有欄位使用底線前綴 (_field)
- 介面使用 I 前綴 (ISchedulePlan)
- 屬性使用 Attribute 後綴 (PlanAttribute)

### 排程表達式格式
排程使用簡易表達式，格式為空白區隔的欄位：
```
PlanTypes.Once: "1 yyyy-MM-dd HH:mm:ss"
PlanTypes.Day: "2 yyyy-MM-dd HH:mm:ss period"
PlanTypes.Week: "3 yyyy-MM-dd HH:mm:ss period weekdays"
PlanTypes.Month: "4 yyyy-MM-dd HH:mm:ss months days"
PlanTypes.MonthWeek: "5 yyyy-MM-dd HH:mm:ss months weeknos weekdays"
PlanTypes.Startup: "6"
PlanTypes.Stoped: "7"
```

## 測試考量

- 使用 xUnit 作為測試框架
- 包含 Moq 用於模擬物件
- 測試覆蓋基本功能、公共 API 和屬性/列舉
- 使用固定日期 (2023-01-01) 確保測試一致性
- 遵循 AAA (Arrange-Act-Assert) 模式

## 依賴項目

主要專案依賴：
- Microsoft.Extensions.Hosting (9.0.8)
- .NET 8.0

測試專案額外依賴：
- Microsoft.NET.Test.Sdk
- xunit
- Moq
- Microsoft.Extensions.DependencyInjection