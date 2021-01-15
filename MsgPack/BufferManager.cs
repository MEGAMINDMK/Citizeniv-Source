// Decompiled with JetBrains decompiler
// Type: MsgPack.BufferManager
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Diagnostics;

namespace MsgPack
{
  internal static class BufferManager
  {
    [ThreadStatic]
    private static byte[] _byteBuffer;
    [ThreadStatic]
    private static char[] _charBuffer;

    public static byte[] GetByteBuffer()
    {
      if (BufferManager._byteBuffer == null)
        BufferManager._byteBuffer = new byte[32768];
      return BufferManager._byteBuffer;
    }

    [Conditional("DEBUG")]
    public static void ReleaseByteBuffer()
    {
    }

    public static char[] GetCharBuffer()
    {
      if (BufferManager._charBuffer == null)
        BufferManager._charBuffer = new char[32768];
      return BufferManager._charBuffer;
    }

    [Conditional("DEBUG")]
    public static void ReleaseCharBuffer()
    {
    }
  }
}
