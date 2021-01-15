// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Tasks.BuildAssemblyTask
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources.Tasks
{
  internal class BuildAssemblyTask : ResourceTask, ILogger
  {
    private static object _compileLock = new object();
    private List<string> m_logDump = new List<string>();

    public override IEnumerable<string> DependsOn
    {
      get
      {
        return (IEnumerable<string>) new string[0];
      }
    }

    public override bool NeedsExecutionFor(Resource resource)
    {
      if (resource.Info.ContainsKey("clr_solution"))
      {
        string path = Path.Combine(resource.Path, resource.Info["clr_solution"]);
        if (File.Exists(path))
          return true;
        this.Log<BuildAssemblyTask>(nameof (NeedsExecutionFor), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 34).Warn("Solution {1} for resource {0} does not exist.", (object) resource.Name, (object) path);
      }
      return false;
    }

    public string Parameters { get; set; }

    public LoggerVerbosity Verbosity { get; set; }

    private bool WasError { get; set; }

    public void Initialize(IEventSource eventSource)
    {
      eventSource.ProjectStarted += new ProjectStartedEventHandler(this.ProjectStarted);
      eventSource.TargetStarted += new TargetStartedEventHandler(this.TargetStarted);
      eventSource.TaskStarted += new TaskStartedEventHandler(this.TaskStarted);
      eventSource.ProjectFinished += new ProjectFinishedEventHandler(this.ProjectFinished);
      eventSource.TargetFinished += new TargetFinishedEventHandler(this.TargetFinished);
      eventSource.TaskFinished += new TaskFinishedEventHandler(this.TaskFinished);
      eventSource.MessageRaised += new BuildMessageEventHandler(this.MessageRaised);
      eventSource.WarningRaised += new BuildWarningEventHandler(this.WarningRaised);
      eventSource.StatusEventRaised += new BuildStatusEventHandler(this.StatusEventRaised);
      eventSource.ErrorRaised += new BuildErrorEventHandler(this.ErrorRaised);
    }

    private void ErrorRaised(object sender, BuildErrorEventArgs e)
    {
      this.m_logDump.Add(string.Format("{0} in {1}({2},{3})", (object) e.Message, (object) e.File, (object) e.LineNumber, (object) e.ColumnNumber));
      this.WasError = true;
    }

    private void TaskFinished(object sender, TaskFinishedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Finished task {0}.", (object) e.TaskName));
    }

    private void ProjectFinished(object sender, ProjectFinishedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Finished build for project {0}.", (object) e.ProjectFile));
    }

    private void TargetFinished(object sender, TargetFinishedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Finished build for target {0}.", (object) e.TargetName));
    }

    private void StatusEventRaised(object sender, BuildStatusEventArgs e)
    {
      this.Log<BuildAssemblyTask>(nameof (StatusEventRaised), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 87).Debug("{0}", (object) e.Message);
    }

    private void MessageRaised(object sender, BuildMessageEventArgs e)
    {
      if (!this.WasError)
      {
        this.Log<BuildAssemblyTask>(nameof (MessageRaised), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 94).Debug("{0}", (object) e.Message);
      }
      else
      {
        this.m_logDump.Add(string.Format("{0}", (object) e.Message));
        this.WasError = false;
      }
    }

    private void WarningRaised(object sender, BuildWarningEventArgs e)
    {
      this.m_logDump.Add(string.Format("{0} in {1}({2},{3})", (object) e.Message, (object) e.File, (object) e.LineNumber, (object) e.ColumnNumber));
    }

    private void TaskStarted(object sender, TaskStartedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Started task {0} - {1}.", (object) e.TaskName, (object) e.Message));
    }

    private void TargetStarted(object sender, TargetStartedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Started build for target {0}.", (object) e.TargetName));
    }

    private void ProjectStarted(object sender, ProjectStartedEventArgs e)
    {
      this.m_logDump.Add(string.Format("Started build for project {0}.", (object) e.ProjectFile));
    }

    public void Shutdown()
    {
    }

    public override async Task<bool> Process(Resource resource, Configuration config)
    {
      BuildAssemblyTask type = this;
      string solution = Path.Combine(resource.Path, resource.Info["clr_solution"]);
      Dictionary<string, string> dictionary = new Dictionary<string, string>()
      {
        ["Configuration"] = "Release",
        ["OutputPath"] = Path.GetFullPath(Path.Combine(Path.Combine(Program.RootDirectory, "cache/resource_bin"), resource.Name)) + "/",
        ["IntermediateOutputPath"] = Path.GetFullPath(Path.Combine(Path.Combine(Program.RootDirectory, "cache/resource_obj"), resource.Name)) + "/"
      };
      dictionary["OutDir"] = dictionary["OutputPath"];
      try
      {
        ProjectRootElement projectRoot = ProjectRootElement.Open(Path.GetFullPath(solution));
        foreach (ProjectPropertyElement property in (IEnumerable<ProjectPropertyElement>) projectRoot.Properties)
        {
          if (property.Name == "TargetFrameworkProfile")
            property.Value = "";
        }
        projectRoot.AddProperty("NoStdLib", "true");
        Func<string, string> func = (Func<string, string>) (a => Path.GetFullPath(Path.Combine(Path.Combine(Program.RootDirectory, "system/clrcore"), a + ".dll")));
        foreach (ProjectItemElement projectItemElement in (IEnumerable<ProjectItemElement>) projectRoot.Items)
        {
          ProjectItemElement item = projectItemElement;
          if (item.ItemType == "Reference")
          {
            item.Metadata.Where<ProjectMetadataElement>((Func<ProjectMetadataElement, bool>) (a => a.Name == "HintPath")).ToList<ProjectMetadataElement>().ForEach((Action<ProjectMetadataElement>) (a => item.RemoveChild((ProjectElement) a)));
            item.AddMetadata("HintPath", func(item.Include));
            item.AddMetadata("Private", "false");
          }
        }
        projectRoot.AddItem("Reference", "mscorlib", (IEnumerable<KeyValuePair<string, string>>) new Dictionary<string, string>()
        {
          {
            "HintPath",
            func("mscorlib")
          },
          {
            "Private",
            "false"
          }
        });
        BuildRequestData buildRequest = new BuildRequestData(new ProjectInstance(projectRoot, (IDictionary<string, string>) dictionary, "4.0", ProjectCollection.GlobalProjectCollection), new string[1]
        {
          "Build"
        }, (HostServices) null);
        BuildSubmission buildSubmission = (BuildSubmission) null;
        await Task.Run((Action) (() =>
        {
          lock (BuildAssemblyTask._compileLock)
          {
            string currentDirectory = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetFullPath(Path.GetDirectoryName(solution));
            BuildManager.DefaultBuildManager.BeginBuild(new BuildParameters()
            {
              Loggers = (IEnumerable<ILogger>) new BuildAssemblyTask[1]
              {
                this
              }
            });
            buildSubmission = BuildManager.DefaultBuildManager.PendBuildRequest(buildRequest);
            buildSubmission.Execute();
            BuildManager.DefaultBuildManager.EndBuild();
            Environment.CurrentDirectory = currentDirectory;
          }
        }));
        type.Log<BuildAssemblyTask>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 215).Info("Build for {0} complete - result: {1}", (object) resource.Name, (object) buildSubmission.BuildResult.OverallResult);
        bool flag = buildSubmission.BuildResult.OverallResult == BuildResultCode.Success;
        if (flag)
        {
          string[] strArray = new string[2]
          {
            "Build",
            "CoreBuild"
          };
          Action<TargetResult> action = (Action<TargetResult>) (buildResult =>
          {
            if (buildResult.Items == null)
              return;
            foreach (ITaskItem taskItem in buildResult.Items)
              resource.ExternalFiles[string.Format("bin/{0}", (object) Path.GetFileName(taskItem.ItemSpec))] = new FileInfo(taskItem.ItemSpec);
          });
          foreach (string key in strArray)
          {
            TargetResult targetResult;
            if (buildSubmission.BuildResult.ResultsByTarget.TryGetValue(key, out targetResult))
              action(targetResult);
          }
        }
        else
        {
          foreach (string message in type.m_logDump)
            type.Log<BuildAssemblyTask>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 254).Error(message);
        }
        type.m_logDump.Clear();
        ProjectCollection.GlobalProjectCollection.UnloadProject(projectRoot);
        return flag;
      }
      catch (Exception ex)
      {
        type.Log<BuildAssemblyTask>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\BuildAssemblyTask.cs", 267).Error((Func<string>) (() => "Building assembly failed: " + ex.Message), ex);
        return false;
      }
    }
  }
}
