using System;
using System.Windows.Forms;

namespace ARC
{
    public partial class HotswapConsole : Form
    {
        public HotswapConsole()
        {
            InitializeComponent();
        }
        public bool Completed = false;

        private void HotswapConsole_Load(object sender, EventArgs e)
        {
        }

        private void txtStatusText_TextChanged(object sender, EventArgs e)
        {
        }

        private void finished_Tick(object sender, EventArgs e)
        {
            if(txtStatusText.Text.Contains("Compressed file packing complete!") || txtStatusText.Text.Contains("Error"))
            {
                this.Close();
            }
        }
    }
}