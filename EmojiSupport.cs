using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace EmojiSupport
{
	public class EmojiSupport : Mod
	{
		internal static EmojiSupport instance;
		internal static UserInterface emojiSupportUserInterface;
		internal EmojiSupportUI emojiSupportUI;

		public EmojiSupport()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
			};
		}

		public override void Load()
		{
			instance = this;

			ChatManager.Register<EmojiTagHandler>(new string[]
			{
				"e",
				"emoji"
			});

			if (!Main.dedServ)
			{
				emojiSupportUI = new EmojiSupportUI();
				emojiSupportUI.Activate();
				emojiSupportUserInterface = new UserInterface();
				emojiSupportUserInterface.SetState(emojiSupportUI);
			}
		}
		int lastSeenScreenWidth;
		int lastSeenScreenHeight;
		public override void ModifyInterfaceLayers(List<MethodSequenceListItem> layers)
		{
			int inventoryLayerIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (inventoryLayerIndex != -1)
			{
				layers.Insert(inventoryLayerIndex, new MethodSequenceListItem(
					"EmojiSupport: Keyboard",
					delegate
					{
						if (Main.drawingPlayerChat)
						{
							if (lastSeenScreenWidth != Main.screenWidth || lastSeenScreenHeight != Main.screenHeight)
							{
								emojiSupportUserInterface.Recalculate();
								lastSeenScreenWidth = Main.screenWidth;
								lastSeenScreenHeight = Main.screenHeight;
							}
							DrawUpdateKeyboardToggle();
							if (EmojiSupportUI.visible)
							{
								emojiSupportUserInterface.Update(Main._drawInterfaceGameTime);
								emojiSupportUI.Draw(Main.spriteBatch);
							}
						}
						else
						{
							if (emojiSupportUI.filterTextBox.focused)
							{
								emojiSupportUI.filterTextBox.Unfocus();
							}
						}
						return true;
					},
					null)
				);
			}


		}

		public override void PostUpdateInput()
		{
			if (Main.drawingPlayerChat && EmojiSupportUI.visible && emojiSupportUI.filterTextBox.focused)
			{
				// this is only capable of capturing non-special keys and copy pastes.
				NewUITextBox box = emojiSupportUI.filterTextBox;
				box.WriteAll(Main.GetInputText(box.Text));
				// it is redundant, but ok I guess, since the NewUITextBox also calls GetInputText
			}
		}

		internal void DrawUpdateKeyboardToggle()
		{
			Point mousePoint = new Point(Main.mouseX, Main.mouseY);
			// calcualte?
			int xPosition = Main.screenWidth - 220; //62; //78;
			int yPosition = Main.screenHeight - 36 + 10;

			Texture2D toggleEmojiKeyboardTexture = EmojiSupportUI.visible ? Main.inventoryTickOnTexture : Main.inventoryTickOffTexture;

			Rectangle toggleEmojiKeyboardButtonRectangle = new Rectangle(xPosition, yPosition, toggleEmojiKeyboardTexture.Width, toggleEmojiKeyboardTexture.Height);
			bool toggleEmojiKeyboardButtonHover = false;
			if (toggleEmojiKeyboardButtonRectangle.Contains(mousePoint))
			{
				Main.LocalPlayer.mouseInterface = true;
				toggleEmojiKeyboardButtonHover = true;
				if (Main.mouseLeft && Main.mouseLeftRelease)
				{
					EmojiSupportUI.visible = !EmojiSupportUI.visible;
					Main.PlaySound(EmojiSupportUI.visible ? SoundID.MenuOpen : SoundID.MenuClose);
				}
			}
			Main.spriteBatch.Draw(toggleEmojiKeyboardTexture, toggleEmojiKeyboardButtonRectangle.TopLeft(), Color.White /** 0.7f*/);
			if (toggleEmojiKeyboardButtonHover)
			{
				Main.toolTip = new Item();
				Main.hoverItemName = "Click to toggle Emoji keyboard";
			}
		}
	}
}
