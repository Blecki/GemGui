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
        public static Mesh CreateStringMesh(
            String String, 
            TileSheet FontSheet, 
            Vector2 GlyphSize,
            out Rectangle Bounds)
        {
            var glyphMeshes = new List<Mesh>();
            var pos = Vector2.Zero;

            foreach (var c in String)
            {
                glyphMeshes.Add(Mesh.Quad()
                    .Texture(FontSheet.TileMatrix(c - ' '))
                    .Scale(GlyphSize.X, GlyphSize.Y)
                    .Translate(pos.X, pos.Y));
                pos.X += GlyphSize.X;
            }

            Bounds = new Rectangle(0, 0, (int)pos.X, (int)(pos.Y + GlyphSize.Y));

            return Merge(glyphMeshes.ToArray());
        }

    }
}