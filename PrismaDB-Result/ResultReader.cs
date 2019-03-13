using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PrismaDB.Result
{
    public class ResultReader : ResultQueryResponse, IDataReader, IDisposable
    {
        protected BlockingCollection<ResultRow> _rows;
        internal ResultRow currentRow;
        private bool _disposed = false;

        [XmlIgnore]
        internal override IEnumerable<ResultRow> rows => _rows;

        public int FieldCount => currentRow.Count;

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => _rows.IsAddingCompleted;

        public int RecordsAffected => RowsAffected;

        //[XmlIgnore]
        //private BlockingCollection<ResultRow> Rows => _rows;

        public ResultReader() : this("") { }

        public ResultReader(string tableName) : base(tableName)
        {
            _rows = new BlockingCollection<ResultRow>();
        }

        public ResultReader(ResultTable table) : base(table)
        {
            _rows = new BlockingCollection<ResultRow>();

            new Task(() =>
            {
                foreach (var row in table.rows)
                    Write(NewRow(row));
                Close();
            }).Start();
        }

        public bool Read()
        {
            try
            {
                currentRow = _rows.Take();
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        public object this[int index]
        {
            get => currentRow[index];
        }

        public object this[string columnName]
        {
            get => currentRow[Columns.Headers.IndexOf(
                Columns.Headers.Single(x => x.ColumnName.Equals(columnName)))];
        }

        public object this[ResultColumnHeader header]
        {
            get => currentRow[Columns.Headers.IndexOf(header)];
        }

        public void Write(ResultRow row)
        {
            _rows.Add(row);
        }

        public void Close()
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
            new Task(() =>
            {
                while (reader.Read())
                {
                    var resRow = NewRow();
                    for (var i = 0; i < Columns.Count; i++)
                        resRow.Add(reader.GetValue(i));
                    _rows.Add(resRow);
                }
                _rows.CompleteAdding();
            }).Start();
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

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(int i)
        {
            return (bool)currentRow[i];
        }

        public byte GetByte(int i)
        {
            return (byte)currentRow[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            return (char)currentRow[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)currentRow[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)currentRow[i];
        }

        public double GetDouble(int i)
        {
            return (double)currentRow[i];
        }

        public Type GetFieldType(int i)
        {
            return Columns[i].DataType;
        }

        public float GetFloat(int i)
        {
            return (float)currentRow[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)currentRow[i];
        }

        public short GetInt16(int i)
        {
            return (short)currentRow[i];
        }

        public int GetInt32(int i)
        {
            return (int)currentRow[i];
        }

        public long GetInt64(int i)
        {
            return (long)currentRow[i];
        }

        public string GetName(int i)
        {
            return Columns[i].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            return (string)currentRow[i];
        }

        public object GetValue(int i)
        {
            return currentRow[i];
        }

        public int GetValues(object[] values)
        {
            values = new object[currentRow.Count];
            for (var i = 0; i < values.Length; i++)
                values[i] = currentRow[i];
            return values.Length;
        }

        public bool IsDBNull(int i)
        {
            return currentRow[i] is DBNull;
        }
    }
}
