// Decompiled with JetBrains decompiler
// Type: MsgPack.MessagePackObjectEqualityComparer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System;
using System.Collections.Generic;

namespace MsgPack
{
  [Serializable]
  public sealed class MessagePackObjectEqualityComparer : IEqualityComparer<MessagePackObject>
  {
    private static readonly MessagePackObjectEqualityComparer _instance = new MessagePackObjectEqualityComparer();

    internal static MessagePackObjectEqualityComparer Instance
    {
      get
      {
        return MessagePackObjectEqualityComparer._instance;
      }
    }

    public bool Equals(MessagePackObject x, MessagePackObject y)
    {
      return x.Equals(y);
    }

    public int GetHashCode(MessagePackObject obj)
    {
      return obj.GetHashCode();
    }
  }
}
