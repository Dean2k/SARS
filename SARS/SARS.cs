using MaterialSkin;
using MaterialSkin.Controls;
using SARS.Models;
using SARS.Modules;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace SARS
{
    public partial class SARS : MaterialForm
    {

        public List<Avatar> _avatars;
        public ConfigSave<Config> _configSave;
        public int _pageAmount = 30;

        public SARS()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.DARK;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void SARS_Load(object sender, EventArgs e)
        {
            ShrekApi shrekApi = new ShrekApi();
            string filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                _configSave = new ConfigSave<Config>(filePath + "\\config.cfg");
            }
            catch
            {
                MessageBox.Show("Error with config file, settings reset");
                File.Delete(filePath + "\\config.cfg");
                Console.WriteLine("Error with config");
            }

            if(string.IsNullOrEmpty(_configSave.Config.UserAgent))
            {
                _configSave.Config.UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/104.0.5112.79 Safari/537.36";
            }
            AvatarSearch avatarSearch = new AvatarSearch { AvatarName = "test", Key = _configSave.Config.ApiKey, Amount = Convert.ToInt32(1000), PrivateAvatars = true, PublicAvatars = true, ContainsSearch = true, DebugMode = true, PcAvatars = true, QuestAvatars = false };
            _avatars = shrekApi.AvatarSearch(avatarSearch, false, null, true);
            ClearFlowPanel2();
            int buttonCount = _avatars.Count / _pageAmount;
            for (int i = 0; i < buttonCount; i++)
            {
                try
                {
                    MaterialButton materialButton = new MaterialButton()
                    {
                        Text = i.ToString()
                    };
                    materialButton.Click += new EventHandler(button_Click);
                    flowLayoutPanel2.Controls.Add(materialButton);
                } catch (Exception ex) {
                    Console.WriteLine(ex.Message);
                
                }
            }
            Task.Run(() => AvatarsSearching(0));
        }

        void button_Click(object sender, System.EventArgs e)
        {
            flowLayoutPanel1.AutoScrollPosition = new Point(0, 0);
            MaterialButton btn = (MaterialButton)sender;
            Task.Run(() => AvatarsSearching(Convert.ToInt32(btn.Text)));
        }

        private async Task<bool> AvatarsSearching(int skip)
        {
            SafeRender(false);
            ClearFlowPanel();
            foreach (var item in _avatars.Skip(skip * _pageAmount).Take(_pageAmount))
            {
                AddControl(item);
            }
            SafeRender(true);
            return true;
        }

        public void AddControl(Avatar avatar)
        {
            if (flowLayoutPanel1.InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { AddControl(avatar); };
                flowLayoutPanel1.Invoke(safeWrite);
            }
            else
            {
                AvatarCard avatarCard = new AvatarCard(avatar);
                flowLayoutPanel1.Controls.Add(avatarCard);
            }
        }
        public void ClearFlowPanel()
        {
            if (flowLayoutPanel1.InvokeRequired)
            {
                Action safeWrite = delegate { ClearFlowPanel(); };
                flowLayoutPanel1.Invoke(safeWrite);
            }
            else
            {
                while (flowLayoutPanel1.Controls.Count > 0)
                {
                    var control = flowLayoutPanel1.Controls[0];
                    flowLayoutPanel1.Controls.RemoveAt(0);
                    control.Dispose();
                }
            }
        }

        public void ClearFlowPanel2()
        {
            if (flowLayoutPanel2.InvokeRequired)
            {
                Action safeWrite = delegate { ClearFlowPanel2(); };
                flowLayoutPanel2.Invoke(safeWrite);
            }
            else
            {
                while (flowLayoutPanel2.Controls.Count > 0)
                {
                    var control = flowLayoutPanel2.Controls[0];
                    flowLayoutPanel2.Controls.RemoveAt(0);
                    control.Dispose();
                }
            }
        }

        public void SafeRender(bool render)
        {
            if (flowLayoutPanel1.InvokeRequired)
            {
                // Call this same method but append THREAD2 to the text
                Action safeWrite = delegate { SafeRender(render); };
                flowLayoutPanel1.Invoke(safeWrite);
            }
            else
            {
                if (!render)
                {
                    flowLayoutPanel1.SuspendLayout();
                }
                else
                {
                    flowLayoutPanel1.ResumeLayout();
                }
            }
        }
    }
}
