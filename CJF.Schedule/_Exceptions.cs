
namespace CJF.Schedules;

#region Public Class : KeyExistsException
/// <summary>當嘗試添加已存在的鍵時拋出此異常。</summary>
/// <param name="key">已存在的鍵。</param>
/// <param name="msg">異常消息。</param>
/// <remarks>此異常用於表示嘗試添加的鍵已經存在於集合中。</remarks>
public class KeyExistsException(string key, string msg) : Exception(msg)
{
    public string Key { get; private set; } = key;
}
#endregion

#region Public Class : ExceptionEventArgs
/// <summary>表示異常事件的參數。</summary>
/// <param name="ex">引發的異常。</param>
/// <remarks>此類用於在異常發生時傳遞相關的異常信息。</remarks>
public class ExceptionEventArgs(Exception ex) : EventArgs()
{
    public Exception Exception { get; private set; } = ex;
}
#endregion

