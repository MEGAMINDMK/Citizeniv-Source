// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ICallRefHandler
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;

namespace CitizenMP.Server.Resources
{
  public interface ICallRefHandler
  {
    Delegate GetRef(int reference);

    bool HasRef(int reference, uint instance);
  }
}
