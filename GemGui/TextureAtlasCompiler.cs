using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GemGui.TextureAtlas
{
    public class Entry
    {
        public JsonTileSheet Sheet;
        public Rectangle Rect;
    }

    public class Atlas
    {
        public Rectangle Dimensions;
        public List<Entry> Textures;
    }

    internal static class BspSubdivision
    {
        internal static void PlaceTextures(Rectangle dim, List<Entry> Textures)
        {
            Entry tex = null;

            //Find largest entry that fits within the dimensions.
            for (int i = 0; i < Textures.Count; ++i)
            {
                var Entry = Textures[i];
                if (Entry.Rect.Width > dim.Width || Entry.Rect.Height > dim.Height)
                    continue;
                Textures.RemoveAt(i);
                tex = Entry;
                break;
            }

            if (tex == null) return;

            tex.Rect.X = dim.X;
            tex.Rect.Y = dim.Y;

            //Subdivide remaining space.
            int HorizontalDifference = dim.Width - tex.Rect.Width;
            int VerticalDifference = dim.Height - tex.Rect.Height;

            if (HorizontalDifference == 0 && VerticalDifference == 0) //Perfect fit!
                return;

            Rectangle? ASpace = null;
            Rectangle? BSpace = null;            

            if (HorizontalDifference >= VerticalDifference)
            {
                ASpace = new Rectangle(dim.X + tex.Rect.Width, dim.Y, HorizontalDifference, dim.Height);
                if (VerticalDifference > 0)
                    BSpace = new Rectangle(dim.X, dim.Y + tex.Rect.Height, tex.Rect.Width, VerticalDifference);
            }
            else
            {
                ASpace = new Rectangle(dim.X, dim.Y + tex.Rect.Height, dim.Width, VerticalDifference);
                if (HorizontalDifference > 0)
                    BSpace = new Rectangle(dim.X + tex.Rect.Width, dim.Y, HorizontalDifference, tex.Rect.Height);
            }

            if (ASpace.HasValue) PlaceTextures(ASpace.Value, Textures);
            if (BSpace.HasValue) PlaceTextures(BSpace.Value, Textures);
        }

        internal static Rectangle ExpandingSpaceHorizontal(Rectangle totalArea, Rectangle workingArea, List<Entry> Textures)
        {
            PlaceTextures(workingArea, Textures);
            if (Textures.Count > 0)
            {
                workingArea = new Rectangle(0, totalArea.Height, totalArea.Width, totalArea.Height);
                totalArea.Height *= 2;
                return ExpandingSpaceVertical(totalArea, workingArea, Textures);
            }
            else
                return totalArea;
        }

        internal static Rectangle ExpandingSpaceVertical(Rectangle totalArea, Rectangle workingArea, List<Entry> Textures)
        {
            PlaceTextures(workingArea, Textures);
            if (Textures.Count > 0)
            {
                workingArea = new Rectangle(totalArea.Width, 0, totalArea.Width, totalArea.Height);
                totalArea.Width *= 2;
                return ExpandingSpaceHorizontal(totalArea, workingArea, Textures);
            }
            else
                return totalArea;
        }
    }

    public class Compiler
    {
        public static Atlas Compile(List<Entry> Entries)
        {
            Entries.Sort((A, B) =>
            {
                return (B.Rect.Width * B.Rect.Height) - (A.Rect.Width * A.Rect.Height);
            });

            var LargestEntry = Entries[0];
            var texSize = new Rectangle(0, 0, 1, 1);
            while (texSize.Width < LargestEntry.Rect.Width)
                texSize.Width *= 2;
            while (texSize.Height < LargestEntry.Rect.Height)
                texSize.Height *= 2;

            var PendingEntries = new List<Entry>(Entries);
            texSize = BspSubdivision.ExpandingSpaceHorizontal(texSize, texSize, PendingEntries);
            return new Atlas { Dimensions = texSize, Textures = Entries };
        }
    }
}
