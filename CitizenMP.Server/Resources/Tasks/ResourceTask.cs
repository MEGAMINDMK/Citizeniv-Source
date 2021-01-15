// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Tasks.ResourceTask
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources.Tasks
{
  public abstract class ResourceTask
  {
    public abstract bool NeedsExecutionFor(Resource resource);

    public abstract Task<bool> Process(Resource resource, Configuration config);

    public string Id
    {
      get
      {
        return this.GetType().Name;
      }
    }

    public abstract IEnumerable<string> DependsOn { get; }
  }
}
