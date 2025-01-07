using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris_C_
{
    public partial class Options : Form
    {
        MainMenu objMainMenu = new MainMenu();

        public Options()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (objMainMenu.musicPlaying)
            {
                objMainMenu.music.Stop();
                objMainMenu.musicPlaying = false;
            } else
            {
                objMainMenu.music.Play();
                objMainMenu.musicPlaying = true;
            } 
        }

        private void button2_Click(object sender, EventArgs e)
        {
            objMainMenu.Show();
            this.Hide();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            Application.Exit();
        }
    }
}
