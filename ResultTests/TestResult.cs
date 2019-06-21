using Newtonsoft.Json;
using PrismaDB.Result;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace ResultTests
{
    public class TestResult
    {
        private readonly ITestOutputHelper output;

        public TestResult(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(DisplayName = "ResultTable Serialization")]
        public void TestResultTableSerialization()
        {
            var table = new ResultTable();
            table.Columns.Add(new ResultColumnHeader("a", typeof(int)));
            table.Columns.Add(new ResultColumnHeader("b", typeof(string)));
            table.Columns.Add(new ResultColumnHeader("c", typeof(string)));
            var row1 = table.NewRow();
            row1.Add(new object[] { 1, "abc", "DEF" });
            table.Rows.Add(row1);
            var row2 = table.NewRow();
            row2.Add(new object[] { 2, DBNull.Value, "xyz" });
            table.Rows.Add(row2);

            string xmlRes;
            using (var stream = new StringWriter())
            {
                var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
                var serializer = new XmlSerializer(table.GetType());
                var settings = new XmlWriterSettings
                {
                    Indent = true,
                    OmitXmlDeclaration = true
                };
                using (var writer = XmlWriter.Create(stream, settings))
                {
                    serializer.Serialize(writer, table, namespaces);
                    xmlRes = stream.ToString();
                }
            }

            var jsonRes = JsonConvert.SerializeObject(table);

            Assert.NotNull(xmlRes);
            Assert.NotNull(jsonRes);

            output.WriteLine(xmlRes);
            output.WriteLine(jsonRes);
        }

        [Fact(DisplayName = "ResultReader Async")]
        public void TestResultReaderAsync()
        {
            var reader = new ResultReader();
            reader.Columns.Add("a");
            reader.Columns.Add("b");
            reader.Columns.Add("c");

            Task.Run(() =>
            {
                for (var i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    var row = reader.NewRow();
                    row.Add(new object[] { (i + 1) * 1, (i + 1) * 2, (i + 1) * 3 });
                    reader.Write(row);
                }
                reader.Close();
            });

            var results = new List<int[]>();
            var sw = new Stopwatch();
            sw.Start();
            while (reader.Read())
            {
                var row = new int[3];
                row[0] = (int)reader[0];
                row[1] = (int)reader[1];
                row[2] = (int)reader[2];
                results.Add(row);
                Assert.True(400 < sw.ElapsedMilliseconds);
                Assert.True(600 > sw.ElapsedMilliseconds);
                sw.Restart();
            }
            reader.Dispose();

            Assert.Equal(6, results[2][1]);
            Assert.Equal(5, results.Count);
        }

        [Fact(DisplayName = "ResultReader to ResultTable")]
        public void TestReaderToTable()
        {
            var reader = new ResultReader();
            reader.Columns.Add("a");
            reader.Columns.Add("b");
            reader.Columns.Add("c");

            Task.Run(() =>
            {
                for (var i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    var row = reader.NewRow();
                    row.Add(new object[] { (i + 1) * 1, (i + 1) * 2, (i + 1) * 3 });
                    reader.Write(row);
                }
                reader.Close();
            });

            var table = new ResultTable(reader);

            Assert.Equal(6, table.Rows[2][1]);
            Assert.Equal(5, table.Rows.Count);
        }

        [Fact(DisplayName = "ResultTable to ResultReader")]
        public void TestTableToReader()
        {
            var table = new ResultTable();
            table.Columns.Add("a");
            table.Columns.Add("b");
            table.Columns.Add("c");

            for (var i = 0; i < 5; i++)
            {
                var row = table.NewRow();
                row.Add(new object[] { (i + 1) * 1, (i + 1) * 2, (i + 1) * 3 });
                table.Rows.Add(row);
            }

            var results = new List<int[]>();
            using (var reader = new ResultReader(table))
            {
                while (reader.Read())
                {
                    var row = new int[3];
                    row[0] = (int)reader[0];
                    row[1] = (int)reader[1];
                    row[2] = (int)reader[2];
                    results.Add(row);
                }
            }

            Assert.Equal(6, results[2][1]);
            Assert.Equal(5, results.Count);
        }
    }
}
