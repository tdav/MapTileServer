﻿namespace TileMapService.Models
{
    public class MetadataItem
    {
        /// <summary>
        /// Name (key) of item.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value of item.
        /// </summary>
        public string Value { get; set; }

        public MetadataItem()
        {

        }

        public MetadataItem(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"\"{this.Name}\": \"{this.Value}\"";
        }

        #region Names of standard items.

        /// <summary>
        /// The human-readable name of the tileset.
        /// </summary>
        public const string KeyName = "name";

        /// <summary>
        /// The file format of the tile data: pbf, jpg, png, webp, or an IETF media type for other formats.
        /// </summary>
        public const string KeyFormat = "format";

        /// <summary>
        /// The maximum extent of the rendered map area (as WGS 84 latitude and longitude values, in the OpenLayers Bounds format: left, bottom, right, top), string of comma-separated numbers.
        /// </summary>
        public const string KeyBounds = "bounds";

        /// <summary>
        /// The longitude, latitude, and zoom level of the default view of the map, string of comma-separated numbers.
        /// </summary>
        public const string KeyCenter = "center";

        /// <summary>
        /// The lowest zoom level (number) for which the tileset provides data.
        /// </summary>
        public const string KeyMinZoom = "minzoom";

        /// <summary>
        /// The highest zoom level (number) for which the tileset provides data.
        /// </summary>
        public const string KeyMaxZoom = "maxzoom";

        /// <summary>
        /// An attribution (HTML) string, which explains the sources of data and/or style for the map.
        /// </summary>
        public const string KeyAttribution = "attribution";

        /// <summary>
        /// A description of the tileset content.
        /// </summary>
        public const string KeyDescription = "description";

        /// <summary>
        /// Type of tileset: "overlay" or "baselayer".
        /// </summary>
        public const string KeyType = "type";

        /// <summary>
        /// The version (revision) of the tileset.
        /// </summary>
        public const string KeyVersion = "version";

        /// <summary>
        /// Lists the layers that appear in the vector tiles and the names and types of the attributes of features that appear in those layers in case of pbf format.
        /// </summary>
        public const string KeyJson = "json";

        #endregion
    }
}
