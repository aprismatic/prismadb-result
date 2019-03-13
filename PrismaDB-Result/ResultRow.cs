using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PrismaDB.Result
{
    public class ResultRow : IEnumerable<object>
    {
        protected ResultQueryResponse _table;

        protected internal List<object> Items { get; protected set; }

        protected ResultRow() { }

        protected internal ResultRow(ResultQueryResponse table)
        {
            _table = table;
            Items = new List<object>(_table.Columns.Headers.Count);
        }

        protected internal ResultRow(ResultQueryResponse table, ResultRow other) : this(table)
        {
            foreach (var item in other)
                Items.Add(item);
        }

        public int Count => this.Count();

        public object this[int index]
        {
            get => Items[index];

            set
            {
                if (index > _table.Columns.Headers.Count - 1)
                    throw new ApplicationException("Index is out of the range of columns in table.");
                Items[index] = value;
            }
        }

        public object this[string columnName]
        {
            get => Items[_table.Columns.Headers.IndexOf(
                _table.Columns.Headers.Single(
                    x => x.ColumnName.Equals(columnName)))];
            set
            {
                Items[_table.Columns.Headers.IndexOf(
                    _table.Columns.Headers.Single(
                        x => x.ColumnName.Equals(columnName)))] = value;
            }
        }

        public object this[ResultColumnHeader header]
        {
            get => Items[_table.Columns.Headers.IndexOf(header)];

            set
            {
                Items[_table.Columns.Headers.IndexOf(header)] = value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<object>)Items).GetEnumerator();
        }

        public IEnumerator<object> GetEnumerator()
        {
            return ((IEnumerable<object>)Items).GetEnumerator();
        }

        public void Add(object value)
        {
            if (Items.Count >= _table.Columns.Headers.Count)
                throw new ApplicationException("Items in row has reached the number of columns in table.");
            Items.Add(value);
        }

        public void Add(IEnumerable<object> valList)
        {
            foreach (var val in valList)
                Add(val);
        }
    }
}