// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.SerializerEmitter
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal abstract class SerializerEmitter : IDisposable
  {
    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }

    protected virtual void Dispose(bool disposing)
    {
    }

    public abstract TracingILGenerator GetPackToMethodILGenerator();

    public abstract TracingILGenerator GetUnpackFromMethodILGenerator();

    public abstract TracingILGenerator GetUnpackToMethodILGenerator();

    public abstract TracingILGenerator GetAddItemMethodILGenerator(
      MethodInfo declaration);

    public abstract TracingILGenerator GetCreateInstanceMethodILGenerator(
      MethodInfo declaration);

    public abstract TracingILGenerator GetRestoreSchemaMethodILGenerator();

    public MessagePackSerializer<T> CreateInstance<T>(
      SerializationContext context,
      PolymorphismSchema schema)
    {
      return this.CreateConstructor<T>()(context, schema);
    }

    public abstract Func<SerializationContext, PolymorphismSchema, MessagePackSerializer<T>> CreateConstructor<T>();

    public abstract Action<TracingILGenerator, int> RegisterSerializer(
      Type targetType,
      EnumMemberSerializationMethod enumMemberSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema,
      Func<IEnumerable<ILConstruct>> schemaRegenerationCodeProvider);

    public virtual Action<TracingILGenerator, int> RegisterField(
      FieldInfo field)
    {
      throw new NotSupportedException();
    }

    public virtual Action<TracingILGenerator, int> RegisterMethod(
      MethodBase method)
    {
      throw new NotSupportedException();
    }
  }
}
