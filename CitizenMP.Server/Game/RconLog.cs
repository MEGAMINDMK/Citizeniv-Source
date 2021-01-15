// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Game.RconLog
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using uhttpsharp;

namespace CitizenMP.Server.Game
{
  internal class RconLog
  {
    private MemoryStream m_dataStream;
    private TextWriter m_textWriter;
    private long m_startTime;

    public RconLog()
    {
      this.m_startTime = Time.CurrentTime;
      this.m_dataStream = new MemoryStream();
      this.m_textWriter = (TextWriter) new StreamWriter((Stream) this.m_dataStream);
    }

    public void Append(string str)
    {
      JObject jobject = JObject.Parse(str);
      jobject.set_Item("msgTime", JToken.op_Implicit((int) (Time.CurrentTime - this.m_startTime)));
      this.m_textWriter.WriteLine(((JToken) jobject).ToString((Formatting) 0, new JsonConverter[0]));
      this.m_textWriter.Flush();
    }

    public void RunHttp(IHttpContext context)
    {
      Stream stream = (Stream) this.m_dataStream;
      string str;
      if (context.get_Request().get_Headers().TryGetByName("range", ref str) && str.StartsWith("bytes="))
      {
        string[] strArray = str.Substring(6).Split('-');
        int num1 = int.Parse(strArray[0]);
        int num2 = int.Parse(strArray[1]);
        stream = (Stream) new PartialStream((Stream) this.m_dataStream, (long) num1, (long) (num2 - num1));
      }
      context.set_Response((IHttpResponse) new HttpResponse((HttpResponseCode) 200, "text/plain", stream, true, false));
    }
  }
}
