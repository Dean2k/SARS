using MetroFramework.Components;
using MetroFramework.Forms;
using SARS.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace SARS
{
    public partial class Avatar_Info : MetroForm
    {
        public AvatarModel _selectedAvatar;
        public Avatar_Info()
        {
            InitializeComponent();
        }

        private void Avatar_Info_Load(object sender, EventArgs e)
        {
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (cbCopy.Text == "Time Detected")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.RecordCreated.ToString());
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar ID")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.AvatarId);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar Name")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.AvatarName);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar Description")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.AvatarDescription);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Author ID")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.AuthorId);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Author Name")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.AuthorName);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "PC Asset URL")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.PcAssetUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Quest Asset URL")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.QuestAssetUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Image URL")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.ImageUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Thumbnail URL")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.ThumbnailUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Unity Version")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.UnityVersion);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Release Status")
            {
                Clipboard.SetText(_selectedAvatar.Avatar.ReleaseStatus);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Tags")
            {               
                Clipboard.SetText(String.Join(", ", _selectedAvatar.Tags.Select(p => p.ToString()).ToArray()));
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}