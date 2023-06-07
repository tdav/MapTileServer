using Microsoft.Data.Sqlite;
using System.Collections.Concurrent;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using TileMapService.Models;

namespace TileMapService.Repositorys
{
    /// <summary>
    /// Data access layer for MBTiles 1.2 (SQLite) database
    /// </summary>
    /// <remarks>
    /// See https://github.com/mapbox/mbtiles-spec/blob/master/1.2/spec.md
    /// </remarks>
    public class MBTilesRepository
    {
        // Using pure ADO.NET instead of ORM for performance reason

        /// <summary>
        /// MBTiles tileset magic number
        /// </summary>
        /// <remarks>
        /// See https://www.sqlite.org/src/artifact?ci=trunk&filename=magic.txt
        /// </remarks>
        private const int ApplicationId = 0x4d504258;

        /// <summary>
        /// Connection string for SQLite database
        /// </summary>
        private readonly string connectionString;

        #region Database objects names

        private const string TableTiles = "tiles";

        private const string ColumnTileColumn = "tile_column";

        private const string ColumnTileRow = "tile_row";

        private const string ColumnZoomLevel = "zoom_level";

        private const string ColumnTileData = "tile_data";

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString">Connection string for SQLite database</param>
        public MBTilesRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<byte[]> ReadTileAsync(int tileColumn, int tileRow, int zoomLevel)
        {
            var commandText = $"SELECT tile_data FROM tiles where zoom_level = {zoomLevel} and tile_column = {tileColumn} and tile_row = {tileRow}";
            byte[] result = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
                    await connection.OpenAsync();
                    using (var dr = await command.ExecuteReaderAsync())
                    {
                        if (await dr.ReadAsync())
                        {
                            return (byte[])dr[0];
                        }

                        dr.Close();
                    }
                }

                connection.Close();
            }

            return result;
        }

        public async Task<byte[]> ReadTileAsync(long rowId)
        {
            var commandText = $"SELECT {ColumnTileData} FROM {TableTiles} WHERE (rowid = @rowId)";
            byte[] result = null;
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
                    command.Parameters.Add(new SqliteParameter("@rowId", rowId));

                    await connection.OpenAsync().ConfigureAwait(false);
                    using (var dr = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        if (await dr.ReadAsync().ConfigureAwait(false))
                        {
                            result = (byte[])dr[0];
                        }

                        dr.Close();
                    }
                }

                connection.Close();
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long CreateTileCoordinatesKey(long zoomLevel, long tileColumn, long tileRow)
        {
            // Z[8] | X[24] | Y[24]
            return zoomLevel << 48 | (tileColumn & 0xFFFFFF) << 24 | tileRow & 0xFFFFFF;
        }

        public async Task ReadTileCoordinatesAsync(ConcurrentDictionary<long, long> tileKeys)
        {
            var commandText = $"SELECT rowid, {ColumnZoomLevel}, {ColumnTileColumn}, {ColumnTileRow} FROM {TableTiles}";
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (var dr = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await dr.ReadAsync().ConfigureAwait(false))
                        {
                            tileKeys.TryAdd(CreateTileCoordinatesKey(dr.GetInt64(1), dr.GetInt64(2), dr.GetInt64(3)), dr.GetInt64(0));
                        }

                        dr.Close();
                    }
                }

                connection.Close();
            }
        }

        public async Task<IList<long>> ReadTileCoordinatesKeysAsync()
        {
            var result = new List<long>();

            var commandText = $"SELECT {ColumnZoomLevel}, {ColumnTileColumn}, {ColumnTileRow} FROM {TableTiles}";
            using (var connection = new SqliteConnection(connectionString))
            {
                using (var command = new SqliteCommand(commandText, connection))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    using (var dr = await command.ExecuteReaderAsync().ConfigureAwait(false))
                    {
                        while (await dr.ReadAsync().ConfigureAwait(false))
                        {
                            result.Add(CreateTileCoordinatesKey((int)dr.GetInt64(0), (int)dr.GetInt64(1), (int)dr.GetInt64(2)));
                        }

                        dr.Close();
                    }
                }

                connection.Close();
            }

            return result;
        }


        public MetadataItem[] ReadMetadata()
        {
            using (var connection = new SqliteConnection(this.connectionString))
            {
                var ReadMetadataCommandText = $"SELECT name, value FROM metadata";
                using (var command = new SqliteCommand(ReadMetadataCommandText, connection))
                {
                    var result = new List<MetadataItem>();

                    connection.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            result.Add(new MetadataItem
                            {
                                Name = dr.IsDBNull(0) ? null : dr.GetString(0),
                                Value = dr.IsDBNull(1) ? null : dr.GetString(1),
                            });
                        }

                        dr.Close();
                    }

                    return result.ToArray();
                }
            }
        }
    }
}
