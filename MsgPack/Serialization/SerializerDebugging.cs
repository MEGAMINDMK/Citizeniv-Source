// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerDebugging
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace MsgPack.Serialization
{
  internal static class SerializerDebugging
  {
    private const string HistoryFile = "MsgPack.Serialization.SerializationGenerationDebugging.CodeDOM.History.txt";
    [ThreadStatic]
    private static bool _traceEnabled;
    [ThreadStatic]
    private static bool _dumpEnabled;
    [ThreadStatic]
    private static bool _avoidsGenericSerializer;
    [ThreadStatic]
    private static StringWriter _ilTraceWriter;
    [ThreadStatic]
    private static AssemblyBuilder _assemblyBuilder;
    [ThreadStatic]
    private static ModuleBuilder _moduleBuilder;
    [ThreadStatic]
    private static IList<string> _runtimeAssemblies;
    [ThreadStatic]
    private static IList<string> _compiledCodeDomSerializerAssemblies;
    private static int _wasDeleted;
    [ThreadStatic]
    private static bool _onTheFlyCodeDomEnabled;

    public static bool TraceEnabled
    {
      get
      {
        return SerializerDebugging._traceEnabled;
      }
      set
      {
        SerializerDebugging._traceEnabled = value;
      }
    }

    public static bool DumpEnabled
    {
      get
      {
        return SerializerDebugging._dumpEnabled;
      }
      set
      {
        SerializerDebugging._dumpEnabled = value;
      }
    }

    public static bool AvoidsGenericSerializer
    {
      get
      {
        return SerializerDebugging._avoidsGenericSerializer;
      }
      set
      {
        SerializerDebugging._avoidsGenericSerializer = value;
      }
    }

    public static TextWriter ILTraceWriter
    {
      get
      {
        if (!SerializerDebugging._traceEnabled)
          return TextWriter.Null;
        if (SerializerDebugging._ilTraceWriter == null)
          SerializerDebugging._ilTraceWriter = new StringWriter((IFormatProvider) CultureInfo.InvariantCulture);
        return (TextWriter) SerializerDebugging._ilTraceWriter;
      }
    }

    public static void TraceEvent(string format, params object[] args)
    {
      if (!SerializerDebugging._traceEnabled)
        return;
      Tracer.Emit.TraceEvent(TraceEventType.Verbose, 102, format, args);
    }

    public static void FlushTraceData()
    {
      if (!SerializerDebugging._traceEnabled)
        return;
      Tracer.Emit.TraceData(TraceEventType.Verbose, 102, (object) SerializerDebugging._ilTraceWriter.ToString());
    }

    public static void PrepareDump(AssemblyBuilder assemblyBuilder)
    {
      if (!SerializerDebugging._dumpEnabled)
        return;
      SerializerDebugging._assemblyBuilder = assemblyBuilder;
    }

    public static void PrepareDump()
    {
      SerializerDebugging._assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("ExpressionTreeSerializerLogics"), AssemblyBuilderAccess.Save, (IEnumerable<CustomAttributeBuilder>) null);
      SerializerDebugging._moduleBuilder = SerializerDebugging._assemblyBuilder.DefineDynamicModule("ExpressionTreeSerializerLogics", "ExpressionTreeSerializerLogics.dll", true);
    }

    public static IEnumerable<string> CodeDomSerializerDependentAssemblies
    {
      get
      {
        SerializerDebugging.EnsureDependentAssembliesListsInitialized();
        foreach (string runtimeAssembly in (IEnumerable<string>) SerializerDebugging._runtimeAssemblies)
          yield return runtimeAssembly;
        foreach (string serializerAssembly in (IEnumerable<string>) SerializerDebugging._compiledCodeDomSerializerAssemblies)
          yield return serializerAssembly;
      }
    }

    public static void AddRuntimeAssembly(string pathToAssembly)
    {
      SerializerDebugging.EnsureDependentAssembliesListsInitialized();
      SerializerDebugging._runtimeAssemblies.Add(pathToAssembly);
    }

    public static void AddCompiledCodeDomAssembly(string pathToAssembly)
    {
      SerializerDebugging.EnsureDependentAssembliesListsInitialized();
      SerializerDebugging._compiledCodeDomSerializerAssemblies.Add(pathToAssembly);
    }

    public static void ResetDependentAssemblies()
    {
      SerializerDebugging.EnsureDependentAssembliesListsInitialized();
      File.AppendAllLines(SerializerDebugging.GetHistoryFilePath(), (IEnumerable<string>) SerializerDebugging._compiledCodeDomSerializerAssemblies);
      SerializerDebugging._compiledCodeDomSerializerAssemblies.Clear();
      SerializerDebugging.ResetRuntimeAssemblies();
    }

    public static void DeletePastTemporaries()
    {
      if (Interlocked.CompareExchange(ref SerializerDebugging._wasDeleted, 1, 0) != 0)
        return;
      try
      {
        string historyFilePath = SerializerDebugging.GetHistoryFilePath();
        if (!File.Exists(historyFilePath))
          return;
        foreach (string readAllLine in File.ReadAllLines(historyFilePath))
        {
          if (!string.IsNullOrEmpty(readAllLine))
            File.Delete(readAllLine);
        }
        new FileStream(historyFilePath, FileMode.Truncate).Close();
      }
      catch (IOException ex)
      {
      }
    }

    private static string GetHistoryFilePath()
    {
      return Path.Combine(Path.GetTempPath(), "MsgPack.Serialization.SerializationGenerationDebugging.CodeDOM.History.txt");
    }

    private static void EnsureDependentAssembliesListsInitialized()
    {
      if (SerializerDebugging._runtimeAssemblies == null)
      {
        SerializerDebugging._runtimeAssemblies = (IList<string>) new List<string>();
        SerializerDebugging.ResetRuntimeAssemblies();
      }
      if (SerializerDebugging._compiledCodeDomSerializerAssemblies != null)
        return;
      SerializerDebugging._compiledCodeDomSerializerAssemblies = (IList<string>) new List<string>();
    }

    private static void ResetRuntimeAssemblies()
    {
      SerializerDebugging._runtimeAssemblies.Add("System.dll");
      SerializerDebugging._runtimeAssemblies.Add("System.Core.dll");
      SerializerDebugging._runtimeAssemblies.Add("System.Numerics.dll");
      SerializerDebugging._runtimeAssemblies.Add(typeof (SerializerDebugging).Assembly.Location);
    }

    public static bool OnTheFlyCodeDomEnabled
    {
      get
      {
        return SerializerDebugging._onTheFlyCodeDomEnabled;
      }
      set
      {
        SerializerDebugging._onTheFlyCodeDomEnabled = value;
      }
    }

    public static TypeBuilder NewTypeBuilder(Type targetType)
    {
      if ((Module) SerializerDebugging._moduleBuilder == (Module) null)
        throw new InvalidOperationException("PrepareDump() was not called.");
      return SerializerDebugging._moduleBuilder.DefineType(IdentifierUtility.EscapeTypeName(targetType) + "SerializerLogics");
    }

    public static void Dump()
    {
      if (!((Assembly) SerializerDebugging._assemblyBuilder != (Assembly) null))
        return;
      SerializerDebugging._assemblyBuilder.Save(SerializerDebugging._assemblyBuilder.GetName().Name + ".dll");
    }

    public static void Reset()
    {
      SerializerDebugging._assemblyBuilder = (AssemblyBuilder) null;
      SerializerDebugging._moduleBuilder = (ModuleBuilder) null;
      if (SerializerDebugging._ilTraceWriter != null)
      {
        SerializerDebugging._ilTraceWriter.Dispose();
        SerializerDebugging._ilTraceWriter = (StringWriter) null;
      }
      SerializerDebugging._dumpEnabled = false;
      SerializerDebugging._traceEnabled = false;
      SerializerDebugging.ResetDependentAssemblies();
    }
  }
}
