using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using GemGui;

namespace GemGuiTest
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        private GemGui.Root GuiRoot;
        private KeyboardInput Input;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 800;
            graphics.PreferredBackBufferHeight = 600;
            IsMouseVisible = true;

            Input = new KeyboardInput(Window.Handle);
        }

        protected override void LoadContent()
        {
            GuiRoot = new GemGui.Root(
                GraphicsDevice,
                new Rectangle(0, 0, 800, 600),
                this.Content,
                "Content/gui/draw",
                "Content/gui/sheets.txt");

            var frame = GuiRoot.RootItem.AddChild(
                GuiRoot.CreateUIItem(GemGui.Layout.Center(new Rectangle(0,0,256,256), GuiRoot.VirtualScreen),
                new GemGui.WidgetProperties()
                {
                    Text = "- MAIN MENU -",
                    Border = "Border",
                    Transparent = false,
                    TextHorizontalAlign = WidgetProperties.HorizontalAlign.Center,
                    TextSize = 2
                }));

            var choiceLayout = new GemGui.BorderLayout(frame.Rect.Interior(8, 36, 8, 8), 2);

            frame.AddChild(
                GuiRoot.CreateUIItem(choiceLayout.Position(GemGui.BorderLayout.Sides.Top, 24),
                new GemGui.WidgetProperties()
                {
                    Text = "OPTION ONE",
                    BackgroundColor = new Vector4(1, 0, 0, 1),
                    Border = "thin",
                    OnClick = (args) => { frame.Properties.Text = "Clicked One"; frame.Invalidate(); },
                    TextHorizontalAlign = WidgetProperties.HorizontalAlign.Left,
                    TextVerticalAlign = WidgetProperties.VerticalAlign.Top,
                    TextSize = 2
                }));

            frame.AddChild(
                GuiRoot.CreateUIItem(choiceLayout.Position(GemGui.BorderLayout.Sides.Top, 24),
                new GemGui.WidgetProperties()
                {
                    Text = "OPTION TWO",
                    BackgroundColor = new Vector4(1, 0, 0, 1),
                    Border = "thin",
                    OnClick = (args) => { frame.Properties.Text = "Clicked Two"; frame.Invalidate(); },
                    TextHorizontalAlign = WidgetProperties.HorizontalAlign.Center,
                    TextVerticalAlign = WidgetProperties.VerticalAlign.Center,
                    TextSize = 2
                }));

            frame.AddChild(
                GuiRoot.CreateUIItem(choiceLayout.Position(GemGui.BorderLayout.Sides.Top, 24),
                new GemGui.WidgetProperties()
                {
                    Text = "Open Dialog",
                    BackgroundColor = new Vector4(1, 0, 0, 1),
                    Border = "thin",
                    OnClick = (args) => 
                    {
                        var dialog = GuiRoot.CreateUIItem<GemGui.Demo.SimpleDialog>(
                            new Rectangle(0,0,1,1),
                            new WidgetProperties() {});
                        GuiRoot.ShowDialog(dialog);
                    },
                    TextHorizontalAlign = WidgetProperties.HorizontalAlign.Right,
                    TextVerticalAlign = WidgetProperties.VerticalAlign.Bottom,
                    TextSize = 2
                }));
        }

        protected override void UnloadContent()
        {
        
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            var mouseState = Mouse.GetState();
            GuiRoot.HandleMouse(mouseState);

            var keyboardInput = Input.GetInputQueue();
            foreach (var @event in keyboardInput)
                GuiRoot.HandleKeyboard(@event.Message, @event.Args);
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
