using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SARS
{
    public partial class AvatarPreview : Form
    {
        public AvatarPreview(Bitmap image)
        {
            InitializeComponent();
            AvatarImage.Image = image;
        }

        private void Avatar_Preview_Load(object sender, EventArgs e)
        {

        }
    }
}
