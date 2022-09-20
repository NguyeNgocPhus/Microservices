using System.Runtime.CompilerServices;

namespace Identity.Application.Services.Logger;

public interface  ILoggerServices
{
    void LogFatal(
      Exception exception,
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogFatal(
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerPath = "",
      [CallerLineNumber] int line = 0,
      params object[] args);

    void LogDebug(
      Exception exception,
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogDebug(
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogError(
      Exception exception,
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogError(
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogInformation(
      Exception exception,
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogInformation(
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogWarning(
      Exception exception,
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);

    void LogWarning(
      string message,
      [CallerMemberName] string caller = "",
      [CallerFilePath] string callerFilePath = "",
      [CallerLineNumber] int callerLineNumber = 0,
      params object[] args);
}