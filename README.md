# Welcome to SqlDsl

[![Build status](https://ci.appveyor.com/api/projects/status/8viaaqblmh5t1gwv?svg=true)](https://ci.appveyor.com/project/oakio/sqldsl)

[![Nuget Package](https://badgen.net/nuget/v/sqldsl)](https://www.nuget.org/packages/SqlDsl)

# Intro
Fluent SQL builder library.

## Features:
* `SELECT`, `DELETE`, `INSERT`, `UPDATE` queries
* `WHERE`, `JOIN`, `ORDER BY`, `GROUP BY`, `HAVING BY` clauses
* `LIKE`, `EXISTS`, `IN`, `BETWEEN` predicates
* `COUNT`, `SUM`, `MAX`, `MIN`, `AVG` functions
* `UNION` queries
* Multiple queries
* Table and Column aliases
* SQL injections free
* Partial `PostgreSQL` dialect support
* Strongly typed (checked at compile time)
* GC friendly

# Examples 
* `SELECT` query
    * [Hello world](#hello-world)
    * [Aliases](#aliases)
    * [Functions](#functions)
    * [`DISTINCT`](#distinct)
    * `WHERE` clause
        * [Predicates](#predicates) (`AND`, `OR`, `IS NULL` and others)
        * [`LIKE`](#like-predicate) predicate
        * [`IN`](#in-predicate) predicate
        * [`EXISTS`](#exists-predicate) predicate
        * [`BETWEEN`](#between-predicate) predicate
    * [`JOIN ON`](#join-on-clause) clause
    * [`ORDER BY`](#order-by-clause) clause
    * [`GROUP BY`](#group-by-clause) clause
    * [`HAVING`](#having-clause) clause
    * [Multiple](#multiple-queries) queries
* [`DELETE`](#delete-query) query
* [`INSERT`](#insert-query) query
* [`UPDATE`](#update-query) query

* `PostreSQL` dialect
    * [`OFFSET` and `LIMIT`](#postgresql-offset-and-limit-clauses) clauses

As an example, consider the following database schema (`authors` and `books` tables with one-to-many relationship):
```sql
CREATE TABLE authors (
    id integer PRIMARY KEY,
    name varchar(64)
)

CREATE TABLE books (
    id integer PRIMARY KEY,
    name varchar(512),
    author_id integer REFERENCES authors (id), -- one-to-many relationship
    rating real
)
```

For these tables create corresponding classes:
```csharp
public class AuthorsTable : Table
{
    public ColumnExpression<int> Id { get; }
    public ColumnExpression<string> Name { get; }
    public AuthorsTable(string alias = null) : base("authors", alias)
    {
        Id = CreateColumn<int>("id");
        Name = CreateColumn<string>("name");
    }
}

public class BooksTable : Table
{
    public ColumnExpression<int> Id { get; }
    public ColumnExpression<string> Name { get; }
    public ColumnExpression<int> AuthorId { get; }
    public ColumnExpression<double> Rating { get; }
    public BooksTable(string alias = null) : base("books", alias)
    {
        Id = CreateColumn<int>("id");
        Name = CreateColumn<string>("name");
        AuthorId = CreateColumn<int>("author_id");
        Rating = CreateColumn<double>("rating");
    }
}
```
## Hello world
```csharp
var b = new BooksTable();
var query = Sql
    .Select()
    .From(b);

// SELECT * FROM books
```
[up &#8593;](#examples)
## Aliases
```csharp
var b = new BooksTable();
var query = Sql
    .Select(b.Id, b.Name)
    .From(b);

// SELECT id, name FROM books
```
```csharp
var b = new BooksTable("t"); // table alias
var query = Sql
    .Select(b.Id, b.Name)
    .From(b);

// SELECT t.id, t.name FROM books t
```
```csharp
var b = new BooksTable("t");
var query = Sql
    .Select(b.Id, b.Name.As("author_name")) // column alias
    .From(b);

// SELECT t.id, t.name AS author_name FROM books t
```
[up &#8593;](#examples)
## Functions
```csharp
var b = new BooksTable();
var query = Sql
    .Select(Sql.Count())
    .From(b);

// SELECT COUNT(*) FROM books
```
```csharp
var b = new BooksTable();
var query = Sql
    .Select(Sql.Avg(b.Rating))
    .From(b);

// SELECT AVG(rating) FROM books
```
[up &#8593;](#examples)
## DISTINCT
```csharp
var a = new AuthorsTable();
var query = Sql
    .Select(a.Name)
    .Distinct()
    .From(a);

// SELECT DISTINCT name FROM authors
```
[up &#8593;](#examples)
## Predicates
```csharp
var b = new BooksTable();
var query = Sql
    .Select()
    .From(b)
    .Where(b.Name.IsNull.And(b.Rating <= 0));

// SELECT * FROM books WHERE name IS NULL AND rating <= @p1
```
[up &#8593;](#examples)
## LIKE predicate
```csharp
var a = new AuthorsTable();
var query = Sql
    .Select()
    .From(a)
    .Where(a.Name.Like("A%")); // started with 'A'

// SELECT * FROM authors WHERE name LIKE @p1
```
[up &#8593;](#examples)
## IN predicate
```csharp
var a = new AuthorsTable();
var query = Sql
    .Select()
    .From(a)
    .Where(a.Id.In(new[] {1, 2})); // where id==1 OR id==2

// SELECT * FROM authors WHERE id IN @p1
```
```csharp
var a = new AuthorsTable("a");
var b = new BooksTable("b");

var subQuery = Sql
    .Select(b.AuthorId)
    .From(b)
    .Where(b.Rating > 3);

var query = Sql
    .Select()
    .From(a)
    .Where(a.Id.In(subQuery)); // IN sub-query
                   
// SELECT * FROM authors a WHERE a.id IN (SELECT b.author_id FROM books b WHERE b.rating > @p1)");
```
[up &#8593;](#examples)
## EXISTS predicate
```csharp
var a = new AuthorsTable("a");
var b = new BooksTable("b");

var subQuery = Sql
    .Select()
    .From(b)
    .Where((a.Id == b.AuthorId).And(b.Rating > 3));

var query = Sql
    .Select()
    .From(a)
    .WhereExists(subQuery);

// SELECT * FROM authors a WHERE EXISTS (SELECT * FROM books b WHERE a.id = b.author_id AND b.rating > @p1
```
[up &#8593;](#examples)
## BETWEEN predicate
```csharp
var b = new BooksTable();
var query = Sql
    .Select()
    .From(b)
    .Where(b.Rating.Between(2, 4));

// SELECT * FROM books WHERE rating BETWEEN @p1 AND @p2
```
[up &#8593;](#examples)
## JOIN ON clause
```csharp
var a = new AuthorsTable("a");
var b = new BooksTable("b");
var query = Sql
    .Select()
    .Join(b, a.Id == b.AuthorId) // also LEFT, RIGHT, FULL JOIN
    .From(a);

// SELECT * FROM authors a JOIN books b ON a.id = b.author_id
```
[up &#8593;](#examples)
## ORDER BY clause
```csharp
var b = new BooksTable();
var query = Sql
    .Select()
    .OrderByDesc(b.Rating)
    .From(b);

// SELECT * FROM books ORDER BY rating DESC
```
[up &#8593;](#examples)
## GROUP BY clause
```csharp
var b = new BooksTable();
var query = Sql
    .Select(b.AuthorId, Sql.Count())
    .GroupBy(b.AuthorId)
    .From(b);

// SELECT author_id, COUNT(*) FROM books GROUP BY author_id
```
[up &#8593;](#examples)
## Multiple queries
```csharp
var a = new AuthorsTable();
var b = new BooksTable();
MultipleQuery query = Sql
    .Multiple(
        Sql.Select().From(a),
        Sql.Select().From(b)
    );
    
// SELECT * FROM authors; SELECT * FROM books        
```
[up &#8593;](#examples)
## HAVING clause
```csharp
var b = new BooksTable();
var query = Sql
    .Select(b.AuthorId, Sql.Count())
    .GroupBy(b.AuthorId)
    .Having(Sql.Count() > 3)
    .From(b);

// SELECT author_id, COUNT(*) FROM books GROUP BY author_id HAVING COUNT(*) > @p1
```
[up &#8593;](#examples)
## DELETE query
```csharp
var b = new BooksTable();
var query = Sql
    .Delete(b)
    .Where(b.Id == 1);

// DELETE FROM books WHERE id = @p1
```
[up &#8593;](#examples)
## INSERT query
```csharp
var a = new AuthorsTable();
var query = Sql
    .Insert(a)
    .Values(a.Id, 1)
    .Values(a.Name, "Adam");
                
// INSERT INTO authors (id, name) VALUES (@p1, @p2)
```
[up &#8593;](#examples)
## UPDATE query
```csharp
var b = new BooksTable();
var query = Sql
    .Update(b)
    .Set(b.Rating, b.Rating + 1)
    .Where(b.AuthorId == 1);

// UPDATE books SET rating = rating + @p1 WHERE author_id = @p2
```
[up &#8593;](#examples)
## PostgreSQL OFFSET and LIMIT clauses
```csharp
var a = new AuthorsTable()
PgSelectQuery query = PgSql
    .Select()
    .From(a)
    .OrderBy(a.Name)
    .Offset(5)
    .Limit(10)

// SELECT * FROM authors ORDER BY name OFFSET @p1 LIMIT @p2
```
[up &#8593;](#examples)

# How to build
```bash
# build
dotnet build ./src

# running tests
dotnet test ./src

# pack
dotnet pack ./src -c=release
```