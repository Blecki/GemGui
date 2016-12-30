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
        public static Mesh CreateScale9Background(Rectangle Rect, TileSheet Tiles)
        {
            var result = new List<Mesh>();

            //Top-left corner
            result.Add(CreateSpriteQuad()
                .Tile(Tiles, 0)
                .Translate(Rect.X, Rect.Y));

            //Top-right corner
            result.Add(CreateSpriteQuad()
                .Tile(Tiles, 2)
                .Translate(Rect.X + Rect.Width - Tiles.TileWidth, Rect.Y));

            // Todo: Cleanup as above.
            //Bottom-left corner
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Tiles.TileWidth, Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(6))
                .Transform(Matrix.CreateTranslation(Rect.X, Rect.Y + Rect.Height - Tiles.TileHeight, 0.0f)));

            //Bottom-right corner
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Tiles.TileWidth, Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(8))
                .Transform(Matrix.CreateTranslation(Rect.X + Rect.Width - Tiles.TileWidth, Rect.Y + Rect.Height - Tiles.TileHeight, 0.0f)));

            //Top edge
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Rect.Width - Tiles.TileWidth - Tiles.TileWidth, Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(1))
                .Transform(Matrix.CreateTranslation(Rect.X + Tiles.TileWidth, Rect.Y, 0.0f)));

            //Bottom edge
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Rect.Width - Tiles.TileWidth - Tiles.TileWidth, Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(7))
                .Transform(Matrix.CreateTranslation(Rect.X + Tiles.TileWidth, Rect.Y + Rect.Height - Tiles.TileHeight, 0.0f)));

            //Left edge
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Tiles.TileWidth, Rect.Height - Tiles.TileHeight - Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(3))
                .Transform(Matrix.CreateTranslation(Rect.X, Rect.Y + Tiles.TileHeight, 0.0f)));

            //Right edge
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Tiles.TileWidth, Rect.Height - Tiles.TileHeight - Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(5))
                .Transform(Matrix.CreateTranslation(Rect.X + Rect.Width - Tiles.TileWidth, Rect.Y + Tiles.TileHeight, 0.0f)));

            //Center
            result.Add(CreateSpriteQuad()
                .Transform(Matrix.CreateScale(Rect.Width - Tiles.TileWidth - Tiles.TileWidth, Rect.Height - Tiles.TileHeight - Tiles.TileHeight, 1.0f))
                .Texture(Tiles.TileMatrix(4))
                .Transform(Matrix.CreateTranslation(Rect.X + Tiles.TileWidth, Rect.Y + Tiles.TileHeight, 0.0f)));

            return Merge(result.ToArray());
        }
    }
}