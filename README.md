
## Information

exomia/database is a generic database manager with pooling & more

![](https://img.shields.io/github/issues-pr/exomia/database.svg)
![](https://img.shields.io/github/issues/exomia/database.svg)
![](https://img.shields.io/github/last-commit/exomia/database.svg)
![](https://img.shields.io/github/contributors/exomia/database.svg)
![](https://img.shields.io/github/commit-activity/y/exomia/database.svg)
![](https://img.shields.io/github/languages/top/exomia/database.svg)
![](https://img.shields.io/github/languages/count/exomia/database.svg)
![](https://img.shields.io/github/license/exomia/database.svg)

## Installing

```shell
[Package Manager]
PM> Install-Package Exomia.Database
```

## Example

Database
```csharp
class PostgreSQL : ADatabase<NpgsqlCommand>
{
	/// <inheritdoc />
	protected override bool CreateConnection(out DbConnection connection)
	{
		connection = new NpgsqlConnection("Server=127.0.0.1;Port=5432;Database=xyz;User Id=xyz;Password=******;");
		return true;
	}

	/// <inheritdoc />
	protected override void PrepareCommands()
	{
		Add(1, "SELECT * FROM private.\"user\" WHERE \"username\" = @username LIMIT 1;", command =>
		{
			command.Parameters.Add("@username", NpgsqlDbType.Text);
		}, true);
	}

	public void SelectUser(string username)
	{
		NpgsqlCommand cmd = Get(1, username);
		using (NpgsqlDataReader reader = cmd.ExecuteReader())
		{
			if (!reader.HasRows) { return; }
			while (reader.Read())
			{
				for (int i = 0; i < reader.FieldCount; ++i)
				{
					Console.Write(reader[i] + " ");
				}
				Console.WriteLine();
			}
		}  
	}

	public object GetColumnFromUser(string username, int column)
	{
		NpgsqlCommand cmd = Get(1, username);
		using (NpgsqlDataReader reader = cmd.ExecuteReader())
		{
			if (!reader.HasRows) { return null; }
			while (reader.Read())
			{
			return reader[column];
			}
		}
		return null;
	}
}
```

Program
```csharp
static void Main(string[] args)
{
	//static DatabaseManager for PostgreSQL
	DatabaseManager<PostgreSQL>.Register(
		5, 
		() => new SemaphoreLockDatabaseIOPoolContainer<PostgreSQL>(5),
		database => database.Connect());

	for (int i = 0; i < 2; i++)
	{
		DatabaseManager<PostgreSQL>.Lock(database => database.SelectUser("username"));
	}

	Parallel.For(0, 10, i =>
	{
		DatabaseManager<PostgreSQL>.Lock(database => database.SelectUser("username"));
	});

	//DatabaseManager for one or multiple Database Types
	DatabaseManager mgr = new DatabaseManager();
	mgr.Register<PostgreSQL>(
		5, 
		() => new SemaphoreLockDatabaseIOPoolContainer<PostgreSQL>(5), 
		database => database.Connect());
	Console.WriteLine("EXECUTE...");
	for (int i = 0; i < 2; i++)
	{
		Console.WriteLine(mgr.Lock<PostgreSQL, object>(database => database.GetColumnFromUser("username", 2)));
	}
	Console.ReadKey();
}
```

