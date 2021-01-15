// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Client
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace CitizenMP.Server
{
  public class Client
  {
    public string Name { get; set; }

    public string Guid { get; set; }

    public string Token { get; set; }

    public ushort NetID { get; set; }

    public uint ProtocolVersion { get; set; }

    public int Base { get; set; }

    public uint OutReliableSequence { get; set; }

    public uint OutReliableAcknowledged { get; set; }

    public uint LastReceivedReliable { get; set; }

    public List<OutReliableCommand> OutReliableCommands { get; set; }

    public IPEndPoint RemoteEP { get; set; }

    public NetChannel NetChannel { get; set; }

    public long LastSeen { get; private set; }

    public bool SentData { get; set; }

    public Socket Socket { get; set; }

    public uint FrameNumber { get; set; }

    public uint LastReceivedFrame { get; set; }

    public ClientFrame[] Frames { get; set; }

    public int Ping { get; private set; }

    public IEnumerable<string> Identifiers { get; set; }

    public Client()
    {
      this.OutReliableCommands = new List<OutReliableCommand>();
      this.ProtocolVersion = 1U;
      this.Frames = new ClientFrame[32];
    }

    public void Touch()
    {
      this.LastSeen = Time.CurrentTime;
    }

    public void CalculatePing()
    {
      int num1 = 0;
      int num2 = 0;
      for (int index = 0; index < this.Frames.Length; ++index)
      {
        if (this.Frames[index].AckedTime > 0L)
        {
          num1 += (int) (this.Frames[index].AckedTime - this.Frames[index].SentTime);
          ++num2;
        }
      }
      if (num2 == 0)
        this.Ping = -1;
      else
        this.Ping = num1 / num2;
    }

    public void WriteReliableBuffer(BinaryWriter writer)
    {
      writer.Write(this.LastReceivedReliable);
      lock (this.OutReliableCommands)
      {
        foreach (OutReliableCommand outReliableCommand in this.OutReliableCommands.GetRange(0, this.OutReliableCommands.Count))
        {
          if (outReliableCommand.Command != null)
          {
            writer.Write(outReliableCommand.Type);
            if (outReliableCommand.Command.Length > (int) ushort.MaxValue)
            {
              writer.Write(outReliableCommand.ID | 2147483648U);
              writer.Write(outReliableCommand.Command.Length);
            }
            else
            {
              writer.Write(outReliableCommand.ID);
              writer.Write((ushort) outReliableCommand.Command.Length);
            }
            writer.Write(outReliableCommand.Command);
          }
        }
      }
    }

    public void SendReliableCommand(uint commandType, byte[] commandData)
    {
      lock (this.OutReliableCommands)
        this.OutReliableCommands.Add(new OutReliableCommand()
        {
          ID = this.OutReliableSequence + 1U,
          Command = commandData,
          Type = commandType
        });
      ++this.OutReliableSequence;
    }

    public void SendRaw(byte[] buffer)
    {
      if (buffer.Length > 1400)
        this.Log<Client>(nameof (SendRaw), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Client.cs", 137).Error("THIS IS BAD");
      if (this.Socket == null)
        return;
      try
      {
        lock (this.Socket)
          this.Socket.SendTo(buffer, (EndPoint) this.RemoteEP);
      }
      catch (SocketException ex)
      {
      }
    }
  }
}
