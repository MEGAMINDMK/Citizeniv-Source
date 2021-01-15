// Decompiled with JetBrains decompiler
// Type: MsgPack.UnassignedMessageTypeException
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack
{
  [Serializable]
  public sealed class UnassignedMessageTypeException : MessageTypeException
  {
    public UnassignedMessageTypeException()
      : this((string) null)
    {
    }

    public UnassignedMessageTypeException(string message)
      : this(message, (Exception) null)
    {
    }

    public UnassignedMessageTypeException(string message, Exception inner)
      : base(message ?? "Invalid message type.", inner)
    {
    }

    private UnassignedMessageTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
