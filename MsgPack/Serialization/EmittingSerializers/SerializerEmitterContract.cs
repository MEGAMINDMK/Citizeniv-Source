// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.SerializerEmitterContract
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class SerializerEmitterContract : SerializerEmitter
  {
    public override TracingILGenerator GetPackToMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetUnpackFromMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetUnpackToMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetAddItemMethodILGenerator(
      MethodInfo declaration)
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetCreateInstanceMethodILGenerator(
      MethodInfo declaration)
    {
      return (TracingILGenerator) null;
    }

    public override TracingILGenerator GetRestoreSchemaMethodILGenerator()
    {
      return (TracingILGenerator) null;
    }

    public override Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>> CreateConstructor<T>()
    {
      return (Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>>) null;
    }

    public override Action<TracingILGenerator, int> RegisterSerializer(
      Type targetType,
      EnumMemberSerializationMethod enumMemberSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema,
      Func<IEnumerable<ILConstruct>> schemaRegenerationCodeProvider)
    {
      return (Action<TracingILGenerator, int>) null;
    }
  }
}
