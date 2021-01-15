// Decompiled with JetBrains decompiler
// Type: MsgPack.UnpackException
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Runtime.Serialization;

namespace MsgPack
{
  [Serializable]
  public sealed class UnpackException : Exception
  {
    public UnpackException()
      : this((string) null)
    {
    }

    public UnpackException(string message)
      : this(message, (Exception) null)
    {
    }

    public UnpackException(string message, Exception inner)
      : base(message ?? "Failed to unpacking.", inner)
    {
    }

    private UnpackException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}
