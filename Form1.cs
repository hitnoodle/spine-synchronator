using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;
using System.Diagnostics;

namespace Spine_Animation_Sync
{
    public partial class Form1 : Form
    {
        public enum Mode
        {
            None,
            Animation,
            Bones,
        };
        protected Mode _SyncMode = Mode.None;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (_SyncMode == Mode.None)
            {
                MessageBox.Show("No mode selected");
                return;
            }

            string masterPath = this.textBox1.Text;
            string syncPath = this.textBox2.Text;

            if (masterPath.Contains(".json"))
            {
                string masterAnimation = File.ReadAllText(masterPath);

                if (_SyncMode == Mode.Animation)
                    SynchronizeAnimation(masterAnimation, syncPath);
                else if (_SyncMode == Mode.Bones)
                    SynchronizeBones(masterAnimation, syncPath);

                MessageBox.Show("Done synchronizing!");
            }
            else
                MessageBox.Show("Master File is not an animation file.");
        }

        private void SynchronizeAnimation(string masterAnimation, string syncPath)
        {
            string animationText = masterAnimation.Substring(masterAnimation.IndexOf("animation"));

            string[] syncFiles = Directory.GetFiles(syncPath, "*.json");
            foreach (string syncFile in syncFiles)
            {
                string syncAnimation = File.ReadAllText(syncFile);

                syncAnimation = syncAnimation.Remove(syncAnimation.IndexOf("animation"));
                syncAnimation += animationText;

                File.WriteAllText(syncFile, syncAnimation);
            }
        }

        private void SynchronizeBones(string masterAnimation, string syncPath)
        {
            int bonesIndex = masterAnimation.IndexOf("bones");
            int startIndex = masterAnimation.IndexOf("[", bonesIndex);
            int endIndex = masterAnimation.IndexOf("]", startIndex);

            string masterBones = masterAnimation.Substring(startIndex, endIndex - startIndex);

            string[] syncFiles = Directory.GetFiles(syncPath, "*.json");
            foreach (string syncFile in syncFiles)
            {
                string syncAnimation = File.ReadAllText(syncFile);

                int syncBoneIndex = syncAnimation.IndexOf("bones");
                int syncStartIndex = syncAnimation.IndexOf("[", syncBoneIndex);
                int syncEndIndex = syncAnimation.IndexOf("]", syncStartIndex);

                string syncBones = syncAnimation.Substring(syncStartIndex, syncEndIndex - syncStartIndex);
                syncAnimation = syncAnimation.Replace(syncBones, masterBones);

                File.WriteAllText(syncFile, syncAnimation);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string modeText = this.comboBox1.Text;
            if (modeText == "Animation")
                _SyncMode = Mode.Animation;
            else if (modeText == "Bones")
                _SyncMode = Mode.Bones;
            else
                _SyncMode = Mode.None;
        }
    }
}
