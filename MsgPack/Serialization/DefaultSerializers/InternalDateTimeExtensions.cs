// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.InternalDateTimeExtensions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.InteropServices.ComTypes;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal static class InternalDateTimeExtensions
  {
    private static readonly DateTime _fileTimeEpocUtc = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public static DateTime ToDateTime(this FILETIME source)
    {
      return InternalDateTimeExtensions._fileTimeEpocUtc.AddTicks((long) source.dwHighDateTime << 32 | (long) source.dwLowDateTime & (long) uint.MaxValue);
    }

    public static FILETIME ToWin32FileTimeUtc(this DateTime source)
    {
      long fileTimeUtc = source.ToFileTimeUtc();
      return new FILETIME()
      {
        dwHighDateTime = (int) (fileTimeUtc >> 32),
        dwLowDateTime = (int) (fileTimeUtc & (long) uint.MaxValue)
      };
    }
  }
}
