using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Gum.Widgets
{
    public class EditableTextField : Widget
    {
        private int CursorPosition = 0;

        public override void Construct()
        {
            if (String.IsNullOrEmpty(Border)) Border = "border-thin";

            // Note: Cursor won't draw properly if these are changed. Click events may also break.
            // Widget should probably be able to handle different alignments.
            TextVerticalAlign = VerticalAlign.Center;
            TextHorizontalAlign = HorizontalAlign.Left;

            OnClick += (args) =>
                {
                    if (Object.ReferenceEquals(this, Root.FocusItem))
                    {
                        // This widget already has focus - move cursor to click position.
                        var clickedChar = (float)(args.X - this.GetDrawableInterior().X) / (float)(Root.GetTileSheet(Font).TileWidth * TextSize);
                        if (clickedChar > Text.Length)
                            CursorPosition = Text.Length;
                        else if (clickedChar <= 0)
                            CursorPosition = 0;
                        else
                            CursorPosition = (int)Math.Round(clickedChar);
                        Invalidate();
                    }
                    else
                    {
                        // Take focus and move cursor to end of text.
                        Root.SetFocus(this);
                        CursorPosition = Text.Length;
                        Invalidate();
                    }
                };

            OnGainFocus += () => this.Invalidate();
            OnLoseFocus += () => this.Invalidate();
            OnUpdateWhileFocus += () => this.Invalidate();

            OnKeyPress += (args) =>
                {
                    // Actual logic of modifying the string is outsourced.
                    Text = TextFieldLogic.Process(Text, CursorPosition, args.KeyValue, out CursorPosition);
                    Invalidate();
                };

            OnKeyDown += (args) =>
                {
                    Text = TextFieldLogic.HandleSpecialKeys(Text, CursorPosition, args.KeyValue, out CursorPosition);
                    Invalidate();
                };
        }

        protected override Mesh Redraw()
        {
            if (Object.ReferenceEquals(this, Root.FocusItem))
            {
                var cursorTime = (int)(Math.Floor(Root.RunTime / Root.CursorBlinkTime));
                if ((cursorTime & 1) == 1)
                {
                    var font = Root.GetTileSheet(Font);
                    var drawableArea = this.GetDrawableInterior();
                    var cursorMesh = Mesh.CreateSpriteQuad()
                        .Scale(font.TileWidth * TextSize, font.TileHeight * TextSize)
                        .Translate(
                            drawableArea.X + (CursorPosition * font.TileWidth * TextSize) - ((font.TileWidth * TextSize) / 2),
                            drawableArea.Y + ((drawableArea.Height - (font.TileHeight * TextSize)) / 2))
                        .Texture(Root.GetTileSheet(Font).TileMatrix((int)('|' - ' ')))
                        .Colorize(new Vector4(1, 0, 0, 1));
                    return Mesh.Merge(base.Redraw(), cursorMesh);
                }
            }
            
            return base.Redraw();
        }
    }
}
