using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 *  Encodes disc format of JSON skin file.
 */

namespace Gum
{
    public class JsonTileSheet
    {
        public String Name;
        public String Texture;
        public int TileWidth;
        public int TileHeight;
    }
    
    public class JsonTileSheetSet
    {
        public List<JsonTileSheet> Sheets;
    }
}
