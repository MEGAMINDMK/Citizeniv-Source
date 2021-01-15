// Decompiled with JetBrains decompiler
// Type: CitizenMP.Server.Logging.WindowedLogger
// Assembly: CitizenMP.Server, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 05F7001E-4DA4-4F15-A443-96D9D1B18E6C
// Assembly location: C:\Users\MEGA\Downloads\Programs\CitizenMP.Server.exe

using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace CitizenMP.Server.Logging
{
  internal static class WindowedLogger
  {
    private static ManualResetEvent ms_waitEvent;
    private static NativeWindow ms_outputBox;
    private static int ms_messageCount;

    public static void Initialize(bool debugLog)
    {
      if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        throw new InvalidOperationException("Can not initialize windowed logger on non-Windows platforms.");
      WindowedLogger.FreeConsole();
      WindowedLogger.ms_waitEvent = new ManualResetEvent(false);
      new Thread(new ThreadStart(WindowedLogger.UIThread)).Start();
      WindowedLogger.ms_waitEvent.WaitOne();
      LoggingConfiguration loggingConfiguration = new LoggingConfiguration();
      MethodCallTarget methodCallTarget = new MethodCallTarget();
      methodCallTarget.set_ClassName(typeof (WindowedLogger).AssemblyQualifiedName);
      methodCallTarget.set_MethodName("LogOne");
      string str = "${message}";
      ((ICollection<MethodCallParameter>) ((MethodCallTargetBase) methodCallTarget).get_Parameters()).Add(new MethodCallParameter(Layout.op_Implicit(str)));
      loggingConfiguration.AddTarget("window", (Target) methodCallTarget);
      ((ICollection<LoggingRule>) loggingConfiguration.get_LoggingRules()).Add(new LoggingRule("*", (LogLevel) LogLevel.Info, (Target) methodCallTarget));
      if (debugLog)
      {
        FileTarget fileTarget = new FileTarget();
        loggingConfiguration.AddTarget("file", (Target) fileTarget);
        fileTarget.set_FileName(Layout.op_Implicit("${basedir}/CitizenMP.Server.log"));
        ((TargetWithLayout) fileTarget).set_Layout(Layout.op_Implicit("[${date:format=HH\\:mm\\:ss}] ${message}"));
        ((ICollection<LoggingRule>) loggingConfiguration.get_LoggingRules()).Add(new LoggingRule("*", (LogLevel) LogLevel.Info, (Target) fileTarget));
      }
      LogManager.set_Configuration(loggingConfiguration);
      LogManager.set_ThrowExceptions(true);
      WindowedLogger.ms_waitEvent.Dispose();
    }

    public static void Fatal(string error)
    {
      if (Environment.OSVersion.Platform != PlatformID.Win32NT)
        return;
      int num = (int) MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
    }

    public static void LogOne(string line)
    {
      WindowedLogger.SendMessage(WindowedLogger.ms_outputBox.Handle, 384, IntPtr.Zero, line);
      Interlocked.Increment(ref WindowedLogger.ms_messageCount);
      WindowedLogger.SendMessage(WindowedLogger.ms_outputBox.Handle, 407, WindowedLogger.ms_messageCount - 1, 0);
    }

    private static void UIThread()
    {
      Form window = new Form();
      window.Text = "CitizenFX Platform Server";
      window.Size = new Size(816, 639);
      window.FormClosed += new FormClosedEventHandler(WindowedLogger.window_FormClosed);
      NativeWindow listBox = new NativeWindow();
      listBox.AssignHandle(WindowedLogger.CreateWindowEx(WindowedLogger.WindowStylesEx.WS_EX_LEFT, "listbox", "", (WindowedLogger.WindowStyles) 1344278528, 0, 0, 800, 533, window.Handle, IntPtr.Zero, Process.GetCurrentProcess().Handle, IntPtr.Zero));
      IntPtr fontW = WindowedLogger.CreateFontW(8, 0, 0, 0, 0, 0U, 0U, 0U, 0U, 0U, 0U, 0U, 0U, "MS Sans Serif");
      WindowedLogger.SendMessage(listBox.Handle, 48, fontW, IntPtr.Zero);
      WindowedLogger.ms_outputBox = listBox;
      WindowedLogger.ms_waitEvent.Set();
      window.Resize += (EventHandler) ((sender, args) => WindowedLogger.MoveWindow(listBox.Handle, 0, 0, window.Width - 16, 533, true));
      Application.Run(window);
    }

    private static void window_FormClosed(object sender, FormClosedEventArgs e)
    {
      Environment.Exit(0);
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool FreeConsole();

    [DllImport("user32.dll", SetLastError = true)]
    internal static extern bool MoveWindow(
      IntPtr hWnd,
      int X,
      int Y,
      int nWidth,
      int nHeight,
      bool bRepaint);

    [DllImport("gdi32", EntryPoint = "CreateFont", CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFontW(
      [In] int nHeight,
      [In] int nWidth,
      [In] int nEscapement,
      [In] int nOrientation,
      [In] int fnWeight,
      [In] uint fdwItalic,
      [In] uint fdwUnderline,
      [In] uint fdwStrikeOut,
      [In] uint fdwCharSet,
      [In] uint fdwOutputPrecision,
      [In] uint fdwClipPrecision,
      [In] uint fdwQuality,
      [In] uint fdwPitchAndFamily,
      [In] string lpszFace);

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowEx(
      WindowedLogger.WindowStylesEx dwExStyle,
      string lpClassName,
      string lpWindowName,
      WindowedLogger.WindowStyles dwStyle,
      int x,
      int y,
      int nWidth,
      int nHeight,
      IntPtr hWndParent,
      IntPtr hMenu,
      IntPtr hInstance,
      IntPtr lpParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(IntPtr hwnd, int msg, int wParam, int lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      IntPtr lParam);

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    internal static extern IntPtr SendMessage(
      IntPtr hwnd,
      int msg,
      IntPtr wParam,
      string lParam);

    [System.Flags]
    private enum WindowStyles : uint
    {
      WS_BORDER = 8388608, // 0x00800000
      WS_CAPTION = 12582912, // 0x00C00000
      WS_CHILD = 1073741824, // 0x40000000
      WS_CLIPCHILDREN = 33554432, // 0x02000000
      WS_CLIPSIBLINGS = 67108864, // 0x04000000
      WS_DISABLED = 134217728, // 0x08000000
      WS_DLGFRAME = 4194304, // 0x00400000
      WS_GROUP = 131072, // 0x00020000
      WS_HSCROLL = 1048576, // 0x00100000
      WS_MAXIMIZE = 16777216, // 0x01000000
      WS_MAXIMIZEBOX = 65536, // 0x00010000
      WS_MINIMIZE = 536870912, // 0x20000000
      WS_MINIMIZEBOX = WS_GROUP, // 0x00020000
      WS_OVERLAPPED = 0,
      WS_OVERLAPPEDWINDOW = 13565952, // 0x00CF0000
      WS_POPUP = 2147483648, // 0x80000000
      WS_POPUPWINDOW = 2156396544, // 0x80880000
      WS_SIZEFRAME = 262144, // 0x00040000
      WS_SYSMENU = 524288, // 0x00080000
      WS_TABSTOP = WS_MAXIMIZEBOX, // 0x00010000
      WS_VISIBLE = 268435456, // 0x10000000
      WS_VSCROLL = 2097152, // 0x00200000
    }

    [System.Flags]
    public enum WindowStylesEx : uint
    {
      WS_EX_ACCEPTFILES = 16, // 0x00000010
      WS_EX_APPWINDOW = 262144, // 0x00040000
      WS_EX_CLIENTEDGE = 512, // 0x00000200
      WS_EX_COMPOSITED = 33554432, // 0x02000000
      WS_EX_CONTEXTHELP = 1024, // 0x00000400
      WS_EX_CONTROLPARENT = 65536, // 0x00010000
      WS_EX_DLGMODALFRAME = 1,
      WS_EX_LAYERED = 524288, // 0x00080000
      WS_EX_LAYOUTRTL = 4194304, // 0x00400000
      WS_EX_LEFT = 0,
      WS_EX_LEFTSCROLLBAR = 16384, // 0x00004000
      WS_EX_LTRREADING = 0,
      WS_EX_MDICHILD = 64, // 0x00000040
      WS_EX_NOACTIVATE = 134217728, // 0x08000000
      WS_EX_NOINHERITLAYOUT = 1048576, // 0x00100000
      WS_EX_NOPARENTNOTIFY = 4,
      WS_EX_OVERLAPPEDWINDOW = 768, // 0x00000300
      WS_EX_PALETTEWINDOW = 392, // 0x00000188
      WS_EX_RIGHT = 4096, // 0x00001000
      WS_EX_RIGHTSCROLLBAR = 0,
      WS_EX_RTLREADING = 8192, // 0x00002000
      WS_EX_STATICEDGE = 131072, // 0x00020000
      WS_EX_TOOLWINDOW = 128, // 0x00000080
      WS_EX_TOPMOST = 8,
      WS_EX_TRANSPARENT = 32, // 0x00000020
      WS_EX_WINDOWEDGE = 256, // 0x00000100
    }
  }
}
