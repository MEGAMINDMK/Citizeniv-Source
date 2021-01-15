// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Polymorphic.KnownTypePolymorphicMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Reflection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace MsgPack.Serialization.Polymorphic
{
  internal sealed class KnownTypePolymorphicMessagePackSerializer<T> : MessagePackSerializer<T>, IPolymorphicDeserializer
  {
    private readonly PolymorphismSchema _schema;
    private readonly IDictionary<string, RuntimeTypeHandle> _typeHandleMap;
    private readonly IDictionary<RuntimeTypeHandle, string> _typeCodeMap;

    public KnownTypePolymorphicMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext)
    {
      if (typeof (T).GetIsValueType())
        throw SerializationExceptions.NewValueTypeCannotBePolymorphic(typeof (T));
      this._schema = schema.FilterSelf();
      this._typeHandleMap = KnownTypePolymorphicMessagePackSerializer<T>.BuildTypeCodeTypeHandleMap(schema.CodeTypeMapping);
      this._typeCodeMap = KnownTypePolymorphicMessagePackSerializer<T>.BuildTypeHandleTypeCodeMap(schema.CodeTypeMapping);
    }

    private static IDictionary<string, RuntimeTypeHandle> BuildTypeCodeTypeHandleMap(
      IDictionary<string, Type> typeMap)
    {
      return (IDictionary<string, RuntimeTypeHandle>) typeMap.ToDictionary<KeyValuePair<string, Type>, string, RuntimeTypeHandle>((Func<KeyValuePair<string, Type>, string>) (kv => kv.Key), (Func<KeyValuePair<string, Type>, RuntimeTypeHandle>) (kv => kv.Value.TypeHandle));
    }

    private static IDictionary<RuntimeTypeHandle, string> BuildTypeHandleTypeCodeMap(
      IDictionary<string, Type> typeMap)
    {
      Dictionary<RuntimeTypeHandle, string> dictionary = new Dictionary<RuntimeTypeHandle, string>(typeMap.Count);
      foreach (IGrouping<Type, KeyValuePair<string, Type>> source in typeMap.GroupBy<KeyValuePair<string, Type>, Type>((Func<KeyValuePair<string, Type>, Type>) (kv => kv.Value)))
      {
        if (source.Count<KeyValuePair<string, Type>>() > 1)
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' is mapped to multiple extension type codes({1}).", (object) source.Key, (object) string.Join(CultureInfo.CurrentCulture.TextInfo.ListSeparator, source.Select<KeyValuePair<string, Type>, string>((Func<KeyValuePair<string, Type>, string>) (kv => kv.Key)))));
        dictionary.Add(source.Key.TypeHandle, source.First<KeyValuePair<string, Type>>().Key);
      }
      return (IDictionary<RuntimeTypeHandle, string>) dictionary;
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
      if (!this._typeCodeMap.TryGetValue(objectTree.GetType().TypeHandle, out string _))
        throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Type '{0}' in assembly '{1}' is not defined as known types.", (object) objectTree.GetType().GetFullName(), (object) objectTree.GetType().GetAssembly()));
      TypeInfoEncoder.Encode(packer, this._typeCodeMap[objectTree.GetType().TypeHandle]);
      this.GetActualTypeSerializer(objectTree.GetType()).PackTo(packer, (object) objectTree);
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      return TypeInfoEncoder.Decode<T>(unpacker, (Func<Unpacker, Type>) (u =>
      {
        string key = u.LastReadData.AsString();
        RuntimeTypeHandle handle;
        if (!this._typeHandleMap.TryGetValue(key, out handle))
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unknown type {0}.", (object) StringEscape.ForDisplay(key)));
        return Type.GetTypeFromHandle(handle);
      }), (Func<Type, Unpacker, T>) ((t, u) => (T) this.GetActualTypeSerializer(t).UnpackFrom(u)));
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
