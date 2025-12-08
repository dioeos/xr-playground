using UnityEngine;
using System;

public class HandLogHandler : ILogHandler {
  public void LogFormat(LogType logType, UnityEngine.Object context,
                        string format, params object[] args) {
    string msg = string.Format(format, args);
    Debug.unityLogger.logHandler.LogFormat(logType, context, "[HAND] " + msg,
                                           args);
  }

  public void LogException(Exception exception, UnityEngine.Object context) {
    Debug.unityLogger.logHandler.LogException(
        new Exception("[HAND] " + exception.Message, exception), context);
  }
}
