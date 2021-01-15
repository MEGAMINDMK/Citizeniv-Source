// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.HTTP.GetConfigurationMethod
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Resources;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Headers;

namespace CitizenMP.Server.HTTP
{
  internal static class GetConfigurationMethod
  {
    public static Func<IHttpHeaders, IHttpContext, Task<JObject>> Get(
      Configuration config,
      ResourceManager resourceMgr)
    {
      return (Func<IHttpHeaders, IHttpContext, Task<JObject>>) ((headers, context) =>
      {
        JObject result = new JObject();
        JArray array = new JArray();
        IEnumerable<Resource> resources = resourceMgr.GetRunningResources();
        string str;
        if (headers.TryGetByName("resources", ref str))
        {
          string[] resourceNames = str.Split(';');
          resources = resources.Where<Resource>((Func<Resource, bool>) (r => ((IEnumerable<string>) resourceNames).Contains<string>(r.Name)));
        }
        else if (config.Imports != null)
        {
          JArray imports = new JArray();
          config.Imports.ForEach((Action<ImportConfiguration>) (a => imports.Add(JToken.op_Implicit(a.ConfigURL))));
          result.set_Item("imports", (JToken) imports);
        }
        resources.GenerateConfiguration(array, (Action<Resource, JObject>) ((resource, rObject) =>
        {
          DownloadConfiguration downloadConfiguration = resourceMgr.Configuration.GetDownloadConfiguration(resource.Name);
          if (downloadConfiguration == null || string.IsNullOrWhiteSpace(downloadConfiguration.BaseURL))
            return;
          rObject.set_Item("fileServer", JToken.op_Implicit(downloadConfiguration.BaseURL));
        }));
        result.set_Item("resources", (JToken) array);
        result.set_Item("fileServer", JToken.op_Implicit("http://%s/files/"));
        result.set_Item("loadScreen", JToken.op_Implicit("nui://keks/index.html"));
        TaskCompletionSource<JObject> completionSource = new TaskCompletionSource<JObject>();
        completionSource.SetResult(result);
        return completionSource.Task;
      });
    }
  }
}
