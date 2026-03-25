using System;
using System.Data;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

[NonParallelizable]
[TestFixture(false)]
[TestFixture(true)]
public class TinyintTests(bool useTinyInt) : TestBase
{
    public bool UseTinyInt { get; } = useTinyInt;

    [OneTimeSetUp]
    public void Setup()
    {
        using var conn = OpenConnection();
        conn.ExecuteNonQuery("DROP TABLE IF EXISTS tinyint_test");
        if (UseTinyInt)
            EnsureExtension(conn, "tinyint");
        else
            DropExtensionAsync(conn, "tinyint").Wait();

        conn.ExecuteNonQuery($"CREATE TABLE tinyint_test (col1 INT, col2 SMALLINT, col3 {(UseTinyInt ? "TINYINT" : "SMALLINT")})");
        // insert 5 rows
        for (var i = 0; i < 5; i++)
            conn.ExecuteNonQuery($"INSERT INTO tinyint_test VALUES ({i}, {i}, {i})");
    }

    [Test]
    public void CanReadTinyint()
    {
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand($"SELECT 8::{(UseTinyInt ? "TINYINT" : "SMALLINT")}, 8::smallint, 8::int", conn);
        using var reader = cmd.ExecuteReader();
        reader.Read();
        Assert.That(reader.GetByte(0), Is.EqualTo(8));
        Assert.That(reader.GetInt16(1), Is.EqualTo(8));
        Assert.That(reader.GetInt32(2), Is.EqualTo(8));
    }

    [Test]
    public void CanRead_tinyint_test()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test ORDER BY col1", conn);

        var iRecord = 0;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }
    }

    [Test]
    public void CanUseTinyintParameter()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<byte>("@p0", 2));

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }
    }

    [Test]
    public void CanUseTinyintValueParameter()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter{ParameterName = "@p0", Value = (byte)2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintValueParameterGeneric()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<byte>{ParameterName = "@p0", TypedValue = 2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintTinyintParameter()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter("@p0", UseTinyInt ? NpgsqlDbType.Tinyint : NpgsqlDbType.Smallint){Value = (byte)2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintTinyintParameterGeneric()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<byte>("@p0", UseTinyInt ? NpgsqlDbType.Tinyint : NpgsqlDbType.Smallint){TypedValue = 2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintByteParameter()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter("@p0", DbType.Byte){Value = (byte)2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintByteParameterGeneric()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<byte>("@p0", DbType.Byte){TypedValue = 2});

        var iRecord = 2;
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            Assert.That(reader.GetInt32(0), Is.EqualTo(iRecord));
            Assert.That(reader.GetInt16(1), Is.EqualTo(iRecord));
            Assert.That(reader.GetByte(2), Is.EqualTo(iRecord));
            iRecord++;
        }

        var p = cmd.Parameters[0];
        p.GetResolutionInfo(out var info, out _, out _);
        var typ = info!.Options.DatabaseInfo.GetPostgresType(info.PgTypeId!.Value);
        Assert.That(typ, Is.Not.Null);
        Assert.That(typ.Name, Is.EqualTo(UseTinyInt ? "tinyint" : "smallint"));
    }

    [Test]
    public void CanUseTinyintBynaryImport()
    {
        using var conn = OpenConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            // Count rows before import
            var before = Convert.ToInt64(new NpgsqlCommand("SELECT count(*) FROM tinyint_test", conn).ExecuteScalar());

            // Insert 3 rows
            using (var importer = conn.BeginBinaryImport("COPY tinyint_test (col1, col2, col3) FROM STDIN (FORMAT BINARY)"))
            {
                for (var i = 0; i < 3; i++)
                {
                    importer.StartRow();
                    var v = 100 + i; // safe value for tinyint/smallint
                    importer.Write(v);
                    importer.Write((short)v);
                    importer.Write((byte)v);
                }

                importer.Complete();
            }

            var after = Convert.ToInt64(new NpgsqlCommand("SELECT count(*) FROM tinyint_test", conn).ExecuteScalar());
            Assert.That(after, Is.EqualTo(before + 3));
        }
        finally
        {
            tx.Rollback();
        }
    }

    [Test]
    public void CanUseTinyintBynaryImportWithType()
    {
        using var conn = OpenConnection();
        using var tx = conn.BeginTransaction();
        try
        {
            // Count rows before import
            var before = Convert.ToInt64(new NpgsqlCommand("SELECT count(*) FROM tinyint_test", conn).ExecuteScalar());

            // Insert 3 rows
            using (var importer = conn.BeginBinaryImport("COPY tinyint_test (col1, col2, col3) FROM STDIN (FORMAT BINARY)"))
            {
                for (var i = 0; i < 3; i++)
                {
                    importer.StartRow();
                    var v = 100 + i; // safe value for tinyint/smallint
                    importer.Write(v);
                    importer.Write((short)v);
                    importer.Write((byte)v, UseTinyInt ? NpgsqlDbType.Tinyint : NpgsqlDbType.Smallint);
                }

                importer.Complete();
            }

            var after = Convert.ToInt64(new NpgsqlCommand("SELECT count(*) FROM tinyint_test", conn).ExecuteScalar());
            Assert.That(after, Is.EqualTo(before + 3));
        }
        finally
        {
            tx.Rollback();
        }
    }

}
