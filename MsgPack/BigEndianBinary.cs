// Decompiled with JetBrains decompiler
// Type: MsgPack.BigEndianBinary
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack
{
  internal static class BigEndianBinary
  {
    public static sbyte ToSByte(byte[] buffer, int offset)
    {
      return (sbyte) buffer[offset];
    }

    public static short ToInt16(byte[] buffer, int offset)
    {
      return (short) ((int) buffer[offset] << 8 | (int) buffer[1 + offset]);
    }

    public static int ToInt32(byte[] buffer, int offset)
    {
      return (int) buffer[offset] << 24 | (int) buffer[1 + offset] << 16 | (int) buffer[2 + offset] << 8 | (int) buffer[3 + offset];
    }

    public static long ToInt64(byte[] buffer, int offset)
    {
      return (long) buffer[offset] << 56 | (long) buffer[1 + offset] << 48 | (long) buffer[2 + offset] << 40 | (long) buffer[3 + offset] << 32 | (long) buffer[4 + offset] << 24 | (long) buffer[5 + offset] << 16 | (long) buffer[6 + offset] << 8 | (long) buffer[7 + offset];
    }

    public static byte ToByte(byte[] buffer, int offset)
    {
      return buffer[offset];
    }

    public static ushort ToUInt16(byte[] buffer, int offset)
    {
      return (ushort) ((uint) buffer[offset] << 8 | (uint) buffer[1 + offset]);
    }

    public static uint ToUInt32(byte[] buffer, int offset)
    {
      return (uint) ((int) buffer[offset] << 24 | (int) buffer[1 + offset] << 16 | (int) buffer[2 + offset] << 8) | (uint) buffer[3 + offset];
    }

    public static ulong ToUInt64(byte[] buffer, int offset)
    {
      return (ulong) ((long) buffer[offset] << 56 | (long) buffer[1 + offset] << 48 | (long) buffer[2 + offset] << 40 | (long) buffer[3 + offset] << 32 | (long) buffer[4 + offset] << 24 | (long) buffer[5 + offset] << 16 | (long) buffer[6 + offset] << 8) | (ulong) buffer[7 + offset];
    }

    public static float ToSingle(byte[] buffer, int offset)
    {
      return new Float32Bits(buffer, offset).Value;
    }

    public static double ToDouble(byte[] buffer, int offset)
    {
      return new Float64Bits(buffer, offset).Value;
    }
  }
}
