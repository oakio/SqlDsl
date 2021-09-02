using System.Collections.Generic;
using SqlDsl.Core;
using SqlDsl.Core.Expressions;

namespace SqlDsl
{
    public class InsertQuery : IQuery
    {
        private readonly Table _table;
        private readonly List<IInsertValue> _values;

        public InsertQuery(Table table)
        {
            _table = table;
            _values = new List<IInsertValue>();
        }

        public InsertQuery Values<T>(ColumnExpression<T> column, T value)
        {
            var insertValue = new InsertValue<T>(column, value);
            _values.Add(insertValue);
            return this;
        }

        public void Format(ISqlWriter sql)
        {
            sql.Append("INSERT INTO ");
            sql.Append(_table.GetName());
            sql.Append(" (");
            for (int i = 0; i < _values.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                _values[i].WriteColumn(sql);
            }
            sql.Append(") VALUES (");
            for (int i = 0; i < _values.Count; i++)
            {
                if (i > 0)
                {
                    sql.Append(", ");
                }
                _values[i].WriteValue(sql);
            }
            sql.Append(")");
        }
    }
}