using System.Collections.Concurrent;
using TileMapService.Models;

namespace TileMapService.Repositorys
{
    public class MBTilesTileSource : ITileSource
    {
        private readonly TileSetConfiguration configuration;
        public readonly string ContentType;

        /// <summary>
        /// [Tile coordinates; row id]
        /// </summary>
        private ConcurrentDictionary<long, long> tileKeys = new ConcurrentDictionary<long, long>();

        private bool isTileKeysReady = false;

        public MBTilesTileSource(IConfiguration conf)
        {
            this.configuration = new TileSetConfiguration()
            {
                Format = conf["Tile:Format"],
                Source = conf["Tile:Source"],
                Tms = Convert.ToBoolean(conf["Tile:Tms"]),
                UseCoordinatesCache = Convert.ToBoolean(conf["Tile:UseCoordinatesCache"]),
            };

            ContentType = Utils.GetContentType(this.configuration.Format); // TODO: from db metadata

            if (this.configuration.UseCoordinatesCache)
            {
                var filePath = GetLocalFilePath(this.configuration.Source);
                if (File.Exists(filePath))
                {
                    // TODO: not the best placement in constructor
                    Task.Run(() =>
                    {
                        var connectionString = GetMBTilesConnectionString(this.configuration.Source);
                        var db = new MBTilesRepository(connectionString);
                        db.ReadTileCoordinatesAsync(tileKeys).Wait(); // TODO: check presence of rowid
                        isTileKeysReady = true;
                    });
                }
            }
        }

        public async Task<byte[]> GetTileAsync(int x, int y, int z)
        {
            if (!configuration.Tms)
            {
                y = Utils.FromTmsY(y, z);
            }

            var connectionString = GetMBTilesConnectionString(configuration.Source);
            var db = new MBTilesRepository(connectionString);

            // TODO: if database contents were changed, coordinates cache should be invalidated

            if (configuration.UseCoordinatesCache)
            {
                var key = MBTilesRepository.CreateTileCoordinatesKey(z, x, y);
                if (tileKeys.ContainsKey(key))
                {
                    // Get rowid from cache, read table record by rowid (very fast, compared to selecting by three columns)
                    return await db.ReadTileAsync(tileKeys[key]);
                }
                else
                {
                    if (isTileKeysReady)
                    {
                        // Assuming there is no tile in database, if it not exists in cache
                        return null;
                    }
                    else
                    {
                        // While cache is not ready, allow simple database read
                        return await db.ReadTileAsync(x, y, z);
                    }
                }
            }
            else
            {
                return await db.ReadTileAsync(x, y, z);
            }
        }

        private static string GetLocalFilePath(string source)
        {
            var uriString = source.Replace(Utils.MBTilesScheme, Utils.LocalFileScheme, StringComparison.Ordinal);
            var uri = new Uri(uriString);

            return uri.LocalPath;
        }

        private static string GetMBTilesConnectionString(string source)
        {
            // https://github.com/aspnet/Microsoft.Data.Sqlite/wiki/Connection-Strings

            return $"Data Source={GetLocalFilePath(source)}; Mode=ReadOnly;";
        }

        public MetadataItem[] ReadMetadata()
        {
            var connectionString = GetMBTilesConnectionString(configuration.Source);
            var db = new MBTilesRepository(connectionString);
            return db.ReadMetadata();
        }
    }
}
