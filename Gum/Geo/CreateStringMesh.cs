﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Gum
{
    public partial class Mesh
    {
        public static Mesh CreateStringMesh(
            String String, 
            ITileSheet FontSheet, 
            Vector2 GlyphScale,
            out Rectangle Bounds)
        {
            var glyphMeshes = new List<Mesh>();
            var pos = Vector2.Zero;

            foreach (var c in String)
            {
                if (c == '\n')
                {
                    pos.X = 0;
                    pos.Y += FontSheet.TileHeight * GlyphScale.Y;
                }
                else if (c < 32) continue;
                else
                {
                    var glyphSize = FontSheet.GlyphSize(c - ' ');
                    glyphMeshes.Add(Mesh.Quad()
                        .Texture(FontSheet.TileMatrix(c - ' '))
                        .Scale(glyphSize.X * GlyphScale.X, glyphSize.Y * GlyphScale.Y)
                        .Translate(pos.X, pos.Y));
                    pos.X += glyphSize.X * GlyphScale.X;
                }
            }

            Bounds = new Rectangle(0, 0, (int)pos.X, (int)(FontSheet.TileHeight * GlyphScale.Y));

            return Merge(glyphMeshes.ToArray());
        }

    }
}