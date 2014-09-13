using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Platinum
{
	public class Utils
	{
		public static Texture2D ConvertToPreMultipliedAlpha(Texture2D texture)
		{
			Color[] data = new Color[texture.Width * texture.Height];
			texture.GetData<Color>(data, 0, data.Length);
			for (int i = 0; i < data.Length; i++)
			{
				byte A = data[i].A;
				data[i] = data[i] * (data[i].A / 255f);
				data[i].A = A;
			}
			texture.SetData<Color>(data, 0, data.Length);
			return texture;
		}

		public static Texture2D PixelUpscale(Texture2D texture, int scale)
		{
			// sanity check
			if (scale < 1) scale = 1;
			if (scale == 1) return texture;

			Texture2D newTex = new Texture2D(texture.GraphicsDevice, texture.Width * scale, texture.Height * scale);
			Color[] dataIn = new Color[texture.Width * texture.Height];
			texture.GetData<Color>(dataIn, 0, dataIn.Length);

			Color[] dataOut = new Color[newTex.Width * newTex.Height];
			for (int x = 0; x < texture.Width; x++)
			{
				for (int y = 0; y < texture.Height; y++)
				{
					Color px = dataIn[x + y * texture.Width];

					for (int i = 0; i < scale; i++)
					{
						for (int j = 0; j < scale; j++)
						{
							int nx = x * scale; int ny = y * scale;

							dataOut[(nx + i) + (ny + j) * newTex.Width] = px;
						}
					}
				}
			}
			newTex.SetData<Color>(dataOut, 0, dataOut.Length);

			return newTex;
		}
	}
}
