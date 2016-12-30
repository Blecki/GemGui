using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Gum;

namespace GemGuiTest
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        private Gum.Root GuiRoot;
        private KeyboardInput Input;
        public String Lib;

        public Main(String Lib)
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            Input = new KeyboardInput(Window.Handle);

            this.Lib = Lib;
        }

        protected override void LoadContent()
        {
            GuiRoot = new Gum.Root(
                GraphicsDevice,
                new Rectangle(0, 0, 640, 480),
                this.Content,
                String.Format("Content/{0}_draw", Lib),
                "Content/dwarf_corp_skin/sheets.txt");

            if (Lib == "xna") GuiRoot.AddPixelOffset = true;
            
            GuiRoot.MousePointer = new MousePointer("mouse", 1.0f, 0);
            GuiRoot.TooltipTextSize = 2;

            var frame = GuiRoot.RootItem.AddChild(
                GuiRoot.CreateWidget(new Widget
                {
                    Rect = new Rectangle(0,0,256,256),
                    Text = "- DEMO MENU -",
                    Border = "border-one",
                    TextSize = 2,
                    TextHorizontalAlign = HorizontalAlign.Center
                }));
            Layout.Center(frame, GuiRoot.VirtualScreen);

            var dock = new DockLayout(frame.GetDrawableInterior().Interior(0, 24, 0, 0), 2);

            dock.Dock(frame.AddChild(GuiRoot.CreateWidget(new Widget
            {
                Text = "View Atlas",
                Border = "border-thin",
                OnClick = (args) =>
                {
                    var dialog = GuiRoot.CreateWidget(new ShowTextureDialog());
                    GuiRoot.ShowDialog(dialog);
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2
            })), DockLayout.Sides.Top);

            dock.Dock(frame.AddChild(GuiRoot.CreateWidget(new Widget
            {
                Text = "View Demo Pane",
                Border = "border-thin",
                OnClick = (args) =>
                {
                    var dialog = GuiRoot.CreateWidget(new DemoDialog());
                    GuiRoot.ShowDialog(dialog);
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2
            })), DockLayout.Sides.Top);
                        
        }

        protected override void UnloadContent()
        {
        
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var inputEvents = Input.GetInputQueue();
            foreach (var @event in inputEvents)
                GuiRoot.HandleInput(@event.Message, @event.Args);

            GuiRoot.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.DepthStencilState = DepthStencilState.None;

            GuiRoot.Draw();
            base.Draw(gameTime);
        }
    }
}
