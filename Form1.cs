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

namespace BingeWatcher
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            axWindowsMediaPlayer1.uiMode = "none";
            axWindowsMediaPlayer1.enableContextMenu = false;
            axWindowsMediaPlayer1.Ctlenabled = false;
            axWindowsMediaPlayer1.stretchToFit = true;
        }

        private void axWindowsMediaPlayer1_Click(object sender, AxWMPLib._WMPOCXEvents_ClickEvent e)
        {
            if (e.nButton == 2)
            {
                if (menuStrip1.Visible == true)
                {
                    menuStrip1.Visible = false;
                    panel1.Visible = false;
                }
                else
                {
                    menuStrip1.Visible = true;
                    panel1.Visible = true;
                }
            }
        }

        //Play a single video file
        private void singleVideoMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Title = "Open a single video";
            openFile.Filter += "Standard Video Files|*.asf;*.wma;*.wmv;*.wm;*.avi;*.mp*;*.m4*;*.wav;*.mov;*.mi*";
            openFile.Filter += "|All Files|*.*";

            try
            {
                if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    axWindowsMediaPlayer1.URL = openFile.FileName;
                    buttonEnable();
                    buttonPlay.Enabled = false;
                    buttonNext.Enabled = false;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Select a Season folder to play the entire season
        private void entireSeasonMenu_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.Description = "Open individual season folder";
            openFolder.ShowNewFolderButton = false;

            try
            {
                if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    List<string> extensions = new List<string> { "*.asf", "*.wma", "*.wmv", "*.wm", "*.avi", "*.mp*", "*.m4*", "*.wav", "*.mov", "*.mi*" };
                    List<string> videos = new List<string>(Directory.GetFiles(openFolder.SelectedPath).Where(s =>
                        s.Contains(".asf") ||
                        s.Contains(".wma") ||
                        s.Contains(".wmv") ||
                        s.Contains(".wm") ||
                        s.Contains(".avi") ||
                        s.Contains(".mp") ||
                        s.Contains(".m4") ||
                        s.Contains(".wav") ||
                        s.Contains(".mov") ||
                        s.Contains(".mi")));
                    string playlistName = openFolder.SelectedPath;
                    WMPLib.IWMPPlaylist playlist;
                    playlist = axWindowsMediaPlayer1.playlistCollection.newPlaylist(playlistName);

                    bool isLeftOff = false;
                    bool fileExists = false;
                    string episode = "";
                    double startPos = 0.0;
                    if (File.Exists(playlistName + @"\bingewatch.txt"))
                    {
                        fileExists = true;
                        string[] lines = System.IO.File.ReadAllLines(playlistName + @"\bingewatch.txt");
                        episode = lines[0];
                        startPos = Convert.ToDouble(lines[1]);
                    }

                    foreach (string eachVideo in videos)
                    {
                        if (fileExists == true)
                        {
                            if (isLeftOff == true || episode == eachVideo)
                            {
                                isLeftOff = true;
                                WMPLib.IWMPMedia video = axWindowsMediaPlayer1.newMedia(eachVideo);
                                playlist.appendItem(video);
                            }
                        }
                        else
                        {
                            WMPLib.IWMPMedia video = axWindowsMediaPlayer1.newMedia(eachVideo);
                            playlist.appendItem(video);
                        }
                    }

                    axWindowsMediaPlayer1.currentPlaylist = playlist;
                    axWindowsMediaPlayer1.Ctlcontrols.currentPosition = startPos;
                    buttonEnable();
                    buttonPlay.Enabled = false;
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Select a Series folder to play the entire series
        private void entireSeriesMenu_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog openFolder = new FolderBrowserDialog();
            openFolder.Description = "Open series folder";
            openFolder.ShowNewFolderButton = false;

            try
            {
                if (openFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string[] seasons = Directory.GetDirectories(openFolder.SelectedPath);
                    string playlistName = openFolder.SelectedPath;
                    WMPLib.IWMPPlaylist playlist;
                    playlist = axWindowsMediaPlayer1.playlistCollection.newPlaylist(playlistName);
                    double startPos = 0.0;

                    foreach (string eachSeason in seasons)
                    {
                        List<string> extensions = new List<string> { "*.asf", "*.wma", "*.wmv", "*.wm", "*.avi", "*.mp*", "*.m4*", "*.wav", "*.mov", "*.mi*" };
                        List<string> videos = new List<string>(Directory.GetFiles(eachSeason).Where(s =>
                            s.Contains(".asf") ||
                            s.Contains(".wma") ||
                            s.Contains(".wmv") ||
                            s.Contains(".wm") ||
                            s.Contains(".avi") ||
                            s.Contains(".mp") ||
                            s.Contains(".m4") ||
                            s.Contains(".wav") ||
                            s.Contains(".mov") ||
                            s.Contains(".mi")));

                        Console.WriteLine(playlistName);
                        bool isLeftOff = false;
                        bool fileExists = false;
                        string episode = "";
                        if (File.Exists(playlistName + @"\bingewatch.txt"))
                        {
                            fileExists = true;
                            string[] lines = System.IO.File.ReadAllLines(playlistName + @"\bingewatch.txt");
                            episode = lines[0];
                            startPos = Convert.ToDouble(lines[1]);
                        }

                        foreach (string eachVideo in videos)
                        {
                            if (fileExists == true)
                            {
                                if (isLeftOff == true || episode == eachVideo)
                                {
                                    isLeftOff = true;
                                    WMPLib.IWMPMedia video = axWindowsMediaPlayer1.newMedia(eachVideo);
                                    playlist.appendItem(video);
                                }
                            }
                            else
                            {
                                WMPLib.IWMPMedia video = axWindowsMediaPlayer1.newMedia(eachVideo);
                                playlist.appendItem(video);
                            }
                        }
                    }

                    axWindowsMediaPlayer1.currentPlaylist = playlist;
                    axWindowsMediaPlayer1.Ctlcontrols.currentPosition = startPos;
                    buttonEnable();
                    buttonPlay.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = false;
                buttonPause.Enabled = true;
                buttonRW.Enabled = true;
                buttonFF.Enabled = true;

                writeCurrentPos();
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.settings.rate = 1.0;
            }
        }

        private void buttonPause_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = true;
                buttonPause.Enabled = false;
                buttonRW.Enabled = true;
                buttonFF.Enabled = true;

                axWindowsMediaPlayer1.Ctlcontrols.pause();
                writeCurrentPos();
            }
        }

        private void buttonFF_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = true;
                buttonPause.Enabled = true;
                buttonRW.Enabled = true;
                buttonFF.Enabled = false;

                axWindowsMediaPlayer1.settings.rate = 2.0;
                writeCurrentPos();
            }
        }

        private void buttonRW_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = true;
                buttonPause.Enabled = true;
                buttonRW.Enabled = false;
                buttonFF.Enabled = true;

                axWindowsMediaPlayer1.settings.rate = -0.5;
                writeCurrentPos();
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                if (axWindowsMediaPlayer1.currentMedia.sourceURL == axWindowsMediaPlayer1.currentPlaylist.get_Item(axWindowsMediaPlayer1.currentPlaylist.count - 2).sourceURL)
                {
                    buttonNext.Enabled = false;
                }
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.Ctlcontrols.next();
                axWindowsMediaPlayer1.settings.rate = 1.0;
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = false;
                buttonPause.Enabled = true;
                buttonRW.Enabled = true;
                buttonFF.Enabled = true;

                writeCurrentPos();
            }
        }

        private void buttonPrev_Click(object sender, EventArgs e)
        {
            if (axWindowsMediaPlayer1.currentMedia.sourceURL != null)
            {
                if (axWindowsMediaPlayer1.Ctlcontrols.currentPosition > 3)
                {
                    axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
                }
                else
                {
                    if (axWindowsMediaPlayer1.currentMedia.sourceURL == axWindowsMediaPlayer1.currentPlaylist.get_Item(0).sourceURL)
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.currentPosition = 0;
                    }
                    else
                    {
                        axWindowsMediaPlayer1.Ctlcontrols.play();
                        axWindowsMediaPlayer1.Ctlcontrols.previous();
                        buttonNext.Enabled = true;
                    }
                }
                axWindowsMediaPlayer1.Ctlcontrols.play();
                axWindowsMediaPlayer1.settings.rate = 1.0;
                buttonPrev.Enabled = true;
                buttonPlay.Enabled = false;
                buttonPause.Enabled = true;
                buttonRW.Enabled = true;
                buttonFF.Enabled = true;

                writeCurrentPos();
            }
        }

        private void buttonEnable()
        {
            axWindowsMediaPlayer1.settings.rate = 1.0;
            buttonPrev.Enabled = true;
            buttonPlay.Enabled = true;
            buttonPause.Enabled = true;
            buttonRW.Enabled = true;
            buttonNext.Enabled = true;
            buttonFF.Enabled = true;
        }

        private void writeCurrentPos()
        {
            if (axWindowsMediaPlayer1.currentPlaylist.name != "Playlist1")
            {
                string file = axWindowsMediaPlayer1.currentPlaylist.name + @"\bingewatch.txt";
                File.Delete(file);
                StreamWriter outfile = new StreamWriter(file);
                outfile.WriteLine(axWindowsMediaPlayer1.currentMedia.sourceURL);
                outfile.WriteLine(axWindowsMediaPlayer1.Ctlcontrols.currentPosition);
                outfile.Close();
                File.SetAttributes(file, File.GetAttributes(file) | FileAttributes.Normal | FileAttributes.Hidden);
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            axWindowsMediaPlayer1.settings.volume = trackBar1.Value;
        }
    }
}