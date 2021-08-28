namespace SqlDsl.Core.Predicates
{
    public class AndExpression : PredicateExpression
    {
        private readonly PredicateExpression _left;
        private readonly PredicateExpression _right;

        public AndExpression(PredicateExpression left, PredicateExpression right)
        {
            _left = left;
            _right = right;
        }

        public override void Format(ISqlWriter sql)
        {
            _left.Format(sql);
            sql.Append(" AND ");
            _right.Format(sql);
        }
    }
}