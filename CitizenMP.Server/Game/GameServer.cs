// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Game.GameServer
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Commands;
using CitizenMP.Server.Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Game
{
  internal class GameServer
  {
    private ConcurrentDictionary<string, GameServer.BanDetails> m_bannedIdentifiers = new ConcurrentDictionary<string, GameServer.BanDetails>();
    private Dictionary<IPEndPoint, int> m_lastRconTimes = new Dictionary<IPEndPoint, int>();
    private Dictionary<uint, int> m_hostVotes = new Dictionary<uint, int>();
    private HashSet<string> m_whitelistedEvents = new HashSet<string>();
    private Queue<Action> m_mainCallbacks = new Queue<Action>();
    public const uint PROTOCOL_VERSION = 3;
    private Socket m_gameSocket;
    private Socket m_gameSocket6;
    private SocketAsyncEventArgs m_asyncEventArgs;
    private SocketAsyncEventArgs m_asyncEventArgs6;
    private byte[] m_receiveBuffer;
    private byte[] m_receiveBuffer6;
    private Client m_host;
    private ResourceManager m_resourceManager;
    private Configuration m_configuration;
    private IPEndPoint m_serverList;
    private int m_residualTime;
    private int m_serverTime;
    private int m_lastSenselessReliableSent;
    private int m_nextHeartbeatTime;

    public void BanIdentifier(string identifier, string reason)
    {
      GameServer.BanDetails details = new GameServer.BanDetails();
      details.Reason = reason;
      details.Expiration = Time.CurrentTime + 1800000L;
      this.m_bannedIdentifiers.AddOrUpdate(identifier, details, (Func<string, GameServer.BanDetails, GameServer.BanDetails>) ((key, oldValue) => details));
    }

    public bool IsIdentifierBanned(string identifier, out string reason)
    {
      reason = string.Empty;
      GameServer.BanDetails banDetails;
      if (!this.m_bannedIdentifiers.TryGetValue(identifier, out banDetails) || banDetails.Expiration < Time.CurrentTime)
        return false;
      reason = banDetails.Reason;
      return true;
    }

    public bool UseAsync { get; set; }

    public CommandManager CommandManager { get; private set; }

    public Configuration Configuration
    {
      get
      {
        return this.m_configuration;
      }
    }

    public ResourceManager ResourceManager
    {
      get
      {
        return this.m_resourceManager;
      }
    }

    public string GameType { get; set; }

    public string MapName { get; set; }

    public GameServer(
      Configuration config,
      ResourceManager resManager,
      CommandManager commandManager)
    {
      this.m_configuration = config;
      commandManager.SetGameServer(this);
      this.CommandManager = commandManager;
      this.m_resourceManager = resManager;
      this.m_resourceManager.SetGameServer(this);
      this.Log<GameServer>(".ctor", "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 74).Info("[Master List] Posting server to server list.");
      foreach (IPAddress address in Dns.GetHostEntry("89.163.208.198").AddressList)
      {
        if (address.AddressFamily == AddressFamily.InterNetwork)
        {
          this.m_serverList = new IPEndPoint(address, 30110);
          this.Log<GameServer>(".ctor", "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 83).Info("[Master List] Server posted to server list successfully!");
        }
      }
      this.UseAsync = true;
      this.Log<GameServer>(".ctor", "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 89).Info("");
    }

    public void Start()
    {
      this.Log<GameServer>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 94).Info("[Server] Starting game server on port {0}.", (object) this.m_configuration.ListenPort);
      this.m_gameSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      this.m_gameSocket.Bind((EndPoint) new IPEndPoint(IPAddress.Any, this.m_configuration.ListenPort));
      this.m_gameSocket.Blocking = false;
      try
      {
        this.m_gameSocket6 = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
        this.m_gameSocket6.Bind((EndPoint) new IPEndPoint(IPAddress.IPv6Any, this.m_configuration.ListenPort));
        this.m_gameSocket6.Blocking = false;
      }
      catch (Exception ex)
      {
        this.Log<GameServer>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 110).Error((Func<string>) (() => "Couldn't create IPv6 socket. Exception message: " + ex.Message), ex);
        this.m_gameSocket6 = (Socket) null;
      }
      this.m_receiveBuffer = new byte[2048];
      this.m_receiveBuffer6 = new byte[2048];
      if (!this.UseAsync)
        return;
      this.m_asyncEventArgs = this.CreateAsyncEventArgs(this.m_gameSocket, this.m_receiveBuffer);
      if (this.m_gameSocket6 == null)
        return;
      this.m_asyncEventArgs6 = this.CreateAsyncEventArgs(this.m_gameSocket6, this.m_receiveBuffer6);
    }

    private SocketAsyncEventArgs CreateAsyncEventArgs(
      Socket socket,
      byte[] receiveBuffer)
    {
      SocketAsyncEventArgs e = new SocketAsyncEventArgs();
      e.SetBuffer(receiveBuffer, 0, receiveBuffer.Length);
      e.RemoteEndPoint = (EndPoint) new IPEndPoint(socket.AddressFamily == AddressFamily.InterNetworkV6 ? IPAddress.IPv6None : IPAddress.None, 0);
      e.Completed += new EventHandler<SocketAsyncEventArgs>(this.m_asyncEventArgs_Completed);
      if (!socket.ReceiveFromAsync(e))
        this.m_asyncEventArgs_Completed((object) socket, e);
      return e;
    }

    private void ProcessOOB(IPEndPoint remoteEP, byte[] buffer, int length)
    {
      string commandText = Encoding.UTF8.GetString(buffer, 4, length - 4);
      string str = commandText.Split(' ')[0];
      if (str == "connect")
      {
        this.Log<GameServer>(nameof (ProcessOOB), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 153).Info("[Connect] Authorizing connection for {0}.", (object) remoteEP);
        this.ProcessConnectCommand(remoteEP, commandText);
      }
      else if (str == "rcon")
        this.ProcessRconCommand(remoteEP, commandText);
      else if (str == "getinfo")
      {
        this.ProcessGetInfoCommand(remoteEP, commandText);
      }
      else
      {
        if (!(str == "getstatus"))
          return;
        this.ProcessGetStatusCommand(remoteEP, commandText);
      }
    }

    private void ProcessGetStatusCommand(IPEndPoint remoteEP, string commandText)
    {
      string[] strArray = Utils.Tokenize(commandText);
      if (strArray.Length < 1)
        return;
      string text = "statusResponse\n" + this.GetServerInfoString(strArray.Length == 1 ? "" : strArray[1]) + "\n" + ClientInstances.Clients.Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (cl => cl.Value)).Where<Client>((Func<Client, bool>) (cl => cl.NetChannel != null)).Select<Client, string>((Func<Client, string>) (cl => "0 " + cl.Ping.ToString() + " \"" + cl.Name + "\"\n")).Aggregate<string, string>("", (Func<string, string, string>) ((a, b) => a + b));
      this.SendOutOfBand(remoteEP, text);
    }

    private void ProcessGetInfoCommand(IPEndPoint remoteEP, string commandText)
    {
      string[] strArray = Utils.Tokenize(commandText);
      if (strArray.Length < 2)
        return;
      this.SendOutOfBand(remoteEP, "infoResponse\n{0}", (object) this.GetServerInfoString(strArray[1]));
      this.m_nextHeartbeatTime = this.m_serverTime + 120000;
    }

    private string GetServerInfoString(string challenge)
    {
      string str = this.m_configuration.Password == null ? "false" : "true";
      return string.Format("\\sv_maxclients\\{0}\\clients\\{1}\\challenge\\{2}\\gamename\\{3}\\protocol\\2\\hostname\\{4}\\gametype\\{5}\\mapname\\{6}\\password\\{7}\\language\\{8}", (object) this.m_configuration.MaxPlayers, (object) ClientInstances.Clients.Count<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (cl => cl.Value.RemoteEP != null)), (object) challenge, (object) (this.m_configuration.Game ?? "GTA4"), (object) (this.m_configuration.Hostname ?? "CitizenMP"), (object) (this.GameType ?? ""), (object) (this.MapName ?? ""), (object) str, (object) (this.m_configuration.Language ?? "English"));
    }

    private void ProcessRconCommand(IPEndPoint remoteEP, string commandText)
    {
      string[] strArray = Utils.Tokenize(commandText);
      if (strArray.Length < 3 || this.m_lastRconTimes.ContainsKey(remoteEP) && this.m_serverTime < this.m_lastRconTimes[remoteEP] + 100)
        return;
      RconPrint.StartRedirect(this, remoteEP);
      if (this.m_configuration.RconPassword != null)
      {
        if (this.m_configuration.RconPassword != strArray[1])
        {
          RconPrint.Print("Invalid rcon password.\n");
          this.Log<GameServer>(nameof (ProcessRconCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 244).Warn("[Warn] Bad rcon from {0}", (object) remoteEP);
        }
        else
        {
          List<string> list = ((IEnumerable<string>) strArray).Skip<string>(3).ToList<string>();
          if (!this.CommandManager.HandleCommand(strArray[2], (IEnumerable<string>) list))
          {
            try
            {
              if (this.m_resourceManager.TriggerEvent("rconCommand", -1, (object) strArray[2], (object) list))
                RconPrint.Print("Unknown command: {0}\n", (object) strArray[2]);
            }
            catch (Exception ex)
            {
              this.Log<GameServer>(nameof (ProcessRconCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 261).Error((Func<string>) (() => "error handling rcon: " + ex.Message), ex);
              RconPrint.Print(ex.Message);
            }
          }
        }
      }
      else
        RconPrint.Print("No rcon password was set on this server.\n");
      RconPrint.EndRedirect();
    }

    private void ProcessConnectCommand(IPEndPoint remoteEP, string commandText)
    {
      Dictionary<string, string> arguments = Utils.ParseQueryString(commandText.Substring("connect ".Length));
      if (!arguments.ContainsKey("token") || !arguments.ContainsKey("guid"))
        return;
      KeyValuePair<string, Client> keyValuePair = ClientInstances.Clients.FirstOrDefault<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (cl => cl.Value.Guid == arguments["guid"] && cl.Value.Token == arguments["token"]));
      if (keyValuePair.Equals((object) new KeyValuePair<string, Client>()) || ClientInstances.Clients.Count<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (cl => cl.Value.RemoteEP != null)) >= this.Configuration.MaxPlayers)
        return;
      Client client1 = keyValuePair.Value;
      client1.Touch();
      client1.NetChannel = new NetChannel(client1);
      client1.NetID = ClientInstances.AssignNetID();
      client1.RemoteEP = remoteEP;
      client1.Socket = remoteEP.AddressFamily == AddressFamily.InterNetworkV6 ? this.m_gameSocket6 : this.m_gameSocket;
      this.SendOutOfBand(remoteEP, "connectOK {0} {1} {2}", (object) client1.NetID, (object) (this.m_host != null ? (int) this.m_host.NetID : -1), (object) (this.m_host != null ? this.m_host.Base : -1));
      this.m_nextHeartbeatTime = this.m_serverTime + 500;
      this.TriggerClientEvent("onPlayerJoining", -1, (object) client1.NetID, (object) client1.Name);
      this.Log<GameServer>(nameof (ProcessConnectCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 312).Info("[Connect] Authorization for {0} ({1}) complete.", (object) client1.RemoteEP, (object) client1.Name);
      foreach (KeyValuePair<string, Client> client2 in ClientInstances.Clients)
      {
        if ((int) client2.Value.NetID != (int) client1.NetID)
          this.TriggerClientEvent("onPlayerJoining", (int) client1.NetID, (object) client2.Value.NetID, (object) client2.Value.Name);
      }
    }

    public void SendOutOfBand(IPEndPoint remoteEP, string text, params object[] data)
    {
      byte[] bytes = Encoding.UTF8.GetBytes("    " + string.Format(text, data));
      bytes[0] = byte.MaxValue;
      bytes[1] = byte.MaxValue;
      bytes[2] = byte.MaxValue;
      bytes[3] = byte.MaxValue;
      try
      {
        if (remoteEP.AddressFamily == AddressFamily.InterNetworkV6)
          this.m_gameSocket6.SendTo(bytes, (EndPoint) remoteEP);
        else
          this.m_gameSocket.SendTo(bytes, (EndPoint) remoteEP);
      }
      catch (SocketException ex)
      {
      }
    }

    private void ProcessClientMessage(Client client, BinaryReader reader)
    {
      client.Touch();
      uint num1 = reader.ReadUInt32();
      if ((int) num1 != (int) client.OutReliableAcknowledged)
      {
        for (int index = client.OutReliableCommands.Count - 1; index >= 0; --index)
        {
          if (client.OutReliableCommands[index].ID <= num1)
            client.OutReliableCommands.RemoveAt(index);
        }
        client.OutReliableAcknowledged = num1;
      }
      if (client.ProtocolVersion >= 2U)
      {
        uint num2 = reader.ReadUInt32();
        if ((int) client.LastReceivedFrame != (int) num2)
        {
          client.Frames[(long) num2 % (long) client.Frames.Length].AckedTime = Time.CurrentTime;
          client.LastReceivedFrame = num2;
        }
      }
      try
      {
        while (true)
        {
          uint messageType = reader.ReadUInt32();
          switch (messageType)
          {
            case 3018469598:
              this.ProcessIHostMessage(client, reader);
              continue;
            case 3394674275:
              goto label_16;
            case 3912778843:
              this.ProcessRoutingMessage(client, reader);
              continue;
            default:
              this.ProcessReliableMessage(client, messageType, reader);
              continue;
          }
        }
label_16:;
      }
      catch (EndOfStreamException ex)
      {
        this.Log<GameServer>(nameof (ProcessClientMessage), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 408).Debug("[Debug] end of stream for client {0}", (object) client.NetID);
      }
    }

    private void ProcessRoutingMessage(Client client, BinaryReader reader)
    {
      client.SentData = true;
      ushort targetNetID = reader.ReadUInt16();
      ushort num = reader.ReadUInt16();
      byte[] buffer = reader.ReadBytes((int) num);
      if (buffer.Length != (int) num)
      {
        this.Log<GameServer>(nameof (ProcessRoutingMessage), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 426).Debug("[Debug] Incomplete routing packet.");
      }
      else
      {
        KeyValuePair<string, Client> keyValuePair = ClientInstances.Clients.FirstOrDefault<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => (int) c.Value.NetID == (int) targetNetID));
        if (keyValuePair.Equals((object) new KeyValuePair<string, Client>()))
        {
          this.Log<GameServer>(nameof (ProcessRoutingMessage), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 436).Debug("[Debug] no target netID {0}", (object) targetNetID);
        }
        else
        {
          Client client1 = keyValuePair.Value;
          lock (client1)
          {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter((Stream) memoryStream);
            client1.WriteReliableBuffer(writer);
            writer.Write(3912778843U);
            writer.Write(client.NetID);
            writer.Write(num);
            writer.Write(buffer);
            writer.Write(3394674275U);
            writer.Flush();
            client1.NetChannel.Send(memoryStream.ToArray());
          }
        }
      }
    }

    private void ProcessIHostMessage(Client client, BinaryReader reader)
    {
      int num = reader.ReadInt32();
      bool flag = false;
      if (this.m_host == null)
        flag = true;
      else if (Time.CurrentTime - this.m_host.LastSeen >= 15000L)
        flag = true;
      if (!flag)
        return;
      client.Base = num;
      this.SetNewHost(client);
    }

    private void SetNewHost(Client client)
    {
      this.m_host = client;
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      binaryWriter.Write(client.NetID);
      binaryWriter.Write(client.Base);
      foreach (KeyValuePair<string, Client> client1 in ClientInstances.Clients)
        client1.Value.SendReliableCommand(3018469598U, memoryStream.ToArray());
      this.m_hostVotes.Clear();
    }

    private void BeforeDropClient(Client client, string reason = "")
    {
      this.m_nextHeartbeatTime = this.m_serverTime + 500;
      if (client.NetChannel != null && reason != "Duplicate GUID")
        this.m_resourceManager.TriggerEvent("playerDropped", (int) client.NetID, (object) reason);
      if (this.m_host == null || (int) client.NetID != (int) this.m_host.NetID)
        return;
      this.m_host = (Client) null;
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      binaryWriter.Write(ushort.MaxValue);
      binaryWriter.Write((int) ushort.MaxValue);
      foreach (KeyValuePair<string, Client> client1 in ClientInstances.Clients)
        client1.Value.SendReliableCommand(3018469598U, memoryStream.ToArray());
    }

    public void DropClient(Client client, string reason)
    {
      byte[] bytes = ((IEnumerable<byte>) BitConverter.GetBytes(-1)).Concat<byte>((IEnumerable<byte>) Encoding.UTF8.GetBytes(string.Format("error {0}", (object) reason))).ToArray<byte>();
      client.SendRaw(bytes);
      Task.Delay(100).ContinueWith((Action<Task>) (task => client.SendRaw(bytes)));
      this.BeforeDropClient(client, reason);
      ClientInstances.RemoveClient(client);
    }

    private void HandleReliableCommand(
      Client client,
      uint messageType,
      BinaryReader reader,
      int size)
    {
      switch (messageType)
      {
        case 1378659793:
          this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 563).Info("[Quit] {0} ({1}) left the server (Disconnected).", (object) client.Name, (object) client.NetID);
          this.BeforeDropClient(client, "Disconnected");
          ClientInstances.RemoveClient(client);
          break;
        case 1933049210:
        case 4202130968:
          int targetNetID = messageType != 4202130968U ? (int) reader.ReadUInt16() : 0;
          ushort num1 = reader.ReadUInt16();
          string eventName = "";
          for (int index = 0; index < (int) num1 - 1; ++index)
            eventName += ((char) reader.ReadByte()).ToString();
          int num2 = (int) reader.ReadByte();
          int count = size - (int) num1 - 4;
          if (messageType == 1933049210U)
          {
            byte[] data = reader.ReadBytes(count);
            this.TriggerClientEvent(eventName, data, targetNetID, (int) client.NetID);
            break;
          }
          byte[] numArray = reader.ReadBytes(count + 2);
          if (!this.m_whitelistedEvents.Contains(eventName))
          {
            this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 664).Warn("[Warn] A client tried to send an event of type {0}, but it was not greenlit for client invocation. You may need to call RegisterServerEvent from your script.", (object) eventName);
            break;
          }
          StringBuilder dataSB = new StringBuilder(numArray.Length);
          foreach (byte num3 in numArray)
            dataSB.Append((char) num3);
          this.QueueCallback((Action) (() => this.m_resourceManager.TriggerEvent(eventName, dataSB.ToString(), (int) client.NetID)));
          break;
        case 2263480443:
          uint allegedNetID = reader.ReadUInt32();
          if (this.m_host != null && (int) allegedNetID == (int) this.m_host.NetID)
          {
            this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 580).Debug("[Debug] Got a late vote for {0}; they are our current host", (object) allegedNetID);
            break;
          }
          int num4 = reader.ReadInt32();
          int num5 = ClientInstances.Clients.Count<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.SentData));
          int num6 = num5 > 0 ? num5 / 3 + (num5 % 3 > 0 ? 1 : 0) : 0;
          int num7;
          int num8 = this.m_hostVotes.TryGetValue(allegedNetID, out num7) ? num7 + 1 : 1;
          this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 602).Debug("[Debug] Received a vote for {0}; current votes {1}, needed {2}", (object) allegedNetID, (object) num8, (object) num6);
          if (num8 >= num6)
          {
            KeyValuePair<string, Client> keyValuePair = ClientInstances.Clients.FirstOrDefault<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (a => (int) a.Value.NetID == (int) allegedNetID));
            if (keyValuePair.Equals((object) new KeyValuePair<string, Client>()))
            {
              this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 612).Debug("[Debug] The vote was rigged! Nobody is host! Bad politics!");
              break;
            }
            this.Log<GameServer>(nameof (HandleReliableCommand), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 617).Debug("[Debug] Net ID {0} won the election; they are the new host-elect.", (object) allegedNetID);
            keyValuePair.Value.Base = num4;
            this.SetNewHost(keyValuePair.Value);
            break;
          }
          this.m_hostVotes[allegedNetID] = num8;
          break;
      }
    }

    public void TriggerClientEvent(
      string eventName,
      string data,
      int targetNetID,
      int sourceNetID)
    {
      byte[] data1 = new byte[data.Length];
      int index = 0;
      foreach (char ch in data)
      {
        data1[index] = (byte) ch;
        ++index;
      }
      this.TriggerClientEvent(eventName, data1, targetNetID, sourceNetID);
    }

    public void TriggerClientEvent(string eventName, int targetNetID, params object[] arguments)
    {
      byte[] data = Utils.SerializeEvent(arguments);
      if (targetNetID >= 0)
      {
        this.TriggerClientEvent(eventName, data, targetNetID, -1);
      }
      else
      {
        foreach (KeyValuePair<string, Client> client in ClientInstances.Clients)
        {
          if (client.Value.NetChannel != null)
            this.TriggerClientEvent(eventName, data, (int) client.Value.NetID, -1);
        }
      }
    }

    public void TriggerClientEvent(
      string eventName,
      byte[] data,
      int targetNetID,
      int sourceNetID)
    {
      MemoryStream memoryStream = new MemoryStream();
      BinaryWriter binaryWriter = new BinaryWriter((Stream) memoryStream);
      binaryWriter.Write((ushort) sourceNetID);
      binaryWriter.Write((ushort) (eventName.Length + 1));
      for (int index = 0; index < eventName.Length; ++index)
        binaryWriter.Write((byte) eventName[index]);
      binaryWriter.Write((byte) 0);
      binaryWriter.Write(data);
      byte[] array = memoryStream.ToArray();
      if (targetNetID == (int) ushort.MaxValue || targetNetID == -1)
      {
        foreach (KeyValuePair<string, Client> client in ClientInstances.Clients)
          client.Value.SendReliableCommand(1933049210U, array);
      }
      else
        ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (a => (int) a.Value.NetID == targetNetID)).Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (a => a.Value)).FirstOrDefault<Client>()?.SendReliableCommand(1933049210U, array);
    }

    public void WhitelistEvent(string eventName)
    {
      if (this.m_whitelistedEvents.Contains(eventName))
        return;
      this.m_whitelistedEvents.Add(eventName);
    }

    private void ProcessReliableMessage(Client client, uint messageType, BinaryReader reader)
    {
      uint num = reader.ReadUInt32();
      int size;
      if (((int) num & int.MinValue) != 0)
      {
        size = reader.ReadInt32();
        num &= (uint) int.MaxValue;
      }
      else
        size = (int) reader.ReadInt16();
      long position = reader.BaseStream.Position;
      if (num > client.LastReceivedReliable)
      {
        this.HandleReliableCommand(client, messageType, reader, size);
        client.LastReceivedReliable = num;
      }
      reader.BaseStream.Position = position + (long) size;
    }

    private void ProcessIncomingPacket(byte[] buffer, int length, IPEndPoint remoteEP)
    {
      using (MemoryStream memoryStream = new MemoryStream(buffer))
      {
        BinaryReader reader = new BinaryReader((Stream) memoryStream);
        if (reader.ReadUInt32() == uint.MaxValue)
        {
          this.ProcessOOB(remoteEP, buffer, length);
        }
        else
        {
          KeyValuePair<string, Client> keyValuePair = ClientInstances.Clients.FirstOrDefault<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.RemoteEP != null && c.Value.RemoteEP.Equals((object) remoteEP)));
          if (keyValuePair.Equals((object) new KeyValuePair<string, Client>()))
          {
            this.Log<GameServer>(nameof (ProcessIncomingPacket), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 814).Debug("[Debug] Received a packet from an unknown source ({0})", (object) remoteEP);
          }
          else
          {
            Client client = keyValuePair.Value;
            if (!client.NetChannel.Process(buffer, length, ref reader))
              return;
            this.ProcessClientMessage(client, reader);
          }
        }
      }
    }

    private void m_asyncEventArgs_Completed(object sender, SocketAsyncEventArgs e)
    {
      try
      {
        if (e.SocketError == SocketError.Success)
        {
          if (e.BytesTransferred > 0)
            this.ProcessIncomingPacket(e.Buffer, e.BytesTransferred, (IPEndPoint) e.RemoteEndPoint);
        }
      }
      catch (Exception ex)
      {
        this.Log<GameServer>(nameof (m_asyncEventArgs_Completed), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 839).Error((Func<string>) (() => "incoming packet failed"), ex);
      }
      if (((Socket) sender).ReceiveFromAsync(e))
        return;
      this.m_asyncEventArgs_Completed(sender, e);
    }

    public int GetHostID()
    {
      return this.m_host == null ? -1 : (int) this.m_host.NetID;
    }

    private void QueueCallback(Action cb)
    {
      lock (this.m_mainCallbacks)
        this.m_mainCallbacks.Enqueue(cb);
    }

    public void Tick(int msec)
    {
      if (!this.UseAsync)
      {
        EndPoint receiveEP = (EndPoint) new IPEndPoint(IPAddress.None, 0);
        Action<Socket> action = (Action<Socket>) (socket =>
        {
          while (true)
          {
            try
            {
              int from = socket.ReceiveFrom(this.m_receiveBuffer, ref receiveEP);
              if (from <= 0)
                break;
              this.ProcessIncomingPacket(this.m_receiveBuffer, from, (IPEndPoint) receiveEP);
            }
            catch (SocketException ex)
            {
              if (ex.SocketErrorCode == SocketError.WouldBlock)
                break;
              this.Log<GameServer>(nameof (Tick), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 895).Warn("[Warn] socket error {0}", (object) ex.Message);
              break;
            }
          }
        });
        action(this.m_gameSocket);
        if (this.m_gameSocket6 != null)
          action(this.m_gameSocket6);
      }
      while (this.m_mainCallbacks.Count > 0)
      {
        Action action;
        lock (this.m_mainCallbacks)
          action = this.m_mainCallbacks.Dequeue();
        try
        {
          action();
        }
        catch (Exception ex)
        {
          this.Log<GameServer>(nameof (Tick), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 926).Error((Func<string>) (() => "Exception during queued callback: " + ex.Message), ex);
        }
      }
      this.m_residualTime += msec;
      while (this.m_residualTime > 50)
      {
        this.m_residualTime -= 50;
        this.m_serverTime += 50;
        this.ProcessServerFrame();
      }
    }

    private void SendHeartbeat()
    {
      this.SendOutOfBand(this.m_serverList, "heartbeat DarkPlaces\n");
    }

    private void ProcessServerFrame()
    {
      long currentTime = Time.CurrentTime;
      List<Client> clientList = new List<Client>();
      foreach (KeyValuePair<string, Client> client in ClientInstances.Clients)
      {
        int num = (client.Value.SentData ? 15 : 90) * 1000;
        if (currentTime - client.Value.LastSeen > (long) num)
        {
          if (client.Value.NetChannel != null)
            this.Log<GameServer>(nameof (ProcessServerFrame), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 970).Info("[Quit] {0} ({1}) left the server (Lost Connection).", (object) client.Value.Name, (object) client.Value.NetID);
          clientList.Add(client.Value);
        }
      }
      foreach (Client client in clientList)
      {
        this.BeforeDropClient(client, "Lost Connection");
        ClientInstances.RemoveClient(client);
      }
      if (this.m_serverTime > this.m_nextHeartbeatTime)
        this.SendHeartbeat();
      this.ResourceManager.Tick();
      foreach (KeyValuePair<string, Client> client1 in ClientInstances.Clients)
      {
        Client client2 = client1.Value;
        if (client2.NetChannel != null)
        {
          client1.Value.CalculatePing();
          lock (client2)
          {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter((Stream) memoryStream);
            client2.WriteReliableBuffer(writer);
            if (client2.ProtocolVersion >= 2U)
            {
              writer.Write(1409284671);
              writer.Write(client1.Value.FrameNumber);
              if (client2.ProtocolVersion >= 3U)
                writer.Write(client1.Value.Ping);
              client1.Value.Frames[(long) client1.Value.FrameNumber % (long) client1.Value.Frames.Length].SentTime = Time.CurrentTime;
              client1.Value.Frames[(long) client1.Value.FrameNumber % (long) client1.Value.Frames.Length].AckedTime = -1L;
              ++client1.Value.FrameNumber;
            }
            writer.Write(3394674275U);
            writer.Flush();
            client2.NetChannel.Send(memoryStream.ToArray());
          }
        }
      }
      long num1 = Time.CurrentTime - currentTime;
      if (num1 <= 25L)
        return;
      this.Log<GameServer>(nameof (ProcessServerFrame), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Game\\GameServer.cs", 1041).Info("[Info] Frame time warning: server frame took {0} msec.", (object) num1);
    }

    private struct BanDetails
    {
      public string Reason { get; set; }

      public long Expiration { get; set; }
    }
  }
}
