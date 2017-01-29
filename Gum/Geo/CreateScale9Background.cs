using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gum
{
    public enum Scale9Corners
    {
        Top = 1,
        Right = 2,
        Bottom = 4,
        Left = 8,
        All = Top | Right | Bottom | Left
    }

    public partial class Mesh
    {
        public static Mesh FittedSprite(Rectangle Rect, ITileSheet Tiles, int Tile)
        {
            return Quad()
                .Scale(Rect.Width, Rect.Height)
                .Translate(Rect.X, Rect.Y)
                .Texture(Tiles.TileMatrix(Tile));
        }


        /// <summary>
        /// Create a mesh for a scale9 background. This assumed the tilesheet is 3*3 and positions the 
        /// corners without scalling, scales the edges on one axis only, and fills the middle with the 
        /// center tile.
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="Tiles"></param>
        /// <returns></returns>
        public static Mesh CreateScale9Background(
            Rectangle Rect, 
            ITileSheet Tiles, 
            Scale9Corners Corners = Scale9Corners.All)
        {
            var rects = new Rectangle[9];

            var margin = new Margin(0, 0, 0, 0);

            if (Corners.HasFlag(Scale9Corners.Left)) margin.Left = Tiles.TileWidth;
            if (Corners.HasFlag(Scale9Corners.Right)) margin.Right = Tiles.TileWidth;
            if (Corners.HasFlag(Scale9Corners.Top)) margin.Top = Tiles.TileHeight;
            if (Corners.HasFlag(Scale9Corners.Bottom)) margin.Bottom = Tiles.TileHeight;

            rects[0] = new Rectangle(Rect.Left, Rect.Top, margin.Left, margin.Top);
            rects[1] = new Rectangle(Rect.Left + margin.Left, Rect.Top, 
                Rect.Width - margin.Left - margin.Right, margin.Top);
            rects[2] = new Rectangle(Rect.Right - margin.Right, Rect.Top, margin.Right, margin.Top);
            rects[3] = new Rectangle(Rect.Left, Rect.Top + margin.Top, margin.Left, 
                Rect.Height - margin.Top - margin.Bottom);
            rects[4] = new Rectangle(Rect.Left + margin.Left, Rect.Top + margin.Top,
                Rect.Width - margin.Left - margin.Right, Rect.Height - margin.Top - margin.Bottom);
            rects[5] = new Rectangle(Rect.Right - margin.Right, Rect.Top + margin.Top, margin.Right,
                Rect.Height - margin.Top - margin.Bottom);
            rects[6] = new Rectangle(Rect.Left, Rect.Bottom - margin.Bottom, margin.Left, margin.Bottom);
            rects[7] = new Rectangle(Rect.Left + margin.Left, Rect.Bottom - margin.Bottom,
                Rect.Width - margin.Left - margin.Right, margin.Bottom);
            rects[8] = new Rectangle(Rect.Right - margin.Right, Rect.Bottom - margin.Bottom,
                margin.Right, margin.Bottom);

            var result = new List<Mesh>();

            for (var i = 0; i < 9; ++i)
            {
                if (rects[i].Width != 0 && rects[i].Height != 0)
                    result.Add(FittedSprite(rects[i], Tiles, i));
            }

             return Merge(result.ToArray());
        }
    }
}