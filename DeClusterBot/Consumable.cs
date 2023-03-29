using System;
using System.Drawing;

namespace DeClusterBot
{ 
	public class Consumable
	{
		public int X;
		public int Y;
		public string texturePath;

		public Consumable(int posX, int posY, string path)
		{
			this.X = posX;
			this.Y = posY;
			this.texturePath = path;
		}

		public Consumable(Point pos, string path)
		{
			this.X = pos.X;
			this.Y = pos.Y;
			this.texturePath = path;
		}
	}
}
