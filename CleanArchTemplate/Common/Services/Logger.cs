using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;

namespace CleanArchTemplate.Common.Services
{
    public class Logger : IDisposable, ILogger
    {
        //if stored in the web.config in web application also can be used in the 
        // class library project but relevent reference should be added then System.web.
        //private static string connectionString = WebConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;


        private static string logDirectoryPath = WebConfigurationManager.AppSettings["LogDirectoryPath"].ToString();

        // if stored in the connection string tag in the app.config in class library project .. 
        // add referece System.Configuration
        //private static string connectionString1 = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;

        //if stored in the appsettings tag in the app.config in the class library project ...
        // add referece System.Configuration
        //private static string connectionString2 = ConfigurationManager.AppSettings["ConnectionString"].ToString(); 


        public static Logger Create()
        {
            return new Logger();
        }

        public static void Log(Exception exception)
        {
            StringBuilder sbExceptionMessage = new StringBuilder();

            do
            {
                sbExceptionMessage.Append("Exception Type" + Environment.NewLine);
                sbExceptionMessage.Append(exception.GetType().Name);
                sbExceptionMessage.Append(Environment.NewLine + Environment.NewLine);
                sbExceptionMessage.Append("Message" + Environment.NewLine);
                sbExceptionMessage.Append(exception.Message + Environment.NewLine + Environment.NewLine);
                sbExceptionMessage.Append("Stack Trace" + Environment.NewLine);
                sbExceptionMessage.Append(exception.StackTrace + Environment.NewLine + Environment.NewLine);

                exception = exception.InnerException;
            }
            while (exception != null);

            string logProvider = ConfigurationManager.AppSettings["LogProvider"];
            if (logProvider.ToLower() == "both")
            {
                LogToDB(sbExceptionMessage.ToString());
                LogToEventViewer(sbExceptionMessage.ToString());
            }
            else if (logProvider.ToLower() == "database")
            {
                LogToDB(sbExceptionMessage.ToString());
            }
            else if (logProvider.ToLower() == "eventviewer")
            {
                LogToEventViewer(sbExceptionMessage.ToString());
            }
        }

        private static void LogToDB(string log)
        {
            // ConfigurationManager class is in System.Configuration namespace
            string connectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;
            // SqlConnection is in System.Data.SqlClient namespace
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                SqlCommand cmd = new SqlCommand("spInsertLog", con);
                // CommandType is in System.Data namespace
                cmd.CommandType = CommandType.StoredProcedure;

                SqlParameter parameter = new SqlParameter("@ExceptionMessage", log);
                cmd.Parameters.Add(parameter);

                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        private static void LogToEventViewer(string log)
        {
            if (EventLog.SourceExists("PragimTech.com"))
            {
                // Create an instance of the eventlog
                EventLog eventLog = new EventLog("PragimTech");
                // set the source for the eventlog
                eventLog.Source = "PragimTech.com";
                // Write the exception details to the event log as an error
                eventLog.WriteEntry(log, EventLogEntryType.Error);
            }
        }

        private static void LogToFile(string log)
        {
            if (EventLog.SourceExists("PragimTech.com"))
            {
                // Create an instance of the eventlog
                EventLog eventLog = new EventLog("PragimTech");
                // set the source for the eventlog
                eventLog.Source = "PragimTech.com";
                // Write the exception details to the event log as an error
                eventLog.WriteEntry(log, EventLogEntryType.Error);
            }
        }


        private static String ErrorlineNo, Errormsg, extype, exurl, hostIp, ErrorLocation, HostAdd;

        public static void SendErrorToText(Exception ex)
        {
            var line = Environment.NewLine + Environment.NewLine;

            ErrorlineNo = ex.StackTrace.Substring(ex.StackTrace.Length - 7, 7);
            Errormsg = ex.GetType().Name.ToString();
            extype = ex.GetType().ToString();
            //exurl = context.Current.Request.Url.ToString();
            ErrorLocation = ex.Message.ToString();

            try
            {
                string filepath = null; //= context.Current.Server.MapPath("~/ExceptionDetailsFile/");  //Text File Path

                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);

                }
                filepath = filepath + DateTime.Today.ToString("dd-MM-yy") + ".txt";   //Text File Name
                if (!File.Exists(filepath))
                {


                    File.Create(filepath).Dispose();

                }
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    string error = "Log Written Date:" + " " + DateTime.Now.ToString() + line + "Error Line No :" + " " + ErrorlineNo + line + "Error Message:" + " " + Errormsg + line + "Exception Type:" + " " + extype + line + "Error Location :" + " " + ErrorLocation + line + " Error Page Url:" + " " + exurl + line + "User Host IP:" + " " + hostIp + line;
                    sw.WriteLine("-----------Exception Details on " + " " + DateTime.Now.ToString() + "-----------------");
                    sw.WriteLine("-------------------------------------------------------------------------------------");
                    sw.WriteLine(line);
                    sw.WriteLine(error);
                    sw.WriteLine("--------------------------------*End*------------------------------------------");
                    sw.WriteLine(line);
                    sw.Flush();
                    sw.Close();

                }

            }
            catch (Exception e)
            {
                e.ToString();

            }
        }


        public static void SendErrorToFile(Exception ex)
        {

            string dir = @"C:\Error.txt";  // folder location
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                File.AppendAllText("~/Error.txt", "Message :" + ex.Message + "<br/>" + Environment.NewLine + "StackTrace :" + ex.StackTrace +
           "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                string New = Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine;
                File.AppendAllText("~/Error.txt", New);
            }

        }


        public static void ErrorLogging(Exception ex)
        {
            string strPath = @"D:\Rekha\Log.txt";



            if (!File.Exists(strPath))
            {
                File.Create(strPath).Dispose();
            }
            using (StreamWriter sw = File.AppendText(strPath))
            {
                sw.WriteLine("=============Error Logging ===========");
                sw.WriteLine("===========Start============= " + DateTime.Now);
                sw.WriteLine("Error Message: " + ex.Message);
                sw.WriteLine("Stack Trace: " + ex.StackTrace);
                sw.WriteLine("===========End============= " + DateTime.Now);

            }

        }


        public static void ReadError()
        {
            string strPath = @"D:\Rekha\Log.txt";
            using (StreamReader sr = new StreamReader(strPath))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }

        public void LogInfo(string info)
        {
            string directoryPath = @logDirectoryPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = @logDirectoryPath + DateTime.Now.ToString("dd_MM_yyyy_hh") + "Info.txt"; ;

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(info);
                writer.Close();
            }

        }


        public void LogError(Exception ex)
        {
            string message = string.Format("Time: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt"));
            do
            {

                message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                message += Environment.NewLine; message += Environment.NewLine;
                message += string.Format("Exception Type: {0}", ex.GetType().Name);
                message += Environment.NewLine; message += Environment.NewLine;
                message += string.Format("Exception Message: {0}", ex.Message);
                message += Environment.NewLine; message += Environment.NewLine;
                message += string.Format("Exception Source: {0}", ex.Source);
                message += Environment.NewLine; message += Environment.NewLine;
                message += string.Format("TargetSite: {0}", ex.TargetSite.ToString());
                message += Environment.NewLine; message += Environment.NewLine;
                message += string.Format("StackTrace: {0}", ex.StackTrace);
                message += Environment.NewLine; message += Environment.NewLine;
                message += "-----------------------------------------------------------";
                //message += Environment.NewLine;

                ex = ex.InnerException;
            }
            while (ex != null);

            string directoryPath = @logDirectoryPath;

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string filePath = @logDirectoryPath + DateTime.Now.ToString("dd_MM_yyyy_hh_mm_ss_tt") + "Log.txt"; ;

            if (!File.Exists(filePath))
            {
                File.Create(filePath).Dispose();
            }

            //using (StreamWriter sw = File.AppendText(strPath))
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //
            }

            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        ~Logger()
        {
            Dispose(false);
        }


    }
}

