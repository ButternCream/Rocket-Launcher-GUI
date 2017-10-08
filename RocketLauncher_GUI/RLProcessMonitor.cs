using System.Diagnostics;

namespace RocketLauncher_GUI
{
    class RLProcessMonitor
    {
        //global process
        public static Process RocketLeagueProcess = null;

        public static void LookForProcess()
        {
            var procs = Process.GetProcessesByName("RocketLeague");
            RocketLeagueProcess = (procs.Length > 0) ? procs[0] : null;
        }
    }
}
