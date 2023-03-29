using System;

namespace DeClusterBot
{
	
	public class GridItem
	{
		public int X;
		public int Y;
		public Character charOnGrid;
		public GridItem.Material material;
		public enum Material
		{
			Air,
			Wall,
			Enemy,
			Player,
			Object
		}

		public GridItem(int posX, int posY)
		{
			this.X = posX;
			this.Y = posY;
			this.material = GridItem.Material.Air;
		}

		public GridItem(int posX, int posY, GridItem.Material mat)
		{
			this.X = posX;
			this.Y = posY;
			this.material = mat;
		}

		public GridItem()
		{
		}

		public override string ToString()
		{
			return string.Concat(new string[]
			{
				"X=",
				this.X.ToString(),
				", Y=",
				this.Y.ToString(),
				", Mat=",
				this.material.ToString()
			});
		}
	}
}
