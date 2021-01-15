// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Configuration
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace CitizenMP.Server
{
  public class Configuration
  {
    public string serverVersion = "1.4";
    public string cleanServerVersion = "14";

    public static Configuration Load(string filename)
    {
      return (Configuration) new Deserializer((IObjectFactory) null, (INamingConvention) null, true).Deserialize<Configuration>((TextReader) new StringReader(File.ReadAllText(filename)));
    }

    public bool ScriptDebug { get; set; }

    public List<string> AutoStartResources { get; set; }

    public List<string> PreParseResources { get; set; }

    public string RconPassword { get; set; }

    public int ListenPort { get; set; }

    public string PlatformServer { get; set; }

    public int PlatformPort { get; set; }

    public bool DisableAuth { get; set; }

    public string Hostname { get; set; }

    public string Game { get; set; }

    public List<ImportConfiguration> Imports { get; set; }

    public Dictionary<string, DownloadConfiguration> Downloads { get; set; }

    public DownloadConfiguration GetDownloadConfiguration(string resourceName)
    {
      DownloadConfiguration downloadConfiguration;
      return this.Downloads.TryGetValue(resourceName, out downloadConfiguration) || this.Downloads.TryGetValue("all", out downloadConfiguration) ? downloadConfiguration : (DownloadConfiguration) null;
    }

    public bool DisableWindowedLogger { get; set; }

    public bool DebugLog { get; set; }

    public bool ScriptHookAllowed { get; set; }

    public string Password { get; set; }

    public bool CheckGameFiles { get; set; }

    public int MaxPlayers { get; set; }

    public string Language { get; set; }

    public string ForceVersion { get; set; }

    public bool Encryption { get; set; }
  }
}
