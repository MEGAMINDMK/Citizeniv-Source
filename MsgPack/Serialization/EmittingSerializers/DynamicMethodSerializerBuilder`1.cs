// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.DynamicMethodSerializerBuilder`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using MsgPack.Serialization.AbstractSerializers;
using MsgPack.Serialization.Metadata;
using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal sealed class DynamicMethodSerializerBuilder<TObject> : ILEmittingSerializerBuilder<DynamicMethodEmittingContext, TObject>
  {
    protected override DynamicMethodEmittingContext CreateCodeGenerationContextForSerializerCreation(
      SerializationContext context)
    {
      return new DynamicMethodEmittingContext(context, typeof (TObject), (Func<SerializerEmitter>) (() => SerializationMethodGeneratorManager.Get().CreateEmitter(SerializerSpecification.CreateAnonymous(typeof (TObject), SerializerBuilder<DynamicMethodEmittingContext, ILConstruct, TObject>.CollectionTraitsOfThis), SerializerBuilder<DynamicMethodEmittingContext, ILConstruct, TObject>.BaseClass, EmitterFlavor.ContextBased)), (Func<EnumSerializerEmitter>) (() => SerializationMethodGeneratorManager.Get().CreateEnumEmitter(context, SerializerSpecification.CreateAnonymous(typeof (TObject), SerializerBuilder<DynamicMethodEmittingContext, ILConstruct, TObject>.CollectionTraitsOfThis), EmitterFlavor.ContextBased)));
    }

    protected override ILConstruct EmitThisReferenceExpression(
      DynamicMethodEmittingContext context)
    {
      throw new NotSupportedException();
    }

    protected override ILConstruct EmitInvokeUnpackTo(
      DynamicMethodEmittingContext context,
      ILConstruct unpacker,
      ILConstruct collection)
    {
      return this.EmitInvokeVoidMethod(context, this.EmitInvokeMethodExpression(context, context.Context, _SerializationContext.GetSerializer1_Method.MakeGenericMethod(typeof (TObject))), typeof (MessagePackSerializer<TObject>).GetMethod("UnpackTo"), unpacker, collection);
    }
  }
}
