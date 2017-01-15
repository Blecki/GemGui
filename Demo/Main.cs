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

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;

            Input = new KeyboardInput(Window.Handle);

        }

        protected override void LoadContent()
        {
            var renderData = new Gum.RenderData(
                GraphicsDevice,
                this.Content,
#if GEMXNA
                "Content/xna_draw",
#elif GEMMONO
                "Content/mono_draw",
#endif
                "Content/dwarf_corp_skin/sheets.txt");
            GuiRoot = new Root(new Point(640, 480), renderData);
            
            GuiRoot.MousePointer = new MousePointer("mouse", 1.0f, 0);
            GuiRoot.TooltipTextSize = 2;

            var frame = GuiRoot.RootItem.AddChild(new Widget
                {
                    Text = "- DEMO MENU -",
                    Border = "border-one",
                    TextSize = 2,
                    TextHorizontalAlign = HorizontalAlign.Center,
                    MinimumSize = new Point(256, 256),
                    AutoLayout = AutoLayout.FloatCenter,
                    InteriorMargin = new Margin
                    {
                        Top = GuiRoot.GetTileSheet("font2").TileHeight * 2
                    },
                    Padding = new Margin(2,2,2,2),
                    Font = "font2"
                });
            
            frame.AddChild(new Widget
            {
                Text = "View Atlas",
                Border = "border-thin",
                OnClick = (sender, args) =>
                {
                    var dialog = GuiRoot.ConstructWidget(new ShowTextureDialog());
                    GuiRoot.ShowDialog(dialog);
                    GuiRoot.RootItem.Layout();
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop,
                Font = "font2"
            });

            frame.AddChild(new Widget
            {
                Text = "View Demo Pane",
                Border = "border-thin",
                OnClick = (sender, args) =>
                {
                    var dialog = GuiRoot.ConstructWidget(new DemoDialog());
                    GuiRoot.ShowDialog(dialog);
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop,
                Font = "font2"
            });

            frame.AddChild(new Widget
            {
                Text = "View Tab Pane",
                Border = "border-thin",
                OnClick = (sender, args) =>
                {
                    var dialog = GuiRoot.ConstructWidget(new TabDialog());
                    GuiRoot.ShowDialog(dialog);
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop,
                Font = "font2"
            });

            frame.AddChild(new Widget
            {
                Text = "Toggle Alignment",
                Border = "border-thin",
                OnClick = (sender, args) =>
                {
                    if (frame.AutoLayout == AutoLayout.FloatTopRight)
                        frame.AutoLayout = AutoLayout.FloatBottomRight;
                    else if (frame.AutoLayout == AutoLayout.FloatBottomRight)
                        frame.AutoLayout = AutoLayout.FloatBottomLeft;
                    else if (frame.AutoLayout == AutoLayout.FloatBottomLeft)
                        frame.AutoLayout = AutoLayout.FloatTopLeft;
                    else if (frame.AutoLayout == AutoLayout.FloatTopLeft)
                        frame.AutoLayout = AutoLayout.FloatTopRight;
                    else
                        frame.AutoLayout = AutoLayout.FloatTopLeft;
                    GuiRoot.RootItem.Layout();
                },
                TextHorizontalAlign = HorizontalAlign.Center,
                TextVerticalAlign = VerticalAlign.Center,
                TextSize = 2,
                AutoLayout = AutoLayout.DockTop,
                Font = "font2"
            });

            GuiRoot.RootItem.Layout();         
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
