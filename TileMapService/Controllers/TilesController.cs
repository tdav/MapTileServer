using Microsoft.AspNetCore.Mvc;
using TileMapService.Repositorys;

namespace TileMapService.Controllers
{
    [Route("api/tiles")]
    public class TilesController : Controller
    {
        private readonly MBTilesTileSource source;

        public TilesController(MBTilesTileSource source)
        {
            this.source = source;
        }

        /// <summary>
        /// Get tile from tileset with specified coordinates 
        /// URL in Google Maps format, like http://localhost/MapTileService/?tileset=world&x=4&y=5&z=3
        /// </summary>
        /// <param name="tileset">Tileset name</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z">Zoom level</param>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetTile1Async(string tileset, int x, int y, int z)
        {
            return await ReadTileAsync(tileset, x, y, z);
        }

        [HttpGet("{z}/{x}/{y}.pbf")]
        public async Task<IActionResult> GetTile2Async(string tileset, int x, int y, int z)
        {
            return await ReadTileAsync(tileset, x, y, z);
        }

        private async Task<IActionResult> ReadTileAsync(string tileset, int x, int y, int z)
        {            
            var data = await source.GetTileAsync(x, Utils.FromTmsY(y, z), z);
            if (data != null)
            {
                return File(data, source.ContentType);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
