// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.OutReliableCommand
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

namespace CitizenMP.Server
{
  public struct OutReliableCommand
  {
    public uint ID { get; set; }

    public uint Type { get; set; }

    public byte[] Command { get; set; }
  }
}
