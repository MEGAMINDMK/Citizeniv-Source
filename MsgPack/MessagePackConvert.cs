// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackConvert
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Text;

namespace MsgPack
{
  public static class MessagePackConvert
  {
    private static readonly Encoding _utf8NonBomStrict = (Encoding) new UTF8Encoding(false, true);
    private static readonly Encoding _utf8NonBom = (Encoding) new UTF8Encoding(false, false);
    private static readonly DateTime _unixEpocUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const long _ticksToMilliseconds = 10000;

    internal static Encoding Utf8NonBom
    {
      get
      {
        return MessagePackConvert._utf8NonBom;
      }
    }

    internal static Encoding Utf8NonBomStrict
    {
      get
      {
        return MessagePackConvert._utf8NonBomStrict;
      }
    }

    public static byte[] EncodeString(string value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return MessagePackConvert._utf8NonBom.GetBytes(value);
    }

    public static string DecodeStringStrict(byte[] value)
    {
      if (value == null)
        throw new ArgumentNullException(nameof (value));
      return MessagePackConvert._utf8NonBomStrict.GetString(value, 0, value.Length);
    }

    public static DateTimeOffset ToDateTimeOffset(long value)
    {
      return (DateTimeOffset) MessagePackConvert._unixEpocUtc.AddTicks(value * 10000L);
    }

    public static DateTime ToDateTime(long value)
    {
      return MessagePackConvert._unixEpocUtc.AddTicks(value * 10000L);
    }

    public static long FromDateTimeOffset(DateTimeOffset value)
    {
      return value.ToUniversalTime().Subtract((DateTimeOffset) MessagePackConvert._unixEpocUtc).Ticks / 10000L;
    }

    public static long FromDateTime(DateTime value)
    {
      return value.ToUniversalTime().Subtract(MessagePackConvert._unixEpocUtc).Ticks / 10000L;
    }
  }
}
