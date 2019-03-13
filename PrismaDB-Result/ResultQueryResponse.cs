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

        public ResultQueryResponse(string tableName) : base()
        {
            Columns = new ResultColumnList(this);
            TableName = tableName;
        }

        public ResultQueryResponse(ResultQueryResponse other) : this(other.TableName)
        {
            foreach (var column in other.Columns)
                Columns.Add(column);
        }

        public ResultRow NewRow()
        {
            return new ResultRow(this);
        }

        public ResultRow NewRow(ResultRow other)
        {
            return new ResultRow(this, other);
        }

        public abstract void Load(IDataReader reader);
    }
}
