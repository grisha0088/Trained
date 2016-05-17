using System;
using NLog;

namespace Trained_WPF.Classes
{
    class NLog
    {
        readonly static Logger Logger = LogManager.GetCurrentClassLogger();

        //лог авторизации
        public static void AuthToLog(string login)
        {
            try
            {
                Logger.Info(login);
            }
            catch
            {
                // ignored
            }
        }

        //exception
        public static void ExceptionToLog(string exception)
        {
            try
            {
                Logger.Error(exception);
            }
            catch 
            {
                // ignored
            }
        }

        //лог операций
        public static void OperationToLog(string operationType, String userId, string author)
        {
            try
            {
                Logger.Info(operationType + "'" + userId + "';" + " Processed by: " + author);
            }
            catch
            {
                // ignored
            }
        }

    }
}
