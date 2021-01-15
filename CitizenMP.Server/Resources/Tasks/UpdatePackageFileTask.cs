// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.Tasks.UpdatePackageFileTask
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources.Tasks
{
  internal class UpdatePackageFileTask : ResourceTask
  {
    private IEnumerable<string> GetRequiredFilesFor(Resource resource)
    {
      List<string> stringList = new List<string>();
      stringList.Add("__resource.lua");
      stringList.AddRange((IEnumerable<string>) resource.Scripts);
      stringList.AddRange((IEnumerable<string>) resource.AuxFiles);
      return (IEnumerable<string>) stringList;
    }

    private string GetRpfNameFor(Resource resource)
    {
      return Path.Combine(Program.RootDirectory, "cache/http-files/" + resource.Name + ".rpf");
    }

    public override bool NeedsExecutionFor(Resource resource)
    {
      string rpfNameFor = this.GetRpfNameFor(resource);
      if (!File.Exists(rpfNameFor))
        return true;
      DateTime dateTime1 = this.GetRequiredFilesFor(resource).Select<string, string>((Func<string, string>) (a => Path.Combine(resource.Path, a))).Concat<string>(resource.ExternalFiles.Select<KeyValuePair<string, FileInfo>, string>((Func<KeyValuePair<string, FileInfo>, string>) (a => a.Value.FullName))).Select<string, DateTime>((Func<string, DateTime>) (a => File.GetLastWriteTime(a))).OrderByDescending<DateTime, DateTime>((Func<DateTime, DateTime>) (a => a)).First<DateTime>();
      DateTime lastWriteTime1 = File.GetLastWriteTime(rpfNameFor);
      DateTime lastWriteTime2 = File.GetLastWriteTime("citmp-server.yml");
      DateTime dateTime2 = lastWriteTime1;
      if (dateTime1 > dateTime2 || lastWriteTime2 > lastWriteTime1)
        return true;
      string fileShA1String = Utils.GetFileSHA1String(rpfNameFor);
      resource.ClientPackageHash = fileShA1String;
      return false;
    }

    public override async Task<bool> Process(Resource resource, Configuration config)
    {
      UpdatePackageFileTask type = this;
      if (!Directory.Exists("cache/http-files/"))
        Directory.CreateDirectory("cache/http-files");
      string rpfNameFor = type.GetRpfNameFor(resource);
      try
      {
        RPFFile rpf = new RPFFile();
        type.GetRequiredFilesFor(resource).Where<string>((Func<string, bool>) (a => File.Exists(Path.Combine(resource.Path, a)))).ToList<string>().ForEach((Action<string>) (a =>
        {
          if (Path.Combine(resource.Path, a).Contains(".lua") && config.Encryption)
          {
            rpf.AddFile(a, Encoding.Default.GetBytes(EncryptHelper.AESEncrypt(File.ReadAllText(Path.Combine(resource.Path, a), Encoding.Default) + "--encryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshitencryptionisshit", "SA#%^433@!#$&#$%")));
            this.Log<UpdatePackageFileTask>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\UpdatePackageFileTask.cs", 88).Info("Successfully encrypted: {0}", (object) a);
          }
          else
            rpf.AddFile(a, File.ReadAllBytes(Path.Combine(resource.Path, a)));
        }));
        resource.ExternalFiles.ToList<KeyValuePair<string, FileInfo>>().ForEach((Action<KeyValuePair<string, FileInfo>>) (a => rpf.AddFile(a.Key, File.ReadAllBytes(a.Value.FullName))));
        rpf.Write(rpfNameFor);
        resource.ClientPackageHash = Utils.GetFileSHA1String(rpfNameFor);
        if (resource.DownloadConfiguration != null && !string.IsNullOrWhiteSpace(resource.DownloadConfiguration.UploadURL))
        {
          ResourceUpdater resourceUpdater = new ResourceUpdater(resource, resource.DownloadConfiguration);
          resource.IsSynchronizing = true;
          await resourceUpdater.SyncResource();
          resource.IsSynchronizing = false;
        }
        return true;
      }
      catch (Exception ex)
      {
        type.Log<UpdatePackageFileTask>(nameof (Process), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\Tasks\\UpdatePackageFileTask.cs", 123).Error((Func<string>) (() => "Error updating client cache file: " + ex.Message), ex);
      }
      return false;
    }

    public override IEnumerable<string> DependsOn
    {
      get
      {
        return (IEnumerable<string>) new string[1]
        {
          "BuildAssemblyTask"
        };
      }
    }
  }
}
