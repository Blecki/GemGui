using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Gum
{
    /// <summary>
    /// Root of a GUI.
    /// </summary>
    public class Root
    {
        public RenderData RenderData { get; private set; }

        public Widget HoverItem { get; private set; }
        public Widget FocusItem { get; private set; }

        public Rectangle VirtualScreen { get; private set; }
        public Rectangle RealScreen { get; private set; }
        public int ScaleRatio { get; private set; }
        public Widget RootItem { get; private set; }
        public Widget PopupItem { get; private set; }
        public Widget TooltipItem { get; private set; }

        public MousePointer MousePointer = null;
        private Point MousePosition = new Point(0, 0);
        private DateTime MouseMotionTime = DateTime.Now;
        public float SecondsBeforeTooltip = 1.0f;
        public String TooltipFont = null;
        public int TooltipTextSize = 1;
        public float CursorBlinkTime = 0.3f;
        internal double RunTime = 0.0f;

        public Root(Point IdealSize, RenderData RenderData)
        {
            this.RenderData = RenderData;
        
            ResizeVirtualScreen(IdealSize);
            ResetGui();
        }
               
        /// <summary>
        /// Reset the gui, clearing out all existing widgets.
        /// </summary>
        public void ResetGui()
        {
            HoverItem = null;
            FocusItem = null;
            PopupItem = null;
            TooltipItem = null;
            RootItem = ConstructWidget(new Widget
                {
                    Rect = VirtualScreen,
                    Transparent = true
                });

        }

        /// <summary>
        /// Resize the virtual screen and find the ideal size and positioning.
        /// </summary>
        /// <param name="VirtualSize"></param>
        public void ResizeVirtualScreen(Point VirtualSize)
        {
            this.VirtualScreen = new Rectangle(0, 0, VirtualSize.X, VirtualSize.Y);

            // Calculate ideal on screen size.
            // Size should never be smaller than the size of the virtual screen supplied.
            var screenSize = RenderData.Device.Viewport.Bounds;
            ScaleRatio = 1;

            // How many times can we multiply the ideal size and still fit on the screen?
            while (((VirtualSize.X * (ScaleRatio + 1)) <= screenSize.Width) &&
                ((VirtualSize.Y * (ScaleRatio + 1)) <= screenSize.Height))
                ScaleRatio += 1;

            // How much space did we leave to the left and right? 
            var horizontalExpansion = ((screenSize.Width - (VirtualSize.X * ScaleRatio)) / 2) / ScaleRatio;
            var verticalExpansion = ((screenSize.Height - (VirtualSize.Y * ScaleRatio)) / 2) / ScaleRatio;

            VirtualScreen = new Rectangle(0, 0, VirtualSize.X + horizontalExpansion + horizontalExpansion,
                VirtualSize.Y + verticalExpansion + verticalExpansion);

            RealScreen = new Rectangle(0, 0, VirtualScreen.Width * ScaleRatio, VirtualScreen.Height * ScaleRatio);
            RealScreen = new Rectangle((screenSize.Width - RealScreen.Width) / 2,
                (screenSize.Height - RealScreen.Height) / 2,
                RealScreen.Width, RealScreen.Height);
        }

        /// <summary>
        /// Get a named tile sheet.
        /// </summary>
        /// <param name="Name"></param>
        /// <returns></returns>
        public TileSheet GetTileSheet(String Name)
        {
            return RenderData.TileSheets[Name];
        }

        /// <summary>
        /// Widgets must be constructed or some operations will fail. Use this function to construct a widget 
        /// when the widget is not being immediately added to its parent.
        /// </summary>
        /// <param name="CreatedWidget"></param>
        /// <returns></returns>
        public Widget ConstructWidget(Widget NewWidget)
        {
            NewWidget._Construct(this);
            return NewWidget;
        }

        /// <summary>
        /// Prepare a widget for destruction - make sure any lingering references to it or its children
        /// are removed.
        /// </summary>
        /// <param name="Widget"></param>
        internal void CleanupWidget(Widget Widget)
        {
            foreach (var child in Widget.Children)
                CleanupWidget(child);
            Widget.Root = null;
            if (Object.ReferenceEquals(FocusItem, Widget)) FocusItem = null;
            if (Object.ReferenceEquals(HoverItem, Widget)) HoverItem = null;
            if (Object.ReferenceEquals(PopupItem, Widget)) PopupItem = null;
            if (Object.ReferenceEquals(TooltipItem, Widget)) TooltipItem = null;
        }

        public void DestroyWidget(Widget Widget)
        {
            CleanupWidget(Widget);
            if (Widget.Parent != null) Widget.Parent.RemoveChild(Widget);
        }            

        /// <summary>
        /// Shortcut function for showing a uiitem as a 'dialog'.
        /// </summary>
        /// <param name="Dialog"></param>
        public void ShowDialog(Widget Dialog)
        {
            RootItem.AddChild(Dialog);
        }
        
        /// <summary>
        /// Show a widget as a popup. Replaces any existing popup widget already displayed.
        /// </summary>
        /// <param name="Popup"></param>
        public void ShowPopup(Widget Popup)
        {
            if (PopupItem != null)
            {
                SafeCall(PopupItem.OnPopupClose, PopupItem);
                DestroyWidget(PopupItem);
            }

            PopupItem = Popup;
            RootItem.AddChild(PopupItem);
        }

        public void ShowTooltip(Point Where, String Tip)
        {
            if (TooltipItem != null)
                DestroyWidget(TooltipItem);

            TooltipItem = ConstructWidget(new Widget
                {
                    Text = Tip,
                    Border = "border-thin",
                    Font = TooltipFont,
                    TextSize = TooltipTextSize
                });
            var bestSize = TooltipItem.GetBestSize();
            TooltipItem.Rect = new Rectangle(
                Where.X + (MousePointer == null ? 0 : GetTileSheet(MousePointer.Sheet).TileWidth) + 2, 
                Where.Y, bestSize.X, bestSize.Y);

            RootItem.AddChild(TooltipItem);
        }

        /// <summary>
        /// Set keyboard focus to the specified widget. Fires lose and gain focus events as appropriate.
        /// </summary>
        /// <param name="On"></param>
        public void SetFocus(Widget On)
        {
            if (!Object.ReferenceEquals(this, On.Root)) throw new InvalidOperationException();
            if (Object.ReferenceEquals(FocusItem, On)) return;

            if (FocusItem != null) SafeCall(FocusItem.OnLoseFocus, FocusItem);
            FocusItem = On; 
            if (FocusItem != null) SafeCall(FocusItem.OnGainFocus, FocusItem);
        }

        /// <summary>
        /// Shortcut to call an action without having to check for null.
        /// </summary>
        /// <param name="Action"></param>
        internal void SafeCall(Action<Widget, InputEventArgs> Action, Widget Widget, InputEventArgs Args)
        {
            if (Action != null) Action(Widget, Args);
        }

        /// <summary>
        /// Shortcut to call an action without having to check for null.
        /// </summary>
        /// <param name="Action"></param>
        internal void SafeCall(Action<Widget> Action, Widget Widget)
        {
            if (Action != null) Action(Widget);
        }

        public Point ScreenPointToGuiPoint(Point P)
        {
            // Transform mouse from screen space to virtual gui space.
            float mouseX = P.X - RealScreen.X;
            float mouseY = P.Y - RealScreen.Y;
            mouseX /= ScaleRatio;
            mouseY /= ScaleRatio;
            return new Point((int)mouseX, (int)mouseY);
        }

        /// <summary>
        /// Process mouse events.
        /// </summary>
        /// <param name="State"></param>
        public void HandleInput(InputEvents Event, InputEventArgs Args)
        {
            switch (Event)
            {
                case InputEvents.MouseMove:
                    {
                        // Destroy tooltips when the mouse moves.
                        MouseMotionTime = DateTime.Now;
                        if (TooltipItem != null) DestroyWidget(TooltipItem);

                        MousePosition = ScreenPointToGuiPoint(new Point(Args.X, Args.Y));
                        var newArgs = new InputEventArgs { X = MousePosition.X, Y = MousePosition.Y };
                        // Detect hover item and fire mouse enter/leave events as appropriate.
                        var newHoverItem = RootItem.FindWidgetAt(MousePosition.X, MousePosition.Y);
                        if (!Object.ReferenceEquals(newHoverItem, HoverItem))
                        {
                            if (HoverItem != null) SafeCall(HoverItem.OnMouseLeave, HoverItem, newArgs);
                            if (newHoverItem != null) SafeCall(newHoverItem.OnMouseEnter, newHoverItem, newArgs);
                            HoverItem = newHoverItem;
                        }
                    }
                    break;
                case InputEvents.MouseClick:
                    {
                        MousePosition = ScreenPointToGuiPoint(new Point(Args.X, Args.Y));
                        var newArgs = new InputEventArgs { X = MousePosition.X, Y = MousePosition.Y };
                        
                        if (!Object.ReferenceEquals(HoverItem, PopupItem) && PopupItem != null) // Clicked off popup item.
                        {
                            // Could have clicked a child of the popup.
                            if (HoverItem == null || !HoverItem.IsChildOf(PopupItem))
                            {
                                SafeCall(PopupItem.OnPopupClose, PopupItem);
                                DestroyWidget(PopupItem);
                                PopupItem = null;
                            }
                        }

                        if (HoverItem != null)
                        {
                            Args.Handled = true;
                            SafeCall(HoverItem.OnClick, HoverItem, newArgs);
                        }
                    }
                    break;
                case InputEvents.KeyPress:
                    if (FocusItem != null) SafeCall(FocusItem.OnKeyPress, FocusItem, Args);
                    break;
                case InputEvents.KeyDown:
                    if (FocusItem != null) SafeCall(FocusItem.OnKeyDown, FocusItem, Args);
                    break;
                case InputEvents.KeyUp:
                    if (FocusItem != null) SafeCall(FocusItem.OnKeyUp, FocusItem, Args);
                    break;
            }
        }

        public void Update(GameTime Time)
        {
            // Update mouse pointer animation.
            if (MousePointer != null)
                MousePointer.Update((float)Time.ElapsedGameTime.TotalSeconds);

            // Check to see if tooltip should be displayed.
            if (HoverItem != null && !String.IsNullOrEmpty(HoverItem.Tooltip))
            {
                var hoverTime = DateTime.Now - MouseMotionTime;
                if (hoverTime.TotalSeconds > SecondsBeforeTooltip)
                    ShowTooltip(MousePosition, HoverItem.Tooltip);
            }

            RunTime = Time.TotalGameTime.TotalSeconds;

            if (FocusItem != null) SafeCall(FocusItem.OnUpdateWhileFocus, FocusItem);
        }

        public void Draw()
        {
            Draw(Point.Zero);
        }

        /// <summary>
        /// Draw the GUI using the device provided earlier. Depth testing should be off.
        /// </summary>
        public void Draw(Point Offset)
        {
            RenderData.Device.DepthStencilState = DepthStencilState.None;

            RenderData.Effect.CurrentTechnique = RenderData.Effect.Techniques[0];

            RenderData.Effect.Parameters["View"].SetValue(Matrix.Identity);

            RenderData.Effect.Parameters["Projection"].SetValue(
                Matrix.CreateOrthographicOffCenter(Offset.X, Offset.X + RenderData.Device.Viewport.Width,
                Offset.Y + RenderData.Device.Viewport.Height, Offset.Y, -32, 32));

            var scale = RealScreen.Width / VirtualScreen.Width;

            // Need to offset by the subpixel portion to avoid screen artifacts.
            // Remove this offset is porting to Monogame, monogame does it correctly.
            RenderData.Effect.Parameters["World"].SetValue(
                Matrix.CreateTranslation(RealScreen.X, RealScreen.Y, 1.0f)
                * Matrix.CreateScale(scale)
#if GEMXNA
                * Matrix.CreateTranslation(0.5f, 0.5f, 0.0f));
#elif GEMMONO
                );
#endif

            RenderData.Effect.Parameters["Texture"].SetValue(RenderData.Texture);

            RenderData.Effect.CurrentTechnique.Passes[0].Apply();

            var mesh = RootItem.GetRenderMesh();
            mesh.Render(RenderData.Device);

            if (MousePointer != null)
            {
                var tileSheet = GetTileSheet(MousePointer.Sheet);
                var mouseMesh = Mesh.Quad()
                    .Scale(tileSheet.TileWidth, tileSheet.TileHeight)
                    .Translate(MousePosition.X, MousePosition.Y)
                    .Texture(tileSheet.TileMatrix(MousePointer.AnimationFrame));
                mouseMesh.Render(RenderData.Device);
            }
        }
    }
}