using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    public class HorizontalSlider : Widget
    {
        private int _scrollArea;
        public int ScrollArea
        {
            get { return _scrollArea; }
            set { _scrollArea = value; AfterScroll(); }
        }

        private int _scrollPosition;
        public int ScrollPosition
        {
            get { return _scrollPosition; }
            set { _scrollPosition = value; AfterScroll(); }
        }

        public float ScrollPercentage
        {
            get { return _scrollArea == 0 ? 0.0f : ((float)_scrollPosition / (float)_scrollArea); }
            set { _scrollPosition = (int)(_scrollArea * value); AfterScroll(); }
        }

        public Action<Widget> OnScroll = null;

        private void AfterScroll()
        {
            if (_scrollPosition < 0) _scrollPosition = 0;
            if (_scrollPosition >= _scrollArea) _scrollPosition = _scrollArea - 1;

            Invalidate();

            // Could be called during construction - before Root is set.
            if (Root != null) Root.SafeCall(OnScroll, this);
        }

        public override void Construct()
        {
            if (String.IsNullOrEmpty(Graphics)) Graphics = "horizontal-slider";

            OnClick += (sender, args) =>
                {
                    var gfx = Root.GetTileSheet(Graphics);
                    var scrollSize = Rect.Width;
                    var clickX = args.X - Rect.X;
                    ScrollPercentage = (float)clickX / (float)scrollSize;
                };
        }

        protected override Mesh Redraw()
        {
            var tiles = Root.GetTileSheet(Graphics);

            var background = Mesh.Quad()
                .Scale(Rect.Width, Rect.Height)
                .Translate(Rect.X, Rect.Y)
                .Texture(tiles.TileMatrix(0));

            var scrollSize = Rect.Width;
            var barPosition = ScrollPercentage * scrollSize;
            var pixelPosition = Rect.X + (int)barPosition;

            var bar = Mesh.Quad()
                .Scale(tiles.TileWidth, tiles.TileHeight)
                .Translate(pixelPosition - (tiles.TileWidth / 2), Rect.Y)
                .Texture(tiles.TileMatrix(1));

            return Mesh.Merge(background, bar);
        }

        public override Point GetBestSize()
        {
            var gfx = Root.GetTileSheet(Graphics);
            return new Point(gfx.TileWidth * 5, gfx.TileHeight);
        }
    }
}
