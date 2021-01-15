// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ReflectionSerializers.ReflectionNilImplicationHandler
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.ReflectionSerializers
{
  internal class ReflectionNilImplicationHandler : NilImplicationHandler<Action<object>, Func<object, bool>, ReflectionSerializerNilImplicationHandlerParameter, ReflectionSerializerNilImplicationHandlerOnUnpackedParameter>
  {
    public static readonly ReflectionNilImplicationHandler Instance = new ReflectionNilImplicationHandler();

    private ReflectionNilImplicationHandler()
    {
    }

    protected override Func<object, bool> OnPackingMessagePackObject(
      ReflectionSerializerNilImplicationHandlerParameter parameter)
    {
      return (Func<object, bool>) (value => ((MessagePackObject) value).IsNil);
    }

    protected override Func<object, bool> OnPackingReferenceTypeObject(
      ReflectionSerializerNilImplicationHandlerParameter parameter)
    {
      return (Func<object, bool>) (value => value == null);
    }

    protected override Func<object, bool> OnPackingNullableValueTypeObject(
      ReflectionSerializerNilImplicationHandlerParameter parameter)
    {
      return (Func<object, bool>) (value => value == null);
    }

    protected override Action<object> OnPackingCore(
      ReflectionSerializerNilImplicationHandlerParameter parameter,
      Func<object, bool> condition)
    {
      return (Action<object>) (value =>
      {
        if (condition(value))
          throw SerializationExceptions.NewNullIsProhibited(parameter.MemberName);
      });
    }

    protected override Action<object> OnNopOnUnpacked(
      ReflectionSerializerNilImplicationHandlerOnUnpackedParameter parameter)
    {
      return (Action<object>) (_ => {});
    }

    protected override Action<object> OnThrowNullIsProhibitedExceptionOnUnpacked(
      ReflectionSerializerNilImplicationHandlerOnUnpackedParameter parameter)
    {
      return (Action<object>) (_ =>
      {
        throw SerializationExceptions.NewNullIsProhibited(parameter.MemberName);
      });
    }

    protected override Action<object> OnThrowValueTypeCannotBeNull3OnUnpacked(
      ReflectionSerializerNilImplicationHandlerOnUnpackedParameter parameter)
    {
      return (Action<object>) (_ =>
      {
        throw SerializationExceptions.NewValueTypeCannotBeNull(parameter.MemberName, parameter.ItemType, parameter.DeclaringType);
      });
    }
  }
}
