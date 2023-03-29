using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace DeClusterBot
{
	
	public class Menu
	{
		private List<Button> buttons = new List<Button>();
		private List<Control> ctrls = new List<Control>();
		private Button playBtn = new Button();
		private Button resBtn = new Button();
		private Button ctrlBtn = new Button();
		private Button about = new Button();
		private Button back = new Button();
		private Button quit = new Button();
		private Label controlsH = new Label();
		private Label controlsTxt = new Label();
		private Label aboutH = new Label();
		private Label aboutTxt = new Label();
		public bool visible = true;
		private Form1 form;
		private Timer timer;
		private Image img;
		private string basePath;
		private PictureBox menuPic = new PictureBox();

		public Menu(Form1 form, Timer timer, string basePath)
		{
			this.form = form;
			this.timer = timer;
			this.basePath = basePath;
			this.img = Image.FromFile(basePath + "Assets/Textures/main_logo2.png");
			this.menuPic.Location = new Point(100, 100);
			this.menuPic.Size = new Size(800, 750);
			this.menuPic.Paint += this.MenuPic_Paint;
			this.menuPic.Image = this.img;
			this.form.Controls.Add(this.menuPic);
			this.menuPic.BringToFront();
			this.playBtn.Text = "PLAY";
			this.playBtn.Location = new Point(150, 675);
			this.playBtn.AutoSize = true;
			this.playBtn.Font = new Font("Verdana", 25f);
			this.playBtn.BackColor = Color.Black;
			this.playBtn.ForeColor = Color.White;
			this.playBtn.Click += this.playBtn_click;
			this.form.Controls.Add(this.playBtn);
			this.playBtn.BringToFront();
			this.buttons.Add(this.playBtn);
			this.resBtn.Text = "RESTART";
			this.resBtn.Location = new Point(284, 675);
			this.resBtn.AutoSize = true;
			this.resBtn.Font = new Font("Verdana", 25f);
			this.resBtn.BackColor = Color.Black;
			this.resBtn.ForeColor = Color.White;
			this.resBtn.Click += this.ResBtn_Click;
			this.form.Controls.Add(this.resBtn);
			this.resBtn.BringToFront();
			this.buttons.Add(this.resBtn);
			this.ctrlBtn.Text = "CONTROLS";
			this.ctrlBtn.Location = new Point(150, 750);
			this.ctrlBtn.AutoSize = true;
			this.ctrlBtn.Font = new Font("Verdana", 25f);
			this.ctrlBtn.BackColor = Color.Black;
			this.ctrlBtn.ForeColor = Color.White;
			this.ctrlBtn.Click += this.CtrlBtn_Click;
			this.form.Controls.Add(this.ctrlBtn);
			this.ctrlBtn.BringToFront();
			this.buttons.Add(this.ctrlBtn);
			this.controlsH.Text = "CONTROLS";
			this.controlsH.Location = new Point(150, 150);
			this.controlsH.AutoSize = true;
			this.controlsH.Font = new Font("Verdana", 35f);
			this.controlsH.BackColor = Color.Black;
			this.controlsH.ForeColor = Color.White;
			this.form.Controls.Add(this.controlsH);
			this.controlsH.BringToFront();
			this.controlsH.Visible = false;
			this.ctrls.Add(this.controlsH);
			this.controlsTxt.Text = "Movement: WASD\nShooting: Left Mouse Button\nMenu: ESC\n\n\nPřes krabice se nedá přeskočit,\nale můžete je přestřelit.";
			this.controlsTxt.Location = new Point(175, 250);
			this.controlsTxt.AutoSize = true;
			this.controlsTxt.Font = new Font("Verdana", 25f);
			this.controlsTxt.BackColor = Color.Black;
			this.controlsTxt.ForeColor = Color.White;
			this.form.Controls.Add(this.controlsTxt);
			this.controlsTxt.BringToFront();
			this.controlsTxt.Visible = false;
			this.ctrls.Add(this.controlsTxt);
			this.about.Text = "ABOUT";
			this.about.Location = new Point(385, 750);
			this.about.AutoSize = true;
			this.about.Font = new Font("Verdana", 25f);
			this.about.BackColor = Color.Black;
			this.about.ForeColor = Color.White;
			this.about.Click += this.About_Click;
			this.form.Controls.Add(this.about);
			this.about.BringToFront();
			this.buttons.Add(this.about);
			this.aboutH.Text = "ABOUT";
			this.aboutH.Location = new Point(150, 150);
			this.aboutH.AutoSize = true;
			this.aboutH.Font = new Font("Verdana", 35f);
			this.aboutH.BackColor = Color.Black;
			this.aboutH.ForeColor = Color.White;
			this.form.Controls.Add(this.aboutH);
			this.aboutH.BringToFront();
			this.aboutH.Visible = false;
			this.ctrls.Add(this.aboutH);
			this.aboutTxt.Text = "ClusterBot je druhý projekt do předmětu\n\"Řízení projektů\" na škole SPŠT, jedná se\no hru vytvořenou ve Windows forms\nv jazyce C# a byla vytvořena ve třech\ntýdenních sprintech. Podrobněji je to\narkádová top - down střílečka na skóre.\n\nVývojový tým:\nLukáš Kurtin - Kapitán, programátor, SFX\nJan Mátl - Programátor, web designer\nVojtěch Mastný - Grafický designer";
			this.aboutTxt.Location = new Point(175, 250);
			this.aboutTxt.AutoSize = true;
			this.aboutTxt.Font = new Font("Verdana", 20f);
			this.aboutTxt.BackColor = Color.Black;
			this.aboutTxt.ForeColor = Color.White;
			this.form.Controls.Add(this.aboutTxt);
			this.aboutTxt.BringToFront();
			this.aboutTxt.Visible = false;
			this.ctrls.Add(this.aboutTxt);
			this.back.Text = "BACK";
			this.back.Location = new Point(725, 750);
			this.back.AutoSize = true;
			this.back.Font = new Font("Verdana", 25f);
			this.back.BackColor = Color.Black;
			this.back.ForeColor = Color.White;
			this.back.Click += this.Back_Click;
			this.form.Controls.Add(this.back);
			this.back.BringToFront();
			this.back.Visible = false;
			this.ctrls.Add(this.back);
			this.quit.Text = "QUIT";
			this.quit.Location = new Point(725, 750);
			this.quit.AutoSize = true;
			this.quit.Font = new Font("Verdana", 25f);
			this.quit.BackColor = Color.Black;
			this.quit.ForeColor = Color.White;
			this.quit.Click += this.Quit_Click;
			this.form.Controls.Add(this.quit);
			this.quit.BringToFront();
			this.buttons.Add(this.quit);
		}

		
		private void MenuPic_Paint(object sender, PaintEventArgs e)
		{
			Graphics g = e.Graphics;
			if (this.back.Visible)
			{
				g.FillRectangle(Brushes.Black, 0, 0, this.menuPic.Width, this.menuPic.Height);
			}
			ControlPaint.DrawBorder(g, this.menuPic.ClientRectangle, Color.White, 5, ButtonBorderStyle.Solid, Color.White, 5, ButtonBorderStyle.Solid, Color.White, 5, ButtonBorderStyle.Solid, Color.White, 5, ButtonBorderStyle.Solid);
		}

		
		private void playBtn_click(object sender, EventArgs e)
		{
			this.Hide();
		}

		
		private void ResBtn_Click(object sender, EventArgs e)
		{
			Application.Restart();
		}

		
		private void CtrlBtn_Click(object sender, EventArgs e)
		{
			this.HideMain();
			this.controlsH.Visible = true;
			this.controlsTxt.Visible = true;
		}

		
		private void About_Click(object sender, EventArgs e)
		{
			this.HideMain();
			this.aboutH.Visible = true;
			this.aboutTxt.Visible = true;
		}

		
		private void HideMain()
		{
			this.form.KeyPreview = false;
			foreach (Button button in this.buttons)
			{
				button.Visible = false;
			}
			this.back.Visible = true;
			this.menuPic.Image = null;
		}

		
		private void Back_Click(object sender, EventArgs e)
		{
			this.form.KeyPreview = true;
			foreach (Button button in this.buttons)
			{
				button.Visible = true;
			}
			foreach (Control control in this.ctrls)
			{
				control.Visible = false;
			}
			this.menuPic.Image = this.img;
		}

		
		private void Quit_Click(object sender, EventArgs e)
		{
			this.form.Close();
		}

		
		public void Show()
		{
			foreach (Button button in this.buttons)
			{
				button.Visible = true;
			}
			this.menuPic.Visible = true;
			this.timer.Stop();
			this.visible = true;
		}

		
		public void Hide()
		{
			foreach (Button button in this.buttons)
			{
				button.Visible = false;
			}
			this.menuPic.Visible = false;
			this.timer.Start();
			this.visible = false;
			this.form.Activate();
		}
	}
}
