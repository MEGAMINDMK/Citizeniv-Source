// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ScriptEnvironment
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using Microsoft.CSharp.RuntimeBinder;
using Neo.IronLua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CitizenMP.Server.Resources
{
  internal class ScriptEnvironment : IDisposable
  {
    private List<LuaChunk> m_curChunks = new List<LuaChunk>();
    private List<string> m_serverScripts = new List<string>();
    private Dictionary<int, Delegate> m_luaReferences = new Dictionary<int, Delegate>();
    private List<ScriptEnvironment.ScriptTimer> m_timers = new List<ScriptEnvironment.ScriptTimer>();
    private Dictionary<string, List<Delegate>> m_eventHandlers = new Dictionary<string, List<Delegate>>();
    private static List<KeyValuePair<string, MethodInfo>> ms_luaFunctions = new List<KeyValuePair<string, MethodInfo>>();
    private Resource m_resource;
    private LuaGlobal m_luaEnvironment;
    private static Lua ms_luaState;
    private static ILuaDebug ms_luaDebug;
    private static LuaCompileOptions ms_luaCompileOptions;
    [ThreadStatic]
    private static ScriptEnvironment ms_currentEnvironment;
    [ThreadStatic]
    private static ScriptEnvironment ms_lastEnvironment;
    [ThreadStatic]
    private static int refCount;
    private static Random ms_instanceGen;
    private static LuaChunk[] ms_initChunks;
    private int m_referenceNum;

    public static ScriptEnvironment CurrentEnvironment
    {
      get
      {
        return ScriptEnvironment.ms_currentEnvironment;
      }
    }

    public static ScriptEnvironment LastEnvironment
    {
      get
      {
        return ScriptEnvironment.ms_lastEnvironment;
      }
      private set
      {
        if (ScriptEnvironment.ms_lastEnvironment == null && value != null)
          ++ScriptEnvironment.refCount;
        else if (ScriptEnvironment.ms_lastEnvironment != null && value == null)
          --ScriptEnvironment.refCount;
        ScriptEnvironment.ms_lastEnvironment = value;
      }
    }

    public static ScriptEnvironment InvokingEnvironment
    {
      get
      {
        return ScriptEnvironment.CurrentEnvironment.Resource != null && ScriptEnvironment.CurrentEnvironment.Resource.State == ResourceState.Parsing ? ScriptEnvironment.CurrentEnvironment : ScriptEnvironment.LastEnvironment ?? ScriptEnvironment.CurrentEnvironment;
      }
    }

    public Resource Resource
    {
      get
      {
        return this.m_resource;
      }
    }

    public Lua LuaState
    {
      get
      {
        return ScriptEnvironment.ms_luaState;
      }
    }

    public LuaGlobal LuaEnvironment
    {
      get
      {
        return this.m_luaEnvironment;
      }
    }

    public uint InstanceID { get; set; }

    static ScriptEnvironment()
    {
      foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
      {
        foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
          LuaMemberAttribute customAttribute = method.GetCustomAttribute<LuaMemberAttribute>();
          if (customAttribute != null)
            ScriptEnvironment.ms_luaFunctions.Add(new KeyValuePair<string, MethodInfo>(customAttribute.get_LuaName(), method));
        }
      }
      ScriptEnvironment.ms_instanceGen = new Random();
      Extensions.Initialize();
    }

    public ScriptEnvironment(Resource resource)
    {
      this.m_resource = resource;
      this.InstanceID = (uint) ScriptEnvironment.ms_instanceGen.Next();
    }

    public bool Create()
    {
      ScriptEnvironment scriptEnvironment1 = (ScriptEnvironment) null;
      ScriptEnvironment scriptEnvironment2 = (ScriptEnvironment) null;
      try
      {
        if (ScriptEnvironment.ms_luaState == null)
        {
          ScriptEnvironment.ms_luaState = new Lua();
          ScriptEnvironment.ms_luaDebug = (ILuaDebug) null;
          if (this.Resource.Manager.Configuration.ScriptDebug)
            ScriptEnvironment.ms_luaDebug = (ILuaDebug) new LuaStackTraceDebugger();
          ScriptEnvironment.ms_luaCompileOptions = new LuaCompileOptions();
          ScriptEnvironment.ms_luaCompileOptions.set_DebugEngine(ScriptEnvironment.ms_luaDebug);
          ScriptEnvironment.ms_initChunks = new LuaChunk[3]
          {
            ScriptEnvironment.ms_luaState.CompileChunk("system/MessagePack.lua", ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0]),
            ScriptEnvironment.ms_luaState.CompileChunk("system/dkjson.lua", ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0]),
            ScriptEnvironment.ms_luaState.CompileChunk("system/resource_init.lua", ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0])
          };
        }
        this.m_luaEnvironment = (LuaGlobal) ScriptEnvironment.ms_luaState.CreateEnvironment<LuaGlobal>();
        foreach (KeyValuePair<string, MethodInfo> msLuaFunction in ScriptEnvironment.ms_luaFunctions)
        {
          Type[] array1 = ((IEnumerable<ParameterInfo>) msLuaFunction.Value.GetParameters()).Select<ParameterInfo, Type>((Func<ParameterInfo, Type>) (p => p.ParameterType)).Concat<Type>((IEnumerable<Type>) new Type[1]
          {
            msLuaFunction.Value.ReturnType
          }).ToArray<Type>();
          Type delegateType = Expression.GetDelegateType(array1);
          Delegate.CreateDelegate(delegateType, (object) null, msLuaFunction.Value);
          ParameterExpression[] array2 = ((IEnumerable<Type>) array1).Take<Type>(((IEnumerable<Type>) array1).Count<Type>() - 1).Select<Type, ParameterExpression>((Func<Type, ParameterExpression>) (a => Expression.Parameter(a))).ToArray<ParameterExpression>();
          ParameterExpression parameterExpression1 = Expression.Variable(typeof (ScriptEnvironment.PushedEnvironment));
          Expression<Func<ScriptEnvironment.PushedEnvironment>> expression1 = (Expression<Func<ScriptEnvironment.PushedEnvironment>>) (() => ScriptEnvironment.PushEnvironment(this));
          Expression<Action<ScriptEnvironment.PushedEnvironment>> expression2 = (Expression<Action<ScriptEnvironment.PushedEnvironment>>) (env => env.PopEnvironment());
          Expression body;
          if (msLuaFunction.Value.ReturnType.Name != "Void")
          {
            ParameterExpression parameterExpression2 = Expression.Variable(msLuaFunction.Value.ReturnType);
            body = (Expression) Expression.Block(msLuaFunction.Value.ReturnType, (IEnumerable<ParameterExpression>) new ParameterExpression[2]
            {
              parameterExpression2,
              parameterExpression1
            }, (Expression) Expression.Assign((Expression) parameterExpression1, (Expression) Expression.Invoke((Expression) expression1)), (Expression) Expression.Assign((Expression) parameterExpression2, (Expression) Expression.Call(msLuaFunction.Value, (Expression[]) array2)), (Expression) Expression.Invoke((Expression) expression2, (Expression) parameterExpression1), (Expression) parameterExpression2);
          }
          else
            body = (Expression) Expression.Block(msLuaFunction.Value.ReturnType, (IEnumerable<ParameterExpression>) new ParameterExpression[1]
            {
              parameterExpression1
            }, (Expression) Expression.Assign((Expression) parameterExpression1, (Expression) Expression.Invoke((Expression) expression1)), (Expression) Expression.Call(msLuaFunction.Value, (Expression[]) array2), (Expression) Expression.Invoke((Expression) expression2, (Expression) parameterExpression1));
          LambdaExpression lambdaExpression = Expression.Lambda(delegateType, body, array2);
          ((LuaTable) this.m_luaEnvironment).set_Item(msLuaFunction.Key, (object) lambdaExpression.Compile());
        }
        this.InitHandler = (Delegate) null;
        lock (this.m_luaEnvironment)
        {
          scriptEnvironment1 = ScriptEnvironment.ms_currentEnvironment;
          ScriptEnvironment.ms_currentEnvironment = this;
          scriptEnvironment2 = ScriptEnvironment.LastEnvironment;
          ScriptEnvironment.LastEnvironment = scriptEnvironment1;
          foreach (LuaChunk msInitChunk in ScriptEnvironment.ms_initChunks)
            this.m_luaEnvironment.DoChunk(msInitChunk, new object[0]);
        }
        return true;
      }
      catch (Exception ex)
      {
        this.Log<ScriptEnvironment>(nameof (Create), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 288).Error((Func<string>) (() => "Error creating script environment for resource " + this.m_resource.Name + ": " + ex.Message), ex);
        if (ex.InnerException != null)
          this.Log<ScriptEnvironment>(nameof (Create), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 292).Error((Func<string>) (() => "Inner exception: " + ex.InnerException.Message), ex.InnerException);
        ScriptEnvironment.PrintLuaStackTrace(ex);
      }
      finally
      {
        ScriptEnvironment.ms_currentEnvironment = scriptEnvironment1;
        ScriptEnvironment.LastEnvironment = scriptEnvironment2;
      }
      return false;
    }

    public void Dispose()
    {
      if (ScriptEnvironment.ms_currentEnvironment == this)
        throw new InvalidOperationException("Tried to dispose the current script environment");
      this.m_curChunks.Clear();
      GC.Collect();
    }

    internal static ScriptEnvironment.PushedEnvironment PushEnvironment(
      ScriptEnvironment env)
    {
      ScriptEnvironment.PushedEnvironment pushedEnvironment = new ScriptEnvironment.PushedEnvironment();
      pushedEnvironment.LastEnvironment = ScriptEnvironment.ms_currentEnvironment;
      ScriptEnvironment.ms_currentEnvironment = env;
      pushedEnvironment.OldLastEnvironment = ScriptEnvironment.LastEnvironment;
      ScriptEnvironment.LastEnvironment = pushedEnvironment.LastEnvironment;
      return pushedEnvironment;
    }

    public Delegate InitHandler { get; set; }

    public bool LoadScripts()
    {
      ScriptEnvironment scriptEnvironment1 = (ScriptEnvironment) null;
      ScriptEnvironment scriptEnvironment2 = (ScriptEnvironment) null;
      try
      {
        scriptEnvironment1 = ScriptEnvironment.ms_currentEnvironment;
        ScriptEnvironment.ms_currentEnvironment = this;
        scriptEnvironment2 = ScriptEnvironment.LastEnvironment;
        ScriptEnvironment.LastEnvironment = scriptEnvironment1;
        foreach (string serverScript in this.m_resource.ServerScripts)
        {
          lock (this.m_luaEnvironment)
          {
            LuaChunk luaChunk = ScriptEnvironment.ms_luaState.CompileChunk(Path.Combine(this.m_resource.Path, serverScript), ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0]);
            this.m_luaEnvironment.DoChunk(luaChunk, new object[0]);
            this.m_curChunks.Add(luaChunk);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.Log<ScriptEnvironment>(nameof (LoadScripts), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 407).Error((Func<string>) (() => "Error creating script environment for resource " + this.m_resource.Name + ": " + ex.Message), ex);
        if (ex.InnerException != null)
          this.Log<ScriptEnvironment>(nameof (LoadScripts), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 411).Error((Func<string>) (() => "Inner exception: " + ex.InnerException.Message), ex.InnerException);
        ScriptEnvironment.PrintLuaStackTrace(ex);
      }
      finally
      {
        ScriptEnvironment.ms_currentEnvironment = scriptEnvironment1;
        ScriptEnvironment.LastEnvironment = scriptEnvironment2;
      }
      return false;
    }

    public void AddServerScript(string script)
    {
      this.m_serverScripts.Add(script);
    }

    public bool DoInitFile(bool preParse)
    {
      ScriptEnvironment scriptEnvironment1 = (ScriptEnvironment) null;
      ScriptEnvironment scriptEnvironment2 = (ScriptEnvironment) null;
      try
      {
        scriptEnvironment1 = ScriptEnvironment.ms_currentEnvironment;
        ScriptEnvironment.ms_currentEnvironment = this;
        scriptEnvironment2 = ScriptEnvironment.LastEnvironment;
        ScriptEnvironment.LastEnvironment = scriptEnvironment1;
        lock (this.m_luaEnvironment)
        {
          LuaChunk initFunction = ScriptEnvironment.ms_luaState.CompileChunk(Path.Combine(this.m_resource.Path, "__resource.lua"), ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0]);
          this.InitHandler.DynamicInvoke((object) (Func<LuaResult>) (() => this.m_luaEnvironment.DoChunk(initFunction, new object[0])), (object) preParse);
        }
        if (!preParse)
        {
          foreach (string serverScript in this.m_serverScripts)
          {
            LuaChunk luaChunk = ScriptEnvironment.ms_luaState.CompileChunk(Path.Combine(this.m_resource.Path, serverScript), ScriptEnvironment.ms_luaCompileOptions, new KeyValuePair<string, Type>[0]);
            this.m_luaEnvironment.DoChunk(luaChunk, new object[0]);
            this.m_curChunks.Add(luaChunk);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.Log<ScriptEnvironment>(nameof (DoInitFile), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 466).Error((Func<string>) (() => "Error creating script environment for resource " + this.m_resource.Name + ": " + ex.Message), ex);
        if (ex.InnerException != null)
          this.Log<ScriptEnvironment>(nameof (DoInitFile), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 470).Error((Func<string>) (() => "Inner exception: " + ex.InnerException.Message), ex.InnerException);
        ScriptEnvironment.PrintLuaStackTrace(ex);
      }
      finally
      {
        ScriptEnvironment.ms_currentEnvironment = scriptEnvironment1;
        ScriptEnvironment.LastEnvironment = scriptEnvironment2;
      }
      return false;
    }

    public void TriggerEvent(string eventName, string argsSerialized, int source)
    {
      List<Delegate> delegateList;
      if (!this.m_eventHandlers.TryGetValue(eventName, out delegateList))
        return;
      ((LuaTable) this.m_luaEnvironment).set_Item(nameof (source), (object) source);
      ScriptEnvironment currentEnvironment = ScriptEnvironment.ms_currentEnvironment;
      ScriptEnvironment.ms_currentEnvironment = this;
      ScriptEnvironment lastEnvironment = ScriptEnvironment.LastEnvironment;
      ScriptEnvironment.LastEnvironment = currentEnvironment;
      object luaEnvironment = (object) this.m_luaEnvironment;
      // ISSUE: reference to a compiler-generated field
      if (ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__2 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, LuaTable>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.Convert(CSharpBinderFlags.None, typeof (LuaTable), typeof (ScriptEnvironment)));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, LuaTable> target1 = ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__2.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, LuaTable>> p2 = ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__2;
      // ISSUE: reference to a compiler-generated field
      if (ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, string, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.InvokeMember(CSharpBinderFlags.None, "unpack", (IEnumerable<Type>) null, typeof (ScriptEnvironment), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, string, object> target2 = ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, string, object>> p1 = ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, "msgpack", typeof (ScriptEnvironment), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__0.Target((CallSite) ScriptEnvironment.\u003C\u003Eo__43.\u003C\u003Ep__0, luaEnvironment);
      string str = argsSerialized;
      object obj2 = target2((CallSite) p1, obj1, str);
      LuaTable luaTable = target1((CallSite) p2, obj2);
      object[] objArray1 = new object[0];
      if (LuaTable.op_Inequality(luaTable, (object) null))
      {
        objArray1 = new object[luaTable.get_Length()];
        int index = 0;
        using (IEnumerator<KeyValuePair<object, object>> enumerator = luaTable.GetEnumerator())
        {
          while (enumerator.MoveNext())
          {
            KeyValuePair<object, object> current = enumerator.Current;
            objArray1[index] = current.Value;
            ++index;
          }
        }
      }
      foreach (Delegate @delegate in delegateList)
      {
        try
        {
          ParameterInfo[] parameters = @delegate.Method.GetParameters();
          object[] objArray2 = objArray1;
          int num = 0;
          if (parameters.Length >= 1 && (((IEnumerable<ParameterInfo>) parameters).Last<ParameterInfo>().ParameterType == typeof (LuaTable) || ((IEnumerable<ParameterInfo>) parameters).First<ParameterInfo>().ParameterType == typeof (Closure)))
            num = 1;
          object[] array = ((IEnumerable<object>) objArray2).Take<object>(parameters.Length - num).ToArray<object>();
          @delegate.DynamicInvoke(array);
        }
        catch (Exception ex)
        {
          Exception e = ex;
          RconPrint.Print("Error in resource {0}: {1}\n", (object) this.m_resource.Name, (object) e.Message);
          this.Log<ScriptEnvironment>(nameof (TriggerEvent), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 542).Error((Func<string>) (() => "Error executing event handler for event " + eventName + " in resource " + this.m_resource.Name + ": " + e.Message), e);
          ScriptEnvironment.PrintLuaStackTrace(e);
          for (; e != null; e = e.InnerException)
          {
            if (e.InnerException != null)
            {
              this.Log<ScriptEnvironment>(nameof (TriggerEvent), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 550).Error((Func<string>) (() => "Inner exception: " + e.InnerException.Message), e.InnerException);
              ScriptEnvironment.PrintLuaStackTrace(e.InnerException);
            }
          }
          ScriptEnvironment.ms_currentEnvironment = currentEnvironment;
          ScriptEnvironment.LastEnvironment = lastEnvironment;
          return;
        }
      }
      ScriptEnvironment.ms_currentEnvironment = currentEnvironment;
      ScriptEnvironment.LastEnvironment = lastEnvironment;
    }

    public Delegate GetRef(int reference)
    {
      return this.m_luaReferences[reference];
    }

    public string CallExport(Delegate func, string argsSerialized)
    {
      lock (this.m_luaEnvironment)
      {
        ScriptEnvironment currentEnvironment = ScriptEnvironment.ms_currentEnvironment;
        ScriptEnvironment.ms_currentEnvironment = this;
        ScriptEnvironment lastEnvironment = ScriptEnvironment.LastEnvironment;
        ScriptEnvironment.LastEnvironment = currentEnvironment;
        string str = "";
        try
        {
          LuaTable luaTable = ((Func<string, LuaTable>) ((LuaTable) ((LuaTable) this.m_luaEnvironment).get_Item("msgpack")).get_Item("unpack"))(argsSerialized);
          object[] objArray = new object[luaTable.get_Length()];
          int index = 0;
          using (IEnumerator<KeyValuePair<object, object>> enumerator = luaTable.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              KeyValuePair<object, object> current = enumerator.Current;
              objArray[index] = current.Value;
              ++index;
            }
          }
          str = EventScriptFunctions.SerializeArguments((LuaResult) func.DynamicInvoke(((IEnumerable<object>) objArray).Take<object>(func.Method.GetParameters().Length - 1).ToArray<object>()));
        }
        catch (Exception ex)
        {
          this.Log<ScriptEnvironment>(nameof (CallExport), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 616).Error((Func<string>) (() => "Error invoking reference for resource " + this.m_resource.Name + ": " + ex.Message), ex);
          if (ex.InnerException != null)
            this.Log<ScriptEnvironment>(nameof (CallExport), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 620).Error((Func<string>) (() => "Inner exception: " + ex.InnerException.Message), ex.InnerException);
          ScriptEnvironment.PrintLuaStackTrace(ex);
        }
        ScriptEnvironment.ms_currentEnvironment = currentEnvironment;
        ScriptEnvironment.LastEnvironment = lastEnvironment;
        return str;
      }
    }

    internal static void PrintLuaStackTrace(Exception e)
    {
      LuaExceptionData data = LuaExceptionData.GetData(e, true);
      if (data == null)
        return;
      using (IEnumerator<LuaStackFrame> enumerator = data.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          LuaStackFrame current = enumerator.Current;
          e.Log<Exception>(nameof (PrintLuaStackTrace), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 641).Error(((object) current).ToString());
        }
      }
    }

    public int AddRef(Delegate method)
    {
      int key = this.m_referenceNum++;
      this.m_luaReferences.Add(key, method);
      return key;
    }

    public bool HasRef(int reference)
    {
      return this.m_luaReferences.ContainsKey(reference);
    }

    public void RemoveRef(int reference)
    {
      this.m_luaReferences.Remove(reference);
    }

    public void Tick()
    {
      List<ScriptEnvironment.ScriptTimer> range = this.m_timers.GetRange(0, this.m_timers.Count);
      long currentTime = Time.CurrentTime;
      foreach (ScriptEnvironment.ScriptTimer scriptTimer in range)
      {
        if (currentTime >= scriptTimer.TickFrom)
        {
          lock (this.m_luaEnvironment)
          {
            ScriptEnvironment currentEnvironment = ScriptEnvironment.ms_currentEnvironment;
            ScriptEnvironment.ms_currentEnvironment = this;
            ScriptEnvironment lastEnvironment = ScriptEnvironment.LastEnvironment;
            ScriptEnvironment.LastEnvironment = currentEnvironment;
            try
            {
              scriptTimer.Function.DynamicInvoke();
            }
            catch (Exception ex)
            {
              this.Log<ScriptEnvironment>(nameof (Tick), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 696).Error((Func<string>) (() => "Error invoking timer in resource " + this.m_resource.Name + ": " + ex.Message), ex);
              ScriptEnvironment.PrintLuaStackTrace(ex);
              if (ex.InnerException != null)
              {
                this.Log<ScriptEnvironment>(nameof (Tick), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ScriptEnvironment.cs", 702).Error((Func<string>) (() => "Inner exception: " + ex.InnerException.Message), ex.InnerException);
                ScriptEnvironment.PrintLuaStackTrace(ex.InnerException);
              }
            }
            ScriptEnvironment.ms_currentEnvironment = currentEnvironment;
            ScriptEnvironment.LastEnvironment = lastEnvironment;
            this.m_timers.Remove(scriptTimer);
          }
        }
      }
    }

    public void SetTimeout(int milliseconds, Delegate callback)
    {
      long num = Time.CurrentTime + (long) milliseconds;
      this.m_timers.Add(new ScriptEnvironment.ScriptTimer()
      {
        TickFrom = num,
        Function = callback
      });
    }

    [LuaMember("SetTimeout", false)]
    private static void SetTimeout_f(int milliseconds, Delegate callback)
    {
      ScriptEnvironment.ms_currentEnvironment.SetTimeout(milliseconds, callback);
    }

    [LuaMember("AddEventHandler", false)]
    private static void AddEventHandler_f(string eventName, Delegate eventHandler)
    {
      ScriptEnvironment.ms_currentEnvironment.AddEventHandler(eventName, eventHandler);
    }

    [LuaMember("GetInstanceId", false)]
    private static int GetInstanceId_f()
    {
      return (int) ScriptEnvironment.ms_currentEnvironment.InstanceID;
    }

    public void AddEventHandler(string eventName, Delegate eventHandler)
    {
      if (!this.m_eventHandlers.ContainsKey(eventName))
        this.m_eventHandlers[eventName] = new List<Delegate>();
      this.m_eventHandlers[eventName].Add(eventHandler);
    }

    internal class PushedEnvironment
    {
      public ScriptEnvironment LastEnvironment { get; set; }

      public ScriptEnvironment OldLastEnvironment { get; set; }

      public void PopEnvironment()
      {
        ScriptEnvironment.ms_currentEnvironment = this.LastEnvironment;
        ScriptEnvironment.LastEnvironment = this.OldLastEnvironment;
      }
    }

    private class ScriptTimer
    {
      public Delegate Function { get; set; }

      public long TickFrom { get; set; }
    }
  }
}
