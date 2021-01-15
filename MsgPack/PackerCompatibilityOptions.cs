// Decompiled with JetBrains decompiler
// Type: MsgPack.PackerCompatibilityOptions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack
{
  [Flags]
  public enum PackerCompatibilityOptions
  {
    None = 0,
    PackBinaryAsRaw = 1,
    ProhibitExtendedTypeObjects = 2,
    Classic = ProhibitExtendedTypeObjects | PackBinaryAsRaw, // 0x00000003
  }
}
