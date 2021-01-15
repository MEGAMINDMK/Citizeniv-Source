// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.MessagePackKnownTupleItemTypeAttribute
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.Polymorphic;
using System;

namespace MsgPack.Serialization
{
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
  public sealed class MessagePackKnownTupleItemTypeAttribute : Attribute, IPolymorphicKnownTypeAttribute, IPolymorphicTupleItemTypeAttribute, IPolymorphicHelperAttribute
  {
    PolymorphismTarget IPolymorphicHelperAttribute.Target
    {
      get
      {
        return PolymorphismTarget.TupleItem;
      }
    }

    public string TypeCode { get; private set; }

    public Type BindingType { get; private set; }

    public int ItemNumber { get; private set; }

    public MessagePackKnownTupleItemTypeAttribute(
      int itemNumber,
      string typeCode,
      Type bindingType)
    {
      this.ItemNumber = itemNumber;
      this.TypeCode = typeCode;
      this.BindingType = bindingType;
    }
  }
}
