// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Commands.ResourceCommands
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using CitizenMP.Server.Resources;
using System;

namespace CitizenMP.Server.Commands
{
  internal class ResourceCommands
  {
    [ConsoleCommand("refresh")]
    private static void Refresh_f(CommandManager manager, string command, string[] args)
    {
      ResourceManager resourceManager = manager.GameServer.ResourceManager;
      try
      {
        resourceManager.ScanResources("resources/", (string) null);
        RconPrint.Print("refresh\n");
      }
      catch (Exception ex)
      {
        resourceManager.Log<ResourceManager>(nameof (Refresh_f), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Commands\\ResourceCommands.cs", 24).Error((Func<string>) (() => "Error refreshing resources."), ex);
        RconPrint.Print("Error refreshing resources: {0}\n", (object) ex.Message);
      }
    }

    [ConsoleCommand("stop")]
    private static void Stop_f(CommandManager manager, string command, string[] args)
    {
      string name = args[0];
      Resource resource = manager.GameServer.ResourceManager.GetResource(name);
      if (resource == null)
        RconPrint.Print("No such resource: {0}.\n", (object) name);
      else if (resource.State != ResourceState.Running)
      {
        RconPrint.Print("Resource isn't running: {0}.\n", (object) name);
      }
      else
      {
        try
        {
          resource.Stop();
          if (resource.State != ResourceState.Stopped)
            RconPrint.Print("err stop {0}\n", (object) name);
          else
            RconPrint.Print("stop {0}\n", (object) name);
        }
        catch (Exception ex)
        {
          resource.Log<Resource>(nameof (Stop_f), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Commands\\ResourceCommands.cs", 63).Error((Func<string>) (() => "Error stopping resource."), ex);
          RconPrint.Print("Error stopping resource {0}: {1}.\n", (object) name, (object) ex.Message);
        }
      }
    }

    [ConsoleCommand("start")]
    private static void Start_f(CommandManager manager, string command, string[] args)
    {
      string name = args[0];
      Resource resource = manager.GameServer.ResourceManager.GetResource(name);
      if (resource == null)
        RconPrint.Print("No such resource: {0}.\n", (object) name);
      else if (resource.State != ResourceState.Stopped)
      {
        RconPrint.Print("Resource isn't stopped: {0}.\n", (object) name);
      }
      else
      {
        try
        {
          resource.Start(manager.GameServer.Configuration);
          if (resource.State != ResourceState.Running)
            RconPrint.Print("err start {0}\n", (object) name);
          else
            RconPrint.Print("start {0}\n", (object) name);
        }
        catch (Exception ex)
        {
          resource.Log<Resource>(nameof (Start_f), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Commands\\ResourceCommands.cs", 102).Error((Func<string>) (() => "Error starting resource."), ex);
          RconPrint.Print("Error starting resource {0}: {1}.\n", (object) name, (object) ex.Message);
        }
      }
    }

    [ConsoleCommand("restart")]
    private static void Restart_f(CommandManager manager, string command, string[] args)
    {
      string name = args[0];
      Resource resource = manager.GameServer.ResourceManager.GetResource(name);
      if (resource == null)
        RconPrint.Print("No such resource: {0}.\n", (object) name);
      else if (resource.State != ResourceState.Running)
      {
        RconPrint.Print("Resource isn't running: {0}.\n", (object) name);
      }
      else
      {
        try
        {
          resource.Stop();
          if (resource.State != ResourceState.Stopped)
          {
            RconPrint.Print("err restart {0}\n", (object) name);
          }
          else
          {
            resource.Start(manager.GameServer.Configuration);
            if (resource.State != ResourceState.Running)
              RconPrint.Print("err restart {0}\n", (object) name);
            else
              RconPrint.Print("restart {0}\n", (object) name);
          }
        }
        catch (Exception ex)
        {
          resource.Log<Resource>(nameof (Restart_f), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Commands\\ResourceCommands.cs", 151).Error((Func<string>) (() => "Error restarting resource."), ex);
          RconPrint.Print("Error restarting resource {0}: {1}.\n", (object) name, (object) ex.Message);
        }
      }
    }
  }
}
