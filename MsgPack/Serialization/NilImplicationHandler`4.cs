// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.NilImplicationHandler`4
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace MsgPack.Serialization
{
  internal abstract class NilImplicationHandler<TAction, TCondition, TPackingParameter, TUnpackedParameter>
    where TAction : class
    where TCondition : class
    where TPackingParameter : INilImplicationHandlerParameter
    where TUnpackedParameter : INilImplicationHandlerOnUnpackedParameter<TAction>
  {
    public TAction OnPacking(TPackingParameter parameter, NilImplication nilImplication)
    {
      if (nilImplication == NilImplication.Prohibit)
      {
        TCondition condition = default (TCondition);
        if (parameter.ItemType == typeof (MessagePackObject))
          condition = this.OnPackingMessagePackObject(parameter);
        else if (!parameter.ItemType.GetIsValueType())
          condition = this.OnPackingReferenceTypeObject(parameter);
        else if (Nullable.GetUnderlyingType(parameter.ItemType) != (Type) null)
          condition = this.OnPackingNullableValueTypeObject(parameter);
        if ((object) condition != null)
          return this.OnPackingCore(parameter, condition);
      }
      return default (TAction);
    }

    protected abstract TCondition OnPackingMessagePackObject(TPackingParameter parameter);

    protected abstract TCondition OnPackingReferenceTypeObject(TPackingParameter parameter);

    protected abstract TCondition OnPackingNullableValueTypeObject(TPackingParameter parameter);

    protected abstract TAction OnPackingCore(TPackingParameter parameter, TCondition condition);

    public TAction OnUnpacked(TUnpackedParameter parameter, NilImplication nilImplication)
    {
      bool flag = parameter.ItemType == typeof (MessagePackObject) || !parameter.ItemType.GetIsValueType() || Nullable.GetUnderlyingType(parameter.ItemType) != (Type) null;
      switch (nilImplication)
      {
        case NilImplication.MemberDefault:
          return this.OnNopOnUnpacked(parameter);
        case NilImplication.Null:
          return flag ? parameter.Store : this.OnThrowValueTypeCannotBeNull3OnUnpacked(parameter);
        case NilImplication.Prohibit:
          return this.OnThrowNullIsProhibitedExceptionOnUnpacked(parameter);
        default:
          throw new SerializationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Unknown NilImplication value '{0}'.", (object) (int) nilImplication));
      }
    }

    protected abstract TAction OnNopOnUnpacked(TUnpackedParameter parameter);

    protected abstract TAction OnThrowNullIsProhibitedExceptionOnUnpacked(
      TUnpackedParameter parameter);

    protected abstract TAction OnThrowValueTypeCannotBeNull3OnUnpacked(TUnpackedParameter parameter);
  }
}
