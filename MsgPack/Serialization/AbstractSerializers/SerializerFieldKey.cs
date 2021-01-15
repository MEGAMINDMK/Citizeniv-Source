// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.AbstractSerializers.SerializerFieldKey
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace MsgPack.Serialization.AbstractSerializers
{
  internal sealed class SerializerFieldKey : IEquatable<SerializerFieldKey>
  {
    public readonly RuntimeTypeHandle TypeHandle;
    public readonly EnumMemberSerializationMethod EnumSerializationMethod;
    public readonly DateTimeMemberConversionMethod DateTimeConversionMethod;
    private readonly SerializerFieldKey.ComparablePolymorphismSchema _schema;

    public PolymorphismSchema PolymorphismSchema
    {
      get
      {
        return this._schema.Value;
      }
    }

    public SerializerFieldKey(
      Type targetType,
      EnumMemberSerializationMethod enumMemberSerializationMethod,
      DateTimeMemberConversionMethod dateTimeConversionMethod,
      PolymorphismSchema polymorphismSchema)
    {
      this.TypeHandle = targetType.TypeHandle;
      this.EnumSerializationMethod = enumMemberSerializationMethod;
      this.DateTimeConversionMethod = dateTimeConversionMethod;
      this._schema = new SerializerFieldKey.ComparablePolymorphismSchema(polymorphismSchema);
    }

    public bool Equals(SerializerFieldKey other)
    {
      return other != null && (this.TypeHandle.Equals(other.TypeHandle) && this.EnumSerializationMethod == other.EnumSerializationMethod && this.DateTimeConversionMethod == other.DateTimeConversionMethod) && this._schema.Equals(other._schema);
    }

    public override bool Equals(object obj)
    {
      return this.Equals(obj as SerializerFieldKey);
    }

    public override int GetHashCode()
    {
      return this.TypeHandle.GetHashCode() ^ this.EnumSerializationMethod.GetHashCode() ^ this.DateTimeConversionMethod.GetHashCode() ^ this._schema.GetHashCode();
    }

    private struct ComparablePolymorphismSchema : IEquatable<SerializerFieldKey.ComparablePolymorphismSchema>
    {
      private readonly MessagePackString _key;
      private readonly PolymorphismSchema _value;

      public PolymorphismSchema Value
      {
        get
        {
          return this._value;
        }
      }

      public ComparablePolymorphismSchema(PolymorphismSchema value)
      {
        this._value = value;
        this._key = new MessagePackString(SerializerFieldKey.ComparablePolymorphismSchema.Pack(value), true);
      }

      private static byte[] Pack(PolymorphismSchema value)
      {
        using (MemoryStream memoryStream = new MemoryStream())
        {
          using (Packer packer = Packer.Create((Stream) memoryStream, PackerCompatibilityOptions.None, false))
          {
            SerializerFieldKey.ComparablePolymorphismSchema.Pack(packer, value);
            return memoryStream.ToArray();
          }
        }
      }

      private static void Pack(Packer packer, PolymorphismSchema value)
      {
        if (value == null)
        {
          packer.PackNull();
        }
        else
        {
          packer.PackArrayHeader(4);
          if (value.TargetType == (Type) null)
            packer.PackNull();
          else
            packer.PackString(value.TargetType.AssemblyQualifiedName);
          packer.Pack((int) value.ChildrenType);
          packer.PackMapHeader(value.CodeTypeMapping.Count);
          foreach (KeyValuePair<string, Type> keyValuePair in (IEnumerable<KeyValuePair<string, Type>>) value.CodeTypeMapping)
          {
            packer.PackString(keyValuePair.Key);
            packer.PackString(keyValuePair.Value.AssemblyQualifiedName);
          }
          packer.PackArrayHeader(value.ChildSchemaList.Count);
          foreach (PolymorphismSchema childSchema in (IEnumerable<PolymorphismSchema>) value.ChildSchemaList)
            SerializerFieldKey.ComparablePolymorphismSchema.Pack(packer, childSchema);
        }
      }

      public override bool Equals(object obj)
      {
        return obj is SerializerFieldKey.ComparablePolymorphismSchema other && this.Equals(other);
      }

      public bool Equals(
        SerializerFieldKey.ComparablePolymorphismSchema other)
      {
        return this._key == null ? other._key == null : this._key.Equals((object) other._key);
      }

      public override int GetHashCode()
      {
        return this._key.GetHashCode();
      }
    }
  }
}
