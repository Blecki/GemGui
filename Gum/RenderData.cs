using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Gum
{
    /// <summary>
    /// Encapsulates rendering data that GUI instances can share. This data is expensive to create
    /// so we only want to do it once.
    /// </summary>
    public class RenderData
    {
        public GraphicsDevice Device { get; private set; }
        public Effect Effect { get; private set; }
        public Texture2D Texture { get; private set; }
        public Dictionary<String, TileSheet> TileSheets { get; private set; }

        public RenderData(GraphicsDevice Device, ContentManager Content, String Effect, String Skin)
        {
            this.Device = Device;
            this.Effect = Content.Load<Effect>(Effect);

            // Load skin from disc. The skin is a set of tilesheets.
            var skin = Newtonsoft.Json.JsonConvert.DeserializeObject<JsonTileSheetSet>(
                System.IO.File.ReadAllText(Skin));

            // Pack skin into a single texture - Build atlas information from texture sizes.
            var atlas = TextureAtlas.Compiler.Compile(skin.Sheets.Select(s =>
                {
                    var realTexture = Content.Load<Texture2D>(s.Texture);
                    return new TextureAtlas.Entry
                    {
                        Sheet = s,
                        Rect = new Rectangle(0, 0, realTexture.Width, realTexture.Height)
                    };
                }).ToList());

            // Create the atlas texture
            Texture = new Texture2D(Device, atlas.Dimensions.Width, atlas.Dimensions.Height);

            foreach (var texture in atlas.Textures)
            {
                // Copy source texture into the atlas
                var realTexture = Content.Load<Texture2D>(texture.Sheet.Texture);
                var textureData = new Color[realTexture.Width * realTexture.Height];
                realTexture.GetData(textureData);
                Texture.SetData(0, texture.Rect, textureData, 0, realTexture.Width * realTexture.Height);

                // Create a tilesheet pointing into the atlas texture.
                TileSheets.Upsert(texture.Sheet.Name, new TileSheet(Texture.Width,
                    Texture.Height, texture.Rect, texture.Sheet.TileWidth, texture.Sheet.TileHeight));
            }
        }
    }
}
