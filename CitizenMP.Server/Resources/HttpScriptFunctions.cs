// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.HttpScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources
{
  internal class HttpScriptFunctions
  {
    [LuaMember("PerformHttpRequest", false)]
    private static async Task PerformHttpRequest(
      string url,
      Func<object, object, object, LuaResult> cb,
      string method = "GET",
      string data = "",
      LuaTable headers = null)
    {
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      int num1 = (^this).\u003C\u003E1__state;
      HttpWebRequest webRequest;
      ScriptEnvironment senv;
      try
      {
        webRequest = WebRequest.CreateHttp(url);
        webRequest.Method = method;
        senv = ScriptEnvironment.CurrentEnvironment;
        if (LuaTable.op_Inequality(headers, (object) null))
        {
          IEnumerator<KeyValuePair<object, object>> enumerator = headers.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<object, object> current = enumerator.Current;
              string name = current.Key.ToString();
              string str = current.Value.ToString();
              if (name.Equals("content-type", StringComparison.InvariantCultureIgnoreCase))
                webRequest.ContentType = str;
              else
                webRequest.Headers.Add(name, str);
            }
          }
          finally
          {
            if (num1 < 0 && enumerator != null)
              enumerator.Dispose();
          }
        }
        try
        {
          int num2;
          if (data != string.Empty)
          {
            TaskAwaiter<Stream> awaiter1 = webRequest.GetRequestStreamAsync().GetAwaiter();
            if (!awaiter1.IsCompleted)
            {
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003E1__state = num2 = 0;
              TaskAwaiter<Stream> taskAwaiter = awaiter1;
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<Stream>, HttpScriptFunctions.\u003CPerformHttpRequest\u003Ed__0>(ref awaiter1, this);
              return;
            }
            Stream result = awaiter1.GetResult();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] buffer = bytes;
            int length = bytes.Length;
            TaskAwaiter awaiter2 = result.WriteAsync(buffer, 0, length).GetAwaiter();
            if (!awaiter2.IsCompleted)
            {
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003E1__state = num2 = 1;
              TaskAwaiter taskAwaiter = awaiter2;
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter, HttpScriptFunctions.\u003CPerformHttpRequest\u003Ed__0>(ref awaiter2, this);
              return;
            }
            awaiter2.GetResult();
          }
          TaskAwaiter<WebResponse> awaiter3 = webRequest.GetResponseAsync().GetAwaiter();
          if (!awaiter3.IsCompleted)
          {
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003E1__state = num2 = 2;
            TaskAwaiter<WebResponse> taskAwaiter = awaiter3;
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<WebResponse>, HttpScriptFunctions.\u003CPerformHttpRequest\u003Ed__0>(ref awaiter3, this);
            return;
          }
          HttpWebResponse result1 = awaiter3.GetResult() as HttpWebResponse;
          LuaTable respHeaders = new LuaTable();
          for (int index = 0; index < result1.Headers.Count; ++index)
            respHeaders.set_Item(result1.Headers.Keys[index], (object) result1.Headers[index]);
          TaskAwaiter<string> awaiter4 = new StreamReader(result1.GetResponseStream()).ReadToEndAsync().GetAwaiter();
          if (!awaiter4.IsCompleted)
          {
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003E1__state = num2 = 3;
            TaskAwaiter<string> taskAwaiter = awaiter4;
            // ISSUE: explicit reference operation
            // ISSUE: reference to a compiler-generated field
            (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<string>, HttpScriptFunctions.\u003CPerformHttpRequest\u003Ed__0>(ref awaiter4, this);
            return;
          }
          string result2 = awaiter4.GetResult();
          try
          {
            ScriptEnvironment.PushedEnvironment pushedEnvironment = ScriptEnvironment.PushEnvironment(senv);
            LuaResult luaResult = cb((object) 0, (object) result2, (object) respHeaders);
            pushedEnvironment.PopEnvironment();
          }
          catch (Exception ex)
          {
            webRequest.Log<HttpWebRequest>(nameof (PerformHttpRequest), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\HttpScriptFunctions.cs", 75).Error("Error in callback for web request: {0}", (object) ex.Message);
            ScriptEnvironment.PrintLuaStackTrace(ex);
          }
          respHeaders = (LuaTable) null;
        }
        catch (WebException ex1)
        {
          webRequest.Log<HttpWebRequest>(nameof (PerformHttpRequest), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\HttpScriptFunctions.cs", 82).Warn("Web request to {0} failed: {1}", (object) url, (object) ex1.Message);
          try
          {
            ScriptEnvironment.PushedEnvironment pushedEnvironment = ScriptEnvironment.PushEnvironment(senv);
            LuaResult luaResult = cb((object) (int) ((HttpWebResponse) ex1.Response).StatusCode, (object) null, (object) null);
            pushedEnvironment.PopEnvironment();
          }
          catch (Exception ex2)
          {
            webRequest.Log<HttpWebRequest>(nameof (PerformHttpRequest), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\HttpScriptFunctions.cs", 92).Error("Error in callback for web request: {0}", (object) ex2.Message);
            ScriptEnvironment.PrintLuaStackTrace(ex2);
          }
        }
      }
      catch (Exception ex)
      {
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003E1__state = -2;
        webRequest = (HttpWebRequest) null;
        senv = (ScriptEnvironment) null;
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003Et__builder.SetException(ex);
        return;
      }
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003E1__state = -2;
      webRequest = (HttpWebRequest) null;
      senv = (ScriptEnvironment) null;
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003Et__builder.SetResult();
    }
  }
}
