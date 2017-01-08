﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    public class TextGrid : Widget
    {
        private Mesh GridMesh;
        public int TextWidth { get; private set; }
        public int TextHeight { get; private set; }

        public override void Construct()
        {
            OnLayout += _OnLayout;
        }

        private void _OnLayout(Widget Sender)
        {
            var interior = GetDrawableInterior();
            var font = Root.GetTileSheet(Font);

            var gridW = (int)(interior.Width / (font.TileWidth * PixelPerfectTextSize));
            var gridH = (int)(interior.Height / (font.TileHeight * PixelPerfectTextSize));

            var realX = interior.X + (interior.Width / 2) - ((font.TileWidth * PixelPerfectTextSize) / 2);
            var realY = interior.Y + (interior.Height / 2) - ((font.TileHeight * PixelPerfectTextSize) / 2);

            var quads = new List<Mesh>();
            for (var x = 0; x < gridW; ++x)
                for (var y = 0; y < gridH; ++y)
                    quads.Add(Mesh.Quad()
                        .Scale(font.TileWidth * PixelPerfectTextSize, font.TileHeight * PixelPerfectTextSize)
                        .Translate((x * font.TileWidth * PixelPerfectTextSize) + realX,
                            (y * font.TileHeight * PixelPerfectTextSize) + realY)
                        .Texture(font.TileMatrix(0)));

            GridMesh = Mesh.Merge(quads.ToArray());
            TextWidth = gridW;
            TextHeight = gridH;
        }

        public void SetCharacter(int Index, char C)
        {
            GridMesh.verticies[(Index * 4) + 0].TextureCoordinate = new Vector2(0.0f, 0.0f);
            GridMesh.verticies[(Index * 4) + 1].TextureCoordinate = new Vector2(1.0f, 0.0f);
            GridMesh.verticies[(Index * 4) + 2].TextureCoordinate = new Vector2(1.0f, 1.0f);
            GridMesh.verticies[(Index * 4) + 3].TextureCoordinate = new Vector2(0.0f, 1.0f);

            var font = Root.GetTileSheet(Font);
            GridMesh.Texture(font.TileMatrix(C - ' '), Index * 4, 4);
        }

        public void SetString(String S)
        {
            for (int i = 0; i < S.Length; ++i)
                SetCharacter(i, S[i]);
        }

        protected override Mesh Redraw()
        {
            return Mesh.Merge(base.Redraw(), GridMesh);
        }
    }
}
