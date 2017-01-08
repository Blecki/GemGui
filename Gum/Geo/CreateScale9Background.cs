using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gum
{
    public partial class Mesh
    {
        /// <summary>
        /// Create a mesh for a scale9 background. This assumed the tilesheet is 3*3 and positions the 
        /// corners without scalling, scales the edges on one axis only, and fills the middle with the 
        /// center tile.
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="Tiles"></param>
        /// <returns></returns>
        public static Mesh CreateScale9Background(Rectangle Rect, TileSheet Tiles)
        {
            var result = new List<Mesh>();

            //Top-left corner
            result.Add(Quad()
                .TileScaleAndTexture(Tiles, 0)
                .Translate(Rect.X, Rect.Y));

            //Top-right corner
            result.Add(Quad()
                .TileScaleAndTexture(Tiles, 2)
                .Translate(Rect.Right - Tiles.TileWidth, Rect.Y));

            //Bottom-left corner
            result.Add(Quad()
                .TileScaleAndTexture(Tiles, 6)
                .Translate(Rect.X, Rect.Bottom - Tiles.TileHeight));

            //Bottom-right corner
            result.Add(Quad()
                .TileScaleAndTexture(Tiles, 8)
                .Translate(Rect.Right - Tiles.TileWidth, Rect.Bottom - Tiles.TileHeight));

            //Top edge
            result.Add(Quad()
                .Texture(Tiles.TileMatrix(1))
                .Scale(Rect.Width - (2 * Tiles.TileWidth), Tiles.TileHeight)
                .Translate(Rect.X + Tiles.TileWidth, Rect.Y));

            //Bottom edge
            result.Add(Quad()
                .Texture(Tiles.TileMatrix(7))
                .Scale(Rect.Width - (2 * Tiles.TileWidth), Tiles.TileHeight)
                .Translate(Rect.X + Tiles.TileWidth, Rect.Bottom - Tiles.TileHeight));

            //Left edge
            result.Add(Quad()
                .Texture(Tiles.TileMatrix(3))
                .Scale(Tiles.TileWidth, Rect.Height - (2 * Tiles.TileHeight))
                .Translate(Rect.X, Rect.Y + Tiles.TileHeight));

            //Right edge
            result.Add(Quad()
                .Texture(Tiles.TileMatrix(5))
                .Scale(Tiles.TileWidth, Rect.Height - (2 * Tiles.TileHeight))
                .Translate(Rect.Right - Tiles.TileWidth, Rect.Y + Tiles.TileHeight));

            //Center
            result.Add(Quad()
                .Texture(Tiles.TileMatrix(4))
                .Scale(Rect.Width - (2 * Tiles.TileWidth), Rect.Height - (2 * Tiles.TileHeight))
                .Translate(Rect.X + Tiles.TileWidth, Rect.Y + Tiles.TileHeight));

            return Merge(result.ToArray());
        }
    }
}