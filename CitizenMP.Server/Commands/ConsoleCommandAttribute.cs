// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Commands.ConsoleCommandAttribute
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;

namespace CitizenMP.Server.Commands
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class ConsoleCommandAttribute : Attribute
  {
    public ConsoleCommandAttribute(string commandName)
    {
      this.CommandName = commandName;
    }

    public string CommandName { get; private set; }
  }
}
