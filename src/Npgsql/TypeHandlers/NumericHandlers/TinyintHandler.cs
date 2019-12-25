using System.Data;
using Npgsql.BackendMessages;
using Npgsql.PostgresTypes;
using Npgsql.TypeHandling;
using Npgsql.TypeMapping;
using NpgsqlTypes;

namespace Npgsql.TypeHandlers.NumericHandlers
{
    /// <summary>
    /// A type handler for the PostgreSQL tinyint extension data type.
    /// </summary>
    /// <remarks>
    /// See http://www.postgresql.org/docs/current/static/datatype-numeric.html.
    ///
    /// The type handler API allows customizing Npgsql's behavior in powerful ways. However, although it is public, it
    /// should be considered somewhat unstable, and  may change in breaking ways, including in non-major releases.
    /// Use it at your own risk.
    /// </remarks>
    [TypeMapping("tinyint", NpgsqlDbType.Tinyint, new[] { DbType.Byte}, new[] { typeof(byte), }, DbType.Byte)]
    public class TinyintHandler : NpgsqlSimpleTypeHandler<byte>
    {
        /// <inheritdoc />
        public TinyintHandler(PostgresType postgresType) : base(postgresType) {}

        #region Read

        /// <inheritdoc />
        public override byte Read(NpgsqlReadBuffer buf, int len, FieldDescription? fieldDescription = null) => buf.ReadByte();

        #endregion

        #region Write

        /// <inheritdoc />
        public override int ValidateAndGetLength(byte value, NpgsqlParameter? parameter) => 1;

        /// <inheritdoc />
        public override void Write(byte value, NpgsqlWriteBuffer buf, NpgsqlParameter? parameter) => buf.WriteByte(value);

        #endregion
    }
}
