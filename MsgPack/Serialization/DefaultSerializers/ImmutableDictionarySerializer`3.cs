// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.DefaultSerializers.ImmutableDictionarySerializer`3
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace MsgPack.Serialization.DefaultSerializers
{
  internal sealed class ImmutableDictionarySerializer<T, TKey, TValue> : MessagePackSerializer<T>
    where T : IEnumerable<KeyValuePair<TKey, TValue>>
  {
    private static readonly Func<KeyValuePair<TKey, TValue>[], T> _factory = ImmutableDictionarySerializer<T, TKey, TValue>.FindFactory();
    private readonly MessagePackSerializer<TKey> _keySerializer;
    private readonly MessagePackSerializer<TValue> _valueSerializer;

    private static Func<KeyValuePair<TKey, TValue>[], T> FindFactory()
    {
      Type factoryType = typeof (T).GetAssembly().GetType(typeof (T).FullName.Remove(typeof (T).FullName.IndexOf('`')));
      if (factoryType == (Type) null)
        return (Func<KeyValuePair<TKey, TValue>[], T>) (_ =>
        {
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' may not be an immutable collection.", (object) typeof (T).AssemblyQualifiedName));
        });
      MethodInfo methodInfo = ((IEnumerable<MethodInfo>) factoryType.GetMethods()).Where<MethodInfo>((Func<MethodInfo, bool>) (m => m.IsStatic && m.IsPublic && (m.IsGenericMethod && m.Name == "CreateRange") && m.GetParameters().Length == 1)).FirstOrDefault<MethodInfo>();
      if (methodInfo == (MethodInfo) null)
        return (Func<KeyValuePair<TKey, TValue>[], T>) (_ =>
        {
          throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "'{0}' does not have CreateRange({1}[]) public static method.", (object) factoryType.AssemblyQualifiedName, (object) typeof (IEnumerable<KeyValuePair<TKey, TValue>>)));
        });
      return methodInfo.MakeGenericMethod(typeof (TKey), typeof (TValue)).CreateDelegate(typeof (Func<KeyValuePair<TKey, TValue>[], T>)) as Func<KeyValuePair<TKey, TValue>[], T>;
    }

    public ImmutableDictionarySerializer(
      SerializationContext ownerContext,
      PolymorphismSchema keysSchema,
      PolymorphismSchema valuesSchema)
      : base(ownerContext)
    {
      this._keySerializer = ownerContext.GetSerializer<TKey>((object) keysSchema);
      this._valueSerializer = ownerContext.GetSerializer<TValue>((object) valuesSchema);
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      packer.PackMapHeader(objectTree.Count<KeyValuePair<TKey, TValue>>());
      using (IEnumerator<KeyValuePair<TKey, TValue>> enumerator = objectTree.GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          KeyValuePair<TKey, TValue> current = enumerator.Current;
          this._keySerializer.PackTo(packer, current.Key);
          this._valueSerializer.PackTo(packer, current.Value);
        }
      }
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsMapHeader)
        throw SerializationExceptions.NewIsNotMapHeader();
      KeyValuePair<TKey, TValue>[] keyValuePairArray = new KeyValuePair<TKey, TValue>[UnpackHelpers.GetItemsCount(unpacker)];
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
      {
        for (int index = 0; index < keyValuePairArray.Length; ++index)
        {
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          TKey key = this._keySerializer.UnpackFrom(unpacker);
          if (!unpacker1.Read())
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          TValue obj = this._valueSerializer.UnpackFrom(unpacker);
          keyValuePairArray[index] = new KeyValuePair<TKey, TValue>(key, obj);
        }
      }
      return ImmutableDictionarySerializer<T, TKey, TValue>._factory(keyValuePairArray);
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T collection)
    {
      throw new NotSupportedException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unable to unpack items to existing immutable dictioary '{0}'.", (object) typeof (T)));
    }
  }
}
