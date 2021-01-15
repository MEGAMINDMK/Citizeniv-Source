// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.HostingProcess.EntryPoint
// Assembly: vshost32, Version=14.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F6E3CE38-7486-468B-BA4C-DA2530F4E4BF
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.vshost.exe

using System.Diagnostics;
using System.Threading;

namespace Microsoft.VisualStudio.HostingProcess
{
  public sealed class EntryPoint
  {
    private EntryPoint()
    {
    }

    [DebuggerNonUserCode]
    public static void Main()
    {
      if (Synchronize.get_HostingProcessInitialized() == null)
        return;
      Synchronize.get_HostingProcessInitialized().Set();
      if (Synchronize.get_StartRunningUsersAssembly() == null || Synchronize.get_ShutdownProcessEvent() == null || Synchronize.get_Shutdown() == null)
        return;
      WaitHandle.WaitAny(new WaitHandle[2]
      {
        (WaitHandle) Synchronize.get_StartRunningUsersAssembly(),
        (WaitHandle) Synchronize.get_ShutdownProcessEvent()
      });
      Synchronize.get_Shutdown().Invoke();
    }
  }
}
