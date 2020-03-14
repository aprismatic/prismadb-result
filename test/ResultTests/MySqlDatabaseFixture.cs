using MySql.Data.MySqlClient;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace ResultTests
{
    public class MySqlDatabaseFixture : IDisposable
    {
        public static string ContainerName = "UnitTestMySQLServer";
        public MySqlConnection DbConn { get; private set; }
        private static uint port = 3306;

        public MySqlDatabaseFixture()
        {
            {
                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./prepare-MySQL.ps1\" -ContainerName \"{ContainerName}\" -Port {port}",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                proc.Start();
                while (!proc.StandardOutput.EndOfStream)
                {
                    var l = proc.StandardOutput.ReadLine();
                    Console.WriteLine(l);
                }
            }

            var ip = "127.0.0.1";

            var csb = new MySqlConnectionStringBuilder
            {
                UserID = "root",
                Password = "Password12!",
                Server = ip,
                Port = port,
                Database = "mysql"
            };

            using (var conn = new MySqlConnection(csb.ToString()))
            {
                var t = new Stopwatch();
                t.Start();
                var success = false;

                // currently takes up to 2 min on Win10 to launch SQL Server container 
                // we make it 10 min in case docker needs to pull the image 
                while (!success && t.Elapsed.TotalSeconds < 600)
                    try
                    {
                        conn.Open();
                        success = true;
                    }
                    catch (MySqlException)
                    {
                        Thread.Sleep(250);
                    }
                using (var cmd = new MySqlCommand("CREATE DATABASE `Test-DB-1`", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            csb.Database = "Test-DB-1";

            DbConn = new MySqlConnection(csb.ToString());
            DbConn.Open();
        }

        public void Dispose()
        {
            DbConn.Dispose();

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "powershell",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./kill-MySQL.ps1\" \"{ContainerName}\"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
            while (!proc.StandardOutput.EndOfStream)
            {
                var l = proc.StandardOutput.ReadLine();
                Console.WriteLine(l);
            }
        }
    }

    [CollectionDefinition("MySQL Database Collection")]
    public class MySqlDatabaseCollection : ICollectionFixture<MySqlDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
