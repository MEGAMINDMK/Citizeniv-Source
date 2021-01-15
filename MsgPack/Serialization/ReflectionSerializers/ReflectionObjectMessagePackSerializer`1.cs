// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionObjectMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal class ReflectionObjectMessagePackSerializer<T> : MessagePackSerializer<T>
  {
    private static readonly PropertyInfo DictionaryEntryKeyProperty = typeof (DictionaryEntry).GetProperty("Key");
    private static readonly PropertyInfo DictionaryEntryValueProperty = typeof (DictionaryEntry).GetProperty("Value");
    private readonly Func<object, object>[] _getters;
    private readonly Action<object, object>[] _setters;
    private readonly MemberInfo[] _memberInfos;
    private readonly DataMemberContract[] _contracts;
    private readonly Dictionary<string, int> _memberIndexes;
    private readonly IMessagePackSerializer[] _serializers;
    private readonly ParameterInfo[] _constructorParameters;
    private readonly Dictionary<int, int> _constructorArgumentIndexes;

    public ReflectionObjectMessagePackSerializer(SerializationContext context)
      : base(context)
    {
      SerializationTarget.VerifyType(typeof (T));
      SerializationTarget serializationTarget = SerializationTarget.Prepare(context, typeof (T));
      ReflectionSerializerHelper.GetMetadata(serializationTarget.Members, context, out this._getters, out this._setters, out this._memberInfos, out this._contracts, out this._serializers);
      this._memberIndexes = ((IEnumerable<DataMemberContract>) this._contracts).Select<DataMemberContract, KeyValuePair<string, int>>((Func<DataMemberContract, int, KeyValuePair<string, int>>) ((contract, index) => new KeyValuePair<string, int>(contract.Name, index))).Where<KeyValuePair<string, int>>((Func<KeyValuePair<string, int>, bool>) (kv => kv.Key != null)).ToDictionary<KeyValuePair<string, int>, string, int>((Func<KeyValuePair<string, int>, string>) (kv => kv.Key), (Func<KeyValuePair<string, int>, int>) (kv => kv.Value));
      this._constructorParameters = typeof (IUnpackable).IsAssignableFrom(typeof (T)) || !serializationTarget.IsConstructorDeserialization ? (ParameterInfo[]) null : serializationTarget.DeserializationConstructor.GetParameters();
      if (this._constructorParameters == null)
        return;
      this._constructorArgumentIndexes = new Dictionary<int, int>(this._memberIndexes.Count);
      foreach (SerializingMember member1 in (IEnumerable<SerializingMember>) serializationTarget.Members)
      {
        SerializingMember member = member1;
        int index = Array.FindIndex<ParameterInfo>(this._constructorParameters, (Predicate<ParameterInfo>) (item => item.Name.Equals(member.Contract.Name, StringComparison.OrdinalIgnoreCase) && item.ParameterType == member.Member.GetMemberValueType()));
        if (index >= 0)
          this._constructorArgumentIndexes.Add(index, this._memberIndexes[member.Contract.Name]);
      }
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      if (objectTree is IPackable packable)
        packable.PackToMessage(packer, (PackingOptions) null);
      else if (this.OwnerContext.SerializationMethod != SerializationMethod.Map)
      {
        packer.PackArrayHeader(this._serializers.Length);
        for (int index = 0; index < this._serializers.Length; ++index)
          this.PackMemberValue(packer, objectTree, index);
      }
      else
      {
        packer.PackMapHeader(this._serializers.Length);
        for (int index = 0; index < this._serializers.Length; ++index)
        {
          packer.PackString(this._contracts[index].Name);
          this.PackMemberValue(packer, objectTree, index);
        }
      }
    }

    private void PackMemberValue(Packer packer, T objectTree, int index)
    {
      if (this._getters[index] == null)
      {
        packer.PackNull();
      }
      else
      {
        object objectTree1 = this._getters[index]((object) objectTree);
        Action<object> action = ReflectionNilImplicationHandler.Instance.OnPacking(new ReflectionSerializerNilImplicationHandlerParameter(this._memberInfos[index].GetMemberValueType(), this._contracts[index].Name), this._contracts[index].NilImplication);
        if (action != null)
          action(objectTree1);
        this._serializers[index].PackTo(packer, objectTree1);
      }
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      object objectGraph = this._constructorParameters == null ? ReflectionExtensions.CreateInstancePreservingExceptionType(typeof (T)) : (object) ((IEnumerable<ParameterInfo>) this._constructorParameters).Select<ParameterInfo, object>((Func<ParameterInfo, object>) (p =>
      {
        if (p.GetHasDefaultValue())
          return p.DefaultValue;
        return !p.ParameterType.GetIsValueType() ? (object) null : ReflectionExtensions.CreateInstancePreservingExceptionType(p.ParameterType);
      })).ToArray<object>();
      int unpacked = 0;
      if (objectGraph is IUnpackable unpackable)
      {
        unpackable.UnpackFromMessage(unpacker);
        return (T) objectGraph;
      }
      if (unpacker.IsArrayHeader)
      {
        int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
        for (int index = 0; index < itemsCount; ++index)
          objectGraph = this.UnpackMemberValue(objectGraph, unpacker, itemsCount, ref unpacked, index, index);
      }
      else
      {
        int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
        for (int index1 = 0; index1 < itemsCount; ++index1)
        {
          string result;
          if (!unpacker.ReadString(out result))
            throw SerializationExceptions.NewUnexpectedEndOfStream();
          if (result == null)
          {
            if (!unpacker.Read())
              throw SerializationExceptions.NewMissingItem(index1);
          }
          else
          {
            int index2;
            if (!this._memberIndexes.TryGetValue(result, out index2))
            {
              if (!unpacker.Skip().HasValue)
                throw SerializationExceptions.NewMissingItem(index1);
            }
            else
              objectGraph = this.UnpackMemberValue(objectGraph, unpacker, itemsCount, ref unpacked, index2, index1);
          }
        }
      }
      return this._constructorParameters == null ? (T) objectGraph : ReflectionExtensions.CreateInstancePreservingExceptionType<T>(typeof (T), objectGraph as object[]);
    }

    private object UnpackMemberValue(
      object objectGraph,
      Unpacker unpacker,
      int itemsCount,
      ref int unpacked,
      int index,
      int unpackerOffset)
    {
      object nullable = (object) null;
      Action<object, object> setter = index < this._setters.Length ? this._setters[index] : (Action<object, object>) null;
      if (unpacked < itemsCount)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(unpackerOffset);
        if (!unpacker.LastReadData.IsNil)
        {
          if (setter != null || this._constructorParameters != null)
            nullable = this.UnpackSingleValue(unpacker, index);
          else if (this._getters[index] != null)
            this.UnpackAndAddCollectionItem(objectGraph, unpacker, index);
        }
      }
      if (this._constructorParameters != null)
      {
        int argumentIndex;
        if (this._constructorArgumentIndexes.TryGetValue(index, out argumentIndex))
        {
          if (nullable == null)
            ReflectionNilImplicationHandler.Instance.OnUnpacked(new ReflectionSerializerNilImplicationHandlerOnUnpackedParameter(this._memberInfos[index].GetMemberValueType(), (Action<object>) (value => (objectGraph as object[])[argumentIndex] = nullable), this._contracts[index].Name, this._memberInfos[index].DeclaringType), this._contracts[index].NilImplication)((object) null);
          else
            (objectGraph as object[])[argumentIndex] = nullable;
        }
      }
      else if (setter != null)
      {
        if (nullable == null)
          ReflectionNilImplicationHandler.Instance.OnUnpacked(new ReflectionSerializerNilImplicationHandlerOnUnpackedParameter(this._memberInfos[index].GetMemberValueType(), (Action<object>) (value => setter(objectGraph, nullable)), this._contracts[index].Name, this._memberInfos[index].DeclaringType), this._contracts[index].NilImplication)((object) null);
        else
          setter(objectGraph, nullable);
      }
      ++unpacked;
      return objectGraph;
    }

    private object UnpackSingleValue(Unpacker unpacker, int index)
    {
      if (!unpacker.IsArrayHeader && !unpacker.IsMapHeader)
        return this._serializers[index].UnpackFrom(unpacker);
      using (Unpacker unpacker1 = unpacker.ReadSubtree())
        return this._serializers[index].UnpackFrom(unpacker1);
    }

    private void UnpackAndAddCollectionItem(object objectGraph, Unpacker unpacker, int index)
    {
      object instance = this._getters[index](objectGraph);
      if (instance == null)
        throw SerializationExceptions.NewReadOnlyMemberItemsMustNotBeNull(this._contracts[index].Name);
      CollectionTraits collectionTraits = instance.GetType().GetCollectionTraits();
      if (collectionTraits.AddMethod == (MethodInfo) null)
        throw SerializationExceptions.NewUnpackToIsNotSupported(instance.GetType(), (Exception) null);
      if (!(this._serializers[index].UnpackFrom(unpacker) is IEnumerable enumerable))
        return;
      switch (collectionTraits.DetailedCollectionType)
      {
        case CollectionDetailedKind.GenericDictionary:
          object[] objArray1 = new object[2];
          PropertyInfo propertyInfo1 = (PropertyInfo) null;
          PropertyInfo propertyInfo2 = (PropertyInfo) null;
          IEnumerator enumerator1 = enumerable.GetEnumerator();
          try
          {
            while (enumerator1.MoveNext())
            {
              object current = enumerator1.Current;
              if (propertyInfo1 == (PropertyInfo) null)
              {
                propertyInfo1 = current.GetType().GetProperty("Key");
                propertyInfo2 = current.GetType().GetProperty("Value");
              }
              objArray1[0] = propertyInfo1.GetValue(current, (object[]) null);
              objArray1[1] = propertyInfo2.GetValue(current, (object[]) null);
              collectionTraits.AddMethod.InvokePreservingExceptionType(instance, objArray1);
            }
            break;
          }
          finally
          {
            if (enumerator1 is IDisposable disposable)
              disposable.Dispose();
          }
        case CollectionDetailedKind.NonGenericDictionary:
          object[] objArray2 = new object[2];
          IEnumerator enumerator2 = enumerable.GetEnumerator();
          try
          {
            while (enumerator2.MoveNext())
            {
              object current = enumerator2.Current;
              objArray2[0] = ReflectionObjectMessagePackSerializer<T>.DictionaryEntryKeyProperty.GetValue(current, (object[]) null);
              objArray2[1] = ReflectionObjectMessagePackSerializer<T>.DictionaryEntryValueProperty.GetValue(current, (object[]) null);
              collectionTraits.AddMethod.InvokePreservingExceptionType(instance, objArray2);
            }
            break;
          }
          finally
          {
            if (enumerator2 is IDisposable disposable)
              disposable.Dispose();
          }
        default:
          object[] objArray3 = new object[1];
          IEnumerator enumerator3 = enumerable.GetEnumerator();
          try
          {
            while (enumerator3.MoveNext())
            {
              object current = enumerator3.Current;
              objArray3[0] = current;
              collectionTraits.AddMethod.InvokePreservingExceptionType(instance, objArray3);
            }
            break;
          }
          finally
          {
            if (enumerator3 is IDisposable disposable)
              disposable.Dispose();
          }
      }
    }
  }
}
