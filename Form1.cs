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
            Events,
            All,
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
                else if (_SyncMode == Mode.Events)
                    SynchronizeEvents(masterAnimation, syncPath);
                else if (_SyncMode == Mode.All)
                    SynchronizeAll(masterAnimation, syncPath);

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

        private void SynchronizeEvents(string masterAnimation, string syncPath)
        {
            int eventsIndex = masterAnimation.IndexOf("events");
            int startIndex = masterAnimation.IndexOf("{", eventsIndex);
            int endIndex = GetCurlyBracesEnd(masterAnimation, startIndex);

            string masterEvents = masterAnimation.Substring(startIndex, endIndex - startIndex);

            string[] syncFiles = Directory.GetFiles(syncPath, "*.json");
            foreach (string syncFile in syncFiles)
            {
                string syncAnimation = File.ReadAllText(syncFile);

                int syncEventIndex = syncAnimation.IndexOf("events");
                if (syncEventIndex != -1)
                {
                    int syncStartIndex = syncAnimation.IndexOf("{", syncEventIndex);
                    int syncEndIndex = GetCurlyBracesEnd(syncAnimation, syncStartIndex);

                    string syncEvents = syncAnimation.Substring(syncStartIndex, syncEndIndex - syncStartIndex);
                    syncAnimation = syncAnimation.Replace(syncEvents, masterEvents);

                    File.WriteAllText(syncFile, syncAnimation);
                }
                else
                {
                    int syncStartIndex = syncAnimation.IndexOf(",\"animations\"");

                    string syncEvent = "},\"events\":" + masterEvents;
                    syncAnimation = syncAnimation.Insert(syncStartIndex - 1, syncEvent);

                    File.WriteAllText(syncFile, syncAnimation);
                }
            }
        }

        int GetCurlyBracesEnd(string master, int startIndex)
        {
            int endIndex = master.IndexOf("}", startIndex);

            bool endFound = false;
            int tempIndex = startIndex + 1;
            while (!endFound)
            {
                int newIndex = master.IndexOf("{", tempIndex);
                if (tempIndex <= newIndex && newIndex < endIndex)
                {
                    endIndex = master.IndexOf("}", endIndex + 1);
                    tempIndex = newIndex + 1;
                }
                else
                {
                    endFound = true;
                }
            }

            return endIndex;
        }

        private void SynchronizeAll(string masterAnimation, string syncPath)
        {
            SynchronizeBones(masterAnimation, syncPath);
            SynchronizeEvents(masterAnimation, syncPath);
            SynchronizeAnimation(masterAnimation, syncPath);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string modeText = this.comboBox1.Text;
            if (modeText == "Animation")
                _SyncMode = Mode.Animation;
            else if (modeText == "Bones")
                _SyncMode = Mode.Bones;
            else if (modeText == "Events")
                _SyncMode = Mode.Events;
            else if (modeText == "All")
                _SyncMode = Mode.All;
            else
                _SyncMode = Mode.None;
        }
    }
}
