// Decompiled with JetBrains decompiler
// Type: MsgPack.Float64Bits
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.InteropServices;

namespace MsgPack
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct Float64Bits
  {
    [FieldOffset(0)]
    public readonly double Value;
    [FieldOffset(0)]
    public readonly byte Byte0;
    [FieldOffset(1)]
    public readonly byte Byte1;
    [FieldOffset(2)]
    public readonly byte Byte2;
    [FieldOffset(3)]
    public readonly byte Byte3;
    [FieldOffset(4)]
    public readonly byte Byte4;
    [FieldOffset(5)]
    public readonly byte Byte5;
    [FieldOffset(6)]
    public readonly byte Byte6;
    [FieldOffset(7)]
    public readonly byte Byte7;

    public Float64Bits(byte[] bigEndianBytes, int offset)
      : this()
    {
      if (BitConverter.IsLittleEndian)
      {
        this.Byte0 = bigEndianBytes[offset + 7];
        this.Byte1 = bigEndianBytes[offset + 6];
        this.Byte2 = bigEndianBytes[offset + 5];
        this.Byte3 = bigEndianBytes[offset + 4];
        this.Byte4 = bigEndianBytes[offset + 3];
        this.Byte5 = bigEndianBytes[offset + 2];
        this.Byte6 = bigEndianBytes[offset + 1];
        this.Byte7 = bigEndianBytes[offset];
      }
      else
      {
        this.Byte0 = bigEndianBytes[offset];
        this.Byte1 = bigEndianBytes[offset + 1];
        this.Byte2 = bigEndianBytes[offset + 2];
        this.Byte3 = bigEndianBytes[offset + 3];
        this.Byte4 = bigEndianBytes[offset + 4];
        this.Byte5 = bigEndianBytes[offset + 5];
        this.Byte6 = bigEndianBytes[offset + 6];
        this.Byte7 = bigEndianBytes[offset + 7];
      }
    }
  }
}
