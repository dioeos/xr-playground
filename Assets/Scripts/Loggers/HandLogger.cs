using UnityEngine;

public static class HandLogger {
  private static readonly Logger _logger;

  static HandLogger() {
    _logger = new Logger(new HandLogHandler());
    _logger.logEnabled = true;
  }

  public static void Log(string message) { _logger.Log(LogType.Log, message); }

  public static void LogWarning(string message) {
    _logger.Log(LogType.Warning, message);
  }

  public static void LogError(string message) {
    _logger.Log(LogType.Error, message);
  }
}
