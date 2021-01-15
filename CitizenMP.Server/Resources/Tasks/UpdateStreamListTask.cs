// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Tasks.UpdateStreamListTask
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources.Tasks
{
  internal class UpdateStreamListTask : ResourceTask
  {
    public override IEnumerable<string> DependsOn
    {
      get
      {
        return (IEnumerable<string>) new string[0];
      }
    }

    public override bool NeedsExecutionFor(Resource resource)
    {
      string str1 = Path.Combine(resource.Path, "streamcache.sfl");
      if (File.Exists(str1))
      {
        this.LoadStreamCacheList(resource, (string[]) null, str1);
        return false;
      }
      string path = Path.Combine(resource.Path, "stream");
      if (!Directory.Exists(path))
        return false;
      string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
      string str2 = string.Format("cache/http-files/{0}.sfl", (object) resource.Name);
      bool flag = false;
      if (!File.Exists(str2))
      {
        this.Log<UpdateStreamListTask>(nameof (NeedsExecutionFor), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\UpdateStreamListTask.cs", 43).Info("Generating stream cache list for {0} (no stream cache)", (object) resource.Name);
        return true;
      }
      if (!flag && ((IEnumerable<string>) files).Select<string, DateTime>((Func<string, DateTime>) (a => File.GetLastWriteTime(a))).OrderByDescending<DateTime, DateTime>((Func<DateTime, DateTime>) (a => a)).First<DateTime>() > File.GetLastWriteTime(str2))
      {
        this.Log<UpdateStreamListTask>(nameof (NeedsExecutionFor), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\UpdateStreamListTask.cs", 55).Info("Generating stream cache list for {0} (modification dates differ)", (object) resource.Name);
        return true;
      }
      this.LoadStreamCacheList(resource, files, str2);
      return false;
    }

    public override Task<bool> Process(Resource resource, Configuration config)
    {
      string[] files = Directory.GetFiles(Path.Combine(resource.Path, "stream"), "*.*", SearchOption.AllDirectories);
      string cacheFilename = string.Format("cache/http-files/{0}.sfl", (object) resource.Name);
      return Task.FromResult<bool>(this.CreateStreamCacheList(resource, files, cacheFilename));
    }

    private bool CreateStreamCacheList(Resource resource, string[] files, string cacheFilename)
    {
      JArray jarray = new JArray();
      foreach (string file in files)
      {
        string fileShA1String = Utils.GetFileSHA1String(file);
        string fileName = Path.GetFileName(file);
        FileStream fileStream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read);
        BinaryReader binaryReader = new BinaryReader((Stream) fileStream);
        long length = fileStream.Length;
        long num1 = length;
        int num2 = 0;
        if (binaryReader.ReadUInt32() == 88298322U)
        {
          num2 = binaryReader.ReadInt32();
          num1 = (long) binaryReader.ReadUInt32();
        }
        JObject jobject = new JObject();
        jobject.set_Item("Hash", JToken.op_Implicit(fileShA1String));
        jobject.set_Item("BaseName", JToken.op_Implicit(fileName));
        jobject.set_Item("Size", JToken.op_Implicit(length));
        jobject.set_Item("RscFlags", JToken.op_Implicit(num1));
        jobject.set_Item("RscVersion", JToken.op_Implicit(num2));
        jarray.Add((JToken) jobject);
      }
      File.WriteAllText(cacheFilename, ((JToken) jarray).ToString((Formatting) 0, new JsonConverter[0]));
      this.LoadStreamCacheList(resource, files, cacheFilename);
      return true;
    }

    private bool LoadStreamCacheList(Resource resource, string[] files, string cacheFile)
    {
      JArray jarray = JArray.Parse(File.ReadAllText(cacheFile));
      Dictionary<string, Resource.StreamCacheEntry> dictionary = new Dictionary<string, Resource.StreamCacheEntry>();
      using (IEnumerator<JToken> enumerator = jarray.GetEnumerator())
      {
        while (((IEnumerator) enumerator).MoveNext())
        {
          if (enumerator.Current is JObject current)
          {
            Resource.StreamCacheEntry streamCacheEntry = new Resource.StreamCacheEntry();
            streamCacheEntry.BaseName = (string) ((JToken) current).Value<string>((object) "BaseName");
            streamCacheEntry.HashString = (string) ((JToken) current).Value<string>((object) "Hash");
            streamCacheEntry.RscFlags = (uint) ((JToken) current).Value<uint>((object) "RscFlags");
            streamCacheEntry.RscVersion = (uint) ((JToken) current).Value<uint>((object) "RscVersion");
            streamCacheEntry.Size = (uint) ((JToken) current).Value<uint>((object) "Size");
            if (!streamCacheEntry.BaseName.Contains(".wpl") || !streamCacheEntry.BaseName.Contains("ktown_stream") && !streamCacheEntry.BaseName.Contains("venice_stream") && (!streamCacheEntry.BaseName.Contains("santamon_stream") && !streamCacheEntry.BaseName.Contains("beverly_stream")) && (!streamCacheEntry.BaseName.Contains("sanpedro_stream") && !streamCacheEntry.BaseName.Contains("airport_stream") && (!streamCacheEntry.BaseName.Contains("downtown_stream") && !streamCacheEntry.BaseName.Contains("hollywood_stream"))) && (!streamCacheEntry.BaseName.Contains("scentral_stream") && !streamCacheEntry.BaseName.Contains("indust_stream") && !streamCacheEntry.BaseName.Contains("port_stream")))
              dictionary.Add((string) ((JToken) current).Value<string>((object) "BaseName"), streamCacheEntry);
          }
        }
      }
      if (files != null)
      {
        foreach (string file in files)
        {
          string fileName = Path.GetFileName(file);
          if (dictionary.ContainsKey(fileName))
            dictionary[fileName].FileName = file;
        }
      }
      resource.StreamEntries = (IDictionary<string, Resource.StreamCacheEntry>) dictionary;
      return true;
    }
  }
}
