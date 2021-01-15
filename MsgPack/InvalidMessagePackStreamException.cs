// Decompiled with JetBrains decompiler
// Type: MsgPack.InvalidMessagePackStreamException
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack
{
  [Serializable]
  public sealed class InvalidMessagePackStreamException : Exception
  {
    public InvalidMessagePackStreamException()
      : this((string) null)
    {
    }

    public InvalidMessagePackStreamException(string message)
      : this(message, (Exception) null)
    {
    }

    public InvalidMessagePackStreamException(string message, Exception inner)
      : base(message ?? "Stream is not valid as serialized Message Pack object.", inner)
    {
    }

    private InvalidMessagePackStreamException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
