// Decompiled with JetBrains decompiler
// Type: MsgPack.MessageTypeException
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack
{
  [Serializable]
  public class MessageTypeException : Exception
  {
    public MessageTypeException()
      : this((string) null)
    {
    }

    public MessageTypeException(string message)
      : this(message, (Exception) null)
    {
    }

    public MessageTypeException(string message, Exception inner)
      : base(message ?? "Invalid message type.", inner)
    {
    }

    protected MessageTypeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
