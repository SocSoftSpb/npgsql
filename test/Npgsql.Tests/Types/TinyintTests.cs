using System.Data;
using System.Threading.Tasks;
using NpgsqlTypes;
using NUnit.Framework;
using static Npgsql.Tests.TestUtil;

namespace Npgsql.Tests.Types;

[TestFixture]
public class TinyintTests : TestBase
{
    public bool UseTinyInt => true;

    [OneTimeSetUp]
    public async Task Setup()
    {
        await using var conn = await OpenConnectionAsync();
        if (UseTinyInt)
            await EnsureExtensionAsync(conn, "tinyint");
        await conn.ExecuteNonQueryAsync("DROP TABLE IF EXISTS tinyint_test");
        await conn.ExecuteNonQueryAsync($"CREATE TABLE tinyint_test (col1 INT, col2 SMALLINT, col3 {(UseTinyInt ? "TINYINT" : "SMALLINT")})");
        // insert 5 rows
        for (var i = 0; i < 5; i++)
            await conn.ExecuteNonQueryAsync($"INSERT INTO tinyint_test VALUES ({i}, {i}, {i})");
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
    public void CanUseTinyintTinyintParameter()
    {
        // Читаем из таблицы tinyint_test, которая содержит 5 строк с числами от 0 до 4
        using var conn = OpenConnection();
        using var cmd = new NpgsqlCommand("SELECT col1, col2, col3 FROM tinyint_test where col3=@p0 ORDER BY col1", conn);
        cmd.Parameters.Add(new NpgsqlParameter<byte>("@p0", NpgsqlDbType.Tinyint){TypedValue = 2});

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
    public void CanUseTinyintByteParameter()
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
    }


}
