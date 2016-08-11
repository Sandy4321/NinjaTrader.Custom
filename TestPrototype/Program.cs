using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;

namespace TestPrototype
{
    class Program
    {

        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {

            log.Info("Run application...");

            DBConnect dbConn = new DBConnect();

            log.Info(string.Format("Database connection: {0} ", dbConn.OpenConnection()));
            log.Info(string.Format("Database disconnected: {0} ", dbConn.CloseConnection()));

            log.Info("Shutdown application...");
        }
    }
}
