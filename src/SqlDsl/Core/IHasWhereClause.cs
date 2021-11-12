using SqlDsl.Core.Predicates;

namespace SqlDsl.Core
{
    public interface IHasWhereClause<out TQuery>
    {
        TQuery Where(PredicateExpression condition);

        TQuery WhereExists<TSubQuery>(TSubQuery query) where TSubQuery : SelectQueryBase<TSubQuery>, new();

        TQuery WhereNotExists<TSubQuery>(TSubQuery query) where TSubQuery : SelectQueryBase<TSubQuery>, new();
    }
}