using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace ResultTests
{
    public class MsSqlDatabaseFixture : IDisposable
    {
        public static string ContainerName = "UnitTestMSSQLServer";
        public SqlConnection DbConn { get; private set; }
        private static uint port = 1433;

        public MsSqlDatabaseFixture()
        {
            {
                var shell = "powershell";
                if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTION")))
                    shell = "pwsh";

                var proc = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = shell,
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./prepare-MSSQL.ps1\" -ContainerName \"{ContainerName}\" -Port {port}",
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

            var csb = new SqlConnectionStringBuilder
            {
                UserID = "sa",
                Password = "Password12!",
                DataSource = $"{ip},{port}",
                InitialCatalog = "master"
            };

            using (var conn = new SqlConnection(csb.ToString()))
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
                    catch (SqlException)
                    {
                        Thread.Sleep(250);
                    }
                using (var cmd = new SqlCommand("CREATE DATABASE [Test-DB-1]", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            csb.InitialCatalog = "Test-DB-1";

            DbConn = new SqlConnection(csb.ToString());
            DbConn.Open();
        }

        public void Dispose()
        {
            DbConn.Dispose();

            var shell = "powershell";
            if (!String.IsNullOrEmpty(Environment.GetEnvironmentVariable("GITHUB_ACTION")))
                shell = "pwsh";

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = shell,
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./kill-MSSQL.ps1\" \"{ContainerName}\"",
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

    [CollectionDefinition("MSSQL Database Collection")]
    public class MsSqlDatabaseCollection : ICollectionFixture<MsSqlDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
