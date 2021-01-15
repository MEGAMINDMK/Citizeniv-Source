// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.RconScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using Neo.IronLua;
using System;

namespace CitizenMP.Server.Resources
{
  internal class RconScriptFunctions
  {
    [LuaMember("RconPrint", false)]
    private static void RconPrint_f(string str)
    {
      RconPrint.Print("{0}", (object) str);
    }

    [LuaMember("RconLog", false)]
    private static void RconLog_f(LuaTable table)
    {
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.RconLog?.Append(((Func<object, object, LuaResult>) ((LuaTable) ((LuaTable) ScriptEnvironment.CurrentEnvironment.LuaEnvironment).get_Item("json")).get_Item("encode"))((object) table, (object) null).get_Values()[0].ToString());
    }
  }
}
