using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Media;

namespace DeClusterBot
{
	
	public partial class Form1 : Form
	{
		private List<Bullet> bullets = new List<Bullet>();
		private List<Consumable> ammoBoxes = new List<Consumable>();
		private List<Consumable> medkits = new List<Consumable>();
		private List<Label> labels = new List<Label>();
		private List<int> ticks = new List<int>();
		private Menu menu;
		private Random rng = new Random();
		private bool left;
		private bool right;
		private bool up;
		private bool down;
		private int speed;
		private double diagonalSpeed;
		private int mouseX;
		private int mouseY;
		private int timer = 10;
		private bool bulletCooldown;
		public static Point[] spawnPoints = new Point[]
		{
			new Point(110, 95),
			new Point(300, 90),
			new Point(30, 290),
			new Point(30, 530),
			new Point(110, 440),
			new Point(140, 670),
			new Point(30, 830),
			new Point(270, 840),
			new Point(320, 300),
			new Point(850, 840),
			new Point(500, 840),
			new Point(600, 600),
			new Point(520, 200),
			new Point(850, 440)
		};
		private int score;
		private int highscore;
		private int killCount;
		private int ammoCount = 50;
		private string hash = "$clu5T3rB0T";
		private string keyLogger = "";
		private bool debugMode;
		private bool generateEnemies = true;
		private bool godMode;
		private SoundPlayer bulletSP = new SoundPlayer();
		private Character player;
		public static GridItem[,] mapGrid;
		public List<Character> enemies = new List<Character>();
		public static string basePath = Assembly.GetExecutingAssembly().Location;
		private Point moveVector = new Point(0, 0);
		
		private Label label2;
		private Label label3;
		private ProgressBar progressBar1;
		private System.Windows.Forms.Timer timer1;
		private PictureBox pictureBox2;
		private Label label1;
		private Label label4;
		private System.Windows.Forms.Timer TickTimer;

		public Form1()
		{
			this.InitializeComponent();
		}

		
		private void Form1_Load(object sender, EventArgs e)
		{
			Form1.basePath = Form1.basePath.Remove(Form1.basePath.LastIndexOf("DeClusterBot.exe"));
			base.KeyPreview = true;
			this.menu = new Menu(this, this.timer1, Form1.basePath);
			this.player = new Character(Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())]);
			this.player.isEnemy = false;
			this.speed = 5;
			this.diagonalSpeed = (double)this.speed / Math.Sqrt(2.0);
			if (!File.Exists(Form1.basePath + "Save\\Highscore.crpt") || !File.Exists(Form1.basePath + "Assets\\Textures\\mapSkeleton.png") || !File.Exists(Form1.basePath + "Assets\\SFX\\Shoot.wav"))
			{
				MessageBox.Show("Chybjející soubory.");
				base.Close();
				return;
			}
			try
			{
				byte[] data = Convert.FromBase64String(File.ReadAllText(Form1.basePath + "Save\\Highscore.crpt"));
				using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
				{
					byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(this.hash));
					using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider
					{
						Key = keys,
						Mode = CipherMode.ECB,
						Padding = PaddingMode.PKCS7
					})
					{
						byte[] results = tripDes.CreateDecryptor().TransformFinalBlock(data, 0, data.Length);
						this.highscore = int.Parse(Encoding.UTF8.GetString(results));
					}
				}
				this.label4.Text = "Highscore: " + this.highscore.ToString();
				IntPtr cursor = Form1.LoadCursorFromFile(Form1.basePath + "Assets\\Textures\\Cursor.cur");
				this.Cursor = new Cursor(cursor);
				base.CenterToScreen();
			}
			catch
			{
				MessageBox.Show("Lídl hashovací error.");
				Application.Exit();
			}
		}

		
		private void Form1_Activated(object sender, EventArgs e)
		{
			if (Form.ActiveForm == null)
			{
				return;
			}
			try
			{
				Bitmap mapImg = new Bitmap(Form1.basePath + "Assets/Textures/mapSkeleton.png");
				Form1.mapGrid = new GridItem[Form.ActiveForm.Width / 10, Form.ActiveForm.Height / 10];
				for (int i = 0; i < Form.ActiveForm.Width / 10; i++)
				{
					for (int j = 0; j < Form.ActiveForm.Height / 10; j++)
					{
						Form1.mapGrid[i, j] = new GridItem(i * 10, j * 10);
						byte r = mapImg.GetPixel(i * 10, j * 10).R;
						if (r != 0)
						{
							if (r != 128)
							{
								Form1.mapGrid[i, j].material = GridItem.Material.Air;
							}
							else
							{
								Form1.mapGrid[i, j].material = GridItem.Material.Object;
							}
						}
						else
						{
							Form1.mapGrid[i, j].material = GridItem.Material.Wall;
						}
					}
				}
			}
			catch
			{
				MessageBox.Show("Chyba v debilitě nazývaný mapGrid GridItem pole.");
				Application.Exit();
			}
			this.bulletSP.SoundLocation = Form1.basePath + "Assets\\SFX\\Shoot.wav";
			this.bulletSP.Load();
		}

		
		private void Form1_Resize(object sender, EventArgs e)
		{
			if (Form.ActiveForm == null)
			{
				return;
			}
			Form1.mapGrid = new GridItem[Form.ActiveForm.Width / 10, Form.ActiveForm.Height / 10];
			for (int i = 0; i < Form.ActiveForm.Width / 10; i++)
			{
				for (int j = 0; j < Form.ActiveForm.Height / 10; j++)
				{
					Form1.mapGrid[i, j] = new GridItem(i * 10, j * 10, GridItem.Material.Air);
				}
			}
			this.pictureBox2.Size = new Size(Form.ActiveForm.Width, Form.ActiveForm.Height);
			this.progressBar1.Location = new Point(Form.ActiveForm.Width - this.progressBar1.Width - 30, this.progressBar1.Location.Y);
			this.label3.Location = new Point(this.progressBar1.Location.X - this.label3.Width - 30, this.label3.Location.Y);
			this.label2.Location = new Point(Form.ActiveForm.Width / 3, this.label2.Location.Y);
		}

		
		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Oemtilde || this.keyLogger.Length > 20)
			{
				this.keyLogger = "";
			}
			else
			{
				this.keyLogger += ((char)e.KeyValue).ToString();
			}
			string text = this.keyLogger;
            switch (text)
            {
				case "AMMO":
					this.ammoCount += 50;
					break;
				case "START":
					this.generateEnemies = true;
					break;
				case "STOP":
					this.generateEnemies = false;
					break;
				case "BOSS":
					for (int i = 0; i < 20; i++)
					{
						this.SpawnEnemy();
					}
					break;
				case "DEBUG":
					this.debugMode = !this.debugMode;
					break;
				case "KILL":
					for (int j = this.enemies.Count - 1; j >= 0; j--)
					{
						this.enemies[j].Die();
						this.enemies.Remove(this.enemies[j]);
					}
					break;
				case "GOD":
					this.godMode = !this.godMode;
					break;
			} 
			Keys keyCode = e.KeyCode;
			if (keyCode <= Keys.A)
			{
				if (keyCode == Keys.Escape)
				{
					this.menu.Show();
					return;
				}
				if (keyCode != Keys.A)
				{
					return;
				}
				this.left = true;
				return;
			}
			else
			{
				if (keyCode == Keys.D)
				{
					this.right = true;
					return;
				}
				if (keyCode == Keys.S)
				{
					this.down = true;
					return;
				}
				if (keyCode != Keys.W)
				{
					return;
				}
				this.up = true;
				return;
			}
		}

		
		private void Form1_KeyUp(object sender, KeyEventArgs e)
		{
			Keys keyCode = e.KeyCode;
			if (keyCode <= Keys.D)
			{
				if (keyCode == Keys.A)
				{
					this.left = false;
					this.moveVector.X = 0;
					return;
				}
				if (keyCode != Keys.D)
				{
					return;
				}
				this.right = false;
				this.moveVector.X = 0;
				return;
			}
			else
			{
				if (keyCode == Keys.S)
				{
					this.down = false;
					this.moveVector.Y = 0;
					return;
				}
				if (keyCode != Keys.W)
				{
					return;
				}
				this.up = false;
				this.moveVector.Y = 0;
				return;
			}
		}

		
		private void pictureBox2_MouseDown(object sender, MouseEventArgs e)
		{
			if (!this.menu.visible && this.ammoCount > 0 && e.Button == MouseButtons.Left && !this.bulletCooldown)
			{
				this.score -= 3;
				this.ammoCount--;
				this.ShootBullet(this.player);
				this.bulletCooldown = true;
				if (this.ammoCount == 0)
				{
					this.label2.ForeColor = System.Drawing.Color.Black;
					this.label2.BackColor = System.Drawing.Color.Red;
				}
			}
		}

		
		private void pictureBox2_MouseMove(object sender, MouseEventArgs e)
		{
			if (e.X < 0 || e.Y < 0)
			{
				return;
			}
			this.mouseX = e.X;
			this.mouseY = e.Y;
		}

		
		private void timer1_Tick(object sender, EventArgs e)
		{
			if (Form.ActiveForm == null)
			{
				return;
			}
			if (this.left)
			{
				this.moveVector.X = -this.speed;
			}
			if (this.right)
			{
				this.moveVector.X = this.speed;
			}
			if (this.up)
			{
				this.moveVector.Y = -this.speed;
			}
			if (this.down)
			{
				this.moveVector.Y = this.speed;
			}
			if ((this.left && this.up) || (this.left && this.down) || (this.right && this.up) || (this.right && this.down))
			{
				this.speed = (int)this.diagonalSpeed;
			}
			else
			{
				this.speed = 5;
			}
			this.player.MoveBy(new Point(this.moveVector.X, this.moveVector.Y));
			for (int i = 0; i < this.bullets.Count; i++)
			{
				Bullet b = this.bullets[i];
				b.X += b.speedX;
				b.Y += b.speedY;
				if (b.X < Form.ActiveForm.Width && b.Y < Form.ActiveForm.Height && b.X > 0 && b.Y > 0)
				{
					if (Form1.mapGrid[b.X / 10, b.Y / 10].material != GridItem.Material.Air)
					{
						Character tempChar = Form1.mapGrid[b.X / 10, b.Y / 10].charOnGrid;
						if (tempChar != null && tempChar != b.originChar && b.originChar.isEnemy != tempChar.isEnemy)
						{
							this.bullets.Remove(b);
							if (tempChar == this.player)
							{
								if (!this.godMode)
								{
									this.player.health -= 7;
								}
								if (this.player.health <= 0)
								{
									this.player.health = 0;
									if (this.score < 0)
									{
										MessageBox.Show("Really man? Negative score? Try again, for your own sake.", "You're actually so bad.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
									else
									{
										MessageBox.Show(string.Concat(new string[]
										{
											(this.score > this.highscore) ? ("Good job! You beat the current highscore of " + this.highscore.ToString() + " points by killing ") : "You died, but managed to kill ",
											this.killCount.ToString(),
											" enemies and earned a ",
											(this.score < 1000) ? "disappointing" : ((this.score < 3000) ? "solid" : ((this.score < 8000) ? "amazing" : ((this.score < 12000) ? "huge" : ((this.score < 16000) ? "enormous" : "absolutely gigantic")))),
											" score of ",
											this.score.ToString(),
											"!"
										}), "You dead.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
									}
									if (this.score >= this.highscore)
									{
										StreamWriter streamWriter = new StreamWriter(Form1.basePath + "Save\\Highscore.crpt");
										streamWriter.Write(this.score.ToString());
										streamWriter.Close(); 
										byte[] data = Encoding.UTF8.GetBytes(File.ReadAllText(Form1.basePath + "Save\\Highscore.crpt"));
										using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
										{
											byte[] keys = md5.ComputeHash(Encoding.UTF8.GetBytes(this.hash));
											using (TripleDESCryptoServiceProvider tripDes = new TripleDESCryptoServiceProvider
											{
												Key = keys,
												Mode = CipherMode.ECB,
												Padding = PaddingMode.PKCS7
											})
											{
												byte[] results = tripDes.CreateEncryptor().TransformFinalBlock(data, 0, data.Length);
												File.WriteAllText(Form1.basePath + "Save\\Highscore.crpt", Convert.ToBase64String(results, 0, results.Length));
											}
										}
									}
									Application.Restart();
								}
								this.progressBar1.Value = this.player.health;
							}
							else
							{
								tempChar.health -= 20;
								if (tempChar.health <= 0)
								{
									this.score += 100;
									if (this.score > this.highscore)
									{
										this.label1.ForeColor = System.Drawing.Color.Black;
										this.label1.BackColor = System.Drawing.Color.Lime;
									}
									else
									{
										this.label1.ForeColor = System.Drawing.Color.White;
										this.label1.BackColor = System.Drawing.Color.Black;
									}
									this.enemies.Remove(tempChar);
									tempChar.Die();
									if (this.enemies.Count == 0)
									{
										this.SpawnEnemy();
									}
									this.killCount++;
									this.PlaySound(Form1.basePath + "\\Assets\\SFX\\Death.wav");
									if (this.rng.Next(0, 2) == 0)
									{
										Consumable box = new Consumable(tempChar.X + 25, tempChar.Y + 25, Form1.basePath + "Assets\\Textures\\AmmoBox.png");
										this.ammoBoxes.Add(box);
									}
									else
									{
										Consumable medkit = new Consumable(tempChar.X + 25, tempChar.Y + 25, Form1.basePath + "Assets\\Textures\\Medkit.png");
										this.medkits.Add(medkit);
									}
								}
							}
						}
					}
					if (Form1.mapGrid[b.X / 10, b.Y / 10].material == GridItem.Material.Wall)
					{
						this.bullets.Remove(b);
					}
				}
				else
				{
					this.bullets.Remove(b);
				}
			}
			for (int j = 0; j < this.ammoBoxes.Count; j++)
			{
				Consumable ammo = this.ammoBoxes[j];
				if (this.player.X + this.player.width > ammo.X && this.player.X < ammo.X + 50 && this.player.Y + this.player.height > ammo.Y && this.player.Y < ammo.Y + 50)
				{
					int ran = this.rng.Next(10, 21);
					this.ammoCount += ran;
					this.ammoBoxes.RemoveAt(j);
					Label lbl = new Label();
					lbl.Name = j.ToString();
					lbl.Text = "+" + ran.ToString();
					lbl.Location = new Point(this.player.centerX, this.player.centerY);
					lbl.AutoSize = true;
					lbl.Font = new Font("Verdana", 15f);
					lbl.BackColor = System.Drawing.Color.Black;
					lbl.ForeColor = System.Drawing.Color.Lime;
					base.Controls.Add(lbl);
					lbl.BringToFront();
					this.labels.Add(lbl);
					this.ticks.Add(30);
					this.label2.ForeColor = System.Drawing.Color.White;
					this.label2.BackColor = System.Drawing.Color.Black;
					this.PlaySound(Form1.basePath + "\\Assets\\SFX\\Reload.wav");
				}
			}
			for (int k = 0; k < this.medkits.Count; k++)
			{
				Consumable med = this.medkits[k];
				if (this.player.X + this.player.width > med.X && this.player.X < med.X + 50 && this.player.Y + this.player.height > med.Y && this.player.Y < med.Y + 50)
				{
					this.player.health += 30;
					if (this.player.health > 100)
					{
						this.player.health = 100;
					}
					this.medkits.RemoveAt(k);
					Label lbl2 = new Label();
					lbl2.Name = k.ToString();
					lbl2.Text = "+" + 30.ToString();
					lbl2.Location = new Point(this.player.centerX, this.player.centerY);
					lbl2.AutoSize = true;
					lbl2.Font = new Font("Verdana", 15f);
					lbl2.BackColor = System.Drawing.Color.Red;
					lbl2.ForeColor = System.Drawing.Color.White;
					base.Controls.Add(lbl2);
					lbl2.BringToFront();
					this.labels.Add(lbl2);
					this.ticks.Add(30);
					this.PlaySound(Form1.basePath + "\\Assets\\SFX\\Reload.wav");
					this.progressBar1.Value = this.player.health;
				}
			}
			for (int l = 0; l < this.labels.Count; l++)
			{
				List<int> list = this.ticks;
				int index = l;
				int num = list[index];
				list[index] = num - 1;
				this.labels[l].Location = new Point(this.labels[l].Location.X, this.labels[l].Location.Y - 2);
				if (this.ticks[l] <= 0)
				{
					base.Controls.Remove(this.labels[l]);
					this.labels.RemoveAt(l);
					this.ticks.RemoveAt(l);
				}
			}
			this.player.angle = this.CalcAngle(this.player, this.mouseX, this.mouseY);
			if (this.generateEnemies && this.timer % 200 == 0)
			{
				for (int m = 0; m < this.timer / (1000 * m + 1); m++)
				{
					this.SpawnEnemy();
				}
			}
			if (this.timer % 700 == 0)
			{
				if (this.rng.Next(0, 2) == 0)
				{
					this.ammoBoxes.Add(new Consumable(Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())], Form1.basePath + "Assets\\Textures\\AmmoBox.png"));
				}
				else
				{
					this.medkits.Add(new Consumable(Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())], Form1.basePath + "Assets\\Textures\\Medkit.png"));
				}
			}
			foreach (Character enemy in this.enemies)
			{
				enemy.angle = this.CalcAngle(enemy, this.player.centerX, this.player.centerY);
				if (this.rng.Next(1, 100) == 1)
				{
					this.ShootBullet(enemy);
				}
				if (Math.Abs(enemy.X - enemy.target.X) <= 5)
				{
					enemy.target.X = enemy.X;
				}
				if (Math.Abs(enemy.Y - enemy.target.Y) <= 5)
				{
					enemy.target.Y = enemy.Y;
				}
				if (enemy.position == enemy.target)
				{
					enemy.target = Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())];
				}
				enemy.MoveBy(new Point((enemy.X > enemy.target.X) ? -3 : ((enemy.target.X == enemy.X) ? 0 : 3), (enemy.Y > enemy.target.Y) ? -3 : ((enemy.target.Y == enemy.Y) ? 0 : 3)));
			}
			if (this.bulletCooldown && this.timer % 10 == 0)
			{
				this.bulletCooldown = false;
			}
			this.label1.Text = "Score: " + this.score.ToString();
			this.label2.Text = "Ammo: " + this.ammoCount.ToString();
			this.pictureBox2.Invalidate();
		}

		
		public double CalcAngle(Character character, int x, int y)
		{
			character.centerX = character.X + character.width / 2;
			character.centerY = character.Y + character.height / 2;
			int diffX = character.centerX - x;
			int diffY = Math.Abs(character.centerY - y);
			double prepona = Math.Sqrt((double)(diffY * diffY + diffX * diffX));
			double calcAngle;
			if (diffX < 0)
			{
				calcAngle = Math.Acos((double)diffX / prepona);
			}
			else
			{
				calcAngle = Math.Asin((double)diffY / prepona);
			}
			calcAngle *= 57.29577951308232;
			if (character.centerY < y)
			{
				calcAngle = 360.0 - calcAngle;
			}
			return calcAngle - 90.0 - 2000.0 / prepona;
		}

		
		public void ShootBullet(Character character)
		{
			double rad = (character.angle + (double)this.rng.Next(-2, 3)) * 3.141592653589793 / 180.0;
			Bullet bullet = new Bullet(character.centerX - 3 + (int)(Math.Sin(1.5707963267948966 + rad) * 30.0), character.centerY - 8 - (int)(Math.Cos(1.5707963267948966 + rad) * 35.0), (float)character.angle, false);
			bullet.speedX = (int)(Math.Sin(rad) * (double)bullet.speed);
			bullet.speedY = (int)(-(int)(Math.Cos(rad) * (double)bullet.speed));
			bullet.rotation = (float)character.angle;
			bullet.originChar = character;
			this.bullets.Add(bullet);
			this.bulletSP.Play();
		}

		
		public void PlaySound(string path)
		{
			SoundPlayer mediaPlayer = new SoundPlayer();
			mediaPlayer.SoundLocation = path;
			mediaPlayer.Play();
		}

		
		private void game_graphics(object sender, PaintEventArgs e)
		{
			if (Form.ActiveForm == null || double.IsNaN(this.player.angle))
			{
				return;
			}
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
			Graphics graphics = e.Graphics;
			GraphicsState state = graphics.Save();
			Bitmap playerMap = new Bitmap(Form1.basePath + "Assets/Textures/PlayerIcon.png");
			Image bulletImage = Image.FromFile(Form1.basePath + "Assets/Textures/Bullet.png");
			Image enemyImage = Image.FromFile(Form1.basePath + "Assets/Textures/EnemyIcon.png");
			foreach (Consumable box in this.ammoBoxes)
			{
				graphics.DrawImage(Image.FromFile(box.texturePath), box.X, box.Y);
			}
			foreach (Consumable med in this.medkits)
			{
				graphics.DrawImage(Image.FromFile(med.texturePath), med.X, med.Y);
			}
			for (int i = 0; i < this.bullets.Count; i++)
			{
				state = graphics.Save();
				Bullet b = this.bullets[i];
				this.Rotate(b.X, b.Y, (double)b.rotation, graphics);
				graphics.DrawImage(bulletImage, b.X, b.Y);
				graphics.Restore(state);
			}
			graphics.Restore(state);
			foreach (Character enemy in this.enemies)
			{
				state = graphics.Save();
				this.Rotate(enemy.centerX, enemy.centerY, enemy.angle, graphics);
				graphics.DrawImage(enemyImage, enemy.X, enemy.Y);
				graphics.Restore(state);
				System.Drawing.Color color;
				if (enemy.health <= 40)
				{
					color = System.Drawing.Color.Red;
				}
				else
				{
					color = System.Drawing.Color.Lime;
				}
				graphics.DrawRectangle(new System.Drawing.Pen(color, 5f), enemy.X + 10, enemy.Y + 120, (int)(0.7 * (double)enemy.health), 5);
			}
			graphics.Restore(state);
			if (this.debugMode)
			{
				GridItem[,] array = Form1.mapGrid;
				int upperBound = array.GetUpperBound(0);
				int upperBound2 = array.GetUpperBound(1);
				for (int j = array.GetLowerBound(0); j <= upperBound; j++)
				{
					for (int k = array.GetLowerBound(1); k <= upperBound2; k++)
					{
						GridItem gi = array[j, k];
						if (gi.material != GridItem.Material.Air)
						{
							graphics.DrawRectangle(new System.Drawing.Pen(System.Drawing.Brushes.Red), gi.X, gi.Y, 10, 10);
						}
					}
				}
			}
			this.Rotate(this.player.centerX, this.player.centerY, this.player.angle, graphics);
			graphics.DrawImage(playerMap, this.player.X, this.player.Y);
		}

		
		public void Rotate(int x, int y, double angle, Graphics g)
		{
			if (double.IsNaN(angle))
			{
				return;
			}
			g.TranslateTransform((float)x, (float)y);
			g.RotateTransform((float)angle);
			g.TranslateTransform((float)(-(float)x), (float)(-(float)y));
		}

		
		[DllImport("user32.dll")] // Why the fuck
		private static extern IntPtr LoadCursorFromFile(string fileName);

		
		public void SpawnEnemy(Point spawnPoint)
		{
			Character enemy = new Character(spawnPoint);
			enemy.isEnemy = true;
			this.enemies.Add(enemy);
			this.ValidateCharGrid(enemy);
		}

		
		public void SpawnEnemy()
		{
			Thread.Sleep(1);
			Character enemy = new Character(Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())]);
			enemy.isEnemy = true;
			enemy.target = Form1.spawnPoints[this.rng.Next(0, Form1.spawnPoints.Count<Point>())];
			this.enemies.Add(enemy);
			this.ValidateCharGrid(enemy);
		}

		
		public void ValidateCharGrid(Character c)
		{
			bool enemy = this.enemies.Contains(c);
			for (int i = c.X / 10 + 1; i < (c.X + c.width) / 10 - 2; i++)
			{
				for (int j = c.Y / 10 + 2; j < (c.Y + c.height) / 10 - 1; j++)
				{
					if (j * 10 < this.pictureBox2.Height && i * 10 < this.pictureBox2.Width)
					{
						Form1.mapGrid[i, j].material = (enemy ? GridItem.Material.Enemy : GridItem.Material.Player);
						Form1.mapGrid[i, j].charOnGrid = c;
					}
				}
			}
		}

		
		private void TickTimer_Tick(object sender, EventArgs e)
		{
			this.timer++;
		}
	}
}
