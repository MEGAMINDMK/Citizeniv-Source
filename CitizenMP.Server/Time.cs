// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Time
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System.Diagnostics;

namespace CitizenMP.Server
{
  public static class Time
  {
    private static long ms_initialCount;

    public static void Initialize()
    {
      Time.ms_initialCount = Stopwatch.GetTimestamp();
    }

    public static long CurrentTime
    {
      get
      {
        return (Stopwatch.GetTimestamp() - Time.ms_initialCount) / (Stopwatch.Frequency / 1000L);
      }
    }
  }
}
