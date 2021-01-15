// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.AbstractCollectionSerializerHelper
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.CollectionSerializers;
using MsgPack.Serialization.Polymorphic;
using System;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal static class AbstractCollectionSerializerHelper
  {
    public static void GetConcreteSerializer(
      SerializationContext context,
      PolymorphismSchema schema,
      Type abstractType,
      Type targetType,
      Type exampleType,
      out ICollectionInstanceFactory factory,
      out IMessagePackSingleObjectSerializer serializer)
    {
      if (abstractType == targetType)
        throw SerializationExceptions.NewNotSupportedBecauseCannotInstanciateAbstractType(abstractType);
      serializer = context.GetSerializer(targetType, (object) schema);
      factory = serializer as ICollectionInstanceFactory;
      if (factory == null && !(serializer is IPolymorphicDeserializer))
        throw SerializationExceptions.NewIncompatibleCollectionSerializer(abstractType, serializer.GetType(), exampleType);
    }
  }
}
