// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Program
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Commands;
using CitizenMP.Server.Game;
using CitizenMP.Server.HTTP;
using CitizenMP.Server.Logging;
using CitizenMP.Server.Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using YamlDotNet.Core;

namespace CitizenMP.Server
{
  internal class Program
  {
    public static string RootDirectory { get; private set; }

    private async Task Start(string configFileName)
    {
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      int num1 = (^this).\u003C\u003E1__state;
      Program type = this;
      Configuration config;
      ResourceManager resManager;
      GameServer gameServer;
      try
      {
        try
        {
          config = Configuration.Load(configFileName ?? "citmp-server.yml");
          if (Environment.OSVersion.Platform == PlatformID.Win32NT && !config.DisableWindowedLogger)
            WindowedLogger.Initialize(config.DebugLog);
          if (config.AutoStartResources == null)
          {
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 30).Fatal("No auto-started resources were configured.");
            goto label_47;
          }
          else if (config.ListenPort == 0)
          {
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 36).Fatal("No port was configured.");
            goto label_47;
          }
          else if (config.Downloads == null)
            config.Downloads = new Dictionary<string, DownloadConfiguration>();
        }
        catch (IOException ex)
        {
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 47).Fatal("Could not open the configuration file {0}.", (object) (configFileName ?? "citmp-server.yml"));
          goto label_47;
        }
        catch (YamlException ex)
        {
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 52).Fatal("Could not parse the configuration file {0} because of the error: {1}", (object) (configFileName ?? "citmp-server.yml"), (object) ((Exception) ex).Message);
          goto label_47;
        }
        if (new WebClient().DownloadString("http://citizeniv.net/sversion.txt") != config.cleanServerVersion)
        {
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 61).Fatal("Please update your server on http://citizeniv.net/.");
        }
        else
        {
          if (config.MaxPlayers < 1 || config.MaxPlayers > 32)
          {
            config.MaxPlayers = 32;
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 68).Info("[Debug] Max Players have been set to 32 because of invalid number.");
          }
          if (!string.IsNullOrEmpty(config.ForceVersion) && config.ForceVersion != "stable" && config.ForceVersion != "beta")
          {
            config.ForceVersion = "none";
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 76).Info("[Debug] Please use only stable or beta in ForceVersion.");
          }
          if (config.Encryption && config.ForceVersion != "beta")
          {
            config.Encryption = false;
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 85).Info("[Debug] Encryption can be only used if you enable ForceVersion: beta.");
          }
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 89).Info("");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 90).Info("====================================================================");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 91).Info(" CitizenMP:IV Reloaded Server {0}", (object) config.serverVersion);
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 92).Info(" Copyright (C) 2017-2020 CitizenFX Reloaded Team");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 93).Info(" Port: {0}", (object) config.ListenPort);
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 94).Info(" Max Players: {0}", (object) config.MaxPlayers);
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 95).Info("====================================================================");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 96).Info("");
          CommandManager commandManager = new CommandManager();
          resManager = new ResourceManager(config);
          gameServer = new GameServer(config, resManager, commandManager);
          List<string>.Enumerator enumerator;
          TaskAwaiter<bool> taskAwaiter;
          if (config.PreParseResources != null)
          {
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 107).Info("=========== Pre-parsing Resources ==========");
            enumerator = config.PreParseResources.GetEnumerator();
            try
            {
              while (enumerator.MoveNext())
              {
                string current = enumerator.Current;
                resManager.ScanResources("resources/", current);
                Resource resource = resManager.GetResource(current);
                if (resource != null)
                {
                  TaskAwaiter<bool> awaiter = resource.Start(config).GetAwaiter();
                  if (!awaiter.IsCompleted)
                  {
                    // ISSUE: explicit reference operation
                    // ISSUE: reference to a compiler-generated field
                    (^this).\u003C\u003E1__state = num1 = 0;
                    taskAwaiter = awaiter;
                    // ISSUE: explicit reference operation
                    // ISSUE: reference to a compiler-generated field
                    (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, Program.\u003CStart\u003Ed__4>(ref awaiter, this);
                    return;
                  }
                  awaiter.GetResult();
                }
              }
            }
            finally
            {
              if (num1 < 0)
                enumerator.Dispose();
            }
            enumerator = new List<string>.Enumerator();
          }
          else
            type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 124).Warn("No PreParseResources defined. This usually means you're using an outdated configuration file. Please consider this.");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", (int) sbyte.MaxValue).Info("========================================");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 128).Info("");
          resManager.ScanResources("resources/", (string) null);
          gameServer.Start();
          new HttpServer(config, resManager).Start();
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 141).Info("");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 142).Info("============ Loading Resources ============");
          enumerator = config.AutoStartResources.GetEnumerator();
          try
          {
            while (enumerator.MoveNext())
            {
              string current = enumerator.Current;
              Resource resource = resManager.GetResource(current);
              if (resource == null)
              {
                type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 149).Error("Could not find auto-started resource {0}.", (object) current);
              }
              else
              {
                TaskAwaiter<bool> awaiter = resource.Start(config).GetAwaiter();
                if (!awaiter.IsCompleted)
                {
                  // ISSUE: explicit reference operation
                  // ISSUE: reference to a compiler-generated field
                  (^this).\u003C\u003E1__state = num1 = 1;
                  taskAwaiter = awaiter;
                  // ISSUE: explicit reference operation
                  // ISSUE: reference to a compiler-generated field
                  (^this).\u003C\u003Et__builder.AwaitUnsafeOnCompleted<TaskAwaiter<bool>, Program.\u003CStart\u003Ed__4>(ref awaiter, this);
                  return;
                }
                awaiter.GetResult();
              }
            }
          }
          finally
          {
            if (num1 < 0)
              enumerator.Dispose();
          }
          enumerator = new List<string>.Enumerator();
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 157).Info("========================================");
          type.Log<Program>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs", 158).Info("");
          resManager.StartSynchronization();
          int num2 = Environment.TickCount;
          while (true)
          {
            Thread.Sleep(5);
            int tickCount = Environment.TickCount;
            gameServer.Tick(tickCount - num2);
            num2 = tickCount;
          }
        }
      }
      catch (Exception ex)
      {
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003E1__state = -2;
        config = (Configuration) null;
        resManager = (ResourceManager) null;
        gameServer = (GameServer) null;
        // ISSUE: explicit reference operation
        // ISSUE: reference to a compiler-generated field
        (^this).\u003C\u003Et__builder.SetException(ex);
        return;
      }
label_47:
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003E1__state = -2;
      config = (Configuration) null;
      resManager = (ResourceManager) null;
      gameServer = (GameServer) null;
      // ISSUE: explicit reference operation
      // ISSUE: reference to a compiler-generated field
      (^this).\u003C\u003Et__builder.SetResult();
    }

    private static void Main(string[] args)
    {
      BaseLog.SetStripSourceFilePath("C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\Program.cs");
      Time.Initialize();
      try
      {
        Program.RootDirectory = Environment.CurrentDirectory;
        new Program().Start(args.Length != 0 ? args[0] : (string) null).Wait();
        Environment.Exit(0);
      }
      catch (AggregateException ex)
      {
        Console.WriteLine(ex.InnerException.ToString());
        Environment.Exit(1);
      }
    }
  }
}
