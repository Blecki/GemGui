using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace GemGui
{
    public class TileReference
    {
        public String Sheet;
        public int Tile;
    }

    /// <summary>
    /// Inheritable properties for UIItems.
    /// Provides minor space savings for UIItems that do not require customization of every property.
    /// </summary>
    public class WidgetProperties
    {
        #region Settable Widget Properties

        /// <summary>
        /// Allows widget types to specify type-specific properties.
        /// </summary>
        public Object TypeProperties
        {
            set { Data.Upsert("type-properties", value); }
            get { return GetProperty("type-properties"); }
        }

        /// <summary>
        /// Transparent items are not drawn, but their children still are.
        /// </summary>
        public bool Transparent
        {
            set { Data.Upsert("transparent", value); }
            get { return (GetProperty("transparent") as bool?).Value; }
        }

        /// <summary>
        /// Color to apply to the uiitem's background
        /// </summary>
        public Vector4 BackgroundColor
        {
            set { Data.Upsert("bg-color", value); }
            get { return (GetProperty("bg-color") as Vector4?).Value; }
        }

        /// <summary>
        /// Background to draw on widget.
        /// </summary>
        public TileReference Background
        {
            set { Data.Upsert("bg", value); }
            get { return GetProperty("bg") as TileReference; }
        }

        /// <summary>
        /// If not null, it is the name of the tile sheet from the skin to use to draw a 'Scale9' border.
        /// </summary>
        public String Border
        {
            set { Data.Upsert("border", value); }
            get { return GetProperty("border") as String; }
        }

        /// <summary>
        /// Color of text on the uiitem.
        /// </summary>
        public Vector4 TextColor
        {
            set { Data.Upsert("text-color", value); }
            get { return (GetProperty("text-color") as Vector4?).Value; }
        }

        /// <summary>
        /// Text to draw
        /// </summary>
        public String Text
        {
            set { Data.Upsert("text", value); }
            get { return GetProperty("text") as String; }
        }

        public enum HorizontalAlign
        {
            Left,
            Right,
            Center
        }

        /// <summary>
        /// Where to position text inside the item.
        /// </summary>
        public HorizontalAlign TextHorizontalAlign
        {
            set { Data.Upsert("text-horizontal-align", value); }
            get { return (GetProperty("text-horizontal-align") as HorizontalAlign?).Value; }
        }

        public enum VerticalAlign
        {
            Top,
            Bottom,
            Center
        }

        /// <summary>
        /// Where to position text inside the item.
        /// </summary>
        public VerticalAlign TextVerticalAlign
        {
            set { Data.Upsert("text-vertical-align", value); }
            get { return (GetProperty("text-vertical-align") as VerticalAlign?).Value; }
        }

        /// <summary>
        /// Integet scalling of font.
        /// </summary>
        public int TextSize
        {
            set { Data.Upsert("text-size", value); }
            get { return (GetProperty("text-size") as int?).Value; }
        }

        // Todo: FONT property

        /// <summary>
        /// Action to be called every frame that item is the hover item.
        /// </summary>
        public Action<MouseEventArgs> OnMouseHover
        {
            set { Data.Upsert("on-mouse-hover", value); }
            get { return GetProperty("on-mouse-hover") as Action<MouseEventArgs>; }
        }

        /// <summary>
        /// Called when mouse enters uiitem.
        /// </summary>
        public Action<MouseEventArgs> OnMouseEnter
        {
            set { Data.Upsert("on-mouse-enter", value); }
            get { return GetProperty("on-mouse-enter") as Action<MouseEventArgs>; }
        }

        /// <summary>
        /// Called when mouse leaves uiitem.
        /// </summary>
        public Action<MouseEventArgs> OnMouseLeave
        {
            set { Data.Upsert("on-mouse-leave", value); }
            get { return GetProperty("on-mouse-leave") as Action<MouseEventArgs>; }
        }

        /// <summary>
        /// Called when uiitem is clicked.
        /// </summary>
        public Action<MouseEventArgs> OnClick
        {
            set { Data.Upsert("on-click", value); }
            get { return GetProperty("on-click") as Action<MouseEventArgs>; }
        }

        /// <summary>
        /// Called when widget is given focus
        /// </summary>
        public Action OnGainFocus
        {
            set { Data.Upsert("on-gain-focus", value); }
            get { return GetProperty("on-gain-focus") as Action; }
        }

        /// <summary>
        /// Called when widget loses focus
        /// </summary>
        public Action OnLoseFocus
        {
            set { Data.Upsert("on-lose-focus", value); }
            get { return GetProperty("on-lose-focus") as Action; }
        }

        public Action<KeyEventArgs> OnKeyPress
        {
            set { Data.Upsert("on-key-press", value); }
            get { return GetProperty("on-key-press") as Action<KeyEventArgs>; }
        }

        public Action<KeyEventArgs> OnKeyDown
        {
            set { Data.Upsert("on-key-down", value); }
            get { return GetProperty("on-key-down") as Action<KeyEventArgs>; }
        }

        public Action<KeyEventArgs> OnKeyUp
        {
            set { Data.Upsert("on-key-up", value); }
            get { return GetProperty("on-key-up") as Action<KeyEventArgs>; }
        }

        #endregion

        /// <summary>
        /// Set the property set that this one will inherit values from.
        /// </summary>
        /// <param name="Parent"></param>
        public void InheritFrom(WidgetProperties Parent)
        {
            this.Parent = Parent;
        }

        #region Private Implementation

        /*
         * This implementation comes with some space savings, since UIItems don't need to store
         * properties they aren't using. However, it comes at the cost of being far more
         * expensive to actually get the properties. The advantage and the reason this implementation
         * exists is the automatic inheritance of property values.
         */

        private Dictionary<String, Object> Data = new Dictionary<String, Object>();
        private WidgetProperties Parent = null;
        
        private Object GetProperty(String Name)
        {
            if (Data.ContainsKey(Name)) return Data[Name];
            else if (Parent != null) return Parent.GetProperty(Name);
            else return null;
        }

        #endregion
    }
}
