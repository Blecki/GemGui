﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gum
{
    /// <summary>
    /// Calculates UV transformation matricies for tiles inside a tilesheet, that is itself a portion
    /// of a larger texture atlas.
    /// </summary>
    public class TileSheet : ITileSheet
    {
        public int TextureWidth { get; private set; }
        public int TextureHeight { get; private set; }
        public Rectangle SourceRect { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public bool RepeatWhenUsedAsBorder { get; private set; }


        public float SourceURange { get { return (float)SourceRect.Width / (float)TextureWidth; } }
        public float SourceVRange { get { return (float)SourceRect.Height / (float)TextureHeight; } }
        public float SourceUOffset { get { return (float)SourceRect.X / (float)TextureWidth; } }
        public float SourceVOffset { get { return (float)SourceRect.Y / (float)TextureHeight; } }
        public int Columns { get { return SourceRect.Width / TileWidth; } }
        public int Rows { get { return SourceRect.Height / TileHeight; } }
        public int Row(int TileIndex) { return TileIndex / Columns; }
        public int Column(int TileIndex) { return TileIndex % Columns; }
        public float TileUStep { get { return SourceURange / Columns; } }
        public float TileVStep { get { return SourceVRange / Rows; } }
        public float ColumnU(int Column) { return SourceUOffset + (TileUStep * Column); }
        public float RowV(int Row) { return SourceVOffset + (TileVStep * Row); }
        public float TileU(int TileIndex) { return ColumnU(Column(TileIndex)); }
        public float TileV(int TileIndex) { return RowV(Row(TileIndex)); }

        // Generate UV transform matricies that align the UV range 0..1 to a tile.
        public Matrix ScaleMatrix { get { return Matrix.CreateScale(TileUStep, TileVStep, 1.0f); } }
        public Matrix TranslationMatrix(int Column, int Row) { return Matrix.CreateTranslation(ColumnU(Column), RowV(Row), 0.0f); }
        public Matrix TileMatrix(int Column, int Row) { return ScaleMatrix * TranslationMatrix(Column, Row); }
        public Matrix TileMatrix(int TileIndex) { return TileMatrix(Column(TileIndex), Row(TileIndex)); }
        public Matrix TileMatrix(int TileIndex, int ColumnSpan, int RowSpan)
        {
            return Matrix.CreateScale(ColumnSpan, RowSpan, 1.0f) * TileMatrix(TileIndex);
        }

        public Matrix TileMatrix(int Column, int Row, int ColumnSpan, int RowSpan)
        {
            return Matrix.CreateScale(ColumnSpan, RowSpan, 1.0f) * TileMatrix(Column, Row);
        }

        public TileSheet(int TextureWidth, int TextureHeight, Rectangle Source, int TileWidth, int TileHeight, bool RepeatWhenUsedAsBorder)
        {
            this.TextureWidth = TextureWidth;
            this.TextureHeight = TextureHeight;
            this.TileWidth = TileWidth;
            this.TileHeight = TileHeight;
            this.SourceRect = Source;
            this.RepeatWhenUsedAsBorder = RepeatWhenUsedAsBorder;
        }

        public Point GlyphSize(int Index)
        {
            return new Point(TileWidth, TileHeight);
        }

        public Point MeasureString(String S)
        {
            return new Point(S.Length * TileWidth, TileHeight);
        }
    }
}
