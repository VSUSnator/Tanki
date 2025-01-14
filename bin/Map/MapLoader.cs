using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tanki.Map
{
    public class MapLoader
    {
        private readonly List<string> mapFiles;
        private int currentMapIndex;

        public MapLoader(List<string> mapFiles)
        {
            this.mapFiles = mapFiles;
            currentMapIndex = 0;
        }

        public string GetNextMap()
        {
            if (currentMapIndex < mapFiles.Count)
            {
                return mapFiles[currentMapIndex++];
            }
            return null; // Если карты закончились
        }
    }
}
