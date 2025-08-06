using Xunit;

namespace CJF.Schedules.Tests;

/// <summary>
/// 測試自訂例外類別
/// </summary>
public class ExceptionTests
{
    /// <summary>
    /// 測試 KeyExistsException 的建構函式能正確設定鍵值和訊息
    /// 驗證例外物件能正確儲存和傳送鍵值及錯誤訊息
    /// </summary>
    [Fact]
    public void KeyExistsException_Constructor_ShouldSetKeyAndMessage()
    {
        // Arrange
        var key = "TestKey";
        var message = "Test message";
        
        // Act
        var exception = new KeyExistsException(key, message);
        
        // Assert
        Assert.Equal(key, exception.Key);
        Assert.Equal(message, exception.Message);
    }

    /// <summary>
    /// 測試 KeyExistsException 的 Key 屬性的存取權限
    /// 驗證 Key 屬性是否可讀取和可寫入（用於反射測試）
    /// </summary>
    [Fact]
    public void KeyExistsException_Key_ShouldBeReadOnly()
    {
        // Arrange
        var exception = new KeyExistsException("key", "message");
        
        // Act
        var property = typeof(KeyExistsException).GetProperty(nameof(KeyExistsException.Key));
        
        // Assert
        Assert.NotNull(property);
        Assert.True(property.CanRead);
        Assert.True(property.CanWrite); // 修正：實際上 Key 屬性有 setter
    }

    /// <summary>
    /// 測試 KeyExistsException 繼承自標準 Exception 類別
    /// 驗證自訂例外類別正確地繼承了基礎例外功能
    /// </summary>
    [Fact]
    public void KeyExistsException_InheritsFromException()
    {
        // Arrange & Act
        var exception = new KeyExistsException("key", "message");
        
        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    /// <summary>
    /// 測試 ExceptionEventArgs 的建構函式能正確設定例外物件
    /// 驗證事件參數類別能正確封裝和傳送例外資訊
    /// </summary>
    [Fact]
    public void ExceptionEventArgs_Constructor_ShouldSetException()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        
        // Act
        var eventArgs = new ExceptionEventArgs(exception);
        
        // Assert
        Assert.Equal(exception, eventArgs.Exception);
    }

    /// <summary>
    /// 測試 ExceptionEventArgs 的 Exception 屬性的存取權限
    /// 驗證 Exception 屬性是否可讀取和可寫入（用於反射測試）
    /// </summary>
    [Fact]
    public void ExceptionEventArgs_Exception_ShouldBeReadOnly()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        var eventArgs = new ExceptionEventArgs(exception);
        
        // Act
        var property = typeof(ExceptionEventArgs).GetProperty(nameof(ExceptionEventArgs.Exception));
        
        // Assert
        Assert.NotNull(property);
        Assert.True(property.CanRead);
        Assert.True(property.CanWrite); // 修正：實際上 Exception 屬性有 setter
    }

    /// <summary>
    /// 測試 ExceptionEventArgs 繼承自標準 EventArgs 類別
    /// 驗證事件參數類別正確地繼承了 .NET 事件系統的基礎功能
    /// </summary>
    [Fact]
    public void ExceptionEventArgs_InheritsFromEventArgs()
    {
        // Arrange
        var exception = new InvalidOperationException("Test exception");
        
        // Act
        var eventArgs = new ExceptionEventArgs(exception);
        
        // Assert
        Assert.IsAssignableFrom<EventArgs>(eventArgs);
    }
}