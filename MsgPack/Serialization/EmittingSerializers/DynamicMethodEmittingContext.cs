// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.EmittingSerializers.DynamicMethodEmittingContext
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;

namespace MsgPack.Serialization.EmittingSerializers
{
  internal class DynamicMethodEmittingContext : ILEmittingContext
  {
    private readonly ILConstruct _context;

    public override ILConstruct Context
    {
      get
      {
        return this._context;
      }
    }

    public DynamicMethodEmittingContext(
      SerializationContext context,
      Type targetType,
      Func<SerializerEmitter> emitterFactory,
      Func<EnumSerializerEmitter> enumEmitterFactory)
      : base(context, emitterFactory, enumEmitterFactory)
    {
      this._context = ILConstruct.Argument(0, typeof (SerializationContext), nameof (context));
      this.Reset(targetType, (Type) null);
    }
  }
}
