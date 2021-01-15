// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.TokenGenerator
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace CitizenMP.Server
{
  public class TokenGenerator
  {
    private static RandomNumberGenerator ms_rng = RandomNumberGenerator.Create();

    public static string GenerateToken()
    {
      byte[] data = new byte[20];
      TokenGenerator.ms_rng.GetBytes(data);
      return ((IEnumerable<byte>) data).Aggregate<byte, string>("", (Func<string, byte, string>) ((a, b) => a + b.ToString("x2")));
    }
  }
}
