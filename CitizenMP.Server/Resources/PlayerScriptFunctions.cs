// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.PlayerScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CitizenMP.Server.Resources
{
  internal class PlayerScriptFunctions
  {
    [LuaMember("GetPlayers", false)]
    private static LuaTable GetPlayers()
    {
      ushort[] array = ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.NetChannel != null)).Select<KeyValuePair<string, Client>, ushort>((Func<KeyValuePair<string, Client>, ushort>) (c => c.Value.NetID)).ToArray<ushort>();
      LuaTable luaTable = new LuaTable();
      int num1 = 1;
      foreach (ushort num2 in array)
      {
        luaTable.set_Item(num1, (object) (int) num2);
        ++num1;
      }
      return luaTable;
    }

    [LuaMember("GetPlayerName", false)]
    private static string GetPlayerName(int source)
    {
      return PlayerScriptFunctions.FindPlayer(source)?.Name;
    }

    [LuaMember("SetPlayerName", false)]
    private static void SetPlayerName(int source, string name)
    {
      if (!(ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.Configuration.ForceVersion == "beta"))
        return;
      Client player = PlayerScriptFunctions.FindPlayer(source);
      if (player == null)
        return;
      player.Name = name;
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      binaryWriter.Write((ushort) source);
      binaryWriter.Write((ushort) (name.Length + 1));
      for (int index = 0; index < name.Length; ++index)
        binaryWriter.Write((byte) name[index]);
      binaryWriter.Write((byte) 0);
      byte[] array = memoryStream.ToArray();
      foreach (Client client in ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.NetChannel != null)).Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (c => c.Value)))
        client.SendReliableCommand(3349219472U, array);
    }

    [LuaMember("GetPlayerGuid", false)]
    private static string GetPlayerGuid(int source)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      if (player == null)
        return (string) null;
      string str = player.Identifiers.FirstOrDefault<string>();
      player.Log<Client>(nameof (GetPlayerGuid), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\PlayerScriptFunctions.cs", 91).Warn("Using GetPlayerGuid is deprecated - with NPv2 authentication a client can have multiple identifiers. Please use a 'in'-style match on GetPlayerIdentifiers()'s result instead.");
      if (str == null)
        str = player.Guid.PadLeft(16, '0');
      return str;
    }

    [LuaMember("GetPlayerIdentifiers", false)]
    private static LuaTable GetPlayerIdentifiers(int source)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      if (player == null)
        return (LuaTable) null;
      LuaTable luaTable = new LuaTable();
      int num = 1;
      foreach (string identifier in player.Identifiers)
      {
        luaTable.set_Item(num, (object) identifier);
        ++num;
      }
      return luaTable;
    }

    [LuaMember("GetPlayerPing", false)]
    private static int GetPlayerPing(int source)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      return player != null ? player.Ping : -1;
    }

    [LuaMember("GetPlayerEP", false)]
    private static string GetPlayerEP(int source)
    {
      return PlayerScriptFunctions.FindPlayer(source)?.RemoteEP.ToString();
    }

    [LuaMember("GetPlayerLastMsg", false)]
    private static double GetPlayerLastMsg_f(int source)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      return player != null ? (double) (Time.CurrentTime - player.LastSeen) : 99999999.0;
    }

    [LuaMember("GetHostId", false)]
    private static int GetHostId()
    {
      return ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.GetHostID();
    }

    [LuaMember("DropPlayer", false)]
    private static void DropPlayer(int source, string reason)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      if (player == null)
        return;
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.DropClient(player, reason);
    }

    [LuaMember("TempBanPlayer", false)]
    private static void TempBanPlayer(int source, string reason)
    {
      Client player = PlayerScriptFunctions.FindPlayer(source);
      if (player == null)
        return;
      GameServer gameServer = ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer;
      gameServer.DropClient(player, reason);
      foreach (string identifier in player.Identifiers)
        gameServer.BanIdentifier(identifier, reason);
    }

    private static Client FindPlayer(int source)
    {
      return ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (a => (int) a.Value.NetID == source)).Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (a => a.Value)).FirstOrDefault<Client>();
    }
  }
}
