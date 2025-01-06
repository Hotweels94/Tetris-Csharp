using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace Tetris_C_
{
    public partial class MainMenu : Form
    {

        public bool playMusic = true;

        public MainMenu()
        {
            InitializeComponent();
            SoundPlayer music = new SoundPlayer("C:\\Tetris-Csharp\\Music\\music_tetris.wav");
            music.Play();
        }

        // Play Button
        private void button1_Click(object sender, EventArgs e)
        {
            Form1 objForm1 = new Form1();
            objForm1.Show();
            this.Hide();
        }

        // Quit Button
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Exit Tetris ?", "", MessageBoxButtons.YesNo);
            switch (dr)
            {
                case DialogResult.Yes:
                    Application.Exit();
                    break;
                case DialogResult.No:
                    break;
            }

        }

        // Options Button
        private void button3_Click(object sender, EventArgs e)
        {
            Options objOptions = new Options();
            objOptions.Show();
            this.Hide();
        }
    }
}
