// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Polymorphic.PolymorphicSerializerProvider`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

namespace MsgPack.Serialization.Polymorphic
{
  internal sealed class PolymorphicSerializerProvider<T> : MessagePackSerializerProvider
  {
    private readonly MessagePackSerializer<T> _defaultSerializer;

    public PolymorphicSerializerProvider(MessagePackSerializer<T> defaultSerializer)
    {
      this._defaultSerializer = defaultSerializer;
    }

    public override object Get(SerializationContext context, object providerParameter)
    {
      if (!(providerParameter is PolymorphismSchema schema) || schema.UseDefault || schema.TargetType != typeof (T))
      {
        if (this._defaultSerializer == null)
          throw SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractType(typeof (T));
        return (object) this._defaultSerializer;
      }
      return schema.UseTypeEmbedding ? (object) new TypeEmbedingPolymorphicMessagePackSerializer<T>(context, schema) : (object) new KnownTypePolymorphicMessagePackSerializer<T>(context, schema);
    }
  }
}
