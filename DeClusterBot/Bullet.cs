using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace DeClusterBot
{
	public class Bullet
	{
		public int X;
		public int Y;
		public int speedX;
		public int speedY;
		public float rotation;
		public int speed = 35;
		private double moveX;
		private double moveY;
		public bool damagePlayer;
		public Character originChar;
		public Bullet(int x, int y, float rotation, bool damagePlayer)
		{
			this.X = x;
			this.Y = y;
			this.rotation = rotation;
			this.moveX = (double)x;
			this.moveY = (double)y;
			this.damagePlayer = damagePlayer;
		}

		public void Move(GridItem[,] mapGrid)
		{
			this.moveX = Math.Cos((double)this.rotation) * 5.0;
			this.moveY = Math.Sin((double)this.rotation) * 5.0;
			this.X -= (int)this.moveX;
			this.Y -= (int)this.moveY;
		}

		public void Draw(Image img, double angle, Graphics g)
		{
			GraphicsState state = g.Save();
			g.TranslateTransform((float)(img.Width / 2), (float)(img.Height / 2));
			g.RotateTransform((float)angle);
			g.DrawImage(img, this.X, this.Y);
			g.Restore(state);
		}

	}
}