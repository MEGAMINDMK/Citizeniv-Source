// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.LogExtensions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Logging;
using System.Runtime.CompilerServices;

namespace CitizenMP.Server
{
  public static class LogExtensions
  {
    public static BaseLog Log<T>(
      this T type,
      [CallerMemberName] string memberName = "",
      [CallerFilePath] string sourceFilePath = "",
      [CallerLineNumber] int sourceLineNumber = 0)
    {
      return new BaseLog(typeof (T).FullName, memberName, sourceFilePath, sourceLineNumber);
    }
  }
}
