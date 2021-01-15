// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.ResolveSerializerEventArgs
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;

namespace MsgPack.Serialization
{
  public sealed class ResolveSerializerEventArgs : EventArgs
  {
    private IMessagePackSerializer _foundSerializer;

    public SerializationContext Context { get; private set; }

    public Type TargetType { get; private set; }

    public PolymorphismSchema PolymorphismSchema { get; private set; }

    internal MessagePackSerializer<T> GetFoundSerializer<T>()
    {
      return (MessagePackSerializer<T>) this._foundSerializer;
    }

    public void SetSerializer<T>(MessagePackSerializer<T> foundSerializer)
    {
      if (typeof (T) != this.TargetType)
        throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "The serializer must be {0} type.", (object) typeof (MessagePackSerializer<>).MakeGenericType(this.TargetType)));
      this._foundSerializer = (IMessagePackSerializer) foundSerializer;
    }

    internal ResolveSerializerEventArgs(
      SerializationContext context,
      Type targetType,
      PolymorphismSchema schema)
    {
      this.Context = context;
      this.TargetType = targetType;
      this.PolymorphismSchema = schema ?? PolymorphismSchema.Default;
    }
  }
}
