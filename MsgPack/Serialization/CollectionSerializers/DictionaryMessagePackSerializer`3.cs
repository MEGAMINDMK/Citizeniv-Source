// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionSerializers.DictionaryMessagePackSerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Collections.Generic;

namespace MsgPack.Serialization.CollectionSerializers
{
  public abstract class DictionaryMessagePackSerializer<TDictionary, TKey, TValue> : DictionaryMessagePackSerializerBase<TDictionary, TKey, TValue>
    where TDictionary : IDictionary<TKey, TValue>
  {
    protected DictionaryMessagePackSerializer(
      SerializationContext ownerContext,
      PolymorphismSchema schema)
      : base(ownerContext, schema)
    {
    }

    protected override int GetCount(TDictionary dictionary)
    {
      return dictionary.Count;
    }

    protected override void AddItem(TDictionary dictionary, TKey key, TValue value)
    {
      dictionary.Add(key, value);
    }
  }
}
