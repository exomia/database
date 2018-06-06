
## Information

exomia/database is a generic database manager with pooling & more

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

## Installing

```shell
[Package Manager]
PM> Install-Package Exomia.Database
```

## Changelog

## License

MIT License
Copyright (c) 2018 exomia

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.


