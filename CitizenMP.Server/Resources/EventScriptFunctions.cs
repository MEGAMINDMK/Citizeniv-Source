// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.EventScriptFunctions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Neo.IronLua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CitizenMP.Server.Resources
{
  internal class EventScriptFunctions
  {
    [LuaMember("CancelEvent", false)]
    private static void CancelEvent_f()
    {
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.CancelEvent();
    }

    [LuaMember("WasEventCanceled", false)]
    private static bool WasEventCanceled_f()
    {
      return ScriptEnvironment.CurrentEnvironment.Resource.Manager.WasEventCanceled();
    }

    [LuaMember("TriggerEvent", false)]
    private static bool TriggerEvent_f(string eventName, params object[] args)
    {
      string argsSerialized = EventScriptFunctions.SerializeArguments(args);
      return ScriptEnvironment.CurrentEnvironment.Resource.Manager.TriggerEvent(eventName, argsSerialized, -1);
    }

    [LuaMember("TriggerClientEvent", false)]
    private static void TriggerClientEvent_f(string eventName, int netID, params object[] args)
    {
      string data = EventScriptFunctions.SerializeArguments(args);
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.TriggerClientEvent(eventName, data, netID, (int) ushort.MaxValue);
    }

    [LuaMember("RegisterServerEvent", false)]
    private static void RegisterServerEvent_f(string eventName)
    {
      ScriptEnvironment.CurrentEnvironment.Resource.Manager.GameServer.WhitelistEvent(eventName);
    }

    [LuaMember("GetFuncRef", false)]
    private static void GetFuncRef_f(
      Delegate func,
      out int reference,
      out int instance,
      out string resource)
    {
      reference = ScriptEnvironment.CurrentEnvironment.AddRef(func);
      instance = (int) ScriptEnvironment.CurrentEnvironment.InstanceID;
      resource = ScriptEnvironment.CurrentEnvironment.Resource.Name;
    }

    private static ICallRefHandler GetCallRefHandler(
      int reference,
      uint instance,
      string resourceName)
    {
      ICallRefHandler callRefHandler = !(resourceName == "__internal") ? (ICallRefHandler) ScriptEnvironment.CurrentEnvironment.Resource.Manager.GetResource(resourceName) : (ICallRefHandler) InternalCallRefHandler.Get();
      if (callRefHandler == null)
        throw new ArgumentException("Invalid resource name.");
      if (callRefHandler is Resource)
      {
        switch (((Resource) callRefHandler).State)
        {
          case ResourceState.Starting:
          case ResourceState.Running:
          case ResourceState.Parsing:
            break;
          default:
            throw new ArgumentException("Resource wasn't running.");
        }
      }
      return callRefHandler;
    }

    private static ICallRefHandler ValidateResourceAndRef(
      int reference,
      uint instance,
      string resourceName)
    {
      ICallRefHandler callRefHandler = EventScriptFunctions.GetCallRefHandler(reference, instance, resourceName);
      if (callRefHandler == null)
        return (ICallRefHandler) null;
      return !callRefHandler.HasRef(reference, instance) ? (ICallRefHandler) null : callRefHandler;
    }

    [LuaMember("GetFuncFromRef", false)]
    private static LuaTable GetFuncFromRef_f(
      int reference,
      uint instance,
      string resourceName)
    {
      ICallRefHandler callRefHandler = EventScriptFunctions.ValidateResourceAndRef(reference, instance, resourceName);
      LuaTable luaTable1 = new LuaTable();
      Delegate func = callRefHandler.GetRef(reference);
      luaTable1.set_Item("__call", (object) (EventScriptFunctions.CallDelegate) (args =>
      {
        if (EventScriptFunctions.ValidateResourceAndRef(reference, instance, resourceName) == null)
          return (object) null;
        ParameterInfo[] parameters = func.Method.GetParameters();
        IEnumerable<object> source = ((IEnumerable<object>) args).Skip<object>(1);
        int num = 0;
        if (parameters.Length >= 1 && (((IEnumerable<ParameterInfo>) parameters).Last<ParameterInfo>().ParameterType == typeof (LuaTable) || ((IEnumerable<ParameterInfo>) parameters).First<ParameterInfo>().ParameterType == typeof (Closure)))
          num = 1;
        return func.DynamicInvoke(((IEnumerable<object>) source.Take<object>(parameters.Length - num).ToArray<object>()).ToArray<object>());
      }));
      LuaTable luaTable2 = new LuaTable();
      luaTable2.set_Item("__reference", (object) reference);
      luaTable2.set_Item("__instance", (object) instance);
      luaTable2.set_Item("__resource", (object) resourceName);
      luaTable2.set_MetaTable(luaTable1);
      return luaTable2;
    }

    public static string SerializeArguments(object[] args)
    {
      if (args == null || args.Length == 0)
        return "À";
      LuaTable luaTable = new LuaTable();
      for (int index = 1; index <= args.Length; ++index)
        luaTable.set_Item(index, args[index - 1]);
      return ((Func<object, LuaResult>) ((LuaTable) ((LuaTable) ScriptEnvironment.CurrentEnvironment.LuaEnvironment).get_Item("msgpack")).get_Item("pack"))((object) luaTable).ToString((IFormatProvider) null);
    }

    public static string SerializeArguments(LuaResult args)
    {
      if (args == null)
        return "À";
      LuaTable luaTable = new LuaTable();
      for (int index = 0; index < args.get_Count(); ++index)
        luaTable.set_Item(index, args.get_Item(index));
      return ((Func<LuaTable, string>) ((LuaTable) ((LuaTable) ScriptEnvironment.CurrentEnvironment.LuaEnvironment).get_Item("msgpack")).get_Item("pack"))(luaTable);
    }

    [LuaMember("ldexp", false)]
    public static double ldexp(double x, int exp)
    {
      return x * Math.Pow(2.0, (double) exp);
    }

    [LuaMember("frexp", false)]
    public static void frexp(double x, out double fr, out int exp)
    {
      exp = (int) Math.Floor(Math.Log(x) / Math.Log(2.0)) + 1;
      fr = 1.0 - (Math.Pow(2.0, (double) exp) - x) / Math.Pow(2.0, (double) exp);
    }

    private delegate object CallDelegate(params object[] args);
  }
}
