// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.DefaultSerializationMethodGeneratorManager
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class DefaultSerializationMethodGeneratorManager : SerializationMethodGeneratorManager
  {
    private static readonly ConstructorInfo _debuggableAttributeCtor = typeof (DebuggableAttribute).GetConstructor(new Type[2]
    {
      typeof (bool),
      typeof (bool)
    });
    private static readonly object[] _debuggableAttributeCtorArguments = new object[2]
    {
      (object) true,
      (object) true
    };
    private static int _assemblySequence = -1;
    private static DefaultSerializationMethodGeneratorManager _canCollect = new DefaultSerializationMethodGeneratorManager(false, true, (AssemblyBuilder) null);
    private static DefaultSerializationMethodGeneratorManager _canDump = new DefaultSerializationMethodGeneratorManager(true, false, (AssemblyBuilder) null);
    private static DefaultSerializationMethodGeneratorManager _fast = new DefaultSerializationMethodGeneratorManager(false, false, (AssemblyBuilder) null);
    private readonly AssemblyBuilder _assembly;
    private readonly ModuleBuilder _module;
    private readonly bool _isDebuggable;

    public static DefaultSerializationMethodGeneratorManager CanCollect
    {
      get
      {
        return DefaultSerializationMethodGeneratorManager._canCollect;
      }
    }

    public static DefaultSerializationMethodGeneratorManager CanDump
    {
      get
      {
        return DefaultSerializationMethodGeneratorManager._canDump;
      }
    }

    public static DefaultSerializationMethodGeneratorManager Fast
    {
      get
      {
        return DefaultSerializationMethodGeneratorManager._fast;
      }
    }

    internal static void Refresh()
    {
      DefaultSerializationMethodGeneratorManager._canCollect = new DefaultSerializationMethodGeneratorManager(false, true, (AssemblyBuilder) null);
      DefaultSerializationMethodGeneratorManager._canDump = new DefaultSerializationMethodGeneratorManager(true, false, (AssemblyBuilder) null);
      DefaultSerializationMethodGeneratorManager._fast = new DefaultSerializationMethodGeneratorManager(false, false, (AssemblyBuilder) null);
    }

    [SecuritySafeCritical]
    private DefaultSerializationMethodGeneratorManager(
      bool isDebuggable,
      bool isCollectable,
      AssemblyBuilder assemblyBuilder)
    {
      this._isDebuggable = isDebuggable;
      string str;
      if ((Assembly) assemblyBuilder != (Assembly) null)
      {
        str = assemblyBuilder.GetName(false).Name;
        this._assembly = assemblyBuilder;
      }
      else
      {
        str = typeof (DefaultSerializationMethodGeneratorManager).Namespace + ".GeneratedSerealizers" + (object) Interlocked.Increment(ref DefaultSerializationMethodGeneratorManager._assemblySequence);
        AssemblyBuilder dedicatedAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(str), isDebuggable ? AssemblyBuilderAccess.RunAndSave : (isCollectable ? AssemblyBuilderAccess.RunAndCollect : AssemblyBuilderAccess.Run));
        DefaultSerializationMethodGeneratorManager.SetUpAssemblyBuilderAttributes(dedicatedAssemblyBuilder, isDebuggable);
        this._assembly = dedicatedAssemblyBuilder;
      }
      if (isDebuggable)
        this._module = this._assembly.DefineDynamicModule(str, str + ".dll", true);
      else
        this._module = this._assembly.DefineDynamicModule(str, true);
    }

    internal static void SetUpAssemblyBuilderAttributes(
      AssemblyBuilder dedicatedAssemblyBuilder,
      bool isDebuggable)
    {
      if (isDebuggable)
        dedicatedAssemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(DefaultSerializationMethodGeneratorManager._debuggableAttributeCtor, DefaultSerializationMethodGeneratorManager._debuggableAttributeCtorArguments));
      else
        dedicatedAssemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (DebuggableAttribute).GetConstructor(new Type[1]
        {
          typeof (DebuggableAttribute.DebuggingModes)
        }), new object[1]
        {
          (object) DebuggableAttribute.DebuggingModes.IgnoreSymbolStoreSequencePoints
        }));
      dedicatedAssemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (CompilationRelaxationsAttribute).GetConstructor(new Type[1]
      {
        typeof (int)
      }), new object[1]{ (object) 8 }));
      dedicatedAssemblyBuilder.SetCustomAttribute(new CustomAttributeBuilder(typeof (SecurityRulesAttribute).GetConstructor(new Type[1]
      {
        typeof (SecurityRuleSet)
      }), new object[1]{ (object) SecurityRuleSet.Level2 }, new PropertyInfo[1]
      {
        typeof (SecurityRulesAttribute).GetProperty("SkipVerificationInFullTrust")
      }, new object[1]{ (object) true }));
    }

    public static SerializationMethodGeneratorManager Create(
      AssemblyBuilder assemblyBuilder)
    {
      return (SerializationMethodGeneratorManager) new DefaultSerializationMethodGeneratorManager(true, false, assemblyBuilder);
    }

    protected override SerializerEmitter CreateEmitterCore(
      SerializerSpecification specification,
      Type baseClass,
      EmitterFlavor emitterFlavor)
    {
      return emitterFlavor == EmitterFlavor.FieldBased ? (SerializerEmitter) new FieldBasedSerializerEmitter(this._module, specification, baseClass, this._isDebuggable) : (SerializerEmitter) new ContextBasedSerializerEmitter(specification);
    }

    protected override EnumSerializerEmitter CreateEnumEmitterCore(
      SerializationContext context,
      SerializerSpecification specification,
      EmitterFlavor emitterFlavor)
    {
      return emitterFlavor == EmitterFlavor.FieldBased ? (EnumSerializerEmitter) new FieldBasedEnumSerializerEmitter(context, this._module, specification, this._isDebuggable) : (EnumSerializerEmitter) new ContextBasedEnumSerializerEmitter(specification);
    }
  }
}
