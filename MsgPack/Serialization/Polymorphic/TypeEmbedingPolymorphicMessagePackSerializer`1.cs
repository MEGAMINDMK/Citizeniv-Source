// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Polymorphic.TypeEmbedingPolymorphicMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.Polymorphic
{
  internal sealed class TypeEmbedingPolymorphicMessagePackSerializer<T> : MessagePackSerializer<T>, IPolymorphicDeserializer
  {
    private readonly PolymorphismSchema _schema;

    public TypeEmbedingPolymorphicMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext)
    {
      if (typeof (T).GetIsValueType())
        throw SerializationExceptions.NewValueTypeCannotBePolymorphic(typeof (T));
      this._schema = schema.FilterSelf();
    }

    private IMessagePackSerializer GetActualTypeSerializer(Type actualType)
    {
      IMessagePackSingleObjectSerializer serializer = this.OwnerContext.GetSerializer(actualType, (object) this._schema);
      if (serializer == null)
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Cannot get serializer for actual type {0} from context.", (object) actualType));
      return (IMessagePackSerializer) serializer;
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      TypeInfoEncoder.Encode(packer, objectTree.GetType());
      this.GetActualTypeSerializer(objectTree.GetType()).PackTo(packer, (object) objectTree);
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      return TypeInfoEncoder.Decode<T>(unpacker, (Func<Unpacker, Type>) (u => TypeInfoEncoder.DecodeRuntimeTypeInfo(u)), (Func<Type, Unpacker, T>) ((t, u) => (T) this.GetActualTypeSerializer(t).UnpackFrom(u)));
    }

    object IPolymorphicDeserializer.PolymorphicUnpackFrom(Unpacker unpacker)
    {
      return (object) this.UnpackFromCore(unpacker);
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T collection)
    {
      this.GetActualTypeSerializer(collection.GetType()).UnpackTo(unpacker, (object) collection);
    }
  }
}
