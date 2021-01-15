// Decompiled with JetBrains decompiler
// Type: MsgPack.UnsafeNativeMethods
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace MsgPack
{
  [SecurityCritical]
  [SuppressUnmanagedCodeSecurity]
  internal static class UnsafeNativeMethods
  {
    private const int _libCAvailability_Unknown = 0;
    private const int _libCAvailability_MSVCRT = 1;
    private const int _libCAvailability_LibC = 2;
    private const int _libCAvailability_None = -1;
    private static int _libCAvailability;

    [DllImport("msvcrt", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmpVC(byte[] s1, byte[] s2, UIntPtr size);

    [DllImport("libc", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl)]
    private static extern int memcmpLibC(byte[] s1, byte[] s2, UIntPtr size);

    public static bool TryMemCmp(byte[] s1, byte[] s2, UIntPtr size, out int result)
    {
      if (UnsafeNativeMethods._libCAvailability < 0)
      {
        result = 0;
        return false;
      }
      if (UnsafeNativeMethods._libCAvailability <= 1)
      {
        try
        {
          result = UnsafeNativeMethods.memcmpVC(s1, s2, size);
          return true;
        }
        catch (DllNotFoundException ex)
        {
          Interlocked.Exchange(ref UnsafeNativeMethods._libCAvailability, 2);
        }
      }
      if (UnsafeNativeMethods._libCAvailability <= 2)
      {
        try
        {
          result = UnsafeNativeMethods.memcmpLibC(s1, s2, size);
          return true;
        }
        catch (DllNotFoundException ex)
        {
          Interlocked.Exchange(ref UnsafeNativeMethods._libCAvailability, -1);
        }
      }
      result = 0;
      return false;
    }
  }
}
