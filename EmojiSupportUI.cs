using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using System;
using Terraria.ID;
using System.Linq;

namespace EmojiSupport
{
	class EmojiSupportUI : UIState
	{
		public static bool visible = false;
		public FixedUIScrollbar keyboardScrollbar;
		public UIPanel keyboardPanel;
		public UIGrid emojiGrid;
		public NewUITextBox filterTextBox;
		float spacing = 8f;
		public static string filterText = "";

		public override void OnInitialize()
		{
			keyboardPanel = new UIPanel();
			keyboardPanel.SetPadding(0);
			keyboardPanel.Left.Set(-250f, 1f);
			keyboardPanel.Top.Set(-250f, 1f);
			keyboardPanel.Width.Set(210f, 0f);
			keyboardPanel.Height.Set(200f, 0f);
			keyboardPanel.BackgroundColor = new Color(73, 94, 171);

			filterTextBox = new NewUITextBox("Type to find emoji", 1f);
			filterTextBox.BackgroundColor = Color.Transparent;
			filterTextBox.BorderColor = Color.Transparent;
			filterTextBox.Left.Pixels = 0;
			filterTextBox.Top.Pixels = 0;
			filterTextBox.Width.Pixels = 170;
			filterTextBox.OnTextChanged += () => { filterText = filterTextBox.Text; updateneeded = true; };
			keyboardPanel.Append(filterTextBox);

			emojiGrid = new UIGrid(6);//UIList();
			emojiGrid.Width.Set(-25f, 1f); // left spacing plus scrollbar
			emojiGrid.Height.Set(-2 * spacing - filterTextBox.GetDimensions().Height, 1f);
			emojiGrid.Left.Set(spacing, 0f);
			emojiGrid.Top.Set(spacing + filterTextBox.GetDimensions().Height, 0f);
			emojiGrid.ListPadding = 10f;
			keyboardPanel.Append(emojiGrid);

			//modList = new UIGrid(4);
			//modList.Top.Pixels = 32f + spacing;
			//modList.Left.Pixels = spacing;
			//modList.Width.Set(-25f, 1f);
			//modList.Height.Set(-55f, 1f);
			//modList.ListPadding = 12f;
			//mainPanel.Append(autoTrashGrid);

			keyboardScrollbar = new FixedUIScrollbar();
			keyboardScrollbar.SetView(100f, 1000f);
			keyboardScrollbar.Top.Pixels = 2 * spacing;
			keyboardScrollbar.Height.Set(-4 * spacing, 1f);
			keyboardScrollbar.Left.Set(-4, 0f);
			keyboardScrollbar.HAlign = 1f;
			keyboardPanel.Append(keyboardScrollbar);

			emojiGrid.SetScrollbar(keyboardScrollbar);

			EmojiTagHandler.Initialize();
			foreach (var item in EmojiTagHandler.GlyphIndexes)
			{
				UITagText text = new UITagText("[e:" + item.Key + "]", item.Value);
				emojiGrid._items.Add(text);
				emojiGrid._innerList.Append(text);
			}

			//for (int i = 0; i < 20; i++)
			//{
			//	UITagText text = new UITagText("[e:poop] [e:poop] [e:poop] [e:poop] [e:poop]", i);
			//	//modList._items.Add(text);
			//	//modList.Add(text);
			//	//	modList.Recalculate();
			//	modList._items.Add(text);
			//	modList._innerList.Append(text);
			//}
			emojiGrid.UpdateOrder();
			emojiGrid._innerList.Recalculate();

			Append(keyboardPanel);
		}

		private bool updateneeded;
		internal void UpdateCheckboxes()
		{
			if (!updateneeded) { return; }
			updateneeded = false;
			emojiGrid.Clear();

			EmojiTagHandler.Initialize();
			foreach (var item in EmojiTagHandler.GlyphIndexes)
			{
				if (item.Key.ToLower().IndexOf(filterText, StringComparison.OrdinalIgnoreCase) != -1)
				{
					UITagText text = new UITagText("[e:" + item.Key + "]", item.Value);
					emojiGrid._items.Add(text);
					emojiGrid._innerList.Append(text);
				}
			}
			emojiGrid.UpdateOrder();
			emojiGrid._innerList.Recalculate();
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			UpdateCheckboxes();
			if (keyboardPanel.ContainsPoint(Main.MouseScreen))
			{
				Main.player[Main.myPlayer].mouseInterface = true;
			}
		}
	}


	public class FixedUIScrollbar : UIScrollbar
	{
		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			UserInterface temp = UserInterface.ActiveInstance;
			UserInterface.ActiveInstance = EmojiSupport.emojiSupportUserInterface;
			base.DrawSelf(spriteBatch);
			UserInterface.ActiveInstance = temp;
		}

		public override void MouseDown(UIMouseEvent evt)
		{
			UserInterface temp = UserInterface.ActiveInstance;
			UserInterface.ActiveInstance = EmojiSupport.emojiSupportUserInterface;
			base.MouseDown(evt);
			UserInterface.ActiveInstance = temp;
		}
	}
}
