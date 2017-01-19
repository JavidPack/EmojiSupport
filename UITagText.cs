using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Localization;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Terraria;
using Terraria.UI.Chat;

namespace EmojiSupport
{
	class UITagText : UIElement
	{
		private object _text = "";
		private float _textScale = 1f;
		private Vector2 _textSize = Vector2.Zero;
		private bool _isLarge;
		private Color _color = Color.White;
		private int order;

		public string Text
		{
			get
			{
				return this._text.ToString();
			}
		}

		public Color TextColor
		{
			get
			{
				return this._color;
			}
			set
			{
				this._color = value;
			}
		}

		public UITagText(string text, int order, float textScale = 1f, bool large = false)
		{
			this.order = order;
			this.InternalSetText(text, textScale, large);
		}

		public UITagText(LocalizedText text, float textScale = 1f, bool large = false)
		{
			this.InternalSetText(text, textScale, large);
		}

		public override void Recalculate()
		{
			this.InternalSetText(this._text, this._textScale, this._isLarge);
			base.Recalculate();
		}

		public void SetText(string text)
		{
			this.InternalSetText(text, this._textScale, this._isLarge);
		}

		public void SetText(LocalizedText text)
		{
			this.InternalSetText(text, this._textScale, this._isLarge);
		}

		public void SetText(string text, float textScale, bool large)
		{
			this.InternalSetText(text, textScale, large);
		}

		public void SetText(LocalizedText text, float textScale, bool large)
		{
			this.InternalSetText(text, textScale, large);
		}

		private void InternalSetText(object text, float textScale, bool large)
		{
			SpriteFont spriteFont = large ? Main.fontDeathText : Main.fontMouseText;
			//Vector2 textSize = new Vector2(spriteFont.MeasureString(text.ToString()).X, large ? 32f : 16f) * textScale;
			this._text = text;
			this._textScale = textScale;
			this._isLarge = large;
			Vector2 stringSize = ChatManager.GetStringSize(Main.fontMouseText, Text, new Vector2(textScale, textScale));
			this._textSize = stringSize;
			this.MinWidth.Set(stringSize.X + this.PaddingLeft + this.PaddingRight, 0f);
			this.MinHeight.Set(stringSize.Y + this.PaddingTop + this.PaddingBottom, 0f);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			Vector2 pos = GetInnerDimensions().Position();

			int hoverSnippetIndex = -1;
			TextSnippet[] array = ChatManager.ParseMessage(Text, Color.White);
			ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, array, pos, 0f, Vector2.Zero, Vector2.One, out hoverSnippetIndex, -1f, 2f);
			if (hoverSnippetIndex > -1)
			{
				array[hoverSnippetIndex].OnHover();
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					array[hoverSnippetIndex].OnClick();
				}
			}
		}

		public override int CompareTo(object obj)
		{
			UITagText other = obj as UITagText;

			return order.CompareTo(other.order);
		}

	}
}
