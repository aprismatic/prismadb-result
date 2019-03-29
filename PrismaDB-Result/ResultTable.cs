using System;
using System.Collections.Generic;
using System.Data;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public class ResultTable : ResultQueryResponse
    {
        protected List<ResultRow> _rows;

        [XmlIgnore]
        internal override IEnumerable<ResultRow> rows => _rows;

        [XmlIgnore]
        public List<ResultRow> Rows => _rows;

        public ResultTable() : this("") { }

        public ResultTable(string tableName) : base(tableName)
        {
            _rows = new List<ResultRow>();
        }

        public ResultTable(ResultReader reader) : base(reader)
        {
            _rows = new List<ResultRow>();

            while (reader.Read())
                _rows.Add(NewRow(reader.CurrentRow));
        }

        public override void Load(IDataReader reader)
        {
            if (Columns.Count > 0)
                throw new ApplicationException("ResultReader is not empty.");

            var schemaTable = reader.GetSchemaTable();
            foreach (DataRow row in reader.GetSchemaTable().Rows)
            {
                var resCol = new ResultColumnHeader();
                foreach (DataColumn col in schemaTable.Columns)
                {
                    if (col.ColumnName == "ColumnName")
                        resCol.ColumnName = (string)row[col.Ordinal];
                    if (col.ColumnName == "ColumnSize")
                        resCol.MaxLength = (int)row[col.Ordinal];
                    if (col.ColumnName == "DataType")
                    {
                        if (row[col.Ordinal] is DBNull)
                            resCol.DataType = typeof(DBNull);
                        else
                            resCol.DataType = (Type)row[col.Ordinal];
                    }
                }
                Columns.Add(resCol);
            }

            RowsAffected = reader.RecordsAffected;

            while (reader.Read())
            {
                var resRow = NewRow();
                for (var i = 0; i < Columns.Count; i++)
                    resRow.Add(reader.GetValue(i));
                _rows.Add(resRow);
            }
        }

        public override string ToString()
        {
            return ResultTablePrinter.GetDataInTableFormat(this);
        }
    }
}
