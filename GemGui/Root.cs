using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GemGui
{
    /// <summary>
    /// Root of a GUI.
    /// </summary>
    public class Root
    {
        private GraphicsDevice Device;
        private Effect Effect;
        private Texture2D GuiTexture;
        private MouseState OldMouseState;

        internal Dictionary<String, TileSheet> TileSheets = new Dictionary<String, TileSheet>();
        internal Widget HoverItem;
        public Widget FocusItem { get; private set; }

        public Rectangle VirtualScreen { get; private set; }
        public GemGui.Widget RootItem { get; private set; }

        public Root(GraphicsDevice Device, Rectangle VirtualScreen, ContentManager TextureSource, String Effect, String Skin)
        {
            this.Device = Device;
            this.VirtualScreen = VirtualScreen;

            this.Effect = TextureSource.Load<Effect>(Effect);

            // Load skin from disc. The skin is a set of tilesheets.
            var skin = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonTileSheetSet>(
                System.IO.File.ReadAllText(Skin));

            // Pack skin into a single texture.
            var atlas = TextureAtlas.Compiler.Compile(skin.Sheets.Select(s =>
                {
                    var realTexture = TextureSource.Load<Texture2D>(s.Texture);
                    return new TextureAtlas.Entry
                    {
                        Sheet = s,
                        Rect = new Rectangle(0, 0, realTexture.Width, realTexture.Height)
                    };
                }).ToList());

            // Create the atlas texture
            GuiTexture = new Texture2D(Device, atlas.Dimensions.Width, atlas.Dimensions.Height);
            foreach (var texture in atlas.Textures)
            {
                // Copy source texture into the atlas
                var realTexture = TextureSource.Load<Texture2D>(texture.Sheet.Texture);
                var textureData = new Color[realTexture.Width * realTexture.Height];
                realTexture.GetData(textureData);
                GuiTexture.SetData(0, texture.Rect, textureData, 0, realTexture.Width * realTexture.Height);

                // Create a tilesheet pointing into the atlas texture.
                TileSheets.Upsert(texture.Sheet.Name, new TileSheet(GuiTexture.Width,
                    GuiTexture.Height, texture.Rect, texture.Sheet.TileWidth, texture.Sheet.TileHeight));
            }

            // Create the default root element.
            RootItem = CreateUIItem(VirtualScreen, new GemGui.WidgetProperties
            {
                BackgroundColor = Vector4.One,
                TextColor = new Vector4(0,0,0,1),
                Border = null,
                Transparent = true,
                TextHorizontalAlign = WidgetProperties.HorizontalAlign.Left,
                TextVerticalAlign = WidgetProperties.VerticalAlign.Top,
                TextSize = 1
            });
        }

        /// <summary>
        /// Create a new UIItem. UIItems can only be added to heirarchy they are created by.
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="Properties"></param>
        /// <returns></returns>
        public T CreateUIItem<T>(Rectangle Rect, WidgetProperties Properties) where T: Widget, new()
        {
            var r = new T();
            r.Construct(Rect, Properties, this);
            return r;
        }

        public Widget CreateUIItem(Rectangle Rect, WidgetProperties Properties) 
        {
            return CreateUIItem<Widget>(Rect, Properties);
        }

        private void CleanupWidget(Widget Widget)
        {
            foreach (var child in Widget.Children)
                CleanupWidget(child);
            Widget.Root = null;
            if (Object.ReferenceEquals(FocusItem, Widget)) FocusItem = null;
            if (Object.ReferenceEquals(HoverItem, Widget)) HoverItem = null;
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
        /// Set keyboard focus to the specified widget. Fires lose and gain focus events as appropriate.
        /// </summary>
        /// <param name="On"></param>
        public void SetFocus(Widget On)
        {
            if (!Object.ReferenceEquals(this, On.Root)) throw new InvalidOperationException();
            if (Object.ReferenceEquals(FocusItem, On)) return;

            if (FocusItem != null) SafeCall(FocusItem.Properties.OnLoseFocus);
            FocusItem = On; 
            if (FocusItem != null) SafeCall(FocusItem.Properties.OnGainFocus);
        }

        /// <summary>
        /// Shortcut to call an action without having to check for null.
        /// </summary>
        /// <param name="Action"></param>
        private void SafeCall(Action<MouseEventArgs> Action, MouseEventArgs Args)
        {
            if (Action != null) Action(Args);
        }

        /// <summary>
        /// Shortcut to call an action without having to check for null.
        /// </summary>
        /// <param name="Action"></param>
        private void SafeCall(Action<KeyEventArgs> Action, KeyEventArgs Args)
        {
            if (Action != null) Action(Args);
        }

        /// <summary>
        /// Shortcut to call an action without having to check for null.
        /// </summary>
        /// <param name="Action"></param>
        private void SafeCall(Action Action)
        {
            if (Action != null) Action();
        }

        /// <summary>
        /// Process mouse events.
        /// </summary>
        /// <param name="State"></param>
        public void HandleMouse(MouseState State)
        {
            // Transform mouse from screen space to virtual gui space.
            var mouseVector = new Vector2(State.X, State.Y);
            mouseVector.X /= Device.Viewport.Width;
            mouseVector.Y /= Device.Viewport.Height;
            mouseVector.X *= VirtualScreen.Width;
            mouseVector.Y *= VirtualScreen.Height;

            var eventArgs = new MouseEventArgs { X = (int)mouseVector.X, Y = (int)mouseVector.Y };

            // Detect hover item and fire mouse enter/leave events as appropriate.
            var newHoverItem = RootItem.FindWidgetAt((int)mouseVector.X, (int)mouseVector.Y);
            if (!Object.ReferenceEquals(newHoverItem, HoverItem))
            {
                if (HoverItem != null) SafeCall(HoverItem.Properties.OnMouseLeave, eventArgs);
                if (newHoverItem != null) SafeCall(newHoverItem.Properties.OnMouseEnter, eventArgs);
                HoverItem = newHoverItem;
            }

            if (HoverItem != null)
            {
                SafeCall(HoverItem.Properties.OnMouseHover, eventArgs);

                if (OldMouseState.LeftButton == ButtonState.Pressed && State.LeftButton == ButtonState.Released)
                    SafeCall(HoverItem.Properties.OnClick, eventArgs);
            }

            OldMouseState = State;
        }

        /// <summary>
        /// Process the supplied key event.
        /// </summary>
        /// <param name="Event"></param>
        /// <param name="Args"></param>
        public void HandleKeyboard(KeyEvents Event, KeyEventArgs Args)
        {
            if (FocusItem != null)
            {
                switch (Event)
                {
                    case KeyEvents.KeyPress:
                        SafeCall(FocusItem.Properties.OnKeyPress, Args);
                        break;
                    case KeyEvents.KeyDown:
                        SafeCall(FocusItem.Properties.OnKeyDown, Args);
                        break;
                    case KeyEvents.KeyUp:
                        SafeCall(FocusItem.Properties.OnKeyUp, Args);
                        break;
                }
            }
        }

        /// <summary>
        /// Draw the GUI using the device provided earlier. Depth testing should be off.
        /// </summary>
        public void Draw()
        {
            Effect.CurrentTechnique = Effect.Techniques[0];

            Effect.Parameters["View"].SetValue(Matrix.Identity);
            
            // Add 0.5f to the viewport because XNA is broken. Remove this offset if using Monogame.
            Effect.Parameters["Projection"].SetValue(
                Matrix.CreateOrthographicOffCenter(0 + 0.5f, VirtualScreen.Width + 0.5f, 
                VirtualScreen.Height + 0.5f, 0 + 0.5f, -32, 32));

            Effect.Parameters["World"].SetValue(Matrix.Identity);
            Effect.Parameters["Texture"].SetValue(GuiTexture);

            Effect.CurrentTechnique.Passes[0].Apply();

            var mesh = RootItem.PrepareRenderMesh();
            mesh.Render(Device);
        }
    }
}
