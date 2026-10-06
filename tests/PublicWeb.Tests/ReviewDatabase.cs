using Microsoft.Data.SqlClient;
using DataAccessLayer.Concrete;
using Microsoft.EntityFrameworkCore;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace PublicWeb.Tests;

// Every run owns one randomly named LocalDB database. No application configuration is read.
public sealed class ReviewDatabase : IDisposable
{
    private readonly string name = "LabourPest_PublicReviewTests_" + Guid.NewGuid().ToString("N");
    private readonly string? previousConnection = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    private const string Master = @"Server=(localdb)\MSSQLLocalDB;Database=master;Integrated Security=True;Encrypt=False;Connect Timeout=10;Pooling=False";
    public string Connection { get; }
    private readonly TestConnectionRedirect redirect;

    public ReviewDatabase()
    {
        Connection = new SqlConnectionStringBuilder(Master) { InitialCatalog = name }.ConnectionString;
        redirect = new TestConnectionRedirect(Connection);
        Execute(Master, $"CREATE DATABASE [{name}]");
        try
        {
            using var context = new Context();
            // Create only the review table from the real EF model. Unrelated legacy FK cycles
            // must not require shared schema changes merely to exercise a public review.
            var sql = System.Text.RegularExpressions.Regex.Match(context.Database.GenerateCreateScript(),
                @"CREATE TABLE \[Comments\] \([\s\S]+?\r?\n\);").Value;
            if (sql.Length == 0) throw new InvalidOperationException("Comments DDL was not found.");
            Execute(Connection, sql);
        }
        catch { Dispose(); throw; }
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", Connection);
    }

    public int Count => Convert.ToInt32(Execute(Connection, "SELECT COUNT(*) FROM dbo.Comments"));
    public void Clear() => Execute(Connection, "DELETE FROM dbo.Comments");
    public static object? Execute(string connection, string sql)
    {
        using var database = new SqlConnection(connection);
        database.Open();
        using var command = new SqlCommand(sql, database);
        return command.ExecuteScalar();
    }

    public void Dispose()
    {
        redirect.Dispose();
        Environment.SetEnvironmentVariable("ConnectionStrings__DefaultConnection", previousConnection);
        SqlConnection.ClearAllPools();
        if (!System.Text.RegularExpressions.Regex.IsMatch(name, "^LabourPest_PublicReviewTests_[a-f0-9]{32}$"))
            throw new InvalidOperationException("Unexpected test database name.");
        Execute(Master, $"ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]");
    }
}
