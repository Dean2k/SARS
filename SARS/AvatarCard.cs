using SARS.Models;
using SARS.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SARS
{
    public partial class AvatarCard : UserControl
    {
        public bool ControlDisposed = false;
        public AvatarCard(Avatar avatar)
        {           
            InitializeComponent();
            lblTitle.Text = avatar.avatar.avatarName;
            toolTip1.SetToolTip(lblTitle, avatar.avatar.avatarName);
            lblBody.Text = avatar.avatar.avatarDescription;
            toolTip1.SetToolTip(lblBody, avatar.avatar.avatarDescription);
            lblAvatarId.Text = avatar.avatar.avatarId;
            toolTip1.SetToolTip(lblAvatarId, avatar.avatar.avatarId);
            if (avatar.avatar.imageUrl != null)
            {
                Task.Run(() => LoadImage(avatar.avatar.thumbnailUrl));               
            }
            if(avatar.avatar.questAssetUrl.ToLower().Trim() == "none")
            {
                lblQuest.Visible = false;
            }
            if (avatar.avatar.pcAssetUrl.ToLower().Trim() == "none")
            {
                lblPc.Visible = false;
            }
        }


        public Task LoadImage(string url)
        {
            using (WebClient webClient = new WebClient())
            {
                //Needs a useragent to be able to view images.
                webClient.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.79 Safari/537.36");
                try
                {
                    Stream stream = webClient.OpenRead(url);
                    Bitmap bitmap = new Bitmap(stream);
                    stream.Close();
                    stream.Dispose();
                    if(bitmap != null && !ControlDisposed)
                    {
                        AddImage(ImageToByte(bitmap));
                    }
                    bitmap.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }

            }

            return null;
        }
        public static byte[] ImageToByte(Image img)
        {
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(img, typeof(byte[]));
        }

        public void AddImage(byte[] image)
        {
            if (imgAvatar.InvokeRequired)
            {
                Action safeWrite = delegate { AddImage(image); };
                imgAvatar.Invoke(safeWrite);
            }
            else
            {
                Bitmap imageBack;
                using (var ms = new MemoryStream(image))
                {
                    imageBack = new Bitmap(ms);
                }
                imgAvatar.Image = imageBack;
            }
        }

        private void AvatarCard_Load(object sender, EventArgs e)
        {

        }

        private void AControlRemoved(object sender, ControlEventArgs e)
        {
            imgAvatar.Image = null;
            ControlDisposed = true;
        }
    }
}
