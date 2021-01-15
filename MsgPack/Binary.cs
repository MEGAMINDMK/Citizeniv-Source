// Decompiled with JetBrains decompiler
// Type: MsgPack.Binary
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Text;

namespace MsgPack
{
  internal static class Binary
  {
    public static readonly byte[] Empty = new byte[0];

    public static string ToHexString(byte[] blob)
    {
      return Binary.ToHexString(blob, true);
    }

    public static string ToHexString(byte[] blob, bool withPrefix)
    {
      if (blob == null || blob.Length == 0)
        return string.Empty;
      StringBuilder buffer = new StringBuilder(blob.Length * 2 + (withPrefix ? 2 : 0));
      Binary.ToHexStringCore(blob, buffer, withPrefix);
      return buffer.ToString();
    }

    public static void ToHexString(byte[] blob, StringBuilder buffer)
    {
      if (blob == null || blob.Length == 0)
        return;
      Binary.ToHexStringCore(blob, buffer, true);
    }

    private static void ToHexStringCore(byte[] blob, StringBuilder buffer, bool withPrefix)
    {
      if (withPrefix)
        buffer.Append("0x");
      foreach (byte num in blob)
      {
        buffer.Append(Binary.ToHexChar((int) num >> 4));
        buffer.Append(Binary.ToHexChar((int) num & 15));
      }
    }

    private static char ToHexChar(int b)
    {
      return b < 10 ? (char) (48 + b) : (char) (65 + (b - 10));
    }
  }
}
