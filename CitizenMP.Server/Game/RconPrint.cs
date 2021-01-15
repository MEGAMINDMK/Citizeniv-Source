// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Game.RconPrint
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Net;
using System.Text;

namespace CitizenMP.Server.Game
{
  internal class RconPrint
  {
    [ThreadStatic]
    private static StringBuilder ms_outBuffer;
    [ThreadStatic]
    private static IPEndPoint ms_endPoint;
    [ThreadStatic]
    private static GameServer ms_gameServer;

    public static void StartRedirect(GameServer gs, IPEndPoint ep)
    {
      RconPrint.ms_outBuffer = new StringBuilder();
      RconPrint.ms_endPoint = ep;
      RconPrint.ms_gameServer = gs;
    }

    public static void Print(string str, params object[] args)
    {
      if (RconPrint.ms_outBuffer == null)
        return;
      if (RconPrint.ms_outBuffer.Length + str.Length > 1000)
        RconPrint.Flush();
      RconPrint.ms_outBuffer.AppendFormat(str, args);
    }

    public static void EndRedirect()
    {
      RconPrint.Flush();
      RconPrint.ms_outBuffer = (StringBuilder) null;
    }

    private static void Flush()
    {
      RconPrint.ms_gameServer.SendOutOfBand(RconPrint.ms_endPoint, "print\n{0}", (object) RconPrint.ms_outBuffer.ToString());
      RconPrint.ms_outBuffer.Clear();
    }
  }
}
