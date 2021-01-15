// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.VMInitScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Neo.IronLua;
using System;

namespace CitizenMP.Server.Resources
{
  internal class VMInitScriptFunctions
  {
    [LuaMember("SetResourceInfo", false)]
    public static void SetResourceInfo_f(string key, string value)
    {
      ScriptEnvironment.InvokingEnvironment.Resource.Info[key] = value;
    }

    [LuaMember("AddClientScript", false)]
    public static void AddClientScript_f(string script)
    {
      ScriptEnvironment.InvokingEnvironment.Resource.Scripts.Add(script);
    }

    [LuaMember("AddServerScript", false)]
    public static void AddServerScript_f(string script)
    {
      ScriptEnvironment.CurrentEnvironment.AddServerScript(script);
    }

    [LuaMember("AddAuxFile", false)]
    public static void AddAuxFile_f(string file)
    {
      ScriptEnvironment.InvokingEnvironment.Resource.AuxFiles.Add(file);
    }

    [LuaMember("AddResourceDependency", false)]
    public static void AddResourceDependency_f(string resource)
    {
      ScriptEnvironment.InvokingEnvironment.Resource.Dependencies.Add(resource);
    }

    [LuaMember("RegisterInitHandler", false)]
    public static void RegisterInitHandler_f(Delegate function)
    {
      ScriptEnvironment.CurrentEnvironment.InitHandler = function;
    }
  }
}
