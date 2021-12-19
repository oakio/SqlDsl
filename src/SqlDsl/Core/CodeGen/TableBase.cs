using SqlDsl.Core.Expressions;

namespace SqlDsl.Core.CodeGen
{
    public abstract class TableBase : ITable
    {
        private readonly string _name;

        private readonly string _alias;

        protected TableBase(string name, string alias)
        {
            _name = string.Intern(name);
            _alias = alias;
        }

        public string GetName() => _name;

        public string GetAlias() => _alias;

        protected Column<T> CreateColumn<T>(string name) => new Column<T>(name, _alias ?? _name);
    }
}