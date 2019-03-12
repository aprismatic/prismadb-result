using Newtonsoft.Json;
using PrismaDB.Result;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ResultTests
{
    public class TestResult
    {
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
            row2.Add(new object[] { 2, null, "xyz" });
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
        }

        [Fact(DisplayName = "ResultReader Async")]
        public void TestResultReaderAsync()
        {
            var reader = new ResultReader();
            reader.Columns.Add("a");
            reader.Columns.Add("b");
            reader.Columns.Add("c");

            new Thread(() =>
            {
                for (var i = 0; i < 5; i++)
                {
                    Thread.Sleep(500);
                    var row = reader.NewRow();
                    row.Add(new object[] { (i + 1) * 1, (i + 1) * 2, (i + 1) * 3 });
                    reader.Write(row);
                }
                reader.EndWrite();
            }).Start();

            var rowCount = 0;
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
                rowCount++;
                Assert.True(400 < sw.ElapsedMilliseconds);
                Assert.True(600 > sw.ElapsedMilliseconds);
                sw.Restart();
            }
            reader.Dispose();

            Assert.Equal(6, results[2][1]);
            Assert.Equal(5, rowCount);
        }
    }
}
