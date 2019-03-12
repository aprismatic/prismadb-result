using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public class ResultReader : ResultQueryResponse, IDisposable
    {
        protected BlockingCollection<ResultRow> _rows;
        private ResultRow _currentRow;
        private bool _disposed = false;

        [XmlIgnore]
        internal override IEnumerable<ResultRow> rows => _rows;

        //[XmlIgnore]
        //private BlockingCollection<ResultRow> Rows => _rows;

        public ResultReader() : this("") { }

        public ResultReader(string tableName) : base(tableName)
        {
            _rows = new BlockingCollection<ResultRow>();
        }

        public ResultReader(ResultQueryResponse other) : this(other.TableName)
        {
            foreach (var row in other.rows)
                _rows.Add(row);
        }

        public bool Read()
        {
            try
            {
                _currentRow = _rows.Take();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public object Get(int index)
        {
            return _currentRow[index];
        }

        public object this[int index]
        {
            get => _currentRow[index];
        }

        public object this[string columnName]
        {
            get => _currentRow[Columns.Headers.IndexOf(
                Columns.Headers.Single(x => x.ColumnName.Equals(columnName)))];
        }

        public object this[ResultColumnHeader header]
        {
            get => _currentRow[Columns.Headers.IndexOf(header)];
        }

        public void Write(ResultRow row)
        {
            _rows.Add(row);
        }

        public void EndWrite()
        {
            _rows.CompleteAdding();
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
                        resCol.DataType = (Type)row[col.Ordinal];
                }
                Columns.Add(resCol);
            }

            while (reader.Read())
            {
                var resRow = NewRow();
                for (var i = 0; i < Columns.Count; i++)
                    resRow.Add(reader.GetValue(i));
                _rows.Add(resRow);
            }
            _rows.CompleteAdding();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _rows.CompleteAdding();
                _rows.Dispose();
            }

            _disposed = true;
        }
    }
}
