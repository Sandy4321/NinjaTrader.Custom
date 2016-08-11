using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using MySql.Data.MySqlClient;


namespace TestPrototype
{
    class DBConnect
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(DBConnect));

        private MySqlConnection connection;
        private string _server;
        private string _database;
        private string _uid;
        private string _password;
        private string _jdbcURL;

        //Constructor
        public DBConnect()
        {
            Initialize();

        }

        //Initialize values
        public void Initialize()
        {
            _server = "localhost";
            _database = "dbTestPrototype";
            _uid = "root";
            _password = "Password1";
            _jdbcURL = string.Format("server={0};database={1};uid={2};pwd={3}", _server, _database, _uid, _password);

            connection = new MySqlConnection(_jdbcURL);
        }

        //Open connection to database
        public bool OpenConnection()        // MARK THIS AS PRIVATE LATER
        {
            log.Debug(string.Format("Connection to {0} ",_jdbcURL));
            try
            {
                log.Debug("Initiate database connection");
                connection.Open();
                log.Debug("Connection sucessful");
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number)
                {
                    case 0:
                        log.Fatal("Cannot connect to database server");
                        break;
                    case 1045:
                        log.Fatal("Invalid username/password");
                        break;
                }

            }
            return false;


        }

        //Close connection to database
        public bool CloseConnection()       // MARK THIS AS PRIVATE LATER
        {
            try
            {
                connection.Close();
                log.Info("Database connection closed successfully");
                return true;

            }
            catch (MySqlException ex)
            {
                log.Info(ex.Message);
                return false;
            }
        }

        //Insert statement
        public void Insert()
        { }
        //Update statement
        public void Update()
        { }
        //Delete statement
        public void Delete()
        { }

        /*
            //Selete statment
            public List<string> [] Select()
            { }
            //Count statement
            public int Count()
            { }
         */

        //Backup
        public void Backup()
        { }
        //Restore
        public void Restore()
        { }

    }
}
