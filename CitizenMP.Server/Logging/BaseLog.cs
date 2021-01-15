// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Logging.BaseLog
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace CitizenMP.Server.Logging
{
  public class BaseLog
  {
    private static Logger ms_logger;
    private static string ms_basePath;

    public BaseLog(
      string typeName,
      string memberName,
      string sourceFilePath,
      int sourceLineNumber)
    {
      if (BaseLog.ms_logger == null)
        BaseLog.ms_logger = LogManager.GetLogger("CitizenMP.Server");
      MappedDiagnosticsContext.Set(nameof (typeName), ((IEnumerable<string>) typeName.Split('.')).Last<string>());
      MappedDiagnosticsContext.Set(nameof (memberName), memberName);
      MappedDiagnosticsContext.Set("sourceFile", sourceFilePath.Replace(BaseLog.ms_basePath, ""));
      MappedDiagnosticsContext.Set("sourceLine", sourceLineNumber.ToString());
    }

    internal static void SetStripSourceFilePath([CallerFilePath] string sourcePath = "")
    {
      BaseLog.ms_basePath = sourcePath.Replace("Program.cs", "");
    }

    public void Debug(string message, params object[] formatting)
    {
      if (!BaseLog.ms_logger.get_IsDebugEnabled())
        return;
      BaseLog.ms_logger.Debug(message, formatting);
    }

    public void Debug(Func<string> message)
    {
      if (!BaseLog.ms_logger.get_IsDebugEnabled())
        return;
      BaseLog.ms_logger.Debug(message());
    }

    public void Info(string message, params object[] formatting)
    {
      if (!BaseLog.ms_logger.get_IsInfoEnabled())
        return;
      BaseLog.ms_logger.Info(message, formatting);
    }

    public void Info(Func<string> message)
    {
      if (!BaseLog.ms_logger.get_IsInfoEnabled())
        return;
      BaseLog.ms_logger.Info(message());
    }

    public void Warn(string message, params object[] formatting)
    {
      if (!BaseLog.ms_logger.get_IsWarnEnabled())
        return;
      BaseLog.ms_logger.Warn(message, formatting);
    }

    public void Warn(Func<string> message)
    {
      if (!BaseLog.ms_logger.get_IsWarnEnabled())
        return;
      BaseLog.ms_logger.Warn(message());
    }

    public void Error(string message, params object[] formatting)
    {
      BaseLog.ms_logger.Error(message, formatting);
    }

    public void Error(Func<string> message)
    {
      BaseLog.ms_logger.Error(message());
    }

    public void Error(Func<string> message, Exception exception)
    {
      BaseLog.ms_logger.Error(message(), exception);
    }

    public void Fatal(string message, params object[] formatting)
    {
      BaseLog.ms_logger.Fatal(message, formatting);
      WindowedLogger.Fatal(string.Format(message, formatting));
    }

    public void Fatal(Func<string> message)
    {
      BaseLog.ms_logger.Fatal(message());
      WindowedLogger.Fatal(message());
    }

    public void Fatal(Func<string> message, Exception exception)
    {
      BaseLog.ms_logger.Fatal(message(), exception);
      WindowedLogger.Fatal(message());
    }
  }
}
