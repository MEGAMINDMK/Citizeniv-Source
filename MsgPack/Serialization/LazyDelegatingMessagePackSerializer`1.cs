// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.LazyDelegatingMessagePackSerializer`1
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Globalization;

namespace MsgPack.Serialization
{
  internal sealed class LazyDelegatingMessagePackSerializer<T> : MessagePackSerializer<T>
  {
    private readonly object _providerParameter;
    private MessagePackSerializer<T> _delegated;

    public LazyDelegatingMessagePackSerializer(
      SerializationContext ownerContext,
      object providerParameter)
      : base(ownerContext)
    {
      this._providerParameter = providerParameter;
    }

    private MessagePackSerializer<T> GetDelegatedSerializer()
    {
      MessagePackSerializer<T> messagePackSerializer = this._delegated;
      if (messagePackSerializer == null)
      {
        messagePackSerializer = this.OwnerContext.GetSerializer<T>(this._providerParameter);
        if (messagePackSerializer is LazyDelegatingMessagePackSerializer<T>)
          throw new InvalidOperationException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "MessagePack serializer for the type '{0}' is not constructed yet.", (object) typeof (T)));
        this._delegated = messagePackSerializer;
      }
      return messagePackSerializer;
    }

    protected internal override void PackToCore(Packer packer, T objectTree)
    {
      this.GetDelegatedSerializer().PackToCore(packer, objectTree);
    }

    protected internal override T UnpackFromCore(Unpacker unpacker)
    {
      return this.GetDelegatedSerializer().UnpackFromCore(unpacker);
    }

    protected internal override void UnpackToCore(Unpacker unpacker, T collection)
    {
      this.GetDelegatedSerializer().UnpackToCore(unpacker, collection);
    }

    public override string ToString()
    {
      return this.GetDelegatedSerializer().ToString();
    }
  }
}
