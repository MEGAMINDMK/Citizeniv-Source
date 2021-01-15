// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.AbstractSerializers.SerializerSpecification
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.AbstractSerializers
{
  internal sealed class SerializerSpecification : IEquatable<SerializerSpecification>
  {
    public Type TargetType { get; private set; }

    public CollectionTraits TargetCollectionTraits { get; private set; }

    public string SerializerTypeFullName { get; private set; }

    public string SerializerTypeName { get; private set; }

    public string SerializerTypeNamespace { get; private set; }

    public SerializerSpecification(
      Type targetType,
      CollectionTraits targetCollectionTraits,
      string serializerTypeName,
      string serializerTypeNamespace)
    {
      this.TargetType = targetType;
      this.TargetCollectionTraits = targetCollectionTraits;
      this.SerializerTypeName = serializerTypeName;
      this.SerializerTypeNamespace = serializerTypeNamespace;
      this.SerializerTypeFullName = string.IsNullOrEmpty(serializerTypeNamespace) ? serializerTypeName : serializerTypeNamespace + "." + serializerTypeName;
    }

    internal static SerializerSpecification CreateAnonymous(
      Type targetType,
      CollectionTraits targetCollectionTraits)
    {
      return new SerializerSpecification(targetType, targetCollectionTraits, "Anonymous", "AnonymousGeneratedSerializers");
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as SerializerSpecification);
    }

    public bool Equals(SerializerSpecification other)
    {
      return SerializerSpecification.Equals(this, other);
    }

    public override int GetHashCode()
    {
      return this.TargetType.GetHashCode() ^ this.SerializerTypeName.GetHashCode() ^ this.SerializerTypeNamespace.GetHashCode();
    }

    public static bool Equals(SerializerSpecification left, SerializerSpecification right)
    {
      if (object.ReferenceEquals((object) left, (object) right))
        return true;
      if (object.ReferenceEquals((object) left, (object) null))
        return object.ReferenceEquals((object) right, (object) null);
      return !object.ReferenceEquals((object) right, (object) null) && left.TargetType == right.TargetType && left.SerializerTypeName == right.SerializerTypeName && left.SerializerTypeNamespace == right.SerializerTypeNamespace;
    }

    public static bool operator ==(SerializerSpecification left, SerializerSpecification right)
    {
      return SerializerSpecification.Equals(left, right);
    }

    public static bool operator !=(SerializerSpecification left, SerializerSpecification right)
    {
      return !SerializerSpecification.Equals(left, right);
    }
  }
}
