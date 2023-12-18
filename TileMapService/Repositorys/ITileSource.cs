using TileMapService.Models;

namespace TileMapService.Repositorys
{
    public interface ITileSource
    {
        Task<byte[]> GetTileAsync(int x, int y, int z);
        MetadataItem[] ReadMetadata();
    }
}
