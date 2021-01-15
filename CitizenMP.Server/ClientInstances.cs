// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.ClientInstances
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CitizenMP.Server
{
  internal static class ClientInstances
  {
    private static ConcurrentDictionary<string, Client> ms_clients = new ConcurrentDictionary<string, Client>();
    private static object ms_netIDLock = new object();
    private static ushort ms_curNetID;

    public static ReadOnlyDictionary<string, Client> Clients { get; private set; }

    static ClientInstances()
    {
      ClientInstances.Clients = new ReadOnlyDictionary<string, Client>((IDictionary<string, Client>) ClientInstances.ms_clients);
    }

    public static void AddClient(Client client)
    {
      ClientInstances.ms_clients[client.Guid] = client;
    }

    public static ushort AssignNetID()
    {
      lock (ClientInstances.ms_netIDLock)
      {
        for (bool flag = false; !flag; flag = !ClientInstances.Clients.Any<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => (int) c.Value.NetID == (int) ClientInstances.ms_curNetID)))
        {
          ++ClientInstances.ms_curNetID;
          if (ClientInstances.ms_curNetID == (ushort) 0 || ClientInstances.ms_curNetID == ushort.MaxValue)
            ClientInstances.ms_curNetID = (ushort) 1;
        }
        return ClientInstances.ms_curNetID;
      }
    }

    public static void RemoveClient(Client client)
    {
      ClientInstances.ms_clients.TryRemove(client.Guid, out Client _);
    }
  }
}
