using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DeClusterBot
{
	
	public class Character
	{
		public Point position;
		public int X;
		public int Y;
		public int centerX;
		public int centerY;
		public double angle;
		public int width = 100;
		public int height = 100;
		public int health = 100;
		public bool isEnemy;
		private GridItem[,] mapGrid;
		private static bool collisionRight;
		private static bool collisionLeft;
		private static bool collisionUp;
		private static bool collisionDown;
		public Point target;
		
		public void MoveBy(Point p)
		{
			try
			{
				int posX = p.X;
				int posY = p.Y;
				if (Form.ActiveForm != null)
				{
					this.mapGrid = Form1.mapGrid;
					Character.collisionRight = (this.X + posX + this.width > Form.ActiveForm.Width - 15);
					Character.collisionLeft = (this.X + posX < 0);
					Character.collisionUp = (this.Y + posY < 65);
					Character.collisionDown = (this.Y + posY + this.height > Form.ActiveForm.Height - 45);
					if (posX > 0 && !Character.collisionRight)
					{
						this.X += posX;
						for (int i = this.Y / 10 + 1; i < this.Y / 10 + 10; i++)
						{
							if (this.mapGrid[this.X / 10 + 8, i].material != GridItem.Material.Air)
							{
								this.X -= posX;
								Character.collisionRight = true;
								break;
							}
						}
					}
					else if (posX < 0 && !Character.collisionLeft)
					{
						this.X += posX;
						for (int j = this.Y / 10 + 1; j < this.Y / 10 + 10; j++)
						{
							if (this.mapGrid[this.X / 10 + 1, j].material != GridItem.Material.Air)
							{
								this.X -= posX;
								Character.collisionLeft = true;
								break;
							}
						}
					}
					if (posY > 0 && !Character.collisionDown)
					{
						this.Y += posY;
						for (int k = this.X / 10 + 1; k < this.X / 10 + 9; k++)
						{
							if (this.mapGrid[k, this.Y / 10 + 9].material != GridItem.Material.Air)
							{
								this.Y -= posY;
								Character.collisionDown = true;
								break;
							}
						}
					}
					else if (posY < 0 && !Character.collisionUp)
					{
						this.Y += posY;
						for (int l = this.X / 10 + 1; l < this.X / 10 + 9; l++)
						{
							if (this.mapGrid[l, this.Y / 10 + 1].material != GridItem.Material.Air)
							{
								this.Y -= posY;
								Character.collisionUp = true;
								break;
							}
						}
					}
					if (!Character.collisionRight)
					{
						for (int m = this.Y / 10 + 1; m < this.Y / 10 + 10; m++)
						{
							this.mapGrid[this.X / 10 + 8, m].material = GridItem.Material.Air;
							this.mapGrid[this.X / 10 + 8, m].charOnGrid = null;
						}
					}
					if (!Character.collisionLeft)
					{
						for (int n = this.Y / 10 + 1; n < this.Y / 10 + 10; n++)
						{
							this.mapGrid[this.X / 10 + 1, n].material = GridItem.Material.Air;
							this.mapGrid[this.X / 10 + 1, n].charOnGrid = null;
						}
					}
					if (!Character.collisionUp)
					{
						for (int i2 = this.X / 10 + 1; i2 < this.X / 10 + 9; i2++)
						{
							this.mapGrid[i2, this.Y / 10 + 1].material = GridItem.Material.Air;
							this.mapGrid[i2, this.Y / 10 + 1].charOnGrid = null;
						}
					}
					if (!Character.collisionDown)
					{
						for (int i3 = this.X / 10 + 1; i3 < this.X / 10 + 9; i3++)
						{
							this.mapGrid[i3, this.Y / 10 + 9].material = GridItem.Material.Air;
							this.mapGrid[i3, this.Y / 10 + 9].charOnGrid = null;
						}
					}
					for (int i4 = this.X / 10 + 2; i4 < this.X / 10 + 8; i4++)
					{
						for (int j2 = this.Y / 10 + 2; j2 < this.Y / 10 + 9; j2++)
						{
							this.mapGrid[i4, j2].material = GridItem.Material.Enemy;
							this.mapGrid[i4, j2].charOnGrid = this;
						}
					}
					if (this.Y == this.position.Y && this.X == this.position.X)
					{
						this.target = Form1.spawnPoints[new Random().Next(0, Form1.spawnPoints.Count<Point>())];
					}
					this.position = new Point(this.X, this.Y);
				}
			}
			catch
			{
				MessageBox.Show("Něco se pokazilo, prosím vraťte Vaše změny v souborech zpět nebo odzipujte hru znovu.");
				Application.Exit();
			}
		}

		public void Die()
		{
			for (int i = this.X / 10 + 1; i < this.X / 10 + 8; i++)
			{
				for (int j = this.Y / 10 + 2; j < this.Y / 10 + 9; j++)
				{
					this.mapGrid[i, j].material = GridItem.Material.Air;
					this.mapGrid[i, j].charOnGrid = null;
				}
			}
		}
		
		public Character(int posX, int posY)
		{
			this.X = posX;
			this.Y = posY;
			this.position = new Point(posX, posY);
		}
		public Character(Point posPoint)
		{
			this.X = posPoint.X;
			this.Y = posPoint.Y;
			this.position = posPoint;
		}

	}
}