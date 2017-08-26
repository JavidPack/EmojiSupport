using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Text;
using Terraria.UI.Chat;
using Newtonsoft.Json.Linq;
using Terraria;
using ReLogic.Graphics;

namespace EmojiSupport
{
	public class EmojiTagHandler : ITagHandler
	{
		private class EmojiSnippet : TextSnippet
		{
			private int index;
			public string sn;
			// index = x*41 + y

			public EmojiSnippet(int index)
				: base("")
			{
				this.index = index;
				this.Color = Color.White;
				CheckForHover = true;
			}

			int resolution = 20;
			public override bool UniqueDraw(bool justCheckingString, out Vector2 size, SpriteBatch spriteBatch, Vector2 position = default(Vector2), Color color = default(Color), float scale = 1f)
			{
				if (!justCheckingString && color != Color.Black)
				{
					int num = this.index;
					color = Color.White;
					//Texture2D texture2D = mod.GetTexture("emoji" + num);// Main.textGlyphTexture[0];
					Texture2D texture2D = EmojiSupport.instance.GetTexture("sheet_google_"+resolution);// Main.textGlyphTexture[0];
					spriteBatch.Draw(texture2D, position, new Rectangle?(new Rectangle((index / 41) * resolution, (index % 41) * resolution, resolution, resolution)), color, 0f, Vector2.Zero, EmojiTagHandler.GlyphsScale, SpriteEffects.None, 0f);

					//spriteBatch.Draw(texture2D, position, null, color, 0f, Vector2.Zero, EmojiTagHandler.GlyphsScale, SpriteEffects.None, 0f);
					//spriteBatch.Draw(texture2D, position, new Rectangle?(texture2D.Frame(25, 1, num, num / 25)), color, 0f, Vector2.Zero, EmojiTagHandler.GlyphsScale, SpriteEffects.None, 0f);
				}
				size = new Vector2(resolution) * EmojiTagHandler.GlyphsScale;
				return true;
			}

			public override float GetStringLength(DynamicSpriteFont font)
			{
				return resolution * EmojiTagHandler.GlyphsScale;
			}

			public override void OnClick()
			{
				ChatManager.AddChatText(Main.fontMouseText, "[e:" + sn + "]", Vector2.One);
			}

			public override void OnHover()
			{
				//Main.toolTip = new Item();
				Main.hoverItemName = $"Emoji code: {sn}";
				Main.instance.MouseText(Main.hoverItemName, Main.rare, 0);
			}
		}

		//private const int GlyphsPerLine = 25;
		//private const int MaxGlyphs = 26;
		public static float GlyphsScale = 1f;
		//private static Dictionary<string, int> GlyphIndexes = new Dictionary<string, int>
		//{
		//	{ "love", 0 },
		//	{ "oh", 1 },
		//	{ "tounge", 2 },
		//	{ "laugh", 3 },
		//};
		internal static Dictionary<string, int> GlyphIndexes;


		public static void Initialize()
		{
			if (GlyphIndexes == null)
			{
				GlyphIndexes = new Dictionary<string, int>();

				string json = Encoding.UTF8.GetString(EmojiSupport.instance.GetFileBytes("emojifixed.json"));
				var jArr = JArray.Parse(json);
				foreach (var item in jArr)
				{
					int x = (int)item["sheet_x"];
					int y = (int)item["sheet_y"];
					int index = x * 41 + y;
					//string name = (string)item["short_name"];
					//GlyphIndexes[name] = index;
					foreach (string sn in item["short_names"])
					{
						GlyphIndexes[sn] = index;
					}
				}
			}
		}

		TextSnippet ITagHandler.Parse(string text, Color baseColor, string options)
		{
			Initialize();

			int num;
			//if (!int.TryParse(text, out num) || num >= 1620)
			//{
			//	return new TextSnippet(text);
			//}
			if (!EmojiTagHandler.GlyphIndexes.TryGetValue(text.ToLower(), out num))
			{
				return new TextSnippet(text);
			}
			return new EmojiTagHandler.EmojiSnippet(num)
			{
				DeleteWhole = true,

				Text = "[e:" + text.ToLower() + "]",
				sn = text.ToLower()
			};
		}

		//public static string GenerateTag(int index)
		//{
		//	string text = "[e";
		//	object obj = text;
		//	return string.Concat(new object[]
		//		{
		//			obj,
		//			":",
		//			index,
		//			"]"
		//		});
		//}

		//public static string GenerateTag(string keyname)
		//{
		//	int index;
		//	if (EmojiTagHandler.GlyphIndexes.TryGetValue(keyname, out index))
		//	{
		//		return EmojiTagHandler.GenerateTag(index);
		//	}
		//	return keyname;
		//}
	}
}
