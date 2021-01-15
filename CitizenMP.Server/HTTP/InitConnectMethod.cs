// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.HTTP.InitConnectMethod
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Headers;

namespace CitizenMP.Server.HTTP
{
  internal static class InitConnectMethod
  {
    public static Func<IHttpHeaders, IHttpContext, Task<JObject>> Get(
      Configuration config,
      GameServer gameServer)
    {
      return (Func<IHttpHeaders, IHttpContext, Task<JObject>>) (async (headers, context) =>
      {
        JObject jobject = new JObject();
        string byName1 = headers.GetByName("name");
        string byName2 = headers.GetByName("guid");
        string s = (string) null;
        headers.TryGetByName("protocol", ref s);
        string str = (string) null;
        if (!context.get_Request().get_Headers().TryGetByName("user-agent", ref str) || !str.Equals("CitizenFX/1"))
          return jobject;
        if (string.IsNullOrEmpty(byName1) || string.IsNullOrEmpty(byName2))
        {
          jobject.set_Item("error", JToken.op_Implicit("fields missing"));
          return jobject;
        }
        if (string.IsNullOrEmpty(s))
          s = "1";
        uint result;
        if (!uint.TryParse(s, out result))
        {
          jobject.set_Item("error", JToken.op_Implicit("invalid protocol version"));
          return jobject;
        }
        if (config.Imports != null && config.Imports.Count > 0 && result < 2U)
        {
          jobject.set_Item("error", JToken.op_Implicit("Your client is too old to support imported resources. Please update to a cleanliness client or higher."));
          return jobject;
        }
        if (ClientInstances.Clients.Count<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (cl => cl.Value.RemoteEP != null)) >= gameServer.Configuration.MaxPlayers)
        {
          jobject.set_Item("error", JToken.op_Implicit("The server is full."));
          return jobject;
        }
        IEnumerable<string> strings = (IEnumerable<string>) new string[1]
        {
          string.Format("ip:{0}", (object) ((IPEndPoint) context.get_RemoteEndPoint()).Address)
        };
        foreach (string identifier in strings)
        {
          string reason1;
          if (gameServer.IsIdentifierBanned(identifier, out reason1))
          {
            jobject.set_Item("error", JToken.op_Implicit("You have been banned from this server. Stated reason: " + reason1));
            return jobject;
          }
        }
        Client client = new Client();
        client.Token = TokenGenerator.GenerateToken();
        client.Name = byName1;
        client.Guid = byName2;
        client.Identifiers = strings;
        client.ProtocolVersion = result;
        client.Touch();
        if (ClientInstances.Clients.ContainsKey(byName2))
          gameServer.DropClient(ClientInstances.Clients[byName2], "Duplicate GUID");
        ClientInstances.AddClient(client);
        string reason = "Resource prevented connection.";
        Action<string> action = (Action<string>) (reasonString => reason = reasonString);
        if (!gameServer.ResourceManager.TriggerEvent("playerConnecting", (int) client.NetID, (object) client.Name, (object) action))
        {
          ClientInstances.RemoveClient(client);
          jobject.set_Item("error", JToken.op_Implicit(reason));
          return jobject;
        }
        jobject.set_Item("token", JToken.op_Implicit(client.Token));
        jobject.set_Item("protocol", JToken.op_Implicit(3U));
        jobject.set_Item("version", JToken.op_Implicit(gameServer.Configuration.cleanServerVersion));
        jobject.set_Item("sH", JToken.op_Implicit(gameServer.Configuration.ScriptHookAllowed));
        jobject.set_Item("password", JToken.op_Implicit(gameServer.Configuration.Password));
        jobject.set_Item("checkGame", JToken.op_Implicit(gameServer.Configuration.CheckGameFiles));
        jobject.set_Item("forceVersion", JToken.op_Implicit(gameServer.Configuration.ForceVersion));
        jobject.set_Item("encryption", JToken.op_Implicit(gameServer.Configuration.Encryption));
        return jobject;
      });
    }
  }
}
