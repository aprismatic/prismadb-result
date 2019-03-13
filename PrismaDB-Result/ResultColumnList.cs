using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PrismaDB.Result
{
    public class ResultColumnList : IEnumerable<ResultColumnHeader>
    {
        protected ResultQueryResponse _table;

        public List<ResultColumnHeader> Headers { get; protected set; }

        protected ResultColumnList() { }

        protected internal ResultColumnList(ResultQueryResponse table)
        {
            _table = table;
            Headers = new List<ResultColumnHeader>();
        }

        public int Count => this.Count();

        public ResultColumnHeader this[int index] => Headers[index];

        public ResultColumnHeader this[string columnName] =>
            this[Headers.IndexOf(Headers.Single(x => x.ColumnName.Equals(columnName)))];

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ResultColumnHeader>)Headers).GetEnumerator();
        }

        public IEnumerator<ResultColumnHeader> GetEnumerator()
        {
            return ((IEnumerable<ResultColumnHeader>)Headers).GetEnumerator();
        }

        public void Add(ResultColumnHeader column)
        {
            if (_table.rows != null && _table.rows.Any())
                throw new ApplicationException("Table is not empty.");
            Headers.Add(column);
        }

        public void Add()
        {
            Add(new ResultColumnHeader());
        }

        public void Add(string columnName, Type dataType = null, int? maxLength = null)
        {
            Add(new ResultColumnHeader(columnName, dataType, maxLength));
        }

        public void Remove(ResultColumnHeader column)
        {
            Remove(Headers.IndexOf(column));
        }

        public void Remove(string columnName)
        {
            Remove(Headers.IndexOf(Headers.Single(x => x.ColumnName.Equals(columnName))));
        }

        public void Remove(int index)
        {
            if (_table is ResultReader)
                throw new NotSupportedException("Cannot remove columns from ResultReader");
            foreach (var row in _table.rows)
                row.Items.RemoveAt(index);
            Headers.RemoveAt(index);
        }
    }
}