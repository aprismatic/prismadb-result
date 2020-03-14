using Npgsql;
using System;
using System.Diagnostics;
using System.Threading;
using Xunit;

namespace ResultTests
{
    public class PostgresDatabaseFixture : IDisposable
    {
        public static string ContainerName = "UnitTestPostgresServer";
        public NpgsqlConnection DbConn { get; private set; }
        private static int port = 5432;

        public PostgresDatabaseFixture()
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
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./prepare-Postgres.ps1\" -ContainerName \"{ContainerName}\" -Port {port}",
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

            var csb = new NpgsqlConnectionStringBuilder
            {
                Username = "postgres",
                Password = "Password12!",
                Host = ip,
                Port = port,
                Database = "postgres"
            };

            using (var conn = new NpgsqlConnection(csb.ToString()))
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
                    catch (NpgsqlException)
                    {
                        Thread.Sleep(250);
                    }
                using (var cmd = new NpgsqlCommand("CREATE DATABASE \"Test-DB-1\"", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            csb.Database = "Test-DB-1";

            DbConn = new NpgsqlConnection(csb.ToString());
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
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -File \"./kill-Postgres.ps1\" \"{ContainerName}\"",
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

    [CollectionDefinition("Postgres Database Collection")]
    public class PostgresDatabaseCollection : ICollectionFixture<PostgresDatabaseFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.
    }
}
