// Decompiled with JetBrains decompiler
// Type: MsgPack.PackerUnpackerExtensions
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MsgPack
{
  public static class PackerUnpackerExtensions
  {
    public static Packer Pack<T>(this Packer source, T value)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      PackerUnpackerExtensions.PackCore<T>(source, value, SerializationContext.Default);
      return source;
    }

    public static Packer Pack<T>(this Packer source, T value, SerializationContext context)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      PackerUnpackerExtensions.PackCore<T>(source, value, context);
      return source;
    }

    private static void PackCore<T>(Packer source, T value, SerializationContext context)
    {
      if ((object) value == null)
        source.PackNull();
      else if (value is IPackable packable)
        packable.PackToMessage(source, new PackingOptions());
      else
        context.GetSerializer<T>().PackTo(source, value);
    }

    public static Packer PackArray<T>(this Packer source, IEnumerable<T> collection)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(source, collection, SerializationContext.Default);
      return source;
    }

    public static Packer PackArray<T>(
      this Packer source,
      IEnumerable<T> collection,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(source, collection, context);
      return source;
    }

    public static Packer PackCollection<T>(this Packer source, IEnumerable<T> collection)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(source, collection, SerializationContext.Default);
      return source;
    }

    public static Packer PackCollection<T>(
      this Packer source,
      IEnumerable<T> collection,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(source, collection, context);
      return source;
    }

    private static void PackCollectionCore<T>(
      Packer source,
      IEnumerable<T> collection,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackCollectionCore<T>(source, collection, context.GetSerializer<T>());
    }

    internal static void PackCollectionCore<T>(
      Packer source,
      IEnumerable<T> collection,
      MessagePackSerializer<T> itemSerializer)
    {
      int count;
      switch (collection)
      {
        case null:
          source.PackNull();
          return;
        case IPackable packable:
          packable.PackToMessage(source, new PackingOptions());
          return;
        case ICollection<T> objs:
          count = objs.Count;
          break;
        case ICollection collection1:
          count = collection1.Count;
          break;
        default:
          T[] array = collection.ToArray<T>();
          count = array.Length;
          collection = (IEnumerable<T>) array;
          break;
      }
      source.PackArrayHeader(count);
      foreach (T objectTree in collection)
        itemSerializer.PackTo(source, objectTree);
    }

    public static Packer PackMap<TKey, TValue>(
      this Packer source,
      IDictionary<TKey, TValue> dictionary)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(source, dictionary, SerializationContext.Default);
      return source;
    }

    public static Packer PackMap<TKey, TValue>(
      this Packer source,
      IDictionary<TKey, TValue> dictionary,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(source, dictionary, context);
      return source;
    }

    public static Packer PackDictionary<TKey, TValue>(
      this Packer source,
      IDictionary<TKey, TValue> dictionary)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(source, dictionary, SerializationContext.Default);
      return source;
    }

    public static Packer PackDictionary<TKey, TValue>(
      this Packer source,
      IDictionary<TKey, TValue> dictionary,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(source, dictionary, context);
      return source;
    }

    private static void PackDictionaryCore<TKey, TValue>(
      Packer source,
      IDictionary<TKey, TValue> dictionary,
      SerializationContext context)
    {
      PackerUnpackerExtensions.PackDictionaryCore<TKey, TValue>(source, dictionary, context.GetSerializer<TKey>(), context.GetSerializer<TValue>());
    }

    internal static void PackDictionaryCore<TKey, TValue>(
      Packer source,
      IDictionary<TKey, TValue> dictionary,
      MessagePackSerializer<TKey> keySerializer,
      MessagePackSerializer<TValue> valueSerializer)
    {
      if (dictionary == null)
        source.PackNull();
      else if (dictionary is IPackable packable)
      {
        packable.PackToMessage(source, new PackingOptions());
      }
      else
      {
        source.PackMapHeader(dictionary.Count);
        foreach (KeyValuePair<TKey, TValue> keyValuePair in (IEnumerable<KeyValuePair<TKey, TValue>>) dictionary)
        {
          keySerializer.PackTo(source, keyValuePair.Key);
          valueSerializer.PackTo(source, keyValuePair.Value);
        }
      }
    }

    public static Packer Pack<T>(this Packer source, IEnumerable<T> items)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      source.PackCore<T>(items, SerializationContext.Default);
      return source;
    }

    public static Packer Pack<T>(
      this Packer source,
      IEnumerable<T> items,
      SerializationContext context)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      source.PackCore<T>(items, context);
      return source;
    }

    private static void PackCore<T>(
      this Packer source,
      IEnumerable<T> items,
      SerializationContext context)
    {
      switch (items)
      {
        case null:
          source.PackNull();
          break;
        case IPackable packable:
          packable.PackToMessage(source, new PackingOptions());
          break;
        case ICollection<T> array:
label_4:
          MessagePackSerializer<T> serializer = context.GetSerializer<T>();
          source.PackArrayHeader(array.Count);
          using (IEnumerator<T> enumerator = array.GetEnumerator())
          {
            while (enumerator.MoveNext())
            {
              T current = enumerator.Current;
              serializer.PackTo(source, current);
            }
            break;
          }
        default:
          array = (ICollection<T>) items.ToArray<T>();
          goto label_4;
      }
    }

    public static Packer PackObject(this Packer source, object value)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      PackerUnpackerExtensions.PackObjectCore(source, value, SerializationContext.Default);
      return source;
    }

    public static Packer PackObject(
      this Packer source,
      object value,
      SerializationContext context)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      PackerUnpackerExtensions.PackObjectCore(source, value, context);
      return source;
    }

    private static void PackObjectCore(Packer source, object value, SerializationContext context)
    {
      if (value == null)
      {
        source.PackNull();
      }
      else
      {
        Type type = value.GetType();
        context.GetSerializer(type).PackTo(source, value);
      }
    }

    public static T Unpack<T>(this Unpacker source)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      return PackerUnpackerExtensions.UnpackCore<T>(source, new SerializationContext());
    }

    public static T Unpack<T>(this Unpacker source, SerializationContext context)
    {
      if (source == null)
        throw new ArgumentNullException(nameof (source));
      if (context == null)
        throw new ArgumentNullException(nameof (context));
      return PackerUnpackerExtensions.UnpackCore<T>(source, context);
    }

    private static T UnpackCore<T>(Unpacker source, SerializationContext context)
    {
      return context.GetSerializer<T>().UnpackFrom(source);
    }
  }
}
