# CJF.Schedule.Test - 測試專案

這是 CJF.Schedule 排程工作器程式庫的測試專案，使用 xUnit 測試框架進行功能驗證。

## 專案結構

```
CJF.Schedule.Test/
├── BasicTests.cs                    # 基本功能測試
├── SchedulePlanTests.cs             # SchedulePlan 類別測試
├── PlanWorkerTests.cs               # PlanWorker 類別測試
├── PlanAttributeTests.cs            # PlanAttribute 屬性測試
├── EnumsTests.cs                    # 列舉類型測試
├── ExceptionTests.cs                # 自訂例外測試
├── HostBuilderExtensionsTests.cs    # 主機建構器擴充測試
├── CJF.Schedule.Test.csproj         # 專案檔案
├── README.md                        # 本文件
└── 測試說明文件.md                    # 詳細測試說明
```

## 測試類別說明

### BasicTests.cs
- **目的**: 驗證核心功能的基本運作
- **測試範圍**: SchedulePlan 基本建立、PlanWorkerOptions 預設值、列舉值驗證、例外處理

### SchedulePlanTests.cs
- **目的**: 測試 SchedulePlan 類別的完整功能
- **測試範圍**: 建構函式、屬性設定、時間表功能、複製功能

### PlanWorkerTests.cs
- **目的**: 測試 PlanWorker 背景服務的功能
- **測試範圍**: 排程管理、生命週期控制、事件處理

### PlanAttributeTests.cs
- **目的**: 測試 PlanAttribute 裝飾器屬性的功能
- **測試範圍**: 各種建構函式、參數驗證、屬性設定

### EnumsTests.cs
- **目的**: 測試所有列舉類型的正確性
- **測試範圍**: PlanTypes、WeekDays、Months、WeekNo、Days 列舉值和旗標操作

### ExceptionTests.cs
- **目的**: 測試自訂例外類別的功能
- **測試範圍**: KeyExistsException、ExceptionEventArgs 的建立和屬性

### HostBuilderExtensionsTests.cs
- **目的**: 測試主機建構器的擴充方法
- **測試範圍**: 服務註冊、設定選項、託管服務註冊

## 技術棧

- **.NET 8.0**: 目標框架
- **xUnit**: 測試框架 (v2.9.2)
- **Moq**: 模擬物件框架 (v4.20.72)
- **Microsoft.Extensions.Hosting**: 主機服務支援 (v9.0.8)
- **Microsoft.Extensions.DependencyInjection**: 依賴注入 (v9.0.8)
- **Microsoft.Extensions.Logging**: 日誌記錄 (v9.0.8)

## 執行測試

### 執行所有測試
```bash
dotnet test
```

### 執行特定測試類別
```bash
dotnet test --filter "FullyQualifiedName~BasicTests"
```

### 產生詳細測試報告
```bash
dotnet test --verbosity normal --logger "trx;LogFileName=TestResults.trx"
```

### 程式碼覆蓋率
```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 測試最佳實務

### 測試結構
所有測試都遵循 AAA (Arrange-Act-Assert) 模式：
```csharp
[Fact]
public void Method_Condition_ExpectedResult()
{
    // Arrange - 準備測試資料和環境
    var input = "test data";
    
    // Act - 執行被測試的方法
    var result = MethodUnderTest(input);
    
    // Assert - 驗證結果
    Assert.Equal(expectedResult, result);
}
```

### 命名慣例
- **測試方法**: `MethodName_Condition_ExpectedResult`
- **測試類別**: `{ClassUnderTest}Tests`
- **測試資料**: 使用描述性的變數名稱

### 測試隔離
- 每個測試都是獨立的，不依賴其他測試的結果
- 使用 `IDisposable` 進行資源清理
- 避免共享狀態

## 已知限制

### PlanWorker 反射測試
由於 PlanWorker 在建構時會掃描所有組件尋找帶有 PlanAttribute 的方法，在測試環境中可能會遇到類型載入問題。這些測試被暫時排除，但基本功能測試仍然有效。

### 測試資料
- 使用固定日期 (2023-01-01) 以確保測試結果的一致性
- 時間相關的測試使用相對時間而非絕對時間

## 故障排除

### 常見問題
1. **TypeLoadException**: 如果遇到類型載入例外，通常是因為測試環境的版本衝突
2. **時間相關測試**: 確保系統時間設定正確
3. **反射相關測試**: 某些反射功能在測試環境中可能有限制

### 解決方案
- 重新建置專案: `dotnet clean && dotnet build`
- 檢查 NuGet 套件版本衝突
- 隔離問題測試到單獨的測試檔案

## 持續改進

測試專案會持續改進以涵蓋更多場景和邊界條件。歡迎提交新的測試案例和改進建議。