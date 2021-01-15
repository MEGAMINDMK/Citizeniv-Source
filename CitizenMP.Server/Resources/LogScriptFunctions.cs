// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.LogScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CitizenMP.Server.Resources
{
  internal class LogScriptFunctions
  {
    [LuaMember("print", false)]
    private static void Print_f(params object[] arguments)
    {
      ScriptEnvironment.CurrentEnvironment.Log<ScriptEnvironment>("script print", "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\LogScriptFunctions.cs", 16).Info((Func<string>) (() => string.Join(" ", ((IEnumerable<object>) arguments).Select<object, object>((Func<object, object>) (a => a ?? (object) "null")).Select<object, string>((Func<object, string>) (a => a.ToString())))));
    }
  }
}
