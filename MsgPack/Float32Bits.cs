// Decompiled with JetBrains decompiler
// Type: MsgPack.Float32Bits
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.InteropServices;

namespace MsgPack
{
  [StructLayout(LayoutKind.Explicit)]
  internal struct Float32Bits
  {
    [FieldOffset(0)]
    public readonly float Value;
    [FieldOffset(0)]
    public readonly byte Byte0;
    [FieldOffset(1)]
    public readonly byte Byte1;
    [FieldOffset(2)]
    public readonly byte Byte2;
    [FieldOffset(3)]
    public readonly byte Byte3;

    public Float32Bits(float value)
      : this()
    {
      this.Value = value;
    }

    public Float32Bits(byte[] bigEndianBytes, int offset)
      : this()
    {
      if (BitConverter.IsLittleEndian)
      {
        this.Byte0 = bigEndianBytes[offset + 3];
        this.Byte1 = bigEndianBytes[offset + 2];
        this.Byte2 = bigEndianBytes[offset + 1];
        this.Byte3 = bigEndianBytes[offset];
      }
      else
      {
        this.Byte0 = bigEndianBytes[offset];
        this.Byte1 = bigEndianBytes[offset + 1];
        this.Byte2 = bigEndianBytes[offset + 2];
        this.Byte3 = bigEndianBytes[offset + 3];
      }
    }
  }
}
