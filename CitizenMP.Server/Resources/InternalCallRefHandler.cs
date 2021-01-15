// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.InternalCallRefHandler
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System;
using System.Collections.Concurrent;
using System.Threading;

namespace CitizenMP.Server.Resources
{
  internal class InternalCallRefHandler : ICallRefHandler
  {
    private ConcurrentDictionary<int, Delegate> m_callbacks = new ConcurrentDictionary<int, Delegate>();
    private int m_callbackId;
    private static InternalCallRefHandler ms_instance;

    public int AddCallback(Delegate deleg)
    {
      int key = Interlocked.Increment(ref this.m_callbackId);
      this.m_callbacks.TryAdd(key, deleg);
      return key;
    }

    public Delegate GetRef(int index)
    {
      Delegate @delegate;
      return this.m_callbacks.TryGetValue(index, out @delegate) ? @delegate : (Delegate) null;
    }

    public bool HasRef(int index, uint instance)
    {
      return this.m_callbacks.TryGetValue(index, out Delegate _);
    }

    public static InternalCallRefHandler Get()
    {
      if (InternalCallRefHandler.ms_instance == null)
        InternalCallRefHandler.ms_instance = new InternalCallRefHandler();
      return InternalCallRefHandler.ms_instance;
    }
  }
}
