using Newtonsoft.Json;
using System;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public class ResultColumnHeader
    {
        public string ColumnName { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public int? MaxLength { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Type DataType { get; set; }

        public ResultColumnHeader() : this("") { }

        public ResultColumnHeader(string columnName, Type dataType = null, int? maxLength = null)
        {
            ColumnName = columnName;
            DataType = dataType;
            MaxLength = maxLength;
        }
    }
}
