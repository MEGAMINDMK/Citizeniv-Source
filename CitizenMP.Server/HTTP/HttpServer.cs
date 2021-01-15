// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.HTTP.HttpServer
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using CitizenMP.Server.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Handlers;
using uhttpsharp.Headers;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;

namespace CitizenMP.Server.HTTP
{
  internal class HttpServer
  {
    private Dictionary<string, Func<IHttpHeaders, IHttpContext, Task<JObject>>> m_handlers;
    private ResourceManager m_resourceManager;
    private Configuration m_configuration;

    public HttpServer(Configuration config, ResourceManager resManager)
    {
      this.m_configuration = config;
      this.m_resourceManager = resManager;
      this.m_handlers = new Dictionary<string, Func<IHttpHeaders, IHttpContext, Task<JObject>>>();
      this.m_handlers["initconnect"] = InitConnectMethod.Get(config, resManager.GameServer);
      this.m_handlers["getconfiguration"] = GetConfigurationMethod.Get(config, resManager);
    }

    public void Start()
    {
      this.Log<HttpServer>(nameof (Start), "C:\\Users\\Tiger\\Desktop\\CitizenMP-IV Reloaded\\cfx-server\\CitizenMP.Server\\HTTP\\HttpServer.cs", 42).Info("[Server] Starting HTTP server on port {0}.", (object) this.m_configuration.ListenPort);
      HttpServer httpServer = new HttpServer((IHttpRequestProvider) new HttpRequestProvider());
      httpServer.Use((IHttpListener) new TcpListenerAdapter(new TcpListener(IPAddress.Any, this.m_configuration.ListenPort)));
      if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        httpServer.Use((IHttpListener) new TcpListenerAdapter(new TcpListener(IPAddress.IPv6Any, this.m_configuration.ListenPort)));
      httpServer.Use((IHttpRequestHandler) new HttpRouter().With("client", (IHttpRequestHandler) new AnonymousHttpRequestHandler((Func<IHttpContext, Func<Task>, Task>) (async (context, next) =>
      {
        HttpResponseCode httpResponseCode;
        JObject jobject;
        if (context.get_Request().get_Method() != 2)
        {
          httpResponseCode = (HttpResponseCode) 400;
          jobject = new JObject();
          jobject.set_Item("err", JToken.op_Implicit("wasn't a POST"));
        }
        else
        {
          IHttpHeaders parsed = context.get_Request().get_Post().get_Parsed();
          string lowerInvariant = parsed.GetByName("method").ToLowerInvariant();
          if (this.m_handlers.ContainsKey(lowerInvariant))
          {
            jobject = await this.m_handlers[lowerInvariant](parsed, context);
          }
          else
          {
            jobject = new JObject();
            jobject.set_Item("err", JToken.op_Implicit("invalid method"));
          }
          httpResponseCode = (HttpResponseCode) 200;
        }
        context.set_Response((IHttpResponse) new HttpResponse(httpResponseCode, "application/json", ((JToken) jobject).ToString((Formatting) 0, new JsonConverter[0]), true));
      }))).With("files", (IHttpRequestHandler) new AnonymousHttpRequestHandler((Func<IHttpContext, Func<Task>, Task>) ((context, next) =>
      {
        string[] strArray = context.get_Request().get_Uri().OriginalString.Split(new char[1]
        {
          '/'
        }, StringSplitOptions.RemoveEmptyEntries);
        if (strArray.Length >= 3)
        {
          Resource resource = this.m_resourceManager.GetResource(strArray[1]);
          if (resource != null)
          {
            if (strArray[2] == "resource.rpf")
            {
              context.set_Response((IHttpResponse) new HttpResponse((HttpResponseCode) 200, "application/x-rockstar-rpf", resource.OpenClientPackage(), true));
            }
            else
            {
              Stream streamFile = resource.GetStreamFile(strArray[2]);
              if (streamFile != null)
                context.set_Response((IHttpResponse) new HttpResponse((HttpResponseCode) 200, "application/octet-stream", streamFile, true));
            }
          }
        }
        if (context.get_Response() == null)
          context.set_Response((IHttpResponse) new HttpResponse((HttpResponseCode) 404, "Not found.", true));
        return TaskFactoryExtensions.GetCompleted(Task.Factory);
      }))).With("log", (IHttpRequestHandler) new AnonymousHttpRequestHandler((Func<IHttpContext, Func<Task>, Task>) ((context, next) =>
      {
        if (this.m_resourceManager.RconLog != null)
          this.m_resourceManager.RconLog.RunHttp(context);
        return TaskFactoryExtensions.GetCompleted(Task.Factory);
      }))));
      HttpServerExtensions.Use(httpServer, (Func<IHttpContext, Func<Task>, Task>) ((context, next) =>
      {
        context.set_Response((IHttpResponse) HttpResponse.CreateWithMessage((HttpResponseCode) 404, "not found", false, ""));
        return TaskFactoryExtensions.GetCompleted(Task.Factory);
      }));
      httpServer.Start();
    }
  }
}
