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

        [DataMember]
        public string TypeName { get; private set; }

        private Type dataType;
        public Type DataType
        {
            get
            {
                return dataType;
            }
            set
            {
                dataType = value;

                if (value == typeof(string))
                    TypeName = "string";
                else if (value == typeof(short))
                    TypeName = "short";
                else if (value == typeof(int))
                    TypeName = "int";
                else if (value == typeof(long))
                    TypeName = "long";
                else if (value == typeof(float))
                    TypeName = "float";
                else if (value == typeof(double))
                    TypeName = "double";
                else if (value == typeof(decimal))
                    TypeName = "decimal";
                else if (value == typeof(DateTime))
                    TypeName = "datetime";
                else if (value == typeof(byte[]))
                    TypeName = "byte[]";
                else if (value == typeof(DBNull))
                    TypeName = "null";
                else
                    TypeName = null;
            }
        }

        public ResultColumnHeader() : this("") { }

        public ResultColumnHeader(string columnName, Type dataType = null, int? maxLength = null)
        {
            ColumnName = columnName;
            DataType = dataType;
            MaxLength = maxLength;
        }
    }
}
