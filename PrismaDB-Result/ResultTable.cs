using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public class ResultTable
    {
        protected List<ResultRow> _rows;

        public ResultColumnList Columns { get; protected set; }

        [XmlIgnore]
        public List<ResultRow> Rows => _rows;

        public string TableName { get; set; }

        public ResultTable()
        {
            Columns = new ResultColumnList(this);
            _rows = new List<ResultRow>();
        }

        public ResultTable(string tableName) : this()
        {
            TableName = tableName;
        }

        public ResultRow NewRow()
        {
            return new ResultRow(this);
        }

        public void Load(IDataReader reader)
        {
            if (Rows.Count > 0 || Columns.Count > 0)
                throw new ApplicationException("ResultTable is not empty.");

            // TODO: Change to GetSchemaTable(), probably better performance
            var dataTable = new DataTable();
            dataTable.Load(reader);

            foreach (DataColumn column in dataTable.Columns)
                Columns.Add(column.ColumnName, column.DataType, column.MaxLength);

            foreach (DataRow row in dataTable.Rows)
            {
                var resRow = this.NewRow();
                resRow.Add(row.ItemArray);
                Rows.Add(resRow);
            }
        }
    }
}
