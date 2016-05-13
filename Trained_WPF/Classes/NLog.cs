using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using NLog;


namespace Trained_WPF.Classes
{
    class NLog
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        //лог авторизации
        public static void AuthToLog(string login)
        {
            try
            {
                logger.Info(login);
            }
            catch (Exception)
            {

            }
        }

        //exception
        public static void ExceptionToLog(string exception)
        {
            try
            {
                logger.Error(exception);
            }
            catch 
            {

            }
        }

        //лог операций
        public static void OperationToLog(string operationType, String userId, string author)
        {
            try
            {
                logger.Info(operationType + "'" + userId + "';" + " Processed by: " + author);
            }
            catch
            {

            }
        }

    }
}
