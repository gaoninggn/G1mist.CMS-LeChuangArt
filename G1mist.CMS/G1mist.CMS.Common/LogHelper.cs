using NLog;

namespace G1mist.CMS.Common
{
    public static class LogHelper
    {
        public static Logger Log { get; set; }

        static LogHelper()
        {
            Log = LogManager.GetCurrentClassLogger();
        }

        public static void Info(string msg)
        {
            Log.Info(msg);
        }
        public static void Debug(string msg)
        {
            Log.Debug(msg);
        }
        public static void Trace(string msg)
        {
            Log.Trace(msg);
        }
        public static void Warn(string msg)
        {
            Log.Warn(msg);
        }
        public static void Error(string msg)
        {
            Log.Error(msg);
        }
        public static void Fatal(string msg)
        {
            Log.Fatal(msg);
        }
    }
}
