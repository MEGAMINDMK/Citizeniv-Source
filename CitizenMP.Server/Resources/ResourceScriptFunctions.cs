// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ResourceScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using Neo.IronLua;
using System;

namespace CitizenMP.Server.Resources
{
  internal class ResourceScriptFunctions
  {
    [LuaMember("GetInvokingResource", false)]
    private static string GetInvokingResource_f()
    {
      return (ScriptEnvironment.LastEnvironment ?? ScriptEnvironment.CurrentEnvironment).Resource.Name;
    }

    [LuaMember("StopResource", false)]
    private static bool StopResource_f(string resourceName)
    {
      Resource resource = ScriptEnvironment.CurrentEnvironment.Resource.Manager.GetResource(resourceName);
      if (resource == null)
        return false;
      if (resource.State != ResourceState.Running)
        return false;
      try
      {
        return resource.Stop();
      }
      catch (Exception ex)
      {
        RconPrint.Print("Error stopping resource {0}: {1}.\n", (object) resourceName, (object) ex.Message);
        return false;
      }
    }

    [LuaMember("StartResource", false)]
    private static bool StartResource_f(string resourceName)
    {
      ResourceManager manager = ScriptEnvironment.CurrentEnvironment.Resource.Manager;
      Resource resource = manager.GetResource(resourceName);
      if (resource == null || resource.State != ResourceState.Stopped && resource.State != ResourceState.Starting)
        return false;
      try
      {
        resource.Start(manager.Configuration).Wait();
        return true;
      }
      catch (Exception ex)
      {
        RconPrint.Print("Error starting resource {0}: {1}.\n", (object) resourceName, (object) ex.Message);
        return false;
      }
    }

    [LuaMember("SetGameType", false)]
    private static void SetGameType_f(string gameType)
    {
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.GameType = gameType;
    }

    [LuaMember("SetMapName", false)]
    private static void SetMapName_f(string mapName)
    {
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.MapName = mapName;
    }
  }
}
