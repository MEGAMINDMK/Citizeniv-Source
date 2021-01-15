// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Commands.CommandManager
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CitizenMP.Server.Commands
{
  public class CommandManager
  {
    private static Dictionary<string, MethodInfo> ms_consoleCommands = new Dictionary<string, MethodInfo>();

    internal GameServer GameServer { get; private set; }

    static CommandManager()
    {
      foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          ConsoleCommandAttribute customAttribute = method.GetCustomAttribute<ConsoleCommandAttribute>();
          if (customAttribute != null)
            CommandManager.ms_consoleCommands.Add(customAttribute.CommandName.ToLowerInvariant(), method);
        }
      }
    }

    public bool HandleCommand(string commandName, IEnumerable<string> args)
    {
      MethodInfo methodInfo;
      if (!CommandManager.ms_consoleCommands.TryGetValue(commandName.ToLowerInvariant(), out methodInfo))
        return false;
      methodInfo.Invoke((object) null, new object[3]
      {
        (object) this,
        (object) commandName,
        (object) args.ToArray<string>()
      });
      return true;
    }

    internal void SetGameServer(GameServer gameServer)
    {
      if (this.GameServer != null)
        throw new InvalidOperationException("This CommandManager is already associated with a GameServer.");
      this.GameServer = gameServer;
    }
  }
}
