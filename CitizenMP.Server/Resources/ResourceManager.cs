// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ResourceManager
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources
{
  public class ResourceManager
  {
    [ThreadStatic]
    private Stack<bool> m_eventCancelationState = new Stack<bool>();
    private Dictionary<string, Resource> m_resources;
    private Configuration m_configuration;
    [ThreadStatic]
    private bool m_eventCanceled;

    public Configuration Configuration
    {
      get
      {
        return this.m_configuration;
      }
    }

    internal GameServer GameServer { get; private set; }

    internal RconLog RconLog { get; private set; }

    public ResourceManager(Configuration config)
    {
      this.m_resources = new Dictionary<string, Resource>();
      this.m_configuration = config;
      this.RconLog = new RconLog();
    }

    internal void SetGameServer(GameServer gameServer)
    {
      if (this.GameServer != null)
        throw new InvalidOperationException("This manager is already associated with a game server.");
      this.GameServer = gameServer;
    }

    public Resource GetResource(string name)
    {
      Resource resource;
      return this.m_resources.TryGetValue(name, out resource) ? resource : (Resource) null;
    }

    public Resource AddResource(string name, string path)
    {
      if (this.m_resources.ContainsKey(name))
        return (Resource) null;
      Resource res = new Resource(name, Path.Combine(Environment.CurrentDirectory, path));
      res.Manager = this;
      res.DownloadConfiguration = this.m_configuration.GetDownloadConfiguration(name);
      this.AddResource(res);
      if (res.Parse())
        return res;
      this.m_resources.Remove(res.Name);
      return (Resource) null;
    }

    public IEnumerable<Resource> GetRunningResources()
    {
      return this.m_resources.Where<KeyValuePair<string, Resource>>((Func<KeyValuePair<string, Resource>, bool>) (r => r.Value.State == ResourceState.Running)).Select<KeyValuePair<string, Resource>, Resource>((Func<KeyValuePair<string, Resource>, Resource>) (r => r.Value));
    }

    public void AddResource(Resource res)
    {
      this.m_resources[res.Name] = res;
    }

    public void ScanResources(string path, string onlyThisResource = null)
    {
      foreach (string directory in Directory.GetDirectories(path))
      {
        string fileName = Path.GetFileName(directory);
        if (fileName[0] == '[')
        {
          if (!fileName.Contains<char>(']'))
            this.Log<ResourceManager>(nameof (ScanResources), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ResourceManager.cs", 105).Info("Ignored {0} - no end bracket", (object) fileName);
          else
            this.ScanResources(directory, onlyThisResource);
        }
        else if (onlyThisResource == null || onlyThisResource == fileName)
          this.AddResource(fileName, directory);
      }
    }

    public void ScanResources(IEnumerable<string> paths)
    {
      foreach (string path in paths)
        this.ScanResources(path, (string) null);
    }

    public void Tick()
    {
      foreach (KeyValuePair<string, Resource> resource in this.m_resources)
        resource.Value.Tick();
    }

    public bool TriggerEvent(string eventName, int source, params object[] args)
    {
      byte[] numArray = Utils.SerializeEvent(args);
      StringBuilder stringBuilder = new StringBuilder(numArray.Length);
      foreach (byte num in numArray)
        stringBuilder.Append((char) num);
      return this.TriggerEvent(eventName, stringBuilder.ToString(), source);
    }

    public bool TriggerEvent(string eventName, string argsSerialized, int source)
    {
      this.m_eventCancelationState.Push(false);
      foreach (KeyValuePair<string, Resource> resource in this.m_resources)
        resource.Value.TriggerEvent(eventName, argsSerialized, source);
      this.m_eventCanceled = this.m_eventCancelationState.Pop();
      return !this.m_eventCanceled;
    }

    public bool WasEventCanceled()
    {
      return this.m_eventCanceled;
    }

    public void CancelEvent()
    {
      this.m_eventCancelationState.Pop();
      this.m_eventCancelationState.Push(true);
    }

    public void StartSynchronization()
    {
      Task.Run((Func<Task>) (async () =>
      {
        foreach (KeyValuePair<string, Resource> resource in this.m_resources)
        {
          if (resource.Value.State == ResourceState.Running)
          {
            DownloadConfiguration downloadConfiguration = this.m_configuration.GetDownloadConfiguration(resource.Key);
            if (downloadConfiguration != null && !string.IsNullOrWhiteSpace(downloadConfiguration.UploadURL))
              await new ResourceUpdater(resource.Value, downloadConfiguration).SyncResource();
          }
        }
      }));
    }
  }
}
