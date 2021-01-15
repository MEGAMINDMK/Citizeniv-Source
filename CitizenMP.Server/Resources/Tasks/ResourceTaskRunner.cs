// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Tasks.ResourceTaskRunner
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using QuickGraph;
using QuickGraph.Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources.Tasks
{
  internal class ResourceTaskRunner
  {
    public async Task<bool> ExecuteTasks(Resource resource, Configuration config)
    {
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      int num = (^this).\u003C\u003E1__state;
      ResourceTaskRunner type = this;
      bool result;
      try
      {
        ResourceTask[] tasks = new ResourceTask[3]
        {
          (ResourceTask) new UpdateStreamListTask(),
          (ResourceTask) new UpdatePackageFileTask(),
          (ResourceTask) new BuildAssemblyTask()
        };
        AdjacencyGraph<string, SEdge<string>> adjacencyGraph = new AdjacencyGraph<string, SEdge<string>>();
        foreach (ResourceTask resourceTask in tasks)
        {
          ResourceTask task = resourceTask;
          adjacencyGraph.AddVertex(task.Id);
          adjacencyGraph.AddEdgeRange(task.DependsOn.Select<string, SEdge<string>>((Func<string, SEdge<string>>) (a => new SEdge<string>(task.Id, a))));
        }
        IEnumerable<ResourceTask> source = ((IEnumerable<string>) AlgorithmExtensions.TopologicalSort<string, SEdge<string>>((IVertexListGraph<M0, M1>) adjacencyGraph)).Reverse<string>().Select<string, ResourceTask>((Func<string, ResourceTask>) (a => ((IEnumerable<ResourceTask>) tasks).First<ResourceTask>((Func<ResourceTask, bool>) (b => b.Id == a)))).Where<ResourceTask>((Func<ResourceTask, bool>) (a => a.NeedsExecutionFor(resource)));
        source.FirstOrDefault<ResourceTask>();
        IEnumerator<ResourceTask> enumerator = source.GetEnumerator();
        try
        {
          while (enumerator.MoveNext())
          {
            ResourceTask task = enumerator.Current;
            TaskAwaiter<bool> awaiter = task.Process(resource, config).GetAwaiter();
            if (!awaiter.IsCompleted)
            {
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003E1__state = num = 0;
              TaskAwaiter<bool> taskAwaiter = awaiter;
              // ISSUE: explicit reference operation
              // ISSUE: reference to a compiler-generated field
              (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, ResourceTaskRunner.\u003CExecuteTasks\u003Ed__0>(ref awaiter, this);
              return;
            }
            if (!awaiter.GetResult())
            {
              type.Log<ResourceTaskRunner>(nameof (ExecuteTasks), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\ResourceTaskRunner.cs", 43).Warn("Task {0} failed.", (object) task.Id);
              result = false;
              goto label_17;
            }
            else
              task = (ResourceTask) null;
          }
        }
        finally
        {
          if (num < 0 && enumerator != null)
            enumerator.Dispose();
        }
        enumerator = (IEnumerator<ResourceTask>) null;
        result = true;
      }
      catch (Exception ex)
      {
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003E1__state = -2;
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003Et__builder.SetException(ex);
        return;
      }
label_17:
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003E1__state = -2;
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003Et__builder.SetResult(result);
    }
  }
}
