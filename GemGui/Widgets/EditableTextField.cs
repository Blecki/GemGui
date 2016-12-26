using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace GemGui.Widgets
{
    public class EditableTextField : Widget
    {
        //Todo: Static method to get best size.

        private int CursorPosition = 0;

        internal override void OnConstruct()
        {
            Properties.Border = "thin";

            // Note: Cursor won't draw properly if these are changed. Click events may also break.
            // Widget should probably be able to handle different alignments.
            Properties.TextVerticalAlign = WidgetProperties.VerticalAlign.Center;
            Properties.TextHorizontalAlign = WidgetProperties.HorizontalAlign.Left;

            Properties.OnClick += (args) =>
                {
                    if (Object.ReferenceEquals(this, Root.FocusItem))
                    {
                        // This widget already has focus - move cursor to click position.
                        // Todo: Configurable font property
                        var clickedChar = (float)(args.X - this.GetDrawableInterior().X) / (float)(Root.TileSheets["Font"].TileWidth * Properties.TextSize);
                        if (clickedChar > Properties.Text.Length)
                            CursorPosition = Properties.Text.Length;
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
                        CursorPosition = Properties.Text.Length;
                        Invalidate();
                    }
                };

            Properties.OnGainFocus += () => this.Invalidate();
            Properties.OnLoseFocus += () => this.Invalidate();

            Properties.OnKeyPress += (args) =>
                {
                    // Actual logic of modifying the string is outsourced.
                    Properties.Text = TextFieldLogic.Process(Properties.Text, CursorPosition, args.KeyValue, out CursorPosition);
                    Invalidate();
                };

            Properties.OnKeyDown += (args) =>
                {
                    Properties.Text = TextFieldLogic.HandleSpecialKeys(Properties.Text, CursorPosition, args.KeyValue, out CursorPosition);
                    Invalidate();
                };
        }

        protected override Mesh Redraw()
        {
            var mesh = base.Redraw();

            if (Object.ReferenceEquals(this, Root.FocusItem))
            {
                var font = Root.TileSheets["Font"]; //Todo: Configurable font property
                var drawableArea = this.GetDrawableInterior();
                var cursorMesh = Mesh.CreateSpriteQuad();
                var fontSize = Properties.TextSize;

                cursorMesh.Transform(Matrix.CreateScale(2.0f * fontSize, font.TileHeight * 1.2f * fontSize, 1.0f));
                // Center cursor in vertical space.
                cursorMesh.Transform(Matrix.CreateTranslation(
                    drawableArea.X + (CursorPosition * font.TileWidth * fontSize) - (fontSize / 2),
                    drawableArea.Y + ((drawableArea.Height - font.TileHeight * 1.2f * fontSize) / 2),
                    0.0f));
                
                // Todo: Find or make an actual cursor sprite.
                cursorMesh.TransformTexture(Root.TileSheets["Basic"].TileMatrix(0));
                cursorMesh.Colorize(new Vector4(1, 0, 0, 1));
                mesh = Mesh.Merge(mesh, cursorMesh);                
            }

            return mesh;
        }
    }
}
