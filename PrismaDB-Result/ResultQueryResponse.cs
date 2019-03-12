using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public abstract class ResultQueryResponse : ResultResponse
    {
        public ResultColumnList Columns { get; protected set; }

        [XmlIgnore]
        internal abstract IEnumerable<ResultRow> rows { get; }

        public string TableName { get; set; }

        public ResultQueryResponse() : this("") { }

        public ResultQueryResponse(string tableName)
        {
            Columns = new ResultColumnList(this);
            TableName = tableName;
        }

        public ResultRow NewRow()
        {
            return new ResultRow(this);
        }

        public abstract void Load(IDataReader reader);
    }
}
