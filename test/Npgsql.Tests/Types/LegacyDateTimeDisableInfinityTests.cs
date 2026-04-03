using Npgsql.Internal.ResolverFactories;
using NUnit.Framework;
using System;
using NpgsqlTypes;
using static Npgsql.Util.Statics;

namespace Npgsql.Tests.Types;

[NonParallelizable]
public class LegacyDateTimeDisableInfinityTests : TestBase
{
    [Test]
    public void Timestamp_write_as_min_max()
    {
        using var con = OpenConnection();
        con.ExecuteNonQuery(
            """
            DROP TABLE IF EXISTS pg_temp.test_timestamp;
            CREATE TABLE pg_temp.test_timestamp (ts timestamp);
            """);

        {
            // Write DateTime.MinValue to a timestamp column - it should be written as 0001-01-01 00:00:00, not -infinity
            using var command = con.CreateCommand();
            command.CommandText = "INSERT INTO pg_temp.test_timestamp (ts) VALUES (@p0);";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();
            command.Parameters.Clear();

            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("0001-01-01 00:00:00"));
        }

        {
            // Write -infinity to a timestamp column - it should be read back as DateTime.MinValue
            using var command = con.CreateCommand();
            command.CommandText = "TRUNCATE TABLE pg_temp.test_timestamp; INSERT INTO pg_temp.test_timestamp (ts) VALUES ('-infinity'::timestamp);";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();

            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("-infinity"));

            command.CommandText = "SELECT ts FROM pg_temp.test_timestamp;";
            value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo(DateTime.MinValue));
        }

        {
            // Write DateTime.MinValue to a timestamp column using binary import - it should be written as 0001-01-01 00:00:00, not -infinity
            using var command = con.CreateCommand();
            command.CommandText = "TRUNCATE TABLE pg_temp.test_timestamp;";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();

            var importer = con.BeginBinaryImport("COPY pg_temp.test_timestamp (ts) FROM STDIN BINARY");
            importer.StartRow();
            importer.Write(DateTime.MinValue, NpgsqlDbType.Timestamp);
            importer.Complete();
            importer.Close();

            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("0001-01-01 00:00:00"));
        }
    }

    [Test]
    public void Date_write_as_min_max()
    {
        using var con = OpenConnection();
        con.ExecuteNonQuery(
            """
            DROP TABLE IF EXISTS pg_temp.test_timestamp;
            CREATE TABLE pg_temp.test_timestamp (ts date);
            """);

        {
            // Write DateTime.MinValue to a date column - it should be written as 0001-01-01, not -infinity
            using var command = con.CreateCommand();
            command.CommandText = "INSERT INTO pg_temp.test_timestamp (ts) VALUES (@p0);";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();

            command.Parameters.Clear();
            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("0001-01-01"));
        }

        {
            // Write -infinity to a date column - it should be read back as DateTime.MinValue
            using var command = con.CreateCommand();
            command.CommandText = "TRUNCATE TABLE pg_temp.test_timestamp; INSERT INTO pg_temp.test_timestamp (ts) VALUES ('-infinity'::timestamp);";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();

            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("-infinity"));

            command.CommandText = "SELECT ts FROM pg_temp.test_timestamp;";
            value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo(DateTime.MinValue));
        }

        {
            // Write DateTime.MinValue to a date column using binary import - it should be written as 0001-01-01, not -infinity
            using var command = con.CreateCommand();
            command.CommandText = "TRUNCATE TABLE pg_temp.test_timestamp;";
            command.Parameters.AddWithValue("p0", DateTime.MinValue);
            command.ExecuteNonQuery();

            var importer = con.BeginBinaryImport("COPY pg_temp.test_timestamp (ts) FROM STDIN BINARY");
            importer.StartRow();
            importer.Write(DateTime.MinValue, NpgsqlDbType.Date);
            importer.Complete();
            importer.Close();

            command.CommandText = "SELECT ts::text FROM pg_temp.test_timestamp;";
            var value = command.ExecuteScalar();
            Assert.That(value, Is.EqualTo("0001-01-01"));
        }
    }



    NpgsqlDataSource _dataSource = null!;
    protected override NpgsqlDataSource DataSource => _dataSource;

    [OneTimeSetUp]
    public void Setup()
    {
#if DEBUG
        LegacyTimestampBehavior = true;
        DisableDateTimeInfinityConversions = true;
        _dataSource = CreateDataSource(builder =>
        {
            // Can't use the static AdoTypeInfoResolver instance, it already captured the feature flag.
            builder.AddTypeInfoResolverFactory(new AdoTypeInfoResolverFactory());
            builder.ConnectionStringBuilder.Timezone = "Europe/Berlin";
        });
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
#else
        Assert.Ignore(
            "Legacy DateTime tests rely on the Npgsql.EnableLegacyTimestampBehavior AppContext switch and can only be run in DEBUG builds");
#endif
    }

#if DEBUG
    [OneTimeTearDown]
    public void Teardown()
    {
        LegacyTimestampBehavior = false;
        DisableDateTimeInfinityConversions = false;
        _dataSource.Dispose();
        NpgsqlDataSourceBuilder.ResetGlobalMappings(overwrite: true);
    }
#endif

}
