using System.ServiceProcess;

namespace KissAnime_Discord_Rich_Presence
{
    static class Program
    {
        public const string version = "0.3.1";

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
