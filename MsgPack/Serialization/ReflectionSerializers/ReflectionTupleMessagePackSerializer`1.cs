// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionTupleMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal class ReflectionTupleMessagePackSerializer<T> : MessagePackSerializer<T>
  {
    private readonly IList<Type> _tupleTypes;
    private readonly IList<ConstructorInfo> _tupleConstructors;
    private readonly IList<Func<T, object>> _getters;
    private readonly IList<IMessagePackSingleObjectSerializer> _itemSerializers;

    public ReflectionTupleMessagePackSerializer(
      SerializationContext ownerContext,
      IList<PolymorphismSchema> itemSchemas)
      : base(ownerContext)
    {
      IList<Type> tupleItemTypes = TupleItems.GetTupleItemTypes(typeof (T));
      this._itemSerializers = (IList<IMessagePackSingleObjectSerializer>) tupleItemTypes.Select<Type, IMessagePackSingleObjectSerializer>((Func<Type, int, IMessagePackSingleObjectSerializer>) ((itemType, i) => ownerContext.GetSerializer(itemType, itemSchemas.Count == 0 ? (object) (PolymorphismSchema) null : (object) itemSchemas[i]))).ToArray<IMessagePackSingleObjectSerializer>();
      this._tupleTypes = (IList<Type>) TupleItems.CreateTupleTypeList(tupleItemTypes);
      this._tupleConstructors = (IList<ConstructorInfo>) this._tupleTypes.Select<Type, ConstructorInfo>((Func<Type, ConstructorInfo>) (tupleType => ((IEnumerable<ConstructorInfo>) tupleType.GetConstructors()).Single<ConstructorInfo>())).ToArray<ConstructorInfo>();
      this._getters = (IList<Func<T, object>>) ReflectionTupleMessagePackSerializer<T>.GetGetters(tupleItemTypes, this._tupleTypes).ToArray<Func<T, object>>();
    }

    private static IEnumerable<Func<T, object>> GetGetters(
      IList<Type> itemTypes,
      IList<Type> tupleTypes)
    {
      int depth = -1;
      List<PropertyInfo> propertyInvocationChain = new List<PropertyInfo>(itemTypes.Count % 7 + 1);
      for (int i = 0; i < itemTypes.Count; ++i)
      {
        if (i % 7 == 0)
          ++depth;
        for (int index = 0; index < depth; ++index)
          propertyInvocationChain.Add(tupleTypes[index].GetProperty("Rest"));
        PropertyInfo itemNProperty = tupleTypes[depth].GetProperty("Item" + (object) (i % 7 + 1));
        propertyInvocationChain.Add(itemNProperty);
        MethodInfo[] getters = propertyInvocationChain.Select<PropertyInfo, MethodInfo>((Func<PropertyInfo, MethodInfo>) (property => property.GetGetMethod())).ToArray<MethodInfo>();
        yield return (Func<T, object>) (tuple =>
        {
          object instance = (object) tuple;
          foreach (MethodInfo source in getters)
            instance = source.InvokePreservingExceptionType(instance);
          return instance;
        });
        propertyInvocationChain.Clear();
      }
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      packer.PackArrayHeader(this._itemSerializers.Count);
      for (int index = 0; index < this._itemSerializers.Count; ++index)
        this._itemSerializers[index].PackTo(packer, this._getters[index](objectTree));
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      if (!unpacker.IsArrayHeader)
        throw SerializationExceptions.NewIsNotArrayHeader();
      int itemsCount = UnpackHelpers.GetItemsCount(unpacker);
      if (itemsCount != this._itemSerializers.Count)
        throw SerializationExceptions.NewTupleCardinarityIsNotMatch(this._itemSerializers.Count, itemsCount);
      List<object> objectList = new List<object>(this._itemSerializers.Count);
      for (int index = 0; index < this._itemSerializers.Count; ++index)
      {
        if (!unpacker.Read())
          throw SerializationExceptions.NewMissingItem(index);
        objectList.Add(this._itemSerializers[index].UnpackFrom(unpacker));
      }
      return this.CreateTuple((IList<object>) objectList);
    }

    private T CreateTuple(IList<object> unpackedItems)
    {
      object obj = (object) null;
      for (int index = this._tupleTypes.Count - 1; index >= 0; --index)
      {
        List<object> list = unpackedItems.Skip<object>(index * 7).Take<object>(Math.Min(unpackedItems.Count, 7)).ToList<object>();
        if (obj != null)
          list.Add(obj);
        obj = this._tupleConstructors[index].InvokePreservingExceptionType(list.ToArray());
      }
      return (T) obj;
    }
  }
}
