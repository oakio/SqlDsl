using System.Collections.Generic;
using SqlDsl.Core.Expressions;

namespace SqlDsl.Core.Predicates
{
    public sealed class InExpression<T> : PredicateExpression
    {
        private readonly ColumnExpression<T> _left;
        private readonly IReadOnlyCollection<T> _right;

        public InExpression(ColumnExpression<T> left, IReadOnlyCollection<T> right)
        {
            _left = left;
            _right = right;
        }

        public override void Format(ISqlWriter writer)
        {
            string name = writer.AddParam(_right);

            _left.Format(writer);
            writer.Append(" IN ");
            writer.Append(name);
        }
    }
}