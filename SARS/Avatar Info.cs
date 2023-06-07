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
        public Avatar _selectedAvatar;
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
                Clipboard.SetText(_selectedAvatar.avatar.recordCreated.ToString());
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar ID")
            {
                Clipboard.SetText(_selectedAvatar.avatar.avatarId);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar Name")
            {
                Clipboard.SetText(_selectedAvatar.avatar.avatarName);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Avatar Description")
            {
                Clipboard.SetText(_selectedAvatar.avatar.avatarDescription);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Author ID")
            {
                Clipboard.SetText(_selectedAvatar.avatar.authorId);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Author Name")
            {
                Clipboard.SetText(_selectedAvatar.avatar.authorName);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "PC Asset URL")
            {
                Clipboard.SetText(_selectedAvatar.avatar.pcAssetUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Quest Asset URL")
            {
                Clipboard.SetText(_selectedAvatar.avatar.questAssetUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Image URL")
            {
                Clipboard.SetText(_selectedAvatar.avatar.imageUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Thumbnail URL")
            {
                Clipboard.SetText(_selectedAvatar.avatar.thumbnailUrl);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Unity Version")
            {
                Clipboard.SetText(_selectedAvatar.avatar.unityVersion);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Release Status")
            {
                Clipboard.SetText(_selectedAvatar.avatar.releaseStatus);
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }

            if (cbCopy.Text == "Tags")
            {               
                Clipboard.SetText(String.Join(", ", _selectedAvatar.tags.Select(p => p.ToString()).ToArray()));
                MessageBox.Show(this, "information copied to clipboard.", "Copied", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }
    }
}