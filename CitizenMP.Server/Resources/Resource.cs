// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Resource
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Resources.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources
{
  public class Resource : ICallRefHandler
  {
    private FileSystemWatcher m_watcher;
    private ScriptEnvironment m_scriptEnvironment;
    private static bool ms_clientUpdateQueued;

    public string Name { get; private set; }

    public string Path { get; private set; }

    public ResourceState State { get; private set; }

    public Dictionary<string, string> Info { get; private set; }

    public List<string> Dependencies { get; private set; }

    public List<string> Exports { get; private set; }

    public List<string> Scripts { get; private set; }

    public List<string> AuxFiles { get; private set; }

    public List<string> Dependants { get; private set; }

    public List<string> ServerScripts { get; private set; }

    public Dictionary<string, FileInfo> ExternalFiles { get; private set; }

    public DownloadConfiguration DownloadConfiguration { get; set; }

    public ResourceManager Manager { get; set; }

    public string ClientPackageHash { get; internal set; }

    public Resource(string name, string path)
    {
      this.Name = name;
      this.Path = path;
      this.State = ResourceState.Stopped;
      this.Dependants = new List<string>();
      this.StreamEntries = (IDictionary<string, Resource.StreamCacheEntry>) new Dictionary<string, Resource.StreamCacheEntry>();
      this.ExternalFiles = new Dictionary<string, FileInfo>();
    }

    public bool Parse()
    {
      if (!this.EnsureScriptEnvironment())
        return false;
      this.State = ResourceState.Parsing;
      this.Info = new Dictionary<string, string>();
      this.Dependencies = new List<string>();
      this.Exports = new List<string>();
      this.Scripts = new List<string>();
      this.AuxFiles = new List<string>();
      this.ServerScripts = new List<string>();
      int num = this.ParseInfoFile() ? 1 : 0;
      this.State = ResourceState.Stopped;
      return num != 0;
    }

    private bool EnsureScriptEnvironment()
    {
      if (this.m_scriptEnvironment == null)
      {
        this.m_scriptEnvironment = new ScriptEnvironment(this);
        if (!this.m_scriptEnvironment.Create())
        {
          this.Log<Resource>(nameof (EnsureScriptEnvironment), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 74).Error("Resource {0} caused an error during loading. Please see the above lines for details.", (object) this.Name);
          this.State = ResourceState.Stopped;
          return false;
        }
      }
      return true;
    }

    private bool ParseInfoFile()
    {
      return this.m_scriptEnvironment.DoInitFile(true);
    }

    public async Task<bool> Start(Configuration config)
    {
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      int num1 = (^this).\u003C\u003E1__state;
      Resource resource = this;
      bool result;
      try
      {
        if (resource.State == ResourceState.Running)
        {
          result = true;
        }
        else
        {
          if (resource.State != ResourceState.Stopped && resource.State != ResourceState.Starting)
            throw new InvalidOperationException("can not start a resource that is not stopped");
          if (resource.State != ResourceState.Starting)
          {
            if (!resource.Parse())
            {
              resource.State = ResourceState.Error;
              result = false;
              goto label_40;
            }
            else
            {
              List<string>.Enumerator enumerator = resource.Dependencies.GetEnumerator();
              TaskAwaiter<bool> taskAwaiter;
              try
              {
                while (enumerator.MoveNext())
                {
                  string current = enumerator.Current;
                  Resource res = resource.Manager.GetResource(current);
                  if (res == null)
                  {
                    resource.Log<Resource>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 120).Warn("Can't resolve dependency {0} from resource {1}.", (object) current, (object) resource.Name);
                    result = false;
                    goto label_40;
                  }
                  else
                  {
                    TaskAwaiter<bool> awaiter = res.Start(config).GetAwaiter();
                    if (!awaiter.IsCompleted)
                    {
                      // ISSUE: explicit reference operation
                      // ISSUE: reference to a compiler-generated field
                      (^this).\u003C\u003E1__state = num1 = 0;
                      taskAwaiter = awaiter;
                      // ISSUE: explicit reference operation
                      // ISSUE: reference to a compiler-generated field
                      (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, Resource.\u003CStart\u003Ed__62>(ref awaiter, this);
                      return;
                    }
                    awaiter.GetResult();
                    res.AddDependant(resource.Name);
                    res = (Resource) null;
                  }
                }
              }
              finally
              {
                if (num1 < 0)
                  enumerator.Dispose();
              }
              enumerator = new List<string>.Enumerator();
              TaskAwaiter<bool> awaiter1 = new ResourceTaskRunner().ExecuteTasks(resource, config).GetAwaiter();
              if (!awaiter1.IsCompleted)
              {
                int num2;
                // ISSUE: explicit reference operation
                // ISSUE: reference to a compiler-generated field
                (^this).\u003C\u003E1__state = num2 = 1;
                taskAwaiter = awaiter1;
                // ISSUE: explicit reference operation
                // ISSUE: reference to a compiler-generated field
                (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, Resource.\u003CStart\u003Ed__62>(ref awaiter1, this);
                return;
              }
              if (!awaiter1.GetResult())
              {
                resource.Log<Resource>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 133).Error("Executing tasks for resource {0} failed.", (object) resource.Name);
                resource.State = ResourceState.Stopped;
                result = false;
                goto label_40;
              }
              else
              {
                resource.m_watcher = new FileSystemWatcher();
                if (!resource.EnsureScriptEnvironment())
                {
                  result = false;
                  goto label_40;
                }
              }
            }
          }
          resource.State = ResourceState.Starting;
          if (!resource.Manager.TriggerEvent("onResourceStarting", -1, (object) resource.Name))
          {
            if (resource.State == ResourceState.Running)
            {
              result = true;
            }
            else
            {
              resource.Stop();
              result = false;
            }
          }
          else
          {
            resource.m_scriptEnvironment.DoInitFile(false);
            resource.m_scriptEnvironment.LoadScripts();
            resource.m_watcher.Path = resource.Path;
            resource.m_watcher.IncludeSubdirectories = true;
            resource.m_watcher.NotifyFilter = NotifyFilters.LastWrite;
            resource.m_watcher.Changed += (FileSystemEventHandler) ((s, e) => this.InvokeUpdateClientPackage(config));
            resource.m_watcher.Created += (FileSystemEventHandler) ((s, e) => this.InvokeUpdateClientPackage(config));
            resource.m_watcher.Deleted += (FileSystemEventHandler) ((s, e) => this.InvokeUpdateClientPackage(config));
            resource.m_watcher.Renamed += (RenamedEventHandler) ((s, e) => this.InvokeUpdateClientPackage(config));
            resource.m_watcher.EnableRaisingEvents = true;
            resource.State = ResourceState.Running;
            if (!resource.Manager.TriggerEvent("onResourceStart", -1, (object) resource.Name))
            {
              resource.Log<Resource>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 185).Info("Resource start canceled by event.");
              resource.Stop();
              result = false;
            }
            else
            {
              resource.Log<Resource>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 194).Info("{0} successfully loaded.", (object) resource.Name);
              IEnumerator<Client> enumerator = ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.NetChannel != null)).Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (c => c.Value)).GetEnumerator();
              try
              {
                while (enumerator.MoveNext())
                  enumerator.Current.SendReliableCommand(2951007562U, Encoding.UTF8.GetBytes(resource.Name));
              }
              finally
              {
                if (num1 < 0 && enumerator != null)
                  enumerator.Dispose();
              }
              result = true;
            }
          }
        }
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
label_40:
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003E1__state = -2;
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003Et__builder.SetResult(result);
    }

    public void RemoveDependant(string name)
    {
      this.Dependants.Remove(name);
    }

    public bool Stop()
    {
      if (this.State != ResourceState.Running && this.State != ResourceState.Starting)
        throw new InvalidOperationException(string.Format("Tried to stop a resource ({0}) that wasn't running.", (object) this.Name));
      this.Log<Resource>(nameof (Stop), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Resource.cs", 219).Info("Stopping resource {0} (last state: {1}).", (object) this.Name, (object) this.State);
      if (this.State == ResourceState.Running)
      {
        if (!this.Manager.TriggerEvent("onResourceStop", -1, (object) this.Name))
          return false;
      }
      foreach (string name in this.Dependants.GetRange(0, this.Dependants.Count))
        this.Manager.GetResource(name).Stop();
      this.Dependants.Clear();
      foreach (string dependency in this.Dependencies)
        this.Manager.GetResource(dependency).RemoveDependant(this.Name);
      this.m_watcher.Dispose();
      this.m_watcher = (FileSystemWatcher) null;
      this.m_scriptEnvironment.Dispose();
      this.m_scriptEnvironment = (ScriptEnvironment) null;
      if (this.State == ResourceState.Running)
      {
        foreach (Client client in ClientInstances.Clients.Where<KeyValuePair<string, Client>>((Func<KeyValuePair<string, Client>, bool>) (c => c.Value.NetChannel != null)).Select<KeyValuePair<string, Client>, Client>((Func<KeyValuePair<string, Client>, Client>) (c => c.Value)))
          client.SendReliableCommand(1172854231U, Encoding.UTF8.GetBytes(this.Name));
      }
      this.State = ResourceState.Stopped;
      return true;
    }

    public bool HasRef(int reference, uint instance)
    {
      return this.m_scriptEnvironment != null && this.m_scriptEnvironment.HasRef(reference) && (int) this.m_scriptEnvironment.InstanceID == (int) instance;
    }

    public void Tick()
    {
      if (this.m_scriptEnvironment == null)
        return;
      this.m_scriptEnvironment.Tick();
    }

    public Delegate GetRef(int reference)
    {
      return this.m_scriptEnvironment.GetRef(reference);
    }

    public string CallRef(Delegate method, string argsSerialized)
    {
      return this.m_scriptEnvironment.CallExport(method, argsSerialized);
    }

    public void RemoveRef(int luaRef)
    {
      this.m_scriptEnvironment.RemoveRef(luaRef);
    }

    private void InvokeUpdateClientPackage(Configuration config)
    {
      if (Resource.ms_clientUpdateQueued)
        return;
      Resource.ms_clientUpdateQueued = true;
      Task.Run((Func<Task>) (async () =>
      {
        await Task.Delay(500);
        new ResourceTaskRunner().ExecuteTasks(this, config);
        Resource.ms_clientUpdateQueued = false;
      }));
    }

    public void AddDependant(string name)
    {
      this.Dependants.Add(name);
    }

    public IDictionary<string, Resource.StreamCacheEntry> StreamEntries { get; set; }

    public bool IsSynchronizing { get; set; }

    public Stream OpenClientPackage()
    {
      return (Stream) File.Open("cache/http-files/" + this.Name + ".rpf", FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public FileInfo GetClientPackageInfo()
    {
      return new FileInfo("cache/http-files/" + this.Name + ".rpf");
    }

    public IEnumerable<FileInfo> GetStreamFilesInfo()
    {
      return this.StreamEntries.Where<KeyValuePair<string, Resource.StreamCacheEntry>>((Func<KeyValuePair<string, Resource.StreamCacheEntry>, bool>) (e => e.Value.FileName != null)).Select<KeyValuePair<string, Resource.StreamCacheEntry>, FileInfo>((Func<KeyValuePair<string, Resource.StreamCacheEntry>, FileInfo>) (e => new FileInfo(e.Value.FileName)));
    }

    public Stream GetStreamFile(string baseName)
    {
      Resource.StreamCacheEntry streamCacheEntry;
      if (!this.StreamEntries.TryGetValue(baseName, out streamCacheEntry))
        return (Stream) null;
      return streamCacheEntry.FileName == null ? (Stream) null : (Stream) File.Open(streamCacheEntry.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
    }

    public void TriggerEvent(string eventName, string argsSerialized, int source)
    {
      if (this.State != ResourceState.Running)
        return;
      this.m_scriptEnvironment.TriggerEvent(eventName, argsSerialized, source);
    }

    public class StreamCacheEntry
    {
      public string BaseName { get; set; }

      public string HashString { get; set; }

      public string FileName { get; set; }

      public uint RscFlags { get; set; }

      public uint RscVersion { get; set; }

      public uint Size { get; set; }
    }

    private class ResourceData
    {
      public Dictionary<string, string> info { get; set; }

      public List<string> exports { get; set; }

      public List<string> dependencies { get; set; }

      public List<string> scripts { get; set; }

      public List<string> auxFiles { get; set; }

      public List<string> serverScripts { get; set; }
    }
  }
}
