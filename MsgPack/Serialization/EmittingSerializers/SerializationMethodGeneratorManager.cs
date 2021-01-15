// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.SerializationMethodGeneratorManager
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using System;
using System.Reflection.Emit;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class SerializationMethodGeneratorManager
  {
    public static SerializationMethodGeneratorManager Get()
    {
      return SerializationMethodGeneratorManager.Get(SerializerDebugging.DumpEnabled ? SerializationMethodGeneratorOption.CanDump : SerializationMethodGeneratorOption.Fast);
    }

    public static SerializationMethodGeneratorManager Get(
      SerializationMethodGeneratorOption option)
    {
      switch (option)
      {
        case SerializationMethodGeneratorOption.CanDump:
          return (SerializationMethodGeneratorManager) DefaultSerializationMethodGeneratorManager.CanDump;
        case SerializationMethodGeneratorOption.CanCollect:
          return (SerializationMethodGeneratorManager) DefaultSerializationMethodGeneratorManager.CanCollect;
        default:
          return (SerializationMethodGeneratorManager) DefaultSerializationMethodGeneratorManager.Fast;
      }
    }

    public static SerializationMethodGeneratorManager Get(
      AssemblyBuilder assemblyBuilder)
    {
      return DefaultSerializationMethodGeneratorManager.Create(assemblyBuilder);
    }

    public SerializerEmitter CreateEmitter(
      SerializerSpecification specification,
      Type baseClass,
      EmitterFlavor emitterFlavor)
    {
      return this.CreateEmitterCore(specification, baseClass, emitterFlavor);
    }

    protected abstract SerializerEmitter CreateEmitterCore(
      SerializerSpecification specification,
      Type baseClass,
      EmitterFlavor emitterFlavor);

    public EnumSerializerEmitter CreateEnumEmitter(
      SerializationContext context,
      SerializerSpecification specification,
      EmitterFlavor emitterFlavor)
    {
      return this.CreateEnumEmitterCore(context, specification, emitterFlavor);
    }

    protected abstract EnumSerializerEmitter CreateEnumEmitterCore(
      SerializationContext context,
      SerializerSpecification specification,
      EmitterFlavor emitterFlavor);
  }
}
