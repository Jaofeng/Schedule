
namespace CJF.Schedules;

#region Public Class : KeyExistsException
public class KeyExistsException(string key, string msg) : Exception(msg)
{
    public string Key { get; private set; } = key;
}
#endregion

#region Public Class : ExceptionEventArgs
public class ExceptionEventArgs(Exception ex) : EventArgs()
{
    public Exception Exception { get; private set; } = ex;
}
#endregion

