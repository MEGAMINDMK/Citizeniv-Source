// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.CollectionTraits
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Reflection;

namespace MsgPack.Serialization
{
  internal sealed class CollectionTraits
  {
    public static readonly CollectionTraits NotCollection = new CollectionTraits(CollectionDetailedKind.NotCollection, (MethodInfo) null, (MethodInfo) null, (MethodInfo) null, (Type) null);
    public static readonly CollectionTraits Unserializable = new CollectionTraits(CollectionDetailedKind.Unserializable, (MethodInfo) null, (MethodInfo) null, (MethodInfo) null, (Type) null);
    public readonly MethodInfo GetEnumeratorMethod;
    public readonly MethodInfo AddMethod;
    public readonly Type ElementType;
    public readonly CollectionDetailedKind DetailedCollectionType;

    public CollectionKind CollectionType
    {
      get
      {
        switch (this.DetailedCollectionType)
        {
          case CollectionDetailedKind.NotCollection:
            return CollectionKind.NotCollection;
          case CollectionDetailedKind.Array:
          case CollectionDetailedKind.GenericList:
          case CollectionDetailedKind.NonGenericList:
          case CollectionDetailedKind.GenericSet:
          case CollectionDetailedKind.GenericCollection:
          case CollectionDetailedKind.NonGenericCollection:
          case CollectionDetailedKind.GenericEnumerable:
          case CollectionDetailedKind.NonGenericEnumerable:
          case CollectionDetailedKind.GenericReadOnlyList:
          case CollectionDetailedKind.GenericReadOnlyCollection:
            return CollectionKind.Array;
          case CollectionDetailedKind.GenericDictionary:
          case CollectionDetailedKind.NonGenericDictionary:
          case CollectionDetailedKind.GenericReadOnlyDictionary:
            return CollectionKind.Map;
          case CollectionDetailedKind.Unserializable:
            return CollectionKind.Unserializable;
          default:
            throw new NotSupportedException("Unknown detailed type:" + (object) this.DetailedCollectionType);
        }
      }
    }

    public CollectionTraits(
      CollectionDetailedKind type,
      MethodInfo addMethod,
      MethodInfo countPropertyGetter,
      MethodInfo getEnumeratorMethod,
      Type elementType)
    {
      this.DetailedCollectionType = type;
      this.GetEnumeratorMethod = getEnumeratorMethod;
      this.AddMethod = addMethod;
      this.ElementType = elementType;
    }
  }
}
