// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Resources.ResourceUpdater
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.FtpClient;
using System.Threading.Tasks;

namespace CitizenMP.Server.Resources
{
  internal class ResourceUpdater
  {
    private Resource m_resource;
    private string m_uploadURL;
    private string m_baseURL;

    public ResourceUpdater(Resource resource, DownloadConfiguration config)
    {
      this.m_resource = resource;
      this.m_uploadURL = config.UploadURL;
      this.m_baseURL = config.BaseURL;
    }

    public async Task SyncResource()
    {
      ResourceUpdater type = this;
      if (type.m_resource.IsSynchronizing)
        return;
      try
      {
        System.Net.FtpClient.FtpClient client = new System.Net.FtpClient.FtpClient();
        Uri url = new Uri(type.m_uploadURL);
        client.set_Host(url.Host);
        client.set_Port(url.Port == -1 ? 21 : url.Port);
        string[] strArray = url.UserInfo.Split(new char[1]
        {
          ':'
        }, 2);
        if (strArray.Length == 2)
          client.set_Credentials(new NetworkCredential(strArray[0], strArray[1]));
        if (url.Scheme == "ftps")
        {
          client.set_EncryptionMode((FtpEncryptionMode) 2);
          client.set_DataConnectionEncryption(false);
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          client.add_ValidateCertificate(ResourceUpdater.\u003C\u003Ec.\u003C\u003E9__4_0 ?? (ResourceUpdater.\u003C\u003Ec.\u003C\u003E9__4_0 = new FtpSslValidation((object) ResourceUpdater.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CSyncResource\u003Eb__4_0))));
        }
        await Task.Factory.FromAsync(new Func<AsyncCallback, object, IAsyncResult>(client.BeginConnect), new Action<IAsyncResult>(client.EndConnect), (object) null);
        IEnumerable<FileInfo> filesNeedingUpdate = (IEnumerable<FileInfo>) null;
        bool needsCreate = false;
        List<FileInfo> localListing = new List<FileInfo>();
        localListing.Add(type.m_resource.GetClientPackageInfo());
        localListing.AddRange(type.m_resource.GetStreamFilesInfo());
        Func<string, string> mapName = (Func<string, string>) (n => n.EndsWith(".rpf") ? "resource.rpf" : n);
        try
        {
          Dictionary<string, FtpListItem> listDictionary = ((IEnumerable<FtpListItem>) await Task.Factory.FromAsync<string, FtpListOption, FtpListItem[]>(new Func<string, FtpListOption, AsyncCallback, object, IAsyncResult>(client.BeginGetListing), new Func<IAsyncResult, FtpListItem[]>(client.EndGetListing), url.AbsolutePath + "/" + type.m_resource.Name, (FtpListOption) 1, (object) null)).Where<FtpListItem>((Func<FtpListItem, bool>) (i => i.get_Type() == 0)).ToDictionary<FtpListItem, string>((Func<FtpListItem, string>) (i => i.get_Name()));
          filesNeedingUpdate = localListing.Where<FileInfo>((Func<FileInfo, bool>) (f => !listDictionary.ContainsKey(mapName(f.Name)) || listDictionary[mapName(f.Name)].get_Modified() < f.LastWriteTime));
          type.Log<ResourceUpdater>(nameof (SyncResource), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ResourceUpdater.cs", 91).Info("Updating {0}: {1} files to update", (object) type.m_resource.Name, (object) filesNeedingUpdate.Count<FileInfo>());
        }
        catch (FtpCommandException ex)
        {
          needsCreate = true;
        }
        if (needsCreate)
        {
          await Task.Factory.FromAsync<string, bool>(new Func<string, bool, AsyncCallback, object, IAsyncResult>(client.BeginCreateDirectory), new Action<IAsyncResult>(client.EndCreateDirectory), url.AbsolutePath + "/" + type.m_resource.Name, true, (object) null);
          filesNeedingUpdate = (IEnumerable<FileInfo>) localListing;
        }
        if (filesNeedingUpdate != null)
        {
          foreach (FileInfo fileInfo in filesNeedingUpdate)
          {
            FileInfo file = fileInfo;
            Stream outStream = await Task.Factory.FromAsync<string, FtpDataType, Stream>(new Func<string, FtpDataType, AsyncCallback, object, IAsyncResult>(client.BeginOpenWrite), new Func<IAsyncResult, Stream>(client.EndOpenWrite), url.AbsolutePath + "/" + type.m_resource.Name + "/" + mapName(file.Name), (FtpDataType) 1, (object) null);
            await file.OpenRead().CopyToAsync(outStream);
            outStream.Close();
            type.Log<ResourceUpdater>(nameof (SyncResource), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ResourceUpdater.cs", 116).Info("Uploaded {0}/{1}\n", (object) type.m_resource.Name, (object) file.Name);
            outStream = (Stream) null;
            file = (FileInfo) null;
          }
        }
        StreamWriter outWriter = new StreamWriter((Stream) new BufferedStream(await Task.Factory.FromAsync<string, FtpDataType, Stream>(new Func<string, FtpDataType, AsyncCallback, object, IAsyncResult>(client.BeginOpenWrite), new Func<IAsyncResult, Stream>(client.EndOpenWrite), url.AbsolutePath + "/" + type.m_resource.Name + ".json", (FtpDataType) 0, (object) null)));
        JObject jobject = new JObject();
        jobject.set_Item("fileServer", JToken.op_Implicit(type.m_baseURL));
        JArray array = new JArray();
        ((IEnumerable<Resource>) new Resource[1]
        {
          type.m_resource
        }).GenerateConfiguration(array, (Action<Resource, JObject>) null);
        jobject.set_Item("resources", (JToken) array);
        await outWriter.WriteAsync(((JToken) jobject).ToString((Formatting) 0, new JsonConverter[0]));
        await outWriter.FlushAsync();
        outWriter.Close();
        outWriter = (StreamWriter) null;
        type.Log<ResourceUpdater>(nameof (SyncResource), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ResourceUpdater.cs", 139).Info("Done updating {0}.", (object) type.m_resource.Name);
        client = (System.Net.FtpClient.FtpClient) null;
        url = (Uri) null;
        filesNeedingUpdate = (IEnumerable<FileInfo>) null;
        localListing = (List<FileInfo>) null;
      }
      catch (Exception ex)
      {
        type.Log<ResourceUpdater>(nameof (SyncResource), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Resources\\ResourceUpdater.cs", 143).Error(ex.ToString());
      }
    }
  }
}
