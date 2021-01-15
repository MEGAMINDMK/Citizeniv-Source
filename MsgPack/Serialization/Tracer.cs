// Decompiled with JetBrains decompiler
// Type: MsgPack.Serialization.Tracer
// Assembly: MsgPack, Version=0.6.0.0, Culture=neutral, PublicKeyToken=a2625990d5dc0167
// MVID: 83295EEB-B7AD-4F18-AE72-5034C35D07DE
// Assembly location: C:\Users\MEGA\Downloads\MsgPack.dll

using System.Diagnostics;

namespace MsgPack.Serialization
{
  internal static class Tracer
  {
    public static readonly TraceSource Emit = new TraceSource("MsgPack.Serialization.Emit");

    public static class EventId
    {
      public const int ILTrace = 101;
      public const int DefineType = 102;
      public const int NoAccessorFound = 901;
      public const int MultipleAccessorFound = 902;
      public const int ReadOnlyValueTypeMember = 903;
      public const int UnsupportedType = 10901;
    }

    public static class EventType
    {
      public const TraceEventType ILTrace = TraceEventType.Verbose;
      public const TraceEventType DefineType = TraceEventType.Verbose;
      public const TraceEventType NoAccessorFound = TraceEventType.Verbose;
      public const TraceEventType MultipleAccessorFound = TraceEventType.Verbose;
      public const TraceEventType ReadOnlyValueTypeMember = TraceEventType.Verbose;
      public const TraceEventType UnsupportedType = TraceEventType.Information;
    }
  }
}
