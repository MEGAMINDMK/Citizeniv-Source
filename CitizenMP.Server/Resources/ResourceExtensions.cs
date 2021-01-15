// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ResourceExtensions
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace CitizenMP.Server.Resources
{
  public static class ResourceExtensions
  {
    public static void GenerateConfiguration(
      this IEnumerable<Resource> resourceSource,
      JArray array,
      Action<Resource, JObject> filter = null)
    {
      foreach (Resource resource in resourceSource)
      {
        JObject jobject1 = new JObject();
        jobject1.set_Item("resource.rpf", JToken.op_Implicit(resource.ClientPackageHash));
        JObject jobject2 = new JObject();
        foreach (KeyValuePair<string, Resource.StreamCacheEntry> streamEntry in (IEnumerable<KeyValuePair<string, Resource.StreamCacheEntry>>) resource.StreamEntries)
        {
          JObject jobject3 = new JObject();
          jobject3.set_Item("hash", JToken.op_Implicit(streamEntry.Value.HashString));
          jobject3.set_Item("rscFlags", JToken.op_Implicit(streamEntry.Value.RscFlags));
          jobject3.set_Item("rscVersion", JToken.op_Implicit(streamEntry.Value.RscVersion));
          jobject3.set_Item("size", JToken.op_Implicit(streamEntry.Value.Size));
          jobject2.set_Item(streamEntry.Value.BaseName, (JToken) jobject3);
        }
        JObject jobject4 = new JObject();
        jobject4.set_Item("name", JToken.op_Implicit(resource.Name));
        jobject4.set_Item("files", (JToken) jobject1);
        jobject4.set_Item("streamFiles", (JToken) jobject2);
        if (filter != null)
          filter(resource, jobject4);
        array.Add((JToken) jobject4);
      }
    }
  }
}
