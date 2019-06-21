using System;
using System.Runtime.Serialization;

namespace PrismaDB.Result
{
    [DataContract]
    public class ResultColumnHeader
    {
        [DataMember]
        public string ColumnName { get; set; }

        [DataMember]
        public int? MaxLength { get; set; }

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
