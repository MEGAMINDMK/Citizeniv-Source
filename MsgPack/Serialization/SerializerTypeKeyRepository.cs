// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.SerializerTypeKeyRepository
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;
using System.Security;

namespace MsgPack.Serialization
{
  [SecuritySafeCritical]
  internal sealed class SerializerTypeKeyRepository : TypeKeyRepository
  {
    public SerializerTypeKeyRepository()
    {
    }

    public SerializerTypeKeyRepository(SerializerTypeKeyRepository copiedFrom)
      : base((TypeKeyRepository) copiedFrom)
    {
    }

    public SerializerTypeKeyRepository(Dictionary<RuntimeTypeHandle, object> table)
      : base(table)
    {
    }

    public object Get(SerializationContext context, Type keyType)
    {
      object matched;
      object genericDefinitionMatched;
      if (!this.Get(keyType, out matched, out genericDefinitionMatched))
        return (object) null;
      if (matched != null)
        return matched;
      return ReflectionExtensions.CreateInstancePreservingExceptionType((genericDefinitionMatched as Type).MakeGenericType(keyType.GetGenericArguments()), (object) context);
    }
  }
}
