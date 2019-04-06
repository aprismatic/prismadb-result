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
        private bool _disposed = false;
        protected Exception _exception;

        [XmlIgnore]
        internal override IEnumerable<ResultRow> rows => _rows;

        public int FieldCount => CurrentRow.Count;

        public ResultRow CurrentRow { get; private set; }

        public int Depth => 0;

        public bool IsClosed => _rows.IsAddingCompleted;

        public int RecordsAffected => RowsAffected;

        public ResultReader() : this("") { }

        public ResultReader(string tableName) : base(tableName)
        {
            _rows = new BlockingCollection<ResultRow>();
        }

        public ResultReader(ResultTable table) : base(table)
        {
            _rows = new BlockingCollection<ResultRow>();

            Task.Run(() =>
            {
                try
                {
                    foreach (var row in table.rows)
                        _rows.Add(NewRow(row));
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
                finally
                {
                    _rows.CompleteAdding();
                }
            });
        }

        public bool Read()
        {
            try
            {
                CurrentRow = _rows.Take();
                return true;
            }
            catch (InvalidOperationException)
            {
                if (_exception != null)
                    throw _exception;
                return false;
            }
        }

        public object this[int index]
        {
            get => CurrentRow[index];
        }

        public object this[string columnName]
        {
            get => CurrentRow[GetOrdinal(columnName)];
        }

        public object this[ResultColumnHeader header]
        {
            get => CurrentRow[Columns.Headers.IndexOf(header)];
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
                throw new InvalidOperationException("ResultReader is not empty.");

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

        public DataTable GetSchemaTable()
        {
            throw new NotSupportedException();
        }

        public bool NextResult()
        {
            throw new NotSupportedException();
        }

        public bool GetBoolean(int i)
        {
            return (bool)CurrentRow[i];
        }

        public byte GetByte(int i)
        {
            return (byte)CurrentRow[i];
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            var bytes = (byte[])CurrentRow[i];
            var sourceMaxLen = bytes.Length - fieldOffset;
            var destMaxLen = buffer.Length - bufferoffset;
            var actualLen = Math.Min(Math.Min(sourceMaxLen, destMaxLen), length);
            Array.Copy(bytes, fieldOffset, buffer, bufferoffset, actualLen);
            return actualLen;
        }

        public char GetChar(int i)
        {
            return (char)CurrentRow[i];
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            var chars = (char[])CurrentRow[i];
            var sourceMaxLen = chars.Length - fieldoffset;
            var destMaxLen = buffer.Length - bufferoffset;
            var actualLen = Math.Min(Math.Min(sourceMaxLen, destMaxLen), length);
            Array.Copy(chars, fieldoffset, buffer, bufferoffset, actualLen);
            return actualLen;
        }

        public IDataReader GetData(int i)
        {
            throw new NotSupportedException();
        }

        public string GetDataTypeName(int i)
        {
            return GetFieldType(i).ToString();
        }

        public DateTime GetDateTime(int i)
        {
            return (DateTime)CurrentRow[i];
        }

        public decimal GetDecimal(int i)
        {
            return (decimal)CurrentRow[i];
        }

        public double GetDouble(int i)
        {
            return (double)CurrentRow[i];
        }

        public Type GetFieldType(int i)
        {
            return Columns[i].DataType;
        }

        public float GetFloat(int i)
        {
            return (float)CurrentRow[i];
        }

        public Guid GetGuid(int i)
        {
            return (Guid)CurrentRow[i];
        }

        public short GetInt16(int i)
        {
            return (short)CurrentRow[i];
        }

        public int GetInt32(int i)
        {
            return (int)CurrentRow[i];
        }

        public long GetInt64(int i)
        {
            return (long)CurrentRow[i];
        }

        public string GetName(int i)
        {
            return Columns[i].ColumnName;
        }

        public int GetOrdinal(string name)
        {
            return Columns.Headers.IndexOf(Columns.Headers.Single(x => x.ColumnName.Equals(name)));
        }

        public string GetString(int i)
        {
            return (string)CurrentRow[i];
        }

        public object GetValue(int i)
        {
            return CurrentRow[i];
        }

        public int GetValues(object[] values)
        {
            var maxLen = Math.Min(CurrentRow.Count, values.Length);
            for (var i = 0; i < maxLen; i++)
                values[i] = CurrentRow[i];
            return maxLen;
        }

        public bool IsDBNull(int i)
        {
            return CurrentRow[i] is DBNull;
        }
    }
}
