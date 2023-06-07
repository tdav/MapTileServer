namespace TileMapService.Models
{
    public class TileSetConfiguration
    {
        public string Format { get; set; }

        public string Source { get; set; }
        public bool Tms { get; set; }
        public bool UseCoordinatesCache { get; set; }
    }
}
