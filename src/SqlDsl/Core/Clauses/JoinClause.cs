using SqlDsl.Core.Expressions;
using SqlDsl.Core.Predicates;

namespace SqlDsl.Core.Clauses
{
    public readonly struct JoinClause : ISqlFormattable
    {
        private readonly string _type;
        private readonly TableAliasExpression _table;
        private readonly PredicateExpression _condition;

        public JoinClause(string type, ITable table, PredicateExpression condition)
        {
            _type = type;
            _condition = condition;
            _table = new TableAliasExpression(table, false);
        }

        public void Format(ISqlWriter sql)
        {
            sql.Append(_type);
            _table.Format(sql);
            sql.Append(" ON ");
            _condition.Format(sql);
        }
    }
}