using DiscordRPC;
using KissAnime_Discord_Rich_Presence.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace KissAnime_Discord_Rich_Presence
{
    public partial class Service1 : ServiceBase
    {
        public string dataDirectory = Environment.GetEnvironmentVariable("LocalAppData") + "\\KADR";
        public static DiscordRpcClient discordRpcClient;
        public WebSocketServer webSocketServer;
        public static EventLog eventLog;

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        public Service1()
        {
            InitializeComponent();

            eventLog = new System.Diagnostics.EventLog();

            if (!System.Diagnostics.EventLog.SourceExists("KADRP"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "KADRP", "KissAnime Discord Rich Presence");
            }

            eventLog.Source = "KADRP";
            eventLog.Log = "KissAnime Discord Rich Presence";
        }

        protected override void OnStart(string[] args)
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 30000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            discordRpcClient = new DiscordRPC.DiscordRpcClient("476645625199460354", false, -1);
            discordRpcClient.Initialize();

            webSocketServer = new WebSocketServer(IPAddress.Parse("127.0.0.1"), 8080);
            webSocketServer.AddWebSocketService<KADRP>("/KADRP");
            webSocketServer.Start();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 10000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            discordRpcClient.Dispose();
            webSocketServer.Stop();
            eventLog.Close();

            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public static void UpdatePresence(string details, string state)
        {
            discordRpcClient.SetPresence(new RichPresence()
            {
                Details = details,
                State = state,
                Assets = new Assets()
                {
                    LargeImageKey = "image_large",
                    LargeImageText = "KissAnime - Watch anime free online",
                    SmallImageKey = "image_small"
                }
            });
        }

        public enum ServiceState
        {
            SERVICE_STOPPED = 0x00000001,
            SERVICE_START_PENDING = 0x00000002,
            SERVICE_STOP_PENDING = 0x00000003,
            SERVICE_RUNNING = 0x00000004,
            SERVICE_CONTINUE_PENDING = 0x00000005,
            SERVICE_PAUSE_PENDING = 0x00000006,
            SERVICE_PAUSED = 0x00000007,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ServiceStatus
        {
            public int dwServiceType;
            public ServiceState dwCurrentState;
            public int dwControlsAccepted;
            public int dwWin32ExitCode;
            public int dwServiceSpecificExitCode;
            public int dwCheckPoint;
            public int dwWaitHint;
        };

        public class KADRP : WebSocketBehavior
        {
            protected override void OnMessage(MessageEventArgs e)
            {
                String json = e.Data;

                try
                {
                    DataString dataString = JsonConvert.DeserializeObject<DataString>(json);

                    switch (dataString.action)
                    {
                        case 0:
                            UpdatePresence("Watching " + dataString.anime, "Episode " + dataString.current_episode);
                            break;
                        case 1:
                            discordRpcClient.ClearPresence();
                            break;
                    }
                }
                catch (Exception exception)
                {
                    eventLog.WriteEntry(exception.Message + "\n\n" + exception.StackTrace);
                }
            }
        }
    }
}
