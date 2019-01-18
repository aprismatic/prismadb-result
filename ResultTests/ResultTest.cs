using Newtonsoft.Json;
using PrismaDB.Result;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Xunit;

namespace ResultTests
{
    public class ResultTest
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
    }
}
