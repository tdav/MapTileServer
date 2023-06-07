# Fork from
	https://github.com/apdevelop/tile-map-service-net5.git 


# Download from OSM Map PBF
	https://download.geofabrik.de/asia/uzbekistan.html

# Convert to pbf mbtiles
	https://github.com/systemed/tilemaker

	bin from https://github.com/systemed/tilemaker/releases

# CLI	 
	tilemaker --input uzbekistan-latest.osm.pbf  --output uzbekistan-latest.mbtiles --config config-openmaptiles.json  --process process-openmaptiles.lua

