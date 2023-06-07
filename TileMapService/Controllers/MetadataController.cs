using Microsoft.AspNetCore.Mvc;
using TileMapService.Repositorys;

namespace TileMapService.Controllers
{
    [Route("metadata")]
    public class MetadataController : Controller
    {
        private readonly MBTilesTileSource source;

        public MetadataController(MBTilesTileSource source)
        {
            this.source = source;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(source.ReadMetadata());
        }
    }
}
