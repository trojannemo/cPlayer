using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using cPlayer.Properties;
using cPlayer.x360;
using Microsoft.VisualBasic;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Mix;
using Un4seen.Bass.Misc;
using Un4seen.Bass.AddOn.Opus;
using WMPLib;
using AxWMPLib;
using NautilusFREE;
using static cPlayer.YARGSongFileStream;
using Un4seen.Bass.AddOn.Enc;
using System.Runtime.InteropServices;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Windows.Media.Animation;

namespace cPlayer
{
    public partial class frmMain : Form
    {
        private static readonly Color mMenuHighlight = Color.FromArgb(135, 0, 0);
        private static readonly Color mMenuBackground = Color.Black;
        private static readonly Color mMenuText = Color.Gray;
        private static readonly Color mMenuBorder = Color.WhiteSmoke;
        private readonly Color ChartOrange = Color.FromArgb(255, 126, 0);
        private readonly Color ChartBlue = Color.FromArgb(0, 0, 255);
        private readonly Color ChartYellow = Color.FromArgb(242, 226, 0);
        private readonly Color ChartRed = Color.FromArgb(255, 0, 0);
        private readonly Color ChartGreen = Color.FromArgb(0, 255, 0);
        private readonly Color Harm1Color = Color.FromArgb(29, 163, 201);
        private readonly Color Harm2Color = Color.FromArgb(227, 144, 24);
        private readonly Color Harm3Color = Color.FromArgb(168, 74, 4);
        private readonly Color LabelBackgroundColor = Color.FromArgb(127, 40, 40, 40);
        private readonly Color TrackBackgroundColor1 = Color.FromArgb(40, 40, 40);
        private readonly Color TrackBackgroundColor2 = Color.FromArgb(80, 80, 80);
        private readonly Color KaraokeBackgroundColor = Color.Transparent;
        private readonly Color DisabledButtonColor = Color.Gray;
        private readonly Color EnabledButtonColor = Color.White;
        public double VolumeLevel = 12.5;
        private string PlayerConsole = "xbox";
        private double FadeLength = 1.0;
        public bool doAudioDrums = true;
        public bool doAudioBass = true;
        public bool doAudioGuitar = true;
        public bool doAudioKeys = true;
        public bool doAudioVocals = true;
        public bool doAudioBacking = true;
        public bool doAudioCrowd = false;
        public bool doMIDIDrums = true;
        public bool doMIDIBass = true;
        public bool doMIDIGuitar = true;
        public bool doMIDIProKeys = true;
        public bool doMIDIKeys = false;
        public bool doMIDIVocals = false;
        public bool doMIDIHarmonies = true;
        public bool doMIDINameVocals = false;
        public bool doMIDINameProKeys = false;
        public bool doStaticLyrics = false;
        public bool doScrollingLyrics = false;
        public bool doKaraokeLyrics = true;
        public bool doWholeWordsLyrics = true;
        public bool doHarmonyLyrics = true;
        public bool doMIDINameTracks = true;
        public bool doMIDIHighlightSolos = true;
        public bool doMIDIBWKeys = false;
        public bool doMIDIHarm1onVocals = false;
        private readonly Visuals Spectrum = new Visuals();
        public double PlaybackWindow = 3.0;
        public int NoteSizingType = 0;
        private const string AppName = "cPlayer";
        private const int BassBuffer = 1000;
        private readonly NemoTools Tools;
        private readonly DTAParser Parser;
        private int mouseX;
        private int mouseY;
        private string SongToLoad;
        private List<Song> StaticPlaylist;
        private List<Song> Playlist;
        private string PlaylistPath;
        private string PlaylistName;
        private Song ActiveSong;
        private Song PlayingSong;
        private Song NextSong;
        private byte[] CurrentSongAudio;
        private string CurrentSongAudioPath;
        private string CurrentSongArt;
        private string CurrentSongArtBlurred;
        private string CurrentSongMIDI;
        //private string NextSongAudioPath;
        private string NextSongArtPNG;
        private string NextSongArtJPG;
        private string NextSongArtBlurred;
        private string NextSongMIDI;
        public double PlaybackSeconds;
        private double PlaybackSeek;
        private bool reset;
        private readonly string config;
        private int NextSongIndex;
        private int StartingCount;
        private bool isScanning;
        private readonly MIDIStuff MIDITools;
        private Graphics Chart;
        private Bitmap ChartBitmap;
        private readonly string TempFolder;
        public bool CancelWorkers;
        private readonly string EXE;
        private readonly string[] RecentPlaylists;
        private int BassMixer;
        private int BassStream;
        private readonly List<int> BassStreams;
        private int SpectrumID;
        private bool VideoIsPlaying;
        public List<PracticeSection> PracticeSessions;
        private string ImgToUpload;
        private string ImgURL;
        private bool showUpdateMessage;
        private bool AlreadyTried;
        private bool DrewFullChart;
        private double IntroSilence;
        private double OutroSilence;
        private double IntroSilenceNext;
        private double OutroSilenceNext;
        private float SilenceThreshold = 0.25f;
        private bool AlreadyFading;
        private PlaylistSorting SortingStyle;
        private bool isClosing;
        private bool ShowingNotFoundMessage;
        private readonly nTools nautilus;
        private SongData ActiveSongData;
        private AxWindowsMediaPlayer MediaPlayer;
        private string[] opusFiles;
        private string[] oggFiles;
        private string[] mp3Files;
        private string[] wavFiles;
        private string[] cltFiles;
        private string[] m4aFiles;
        private string currentKLIC;
        private string pkgPath;
        private string sngPath;
        private string oggPath;
        private string psarcPath;
        private string ghwtPath;
        public bool isChoosingStems;
        private bool hasNoMIDI;
        private int overrideSongLength;
        private string XML_PATH;
        private string XMA_EXT_PATH;
        private string XMA_PATH;
        private string BandFusePath;
        private readonly NemoFnFParser fnfParser;
        public bool isPlayingM4A;
        public string activeM4AFile;
        private KaraokeOverlayForm KaraokeOverlay;
        private gifOverlay GIFOverlay;

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

        private sealed class DarkRenderer : ToolStripProfessionalRenderer
        {
            public DarkRenderer() : base(new DarkColors()) { }
        }

        private sealed class DarkColors : ProfessionalColorTable
        {
            public override Color MenuItemSelected
            {
                get { return mMenuHighlight; }
            }
            public override Color MenuItemSelectedGradientBegin
            {
                get { return mMenuHighlight; }
            }
            public override Color MenuItemSelectedGradientEnd
            {
                get { return mMenuHighlight; }
            }
            public override Color MenuBorder
            {
                get { return mMenuBorder; }
            }
            public override Color MenuItemBorder
            {
                get { return mMenuBorder; }
            }
            public override Color MenuItemPressedGradientBegin
            {
                get { return mMenuHighlight; }
            }
            public override Color MenuItemPressedGradientEnd
            {
                get { return mMenuHighlight; }
            }
            public override Color MenuItemPressedGradientMiddle
            {
                get { return mMenuHighlight; }
            }
            public override Color CheckBackground
            {
                get { return mMenuHighlight; }
            }
            public override Color CheckPressedBackground
            {
                get { return mMenuHighlight; }
            }
            public override Color CheckSelectedBackground
            {
                get { return mMenuHighlight; }
            }
            public override Color ButtonSelectedBorder
            {
                get { return mMenuHighlight; }
            }
            public override Color SeparatorDark
            {
                get { return mMenuText; }
            }
            public override Color SeparatorLight
            {
                get { return mMenuText; }
            }
            public override Color ImageMarginGradientBegin
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientEnd
            {
                get { return mMenuBackground; }
            }
            public override Color ImageMarginGradientMiddle
            {
                get { return mMenuBackground; }
            }
            public override Color ToolStripDropDownBackground
            {
                get { return mMenuBackground; }
            }
        }

        public frmMain()
        {
            InitializeComponent();                    
            Log("Initialized");
            EXE = "." + "e" + "x" + "e";
            Tools = new NemoTools();
            nautilus = new nTools();
            fnfParser = new NemoFnFParser();
            Parser = new DTAParser();
            var DarkRenderer = new DarkRenderer();
            menuStrip1.Renderer = DarkRenderer;
            PlaylistContextMenu.Renderer = DarkRenderer;
            NotifyContextMenu.Renderer = DarkRenderer;
            VisualsContextMenu.Renderer = DarkRenderer;
            workingContextMenu.Renderer = DarkRenderer;
            //picWorking.Parent = lstPlaylist;
            SongsToAdd = new List<string>();
            Playlist = new List<Song>();
            StaticPlaylist = new List<Song>();
            MIDITools = new MIDIStuff();
            BassStreams = new List<int>();
            opusFiles = new string[20];
            oggFiles = new string[20];
            mp3Files = new string[20];
            m4aFiles = new string[20];
            RecentPlaylists = new string[5];
            PracticeSessions = new List<PracticeSection>();
            MediaPlayer = new AxWindowsMediaPlayer();
            MediaPlayer.CreateControl();
            // Handle the MouseDown event to show the context menu
            MediaPlayer.MouseDownEvent += (object sender, _WMPOCXEvents_MouseDownEvent e) =>
            {
                if (e.nButton == 2) // Right mouse button
                {
                    VisualsContextMenu.Show(Cursor.Position); // Show the context menu at the mouse position
                }
            };            
            KaraokeOverlay = new KaraokeOverlayForm (this)
            {
                StartPosition = FormStartPosition.Manual,
                TopMost = true
            };            
            UpdateOverlayPosition();
            KaraokeOverlay.Show();
            // Hook into relevant events to keep the overlay aligned
            this.Resize += (s, e) => UpdateOverlayPosition();
            this.Move += (s, e) => UpdateOverlayPosition();
            MediaPlayer.Resize += (s, e) => UpdateOverlayPosition();

            for (var i = 0; i < 5; i++)
            {
                RecentPlaylists[i] = "";
            }
            SetDefaultPaths();
            if (!Directory.Exists(Application.StartupPath + "\\playlists\\"))
            {
                Directory.CreateDirectory(Application.StartupPath + "\\playlists\\");
            }
            TempFolder = Application.StartupPath + "\\bin\\temp\\";
            config = Application.StartupPath + "\\bin\\player.config";
            DeleteUsedFiles();
            CreateHiddenFolder();
            ActiveSongData = new SongData();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
            this.UpdateStyles();
        }               

        private bool MonitorApplicationFocus()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            GetWindowThreadProcessId(foregroundWindow, out uint foregroundProcessId);
            uint currentProcessId = (uint)Process.GetCurrentProcess().Id;

            if (foregroundProcessId != currentProcessId)
            {
                // Application is not focused, hide the overlay if visible
                return false;
            }
            else //(foregroundProcessId == currentProcessId)
            {
                // Application is focused, show the overlay if hidden                    
                return true;
            }
        }

        private void UpdateOverlayPosition()
        {
            if (KaraokeOverlay != null && !KaraokeOverlay.IsDisposed)
            {
                KaraokeOverlay.Location = new Point(this.Left + picVisuals.Location.X,this.Top + picVisuals.Location.Y);
                KaraokeOverlay.Size = picVisuals.ClientSize;
            }
            if (GIFOverlay != null && !GIFOverlay.IsDisposed)
            {
                // Position the overlay so it is centered on the ListView
                GIFOverlay.Left = Left + panelPlaylist.Left + ((panelPlaylist.Width - GIFOverlay.Width) / 2);
                GIFOverlay.Top = Top + panelPlaylist.Top + ((panelPlaylist.Height - GIFOverlay.Height) / 2);
            }
        }

        private void SetDefaultPaths()
        {
            CurrentSongArt = Path.GetTempPath() + "play.png";
            CurrentSongArtBlurred = Path.GetTempPath() + "playb.png";
            CurrentSongMIDI = Path.GetTempPath() + "play.mid";
            NextSongArtPNG = Path.GetTempPath() + "next.png";
            NextSongArtJPG = Path.GetTempPath() + "next.jpg";
            NextSongArtBlurred = Path.GetTempPath() + "nextb.png";
            NextSongMIDI = Path.GetTempPath() + "next.mid";
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void CreateHiddenFolder()
        {
            Tools.DeleteFolder(TempFolder, true);
            var di = Directory.CreateDirectory(TempFolder);
            di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
        }

        private void frmMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            Cursor = Cursors.NoMove2D;
            mouseX = MousePosition.X;
            mouseY = MousePosition.Y;
            if (!displayAudioSpectrum.Checked || PlayingSong == null) return;
            SpectrumID++;
            picVisuals.Image = null;
            Spectrum.ClearPeaks();
            Log("Changed audio spectrum visualization to #" + SpectrumID);
        }

        private void frmMain_MouseMove(object sender, MouseEventArgs e)
        {
            if (Cursor != Cursors.NoMove2D) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    Left = Left + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    Left = Left - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }

            if (MousePosition.Y == mouseY) return;
            if (MousePosition.Y > mouseY)
            {
                Top = Top + (MousePosition.Y - mouseY);
            }
            else if (MousePosition.Y < mouseY)
            {
                Top = Top - (mouseY - MousePosition.Y);
            }
            mouseY = MousePosition.Y;
        }

        private void frmMain_MouseUp(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Default;
        }

        private void createNewPlaylist_Click(object sender, EventArgs e)
        {
            Log("createNewPlaylist_Click");
            StartNew(true);
        }

        private void StartNew(bool confirm)
        {
            if (Text.Contains("*") && confirm)
            {
                Log("There are unsaved changes. Confirm?");
                if (MessageBox.Show("You have unsaved changes on the current playlist\nAre you sure you want to do that?",
                        AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    Log("No");
                    return;
                }
                Log("Yes");
            }

            Tools.DeleteFile(CurrentSongArt);
            Tools.DeleteFile(CurrentSongArtBlurred);
            Tools.DeleteFile(CurrentSongMIDI);
            Tools.DeleteFile(NextSongArtPNG);
            Tools.DeleteFile(NextSongArtJPG);
            Tools.DeleteFile(NextSongArtBlurred);
            Tools.DeleteFile(NextSongMIDI);

            PlaylistPath = "";
            PlaylistName = "";
            Playlist = new List<Song>();
            lblUpdates.Text = "";
            lstPlaylist.Items.Clear();
            btnClear.PerformClick();
            ClearAll();
            ClearVisuals();
            ClearLyrics();
            ActiveSong = null;
            PlayingSong = null;
            Text = AppName;
            DeleteUsedFiles();
            Tools.DeleteFile(activeM4AFile);
            activeM4AFile = "";
            Log("Created new " + PlayerConsole + " playlist");
        }

        private void ClearAll()
        {
            reset = true;
            StopPlayback();
            MediaPlayer.Visible = false;
            picPreview.Image = Resources.noart;
            picPreview.Cursor = Cursors.Default;
            ClearLyrics();
            lblSections.Invoke(new MethodInvoker(() => lblSections.Text = ""));
            lblSections.Invoke(new MethodInvoker(() => lblSections.Image = null));
            lblSections.Invoke(new MethodInvoker(() => lblSections.CreateGraphics().Clear(LabelBackgroundColor)));
            picVisuals.Invoke(new MethodInvoker(() => picVisuals.Image = null));
            toolTip1.SetToolTip(picPreview, "");
            toolTip1.SetToolTip(lblArtist, "");
            toolTip1.SetToolTip(lblSong, "");
            toolTip1.SetToolTip(lblAlbum, "");
            lblArtist.Text = "Artist:";
            lblSong.Text = "Song:";
            lblAlbum.Text = "Album:";
            lblGenre.Text = "Genre:";
            lblTrack.Text = "Track #:";
            lblYear.Text = "Year:";
            lblTime.Text = "0:00";
            lblDuration.Text = "0:00";
            lblAuthor.Text = "";
            panelSlider.Left = panelLine.Left;
            SongToLoad = "";
            EnableDisableButtons(false);
            PlaybackSeconds = 0;
            PlaybackTimer.Enabled = false;
            UpdateTime();
            panelSlider.Cursor = Cursors.Default;
            panelLine.Cursor = Cursors.Default;
            btnClear.PerformClick();
            PlayingSong = null;
            MIDITools.Initialize(true);
            AlreadyFading = false;
            reset = false;
        }

        private void lstPlaylist_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Environment.CurrentDirectory = Path.GetDirectoryName(files[0]);
            Log("lstPlaylist_DragDrop " + files.Count() + " file(s)");
            if (files[0].EndsWith(".playlist", StringComparison.Ordinal))
            {
                Log("Drag/dropped playlist file " + files[0]);
                PrepareToLoadPlaylist(files[0]);
                return;
            }

            SongsToAdd = new List<string> { };

            if (xbox360.Checked || bandFuse.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => VariousFunctions.ReadFileType(file) == XboxFileType.STFS).ToList());
            }
            else if (yarg.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "song.ini").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".sng").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".yargsong").ToList());
            }
            else if (pS3.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".pkg").ToList());
            }
            else if (rockSmith.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".psarc").ToList());
            }
            else if (wii.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
            }
            else if (guitarHero.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "song.ini").ToList());
            }
            else if (fortNite.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".fnf").ToList());
            }
            else if (powerGig.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".xml").ToList());
            }

            if (!SongsToAdd.Any())
            {
                Log("Drag/dropped file(s) not valid");
                MessageBox.Show(files.Count() == 1 ? "That's not a valid file" : "Those are not valid files", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (batchSongLoader.IsBusy || songLoader.IsBusy)
            {
                Log("Background worker is busy, not adding song(s) now");
                MessageBox.Show("Please wait while I finish extracting the last file(s)", AppName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            btnClear.PerformClick();
            EnableDisable(false);
            StartingCount = lstPlaylist.Items.Count;
            Log("Songs to add: " + SongsToAdd.Count);
            Log("Starting batch song loader");
            batchSongLoader.RunWorkerAsync();
        }

        private void EnableDisable(bool enabled, bool hide = false)
        {
            fileToolStripMenuItem.Enabled = enabled && !isScanning;
            toolsToolStripMenuItem.Enabled = fileToolStripMenuItem.Enabled;
            optionsToolStripMenuItem.Enabled = fileToolStripMenuItem.Enabled;
            helpToolStripMenuItem.Enabled = fileToolStripMenuItem.Enabled;
            var working = (!hide && !enabled) || isScanning;
            /*if (working)
            {
                InitiateGIFOverlay();
            }
            else if (GIFOverlay != null)
            {
                GIFOverlay.Close();
                GIFOverlay = null;
            }*/
            txtSearch.Enabled = enabled && !isScanning;
        }

        private void InitiateGIFOverlay()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(InitiateGIFOverlay));
                return;
            }

            GIFOverlay = new gifOverlay(this)
            {
                StartPosition = FormStartPosition.Manual,
                TopMost = true,
                Width = 100, // Set overlay size
                Height = 100,
                Owner = this
            };

            UpdateOverlayPosition();

            GIFOverlay.Start();
        }

        private bool ValidateDTAFile(string file, bool message)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file)) return false;
            CreateHiddenFolder();
            Log("Validating DTA file from: " + file);
            if (xbox360.Checked)
            {
                Log("Extracting DTA file from CON file");
                if (!Parser.ExtractDTA(file))
                {
                    if (message)
                    {
                        MessageBox.Show("Something went wrong extracting the songs.dta file, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Log("Failed");
                    return false;
                }
                Log("Success");
            }
            Log("Parsing DTA file");
            if (!Parser.ReadDTA(xbox360.Checked ? Parser.DTA : File.ReadAllBytes(file)) || !Parser.Songs.Any())
            {
                if (message)
                {
                    MessageBox.Show("Something went wrong reading that songs.dta file, can't add to the playlist", AppName,
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Log("Failed");
                return false;
            }
            Log("Success - " + Parser.Songs.Count + " song(s) found in DTA file");
            if (Parser.Songs.Count == 1) return true;
            isScanning = true;
            UpdateNotifyTray();
            return true;
        }

        private bool ValidateNewSong(SongData song, int index, string location, bool scanning, bool message, out Song newsong)
        {
            Log("Validating new song: '" + song.Artist + " - " + song.Name + "'");
            var new_song = new Song
            {
                Name = CleanArtistSong(song.Name),
                Artist = CleanArtistSong(song.Artist),
                Location = location,
                Length = song.Length,
                InternalName = song.InternalName,
                Album = song.Album,
                Year = song.YearReleased,
                Track = song.TrackNumber,
                Genre = Parser.doGenre(song.RawGenre),
                Index = -1,
                AddToPlaylist = true,
                AttenuationValues = song.AttenuationValues.Replace("\t", ""),
                PanningValues = song.PanningValues.Replace("\t", ""),
                Charter = song.ChartAuthor,
                ChannelsDrums = song.ChannelsDrums,
                ChannelsBass = song.ChannelsBass,
                ChannelsGuitar = song.ChannelsGuitar,
                ChannelsKeys = song.ChannelsKeys,
                ChannelsVocals = song.ChannelsVocals,
                ChannelsCrowd = song.ChannelsCrowd,
                ChannelsBacking = song.ChannelsBacking(),
                ChannelsBassStart = song.ChannelsBassStart,
                ChannelsCrowdStart = song.ChannelsCrowdStart,
                ChannelsGuitarStart = song.ChannelsGuitarStart,
                ChannelsKeysStart = song.ChannelsKeysStart,
                ChannelsDrumsStart = song.ChannelsDrumsStart,
                ChannelsVocalsStart = song.ChannelsVocalsStart,
                ChannelsTotal = song.ChannelsTotal,
                DTAIndex = index,
                isRhythmOnBass = song.RhythmBass,
                isRhythmOnKeys = song.RhythmKeys || (song.Name.Contains("Rhythm Version") && !song.RhythmBass),
                hasProKeys = song.ProKeysDiff > 0,
                PSDelay = song.PSDelay,
                yargPath = song.YargPath

            };

            ActiveSongData = song;
            newsong = new_song;
            if (!scanning) return true;
            var exists = Playlist.Any(oldsong => String.Equals(oldsong.Artist, new_song.Artist, StringComparison.InvariantCultureIgnoreCase) &&
                                                 String.Equals(oldsong.Name, new_song.Name, StringComparison.InvariantCultureIgnoreCase));
            if (!exists)
            {
                Log("Song didn't exist in playlist, adding");
                return true;
            }
            if (message)
            {
                MessageBox.Show("Song '" + new_song.Artist + " - " + new_song.Name + "' is already in your playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            Log("Song already existed in playlist, not adding");
            return false;
        }

        private void loadDTA(string dta, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            Log("Loading DTA file " + dta);
            if (!ValidateDTAFile(dta, message))
            {
                Log("Failed to validate that DTA file, skipping");
                return;
            }

            Log("DTA file contains " + Parser.Songs.Count + " song(s)");
            hasNoMIDI = false;
            for (var i = 0; i < Parser.Songs.Count; i++)
            {
                if (CancelWorkers) return;
                var song = prep ? Parser.Songs[ActiveSong.DTAIndex] : (next ? Parser.Songs[NextSong.DTAIndex] : Parser.Songs[i]);
                Log("Processing song '" + song.Artist + " - " + song.Name + "'");

                string internalName = "";
                string PNG = "";
                var EDAT = "";
                string audioPath = "";
                string yarg = "";

                if (wii.Checked)
                {
                    Log("It's Wii playlist - processing as Wii song");
                    var index = song.FilePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
                    song.InternalName = song.FilePath.Substring(index, song.FilePath.Length - index);
                    internalName = song.InternalName;
                    PNG = Path.GetDirectoryName(dta) + "\\" + internalName + "\\gen\\" + internalName + "_keep.png_wii";
                    audioPath = Path.GetDirectoryName(dta).Replace("_meta", "_song") + "\\" + internalName + "\\" + internalName + ".mogg";
                    NextSongMIDI = Path.GetDirectoryName(audioPath) + "\\" + internalName + ".mid";
                    Log("MIDI - " + NextSongMIDI);
                }
                else if (pS3.Checked)
                {
                    Log("It's PS3 playlist - processing as PS3 song");
                    internalName = song.InternalName;
                    PNG = Path.GetDirectoryName(dta) + "\\" + internalName + "\\gen\\" + internalName + "_keep.png_ps3";
                    audioPath = Path.GetDirectoryName(dta) + "\\" + internalName + "\\" + internalName + ".mogg";
                    EDAT = Path.GetDirectoryName(audioPath) + "\\" + internalName + ".mid.edat";
                    NextSongMIDI = EDAT.Replace(".mid.edat", ".mid");
                    Log("EDAT - " + EDAT);
                }
                else //is YARG
                {
                    Log("It's YARG playlist - processing as YARG loose song");
                    internalName = song.InternalName;
                    PNG = Path.GetDirectoryName(dta) + "\\" + internalName + "\\gen\\" + internalName + "_keep.png_xbox";
                    audioPath = Path.GetDirectoryName(dta) + "\\" + internalName + "\\" + internalName + ".mogg";
                    yarg = audioPath.Replace(".mogg", ".yarg_mogg");
                    NextSongMIDI = Path.GetDirectoryName(audioPath) + "\\" + internalName + ".mid";
                    Log("MIDI - " + NextSongMIDI);
                    CurrentSongAudioPath = yarg;
                }
                Log("Audio - " + audioPath);
                Log("Art - " + PNG);
                Log("Internal name - " + internalName);

                if (!File.Exists(audioPath) && !File.Exists(yarg))
                {
                    if (message)
                    {
                        MessageBox.Show("Couldn't locate audio file(s) for song '" + song.Artist + " - " + song.Name + "', can't add to the playlist",
                            AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    Log("Audio file(s) not found");
                    if (next || prep) return;
                    continue;
                }

                if (File.Exists(yarg))
                {
                    if (!nautilus.DecY(yarg, DecryptMode.ToMemory))
                    {
                        if (message)
                        {
                            MessageBox.Show("Song '" + song.Artist + " - " + song.Name + "' is YARG encrypted and I couldn't decrypt it, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Log("YARG audio file is encrypted and failed to decrypt");
                        if (next || prep) return;
                        continue;
                    }
                }
                else
                {
                    var mData = File.ReadAllBytes(audioPath);
                    if (!nautilus.DecM(mData, false, true, DecryptMode.ToMemory))
                    {
                        if (message)
                        {
                            MessageBox.Show("Song '" + song.Artist + " - " + song.Name + "' is encrypted, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Log("Audio file is encrypted and failed to decrypt");
                        if (next || prep) return;
                        continue;
                    }
                }
                Log("Audio file is not encrypted or decrypted successfully");

                Song newSong;
                if (!ValidateNewSong(song, i, string.IsNullOrEmpty(pkgPath) ? dta : pkgPath, scanning, message, out newSong)) continue;

                if (CancelWorkers) return;
                try
                {
                    newSong.BPM = 120;//default in case something fails below
                    if (File.Exists(EDAT) && pS3.Checked)
                    {
                        Log("Decrypting EDAT file");
                        DecryptPS3EDAT(EDAT, message);
                    }
                    if (File.Exists(NextSongMIDI))
                    {
                        hasNoMIDI = false;
                        Log("Reading MIDI file for contents");
                        MIDITools.Initialize(false);
                        if (MIDITools.ReadMIDIFile(NextSongMIDI, false))
                        {
                            newSong.BPM = MIDITools.MIDIInfo.AverageBPM;
                            Log("Success - Average BPM: " + newSong.BPM);
                        }
                        else
                        {
                            Log("Failed");
                        }
                    }
                    else
                    {
                        hasNoMIDI = true;
                        Log("MIDI file not found");
                    }

                    if (next || prep) //only do when processing for playback
                    {
                        Tools.DeleteFile(NextSongArtPNG);
                        Tools.DeleteFile(NextSongArtBlurred);
                        if (File.Exists(PNG))
                        {
                            NextSongArtPNG = Path.GetDirectoryName(PNG) + "\\" + Path.GetFileNameWithoutExtension(PNG) + ".png";
                            NextSongArtBlurred = NextSongArtPNG.Replace(".png", "_b.png");
                            Log("Converting album art from Rock Band format to PNG");
                            var converted = wii.Checked ? Tools.ConvertWiiImage(PNG, NextSongArtPNG, "png", false) :
                                Tools.ConvertRBImage(PNG, NextSongArtPNG, "png", false);
                            if (converted)
                            {
                                Log("Success");
                                Log("Creating album art composite");
                                Tools.CreateBlurredArt(NextSongArtPNG, NextSongArtBlurred);
                                Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                            }
                            else
                            {
                                Log("Failed");
                            }
                        }
                        else
                        {
                            Log("Album art not found");
                        }
                    }

                    long length;
                    ProcessMogg(scanning, song.Length, "", out length);
                    newSong.Length = length;
                    Log("Song length: " + length);

                    if (!scanning) return;

                    Playlist.Add(newSong);
                    Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                    if (isScanning)
                    {
                        ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                    }
                }
                catch (Exception ex)
                {
                    Log("Error loading DTA: " + ex.Message);
                    if (message)
                    {
                        MessageBox.Show("Error reading that file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private bool ValidateINIFile(string file, bool message)
        {
            Log("Validating INI file: " + file);
            if (Parser.ReadINIFile(file))
            {
                Log("Success");
                Log("INI file contains song '" + Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name + "'");
                return true;
            }
            Log("Failed to read INI file");
            if (message)
            {
                MessageBox.Show("Something went wrong reading that INI file, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void loadINI(string input, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var INI = "";
            if (Path.GetExtension(input) == ".yargsong")
            {
                INI = DecryptExtractYARG(input, message, scanning, next, prep);
            }
            else if (Path.GetExtension(input) == ".fnf")
            {
                INI = input;
            }
            Log("Loading INI file " + input);
            if (!ValidateINIFile(INI, message))
            {
                Log("Failed to validate that INI file, skipping");
                return;
            }

            if (CancelWorkers) return;
            var song = Parser.Songs[0];
            Log("Processing song '" + song.Artist + " - " + song.Name + "'");
            Log("It's YARG / CH / PS or FNF playlist - processing as that type of song");
            
            NextSongArtPNG = Path.GetDirectoryName(INI) + "\\album.png";
            NextSongArtJPG = Path.GetDirectoryName(INI) + "\\album.jpg";
            var notesMIDI = Path.GetDirectoryName(INI) + "\\notes.mid";
            var nameMIDI = Path.GetDirectoryName(INI) + "\\" + song.ShortName + ".mid";

            if (File.Exists(nameMIDI))
            {
                NextSongMIDI = nameMIDI; //this is primarily for Fornite Festival songs
            }
            else
            {
                NextSongMIDI = notesMIDI;
            }

            oggFiles = Directory.GetFiles(Path.GetDirectoryName(INI), "*.ogg", SearchOption.TopDirectoryOnly);
            opusFiles = Directory.GetFiles(Path.GetDirectoryName(INI), "*.opus", SearchOption.TopDirectoryOnly);
            m4aFiles = Directory.GetFiles(Path.GetDirectoryName(INI), "*.m4a", SearchOption.TopDirectoryOnly);

            if (!oggFiles.Any() && !opusFiles.Any() && !m4aFiles.Any())
            {
                if (message)
                {
                    MessageBox.Show("Couldn't find audio files for song '" + song.Artist + " - " + song.Name + "', can't add to the playlist",
                        AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            Log("MIDI - " + NextSongMIDI);
            var audio = opusFiles.Any() ? opusFiles.Aggregate("", (current, opus) => current + " " + Path.GetFileName(opus)) : oggFiles.Aggregate("", (current, ogg) => current + " " + Path.GetFileName(ogg));
            if (m4aFiles.Any())
            {
                foreach (var m4a in m4aFiles)
                {
                    if (Path.GetFileName(m4a) != "preview.m4a")
                    {
                        audio = m4a;
                        break;
                    }
                }
            }
            Log("Audio - " + audio);
            Log("Art - " + (File.Exists(NextSongArtPNG) ? NextSongArtPNG : NextSongArtJPG));

            Song newSong;
            if (!ValidateNewSong(song, 0, string.IsNullOrEmpty(sngPath) ? INI : sngPath, scanning, message, out newSong)) return;
            newSong.Location = input;//for .yargsong files

            if (CancelWorkers) return;
            try
            {
                newSong.BPM = 120;//default in case something fails below
                if (File.Exists(NextSongMIDI))
                {
                    Log("MIDI file found");
                    Log("Reading MIDI file for contents");
                    MIDITools.Initialize(false);
                    if (MIDITools.ReadMIDIFile(NextSongMIDI, false))
                    {
                        hasNoMIDI = false;
                        newSong.BPM = MIDITools.MIDIInfo.AverageBPM;
                        Log("Success - Average BPM: " + newSong.BPM);
                    }
                    else
                    {
                        Log("Failed");
                    }
                }
                else
                {
                    hasNoMIDI = true;
                    Log("MIDI file not found");
                }

                if (next || prep) //only do when processing for playback
                {
                    Tools.DeleteFile(NextSongArtBlurred);
                    if (File.Exists(NextSongArtPNG) || File.Exists(NextSongArtJPG))
                    {
                        if (!File.Exists(NextSongArtPNG) && File.Exists(NextSongArtJPG))
                        {
                            NextSongArtBlurred = NextSongArtBlurred.Replace(".png", ".jpg");
                        }
                        Log("Album art found");
                        Log("Creating album art composite");
                        Tools.CreateBlurredArt(File.Exists(NextSongArtPNG) ? NextSongArtPNG : NextSongArtJPG, NextSongArtBlurred);
                        Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                    }
                    else
                    {
                        Log("Album art not found");
                    }
                }

                newSong.Length = song.Length;
                if (newSong.Length <= 0)
                {
                    if (opusFiles.Any())
                    {
                        foreach (var opus in opusFiles)
                        {
                            long length;
                            ProcessMogg(true, 0, opus, out length);
                            if (length > newSong.Length)
                            {
                                newSong.Length = length;
                            }
                        }
                    }
                    foreach (var ogg in oggFiles)
                    {
                        nautilus.NextSongOggData = File.ReadAllBytes(ogg);
                        long length;
                        ProcessMogg(true, 0, "", out length);
                        if (length > newSong.Length)
                        {
                            newSong.Length = length;
                        }
                    }
                }
                Log("Song length: " + newSong.Length);

                if (!scanning)
                {           
                    if (m4aFiles.Any())
                    {
                        PrepareFortniteM4A();
                    }
                    return;
                }

                Playlist.Add(newSong);
                Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                if (isScanning)
                {
                    ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                }
            }
            catch (Exception ex)
            {
                Log("Error loading INI: " + ex.Message);
                if (message)
                {
                    MessageBox.Show("Error reading that file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        private void PrepareFortniteM4A()
        {
            var audio = "";
            activeM4AFile = "";
            foreach (var m4a in m4aFiles)
            {
                if (Path.GetFileName(m4a) != "preview.m4a")
                {
                    audio = m4a;
                    break;
                }
            }
            Bass.BASS_ChannelFree(BassStream);
            BassStream = fnfParser.m4aToBassStream(audio, 10);//always 10 channels, no preview allowed here
            if (BassStream == 0)
            {
                MessageBox.Show("File '" + audio + "' is not a valid input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Bass.BASS_ChannelFree(BassStream);
                return;
            }

            var tempFile = Path.GetTempFileName();

            //this next bit is an ugly hack but temporary until Ian @ BASS implements a better solution
            //writes the raw opus data to a temporary wav file (fastest encoder) and then reads it back in the StartPlayback function
            BassEnc.BASS_Encode_Start(BassStream, tempFile, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE, null, IntPtr.Zero);
            while (true)
            {
                var buffer = new byte[20000];
                var c = Bass.BASS_ChannelGetData(BassStream, buffer, buffer.Length);
                if (c <= 0) break;
            }
            Bass.BASS_ChannelFree(BassStream);

            BassStream = Bass.BASS_StreamCreateFile(tempFile, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            if (BassStream == 0)
            {
                MessageBox.Show("That is not a valid .m4a input file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                File.Delete(tempFile);
                Bass.BASS_ChannelFree(BassStream);
                return;
            }

            activeM4AFile = tempFile;
        }

        private void ProcessMogg(bool scanning, long in_length, string file, out long Length)
        {
            Length = in_length;
            if (scanning && in_length == 0)
            {
                Log("Processing audio file for length");
                try
                {
                    var stream = 0;
                    if (opusFiles.Any())
                    {
                        stream = BassOpus.BASS_OPUS_StreamCreateFile(file, 0, File.ReadAllBytes(file).Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                    }
                    else
                    {
                        stream = Bass.BASS_StreamCreateFile(nautilus.GetOggStreamIntPtr(true), 0L, nautilus.NextSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                    }
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Length = (int)(totaltime * 1000);
                    if (!opusFiles.Any())
                    {
                        nautilus.ReleaseStreamHandle(true);
                    }
                }
                catch (Exception ex)
                {
                    Log("Error processing audio file: " + ex.Message);
                }
            }
        }

        private void loadCON(string con, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            Log("Loading CON file " + con);
            if (!ValidateDTAFile(con, message))
            {
                Log("Failed to validate that CON file, skipping");
                return;
            }
            hasNoMIDI = false;
            if (message && isScanning)
            {
                message = false;
            }
            var xPackage = new STFSPackage(con);
            if (!xPackage.ParseSuccess)
            {
                Log("There was an error parsing that " + (Parser.Songs.Count > 1 ? "pack" : "song"));
                if (message)
                {
                    MessageBox.Show("There was an error parsing that " + (Parser.Songs.Count > 1 ? "pack" : "song"), AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            Log("CON file contains " + Parser.Songs.Count + " song(s)");
            for (var i = 0; i < Parser.Songs.Count; i++)
            {
                if (CancelWorkers) return;
                var song = prep ? Parser.Songs[ActiveSong.DTAIndex] : (next ? Parser.Songs[NextSong.DTAIndex] : Parser.Songs[i]);
                Log("Processing song '" + song.Artist + " - " + song.Name + "'");

                Song newsong;
                if (!ValidateNewSong(song, i, con, scanning, message, out newsong)) continue;
                if (ActiveSongData == null || prep)
                {
                    ActiveSongData = song;
                }

                if (CancelWorkers) return;
                var internalname = song.InternalName;
                Log("Internal name: " + internalname);
                try
                {
                    Log("Searching for mogg file");
                    var xFile = xPackage.GetFile("songs/" + internalname + "/" + internalname + ".mogg");
                    if (xFile == null)
                    {
                        Log("Mogg file not found");
                        xPackage.CloseIO();
                        return;
                    }
                    Log("Success");
                    Log("Extracting mogg file");
                    var mData = xFile.Extract();
                    if (mData == null || mData.Length == 0)
                    {
                        Log("Failed");
                        xPackage.CloseIO();
                        return;
                    }
                    Log("Success");

                    Tools.DeleteFile(NextSongMIDI);
                    newsong.BPM = 120;//default in case something fails below
                    xFile = xPackage.GetFile("songs/" + internalname + "/" + internalname + ".mid");
                    Log("Searching for MIDI file");
                    if (xFile != null)
                    {
                        Log("Success");
                        Log("Extracting MIDI file");
                        if (xFile.ExtractToFile(NextSongMIDI))
                        {
                            Log("Reading MIDI file for contents");
                            MIDITools.Initialize(false);
                            if (MIDITools.ReadMIDIFile(NextSongMIDI, false))
                            {
                                newsong.BPM = MIDITools.MIDIInfo.AverageBPM;
                                Log("Success - Average BPM: " + newsong.BPM);
                            }
                            else
                            {
                                Log("Failed");
                            }
                        }
                        else
                        {
                            Log("Failed");
                        }
                    }
                    else
                    {
                        Log("MIDI file not found");
                    }

                    if (next || prep) //only do when processing for playback
                    {
                        Tools.DeleteFile(NextSongArtPNG);
                        Tools.DeleteFile(NextSongArtBlurred);
                        xFile = xPackage.GetFile("songs/" + internalname + "/gen/" + internalname + "_keep.png_xbox");
                        Log("Searching for album art file");
                        if (xFile != null)
                        {
                            var art = Path.GetTempPath() + "next.png_xbox";
                            Tools.DeleteFile(art);

                            if (xFile.ExtractToFile(art))
                            {
                                Log("Converting album art from Rock Band format to PNG");
                                var converted = Tools.ConvertRBImage(art, NextSongArtPNG, "png", true);
                                if (converted)
                                {
                                    Log("Success");
                                    Log("Creating album art composite");
                                    Tools.CreateBlurredArt(NextSongArtPNG, NextSongArtBlurred);
                                    Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                                }
                                else
                                {
                                    Log("Failed");
                                }
                            }
                            else
                            {
                                Log("Failed");
                            }
                        }
                        else
                        {
                            Log("Album art file not found");
                        }
                    }

                    if (CancelWorkers) return;
                    if (!nautilus.DecM(mData, false, true, DecryptMode.ToMemory))
                    {
                        if (message && Parser.Songs.Count == 1)
                        {
                            MessageBox.Show("Song '" + song.Artist + " - " + song.Name + "' is encrypted, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        Log("Audio file is encrypted and failed to decrypt");
                        xPackage.CloseIO();
                        return;
                    }

                    Log("Audio file is not encrypted or decrypted successfully");

                    long length;
                    ProcessMogg(scanning, song.Length, "", out length);
                    newsong.Length = length;
                    Log("Song length: " + length);

                    if (!scanning)
                    {
                        xPackage.CloseIO();
                        return;
                    }

                    Playlist.Add(newsong);
                    Log("Added '" + newsong.Artist + " - " + newsong.Name + "' to playlist");
                    if (isScanning)
                    {
                        ShowUpdate("Added '" + newsong.Artist + " - " + newsong.Name + "'");
                    }
                }
                catch (Exception ex)
                {
                    Log("Error loading CON: " + ex.Message);
                    if (message)
                    {
                        MessageBox.Show("Error reading that file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            try
            {
                xPackage.CloseIO();
            }
            catch (Exception ex)
            {
                Log("Error closing STFS file IO: " + ex.Message);
            }
        }

        private void DecryptPS3EDAT(string edat, bool message)
        {
            if (!File.Exists(edat)) return;
            Log("Decrypting EDAT to MIDI");
            Tools.DeleteFile(NextSongMIDI);
            if (!Tools.DecryptEdat(edat, NextSongMIDI, currentKLIC))
            {
                Log("Failed");
            }
            if (File.Exists(NextSongMIDI))
            {
                Log("Decrypted to MIDI successfully");
            }
            else
            {
                Log("Decrypting to MIDI failed");
                if (message)
                {
                    MessageBox.Show("Failed to decrypt that song's EDAT file to a usable MIDI", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
        }

        private void batchSongLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Batch song loader working");
            if (xbox360.Checked)
            {
                Log("loadCON: " + SongsToAdd[0]);
                loadCON(SongsToAdd[0], !isScanning);
            }
            else if (yarg.Checked)
            {
                if (Path.GetExtension(SongsToAdd[0]) == ".yargsong")
                {
                    pkgPath = "";
                    sngPath = SongsToAdd[0];
                    Log("DecryptExtractYARG: " + SongsToAdd[0]);
                    loadINI(SongsToAdd[0], !isScanning);
                }
                else if (Path.GetExtension(SongsToAdd[0]) == ".sng")
                {
                    sngPath = SongsToAdd[0];
                    Log("loadSNG: " + SongsToAdd[0]);
                    loadSNG(SongsToAdd[0], !isScanning);
                }
                else if (Path.GetFileName(SongsToAdd[0]) == "songs.dta")
                {
                    pkgPath = "";
                    sngPath = "";
                    Log("loadDTA: " + SongsToAdd[0]);
                    loadDTA(SongsToAdd[0], !isScanning);
                }
                else
                {
                    sngPath = "";
                    Log("loadINI: " + SongsToAdd[0]);
                    loadINI(SongsToAdd[0], !isScanning);
                }
            }
            else if (rockSmith.Checked)
            {
                Log("loadPSARC: " + SongsToAdd[0]);
                loadPSARC(SongsToAdd[0], !isScanning);
            }
            else if (guitarHero.Checked)
            {
                ghwtPath = SongsToAdd[0];
                Log("loadGHWT: " + SongsToAdd[0]);
                loadGHWT(SongsToAdd[0], !isScanning);
            }
            else if (fortNite.Checked)
            {
                Log("loadFNF: " + SongsToAdd[0]);
                loadINI(SongsToAdd[0], !isScanning);
            }
            else if (powerGig.Checked)
            {
                Log("ExtractXMA: " + SongsToAdd[0]);
                ExtractXMA(SongsToAdd[0], !isScanning);
            }
            else if (bandFuse.Checked)
            {
                BandFusePath = SongsToAdd[0];
                Log("ExtractBandFuse: " + SongsToAdd[0]);
                ExtractBandFuse(SongsToAdd[0], !isScanning);
            }
            else
            {
                if (pS3.Checked && Path.GetExtension(SongsToAdd[0]) == ".pkg")
                {
                    pkgPath = SongsToAdd[0];
                    Log("loadPKG: " + SongsToAdd[0]);
                    loadPKG(SongsToAdd[0], !isScanning);
                }
                else
                {
                    pkgPath = "";
                    ActiveSong.yargPath = "";
                    Log("loadDTA: " + SongsToAdd[0]);
                    loadDTA(SongsToAdd[0], !isScanning);
                }
            }
            SongsToAdd.RemoveAt(0);
        }

        private void ExtractBandFuse(string file, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var temp = Application.StartupPath + "\\temp\\";
            Tools.DeleteFolder(temp, true);//clean up before starting with new song
            Directory.CreateDirectory(temp);
            
            var songFuse = Application.StartupPath + "\\bin\\songfuse.exe";
            if (!File.Exists(songFuse))
            {
                MessageBox.Show("Could not find songfuse.exe in the \\bin\\ folder, can't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            var package = new STFSPackage(file);
            if (package.Header.TitleID != (uint)1296435155)
            {
                package.CloseIO();
                Log("Right kind of file but invalid game ID, skipping...");
                if (message)
                {
                    MessageBox.Show("This is the right kind of file but it has an invalid game ID, this is not a match for BandFuse, skipping this file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            var header = package.Header.Description;
            var index = header.IndexOf(" - ");

            SongData song = new SongData();
            song.Initialize();
            song.Name = header.Substring(0, index).Trim();
            song.Artist = header.Substring(index + 3, header.Length - (index + 3)).Trim();
            song.ChartAuthor = "BandFuse";
            Parser.Songs = new List<SongData> { song };

            Song newSong;
            if (!ValidateNewSong(song, 0, file, scanning, message, out newSong)) return;                    

            if (scanning)
            {
                package.CloseIO();

                Playlist.Add(newSong);
                Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                if (isScanning)
                {
                    ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                }

                return;
            }

            if (!package.ExtractPayload(temp, true, false))
            {
                MessageBox.Show("Failed to extract file contents, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                package.CloseIO();
                return;
            }
            package.CloseIO();

            if (CancelWorkers) return;
            var art = temp + "album.png";
            var album_art = "";
            var xpr = Directory.GetFiles(temp, "*.xpr", SearchOption.AllDirectories);
            if (xpr.Count() > 0)
            {
                if (!Tools.ConvertBandFuse("texture", xpr[0], art))
                {
                    MessageBox.Show("Failed to convert album art texture", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    album_art = art;
                }
            }

            NextSongArtPNG = album_art;
            NextSongMIDI = "";
            hasNoMIDI = true;

            if (next || prep) //only do when processing for playback
            {
                Tools.DeleteFile(NextSongArtBlurred);
                if (File.Exists(NextSongArtPNG))
                {
                    Log("Album art found");
                    Log("Creating album art composite");
                    Tools.CreateBlurredArt(NextSongArtPNG, NextSongArtBlurred);
                    Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                }
                else
                {
                    Log("Album art not found");
                }
            }         

            cltFiles = Directory.GetFiles(temp, "*.clt", SearchOption.AllDirectories);
            if (!cltFiles.Any())
            {
                MessageBox.Show("No audio files found to decrypt, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }                      

            var didBacking = false;
            var didBass = false;
            var didDrums = false;
            var didGuitar1 = false;
            var didGuitar2 = false;
            var didVocals = false;

            if (CancelWorkers) return;
            foreach (var clt in cltFiles)
            {
                if (clt.EndsWith("\\back\\audio.clt") && !didBacking)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "backing.wav");
                    didBacking = true;
                }
                else if (clt.EndsWith("\\bass\\audio.clt") && !didBass)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "bass.wav");
                    didBass = true;
                }
                else if (clt.EndsWith("\\drums\\audio.clt") && !didDrums)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "drums.wav");
                    didDrums = true;
                }
                else if (clt.EndsWith("\\gtr1\\audio.clt") && !didGuitar1)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "guitar_1.wav");
                    didGuitar1 = true;
                }
                else if (clt.EndsWith("\\gtr2\\audio.clt") && !didGuitar2)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "guitar_2.wav");
                    didGuitar2 = true;
                }
                else if (clt.EndsWith("\\vox\\audio.clt") && !didVocals)
                {
                    Tools.ConvertBandFuse("audio", clt, temp + "vocals.wav");
                    didVocals = true;
                }
            }

            BandFusePath = temp;
            wavFiles = Directory.GetFiles(temp, "*.wav", SearchOption.TopDirectoryOnly);
            if (wavFiles.Any())
            {
                try
                {
                    //the metadata doesn't contain song data, let's get it from the wav files
                    var stream = Bass.BASS_StreamCreateFile(wavFiles[0], 0L, File.ReadAllBytes(wavFiles[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Parser.Songs[0].Length = (int)(totaltime * 1000);
                    overrideSongLength = Parser.Songs[0].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:\n" + ex.Message + "\n\nBASS status:\n" + Bass.BASS_ErrorGetCode());
                }

            }
            else
            {
                if (!message) return;
                MessageBox.Show("Failed to extract audio streams from XMA file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void loadPKG(string pkg, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var folder = Application.StartupPath + "\\temp\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var outFolder = folder + Path.GetFileNameWithoutExtension(pkg).Replace(" ", "").Replace("-", "").Replace("_", "").Trim() + "_ex";
            Tools.DeleteFolder(outFolder, true);
            if (!Tools.ExtractPKG(pkg, outFolder, out currentKLIC))
            {
                MessageBox.Show("Failed to process that PKG file, can't play it", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var DTA = Directory.GetFiles(outFolder, "songs.dta", SearchOption.AllDirectories);
            if (DTA.Count() == 0)
            {
                MessageBox.Show("No songs.dta file found, can't play that PKG file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            pkgPath = pkg;
            loadDTA(DTA[0], message, scanning, next, prep);
        }

        private void loadSNG(string sng, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var outFolder = Application.StartupPath + "\\temp";
            if (Directory.Exists(outFolder))
            {
                Tools.DeleteFolder(outFolder, true);
            }
            Directory.CreateDirectory(outFolder);

            if (!Tools.ExtractSNG(sng, outFolder))
            {
                MessageBox.Show("Failed to process that SNG file, can't play it", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var INI = Directory.GetFiles(outFolder, "song.ini", SearchOption.TopDirectoryOnly);
            if (INI.Count() == 0)
            {
                MessageBox.Show("No song.ini file found, can't play that SNG file", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            sngPath = sng;
            loadINI(INI[0], message, scanning, next, prep);
        }

        private void ExtractXMA(string xml, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            XML_PATH = xml;
            var XMAs = Directory.GetFiles(Path.GetDirectoryName(XML_PATH), "*.xma", SearchOption.TopDirectoryOnly);
            var ogXMA = XMAs[0];

            Log("Loading XML file " + xml);
            if (!ValidateXMLFile(xml, message))
            {
                Log("Failed to validate that XML file, skipping");
                return;
            }

            var album_art = "";
            var albumPNG = Path.GetDirectoryName(XML_PATH) + "\\album.png";
            var albumJPG = Path.GetDirectoryName(XML_PATH) + "\\album.jpg";
            if (File.Exists(albumPNG))
            {
                album_art = albumPNG;
            }
            else if (File.Exists(albumJPG))
            {
                album_art = albumJPG;
            }

            if (CancelWorkers) return;
            var song = Parser.Songs[0];
            Log("Processing song '" + song.Artist + " - " + song.Name + "'");
            Log("It's Power Gig playlist - processing as that type of song");

            NextSongArtPNG = album_art;
            NextSongMIDI = "";
            hasNoMIDI = true;

            Log("No MIDI file for this type of song!");
            Log("Audio - WAV FILES");
            Log("Art - " + (File.Exists(NextSongArtPNG) ? NextSongArtPNG : "NONE"));

            Song newSong;
            if (!ValidateNewSong(song, 0, xml, scanning, message, out newSong)) return;

            if (CancelWorkers) return;
            try
            {
                if (next || prep) //only do when processing for playback
                {
                    Tools.DeleteFile(NextSongArtBlurred);
                    if (File.Exists(NextSongArtPNG))
                    {
                        Log("Album art found");
                        Log("Creating album art composite");
                        Tools.CreateBlurredArt(NextSongArtPNG, NextSongArtBlurred);
                        Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                    }
                    else
                    {
                        Log("Album art not found");
                    }
                }               

                if (scanning)
                {
                    Playlist.Add(newSong);
                    Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                    if (isScanning)
                    {
                        ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Log("Error loading XML: " + ex.Message);
                if (message)
                {
                    MessageBox.Show("Error reading that file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return;
            }

            var temp = Application.StartupPath + "\\temp\\";
            Tools.DeleteFolder(temp, true);
            Directory.CreateDirectory(temp);
            XMA_EXT_PATH = temp + (string.IsNullOrEmpty(Parser.Songs[0].InternalName) ? "temp" : Parser.Songs[0].InternalName);
            if (!Directory.Exists(XMA_EXT_PATH))
            {
                Directory.CreateDirectory(XMA_EXT_PATH);
            }
            XMA_PATH = XMA_EXT_PATH + "\\" + Path.GetFileNameWithoutExtension(XML_PATH) + "_all.xma";
                        

            if (!File.Exists(ogXMA))
            {
                MessageBox.Show("Expected XMA file '" + Path.GetFileName(ogXMA) + "' not found, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            File.Copy(ogXMA, XMA_PATH, true);

            if (!Tools.XMASH(XMA_PATH))
            {
                MessageBox.Show("Failed to extract the audio streams from '" + Path.GetFileName(XMA_PATH) + "' - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Tools.DeleteFile(XMA_EXT_PATH + "\\xmash.exe");
                return;
            }
            Tools.DeleteFile(XMA_EXT_PATH + "\\xmash.exe");
            Tools.DeleteFile(XMA_PATH);

            var sepXMAs = Directory.GetFiles(XMA_EXT_PATH, "*.xma", SearchOption.TopDirectoryOnly);
            if (sepXMAs.Count() == 0)
            {
                MessageBox.Show("Failed to extract the audio streams from '" + Path.GetFileName(XMA_PATH) + "' - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            foreach (var xma in sepXMAs)
            {
                if (!Tools.toWAV(xma))
                {
                    MessageBox.Show("Failed to convert XMA file to WAV - can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Tools.DeleteFile(XMA_EXT_PATH + "\\towav.exe");
                    return;
                }
            }
            Tools.DeleteFile(XMA_EXT_PATH + "\\towav.exe");

            foreach (var xma in sepXMAs)
            {
                Tools.DeleteFile(xma);
            }

            //rename the files based on assumed order
            wavFiles = Directory.GetFiles(XMA_EXT_PATH, "*.wav");
            if (wavFiles.Any())
            {
                try
                {
                    //the metadata doesn't contain song data, let's get it from the wav files
                    var stream = Bass.BASS_StreamCreateFile(wavFiles[0], 0L, File.ReadAllBytes(wavFiles[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                    var len = Bass.BASS_ChannelGetLength(stream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                    Parser.Songs[0].Length = (int)(totaltime * 1000);
                    overrideSongLength = Parser.Songs[0].Length;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error:\n" + ex.Message + "\n\nBASS status:\n" + Bass.BASS_ErrorGetCode());
                }

            }
            else
            {
                if (!message) return;
                MessageBox.Show("Failed to extract audio streams from XMA file, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            for (var i = wavFiles.Count(); i >= 0; i--)
            {
                if (i == wavFiles.Count() - 1)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\song.wav");
                }
                else if (i == wavFiles.Count() - 2)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\guitar.wav");
                }
                else if (i == wavFiles.Count() - 3)
                {
                    File.Move(wavFiles[i], XMA_EXT_PATH + "\\vocals.wav");
                }
                else if (i <= wavFiles.Count() - 4)
                {
                    switch (i)
                    {
                        case 0:
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_1.wav");
                            i = -1;
                            break;
                        case 1:
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_2.wav");
                            i = -1;
                            break;
                        case 2:
                            File.Move(wavFiles[i - 3], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_2.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_3.wav");
                            i = -1;
                            break;
                        case 3:
                            File.Move(wavFiles[i - 3], XMA_EXT_PATH + "\\drums_1.wav");
                            File.Move(wavFiles[i - 2], XMA_EXT_PATH + "\\drums_2.wav");
                            File.Move(wavFiles[i - 1], XMA_EXT_PATH + "\\drums_3.wav");
                            File.Move(wavFiles[i], XMA_EXT_PATH + "\\drums_4.wav");
                            i = -1;
                            break;
                    }
                }
            }            
        }

        private void loadPSARC(string psarc, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var outFolder = Application.StartupPath + "\\temp";
            if (Directory.Exists(outFolder))
            {
                Tools.DeleteFolder(outFolder, true);
            }
            Directory.CreateDirectory(outFolder);

            var rsFolder = outFolder + "\\" + Path.GetFileNameWithoutExtension(psarc) + "_psarc_RS2014_Pc";

            if (!Tools.ExtractPsArc(psarc, outFolder, rsFolder))
            {
                MessageBox.Show("Failed to process that PsArc file, can't play it", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            PlayARCFolder(rsFolder, psarc, message, scanning, next, prep);
        }

        private bool ValidateHSANFile(string file, bool message)
        {
            Log("Validating HSAN file: " + file);
            if (Parser.ReadHSANFile(file))
            {
                Log("Success");
                Log("HSAN file contains song '" + Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name + "'");
                return true;
            }
            Log("Failed to read HSAN file");
            if (message)
            {
                MessageBox.Show("Something went wrong reading that HSAN file, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private bool ValidateXMLFile(string file, bool message)
        {
            Log("Validating XML file: " + file);
            if (Parser.ReadXMLFile(file))
            {
                Log("Success");
                Log("XML file contains song '" + Parser.Songs[0].Artist + " - " + Parser.Songs[0].Name + "'");
                return true;
            }
            Log("Failed to read XML file");
            if (message)
            {
                MessageBox.Show("Something went wrong reading that XML file, can't add to the playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void PlayARCFolder(string folder, string psarc, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var audioFolder = folder + "\\audio\\windows\\";
            var OggFiles = Directory.GetFiles(audioFolder, "*.ogg");
            var artFolder = folder + "\\gfxassets\\album_art\\";
            var pngFiles = Directory.GetFiles(artFolder, "*.png");
            var ddsFiles = Directory.GetFiles(artFolder, "*.dds");
            var album_art = "";
            var manifestFolder = folder + "\\manifests\\";
            var metadataFiles = Directory.GetFiles(manifestFolder, "*.hsan", SearchOption.AllDirectories);
            var sngFolder = folder + "\\songs\\bin\\generic\\";
            var sngFiles = Directory.GetFiles(sngFolder, "*.sng");
            var HSAN = "";

            //bool hasBass = false;
            //bool hasLead = false;
            bool hasRhythm = false;
            bool hasVocals = false;
            
            if (metadataFiles.Count() > 0)
            {
                HSAN = metadataFiles[0];
            }
            Log("Loading HSAN file " + HSAN);
            if (!ValidateHSANFile(HSAN, message))
            {
                Log("Failed to validate that HSAN file, skipping");
                return;
            }

            if (pngFiles.Count() > 0)
            {
                album_art = pngFiles[0];
            }
            else if (ddsFiles.Count() > 0)
            {
                for (var i = 0; i < ddsFiles.Count(); i++)
                {
                    if (ddsFiles[i].Contains("256"))
                    {
                        album_art = ddsFiles[i];
                        break;
                    }                    
                }
            }

            if (sngFiles.Count() > 0)
            {
                foreach (var sng in sngFiles)
                {
                    if (sng.Contains("_bass.sng"))
                    {
                        //hasBass = true;
                    }
                    else if (sng.Contains("_lead.sng"))
                    {
                        //hasLead = true;
                    }
                    else if (sng.Contains("_rhythm.sng"))
                    {
                        hasRhythm = true;
                    }
                    else if (sng.Contains("_vocals.sng"))
                    {
                        hasVocals = true;
                    }
                }
            }

            if (hasVocals)
            {
                Parser.Songs[0].VocalsDiff = 1;
            }
            else if (hasRhythm)
            {
                Parser.Songs[0].RhythmBass = true;
            }

            var bigOgg = "";
            var sortedOggs = from f in OggFiles orderby new FileInfo(f).Length ascending select f;
            foreach (var sorted in sortedOggs)
            {
                bigOgg = sorted.ToString();
            }
            var songAudio = Path.GetDirectoryName(audioFolder) + "\\song.ogg";
            Tools.MoveFile(bigOgg, songAudio);
            
            if (CancelWorkers) return;
            var song = Parser.Songs[0];
            Log("Processing song '" + song.Artist + " - " + song.Name + "'");
            Log("It's RS2014 playlist - processing as that type of song");

            NextSongArtPNG = album_art;
            NextSongMIDI = "";
            hasNoMIDI = true;

            if (!OggFiles.Any())
            {
                MessageBox.Show("Couldn't find audio files for song '" + song.Artist + " - " + song.Name + "', can't add to the playlist",
                        AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            Log("No MIDI file for this type of song!");
            Log("Audio - OGG FILE");
            Log("Art - " + (File.Exists(NextSongArtPNG) ? NextSongArtPNG : "NONE"));

            Song newSong;
            if (!ValidateNewSong(song, 0, psarc, scanning, message, out newSong)) return;

            if (CancelWorkers) return;
            try
            {
                hasNoMIDI = true;

                if (next || prep) //only do when processing for playback
                {
                    Tools.DeleteFile(NextSongArtBlurred);
                    if (File.Exists(NextSongArtPNG))
                    {                        
                        Log("Album art found");
                        Log("Creating album art composite");
                        Tools.CreateBlurredArt(NextSongArtPNG, NextSongArtBlurred);
                        Log(File.Exists(NextSongArtBlurred) ? "Success" : "Failed");
                    }
                    else
                    {
                        Log("Album art not found");
                    }
                }

                nautilus.NextSongOggData = File.ReadAllBytes(songAudio);
                newSong.Length = song.Length;
                long length;
                ProcessMogg(true, 0, "", out length);
                if (length > newSong.Length)
                {
                    newSong.Length = length;
                }
                Log("Song length: " + newSong.Length);

                if (scanning)
                {
                    Playlist.Add(newSong);
                    Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                    if (isScanning)
                    {
                        ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error loading HSAN: " + ex.Message);
                if (message)
                {
                    MessageBox.Show("Error reading that file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            psarcPath = audioFolder;
            MoveSongFiles();
            PrepareForPlayback();
        }

        private void loadGHWT(string ini, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            var outFolder = Application.StartupPath + "\\temp";
            if (Directory.Exists(outFolder))
            {
                Tools.DeleteFolder(outFolder, true);
            }
            Directory.CreateDirectory(outFolder);
                        
            Parser.ReadGHWTDEFile(ini);

            if (CancelWorkers) return;
            var song = Parser.Songs[0];

            Song newSong;
            if (!ValidateNewSong(song, 0, ini, scanning, message, out newSong)) return;

            if (scanning)
            {
                Playlist.Add(newSong);
                Log("Added '" + newSong.Artist + " - " + newSong.Name + "' to playlist");
                if (isScanning)
                {
                    ShowUpdate("Added '" + newSong.Artist + " - " + newSong.Name + "'");
                }
                return;
            }

            var albumPNG = Path.GetDirectoryName(ini) + "\\Content\\album.png";
            var albumJPG = Path.GetDirectoryName(ini) + "\\Content\\album.jpg";

            if (File.Exists(albumPNG))
            {
                NextSongArtPNG = albumPNG;
            }
            else if (File.Exists(albumJPG))
            {
                NextSongArtPNG = albumJPG;
            }
            NextSongMIDI = "";
            hasNoMIDI = true;

            var temp = Application.StartupPath + "\\temp\\";
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
            var ext_path = temp + (string.IsNullOrEmpty(Parser.Songs[0].InternalName) ? "temp" : Parser.Songs[0].InternalName);
            if (!Directory.Exists(ext_path))
            {
                Directory.CreateDirectory(ext_path);
            }
            ghwtPath = ext_path;

            var fsb1 = Path.GetDirectoryName(ini) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_1.fsb.xen";
            var fsb2 = Path.GetDirectoryName(ini) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_2.fsb.xen";
            var fsb3 = Path.GetDirectoryName(ini) + "\\Content\\MUSIC\\" + Parser.Songs[0].InternalName + "_3.fsb.xen";
            
            var decFSB1 = temp + Path.GetFileName(fsb1);
            var decFSB2 = temp + Path.GetFileName(fsb2);
            var decFSB3 = temp + Path.GetFileName(fsb3);

            if (CancelWorkers) return;
            if (Tools.fsbIsEncrypted(fsb1))
            {
                var dec = Tools.DecryptFSBFile(fsb1);
                File.WriteAllBytes(decFSB1, dec);
                fsb1 = decFSB1;
            }
            if (File.Exists(decFSB1) && Tools.fsbIsEncrypted(decFSB1))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb1) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB1);
                return;
            }

            if (CancelWorkers) return;
            if (Tools.fsbIsEncrypted(fsb2))
            {
                var dec = Tools.DecryptFSBFile(fsb2);
                File.WriteAllBytes(decFSB2, dec);
                fsb2 = decFSB2;
            }
            if (File.Exists(decFSB2) && Tools.fsbIsEncrypted(decFSB2))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb2) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB2);
                return;
            }

            if (CancelWorkers) return;
            if (Tools.fsbIsEncrypted(fsb3))
            {
                var dec = Tools.DecryptFSBFile(fsb3);
                File.WriteAllBytes(decFSB3, dec);
                fsb3 = decFSB3;
            }
            if (File.Exists(decFSB3) && Tools.fsbIsEncrypted(decFSB3))
            {
                MessageBox.Show("File '" + Path.GetFileName(fsb3) + "' is encrypted and I failed to decrypt it\nCan't continue", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                Tools.DeleteFile(decFSB3);
                return;
            }

            //extract drum tracks
            const int frame = 384;//size of each frame of audio
            const int spacer1 = 1152;//the spacer from one track past the other three
            const int spacer2 = 768;//the spacer from one track past the other two
            const int spacer3 = 384;//the spacer from one track past the other

            var kick_audio = ext_path + "\\drums_1.mp3";
            var snare_audio = ext_path + "\\drums_2.mp3";
            var cymbal_audio = ext_path + "\\drums_3.mp3";
            var tom_audio = ext_path + "\\drums_4.mp3";
            var guitar_audio = ext_path + "\\guitar.mp3";
            var bass_audio = ext_path + "\\bass.mp3";
            var vocals_audio = ext_path + "\\vocals.mp3";
            var backing_audio = ext_path + "\\song.mp3";
            var crowd_audio = ext_path + "\\crowd.mp3";

            var kick_offset = 0x80;
            var snare_offset = kick_offset + frame;
            var cymbal_offset = snare_offset + frame;
            var tom_offset = cymbal_offset + frame;

            if (CancelWorkers) return;
            ExtractFSBAudio(fsb1, kick_offset, spacer1, kick_audio);
            ExtractFSBAudio(fsb1, snare_offset, spacer1, snare_audio);
            ExtractFSBAudio(fsb1, cymbal_offset, spacer1, cymbal_audio);
            ExtractFSBAudio(fsb1, tom_offset, spacer1, tom_audio);

            //extract guitar bass vocals
            var guitar_offset = 0x80;
            var bass_offset = guitar_offset + frame;
            var vocals_offset = bass_offset + frame;

            if (CancelWorkers) return;
            ExtractFSBAudio(fsb2, guitar_offset, spacer2, guitar_audio);
            ExtractFSBAudio(fsb2, bass_offset, spacer2, bass_audio);
            ExtractFSBAudio(fsb2, vocals_offset, spacer2, vocals_audio);

            //extract backing and crowd
            var backing_offset = 0x80;
            var crowd_offset = backing_offset + frame;

            if (CancelWorkers) return;
            ExtractFSBAudio(fsb3, backing_offset, spacer3, backing_audio);
            ExtractFSBAudio(fsb3, crowd_offset, spacer3, crowd_audio);

            mp3Files = Directory.GetFiles(ext_path, "*.mp3");
            if (mp3Files.Any())
            {
                //the metadata doesn't contain song data, let's get it from the mp3 files
                var stream = Bass.BASS_StreamCreateFile(mp3Files[0], 0L, File.ReadAllBytes(mp3Files[0]).Length, BASSFlag.BASS_SAMPLE_FLOAT);
                var len = Bass.BASS_ChannelGetLength(stream);
                var totaltime = Bass.BASS_ChannelBytes2Seconds(stream, len); // the total time length
                Parser.Songs[0].Length = (int)(totaltime * 1000);
                overrideSongLength = Parser.Songs[0].Length;
            }
            else
            {
                if (!message) return;
                MessageBox.Show("Failed to extract audio files from FSB files, can't play this song", Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void ExtractFSBAudio(string fsb, long offset, int spacer, string mp3)
        {
            byte[] mp3_data;
            const int frame = 384;//size of each frame of audio

            using (var ms = new MemoryStream(File.ReadAllBytes(fsb)))
            {
                using (var br = new BinaryReader(ms))
                {
                    br.BaseStream.Seek(offset, SeekOrigin.Begin);
                    while (br.BaseStream.Position < br.BaseStream.Length)
                    {
                        mp3_data = br.ReadBytes(frame);
                        br.BaseStream.Seek(spacer, SeekOrigin.Current);

                        using (var bw = new BinaryWriter(new FileStream(mp3, FileMode.Append)))
                        {
                            bw.Write(mp3_data);
                        }
                    }
                }
            }
        }

        private void songLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Song loader working");
            if (xbox360.Checked)
            {
                Log("loadCON: " + SongToLoad);
                loadCON(SongToLoad, true);
            }
            else if (yarg.Checked)
            {
                if (Path.GetFileName(SongToLoad) == ".yargsong")
                {
                    sngPath = SongToLoad;
                    pkgPath = "";
                    Log("DecryptExtractYARG: " + SongToLoad);
                    loadINI(SongToLoad, true);
                }
                else if (Path.GetExtension(SongToLoad) == ".sng")
                {
                    sngPath = SongToLoad;
                    Log("loadSNG: " + SongToLoad);
                    loadSNG(SongToLoad, true);
                }
                else if (Path.GetFileName(SongToLoad) == "songs.dta")
                {
                    sngPath = "";
                    pkgPath = "";
                    Log("loadDTA: " + SongToLoad);
                    loadDTA(SongToLoad, true);
                }
                else
                {
                    sngPath = "";
                    Log("loadINI: " + SongToLoad);
                    loadINI(SongToLoad, true);
                }
            }
            else if (rockSmith.Checked)
            {
                Log("loadPSARC: " + SongToLoad);
                loadPSARC(SongToLoad, true);
            }
            else if (powerGig.Checked)
            {
                Log("ExtractXMA: " + SongToLoad);
                ExtractXMA(SongToLoad, true);
            }
            else if (bandFuse.Checked)
            {
                BandFusePath = SongToLoad;
                Log("ExtractBandFuse: " + SongToLoad);
                ExtractBandFuse(SongToLoad, true);
            }
            else if (guitarHero.Checked)
            {
                ghwtPath = SongToLoad;
                Log("loadGHWTDE: " + SongToLoad);
                loadGHWT(SongToLoad, true);
            }
            else
            {
                if (pS3.Checked && Path.GetExtension(SongToLoad) == ".pkg")
                {
                    pkgPath = SongToLoad;
                    Log("loadPKG: " + SongToLoad);
                    loadPKG(SongToLoad, true);
                }
                else
                {
                    pkgPath = "";
                    ActiveSong.yargPath = "";
                    Log("loadDTA: " + SongToLoad);
                    loadDTA(SongToLoad, true);
                }
            }
        }

        private void batchSongLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Batch song loader finished");
            if (SongsToAdd.Any() && !CancelWorkers)
            {
                Log("There are more songs left to be processed, moving to next song");
                batchSongLoader.RunWorkerAsync();
                return;
            }
            StaticPlaylist = Playlist;
            ReloadPlaylist(Playlist, false);
            isScanning = false;
            UpdateNotifyTray();
            if (GIFOverlay != null)
            {
                GIFOverlay.Close();
                GIFOverlay = null;
            }
            consoleToolStripMenuItem.Enabled = true;
            EnableDisable(true);
            CancelWorkers = false;
            if (WindowState == FormWindowState.Minimized)
            {
                NotifyTray_MouseDoubleClick(null, null);
            }
            AddedSongs();
        }

        private void AddedSongs()
        {
            var added = lstPlaylist.Items.Count - StartingCount;
            if (added == 0)
            {
                const string msg = "No new songs were added";
                Log(msg);
                ShowUpdate(msg);
            }
            else
            {
                var msg = "Added " + added + " new " + (added == 1 ? "song" : "songs");
                Log(msg);
                ShowUpdate(msg);
                MarkAsModified();
                if (PlayingSong == null) return;
                if (btnShuffle.Tag.ToString() != "shuffle" && PlayingSong.Index == lstPlaylist.Items.Count - 1)
                {
                    GetNextSong();
                }
            }
        }

        private void UpdateNotifyTray()
        {
            string text;
            if (menuStrip1.InvokeRequired)
            {
                menuStrip1.Invoke(new MethodInvoker(() => consoleToolStripMenuItem.Enabled = !isScanning));
            }
            else
            {
                consoleToolStripMenuItem.Enabled = !isScanning;
            }
            if (isScanning)
            {
                text = "Scanning for songs...";
            }
            else if (btnPlayPause.Tag.ToString() == "pause")
            {
                var notify = "Playing: " + PlayingSong.Artist + " - " + PlayingSong.Name;
                text = notify.Length > 63 ? notify.Substring(0, 63) : notify;
            }
            else if (PlaybackSeconds == 0 || PlayingSong == null)
            {
                text = "Inactive";
            }
            else
            {
                var notify = "Paused: " + PlayingSong.Artist + " - " + PlayingSong.Name;
                text = notify.Length > 63 ? notify.Substring(0, 63) : notify;
            }
            Log("Updating notification tray text: " + text);
            NotifyTray.Text = text;
        }

        private void songLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Song loader finished");
            StaticPlaylist = Playlist;
            ReloadPlaylist(Playlist, false);
            isScanning = batchSongLoader.IsBusy || songLoader.IsBusy;
            UpdateNotifyTray();
            consoleToolStripMenuItem.Enabled = !isScanning;
            EnableDisable(true);
            CancelWorkers = false;
            AddedSongs();
        }

        private void lstPlaylist_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.All;
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GIFOverlay != null)
            {
                Log("User tried to close program - please wait until the current process finishes");
                MessageBox.Show("Please wait until the current process finishes", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                e.Cancel = true;
                return;
            }
            if (Text.Contains("*"))
            {
                Log("Tried to close with unsaved changes - confirm?");
                if (MessageBox.Show("You have unsaved changes on the current playlist\nAre you sure you want to do that?",
                    AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    Log("No");
                    e.Cancel = true;
                    return;
                }
                Log("Yes");
            }
            isClosing = true;
            StopPlayback();
            Bass.BASS_Free();
            SaveConfig();
            DeleteUsedFiles();
            var folder = Application.StartupPath + "\\temp\\";
            Tools.DeleteFolder(folder, true);
        }

        private void DeleteUsedFiles(bool all_files = true)
        {
            Log("Cleaning used and temporary files");
            //let's not leave over any files by mistake
            Tools.DeleteFile(Path.GetTempPath() + "o");
            Tools.DeleteFile(Path.GetTempPath() + "m");
            Tools.DeleteFile(Path.GetTempPath() + "temp");
            Tools.DeleteFolder(TempFolder, true);
            Tools.DeleteFile(NextSongArtBlurred);
            if (xbox360.Checked || pS3.Checked)
            {
                Tools.DeleteFile(NextSongMIDI);
            }
            if (!yarg.Checked && !fortNite.Checked && rockSmith.Checked && !guitarHero.Checked)
            {
                Tools.DeleteFile(NextSongArtPNG);
            }
            if (!all_files) return;
            Tools.DeleteFile(CurrentSongArtBlurred);
            if (xbox360.Checked || pS3.Checked)
            {
                Tools.DeleteFile(CurrentSongMIDI);
            }
            if (!yarg.Checked)
            {
                Tools.DeleteFile(CurrentSongArt);
            }
        }

        private void btnPlayPause_EnabledChanged(object sender, EventArgs e)
        {
            btnPlayPause.BackColor = btnPlayPause.Tag.ToString() == "play" ? Color.PaleGreen : Color.LightBlue;
            btnLoop.BackColor = btnLoop.Tag.ToString() == "loop" ? Color.LightYellow : Color.LightGray;
            btnShuffle.BackColor = btnShuffle.Tag.ToString() == "shuffle" ? Color.Thistle : Color.LightGray;

            btnPlayPause.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnPlayPause.BackColor);
            btnStop.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnStop.BackColor);
            btnNext.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnNext.BackColor);
            btnLoop.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnLoop.BackColor);
            btnShuffle.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnShuffle.BackColor);
        }

        private void lstPlaylist_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstPlaylist.SelectedItems.Count == 0 || GIFOverlay != null) return;
            GetActiveSong(lstPlaylist.SelectedItems[0].SubItems[0]);
        }

        private void GetActiveSong(ListViewItem.ListViewSubItem item)
        {
            var index = Convert.ToInt16(item.Text) - 1;
            Playlist[index].Index = lstPlaylist.SelectedIndices[0];
            ActiveSong = Playlist[index];
            Log("Selected new song in playlist: index: " + index + " '" + ActiveSong.Artist + " - " + ActiveSong.Name + "'");
        }

        private void btnPlayPause_Click(object sender, EventArgs e)
        {
            Log("btnPlayPause_Click");
            if (btnPlayPause.Tag.ToString() == "play")
            {
                if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PAUSED)
                {
                    Log("Resuming playback");
                    Bass.BASS_ChannelPlay(BassMixer, false);
                    if (MediaPlayer.playState == WMPPlayState.wmppsPaused)
                    {
                        MediaPlayer.Ctlcontrols.play();
                    }
                    UpdatePlaybackStuff();
                }
                else
                {
                    Log("Starting playback");
                    PlayingSong = ActiveSong;
                    StartPlayback(PlaybackSeconds == 0, true);
                }
            }
            else
            {
                Log("Pausing playback");
                StopPlayback(true);
                UpdateNotifyTray();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            Log("btnStop_Click");
            Log("Stopping playback");
            DoClickStop();
        }

        private void DoClickStop()
        {
            StopBASS();
            PlaybackTimer.Enabled = false;
            ClearLyrics();
            ClearVisuals();
            lblSections.Text = "";
            PlaybackSeconds = 0;
            StopPlayback();
            UpdateTime();
            UpdateNotifyTray();
        }

        private void ClearLyrics()
        {
            lblHarm1.Invoke(new MethodInvoker(() => lblHarm1.Text = ""));
            lblHarm2.Invoke(new MethodInvoker(() => lblHarm2.Text = ""));
            lblHarm3.Invoke(new MethodInvoker(() => lblHarm3.Text = ""));
            lblHarm1.Invoke(new MethodInvoker(() => lblHarm1.Image = null));
            lblHarm2.Invoke(new MethodInvoker(() => lblHarm2.Image = null));
            lblHarm3.Invoke(new MethodInvoker(() => lblHarm3.Image = null));
            if (!doKaraokeLyrics && !doScrollingLyrics && !doStaticLyrics) return;
            lblHarm1.Invoke(new MethodInvoker(() => lblHarm1.CreateGraphics().Clear(LabelBackgroundColor)));
            lblHarm2.Invoke(new MethodInvoker(() => lblHarm2.CreateGraphics().Clear(LabelBackgroundColor)));
            lblHarm3.Invoke(new MethodInvoker(() => lblHarm3.CreateGraphics().Clear(LabelBackgroundColor)));
        }

        public int[] ArrangeStreamChannels(int totalChannels, bool isOgg)
        {
            var channels = new int[totalChannels];
            if (isOgg)
            {
                switch (totalChannels)
                {
                    case 3:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        break;
                    case 5:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 3;
                        channels[4] = 4;
                        break;
                    case 6:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 4;
                        channels[4] = 5;
                        channels[5] = 3;
                        break;
                    case 7:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 4;
                        channels[4] = 5;
                        channels[5] = 6;
                        channels[6] = 3;
                        break;
                    case 8:
                        channels[0] = 0;
                        channels[1] = 2;
                        channels[2] = 1;
                        channels[3] = 4;
                        channels[4] = 5;
                        channels[5] = 6;
                        channels[6] = 7;
                        channels[7] = 3;
                        break;
                    default:
                        goto DoAllChannels;
                }
                return channels;
            }
        DoAllChannels:
            for (var i = 0; i < totalChannels; i++)
            {
                channels[i] = i;
            }
            return channels;
        }

        public float[,] GetChannelMatrix(int chans)
        {
            //initialize matrix
            //matrix must be float[output_channels, input_channels]
            var matrix = new float[2, chans];
            var ArrangedChannels = ArrangeStreamChannels(chans, true);
            if (ActiveSongData.ChannelsDrums > 0 && doAudioDrums)
            {
                //for drums it's a bit tricky because of the possible combinations
                switch (ActiveSongData.ChannelsDrums)
                {
                    case 2:
                        //stereo kit
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 0);
                        break;
                    case 3:
                        //mono kick
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 1, 0);
                        //stereo kit
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 1);
                        break;
                    case 4:
                        //mono kick
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 1, 0);
                        //mono snare
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 1, 1);
                        //stereo kit
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 2);
                        break;
                    case 5:
                        //mono kick
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 1, 0);
                        //stereo snare
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 1);
                        //stereo kit
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 3);
                        break;
                    case 6:
                        //stereo kick
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 0);
                        //stereo snare
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 2);
                        //stereo kit
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, 2, 4);
                        break;
                }
            }
            //var channel = song.ChannelsDrums;
            if (ActiveSongData.ChannelsBass > 0 && doAudioBass)
            {
                matrix = DoMatrixPanning(matrix, ArrangedChannels, ActiveSongData.ChannelsBass, ActiveSongData.ChannelsBassStart);//channel);
            }
            //channel = channel + song.ChannelsBass;
            if (ActiveSongData.ChannelsGuitar > 0 && doAudioGuitar)
            {
                matrix = DoMatrixPanning(matrix, ArrangedChannels, ActiveSongData.ChannelsGuitar, ActiveSongData.ChannelsGuitarStart);//channel);
            }
            //channel = channel + song.ChannelsGuitar;
            if (ActiveSongData.ChannelsVocals > 0 && doAudioVocals)
            {
                matrix = DoMatrixPanning(matrix, ArrangedChannels, ActiveSongData.ChannelsVocals, ActiveSongData.ChannelsVocalsStart);//channel);
            }
            //channel = channel + song.ChannelsVocals;
            if (ActiveSongData.ChannelsKeys > 0 && doAudioKeys)
            {
                matrix = DoMatrixPanning(matrix, ArrangedChannels, ActiveSongData.ChannelsKeys, ActiveSongData.ChannelsKeysStart);//channel);
            }
            //channel = channel + song.ChannelsKeys;
            if (ActiveSongData.ChannelsCrowd > 0 && doAudioCrowd)
            {
                matrix = DoMatrixPanning(matrix, ArrangedChannels, ActiveSongData.ChannelsCrowd, ActiveSongData.ChannelsCrowdStart);//channel);
            }
            //channel = channel + song.ChannelsCrowd;
            if (doAudioBacking) //song.ChannelsBacking > 0 &&  ---- should always be enabled per specifications
            {
                if (ActiveSongData.ChannelsTotal == 0 && chans > 0)
                {
                    ActiveSongData.ChannelsTotal = chans;
                }
                var backing = ActiveSongData.ChannelsTotal - ActiveSongData.ChannelsBass - ActiveSongData.ChannelsDrums - ActiveSongData.ChannelsGuitar - ActiveSongData.ChannelsKeys - ActiveSongData.ChannelsVocals - ActiveSongData.ChannelsCrowd;
                if (backing > 0) //backing not required 
                {
                    if (ActiveSongData.ChannelsCrowdStart + ActiveSongData.ChannelsCrowd == ActiveSongData.ChannelsTotal) //crowd channels are last
                    {
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, backing, ActiveSongData.ChannelsCrowdStart - backing);//channel);                        
                    }
                    else
                    {
                        matrix = DoMatrixPanning(matrix, ArrangedChannels, backing, ActiveSongData.ChannelsTotal - backing);//channel);
                    }
                }
            }
            return matrix;
        }

        private float[,] DoMatrixPanning(float[,] in_matrix, IList<int> ArrangedChannels, int inst_channels, int curr_channel)
        {
            //by default matrix values will be 0 = 0 volume
            //if nothing is assigned here, it stays at 0 so that channel won't be played
            //otherwise we assign a volume level based on the dta volumes

            //initialize output matrix based on input matrix, just in case something fails there's something going out
            var matrix = in_matrix;

            //split attenuation and panning info from DTA file for index access
            var volumes = ActiveSongData.AttenuationValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);
            var pans = ActiveSongData.PanningValues.Split(new char[0], StringSplitOptions.RemoveEmptyEntries);

            //BASS.NET lets us specify maximum volume when converting dB to Level
            //in case we want to change this later, it's only one value to change
            const double max_dB = 1.0;

            //technically we could do each channel, but Magma only allows us to specify volume per track, 
            //so both channels should have same volume, let's save a tiny bit of processing power
            float vol;
            try
            {
                vol = (float)Utils.DBToLevel(Convert.ToDouble(volumes[ArrangedChannels[curr_channel]]), max_dB);
            }
            catch (Exception)
            {
                vol = (float)1.0;
            }

            //assign volume level to channels in the matrix
            if (inst_channels == 2) //is it a stereo track
            {
                try
                {
                    //assign current channel (left) to left channel
                    matrix[0, ArrangedChannels[curr_channel]] = vol;
                }
                catch (Exception)
                { }
                try
                {
                    //assign next channel (right) to the right channel
                    matrix[1, ArrangedChannels[curr_channel + 1]] = vol;
                }
                catch (Exception)
                { }
            }
            else
            {
                //it's a mono track, let's assign based on the panning value
                double pan;
                try
                {
                    pan = Convert.ToDouble(pans[ArrangedChannels[curr_channel]]);
                }
                catch (Exception)
                {
                    pan = 0.0; // in case there's an error above, it gets centered
                }

                if (pan <= 0) //centered or left, assign it to the left channel
                {
                    matrix[0, ArrangedChannels[curr_channel]] = vol;
                }
                if (pan >= 0) //centered or right, assignt to the right channel
                {
                    matrix[1, ArrangedChannels[curr_channel]] = vol;
                }
            }
            return matrix;
        }

        private void UpdateTime(bool seek = false, bool update = false)
        {
            string time;
            var TimeSelection = seek ? PlaybackSeek : PlaybackSeconds;
            if (PlayingSong != null && TimeSelection * 1000 > PlayingSong.Length)
            {
                btnNext_Click(null, null);
                return;
            }
            if (TimeSelection >= 3600)
            {
                var hours = (int)(TimeSelection / 3600);
                var minutes = (int)(TimeSelection - (hours * 3600));
                var seconds = (int)(TimeSelection - (minutes * 60));
                time = hours + ":" + (minutes < 10 ? "0" : "") + minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            }
            else if (TimeSelection >= 60)
            {
                var minutes = (int)(TimeSelection / 60);
                var seconds = (int)(TimeSelection - (minutes * 60));
                time = minutes + ":" + (seconds < 10 ? "0" : "") + seconds;
            }
            else
            {
                time = "0:" + (TimeSelection < 10 ? "0" : "") + (int)TimeSelection;
            }
            if (lblTime.InvokeRequired)
            {
                lblTime.Invoke(new MethodInvoker(() => lblTime.Text = time));
            }
            else
            {
                lblTime.Text = time;
            }
            if (panelSlider.Cursor == Cursors.NoMoveHoriz || reset || PlayingSong == null) return;
            var percent = TimeSelection / ((double)PlayingSong.Length / 1000);
            panelSlider.Invoke(new MethodInvoker(() => panelSlider.Left = panelLine.Left + (int)((panelLine.Width - panelSlider.Width) * percent)));
            if (!update || displayKaraokeMode.Checked) return;
            DrawLyrics();
            DoPracticeSessions();
        }

        private void panelLine_MouseClick(object sender, MouseEventArgs e)
        {
            if (panelSlider.Cursor != Cursors.Hand || panelLine.Cursor != Cursors.Hand) return;
            if (e.Button == MouseButtons.Right && PracticeSessions != null && PracticeSessions.Any())
            {
                var selector = new SectionSelector(this, Cursor.Position);
                selector.Show();
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            Log("panelLine_MouseClick");
            ClearVisuals();
            PlaybackSeconds = ((double)PlayingSong.Length / 1000) * ((double)(e.X - (panelSlider.Width / 2)) / (panelLine.Width - panelSlider.Width));
            if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PAUSED || Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
            {
                Log("Setting audio location based on user input: " + PlaybackSeconds + " seconds");
                SetPlayLocation(PlaybackSeconds);
                var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * VolumeLevel), 1.0);
                Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
            }
            UpdateTime(false, !PlaybackTimer.Enabled);
        }

        private void panelLine_MouseHover(object sender, EventArgs e)
        {
            if (PlayingSong == null) return;
            var mouse = panelLine.PointToClient(Cursor.Position);
            var time = ((double)PlayingSong.Length / 1000) * ((double)(mouse.X - (panelSlider.Width / 2)) / (panelLine.Width - panelSlider.Width));
            toolTip1.Show(GetJumpMessage(time).Trim(), panelLine, mouse.X, mouse.Y - 30, 1000);
        }

        public void UpdatePlayback(bool doFade)
        {
            if (btnPlayPause.Tag.ToString() != "pause") return;
            PlaybackTimer.Enabled = false;
            StopPlayback();            
            StartPlayback(doFade, false);
        }

        private int ShuffleSongs(bool can_repeat = false)
        {
            Log("Shuffling songs");
            var random = new Random();
            int index;
            do
            {
                index = random.Next(0, lstPlaylist.Items.Count);
            }
            while ((PlayingSong != null && index == PlayingSong.Index) || (lstPlaylist.Items[index].Tag.ToString() == "1" && !can_repeat));
            Log("Index: " + index);
            return index;
        }

        private void playNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("playNowToolStripMenuItem_Click");
            doSongPreparer();
        }

        private void doSongPreparer()
        {
            GetActiveSong(lstPlaylist.SelectedItems[0].SubItems[0]);
            Log("Active song: '" + ActiveSong.Artist + " - " + ActiveSong.Name + "");
            NextSongIndex = lstPlaylist.SelectedIndices[0];
            EnableDisable(false);
            btnLoop.Enabled = false;
            btnShuffle.Enabled = false;
            nautilus.NextSongOggData = new byte[0];
            nautilus.ReleaseStreamHandle(true);
            ActiveSong.yargPath = "";
            songPreparer.RunWorkerAsync();
        }

        private void UpdateHighlights()
        {
            for (var i = 0; i < lstPlaylist.Items.Count; i++)
            {
                lstPlaylist.Items[i].BackColor = Color.Black;
                lstPlaylist.Items[i].ForeColor = lstPlaylist.Items[i].Tag.ToString() == "1" ? Color.Gray : Color.White;
            }
            if (lstPlaylist.SelectedItems.Count <= 0) return;
            var index = PlayingSong == null || PlayingSong.Index >= lstPlaylist.Items.Count ? lstPlaylist.SelectedIndices[0] : PlayingSong.Index;
            lstPlaylist.EnsureVisible(index);
            if (PlayingSong == null) return;
            var it = Convert.ToInt16(lstPlaylist.Items[index].SubItems[0].Text) - 1;
            if (Playlist[it].Artist != PlayingSong.Artist || Playlist[it].Name != PlayingSong.Name) return;
            lstPlaylist.Items[index].BackColor = Color.BurlyWood;
            lstPlaylist.Items[index].Tag = 1; //played
        }

        private void songPreparer_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Song preparer working");
            if (songExtractor.IsBusy && NextSong.Location == ActiveSong.Location)
            {
                do
                {//wait here
                } while (songExtractor.IsBusy);
            }

            if (xbox360.Checked)
            {
                Log("loadCON: " + ActiveSong.Location);
                loadCON(ActiveSong.Location, false, false, false, true);
            }
            else if (yarg.Checked)
            {
                if (Path.GetExtension(ActiveSong.Location) == ".yargsong")
                {
                    sngPath = ActiveSong.Location;
                    Log("loadINI: " + ActiveSong.Location);
                    loadINI(ActiveSong.Location, false, false, false, true);
                }
                else if (Path.GetExtension(ActiveSong.Location) == ".sng")
                {
                    sngPath = ActiveSong.Location;
                    Log("loadSNG: " + ActiveSong.Location);
                    loadSNG(ActiveSong.Location, false, false, false, true);
                }
                else if (Path.GetFileName(ActiveSong.Location) == "songs.dta")
                {
                    pkgPath = "";
                    sngPath = "";
                    Log("loadDTA: " + ActiveSong.Location);
                    loadDTA(ActiveSong.Location, false, false, false, true);
                }
                else
                {
                    sngPath = "";
                    Log("loadINI: " + ActiveSong.Location);
                    loadINI(ActiveSong.Location, false, false, false, true);
                }
            }
            else if (fortNite.Checked)
            {
                Log("loadFNF: " + ActiveSong.Location);
                loadINI(ActiveSong.Location, false, false, false, true);
            }
            else if (guitarHero.Checked)
            {
                ghwtPath = ActiveSong.Location;
                Log("loadGHWT: " + ActiveSong.Location);
                loadGHWT(ActiveSong.Location, false, false, false, true);
            }
            else if (rockSmith.Checked)
            {
                Log("loadPSARC: " + ActiveSong.Location);
                loadPSARC(ActiveSong.Location, false, false, false, true);
            }
            else if (powerGig.Checked)
            {
                Log("ExtractXMA: " + ActiveSong.Location);
                ExtractXMA(ActiveSong.Location, false, false, false, true);
            }
            else if (bandFuse.Checked)
            {
                BandFusePath = ActiveSong.Location;
                Log("ExtractBandFuse: " + ActiveSong.Location);
                ExtractBandFuse(ActiveSong.Location, false, false, false, true);
            }
            else
            {
                if (pS3.Checked && Path.GetExtension(ActiveSong.Location) == ".pkg")
                {
                    pkgPath = ActiveSong.Location;
                    Log("loadPKG: " + ActiveSong.Location);
                    loadPKG(ActiveSong.Location, false, false, false, true);
                }
                else
                {
                    pkgPath = "";
                    Log("loadDTA: " + ActiveSong.Location);
                    loadDTA(ActiveSong.Location, false, false, false, true);
                }
            }
            if (yarg.Checked)
            {
                GetIntroOutroSilencePS();
            }
            else
            {
                GetIntroOutroSilence();
            }
        }

        private void songPreparer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Song preparer finished");
            btnLoop.Enabled = true;
            btnShuffle.Enabled = true;
            MoveSongFiles();
            isScanning = batchSongLoader.IsBusy || songLoader.IsBusy;
            UpdateNotifyTray();
            if (GIFOverlay != null)
            {
                GIFOverlay.Close();
                GIFOverlay = null;
            }
            PrepareForPlayback();
            UpdateHighlights();
        }

        private void PrepareForPlayback() //mainly for GHWT:DE
        {
            Log("Preparing for playback");
            if ((!yarg.Checked && !fortNite.Checked && !guitarHero.Checked && !powerGig.Checked && !bandFuse.Checked) && (CurrentSongAudio == null || CurrentSongAudio.Length == 0))
            {
                if (AlreadyTried)
                {
                    Log("Can't play that song - audio file missing or encrypted");
                    MessageBox.Show("Unable to play that song - either the song files are in use by another program or the audio file is encrypted", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    EnableDisable(true);
                    AlreadyTried = false;
                }
                else
                {
                    AlreadyTried = true;
                    lstPlaylist_MouseDoubleClick(null, null);
                }
                return;
            }
            ClearAll();
            EnableDisable(true);
            ChangeDisplay();
            var index = Convert.ToInt16(lstPlaylist.Items[NextSongIndex].SubItems[0].Text) - 1;
            lstPlaylist.Items[NextSongIndex].Tag = 1; //played
            PlayingSong = Playlist[index];
            PlayingSong.Index = NextSongIndex;
            lblArtist.Text = "Artist: " + PlayingSong.Artist;
            lblSong.Text = "Song: " + PlayingSong.Name;
            lblAlbum.Text = string.IsNullOrEmpty(PlayingSong.Album.Trim()) ? "" : "Album: " + PlayingSong.Album;
            lblGenre.Text = string.IsNullOrEmpty(PlayingSong.Genre.Trim()) ? "" : "Genre: " + PlayingSong.Genre;
            lblTrack.Text = string.IsNullOrEmpty(PlayingSong.Album.Trim()) ? "" : "Track #: " + PlayingSong.Track;
            lblTrack.Visible = PlayingSong.Track > 0;
            lblYear.Text = PlayingSong.Year == 0 ? "" : "Year: " + PlayingSong.Year;
            if (PlayingSong.Length == 0)
            {
                PlayingSong.Length = overrideSongLength; //to accomodate songs missing that info
            }
            lblDuration.Text = Parser.GetSongDuration(PlayingSong.Length.ToString(CultureInfo.InvariantCulture));
            lblAuthor.Text = string.IsNullOrEmpty(PlayingSong.Charter.Trim()) ? "" : "Author: " + PlayingSong.Charter.Trim();
            toolTip1.SetToolTip(lblArtist, lblArtist.Text);
            toolTip1.SetToolTip(lblSong, lblSong.Text);
            toolTip1.SetToolTip(lblAlbum, lblAlbum.Text);
            toolTip1.SetToolTip(lblGenre, lblGenre.Text);
            toolTip1.SetToolTip(lblTrack, lblTrack.Text);
            toolTip1.SetToolTip(lblYear, lblYear.Text);
            toolTip1.SetToolTip(lblAuthor, lblAuthor.Text);
            EnableDisableButtons(true);
            panelSlider.Cursor = Cursors.Hand;
            panelLine.Cursor = Cursors.Hand;
            SetVideoPlayerPath(PlayingSong.Location);
            if (!File.Exists(CurrentSongArt))
            {
                displayAlbumArt.Checked = false;
                if (!displayMIDIChartVisuals.Checked && !displayKaraokeMode.Checked)
                {
                    displayAudioSpectrum.Checked = true;
                }
                if (!displayAudioSpectrum.Checked)
                {
                    toolTip1.SetToolTip(picPreview, "Click to change spectrum style");
                }
            }
            UpdateButtons();
            UpdateDisplay();
            Log("Song to play is '" + PlayingSong.Artist + " - " + PlayingSong.Name + "'");
            StartPlayback(PlaybackSeconds == 0, true);
        }

        private void EnableDisableButtons(bool enabled)
        {
            btnPlayPause.Enabled = enabled;
            btnStop.Enabled = enabled;
            btnNext.Enabled = enabled;
            btnLoop.Enabled = enabled;
            btnShuffle.Enabled = enabled;
        }

        private void PrepareForDrawing()
        {
            if (PlayingSong == null) return;
            Log("Preparing to draw MIDI file");
            MIDITools.Initialize(true);
            if (!MIDITools.ReadMIDIFile(CurrentSongMIDI))
            {
                Log("Failed - can't read that MIDI file - can't draw that song");
                ShowUpdate("Error reading MIDI file!");
                displayAudioSpectrum.Checked = true;
                displayMIDIChartVisuals.Checked = false;
            }
            else
            {
                Log("Success");
            }
            PracticeSessions = MIDITools.PracticeSessions;
            if (displayKaraokeMode.Checked && (!MIDITools.PhrasesVocals.Phrases.Any() || !MIDITools.LyricsVocals.Lyrics.Any()))
            {
                displayKaraokeMode.Checked = false;
                displayAudioSpectrum.Checked = true;
            }
            DrewFullChart = false;
            try
            {
                ChartBitmap = new Bitmap(picVisuals.Width, picVisuals.Height);
                Chart = Graphics.FromImage(ChartBitmap);
            }
            catch (Exception)
            { }
        }

        private int GetTrackstoDraw()
        {
            const int tall = 2;
            var tracks = 0;
            if (MIDITools.MIDI_Chart.Drums.ChartedNotes.Any() && doMIDIDrums)
            {
                tracks++;
            }
            if (MIDITools.MIDI_Chart.Bass.ChartedNotes.Any() && doMIDIBass)
            {
                tracks++;
            }
            if (MIDITools.MIDI_Chart.Guitar.ChartedNotes.Any() && doMIDIGuitar)
            {
                tracks++;
            }
            if (MIDITools.MIDI_Chart.Keys.ChartedNotes.Any() && doMIDIKeys)
            {
                tracks++;
            }
            else if (MIDITools.MIDI_Chart.ProKeys.ChartedNotes.Any() && doMIDIProKeys && PlayingSong.hasProKeys)
            {
                if (MIDITools.MIDI_Chart.ProKeys.NoteRange.Count > 8)
                {
                    tracks += tall;
                }
                else
                {
                    tracks++;
                }
            }
            if (!MIDITools.MIDI_Chart.Vocals.ChartedNotes.Any() || (!doMIDIVocals && !doMIDIHarmonies)) return tracks;
            if (MIDITools.MIDI_Chart.Vocals.NoteRange.Count > 8)
            {
                tracks += tall;
            }
            else
            {
                tracks++;
            }
            return tracks;
        }

        private void DrawMIDIFile(Graphics graphics)
        {
            if (MIDITools.MIDI_Chart == null || MIDITools.PhrasesVocals == null) return;

            const int lbl_height = 19;
            const int tall = 2;
            var tracks = GetTrackstoDraw();
            if (tracks == 0) return;
            var labels = new List<Label> { lblHarm1, lblHarm2, lblHarm3, lblSections };
            var panel_height = picVisuals.Height;
            panel_height = labels.Where(label => label.Visible).Aggregate(panel_height, (current, label) => current - lbl_height);
            var track_height = panel_height / tracks;
            var track_y = lblSections.Visible ? lblSections.Height : 0;
            int Index;
            var track_color = 1;
            if (MIDITools.MIDI_Chart.Drums.ChartedNotes.Count > 0 && doMIDIDrums)
            {
                track_y += track_height;
                DrawTrackBackground(graphics, track_y, track_height, track_color, "DRUMS", MIDITools.MIDI_Chart.Drums.Solos);
                DrawNotes(graphics, MIDITools.MIDI_Chart.Drums, track_height, track_y, true, -1, out Index);
                MIDITools.MIDI_Chart.Drums.ActiveIndex = Index;
                track_color++;
            }
            if (MIDITools.MIDI_Chart.Bass.ChartedNotes.Count > 0 && doMIDIBass)
            {
                track_y += track_height;
                DrawTrackBackground(graphics, track_y, track_height, track_color, PlayingSong.isRhythmOnBass ? "RHYTHM GUITAR" : "BASS", MIDITools.MIDI_Chart.Bass.Solos);
                DrawNotes(graphics, MIDITools.MIDI_Chart.Bass, track_height, track_y, false, -1, out Index);
                MIDITools.MIDI_Chart.Bass.ActiveIndex = Index;
                track_color++;
            }
            if (MIDITools.MIDI_Chart.Guitar.ChartedNotes.Count > 0 && doMIDIGuitar)
            {
                track_y += track_height;
                DrawTrackBackground(graphics, track_y, track_height, track_color, "GUITAR", MIDITools.MIDI_Chart.Guitar.Solos);
                DrawNotes(graphics, MIDITools.MIDI_Chart.Guitar, track_height, track_y, false, -1, out Index);
                MIDITools.MIDI_Chart.Guitar.ActiveIndex = Index;
                track_color++;
            }
            if (MIDITools.MIDI_Chart.ProKeys.ChartedNotes.Count > 0 && PlayingSong.hasProKeys && doMIDIProKeys)
            {
                var multKeys = 1;
                if (MIDITools.MIDI_Chart.ProKeys.NoteRange.Count > 8)
                {
                    multKeys = tall;
                }
                track_y += track_height * multKeys;
                DrawTrackBackground(graphics, track_y, track_height * multKeys, track_color, "PRO KEYS", MIDITools.MIDI_Chart.ProKeys.Solos);
                DrawNotes(graphics, MIDITools.MIDI_Chart.ProKeys, track_height * multKeys, track_y, false, -1, out Index);
                MIDITools.MIDI_Chart.ProKeys.ActiveIndex = Index;
                track_color++;
            }
            else if (MIDITools.MIDI_Chart.Keys.ChartedNotes.Count > 0 && doMIDIKeys)
            {
                track_y += track_height;
                DrawTrackBackground(graphics, track_y, track_height, track_color, PlayingSong.isRhythmOnKeys ? "RHYTHM GUITAR" : "KEYS", MIDITools.MIDI_Chart.Keys.Solos);
                DrawNotes(graphics, MIDITools.MIDI_Chart.Keys, track_height, track_y, false, -1, out Index);
                MIDITools.MIDI_Chart.Keys.ActiveIndex = Index;
                track_color++;
            }
            if (MIDITools.MIDI_Chart.Vocals.ChartedNotes.Count <= 0 || (!doMIDIVocals && !doMIDIHarmonies)) return;
            var multVocals = 1;
            if (MIDITools.MIDI_Chart.Vocals.NoteRange.Count > 8)
            {
                multVocals = tall;
            }
            track_y += track_height * multVocals;
            DrawTrackBackground(graphics, track_y, track_height * multVocals, track_color, MIDITools.MIDI_Chart.Harm1.ChartedNotes.Any() && doMIDIHarmonies ? "HARMONIES" : "VOCALS", null);
            if (chartSnippet.Checked)
            {
                DrawPhraseMarkers(graphics, MIDITools.PhrasesVocals, track_height * multVocals, track_y);
            }
            //let's draw Harm3 - Harm2 - Harm1 in case they overlap, Harm1 stays on top
            if (MIDITools.MIDI_Chart.Harm3.ChartedNotes.Count > 0 && doMIDIHarmonies)
            {
                DrawNotes(graphics, MIDITools.MIDI_Chart.Harm3, track_height * multVocals, track_y, false, 3, out Index);
                MIDITools.MIDI_Chart.Harm3.ActiveIndex = Index;
            }
            if (MIDITools.MIDI_Chart.Harm2.ChartedNotes.Count > 0 && doMIDIHarmonies)
            {
                DrawNotes(graphics, MIDITools.MIDI_Chart.Harm2, track_height * multVocals, track_y, false, 2, out Index);
                MIDITools.MIDI_Chart.Harm2.ActiveIndex = Index;
            }
            if (MIDITools.MIDI_Chart.Harm1.ChartedNotes.Count > 0 && doMIDIHarmonies)
            {
                DrawNotes(graphics, MIDITools.MIDI_Chart.Harm1, track_height * multVocals, track_y, false, 1, out Index);
                MIDITools.MIDI_Chart.Harm1.ActiveIndex = Index;
            }
            else
            {
                DrawNotes(graphics, MIDITools.MIDI_Chart.Vocals, track_height * multVocals, track_y, false, 0, out Index);
                MIDITools.MIDI_Chart.Vocals.ActiveIndex = Index;
            }
        }

        private void DrawPhraseMarkers(Graphics graphics, PhraseCollection phrases, int track_height, int track_y)
        {
            var time = GetCorrectedTime();
            if (phrases == null) return;
            var curr = -1;
            for (var p = 0; p < phrases.Phrases.Count; p++)
            {
                if (phrases.Phrases[p].PhraseStart > time) continue;
                if (phrases.Phrases[p].PhraseEnd > time + PlaybackWindow) break;
                curr = p;
            }
            if (curr == -1) return;
            var diff = phrases.Phrases[curr].PhraseEnd - time;
            if (phrases.Phrases.Count > curr + 1)
            {
                diff = (phrases.Phrases[curr].PhraseEnd + ((phrases.Phrases[curr + 1].PhraseStart - phrases.Phrases[curr].PhraseEnd) / 2)) - time;
            }
            var left = (float)((diff / PlaybackWindow) * picVisuals.Width);
            using (var DrawingPen = new SolidBrush(Color.LightSteelBlue))
            {
                graphics.FillRectangle(DrawingPen, (int)left, track_y - track_height + 10, 2, track_height - 20);
            }
        }

        private void DrawTrackBackground(Graphics graphics, int y, int height, int index, string name, ICollection<SpecialMarker> solos)
        {
            if (!chartSnippet.Checked) return;
            var is_solo = false;
            if (solos != null && solos.Count > 0 && doMIDIHighlightSolos)
            {
                if (solos.Any(solo => solo.MarkerBegin <= PlaybackSeconds && solo.MarkerEnd > PlaybackSeconds))
                {
                    is_solo = true;
                }
            }
            if (is_solo)
            {
                using (var DrawingPen = new SolidBrush(Color.LightSteelBlue))
                {
                    graphics.FillRectangle(DrawingPen, 0, y - height, picVisuals.Width, height);
                }
            }
            else
            {
                using (var DrawingPen = new SolidBrush(index % 2 == 0 ? TrackBackgroundColor2 : TrackBackgroundColor1))
                {
                    graphics.FillRectangle(DrawingPen, 0, y - height, picVisuals.Width, height);
                }
            }
            if (!doMIDINameTracks) return;
            Font font;
            try
            {
                font = new Font("Tahoma", 10, FontStyle.Bold);
            }
            catch (Exception)
            {
                font = new Font("Times New Roman", 10, FontStyle.Bold);
            }
            var rectangle = new Rectangle(0, y - height, picVisuals.Width, height);
            TextRenderer.DrawText(graphics, name + (is_solo ? " SOLO!" : ""), font, rectangle, index % 2 == 0 ? TrackBackgroundColor1 : TrackBackgroundColor2, TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }

        private void DrawNotes(Graphics graphics, MIDITrack track, int track_height, int track_y, bool drums, int harm, out int LastPlayedIndex)
        {
            LastPlayedIndex = track.ActiveIndex;
            var correctedTime = GetCorrectedTime();
            track_y++;
            track_height--;
            for (var z = chartSnippet.Checked || chartFull.Checked ? 0 : track.ActiveIndex; z < track.ChartedNotes.Count(); z++)
            {
                var note = track.ChartedNotes[z];
                if (note.NoteEnd < correctedTime && !chartFull.Checked && !chartSnippet.Checked) continue;
                if (note.NoteStart > correctedTime && !chartFull.Checked && !chartSnippet.Checked) break;
                if (chartSnippet.Checked && note.NoteEnd < correctedTime) continue;
                if (chartSnippet.Checked && note.NoteStart > correctedTime + (PlaybackWindow * 2)) break;
                LastPlayedIndex = z;
                if (doMIDIBWKeys && track.Name == "ProKeys")
                {
                    note.NoteColor = note.NoteName.Contains("#") ? Color.Black : Color.WhiteSmoke;
                }
                else if (note.NoteColor == Color.Empty)
                {
                    switch (harm)
                    {
                        case 0:
                            note.NoteColor = !doMIDIHarm1onVocals ? GetNoteColor(note.NoteNumber, drums) : Harm1Color;
                            break;
                        case 1:
                            note.NoteColor = Harm1Color;
                            break;
                        case 2:
                            note.NoteColor = Harm2Color;
                            break;
                        case 3:
                            note.NoteColor = Harm3Color;
                            break;
                        default:
                            note.NoteColor = GetNoteColor(note.NoteNumber, drums);
                            break;
                    }
                }

                var note_width = ((note.NoteLength / (PlayingSong.Length / 1000.0)) * picVisuals.Width);
                if (note_width < 1.0)
                {
                    note_width = 1.0;
                }
                var x = (note.NoteStart / (PlayingSong.Length / 1000.0)) * picVisuals.Width;
                var y = track_y;
                List<int> range;
                switch (NoteSizingType)
                {
                    default:
                        if (track.Name == "ProKeys" || track.Name == "Vocals" || track.Name == "Harm1" ||
                            track.Name == "Harm2" || track.Name == "Harm3")
                        {
                            range = track.NoteRange; //for these only use present note range for measuring
                        }
                        else
                        {
                            range = track.ValidNotes; //use all possible notes for measuring
                        }
                        break;
                    case 1:
                        //only use present note range for measuring
                        range = track.NoteRange;
                        break;
                    case 2:
                        //use all possible notes for measuring
                        range = track.ValidNotes;
                        break;
                }

                var note_height = track_height / range.Count;
                if (chartFull.Checked || chartSnippet.Checked)
                {
                    for (var i = 0; i < range.Count; i++)
                    {
                        if (note.NoteNumber != range[i]) continue;
                        y = track_y - (note_height * (range.Count - i));
                        break;
                    }
                }
                if (chartFull.Checked)
                {
                    using (var DrawingPen = new SolidBrush(note.NoteColor))
                    {
                        Chart.FillRectangle(DrawingPen, (float)x, y, (float)note_width, note_height);
                    }
                }
                else if (chartSnippet.Checked)
                {
                    var width = ((note.NoteLength / PlaybackWindow) * picVisuals.Width) * 0.8;
                    if (width < 1)
                    {
                        width = 1; //won't draw something less than one pixel wide, and we want something to show!
                    }
                    if ((note.NoteNumber == 97 || note.NoteNumber == 96) && harm >= 0)
                    {
                        var xPos = (float)(((note.NoteStart - correctedTime) / PlaybackWindow) * picVisuals.Width);
                        //draw circles for percussion notes
                        using (var DrawingPen = new Pen(note.NoteColor))
                        {
                            var height = note_height < 1.0 ? 1 : note_height;
                            graphics.DrawEllipse(DrawingPen, xPos, y, height, height);
                        }
                    }
                    else
                    {
                        var left = (float)((note.NoteStart - correctedTime) / PlaybackWindow) * picVisuals.Width;
                        if (drums && !note.isTom && (note.NoteNumber == 98 || note.NoteNumber == 99 || note.NoteNumber == 100))
                        {
                            using (var solidBrush = new SolidBrush(note.NoteColor))
                            {
                                graphics.FillEllipse(solidBrush, left, y, (float)width, note_height);
                            }
                        }
                        else
                        {
                            using (var solidBrush = new SolidBrush(note.NoteColor))
                            {
                                graphics.FillRectangle(solidBrush, left, y, (float)width, note_height);
                            }
                        }
                        if (z + 1 < track.ChartedNotes.Count() && (track.Name == "Vocals" || track.Name == "Harm1" || track.Name == "Harm2" || track.Name == "Harm3"))
                        {
                            var nextNote = track.ChartedNotes[z + 1];
                            try
                            {
                                IEnumerable<Lyric> source = null;
                                switch (track.Name)
                                {
                                    case "Vocals":
                                        source = MIDITools.LyricsVocals.Lyrics;
                                        break;
                                    case "Harm1":
                                        source = MIDITools.LyricsHarm1.Lyrics;
                                        break;
                                    case "Harm2":
                                        source = MIDITools.LyricsHarm2.Lyrics;
                                        break;
                                    case "Harm3":
                                        source = MIDITools.LyricsHarm3.Lyrics;
                                        break;
                                }
                                var str = "";
                                using (var enumerator = source.Where(lyric => lyric.LyricStart == nextNote.NoteStart).GetEnumerator())
                                {
                                    if (enumerator.MoveNext())
                                    {
                                        str = enumerator.Current.LyricText;
                                    }
                                }
                                if (!string.IsNullOrEmpty(str) && str.Replace("-", "").Trim() == "+")
                                {
                                    var x2 = (float)((nextNote.NoteStart - correctedTime) / PlaybackWindow) * picVisuals.Width;
                                    var num6 = y;
                                    for (var index2 = 0; index2 < range.Count; index2++)
                                    {
                                        if (nextNote.NoteNumber != range[index2]) continue;
                                        num6 = track_y - note_height * (range.Count - index2);
                                        break;
                                    }
                                    var pointF1 = new PointF(left + (float)width, y);
                                    var pointF2 = new PointF(left + (float)width, y + note_height);
                                    var pointF3 = new PointF(x2, num6 + note_height);
                                    var pointF4 = new PointF(x2, num6);
                                    using (var solidBrush = new SolidBrush(((track.Name == "Vocals" && doMIDIHarm1onVocals) || track.Name != "Vocals") ? note.NoteColor : Color.LightGray))
                                    {
                                        graphics.FillPolygon(solidBrush, new[] { pointF1, pointF2, pointF3, pointF4 });
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log("Error drawing vocal slide: " + ex.Message);
                            }
                        }
                        if ((!doMIDINameVocals || (track.Name != "Vocals" && track.Name != "Harm1" && (track.Name != "Harm2" && track.Name != "Harm3"))) && (!doMIDINameProKeys || track.Name != "ProKeys")) continue;
                        var emSize = note_height * 0.8f;
                        if (emSize > 20.0)
                        {
                            emSize = 20f;
                        }
                        var font = new Font("Impact", emSize);
                        TextRenderer.DrawText(graphics, note.NoteName, font, new Point((int)left + 1, y - 1), !doMIDIBWKeys || track.Name != "ProKeys" ? Color.White : (note.NoteName.Contains("#") ? Color.White : Color.Black), TextFormatFlags.NoPadding);
                    }
                }
            }
        }

        private Color GetNoteColor(int note_number, bool drums = false)
        {
            Color color;
            switch (note_number)
            {
                case 36:
                case 48:
                case 60:
                case 72:
                case 84:
                case 96:
                    color = drums ? ChartOrange : ChartGreen;
                    break;
                case 37:
                case 49:
                case 61:
                case 73:
                case 97:
                    color = ChartRed;
                    break;
                case 38:
                case 50:
                case 62:
                case 74:
                case 98:
                case 110:
                    color = ChartYellow;
                    break;
                case 39:
                case 51:
                case 63:
                case 75:
                case 99:
                case 111:
                    color = ChartBlue;
                    break;
                case 40:
                case 52:
                case 64:
                case 76:
                case 100:
                case 112:
                    color = drums ? ChartGreen : ChartOrange;
                    break;
                case 41:
                case 53:
                case 65:
                case 77:
                    color = Color.FromArgb(183, 0, 174);
                    break;
                case 42:
                case 54:
                case 66:
                case 78:
                    color = Color.FromArgb(114, 86, 0);
                    break;
                case 43:
                case 55:
                case 67:
                case 79:
                case 103:
                case 115:
                    color = Color.FromArgb(0, 20, 130);
                    break;
                case 44:
                case 56:
                case 68:
                case 80:
                    color = Color.FromArgb(246, 200, 55);
                    break;
                case 45:
                case 57:
                case 69:
                case 81:
                    color = Color.FromArgb(64, 64, 64);
                    break;
                case 46:
                case 58:
                case 70:
                case 82:
                    color = Color.FromArgb(0, 194, 229);
                    break;
                case 47:
                case 59:
                case 71:
                case 83:
                    color = Color.FromArgb(114, 0, byte.MaxValue);
                    break;
                default:
                    color = Color.FromArgb(byte.MaxValue, byte.MaxValue, byte.MaxValue);
                    break;
            }
            return color;
        }

        private void UpdateButtons()
        {
            btnShuffle.Enabled = lstPlaylist.Items.Count > 1;
            toolTip1.SetToolTip(btnShuffle, btnShuffle.Tag.ToString() == "shuffle" ? "Disable track shuffling" : "Enable track shuffling");
            btnLoop.Enabled = lstPlaylist.Items.Count > 0;
            toolTip1.SetToolTip(btnLoop, btnLoop.Tag.ToString() == "loop" ? "Disable track looping" : "Enable track looping");
        }

        private void panelSlider_MouseDown(object sender, MouseEventArgs e)
        {
            if (panelSlider.Cursor != Cursors.Hand || panelLine.Cursor != Cursors.Hand) return;
            panelSlider.Cursor = Cursors.NoMoveHoriz;
            mouseX = MousePosition.X;
        }

        private void panelSlider_MouseUp(object sender, MouseEventArgs e)
        {
            if (panelSlider.Cursor != Cursors.NoMoveHoriz || PlayingSong == null) return;
            panelSlider.Cursor = btnPlayPause.Enabled ? Cursors.Hand : Cursors.Default;
            lblAuthor.Text = string.IsNullOrEmpty(PlayingSong.Charter.Trim()) ? "" : "Author: " + PlayingSong.Charter.Trim();
            PlaybackSeconds = PlaybackSeek;
            Log("Setting audio location based on user input: " + PlaybackSeconds + " seconds");
            UpdateTime(false, !PlaybackTimer.Enabled);
            if (MediaPlayer.playState == WMPPlayState.wmppsPlaying || MediaPlayer.playState == WMPPlayState.wmppsPaused)
            {
                MediaPlayer.Ctlcontrols.currentPosition = PlaybackSeconds;
            }
        }

        private void panelSlider_MouseMove(object sender, MouseEventArgs e)
        {
            if (panelSlider.Cursor != Cursors.NoMoveHoriz) return;
            if (MousePosition.X != mouseX)
            {
                if (MousePosition.X > mouseX)
                {
                    panelSlider.Left = panelSlider.Left + (MousePosition.X - mouseX);
                }
                else if (MousePosition.X < mouseX)
                {
                    panelSlider.Left = panelSlider.Left - (mouseX - MousePosition.X);
                }
                mouseX = MousePosition.X;
            }
            var min = panelLine.Left;
            var max = panelLine.Left + panelLine.Width - panelSlider.Width;
            if (panelSlider.Left < min)
            {
                panelSlider.Left = min;
            }
            else if (panelSlider.Left > max)
            {
                panelSlider.Left = max;
            }
            ClearVisuals();
            PlaybackSeek = (int)(((double)PlayingSong.Length / 1000) * ((double)(panelSlider.Left - panelLine.Left) / (panelLine.Width - panelSlider.Width)));
            if (PlaybackSeek < 0)
            {
                PlaybackSeek = 0;
            }
            else if (PlaybackSeek * 1000 > PlayingSong.Length)
            {
                PlaybackSeek = PlayingSong.Length / 1000;
            }
            lblAuthor.Text = GetJumpMessage(PlaybackSeek);
            UpdateTime(true);
            if (Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PAUSED && Bass.BASS_ChannelIsActive(BassMixer) != BASSActive.BASS_ACTIVE_PLAYING) return;
            SetPlayLocation(PlaybackSeek, true);
            var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * VolumeLevel), 1.0);
            Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
        }

        private void btnLoop_Click(object sender, EventArgs e)
        {
            if (songExtractor.IsBusy || songPreparer.IsBusy) return;
            Log("btnLoop_Click");
            btnLoop.Tag = btnLoop.Tag.ToString() == "loop" ? "noloop" : "loop";
            btnLoop.BackColor = btnLoop.Tag.ToString() == "loop" ? Color.LightYellow : Color.LightGray;
            toolTip1.SetToolTip(btnLoop, btnLoop.Tag.ToString() == "loop" ? "Disable track looping" : "Enable track looping");
            btnShuffle.Tag = "noshuffle";
            btnShuffle.BackColor = Color.LightGray;
            toolTip1.SetToolTip(btnShuffle, "Enable track shuffling");
            Log("Loop tag: " + btnLoop.Tag);
            Log("Shuffle tag: " + btnShuffle.Tag);
        }

        private void btnShuffle_Click(object sender, EventArgs e)
        {
            if (songExtractor.IsBusy || songPreparer.IsBusy) return;
            Log("btnShuffle_Click");
            btnShuffle.Tag = btnShuffle.Tag.ToString() == "shuffle" ? "noshuffle" : "shuffle";
            btnShuffle.BackColor = btnShuffle.Tag.ToString() == "shuffle" ? Color.Thistle : Color.LightGray;
            toolTip1.SetToolTip(btnShuffle, btnShuffle.Tag.ToString() == "shuffle" ? "Disable track shuffling" : "Enable track shuffling");
            btnLoop.Tag = "noloop";
            btnLoop.BackColor = Color.LightGray;
            toolTip1.SetToolTip(btnLoop, "Enable track looping");
            Log("Loop tag: " + btnLoop.Tag);
            Log("Shuffle tag: " + btnShuffle.Tag);
        }

        private void picPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || PlayingSong == null) return;
            if (!displayAlbumArt.Checked && File.Exists(CurrentSongArt))
            {
                Log("Displaying full size album art");
                var display = new Art(Cursor.Position, CurrentSongArt);
                display.Show();
                return;
            }
            if (!displayAlbumArt.Checked && (File.Exists(CurrentSongArt) || displayAudioSpectrum.Checked)) return;
            SpectrumID++;
            Log("Changed audio spectrum visualization - ID #" + SpectrumID);
            picPreview.Image = null;
            Spectrum.ClearPeaks();
        }

        private void lstPlaylist_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstPlaylist.SelectedItems.Count != 1 || (GIFOverlay != null && !AlreadyTried)) return;
            if (songPreparer.IsBusy) return;
            Log("lstPlaylist_MouseDoubleClick");
            doSongPreparer();
        }

        private void MoveSongFiles()
        {                
            Tools.MoveFile(NextSongArtBlurred, CurrentSongArtBlurred);
            if (yarg.Checked || fortNite.Checked || guitarHero.Checked)
            {
                CurrentSongArt = File.Exists(NextSongArtPNG) ? NextSongArtPNG : NextSongArtJPG;
                CurrentSongMIDI = NextSongMIDI;
                //CurrentSongAudioPath = NextSongAudioPath;
                nautilus.PlayingSongOggData = nautilus.NextSongOggData;
                nautilus.NextSongOggData = new byte[0];
                nautilus.ReleaseStreamHandle(true);
                CurrentSongAudio = nautilus.PlayingSongOggData;
                return;
            }            
            Tools.MoveFile(NextSongArtPNG, CurrentSongArt);
            if (nautilus.NextSongOggData != null && nautilus.NextSongOggData.Length > 0)
            {
                nautilus.PlayingSongOggData = nautilus.NextSongOggData;
                nautilus.NextSongOggData = new byte[0];
                nautilus.ReleaseStreamHandle(true);
            }
            if (wii.Checked)
            {
                CurrentSongMIDI = NextSongMIDI;
                CurrentSongAudio = nautilus.PlayingSongOggData;
                return;
            }
            Tools.MoveFile(NextSongMIDI, CurrentSongMIDI);
            CurrentSongAudio = nautilus.PlayingSongOggData;
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("removeToolStripMenuItem_Click");
            var indexes = lstPlaylist.SelectedIndices;
            var savedIndex = lstPlaylist.SelectedIndices[0];
            var playing = btnPlayPause.Text == "PAUSE";
            var to_remove = new List<int>();
            Log("Removing " + indexes.Count + " song(s) from the playlist");
            foreach (int index in indexes)
            {
                if (PlayingSong != null && index == PlayingSong.Index)
                {
                    ClearAll();
                    ClearVisuals(true);
                    DeleteUsedFiles();
                }
                to_remove.Add(Convert.ToInt16(lstPlaylist.Items[index].SubItems[0].Text) - 1);
            }
            to_remove.Sort();
            var ind = to_remove.Aggregate("", (current, t) => current + " t");
            Log("Indeces to remove: " + ind);
            for (var i = to_remove.Count - 1; i >= 0; i--)
            {
                var song = Playlist[to_remove[i]];
                Playlist.Remove(song);
                StaticPlaylist.Remove(song);
                Log("Removed song '" + song.Artist + " - " + song.Name + "'");
            }
            txtSearch.Text = "Type to search playlist...";
            ReloadPlaylist(Playlist, true, true, false);
            Log(lstPlaylist.Items.Count + " song(s) left in the playlist");
            if (lstPlaylist.Items.Count > 0)
            {
                if (savedIndex > lstPlaylist.Items.Count - indexes.Count)
                {
                    lstPlaylist.Items[lstPlaylist.Items.Count - indexes.Count].Selected = true;
                }
                else if (savedIndex < lstPlaylist.Items.Count)
                {
                    lstPlaylist.Items[savedIndex].Selected = true;
                }
                else
                {
                    lstPlaylist.Items[0].Selected = true;
                }
            }
            if (lstPlaylist.SelectedItems.Count > 0)
            {
                if (playing && btnPlayPause.Text == "PLAY")
                {
                    lstPlaylist_MouseDoubleClick(null, null);
                }
                lstPlaylist.EnsureVisible(lstPlaylist.SelectedIndices[0]);
            }
            GetNextSong();
            UpdateHighlights();
            MarkAsModified();
        }

        private void MarkAsModified()
        {
            Text = Text.Replace("*", "") + "*";
        }

        public string CleanArtistSong(string input)
        {
            return string.IsNullOrEmpty(input) ? "" : (input.Replace("(RB3 version)", "").Replace("(2x Bass Pedal)", "").Replace("(Rhythm Version)", "").Replace("(Rhythm version)", "").Replace("featuring ", "ft. ").Replace("feat. ", "ft. ").Replace(" feat ", " ft. ").Replace("(feat ", ")ft. ")).Trim();
        }

        private void ReloadPlaylist(IList<Song> playlist, bool update = true, bool search = true, bool doExtract = true)
        {
            lstPlaylist.Items.Clear();
            lstPlaylist.Refresh();
            Log("Reloading playlist");

            var searchTerm = txtSearch.Text;
            lstPlaylist.BeginUpdate();
            for (var i = 0; i < playlist.Count; i++)
            {
                if (searchTerm != "Type to search playlist..." && !string.IsNullOrEmpty(searchTerm.Trim()) && search)
                {
                    if (!playlist[i].Artist.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) &&
                        !playlist[i].Name.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()))
                    {
                        continue;
                    }
                }

                //format leading index number
                var digits = 3; //999 songs
                var index = "000";
                if (playlist.Count > 99999)
                {
                    digits = 6; //999,999 songs ... unlikely but in case i'm not around
                    index = "000000";
                }
                else if (playlist.Count > 9999)
                {
                    digits = 5; //99,999 songs
                    index = "00000";
                }
                else if (playlist.Count > 999)
                {
                    digits = 4; //9,999 songs
                    index = "0000";
                }
                index = index + (i + 1);
                index = index.Substring(index.Length - digits, digits);

                //add entry to playlist panel
                var entry = new ListViewItem(index);
                entry.SubItems.Add(CleanArtistSong(playlist[i].Artist + " - " + CleanArtistSong(playlist[i].Name)));
                if (playlist[i].Length == 0)
                {
                    entry.SubItems.Add("");//we don't have song duration for Fornite Festival m4a files so blank it out at this point
                }
                else
                {
                    entry.SubItems.Add(Parser.GetSongDuration(playlist[i].Length.ToString(CultureInfo.InvariantCulture)));
                }                
                entry.Tag = 0; //not played
                lstPlaylist.Items.Add(entry);
            }
            lstPlaylist.EndUpdate();

            var itemCount = lstPlaylist.Items.Count;
            if (itemCount > 0)
            {
                var ind = 0;
                if (PlayingSong != null && search)
                {
                    for (var i = 0; i < itemCount; i++)
                    {
                        var index = 0;
                        lstPlaylist.Invoke(new MethodInvoker(() => index = Convert.ToInt16(lstPlaylist.Items[i].SubItems[0].Text) - 1));
                        if (playlist[index].Artist != PlayingSong.Artist || playlist[index].Name != PlayingSong.Name) continue;
                        ind = i;
                        break;
                    }
                }
                lstPlaylist.Items[ind].Selected = true;
                lstPlaylist.Items[ind].Focused = true;
                lstPlaylist.EnsureVisible(ind);
                if (doExtract)
                {
                    GetNextSong();
                }
            }

            var msg = "Loaded " + itemCount + (itemCount == 1 ? " song" : " songs");
            Log(msg);
            if (!update) return;
            ShowUpdate(msg);
        }

        private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectionUp();
        }

        private void MoveSelectionUp()
        {
            var itemsToBeMoved = lstPlaylist.SelectedItems.Cast<ListViewItem>().ToArray<ListViewItem>();
            var itemsToBeMovedEnum = itemsToBeMoved;
            foreach (var item in itemsToBeMovedEnum)
            {
                var index = item.Index - 1;
                lstPlaylist.Items.RemoveAt(item.Index);
                lstPlaylist.Items.Insert(index, item);
            }
            MarkAsModified();
        }

        private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MoveSelectionDown();
        }

        private void MoveSelectionDown()
        {
            var itemsToBeMoved = lstPlaylist.SelectedItems.Cast<ListViewItem>().ToArray<ListViewItem>();
            var itemsToBeMovedEnum = itemsToBeMoved.Reverse();
            foreach (var item in itemsToBeMovedEnum)
            {
                var index = item.Index + 1;
                lstPlaylist.Items.RemoveAt(item.Index);
                lstPlaylist.Items.Insert(index, item);
            }
            MarkAsModified();
        }

        private void playNextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("playNextToolStripMenuItem_Click");
            var item = lstPlaylist.SelectedItems[0];
            item.Tag = 0;
            item.BackColor = Color.Black;
            item.ForeColor = Color.White;
            var substract = lstPlaylist.SelectedIndices[0] < PlayingSong.Index;
            lstPlaylist.Items.RemoveAt(item.Index);
            if (substract)
            {
                PlayingSong.Index--;
            }
            lstPlaylist.Items.Insert(PlayingSong.Index + 1, item);
            lstPlaylist.EnsureVisible(PlayingSong.Index);

            if (btnShuffle.Tag.ToString() == "shuffle")
            {
                btnShuffle_Click(null, null);
            }
            else
            {
                GetNextSong();
            }
            MarkAsModified();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Log("btnNext_Click");
            if (btnLoop.Tag.ToString() == "loop")
            {
                DoLoop();
                return;
            }
            GetNextSong();        
        }

        private void GetNextSong()
        {
            if (lstPlaylist.Items.Count == 0) return;
            Log("Getting next song index");
            var itemCount = lstPlaylist.Items.Count;
            if (btnShuffle.Tag.ToString() == "shuffle" && itemCount > 1)
            {
                DoShuffleSongs();
                return;
            }
            else if (PlayingSong != null)
            {
                if (lstPlaylist.SelectedIndices.Count <= 0)
                {
                    NextSongIndex = PlayingSong.Index;
                }
                else if (PlayingSong.Index + 1 == itemCount)
                {
                    NextSongIndex = 0;
                }
                else
                {
                    NextSongIndex = PlayingSong.Index + 1;
                }
            }
            else
            {
                NextSongIndex = 0;
            }
            if (NextSongIndex >= itemCount)
            {
                NextSongIndex = itemCount - 1;
            }
            Log("Index: " + NextSongIndex);
            if (PlayingSong == null || NextSongIndex == PlayingSong.Index) return;
            var index = Convert.ToInt16(lstPlaylist.Items[NextSongIndex].SubItems[0].Text) - 1;
            NextSong = Playlist[index];
            if (songExtractor.IsBusy) return;
            Tools.DeleteFile(NextSongArtBlurred);
            if (xbox360.Checked)
            {
                Tools.DeleteFile(NextSongArtPNG);
                Tools.DeleteFile(NextSongMIDI);
            }
            else if (pS3.Checked)
            {
                Tools.DeleteFile(NextSongMIDI);
            }
            btnLoop.Enabled = false;
            btnShuffle.Enabled = false;
            songExtractor.RunWorkerAsync();
        }

        private void saveCurrentPlaylist_Click(object sender, EventArgs e)
        {
            SavePlaylist(false);
        }

        private void SavePlaylist(bool force_new)
        {
            Log("Saving playlist: " + PlaylistPath);

            var vers = Assembly.GetExecutingAssembly().GetName().Version;
            var version = " v" + String.Format("{0}.{1}.{2}", vers.Major, vers.Minor, vers.Build);

            if (string.IsNullOrEmpty(PlaylistPath) || force_new)
            {
                const string message = "Enter playlist name:";
                var input = Interaction.InputBox(message, AppName);
                if (string.IsNullOrEmpty(input)) return;

                PlaylistName = input;
                PlaylistPath = Application.StartupPath + "\\playlists\\" + Tools.CleanString(input, true) + ".playlist";
                Tools.DeleteFile(PlaylistPath);
            }

            using (var sw = new StreamWriter(PlaylistPath, false))
            {
                sw.Write("//Created by " + AppName + version);
                sw.Write("\r\n//PlaylistConsole=" + PlayerConsole);
                sw.Write("\r\n//PlaylistName=" + PlaylistName);
                sw.Write("\r\n//TotalSongs=" + (force_new ? lstPlaylist.Items.Count : Playlist.Count));

                for (var i = 0; i < (force_new ? lstPlaylist.Items.Count : Playlist.Count); i++)
                {
                    var index = force_new ? (Convert.ToInt16(lstPlaylist.Items[i].SubItems[0].Text) - 1) : i;
                    var song = Playlist[index];

                    sw.Write("\r\n" + song.Artist + "\t");
                    sw.Write(song.Name + "\t");
                    sw.Write(song.Album + "\t");
                    sw.Write(song.Track.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.Genre + "\t");
                    sw.Write(song.Year.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.Length.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.AttenuationValues + "\t");
                    sw.Write(song.PanningValues + "\t");
                    sw.Write(song.ChannelsDrums.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsBass.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsGuitar.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsVocals.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsKeys.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsCrowd.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.ChannelsBacking.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.Charter.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.InternalName.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.Location.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.DTAIndex.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.AddToPlaylist.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.BPM.ToString(CultureInfo.InvariantCulture) + "\t");
                    sw.Write(song.isRhythmOnBass + "\t");
                    sw.Write(song.isRhythmOnKeys + "\t");
                    sw.Write(song.hasProKeys + "\t");
                    sw.Write(song.PSDelay);
                }
            }
            Log("Saved playlist with " + (force_new ? lstPlaylist.Items.Count : Playlist.Count) + " song(s)");
            UpdateRecentPlaylists(PlaylistPath);
            Text = AppName + " - " + PlaylistName;
        }

        private void loadExistingPlaylist_Click(object sender, EventArgs e)
        {
            if (Text.Contains("*"))
            {
                if (MessageBox.Show("You have unsaved changes on the current playlist\nAre you sure you want to do that?",
                        AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            }
            var ofd = new OpenFileDialog
            {
                Title = "Select " + AppName + " Playlist",
                Multiselect = false,
                InitialDirectory = Application.StartupPath + "\\playlists\\",
                Filter = AppName + " Playlist (*.playlist)|*.playlist",
            };
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                ofd.Dispose();
                return;
            }
            StartNew(false);
            PrepareToLoadPlaylist(ofd.FileName);
            ofd.Dispose();
        }

        private void LoadPlaylist()
        {
            if (string.IsNullOrEmpty(PlaylistPath)) return;
            var showWait = false;
            Log("Loading playlist: " + PlaylistPath);
            if (!File.Exists(PlaylistPath))
            {
                Log("Can't find that file!");
                MessageBox.Show("Can't find that playlist file!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (GIFOverlay == null)
            {
                InitiateGIFOverlay();
                showWait = true;
            }
            var error = false;
            var sr = new StreamReader(PlaylistPath);
            try
            {
                var header = sr.ReadLine();
                if (!header.Contains(AppName + " v"))
                {
                    Log("Not a valid " + AppName + " Playlist");
                    Log("Line: " + header);
                    MessageBox.Show("Not a valid " + AppName + " Playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    sr.Dispose();
                    return;
                }
                var console = Tools.GetConfigString(sr.ReadLine());
                if (console != PlayerConsole)
                {
                    var path = PlaylistPath;
                    switch (console)
                    {
                        case "xbox":
                            xbox360.PerformClick();
                            break;
                        case "wii":
                            wii.PerformClick();
                            break;
                        case "ps3":
                            pS3.PerformClick();
                            break;
                        case "yarg":
                            yarg.PerformClick();
                            break;
                        case "rocksmith":
                            rockSmith.PerformClick();
                            break;
                        case "fortnite":
                            fortNite.PerformClick();
                            break;
                        case "guitarhero":
                            guitarHero.PerformClick();
                            break;
                        case "bandfuse":
                            bandFuse.PerformClick();
                            break;
                        case "powergig":
                            powerGig.PerformClick();
                            break;
                    }
                    PlaylistPath = path;
                }
                ClearVisuals(true);
                Playlist = new List<Song>();
                StaticPlaylist = new List<Song>();
                ClearAll();
                PlayingSong = null;
                lstPlaylist.Items.Clear();
                lstPlaylist.Refresh();
                if (MIDITools.PhrasesVocals != null)
                {
                    MIDITools.PhrasesVocals.Phrases.Clear();
                }
                PlaylistName = Tools.GetConfigString(sr.ReadLine());
                var songcount = Convert.ToInt32(Tools.GetConfigString(sr.ReadLine()));
                var line_number = 4;
                for (var i = 0; i < songcount; i++)
                {
                    var line = "";
                    try
                    {
                        line_number++;
                        line = sr.ReadLine();
                        var song_info = line.Split(new[] { "\t" }, StringSplitOptions.None);
                        var song = new Song
                        {
                            Artist = song_info[0],
                            Name = song_info[1],
                            Album = song_info[2],
                            Track = Convert.ToInt16(song_info[3]),
                            Genre = song_info[4],
                            Year = Convert.ToInt16(song_info[5]),
                            Length = Convert.ToInt64(song_info[6]),
                            AttenuationValues = song_info[7],
                            PanningValues = song_info[8],
                            ChannelsDrums = Convert.ToInt16(song_info[9]),
                            ChannelsBass = Convert.ToInt16(song_info[10]),
                            ChannelsGuitar = Convert.ToInt16(song_info[11]),
                            ChannelsVocals = Convert.ToInt16(song_info[12]),
                            ChannelsKeys = Convert.ToInt16(song_info[13]),
                            ChannelsCrowd = Convert.ToInt16(song_info[14]),
                            ChannelsBacking = Convert.ToInt16(song_info[15]),
                            Charter = song_info[16],
                            InternalName = song_info[17],
                            Location = song_info[18],
                            DTAIndex = Convert.ToInt16(song_info[19]),
                            AddToPlaylist = song_info[20].Contains("True"),
                            Index = -1,
                            //v1.0 added BPM
                            BPM = song_info.Count() >= 22 ? Convert.ToDouble(song_info[21]) : 120, //default value if not already stored
                            //v2.0 added isRhythmOnBass, isRhythmOnKeys, hasProKeys
                            isRhythmOnBass = song_info.Count() >= 25 && song_info[22].Contains("True"),
                            isRhythmOnKeys = song_info.Count() >= 25 && song_info[23].Contains("True"),
                            hasProKeys = song_info.Count() >= 25 && song_info[24].Contains("True"),
                            //v2.1.1 added Phase Shift Delay
                            PSDelay = song_info.Count() >= 26 ? Convert.ToInt16(song_info[25]) : 0
                        };
                        if (File.Exists(song.Location))
                        {
                            Playlist.Add(song);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log("Error loading playlist: " + ex.Message);
                        Log("Line #" + line_number + ": " + line);
                        error = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Log("Error: " + ex.Message);
                MessageBox.Show("Error loading that Playlist\nError: " + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            sr.Dispose();

            if (error)
            {
                if (Playlist.Any())
                {
                    var msg = "Some of the song entries in that playlist were corrupt or in a format I wasn't expecting\nPlease don't modify the playlist files manually\n\nI was able to recover " + Playlist.Count + (Playlist.Count == 1 ? " song" : " songs") + " :-)\n\nSee the log file to track down the problem song(s)";
                    Log(msg);
                    MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    if (showWait)
                    {
                        if (GIFOverlay != null)
                        {
                            GIFOverlay.Close();
                            GIFOverlay = null;
                        }
                    }
                    const string msg = "Some of the song entries in that playlist were corrupt or in a format I wasn't expecting\nPlease don't modify the playlist files manually\n\nUnfortunately I wasn't able to recover any songs :-(\n\nSee the log file to track down the problem song(s)";
                    Log(msg);
                    MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }
            if (!Playlist.Any())
            {
                if (showWait)
                {
                    if (GIFOverlay != null)
                    {
                        GIFOverlay.Close();
                        GIFOverlay = null;
                    }
                }
                Log("Nothing could be loaded from that playlist");
                MessageBox.Show("Nothing could be loaded from that playlist", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            UpdateRecentPlaylists(PlaylistPath);
            StaticPlaylist = Playlist;
            ActiveSong = null;
            ReloadPlaylist(Playlist, true, true, false);
            Text = AppName + " - " + PlaylistName;
            if (showWait)
            {
                if (GIFOverlay != null)
                {
                    GIFOverlay.Close();
                    GIFOverlay = null;
                }
            }
            if (lstPlaylist.Items.Count == 0 || songExtractor.IsBusy || !autoPlay.Checked) return;
            if (autoPlay.Checked && btnShuffle.Tag.ToString() == "shuffle")
            {
                lstPlaylist.Items[0].Selected = false;
                lstPlaylist.Items[ShuffleSongs()].Selected = true;
            }
            lstPlaylist_MouseDoubleClick(null, null);
        }

        private void UpdateRecentPlaylists(string playlist)
        {
            if (!string.IsNullOrEmpty(playlist))
            {
                //remove if already in list
                for (var i = 0; i < 5; i++)
                {
                    if (RecentPlaylists[i] == playlist)
                    {
                        RecentPlaylists[i] = "";
                    }
                }
                //move down playlists
                for (var i = 4; i > 0; i--)
                {
                    RecentPlaylists[i] = RecentPlaylists[i - 1];
                }
                RecentPlaylists[0] = playlist; //add newest one to the top
            }
            recent1.Visible = false;
            recent2.Visible = false;
            recent3.Visible = false;
            recent4.Visible = false;
            recent5.Visible = false;
            recent1.Text = Path.GetFileName(RecentPlaylists[0]);
            recent1.Visible = !string.IsNullOrEmpty(recent1.Text) && File.Exists(RecentPlaylists[0]);
            recent2.Text = Path.GetFileName(RecentPlaylists[1]);
            recent2.Visible = !string.IsNullOrEmpty(recent2.Text) && File.Exists(RecentPlaylists[1]);
            recent3.Text = Path.GetFileName(RecentPlaylists[2]);
            recent3.Visible = !string.IsNullOrEmpty(recent3.Text) && File.Exists(RecentPlaylists[2]);
            recent4.Text = Path.GetFileName(RecentPlaylists[3]);
            recent4.Visible = !string.IsNullOrEmpty(recent4.Text) && File.Exists(RecentPlaylists[3]);
            recent5.Text = Path.GetFileName(RecentPlaylists[4]);
            recent5.Visible = !string.IsNullOrEmpty(recent5.Text) && File.Exists(RecentPlaylists[4]);
        }

        private void SaveConfig()
        {
            Log("Saving configuration file");
            using (var sw = new StreamWriter(config, false))
            {
                sw.WriteLine("PlayerConsole=" + PlayerConsole);
                sw.WriteLine("LoopPlayback=" + (btnLoop.Tag.ToString() == "loop"));
                sw.WriteLine("ShufflePlayback=" + (btnShuffle.Tag.ToString() == "shuffle"));
                sw.WriteLine("AutoloadPlaylist=" + autoloadLastPlaylist.Checked);
                sw.WriteLine("LastPlaylist=" + PlaylistPath);
                sw.WriteLine("AutoPlay=" + autoPlay.Checked);
                sw.WriteLine("PlayCrowdTrack=" + doAudioCrowd);
                sw.WriteLine("EnableCrossFade=False //feature is deprecated");
                sw.WriteLine("CrossFadeLength=0.0 //feature is deprecated");
                sw.WriteLine("ShowPracticeSessions=" + showPracticeSections.Checked);
                sw.WriteLine("ShowLyrics=" + doStaticLyrics);
                sw.WriteLine("WholeWords=" + doWholeWordsLyrics);
                sw.WriteLine("GameSyllables=" + !doWholeWordsLyrics);
                sw.WriteLine("ShowMIDIVisuals=" + displayMIDIChartVisuals.Checked);
                sw.WriteLine("OpenSideWindow=" + openSideWindow.Checked);
                sw.WriteLine("VolumeLevel=" + VolumeLevel);
                sw.WriteLine("UseGameColors=False //feature is deprecated");
                sw.WriteLine("UseRandomColors=False //feature is deprecated");
                sw.WriteLine("InvertAllColors=False //feature is deprecated"); //keep this old line from prior version so as not to break config compatibility
                sw.WriteLine("ModeBPM=False //feature is deprecated");
                sw.WriteLine("ModeAbstract=False //feature is deprecated");
                sw.WriteLine("DrawFullChart=" + chartFull.Checked);
                sw.WriteLine("DrawSnippet=" + chartSnippet.Checked);
                sw.WriteLine("DrawNoteNames=False //feature is deprecated");
                sw.WriteLine("DrawCircles=False //feature is deprecated");
                sw.WriteLine("DrawRectangles=False //feature is deprecated");
                sw.WriteLine("DrawLines=False //feature is deprecated");
                sw.WriteLine("DrawSpirals=False //feature is deprecated");
                sw.WriteLine("DrawRandomShapes=False //feature is deprecated");
                for (var i = 0; i < 5; i++)
                {
                    sw.WriteLine("RecentPlaylist" + (i + 1) + "=" + RecentPlaylists[i]);
                }
                sw.WriteLine("DrawLyrics=False //feature is deprecated");
                sw.WriteLine("DrawSpectrum=" + displayAudioSpectrum.Checked);
                sw.WriteLine("SpectrumID=" + SpectrumID);
                sw.WriteLine("DisplayAlbumArt=" + displayAlbumArt.Checked);
                sw.WriteLine("DisplayHarmonies=" + doHarmonyLyrics);
                sw.WriteLine("DontDisplayLyrics=" + (!doStaticLyrics && !doKaraokeLyrics && !doScrollingLyrics));
                sw.WriteLine("KaraokeLyrics=" + doKaraokeLyrics);
                sw.WriteLine("ScrollingLyrics=" + doScrollingLyrics);
                sw.WriteLine("LabelTracks=" + doMIDINameTracks);
                sw.WriteLine("PlaybackWindow=" + PlaybackWindow);
                sw.WriteLine("NoteSizingType=" + NoteSizingType);
                sw.WriteLine("NameProKeysNotes=" + doMIDINameProKeys);
                sw.WriteLine("NameVocalNotes=" + doMIDINameVocals);
                sw.WriteLine("DisplayBackgroundVideo=" + displayBackgroundVideo.Checked);
                sw.WriteLine("StartMaximized=" + (WindowState == FormWindowState.Maximized));
                sw.WriteLine("HighlightSolos=" + doMIDIHighlightSolos);
                sw.WriteLine("UploadtoImgur=" + uploadScreenshots.Checked);
                sw.WriteLine("BWProKeys=" + doMIDIBWKeys);
                sw.WriteLine("UseHarm1ColorOnVocals=" + doMIDIHarm1onVocals);
                sw.WriteLine("UseKaraokeMode=" + displayKaraokeMode.Checked);
                sw.WriteLine("SkipIntroOutroSilence=" + skipIntroOutroSilence.Checked);
                sw.WriteLine("SilenceThreshold=" + SilenceThreshold);
                sw.WriteLine("FadeInLength=" + FadeLength);
            }
        }

        private void LoadConfig()
        {
            Log("Loading configuration file");
            if (!File.Exists(config))
            {
                Log("Not found");
                return;
            }
            var sr = new StreamReader(config);
            try
            {
                PlayerConsole = Tools.GetConfigString(sr.ReadLine());
                xbox360.Checked = false;
                pS3.Checked = false;
                wii.Checked = false;
                yarg.Checked = false;
                rockSmith.Checked = false;
                guitarHero.Checked = false;
                fortNite.Checked = false;
                powerGig.Checked = false;
                bandFuse.Checked = false;
                switch (PlayerConsole)
                {
                    case "xbox":
                        xbox360.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: Xbox 360 (Rock Band)";
                        break;
                    case "ps3":
                        pS3.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PlayStation 3 (Rock Band)";
                        break;
                    case "wii":
                        wii.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: Wii (Rock Band)";
                        break;
                    case "yarg":
                        yarg.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PC (YARG / Clone Hero)";
                        break;
                    case "rocksmith":
                        rockSmith.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PC (Rocksmith 2014)";
                        break;
                    case "guitarhero":
                        guitarHero.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PC (GHWT:DE)";
                        break;
                    case "fortnite":
                        fortNite.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PC (Fortnite Festival)";
                        break;
                    case "bandfuse":
                        bandFuse.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: Xbox 360 (BandFuse)";
                        break;
                    case "powergig":
                        powerGig.Checked = true;
                        consoleToolStripMenuItem.Text = "Your console: PC (Power Gig)";
                        break;
                }
                var loop = sr.ReadLine().Contains("True");
                btnLoop.Tag = !loop ? "noloop" : "loop";
                btnLoop.BackColor = loop ? Color.LightYellow : Color.LightGray;
                toolTip1.SetToolTip(btnLoop, loop ? "Disable track looping" : "Enable track looping");
                var shuffle = sr.ReadLine().Contains("True");
                btnShuffle.Tag = !shuffle ? "noshuffle" : "shuffle";
                btnShuffle.BackColor = shuffle ? Color.Thistle : Color.LightGray;
                toolTip1.SetToolTip(btnShuffle, shuffle ? "Disable track shuffling" : "Enable track shuffling");
                autoloadLastPlaylist.Checked = sr.ReadLine().Contains("True");
                PlaylistPath = Tools.GetConfigString(sr.ReadLine());
                autoPlay.Checked = sr.ReadLine().Contains("True");
                doAudioCrowd = sr.ReadLine().Contains("True");
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                showPracticeSections.Checked = sr.ReadLine().Contains("True");
                doHarmonyLyrics = !sr.ReadLine().Contains("True");
                doWholeWordsLyrics = sr.ReadLine().Contains("True");
                sr.ReadLine(); //no longer need this
                displayMIDIChartVisuals.Checked = sr.ReadLine().Contains("True");
                openSideWindow.Checked = sr.ReadLine().Contains("True");
                VolumeLevel = Convert.ToDouble(Tools.GetConfigString(sr.ReadLine()));
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                chartFull.Checked = sr.ReadLine().Contains("True");
                chartSnippet.Checked = sr.ReadLine().Contains("True");
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                sr.ReadLine(); //no longer need this
                var playlists = new List<string>();
                for (var i = 0; i < 5; i++)
                {
                    var line = Tools.GetConfigString(sr.ReadLine());
                    if (!string.IsNullOrEmpty(line) && File.Exists(line))
                    {
                        playlists.Add(line);
                    }
                }
                for (var i = 0; i < playlists.Count; i++)
                {
                    RecentPlaylists[i] = playlists[i];
                }
                sr.ReadLine(); //no longer need this
                displayAudioSpectrum.Checked = sr.ReadLine().Contains("True");
                SpectrumID = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                displayAlbumArt.Checked = sr.ReadLine().Contains("True");
                doHarmonyLyrics = sr.ReadLine().Contains("True");
                var no_lyrics = sr.ReadLine().Contains("True");
                doKaraokeLyrics = sr.ReadLine().Contains("True");
                doScrollingLyrics = sr.ReadLine().Contains("True");
                if (no_lyrics)
                {
                    doKaraokeLyrics = false;
                    doScrollingLyrics = false;
                }
                doMIDINameTracks = sr.ReadLine().Contains("True");
                PlaybackWindow = Convert.ToDouble(Tools.GetConfigString(sr.ReadLine()));
                NoteSizingType = Convert.ToInt16(Tools.GetConfigString(sr.ReadLine()));
                doMIDINameProKeys = sr.ReadLine().Contains("True");
                doMIDINameVocals = sr.ReadLine().Contains("True");
                displayBackgroundVideo.Checked = sr.ReadLine().Contains("True");
                playBGVideos.Checked = displayBackgroundVideo.Checked;
                if (sr.ReadLine().Contains("True"))
                {
                    WindowState = FormWindowState.Maximized;
                }
                doMIDIHighlightSolos = sr.ReadLine().Contains("True");
                uploadScreenshots.Checked = sr.ReadLine().Contains("True");
                doMIDIBWKeys = sr.ReadLine().Contains("True");
                doMIDIHarm1onVocals = sr.ReadLine().Contains("True");
                displayKaraokeMode.Checked = sr.ReadLine().Contains("True");
                skipIntroOutroSilence.Checked = sr.ReadLine().Contains("True");
                SilenceThreshold = float.Parse(Tools.GetConfigString(sr.ReadLine()));
                FadeLength = Convert.ToDouble(Tools.GetConfigString(sr.ReadLine()));
            }
            catch (Exception ex)
            {
                Log("Error loading config: " + ex.Message);
            }
            sr.Dispose();
            Log("Success");
            Log("Playlist console: " + PlayerConsole);
            styleToolStripMenuItem.Visible = displayMIDIChartVisuals.Checked;
            toolStripMenuItem8.Visible = displayMIDIChartVisuals.Checked;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var version = GetAppVersion();
            var message = AppName + " - The Rock Band Music Player\nVersion: " + version + " (Dark Edition)\n© TrojanNemo, 2014-2024\n\n";
            var credits = Tools.ReadHelpFile("credits");
            MessageBox.Show(message + credits, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Log("Displayed About message");
        }

        private void picVolume_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var Volume = new Volume(this, Cursor.Position);
            Volume.Show();
            Log("Displaying volume control form");
        }

        public void UpdateVolume(double volume)
        {
            if (PlayingSong == null) return;
            var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * volume), 1.0);
            Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
            Log("Update volume: " + track_vol);
        }

        private void markAsUnplayed_Click(object sender, EventArgs e)
        {
            var indexes = lstPlaylist.SelectedIndices;
            foreach (int index in indexes)
            {
                lstPlaylist.Items[index].Tag = 0;
                lstPlaylist.Items[index].BackColor = Color.Black;
                lstPlaylist.Items[index].ForeColor = Color.White;
            }
            Log("Marked " + indexes.Count + " song(s)s as unplayed");
            GetNextSong();
        }

        private void markAsPlayed_Click(object sender, EventArgs e)
        {
            var indexes = lstPlaylist.SelectedIndices;
            foreach (int index in indexes)
            {
                lstPlaylist.Items[index].Tag = 1;
                lstPlaylist.Items[index].BackColor = Color.Black;
                lstPlaylist.Items[index].ForeColor = Color.Gray;
            }
            Log("Marked " + indexes.Count + " song(s)s as played");
            GetNextSong();
        }

        private enum PlaylistFilters
        {
            ByArtist, ByAlbum, ByGenre
        }

        private void FilterPlaylist(PlaylistFilters filter)
        {
            Log("Filtering playlist");
            Playlist = new List<Song>();
            foreach (var song in StaticPlaylist)
            {
                switch (filter)
                {
                    case PlaylistFilters.ByArtist:
                        if (CleanArtistSong(song.Artist).ToLowerInvariant().Contains(CleanArtistSong(ActiveSong.Artist).ToLowerInvariant()))
                        {
                            Playlist.Add(song);
                        }
                        break;
                    case PlaylistFilters.ByAlbum:
                        if (song.Album.Trim() == ActiveSong.Album.Trim())
                        {
                            Playlist.Add(song);
                        }
                        break;
                    case PlaylistFilters.ByGenre:
                        if (song.Genre.Trim() == ActiveSong.Genre.Trim())
                        {
                            Playlist.Add(song);
                        }
                        break;
                }
            }
            if (Playlist.Any())
            {
                switch (filter)
                {
                    case PlaylistFilters.ByArtist:
                        Playlist.Sort((a, b) => String.CompareOrdinal(a.Name.ToLowerInvariant(), b.Name.ToLowerInvariant()));
                        Log("Filtered by artist");
                        break;
                    case PlaylistFilters.ByAlbum:
                        Playlist.Sort((a, b) => a.Track.CompareTo(b.Track));
                        Log("Filtered by album");
                        break;
                    case PlaylistFilters.ByGenre:
                        Playlist.Sort((a, b) => String.CompareOrdinal(a.Artist.ToLowerInvariant(), b.Artist.ToLowerInvariant()));
                        Log("Filtered by genre");
                        break;
                }
            }
            txtSearch.Text = "Type to search playlist...";
            ReloadPlaylist(Playlist);
            UpdateButtons();
            UpdateHighlights();
        }

        private void goToArtist_Click(object sender, EventArgs e)
        {
            FilterPlaylist(PlaylistFilters.ByArtist);
        }

        private void goToAlbum_Click(object sender, EventArgs e)
        {
            FilterPlaylist(PlaylistFilters.ByAlbum);
        }

        private void goToGenre_Click(object sender, EventArgs e)
        {
            FilterPlaylist(PlaylistFilters.ByGenre);
        }

        private void songExtractor_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Song extractor working");
            if (xbox360.Checked)
            {
                NextSong.yargPath = "";
                Log("loadCON: " + NextSong.Location);
                loadCON(NextSong.Location, false, false, true);
            }
            else if (yarg.Checked)
            {
                if (Path.GetExtension(NextSong.Location) == ".yargsong")
                {
                    sngPath = NextSong.Location;
                    Log("DecryptExtractYARG: " + NextSong.Location);
                    loadINI(NextSong.Location, false, false, true);
                }
                else if (Path.GetExtension(NextSong.Location) == ".sng")
                {
                    NextSong.yargPath = "";
                    sngPath = NextSong.Location;
                    Log("loadSNG: " + NextSong.Location);
                    loadSNG(NextSong.Location, false, false, true);
                }
                else if (Path.GetFileName(NextSong.Location) == "songs.dta")
                {
                    pkgPath = "";
                    Log("loadDTA: " + NextSong.Location);
                    loadDTA(NextSong.Location, false, false, true);
                }
                else
                {
                    NextSong.yargPath = "";
                    Log("loadINI: " + NextSong.Location);
                    loadINI(NextSong.Location, false, false, true);
                }
            }
            else if (rockSmith.Checked)
            {
                Log("loadPSARC: " + NextSong.Location);
                loadPSARC(NextSong.Location, false, false, true);
            }
            else if (powerGig.Checked)
            {
                Log("ExtractXMA: " + NextSong.Location);
                ExtractXMA(NextSong.Location, false, false, true);
            }
            else if (bandFuse.Checked)
            {
                BandFusePath = NextSong.Location;
                Log("ExtractBandFuse: " + NextSong.Location);
                ExtractBandFuse(NextSong.Location, false, false, true);
            }
            else if (fortNite.Checked)
            {
                Log("loadINI: " + NextSong.Location);
                loadINI(NextSong.Location, false, false, true);
            }
            else if (guitarHero.Checked)
            {
                ghwtPath = NextSong.Location;
                Log("loadGHWT: " + NextSong.Location);
                loadGHWT(NextSong.Location, false, false, true);
            }
            else
            {
                if (pS3.Checked && Path.GetExtension(NextSong.Location) == ".pkg")
                {
                    pkgPath = NextSong.Location;
                    Log("loadPKG: " + NextSong.Location);
                    loadPKG(NextSong.Location, false, false, true);
                }
                else
                {
                    pkgPath = "";
                    NextSong.yargPath = "";
                    Log("loadDTA: " + NextSong.Location);
                    loadDTA(NextSong.Location, false, false, true);
                }
            }

            if (yarg.Checked)
            {
                GetIntroOutroSilencePS();
            }
            else
            {
                GetIntroOutroSilence();
            }
        }

        private string DecryptExtractYARG(string inFile, bool message = false, bool scanning = true, bool next = false, bool prep = false)
        {
            byte[] SNGPKG = { (byte)'S', (byte)'N', (byte)'G', (byte)'P', (byte)'K', (byte)'G' };
            var tempFolder = Application.StartupPath + "\\bin\\temp";
            var tempFile = tempFolder + "\\temp.sng";
            Tools.DeleteFolder(tempFolder, true);
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }
            using (FileStream fileStream = File.OpenRead(inFile))
            {
                YARGSongFileStream yargFileStream = TryLoad(fileStream);
                byte[] bytes = new byte[yargFileStream.Length];
                yargFileStream.Read(bytes, 0, bytes.Length);
                yargFileStream.Close();
                using (var fs = File.Create(tempFile))
                {
                    using (var bw = new BinaryWriter(fs))
                    {
                        bw.Write(bytes);
                    }
                }
            }
            using (FileStream fileStream = new FileStream(tempFile, FileMode.Open, FileAccess.Write))
            {
                fileStream.Write(SNGPKG, 0, SNGPKG.Length);
            }
            if (!Tools.ExtractSNG(tempFile, tempFolder))
            {
                Tools.DeleteFile(tempFile);
                if (message)
                {
                    var choice = MessageBox.Show("Decrypting YARG .yargsong files requires .NET Desktop Runtime 7\n\nIf you already have .NET Desktop Runtime 7 installed and it still doesn't work, notify Nemo\n\nIf you don't have .NET Desktop Runtime 7 installed, click OK to go to the Microsoft website and download it from there\n\nOr Click Cancel to go back", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                    if (choice == DialogResult.OK)
                    {
                        Process.Start("https://dotnet.microsoft.com/en-us/download/dotnet/7.0");
                    }
                }
                return "";
            }
            var ini = Directory.GetFiles(tempFolder, "song.ini", SearchOption.AllDirectories);
            Tools.DeleteFile(tempFile);
            return ini[0];            
        }

        private void songExtractor_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Song extractor finished");
            isScanning = batchSongLoader.IsBusy || songLoader.IsBusy;
            UpdateNotifyTray();
            btnLoop.Enabled = true;
            btnShuffle.Enabled = true;
            if (GIFOverlay != null)
            {
                GIFOverlay.Close();
                GIFOverlay = null;
            }
            if (PlayingSong.Index <= lstPlaylist.Items.Count - 1)
            {
                lstPlaylist.Items[PlayingSong.Index].Selected = false;
            }
            if (NextSongIndex > lstPlaylist.Items.Count - 1)
            {
                NextSongIndex = 0;
                DeleteUsedFiles(false);
            }
            lstPlaylist.Items[NextSongIndex].Selected = true;
            lstPlaylist.Items[NextSongIndex].Focused = true;
            lstPlaylist.EnsureVisible(NextSongIndex);
            if (!yarg.Checked && lstPlaylist.Items.Count > 0)
            {
                Log("Next song files not found, processing song files again");
                lstPlaylist_MouseDoubleClick(null, null);
                return;
            }
            Log("Preparing to play next song");
            PlaybackTimer.Enabled = false;
            MoveSongFiles();
            PrepareForPlayback();
            UpdateHighlights();
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                openSideWindow.Checked = true;
            }
            MediaPlayer.Height = picVisuals.Height - GetHeightDiff();
            MediaPlayer.Width = picVisuals.Width;
            lblUpdates.Left = (openSideWindow.Checked ? picVisuals.Left + picVisuals.Width : panelPlaying.Left + panelPlaying.Width) - lblUpdates.Width;
            if (WindowState != FormWindowState.Minimized) return;
            Log("Minimized to system tray");
            NotifyTray.ShowBalloonTip(250);
            Hide();
        }

        private void NotifyTray_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
                Log("Restored from system tray");
                UpdateHighlights();
            }
            else
            {
                WindowState = FormWindowState.Minimized;
            }
        }

        private void restoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Log("restoreToolStripMenuItem_Click");
            NotifyTray_MouseDoubleClick(null, null);
        }

        private void sortPlaylistByArtist_Click(object sender, EventArgs e)
        {
            SortPlaylist(PlaylistSorting.BySongArtist);
        }

        private void sortPlaylistBySong_Click(object sender, EventArgs e)
        {
            SortPlaylist(PlaylistSorting.BySongName);
        }

        private void sortPlaylistByDuration_Click(object sender, EventArgs e)
        {
            SortPlaylist(PlaylistSorting.BySongDuration);
        }

        private enum PlaylistSorting
        {
            BySongArtist, BySongName, BySongDuration, ByModifiedDate, Shuffle
        }

        private void SortPlaylist(PlaylistSorting sort)
        {
            Log("Sorting playlist");
            SortingStyle = sort;
            switch (SortingStyle)
            {
                case PlaylistSorting.BySongArtist:
                    Playlist.Sort((a, b) => String.CompareOrdinal(a.Artist.ToLowerInvariant() + " - " + a.Name.ToLowerInvariant(), b.Artist.ToLowerInvariant() + " - " + b.Name.ToLowerInvariant()));
                    Log("Sorted by artist name");
                    break;
                case PlaylistSorting.BySongName:
                    Playlist.Sort((a, b) => String.CompareOrdinal(a.Name.ToLowerInvariant() + " - " + a.Artist.ToLowerInvariant(), b.Name.ToLowerInvariant() + " - " + b.Artist.ToLowerInvariant()));
                    Log("Sorted by song title");
                    break;
                case PlaylistSorting.BySongDuration:
                    Playlist.Sort((a, b) => a.Length.CompareTo(b.Length));
                    Log("Sorted by song duration");
                    break;
                case PlaylistSorting.ByModifiedDate:
                    Playlist.Sort((a, b) => File.GetLastWriteTimeUtc(a.Location).CompareTo(File.GetLastWriteTimeUtc(b.Location)));
                    Playlist.Reverse();
                    Log("Sorted by file modified date");
                    break;
                case PlaylistSorting.Shuffle:
                    Shuffle(Playlist);
                    Log("Shuffled");
                    break;
            }
            ReloadPlaylist(Playlist);
            txtSearch.Text = "Type to search playlist...";
            UpdateHighlights();
            MarkAsModified();
        }

        private void ShowUpdate(string update)
        {
            UpdateTimer.Stop();
            lblUpdates.Invoke(new MethodInvoker(() => lblUpdates.Text = update));
            UpdateTimer.Enabled = true;
        }

        private void txtSearch_Leave(object sender, EventArgs e)
        {
            if (txtSearch.Text.Trim() != "") return;
            txtSearch.Text = "Type to search playlist...";
        }

        private void txtSearch_MouseClick(object sender, MouseEventArgs e)
        {
            if (txtSearch.Text.Trim() != "Type to search playlist...") return;
            txtSearch.Text = "";
        }

        private bool lastWordWasDash;
        private bool IsMiddleOfWord(string line)
        {
            line = line.Replace("^", "").Replace("#", "").Replace("$", "").Trim();
            if (line == "+")
            {
                return lastWordWasDash;
            }
            lastWordWasDash = line.EndsWith("-", StringComparison.Ordinal);
            return lastWordWasDash;
        }
                
        public int GetKaraokeCurrentLineTop()
        {
            return (int)(picVisuals.Height * 0.05);
        }
        public int GetKaraokeNextLineTop()
        {
            return (int)(picVisuals.Height * 0.95);
        }

        private void DoKaraokeMode(Graphics graphics, IList<LyricPhrase> phrases, IEnumerable<Lyric> lyrics)
        {
            var time = GetCorrectedTime();
            LyricPhrase currentLine = null;
            LyricPhrase nextLine = null;
            LyricPhrase lastLine = null;
            //get active and next phrase, and store last used phrase
            for (var i = 0; i < phrases.Count(); i++)
            {
                var phrase = phrases[i];
                if (string.IsNullOrEmpty(phrase.PhraseText)) continue;
                if (phrase.PhraseEnd < time)
                {
                    lastLine = phrases[i];
                    continue;
                }
                if (phrase.PhraseStart > time)
                {
                    nextLine = phrases[i];
                    break;
                }
                currentLine = phrase;
                if (i < phrases.Count - 1)
                {
                    nextLine = phrases[i + 1];
                }
                break;
            }
            var currentLineTop = GetKaraokeCurrentLineTop();
            var nextLineTop = GetKaraokeNextLineTop();
            string lineText;
            Font lineFont;
            Size lineSize;
            int posX;
            if (currentLine != null && !string.IsNullOrEmpty(currentLine.PhraseText))
            {
                //draw entire current phrase on top
                lineText = ProcessLine(currentLine.PhraseText, true);
                lineFont = new Font("Tahoma", GetScaledFontSize(graphics, lineText, new Font("Tahoma", (float)12.0), 120));
                lineSize = TextRenderer.MeasureText(lineText, lineFont);
                posX = (picVisuals.Width - lineSize.Width) / 2;
                TextRenderer.DrawText(graphics, lineText, lineFont, new Point(posX, currentLineTop), Color.WhiteSmoke, KaraokeBackgroundColor);

                //draw portion of current phrase that's already been sung
                var line2 = lyrics.Where(lyr => !(lyr.LyricStart < currentLine.PhraseStart)).TakeWhile(lyr => !(lyr.LyricStart > time)).Aggregate("", (current, lyr) => current + " " + lyr.LyricText);
                line2 = ProcessLine(line2, true);
                if (!string.IsNullOrEmpty(line2))
                {
                    TextRenderer.DrawText(graphics, line2, lineFont, new Point(posX, currentLineTop), Color.LightSkyBlue, KaraokeBackgroundColor);
                }

                var lyricsList = lyrics.ToList();
                var wordList = new List<ActiveWord>();
                if (currentLine.PhraseStart <= time - 0.1)
                {
                    var word = "";
                    double wordStart = 0, wordEnd = 0;
                    var activeWord = new ActiveWord(word, wordStart, wordEnd);

                    for (int i = 0; i < lyricsList.Count(); i++)
                    {
                        var lyric = lyricsList[i];

                        // Skip lyrics outside the proper time
                        if (lyric.LyricStart < currentLine.PhraseStart || lyric.LyricStart > currentLine.PhraseEnd)
                        {
                            continue;
                        }
                        if (string.IsNullOrEmpty(word))
                        {
                            wordStart = lyric.LyricStart;
                        }
                        if (lyric.LyricText.Contains("-")) //is a syllable
                        {
                            word += ProcessLine(lyric.LyricText, true);
                            wordEnd = lyric.LyricStart + lyric.LyricDuration;                          
                            continue;
                        }
                        // Handle sustains
                        else if (!string.IsNullOrEmpty(word) && lyric.LyricText.Contains("+"))
                        {
                            //word += "+";
                            wordEnd = lyric.LyricStart + lyric.LyricDuration;

                            // Extend for consecutive sustains
                            for (var a = i + 1; a < lyricsList.Count; a++)
                            {
                                if (lyricsList[a].LyricText.Contains("+"))
                                {
                                    //word += "+"; // Append the sustain
                                    wordEnd = lyricsList[a].LyricStart + lyricsList[a].LyricDuration;
                                    i = a; // Update `i` to skip processed sustain notes
                                }
                                else
                                {
                                    break; // Exit sustain processing
                                }
                            }
                            continue; // Continue to process the next lyric
                        }
                        else
                        {
                            // Append regular lyrics to the word
                            word += ProcessLine(lyric.LyricText, true);
                            wordEnd = lyric.LyricStart + lyric.LyricDuration;

                            //look ahead to double check next lyric(s) aren't + sustains
                            for (var z = i + 1; z < lyricsList.Count - i - 1; z++)
                            {
                                if (lyricsList[z].LyricText.Contains("+"))
                                {
                                    //word += "+"; // Append the sustain
                                    wordEnd = lyricsList[z].LyricStart + lyricsList[z].LyricDuration;
                                    i = z;
                                    continue;
                                }
                                else
                                {
                                    i = z - 1;
                                    break;
                                }
                            }

                            // Finalize the word if it’s not a middle syllable
                            if (!string.IsNullOrEmpty(word))
                            {
                                wordList.Add(new ActiveWord(word.Trim(), wordStart, wordEnd));
                                word = ""; // Reset the word
                            }
                        }
                    }

                    // Find the active word matching playback time
                    activeWord = wordList.FirstOrDefault(w => w.WordStart <= time && w.WordEnd > time);

                    if (activeWord != null && !string.IsNullOrEmpty(activeWord.Text))
                    {
                        // Measure the word size for centering
                        lineFont = new Font("Tahoma", GetScaledFontSize(graphics, activeWord.Text, new Font("Tahoma", (float)12.0), 200));
                        lineSize = TextRenderer.MeasureText(activeWord.Text, lineFont);
                        posX = (picVisuals.Width - lineSize.Width) / 2;
                        var posY = (picVisuals.Height - lineSize.Height) / 2;

                        // Draw the entire word in white
                        TextRenderer.DrawText(graphics, activeWord.Text, lineFont, new Point(posX, posY), Color.WhiteSmoke, KaraokeBackgroundColor);
                        
                        // Calculate progress for the sung portion
                        var timeElapsed = time - activeWord.WordStart;
                        var progress = Clamp((float)(timeElapsed / (activeWord.WordEnd - activeWord.WordStart)), 0.0f, 1.0f);

                        // Determine the portion of the word to highlight
                        var numCharsToHighlight = (int)Math.Ceiling(progress * activeWord.Text.Length); // Ensure rounding up
                        numCharsToHighlight = Math.Min(numCharsToHighlight, activeWord.Text.Length);

                        // Extract the sung portion
                        var sungPortion = activeWord.Text.Substring(0, numCharsToHighlight);

                        // Overlay the sung portion in blue
                        if (!string.IsNullOrEmpty(sungPortion))
                        {
                            TextRenderer.DrawText(graphics, sungPortion, lineFont, new Point(posX, posY), Color.LightSkyBlue, KaraokeBackgroundColor);                            
                        }
                    }
                }                
            }
            if (nextLine != null && !string.IsNullOrEmpty(nextLine.PhraseText))
            {
                //draw entire next phrase on bottom
                lineText = ProcessLine(nextLine.PhraseText, true);
                lineFont = new Font("Tahoma", GetScaledFontSize(graphics, lineText, new Font("Tahoma", (float)12.0), 120));
                lineSize = TextRenderer.MeasureText(lineText, lineFont);
                posX = (picVisuals.Width - lineSize.Width) / 2;
                TextRenderer.DrawText(graphics, lineText, lineFont, new Point(posX, nextLineTop - lineSize.Height), Color.DarkGray, KaraokeBackgroundColor);
            }

            //draw waiting/countdown info
            if (currentLine != null && nextLine != null) return;
            if (lastLine != null && nextLine != null)
            {
                var difference = nextLine.PhraseStart - lastLine.PhraseEnd;
                if (difference < 5) return;
            }
            var middleText = "";
            var textColor = Color.DarkGray;
            if (currentLine == null && nextLine != null)
            {
                var wait = nextLine.PhraseStart - time;
                if (wait < 1.5) return;
                middleText = wait <= 5 ? "[GET READY]" : "[WAIT: " + ((int)(wait + 0.5)) + "]";
                textColor = wait <= 5 ? Color.LightGreen : Color.LightYellow;
            }
            else if (currentLine == null)
            {
                middleText = "[fin]";
            }
            lineFont = new Font("Tahoma", GetScaledFontSize(graphics, middleText, new Font("Tahoma", (float)12.0), 200));
            lineSize = TextRenderer.MeasureText(middleText, lineFont);
            posX = (picVisuals.Width - lineSize.Width) / 2;
            TextRenderer.DrawText(graphics, middleText, lineFont, new Point(posX, (picVisuals.Height - lineSize.Height) / 2), textColor, KaraokeBackgroundColor);
        }

        public float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public float GetScaledFontSize(Graphics g, string line, Font PreferedFont, float maxSize)
        {
            var maxWidth = picVisuals.Width * 0.95;
            var RealSize = g.MeasureString(line, PreferedFont);
            var ScaleRatio = maxWidth / RealSize.Width;
            var ScaledSize = PreferedFont.Size * ScaleRatio;
            if (ScaledSize > maxSize)
            {
                return maxSize;
            }
            return (float)ScaledSize;
        }

        private double GetCorrectedTime()
        {
            return PlaybackSeconds - ((double)BassBuffer / 1000) - ((double)PlayingSong.PSDelay / 1000);
        }

        public void ClearVisuals(bool clear_chart = false)
        {
            if (clear_chart && Chart != null)
            {
                Chart.Clear(chartSnippet.Checked ? TrackBackgroundColor1 : GetNoteColor(100));
            }
        }

        private void UpdateVisualStyle(object sender, EventArgs e)
        {
            ClearVisuals();
            UncheckAll();
            ((ToolStripMenuItem)sender).Checked = true;
            PrepareForDrawing();
        }

        private void UncheckAll()
        {
            chartFull.Checked = false;
            chartSnippet.Checked = false;
        }

        private void UpdateConsole(object sender, EventArgs e)
        {
            if (songLoader.IsBusy || batchSongLoader.IsBusy) return;
            if (Text.Contains("*"))
            {
                if (MessageBox.Show("You have unsaved changes on the current playlist\nAre you sure you want to do that?",
                    AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                {
                    return;
                }
            }

            var sentBy = (ToolStripMenuItem)sender;
            string newConsole;
            if (sentBy == xbox360)
            {
                newConsole = "xbox";
                consoleToolStripMenuItem.Text = "Your console: Xbox 360 (Rock Band)";
                SetDefaultPaths();
            }
            else if (sentBy == pS3)
            {
                newConsole = "ps3";
                consoleToolStripMenuItem.Text = "Your console: PlayStation 3 (Rock Band)";
            }
            else if (sentBy == wii)
            {
                newConsole = "wii";
                consoleToolStripMenuItem.Text = "Your console: Wii (Rock Band)";
            }
            else if (sentBy == yarg)
            {
                newConsole = "yarg";
                consoleToolStripMenuItem.Text = "Your console: PC (YARG / Clone Hero)";
            }
            else if (sentBy == rockSmith)
            {
                newConsole = "rocksmith";
                consoleToolStripMenuItem.Text = "Your console: PC (Rocksmith 2014)";
            }
            else if (sentBy == guitarHero)
            {
                newConsole = "guitarhero";
                consoleToolStripMenuItem.Text = "Your console: PC (GHWT:DE)";
            }
            else if (sentBy == fortNite)
            {
                newConsole = "fortnite";
                consoleToolStripMenuItem.Text = "Your console: PC (Fortnite Festival)";
            }
            else if (sentBy == powerGig)
            {
                newConsole = "powergig";
                consoleToolStripMenuItem.Text = "Your console: PC (Power Gig)";
            }
            else if (sentBy == bandFuse)
            {
                newConsole = "bandfuse";
                consoleToolStripMenuItem.Text = "Your console: Xbox 360 (BandFuse)";
            }
            else
            {
                return;
            }
            if (PlayerConsole == newConsole) return;
            Log("Updated console to " + newConsole);
            DeleteUsedFiles();
            xbox360.Checked = sentBy == xbox360;
            pS3.Checked = sentBy == pS3;
            wii.Checked = sentBy == wii;
            yarg.Checked = sentBy == yarg;
            rockSmith.Checked = sentBy == rockSmith;
            guitarHero.Checked = sentBy == guitarHero;
            fortNite.Checked = sentBy == fortNite;
            bandFuse.Checked = sentBy == bandFuse;
            powerGig.Checked = sentBy == powerGig;
            PlayerConsole = newConsole;
            StartNew(false);
        }

        private void PlaybackTimer_Tick(object sender, EventArgs e)
        {
            try
            {                
                if (Bass.BASS_ChannelIsActive(BassMixer) == BASSActive.BASS_ACTIVE_PLAYING)
                {
                    // the stream is still playing...                    
                    var pos = Bass.BASS_ChannelGetPosition(BassStream); // position in bytes
                    PlaybackSeconds = Bass.BASS_ChannelBytes2Seconds(BassStream, pos); // the elapsed time length                   

                    //calculate how many seconds are left to play
                    var time_left = ((double)PlayingSong.Length / 1000) - PlaybackSeconds;
                    if ((((!skipIntroOutroSilence.Checked || OutroSilence == 0.0) && time_left <= FadeLength) ||
                        (skipIntroOutroSilence.Checked && OutroSilence > 0.0 && PlaybackSeconds + FadeLength >= OutroSilence)) && !AlreadyFading) //skip to next song
                    {
                        Bass.BASS_ChannelSlideAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, 0, (int)FadeLength * 1000);
                        AlreadyFading = true;
                    }
                    if (skipIntroOutroSilence.Checked && OutroSilence > 0.0)
                    {
                        if (PlaybackSeconds >= OutroSilence)
                        {
                            goto GoToNextSong;
                        }
                    }
                    else if (PlaybackSeconds * 1000 >= PlayingSong.Length && btnShuffle.Tag.ToString() == "shuffle")
                    {
                        PlaybackTimer.Enabled = false;
                        DoShuffleSongs();
                        return;
                    }
                    UpdateTime();
                    DoPracticeSessions();
                    if (!displayKaraokeMode.Checked)
                    {
                        DrawLyrics();
                    }
                    if ((displayAlbumArt.Checked && File.Exists(CurrentSongArt)) || (!File.Exists(CurrentSongArt) && !displayAudioSpectrum.Checked))
                    {
                        picPreview.Invalidate();
                    }
                    if (displayBackgroundVideo.Checked && VideoIsPlaying && displayKaraokeMode.Checked)
                    {
                        KaraokeOverlay.Visible = MonitorApplicationFocus();
                        if (KaraokeOverlay.Visible)
                        {
                            KaraokeOverlay.Phrases = MIDITools.PhrasesVocals.Phrases;
                            KaraokeOverlay.Lyrics = MIDITools.LyricsVocals.Lyrics;
                            KaraokeOverlay.CorrectedTime = GetCorrectedTime();
                            KaraokeOverlay.Invalidate();
                        }
                    }
                    else
                    {
                        KaraokeOverlay.Visible = false;
                        picVisuals.Invalidate();
                    }
                    return;
                }
                if (btnLoop.Tag.ToString() == "loop")
                {
                    goto GoToNextSong;
                }
                else if (btnShuffle.Tag.ToString() != "shuffle")
                {
                    GetNextSong();
                }
                else
                {
                    return;
                }
            }
            catch (Exception)
            {
                return;
            }

        GoToNextSong:
            if (isChoosingStems) return;
            if (btnLoop.Tag.ToString() == "loop")
            {
                DoLoop();
            }
            else if (btnShuffle.Tag.ToString() == "shuffle")
            {
                DoShuffleSongs();
                return;
            }
            else
            {
                btnNext_Click(null, null);
                PlaybackTimer.Enabled = false;
            }
        }

        private void DoLoop()
        {
            PlaybackTimer.Enabled = false;
            StopPlayback();
            PlaybackSeconds = 0;
            StartPlayback(true, false);
        }

        private void DoPracticeSessions()
        {
            lblSections.Visible = showPracticeSections.Checked && MIDITools.PracticeSessions.Any();
            if (!openSideWindow.Checked) return;
            if (!showPracticeSections.Checked || !MIDITools.PracticeSessions.Any())
            {
                lblSections.Text = "";
                return;
            }
            lblSections.Text = GetCurrentSection(GetCorrectedTime());
        }

        private string GetCurrentSection(double time)
        {
            var curr_session = "";
            foreach (var session in MIDITools.PracticeSessions.TakeWhile(session => session.SectionStart <= time))
            {
                curr_session = session.SectionName;
            }
            return curr_session;
        }

        private void DrawLyrics()
        {
            if (!openSideWindow.Checked) return;
            if ((!doStaticLyrics && !doScrollingLyrics && !doKaraokeLyrics) || !MIDITools.PhrasesVocals.Phrases.Any())
            {
                ClearLyrics();
                UpdateLyricLabels();
                return;
            }
            lblHarm1.Invalidate();
            if (!doHarmonyLyrics) return;
            lblHarm2.Invalidate();
            lblHarm3.Invalidate();
        }

        private void DrawLyricsStatic(IEnumerable<LyricPhrase> phrases, Control label, Color color, Graphics graphics)
        {
            var time = GetCorrectedTime();
            label.Text = "";
            using (var pen = new SolidBrush(MediaPlayer.Visible ? picVisuals.BackColor : LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            LyricPhrase phrase = null;
            foreach (var lyric in phrases.TakeWhile(lyric => lyric.PhraseStart <= time).Where(lyric => lyric.PhraseEnd >= time))
            {
                phrase = lyric;
            }
            string line;
            try
            {
                line = phrase == null || string.IsNullOrEmpty(phrase.PhraseText.Trim()) ? GetMusicNotes() : ProcessLine(phrase.PhraseText, doWholeWordsLyrics);
            }
            catch (Exception)
            {
                line = GetMusicNotes();
            }
            var measure = TextRenderer.MeasureText(ProcessLine(line, doWholeWordsLyrics), label.Font);
            var left = (label.Width - measure.Width) / 2;
            TextRenderer.DrawText(graphics, line, label.Font, new Point(left, 0), color);
        }

        private void InitBASS()
        {
            Log("Initializing BASS and BASS_ASIO");
            //initialize BASS            
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                Log("Success");
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, BassBuffer);
                Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATEPERIOD, 50);
            }
            else
            {
                Log("BASS Error: " + Bass.BASS_ErrorGetCode());
                MessageBox.Show("Error initializing BASS\n" + Bass.BASS_ErrorGetCode(), AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool PrepMixerRB3(bool isM4A = false)
        {
            Log("Preparing audio mixer using RB3 mogg file");
            BassStreams.Clear();
            try
            {
                if(isM4A)
                {
                    BassStream = Bass.BASS_StreamCreateFile(activeM4AFile, 0L, 0L, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else
                {                    
                    BassStream = Bass.BASS_StreamCreateFile(nautilus.GetOggStreamIntPtr(), 0L, nautilus.PlayingSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                    BassStreams.Add(BassStream);
                }

                // create a decoder for the input file(s)
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);

                // create a stereo mixer with same frequency rate as the input file(s)
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_MIXER_END);//BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT | BASSFlag.BASS_MIXER_END);
                BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX | BASSFlag.BASS_MIXER_CHAN_BUFFER);

                if (isM4A)
                {
                    ActiveSongData.ChannelsDrums = 2;
                    ActiveSongData.ChannelsBassStart = 0;
                    ActiveSongData.ChannelsBass = 2;
                    ActiveSongData.ChannelsBassStart = 2;
                    ActiveSongData.ChannelsGuitar = 2;
                    ActiveSongData.ChannelsGuitarStart = 4;
                    ActiveSongData.ChannelsVocals = 2;
                    ActiveSongData.ChannelsVocalsStart = 6;
                    ActiveSongData.ChannelsTotal = 10;
                    ActiveSongData.AttenuationValues = "";
                    ActiveSongData.PanningValues = "";

                    var len = Bass.BASS_ChannelGetLength(BassStream);
                    var totaltime = Bass.BASS_ChannelBytes2Seconds(BassStream, len); // the total time length
                    ActiveSongData.Length = (int)(totaltime * 1000);
                    ActiveSong.Length = ActiveSongData.Length;
                    lblDuration.Text = Parser.GetSongDuration(ActiveSong.Length.ToString(CultureInfo.InvariantCulture));
                }

                //get and apply channel matrix
                var matrix = GetChannelMatrix(channel_info.chans);
                BassMix.BASS_Mixer_ChannelSetMatrix(BassStream, matrix);                
            }
            catch (Exception ex)
            {
                Log("Error preparing Rock Band mixer: " + ex.Message);
                return false;
            }
            Log("Success");
            return true;
        }

        private bool PrepMixerPS(IList<string> audioFiles, out int mixer, out List<int> NextSongStreams)
        {
            Log("Preparing audio mixer using YARG / Clone Hero file(s)");
            BassStreams.Clear();
            try
            {
                var audioFile = opusFiles.Any() ? opusFiles[0] : (mp3Files.Any() ? mp3Files[0] : (wavFiles.Any() ? wavFiles[0] : oggFiles[0]));
                if (opusFiles.Any())
                {
                    BassStream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }
                else //OGG or MP3 or WAV
                {
                    BassStream = Bass.BASS_StreamCreateFile(audioFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
                }

                // create a decoder for the audio file(s)
                var channel_info = Bass.BASS_ChannelGetInfo(BassStream);

                // create a stereo mixer with same frequency rate as the input file
                BassMixer = BassMix.BASS_Mixer_StreamCreate(channel_info.freq, 2, BASSFlag.BASS_MIXER_END);//BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);

                var folder = Path.GetDirectoryName(audioFile) + "\\";
                var ext = opusFiles.Any() ? "opus" : (mp3Files.Any() ? "mp3" : (wavFiles.Any() ? "wav" : "ogg"));
                var drums = folder + "drums." + ext;
                var drums1 = folder + "drums_1." + ext;
                var drums2 = folder + "drums_2." + ext;
                var drums3 = folder + "drums_3." + ext;
                var drums4 = folder + "drums_4." + ext;
                var bass = folder + "bass." + ext;
                var rhythm = folder + "rhythm." + ext;
                var guitar = folder + "guitar." + ext;
                var guitar1 = folder + "guitar_1." + ext;
                var guitar2 = folder + "guitar_2." + ext;
                var keys = folder + "keys." + ext;
                var vocals = folder + "vocals." + ext;
                var vocals1 = folder + "vocals_1." + ext;
                var vocals2 = folder + "vocals_2." + ext;
                var backing = folder + "backing." + ext;
                var song = folder + "song." + ext;
                var crowd = folder + "crowd." + ext;

                if (File.Exists(drums) || File.Exists(drums1) || File.Exists(drums2) || File.Exists(drums3) || File.Exists(drums4))
                {
                    Parser.Songs[0].ChannelsDrums = 2; //don't matter as long as it's more than 0 to enable it
                }
                if (File.Exists(bass) || File.Exists(rhythm))
                {
                    Parser.Songs[0].ChannelsBass = 2;
                }
                if (File.Exists(guitar) || File.Exists(guitar1) || File.Exists(guitar2))
                {
                    Parser.Songs[0].ChannelsGuitar = 2;
                }
                if (File.Exists(keys))
                {
                    Parser.Songs[0].ChannelsKeys = 2;
                }
                if (File.Exists(vocals) || File.Exists(vocals1) || File.Exists(vocals2))
                {
                    Parser.Songs[0].ChannelsVocals = 2;
                }
                if (File.Exists(crowd))
                {
                    Parser.Songs[0].ChannelsCrowd = 2;
                }

                if (doAudioDrums)
                {
                    if (File.Exists(drums))
                    {
                        AddAudioToMixer(drums);
                    }
                    else
                    {
                        var split_drums = new List<string> { drums1, drums2, drums3, drums4 };
                        foreach (var drum in split_drums.Where(File.Exists))
                        {
                            AddAudioToMixer(drum);
                        }
                    }
                }
                if (doAudioBass)
                {
                    if (File.Exists(bass))
                    {
                        AddAudioToMixer(bass);
                    }
                    else if (File.Exists(rhythm))
                    {
                        AddAudioToMixer(rhythm);
                    }
                }
                if (doAudioGuitar)
                {
                    if (File.Exists(guitar))
                    {
                        AddAudioToMixer(guitar);
                    }
                    else
                    {
                        var split_guitar = new List<string> { guitar1, guitar2 };
                        foreach (var gtr in split_guitar.Where(File.Exists))
                        {
                            AddAudioToMixer(gtr);
                        }
                    }
                    if (File.Exists(rhythm) && !File.Exists(bass))
                    {
                        AddAudioToMixer(rhythm);
                    }
                }
                if (doAudioKeys && File.Exists(keys))
                {
                    AddAudioToMixer(keys);
                }
                if (doAudioVocals)
                {
                    if (File.Exists(vocals))
                    {
                        AddAudioToMixer(vocals);
                    }
                    else
                    {
                        var split_vocals = new List<string> { vocals1, vocals2 };
                        foreach (var vocal in split_vocals.Where(File.Exists))
                        {
                            AddAudioToMixer(vocal);
                        }
                    }
                }
                if (doAudioBacking)
                {
                    if (File.Exists(backing))
                    {
                        AddAudioToMixer(backing);
                    }
                    else if (File.Exists(song))
                    {
                        AddAudioToMixer(song);
                    }
                    else if (audioFiles[0] == guitar)
                    {
                        AddAudioToMixer(guitar);
                    }
                }
                if (doAudioCrowd && File.Exists(crowd))
                {
                    AddAudioToMixer(crowd);
                }
            }
            catch (Exception ex)
            {
                Log("Error preparing mixer: " + ex.Message);
                mixer = 0;
                NextSongStreams = null;
                return false;
            }
            Log("Success");
            Log("Added " + BassStreams.Count + " file(s) to the mixer");
            mixer = BassMixer;
            NextSongStreams = BassStreams;
            return true;
        }

        private void AddAudioToMixer(string audioFile)
        {
            Log("Adding audio file to mixer: " + audioFile);
            if (opusFiles.Any())
            {
                BassStream = BassOpus.BASS_OPUS_StreamCreateFile(audioFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            else //ogg or mp3 or wav
            {
                BassStream = Bass.BASS_StreamCreateFile(audioFile, 0, 0, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            }
            var stream_info = Bass.BASS_ChannelGetInfo(BassStream);
            if (stream_info.chans == 0) return;
            BassMix.BASS_Mixer_StreamAddChannel(BassMixer, BassStream, BASSFlag.BASS_MIXER_MATRIX);
            BassStreams.Add(BassStream);
        }

        private void StartPlayback(bool doFade, bool doNext, bool PlayAudio = true)
        {
            Log("Starting playback");
            if (PlayAudio)
            {
                if ((!yarg.Checked && !fortNite.Checked && !guitarHero.Checked && !powerGig.Checked && !bandFuse.Checked) && (CurrentSongAudio == null || CurrentSongAudio.Length == 0))
                {
                    if (AlreadyTried || lstPlaylist.SelectedItems.Count == 0)
                    {
                        var msg = "Audio file (*.mogg) for song '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' is missing";
                        Log(msg);
                        MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        StopPlayback();
                        AlreadyTried = false;
                    }
                    else
                    {
                        AlreadyTried = true;
                        lstPlaylist_MouseDoubleClick(null, null);
                    }
                    return;
                }

                var directory = Path.GetDirectoryName(PlayingSong.Location);
                if (yarg.Checked && !string.IsNullOrEmpty(sngPath))
                {
                    directory = Application.StartupPath + "\\bin\\temp\\";
                }
                else if (rockSmith.Checked && !string.IsNullOrEmpty(psarcPath))
                {
                    directory = psarcPath;
                }
                else if (guitarHero.Checked && !string.IsNullOrEmpty(ghwtPath))
                {
                    directory = ghwtPath;
                }
                else if (powerGig.Checked && !string.IsNullOrEmpty(XMA_EXT_PATH))
                {
                    directory = XMA_EXT_PATH;
                }
                else if (bandFuse.Checked && !string.IsNullOrEmpty(BandFusePath))
                {
                    directory = Application.StartupPath + "\\bin\\temp\\";
                }
                oggFiles = Directory.GetFiles(directory, "*.ogg", SearchOption.TopDirectoryOnly);
                opusFiles = Directory.GetFiles(directory, "*.opus", SearchOption.TopDirectoryOnly);
                mp3Files = Directory.GetFiles(directory, "*.mp3", SearchOption.TopDirectoryOnly);
                wavFiles = Directory.GetFiles(directory, "*.wav", SearchOption.TopDirectoryOnly);

                if (fortNite.Checked && !string.IsNullOrEmpty(activeM4AFile))
                {
                    if (!PrepMixerRB3(true))
                    {
                        MessageBox.Show("Error preparing audio mixer - can't play that song", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        StopPlayback();
                        return;
                    }
                }
                else if ((yarg.Checked || fortNite.Checked || guitarHero.Checked || powerGig.Checked || bandFuse.Checked) && (oggFiles.Any() || opusFiles.Any() || mp3Files.Any() || wavFiles.Any()))
                {
                    List<string> AudioFiles;
                    if (opusFiles.Any())
                    {
                        if (!opusFiles.Any())
                        {
                            var msg = "Audio files (*.opus) for song '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' are missing";
                            Log(msg);
                            MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            StopPlayback();
                            return;
                        }
                        AudioFiles = opusFiles.ToList();
                    }
                    else if (mp3Files.Any())
                    {
                        if (!mp3Files.Any())
                        {
                            var msg = "Audio files (*.mp3) for song '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' are missing";
                            Log(msg);
                            MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            StopPlayback();
                            return;
                        }
                        AudioFiles = mp3Files.ToList();
                    }
                    else if (wavFiles.Any())
                    {
                        if (!wavFiles.Any())
                        {
                            var msg = "Audio files (*.wav) for song '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' are missing";
                            Log(msg);
                            MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            StopPlayback();
                            return;
                        }
                        AudioFiles = wavFiles.ToList();
                    }
                    else
                    {
                        if (!oggFiles.Any())
                        {
                            var msg = "Audio files (*.ogg) for song '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' are missing";
                            Log(msg);
                            MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            StopPlayback();
                            return;
                        }
                        AudioFiles = oggFiles.ToList();
                    }
                    int mixer;
                    List<int> streams;
                    if (!PrepMixerPS(AudioFiles, out mixer, out streams))
                    {
                        const string msg = "Error preparing audio mixer - can't play that song";
                        Log(msg);
                        MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        StopPlayback();
                        return;
                    }                
                }
                else
                {
                    if (File.Exists(CurrentSongAudioPath))
                    {
                        if (Path.GetExtension(CurrentSongAudioPath) == ".mogg")
                        {
                            oggPath = CurrentSongAudioPath.Replace(".mogg", ".ogg");
                            if (!nautilus.DecM(File.ReadAllBytes(CurrentSongAudioPath), false, doNext, DecryptMode.ToFile, oggPath))
                            {
                                var msg = "Audio file for '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' is encrypted, can't play it";
                                Log(msg);
                                MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                StopPlayback();
                                return;
                            }
                        }
                        else if (Path.GetExtension(CurrentSongAudioPath) == ".yarg_mogg")
                        {
                            oggPath = CurrentSongAudioPath.Replace(".yarg_mogg", ".ogg");
                            if (!nautilus.DecY(CurrentSongAudioPath, DecryptMode.ToFile, oggPath))
                            {
                                var msg = "Audio file for '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' is encrypted, can't play it";
                                Log(msg);
                                MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                                StopPlayback();
                                return;
                            }
                            else
                            {
                                nautilus.RemoveMHeader(File.ReadAllBytes(oggPath), doNext, DecryptMode.ToFile, oggPath);
                            }
                        }
                    }
                    if (nautilus.PlayingSongOggData == null && nautilus.NextSongOggData != null)
                    {
                        nautilus.PlayingSongOggData = nautilus.NextSongOggData;
                    }
                    if (nautilus.PlayingSongOggData == null || nautilus.PlayingSongOggData.Length == 0)
                    {
                        if (!nautilus.DecM(CurrentSongAudio, false, false, DecryptMode.ToFile, oggPath))
                        {
                            var msg = "Audio file for '" + PlayingSong.Artist + " - " + PlayingSong.Name + "' is encrypted, can't play it";
                            Log(msg);
                            MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            StopPlayback();
                            return;
                        }
                    }
                    if (!PrepMixerRB3())
                    {
                        MessageBox.Show("Error preparing audio mixer - can't play that song", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        StopPlayback();
                        return;
                    }
                }

                IntroSilence = skipIntroOutroSilence.Checked ? IntroSilenceNext : 0.0;
                OutroSilence = skipIntroOutroSilence.Checked ? OutroSilenceNext : 0.0;
                IntroSilenceNext = 0.0;
                OutroSilenceNext = 0.0;
                if (PlaybackSeconds == 0 && skipIntroOutroSilence.Checked)
                {
                    PlaybackSeconds = IntroSilence;
                }

                SetPlayLocation(PlaybackSeconds);
                Log("Playback start time: " + PlaybackSeconds + " seconds");

                //apply volume correction to entire track
                var track_vol = (float)Utils.DBToLevel(Convert.ToDouble(-1 * VolumeLevel), 1.0);
                if (doFade) //enable fade-in
                {
                    Log("Fade-in enabled");
                    Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, 0);
                    Bass.BASS_ChannelSlideAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol, (int)(FadeLength * 1000));
                }
                else //no fade-in
                {
                    Log("Fade-in disabled");
                    Bass.BASS_ChannelSetAttribute(BassMixer, BASSAttribute.BASS_ATTRIB_VOL, track_vol);
                }

                //start video playback if possible
                if (yarg.Checked && displayBackgroundVideo.Checked)
                {
                    StartVideoPlayback();
                }
                //start mix playback
                if (!Bass.BASS_ChannelPlay(BassMixer, false))
                {
                    MessageBox.Show("Error starting BASS playback:\n" + Bass.BASS_ErrorGetCode(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            PrepareForDrawing();
            UpdatePlaybackStuff();
            UpdateStats();

            if (doNext && btnLoop.Tag.ToString() != "loop")
            {
                //GetNextSong();
            }
        }

        private void SetVideoPlayerPath(string ini)
        {
            Log("Video not yet loaded, trying to load");
            MediaPlayer.URL = "";
            var video_path = "";
            if (string.IsNullOrEmpty(ini) || !File.Exists(ini)) return;
            var sr = new StreamReader(ini);
            while (sr.Peek() >= 0)
            {   //read value for Phase Shift entry in song.ini
                var line = sr.ReadLine();
                if (!line.Contains("video =") && !line.Contains("video=")) continue;
                video_path = Path.GetDirectoryName(ini) + "\\" + Tools.GetConfigString(line).Trim();
                break;
            }
            sr.Dispose();
            if (string.IsNullOrEmpty(video_path) || !File.Exists(video_path))
            {
                var path = Path.GetDirectoryName(ini); //default
                //search for mid file in YARG exCON folder, video should be where the .mid file is
                var possible_folder = Directory.GetFiles(Path.GetDirectoryName(ini), "*.mid", SearchOption.AllDirectories);
                if (possible_folder.Any())
                {
                    path = Path.GetDirectoryName(possible_folder[0]);
                }
                var backgrounds = Directory.GetFiles(path);
                for (var i = 0; i < backgrounds.Count(); i++)
                {
                    switch (Path.GetFileName(backgrounds[i]).ToLowerInvariant())
                    {
                        case "background.avi":
                        case "video.mp4":
                        case "video.webm":
                        case "bg.mp4":
                        case "bg.webm":
                            video_path = backgrounds[i];
                            break;
                        default:
                            break;
                    }
                }
            }
            if (string.IsNullOrEmpty(video_path)) return;
            MediaPlayer.URL = video_path;
        }

        private void StartVideoPlayback()
        {
            if (PlayingSong == null) return;
            if (string.IsNullOrEmpty(MediaPlayer.URL))
            {
                SetVideoPlayerPath(string.IsNullOrEmpty(sngPath) ? PlayingSong.Location : Application.StartupPath + "\\temp\\song.ini");
            }
            if (string.IsNullOrEmpty(MediaPlayer.URL)) return;
            Log("Starting video playback");
            VideoIsPlaying = true;
            ClearVisuals();
            MediaPlayer.Visible = true;
            MediaPlayer.BringToFront();            
            MediaPlayer.Ctlcontrols.play(); 
            MediaPlayer.Ctlcontrols.currentPosition = PlaybackSeconds;
        }

        public void SetPlayLocation(double time, bool seeking = false)
        {
            if (time < 0)
            {
                time = 0.0;
            }

            if (time < 0)
            {
                time = 0.0;
            }
            if ((MediaPlayer.playState == WMPPlayState.wmppsPlaying || MediaPlayer.playState == WMPPlayState.wmppsPaused) && !seeking)
            {
                MediaPlayer.Ctlcontrols.currentPosition = time;
            }
            if ((opusFiles.Any() || oggFiles.Any() || wavFiles.Any() || mp3Files.Any()) && BassStreams.Count() > 1)
            {
                foreach (var stream in BassStreams)
                {
                    try
                    {
                        BassMix.BASS_Mixer_ChannelSetPosition(stream, Bass.BASS_ChannelSeconds2Bytes(stream, time));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error setting play location: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                try
                {
                    BassMix.BASS_Mixer_ChannelSetPosition(BassStream, Bass.BASS_ChannelSeconds2Bytes(BassStream, time));
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error setting play location: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }            
        }

        private void UpdatePlaybackStuff()
        {
            ClearLyrics();
            btnPlayPause.Text = "PAUSE";
            btnPlayPause.Tag = "pause";
            toolTip1.SetToolTip(btnPlayPause, "Pause");
            btnPlayPause.BackColor = Color.LightBlue;
            UpdateNotifyTray();
            btnPlayPause.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnPlayPause.BackColor);
            PlaybackTimer.Enabled = true;
        }

        private void StopPlayback(bool Pause = false)
        {
            try
            {
                PlaybackTimer.Enabled = false;
                if (Pause)
                {
                    Log("Pausing playback");
                    if (!Bass.BASS_ChannelPause(BassMixer))                    
                    {
                        Log("Error pausing playback: " + Bass.BASS_ErrorGetCode());
                        MessageBox.Show("Error pausing playback\n" + Bass.BASS_ErrorGetCode());
                    }
                    if (MediaPlayer.playState == WMPPlayState.wmppsPlaying)
                    {
                        MediaPlayer.Ctlcontrols.pause();
                    }
                }
                else
                {
                    Log("Stopping playback");
                    StopVideoPlayback();
                    StopBASS();
                }                
            }
            catch (Exception ex)
            {
                Log("Error stopping playback: " + ex.Message);
            }
            btnPlayPause.Text = "PLAY";
            btnPlayPause.Tag = "play";
            toolTip1.SetToolTip(btnPlayPause, "Play");
            btnPlayPause.BackColor = Color.PaleGreen;
            btnPlayPause.FlatAppearance.MouseOverBackColor = Tools.LightenColor(btnPlayPause.BackColor);
        }

        private void StopVideoPlayback(bool stop = true)
        {
            if (MediaPlayer.playState != WMPPlayState.wmppsPlaying && MediaPlayer.playState != WMPPlayState.wmppsPaused) return;
            Log("Stopping video playback");
            if (stop)
            {
                MediaPlayer.Ctlcontrols.stop();
                MediaPlayer.close();
            }
            else
            {
                MediaPlayer.Ctlcontrols.pause();
            }
            MediaPlayer.Visible = false;
            VideoIsPlaying = false;
        }

        private void StopBASS()
        {            
            Log("Releasing BASS resources");
            try
            {
                Bass.BASS_ChannelStop(BassMixer);
                Bass.BASS_StreamFree(BassMixer);
                Bass.BASS_StreamFree(BassStream);
                foreach (var stream in BassStreams)
                {
                    Bass.BASS_StreamFree(stream);
                }
            }
            catch (Exception ex)
            {
                Log("Error stopping BASS: " + ex.Message);
            }
        }

        private string GetMusicNotes()
        {
            //"♫ ♫ ♫ ♫"
            var quarter = (int)((PlaybackSeconds - (int) PlaybackSeconds) * 100);
            string notes;
            if (quarter >= 0 && quarter < 25)
            {
                notes = "♫";
            }
            else if (quarter >= 25 && quarter < 50)
            {
                notes = "♫ ♫";
            }
            else if (quarter >= 50 && quarter < 75)
            {
                notes = "♫ ♫ ♫";
            }
            else
            {
                notes = "♫ ♫ ♫ ♫";
            }
            return notes;
        }

        public string ProcessLine(string line, bool clean)
        {
            if (line == null) return "";
            string newline;
            if (clean)
            {
                newline = line.Replace("$", "");
                newline = newline.Replace("#", "");
                newline = newline.Replace("^", "");
                newline = newline.Replace("- + ", "");
                newline = newline.Replace("+- ", "");
                newline = newline.Replace("- ", "");
                newline = newline.Replace(" + ", " ");
                newline = newline.Replace(" +", "");
                newline = newline.Replace("+ ", "");
                newline = newline.Replace("+-", "");
                newline = newline.Replace("=", "-");
                newline = newline.Replace("§", "‿");
                newline = newline.Replace("- ", "-").Trim();
                if (newline.EndsWith("+", StringComparison.Ordinal))
                {
                    newline = newline.Substring(0, newline.Length - 1).Trim();
                }
                if (newline.EndsWith("-", StringComparison.Ordinal))
                {
                    newline = newline.Substring(0, newline.Length - 1);
                }
            }
            else
            {
                newline = line;
            }
            return newline.Replace("/","").Trim();
        }

        public void UpdateDisplay(bool PrepareToDraw = true)
        {
            if (isClosing) return;
            if (PrepareToDraw)
            {
                PrepareForDrawing();
            }
            var doShow = openSideWindow.Checked;
            Width = doShow ? 1000 : 412;
            lblUpdates.Width = doShow ? 590 : 184;
            lblUpdates.Left = (openSideWindow.Checked? picVisuals.Left + picVisuals.Width : panelPlaying.Left + panelPlaying.Width) - lblUpdates.Width;
            picVisuals.Image = displayAlbumArt.Checked && File.Exists(CurrentSongArtBlurred) ? Tools.NemoLoadImage(CurrentSongArtBlurred) : null;
            lblSections.Parent = picVisuals;
            lblSections.Visible = showPracticeSections.Checked && MIDITools.PracticeSessions.Any();
            lblSections.BackColor = yarg.Checked && displayBackgroundVideo.Checked && !string.IsNullOrEmpty(MediaPlayer.URL) ? Color.Black : LabelBackgroundColor;
            lblSections.Refresh();
            UpdateLyricLabels();
            
            MediaPlayer.Parent = picVisuals;
            MediaPlayer.Top = lblSections.Visible ? lblSections.Height : 0;
            MediaPlayer.Left = 0;
            MediaPlayer.Height = picVisuals.Height - GetHeightDiff();
            MediaPlayer.Width = picVisuals.Width;
            if (showMIDIVisuals.Checked)
            {
                PlaybackTimer.Interval = 15;
            }
            else if (displayKaraokeMode.Checked)
            {
                PlaybackTimer.Interval = 30;
            }
            else
            {
                PlaybackTimer.Interval = 50;
            }
        }

        private int GetHeightDiff()
        {
            var heightDiff = 0;
            if (lblSections.Visible)
            {
                heightDiff += lblSections.Height;
            }
            if (lblHarm3.Visible)
            {
                heightDiff += lblHarm1.Height * 3;
            }
            else if (lblHarm2.Visible)
            {
                heightDiff += lblHarm1.Height * 2;
            }
            else if (lblHarm1.Visible)
            {
                heightDiff += lblHarm1.Height - 1;
            }
            return heightDiff;
        }

        private static void UpdateTextQuality(Graphics graphics)
        {
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
        }

        public void UpdateLyricLabels()
        {
            ClearNoteColors(true);
            var no_lyrics = !doScrollingLyrics && !doStaticLyrics && !doKaraokeLyrics;
            if (MIDITools.LyricsVocals == null || !MIDITools.LyricsVocals.Lyrics.Any() || (displayKaraokeMode.Enabled && displayKaraokeMode.Checked))
            {
                lblHarm1.Visible = false;
                lblHarm2.Visible = false;
                lblHarm3.Visible = false;
            }
            else
            {
                lblHarm1.Visible = !no_lyrics && (MIDITools.LyricsVocals.Lyrics.Any() || MIDITools.LyricsHarm1.Lyrics.Any());
                lblHarm2.Visible = doHarmonyLyrics && MIDITools.LyricsHarm2.Lyrics.Any();
                lblHarm3.Visible = doHarmonyLyrics && MIDITools.LyricsHarm3.Lyrics.Any();
            }
            if (lblHarm3.Visible)
            {
                lblHarm2.Top = lblHarm3.Top - lblHarm2.Height + 1;
                lblHarm1.Top = lblHarm2.Top - lblHarm1.Height + 1;
            }
            else if (lblHarm2.Visible)
            {
                lblHarm2.Top = lblHarm3.Top;
                lblHarm1.Top = lblHarm2.Top - lblHarm1.Height + 1;
            }
            else
            {
                lblHarm1.Top = lblHarm3.Top;
            }
            lblHarm1.ForeColor = !displayMIDIChartVisuals.Checked || !chartSnippet.Checked ? Color.White : Harm1Color;
            lblHarm2.ForeColor = Harm2Color;
            lblHarm3.ForeColor = Harm3Color;
            lblHarm1.Refresh();
            lblHarm2.Refresh();
            lblHarm3.Refresh();
        }
        
        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var message = Tools.ReadHelpFile("pl");
            var help = new HelpForm(AppName + " - Help", message, true);
            help.ShowDialog();
            Log("Displayed help file");
        }

        private void folderScanner_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Folder scanner working");
            Log("Searching for " + GetCurrentDataType() + " files");

            var files = Directory.GetFiles(Environment.CurrentDirectory, "*.*", SearchOption.AllDirectories);

            if (xbox360.Checked || bandFuse.Checked)
            {
                SongsToAdd.AddRange(
                    files.Where(file =>
                    {
                        try
                        {
                            return VariousFunctions.ReadFileType(file) == XboxFileType.STFS;
                        }
                        catch
                        {                            
                            return false; // Skip this file on error
                        }
                    }).ToList());
            }
            else if (yarg.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "song.ini").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".sng").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".yargsong").ToList());
            }
            else if (pS3.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".pkg").ToList());
            }
            else if (rockSmith.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".psarc").ToList());
            }
            else if (wii.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "songs.dta").ToList());
            }
            else if (guitarHero.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetFileName(file) == "song.ini").ToList());
            }
            else if (fortNite.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".fnf").ToList());
            }
            else if (powerGig.Checked)
            {
                SongsToAdd.AddRange(files.Where(file => Path.GetExtension(file) == ".xml").ToList());
            }
            Log("Found " + SongsToAdd.Count + " " + GetCurrentDataType() + " file(s)");
        }

        private string GetCurrentDataType()
        {
            string type;
            if (xbox360.Checked)
            {
                type = "CON | LIVE";
            }
            else if (yarg.Checked)
            {
                type = "song.ini | songs.dta | .sng | .yargsong";
            }
            else if (pS3.Checked)
            {
                type = "songs.dta | .pkg";
            }
            else if (rockSmith.Checked)
            {
                type = ".psarc";
            }
            else if (fortNite.Checked)
            {
                type = ".fnf | .m4a";
            }
            else if (guitarHero.Checked)
            {
                type = "song.ini | fsb.xen";
            }
            else if (powerGig.Checked)
            {
                type = ".xml";
            }
            else if (bandFuse.Checked)
            {
                type = "LIVE";
            }
            else
            {
                type = "songs.dta";
            }
            return type;
        }

        private void folderScanner_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Folder scanner finished");
            if (GIFOverlay != null)
            {
                GIFOverlay.Close();
                GIFOverlay = null;
            }
            var type = GetCurrentDataType();
            if (!SongsToAdd.Any())
            {
                var msg = "No " + type + " files found in that folder, nothing to add to the playlist";
                Log(msg);
                MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                isScanning = false;
                EnableDisable(true);
                return;
            }
            var found = "Found " + SongsToAdd.Count + " " + type + " " + (SongsToAdd.Count == 1 ? "file" : "files") + ", analyzing...";
            Log(found);
            ShowUpdate(found);
            StartingCount = lstPlaylist.Items.Count;
            isScanning = true;
            UpdateNotifyTray();
            InitiateGIFOverlay();
            batchSongLoader.RunWorkerAsync();
        }

        private void cancelProcess_Click(object sender, EventArgs e)
        {
            if (!batchSongLoader.IsBusy && !songLoader.IsBusy) return;
            CancelWorkers = true;
            Log("User cancelled running process");
        }

        private void openFileLocation_Click(object sender, EventArgs e)
        {
            var file = ActiveSong.Location;
            Process.Start("explorer" + EXE, "/select," + "\"" + file + "\"");
            Log("Opened file in location: " + file);
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (lblUpdates.InvokeRequired)
            {
                lblUpdates.Invoke(new MethodInvoker(() => lblUpdates.Text = ""));
            }
            else
            {
                UpdateStats();
            }
            UpdateTimer.Enabled = false;
        }

        private void UpdateStats()
        {
            lblUpdates.Text = "";
            if (lstPlaylist.Items.Count == 0) return;
            
            try
            {
                long time = 0;
                for (var i = 0; i < lstPlaylist.Items.Count; i++)
                {
                    var ind = Convert.ToInt16(lstPlaylist.Items[i].SubItems[0].Text) - 1;
                    time += Playlist[ind].Length;
                }
                lblUpdates.Text = "Songs: " + lstPlaylist.Items.Count;
                if (openSideWindow.Checked && string.IsNullOrEmpty(activeM4AFile))
                {
                    lblUpdates.Text = lblUpdates.Text + "   |   Playing Time: " + Parser.GetSongDuration(time.ToString(CultureInfo.InvariantCulture));
                }
                Log(lblUpdates.Text);
            }
            catch (Exception ex)
            {
                Log("Error in UpdateStats: " + ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private void PlaylistContextMenu_Opening(object sender, CancelEventArgs e)
        {
            PlaylistContextMenu.Enabled = !songExtractor.IsBusy && !songPreparer.IsBusy;
            if (GIFOverlay != null || lstPlaylist.Items.Count == 0)
            {
                e.Cancel = true;
                return;
            }

            playNowToolStripMenuItem.Visible = lstPlaylist.SelectedItems.Count == 1;
            playNextToolStripMenuItem.Visible = lstPlaylist.SelectedItems.Count == 1 && PlayingSong != null;
            removeToolStripMenuItem.Visible = lstPlaylist.SelectedItems.Count > 0;
            moveUpToolStripMenuItem.Visible = lstPlaylist.SelectedItems.Count == 1;
            moveDownToolStripMenuItem.Visible = lstPlaylist.SelectedItems.Count == 1;
            goToArtist.Visible = lstPlaylist.SelectedItems.Count == 1;
            markAsPlayed.Visible = false;
            markAsUnplayed.Visible = false;
            goToAlbum.Visible = false;
            goToGenre.Visible = false;
            returnToPlaylist.Visible = Playlist.Count != StaticPlaylist.Count;
            sortPlaylistByArtist.Visible = lstPlaylist.Items.Count > 0;
            sortPlaylistByDuration.Visible = lstPlaylist.Items.Count > 0 && !m4aFiles.Any();
            sortPlaylistBySong.Visible = lstPlaylist.Items.Count > 0;
            randomizePlaylist.Visible = lstPlaylist.Items.Count > 0;
            startInstaMix.Visible = lstPlaylist.Items.Count > 0;
            openFileLocation.Visible = lstPlaylist.SelectedItems.Count == 1;
            try
            {
                var index = lstPlaylist.SelectedIndices[0];
                if (index == 0)
                {
                    moveUpToolStripMenuItem.Visible = false;
                }
                if (index == lstPlaylist.Items.Count - 1)
                {
                    moveDownToolStripMenuItem.Visible = false;
                }
                if (btnPlayPause.Tag.ToString() == "pause" && PlayingSong.Index == index)
                {
                    playNextToolStripMenuItem.Visible = false;
                }
                var ind = Convert.ToInt16(lstPlaylist.Items[index].SubItems[0].Text) - 1;
                goToAlbum.Visible = lstPlaylist.SelectedItems.Count == 1 && !string.IsNullOrEmpty(Playlist[ind].Album);
                goToGenre.Visible = lstPlaylist.SelectedItems.Count == 1 && !string.IsNullOrEmpty(Playlist[ind].Genre);
                markAsPlayed.Visible = lstPlaylist.SelectedItems.Count > 1 ||
                                       (lstPlaylist.SelectedItems.Count == 1 &&
                                        lstPlaylist.SelectedItems[0].Tag.ToString() == "0");
                markAsUnplayed.Visible = lstPlaylist.SelectedItems.Count > 1 ||
                                         (lstPlaylist.SelectedItems.Count == 1 &&
                                          lstPlaylist.SelectedItems[0].Tag.ToString() == "1");
            }
            catch (Exception ex)
            {
                Log("Error in PlaylistContextMenu_Opening:" + ex.Message);
            }
        }

        private void NotifyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            restoreToolStripMenuItem.Visible = WindowState == FormWindowState.Minimized;
            playToolStripMenuItem.Visible = btnPlayPause.Tag.ToString() == "play" && btnPlayPause.Enabled;
            pauseToolStripMenuItem.Visible = btnPlayPause.Tag.ToString() == "pause" && btnPlayPause.Enabled;
            nextToolStripMenuItem.Visible = btnNext.Enabled;
        }

        private void returnToPlaylist_Click(object sender, EventArgs e)
        {
            Log("Current playlist has " + Playlist.Count + " song(s)");
            Log("Restoring full playlist which has " + StaticPlaylist.Count + " song(s)");
            Playlist = StaticPlaylist;
            btnClear.PerformClick();
            ReloadPlaylist(Playlist);
            UpdateHighlights();
        }

        private void txtSearch_EnabledChanged(object sender, EventArgs e)
        {
            btnClear.Enabled = txtSearch.Enabled;
            btnSearch.Enabled = txtSearch.Enabled;
            btnGoTo.Enabled = txtSearch.Enabled;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Type to search playlist..." || btnClear.ForeColor != EnabledButtonColor) return;
            txtSearch.Invoke(new MethodInvoker(() => txtSearch.Text = "Type to search playlist..."));
            if (lstPlaylist.Items.Count != StaticPlaylist.Count)
            {
                ReloadPlaylist(Playlist, true, true, false);
            }
            if (PlayingSong == null) return;
            UpdateHighlights();
        }

        private void scanForSongsAutomatically_Click(object sender, EventArgs e)
        {
            if (batchSongLoader.IsBusy)
            {
                MessageBox.Show("Wait until I finish loading the last batch of songs added", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var ofd = new FolderPicker
            {
                Title = "Select folder to scan for songs",
                InputPath = Environment.CurrentDirectory,
            };
            if (ofd.ShowDialog(IntPtr.Zero) != true || string.IsNullOrEmpty(ofd.ResultPath)) return;
            Environment.CurrentDirectory = ofd.ResultPath;
            if (MessageBox.Show("This might take a while depending on how many subfolders and how many files are in the folder\nAre you sure you want to do this now?",
                    AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }
            EnableDisable(false);
            SongsToAdd.Clear();
            Log("scanForSongsAutomatically_Click");
            isScanning = true;
            UpdateNotifyTray();
            InitiateGIFOverlay();
            folderScanner.RunWorkerAsync();
        }

        private void selectAndAddSongsManually_Click(object sender, EventArgs e)
        {
            if (batchSongLoader.IsBusy)
            {
                MessageBox.Show("Wait until I finish loading the last batch of songs added", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            Log("selectAndAddSongsManually_Click");
            
            var ofd = new OpenFileDialog
            {
                Title = "Select files to add to playlist",
                Multiselect = true,
                InitialDirectory = Environment.CurrentDirectory
            };
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                Log("User cancelled");
                ofd.Dispose();
                return;
            }
            Environment.CurrentDirectory = Path.GetDirectoryName(ofd.FileNames[0]);
            EnableDisable(false);
            SongsToAdd.Clear();
            SongToLoad = "";
            if (xbox360.Checked)
            {
                try
                {
                    SongsToAdd = ofd.FileNames.Where(file => VariousFunctions.ReadFileType(file) == XboxFileType.STFS).ToList();
                }
                catch (Exception ex)
                {
                    Log("Error reading file: " + ex.Message);
                    MessageBox.Show((ofd.FileNames.Count() == 1 ? "There was an error reading that file" : "One or more of those files caused a read error") + ":\n'" + ex.Message + "'\nTry again",
                        AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else if (yarg.Checked)
            {
                SongsToAdd = ofd.FileNames.Where(file => Path.GetFileName(file) == "song.ini").ToList();
                var sng = ofd.FileNames.Where(file => Path.GetExtension(file) == ".sng").ToList();
                SongsToAdd.AddRange(sng);
                var yargsong = ofd.FileNames.Where(file => Path.GetExtension(file) == ".yargsong").ToList();
                SongsToAdd.AddRange(yargsong);
            }
            else if (rockSmith.Checked)
            {
                SongsToAdd = ofd.FileNames.Where(file => Path.GetExtension(file) == ".psarc").ToList();
            }
            else
            {
                SongsToAdd = ofd.FileNames.Where(file => Path.GetFileName(file) == "songs.dta").ToList();
                if (pS3.Checked)
                {
                    var pkg = ofd.FileNames.Where(file => Path.GetExtension(file) == ".pkg").ToList();
                    SongsToAdd.AddRange(pkg);
                }
            }
            if (SongsToAdd.Any())
            {
                SongToLoad = SongsToAdd[0];
            }
            if (!SongsToAdd.Any() && string.IsNullOrEmpty(SongToLoad))
            {
                var msg = "No valid files were selected";
                Log(msg);
                MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                EnableDisable(true);
                ofd.Dispose();
                return;
            }
            Log("Selected " + SongsToAdd.Count + " file(s)");
            StartingCount = lstPlaylist.Items.Count;
            isScanning = true;
            UpdateNotifyTray();
            InitiateGIFOverlay();
            if (ofd.FileNames.Count() > 1)
            {
                batchSongLoader.RunWorkerAsync();
            }
            else
            {
                songLoader.RunWorkerAsync();
            }
            ofd.Dispose();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SavePlaylist(true);
        }

        private void renamePlaylist_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(PlaylistName) || string.IsNullOrEmpty(PlaylistPath)) return;
            const string message = "Enter playlist name:";
            var input = Interaction.InputBox(message, AppName, PlaylistName);
            if (string.IsNullOrEmpty(input) || input.Trim() == PlaylistName) return;
            Log("Renamed playlist from " + PlaylistName + " to " + input);
            PlaylistName = input;
            Tools.DeleteFile(PlaylistPath);
            PlaylistPath = Application.StartupPath + "\\playlists\\" + Tools.CleanString(input, true) + ".playlist";
            var unsaved = Text.Contains("*");
            SavePlaylist(false);
            if (unsaved)
            {
                MarkAsModified();
            }
        }

        private void frmMain_Shown(object sender, EventArgs e)
        {
            ClearAll();
            LoadConfig();
            CenterToScreen();
            var dice = Application.StartupPath + "\\bin\\dice.gif";
            picRandom.Image = File.Exists(dice) ? Image.FromFile(dice) : Resources.random;
            UpdateRecentPlaylists("");            
            MediaPlayer.uiMode = "none";
            MediaPlayer.settings.volume = 0;
            MediaPlayer.windowlessVideo = true;
            MediaPlayer.Ctlenabled = false;
            MediaPlayer.enableContextMenu = false;
            MediaPlayer.stretchToFit = true;
            UpdateDisplay(false);
            Application.DoEvents();
            Activate();
            InitBASS();
            if (!string.IsNullOrEmpty(PlaylistPath) && autoloadLastPlaylist.Checked && File.Exists(PlaylistPath))
            {
                PrepareToLoadPlaylist();
            }
            updater.RunWorkerAsync();
        }

        private void PrepareToLoadPlaylist(string playlist = "")
        {
            if (!string.IsNullOrEmpty(playlist))
            {
                PlaylistPath = playlist;
            }
            lblUpdates.Text = "Loading Playlist...";
            lblUpdates.Refresh();
            LoadPlaylist();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F && ModifierKeys.HasFlag(Keys.Control))
            {
                btnClear.PerformClick();
                txtSearch.Focus();
            }
            else if (e.KeyCode == Keys.Enter && ModifierKeys.HasFlag(Keys.Control))
            {
                lstPlaylist_MouseDoubleClick(null,null);
            }
            else if (e.KeyCode == Keys.Space && !txtSearch.Focused && txtSearch.BackColor == Color.Black)
            {
                btnPlayPause.PerformClick();
            }
        }
        
        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter) return;
            var enabled = !string.IsNullOrEmpty(txtSearch.Text.Trim()) && txtSearch.Text != "Type to search playlist...";
            if (!enabled) return;
            btnSearch.ForeColor = EnabledButtonColor;
            btnGoTo.ForeColor = EnabledButtonColor;
            btnClear.ForeColor = EnabledButtonColor;
        }
        
        static public Bitmap CopyChartSection(Bitmap srcBitmap, Rectangle section)
        {
            var bmp = new Bitmap(section.Width, section.Height);
            var g = Graphics.FromImage(bmp);
            g.DrawImage(srcBitmap, 0, 0, section, GraphicsUnit.Pixel);
            g.Dispose();
            return bmp;
        }
        
        public void ClearNoteColors(bool vocals_only = false, bool prokeys_only = false)
        {
            if (MIDITools.MIDI_Chart == null || MIDITools.PhrasesVocals == null) return;
            try
            {
                if (!vocals_only)
                {
                    foreach (var var in MIDITools.MIDI_Chart.ProKeys.ChartedNotes)
                    {
                        var.NoteColor = Color.Empty;
                    }
                }
                if (prokeys_only) return;
                foreach (var var in MIDITools.MIDI_Chart.Vocals.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Harm1.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Harm2.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Harm3.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                if (vocals_only) return;
                foreach (var var in MIDITools.MIDI_Chart.Drums.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Bass.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Guitar.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
                foreach (var var in MIDITools.MIDI_Chart.Keys.ChartedNotes)
                {
                    var.NoteColor = Color.Empty;
                }
            }
            catch (Exception)
            {}
        }
        
        private void openSideWindow_Click(object sender, EventArgs e)
        {
            UpdateDisplay();
            UpdateStats();
            if (!openSideWindow.Checked && WindowState == FormWindowState.Maximized)
            {
                WindowState = FormWindowState.Normal;
            }
        }

        private void VisualsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            displayBackgroundVideo.Visible = yarg.Checked;
            toolStripMenuItem2.Visible = yarg.Checked;
            displayKaraokeMode.Enabled = PlayingSong == null || (MIDITools.PhrasesVocals.Phrases.Any() && MIDITools.LyricsVocals.Lyrics.Any());
            styleToolStripMenuItem.Visible = displayMIDIChartVisuals.Checked;
            toolStripMenuItem8.Visible = displayMIDIChartVisuals.Checked;
            displayAlbumArt.Enabled = PlayingSong == null || File.Exists(CurrentSongArtBlurred);
            displayMIDIChartVisuals.Enabled = !hasNoMIDI;
        }

        private void txtSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char) Keys.Return) return;
            e.Handled = true;
            if (ShowingNotFoundMessage) return;
            btnGoTo.PerformClick();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Type to search playlist..." || Playlist.Count == 0 || btnGoTo.ForeColor != EnabledButtonColor) return;
            Log("btnSearch_Click: " + txtSearch.Text);
            ReloadPlaylist(Playlist);
        }

        private void btnGoTo_Click(object sender, EventArgs e)
        {
            if (txtSearch.Text == "Type to search playlist..." || Playlist.Count == 0 || btnGoTo.ForeColor != EnabledButtonColor) return;
            Log("btnGoTo_Click: " + txtSearch.Text);
            GoToSearchTerm(txtSearch.Text.Trim().ToLowerInvariant(), true);
        }

        private void GoToSearchTerm(string search, bool UserSearch)
        {
            try
            {
                var select = -1;
                var start = lstPlaylist.SelectedIndices[0] + 1;
                //start from current selection and go down to bottom
                for (var i = start; i < lstPlaylist.Items.Count; i++)
                {
                    if (UserSearch && !lstPlaylist.Items[i].SubItems[1].Text.ToLowerInvariant().Contains(search))
                    {
                        continue;
                    }
                    if (!UserSearch && !lstPlaylist.Items[i].SubItems[1].Text.ToLowerInvariant().StartsWith(search, StringComparison.Ordinal))
                    {
                        continue;
                    }
                    select = i;
                    break;
                }
                if (select == -1)
                {
                    //nothing found, let's try from the top to the current selection
                    for (var i = 0; i < start; i++)
                    {
                        if (UserSearch && !lstPlaylist.Items[i].SubItems[1].Text.ToLowerInvariant().Contains(search))
                        {
                            continue;
                        }
                        if (!UserSearch && !lstPlaylist.Items[i].SubItems[1].Text.ToLowerInvariant().StartsWith(search, StringComparison.Ordinal))
                        {
                            continue;
                        }
                        select = i;
                        break;
                    }
                }
                if (select == -1)
                {
                    if (!UserSearch) return;
                    txtSearch.BackColor = Color.DarkRed;
                    txtSearch.Refresh();
                    var msg = "Search term '" + search + "' was not found";
                    Log(msg);
                    ShowingNotFoundMessage = true;
                    MessageBox.Show(msg, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ShowingNotFoundMessage = false;
                    txtSearch.BackColor = Color.Black;
                    return;
                }
                Log("Search term found, index: " + select);
                if (ActiveSong != null)
                {
                    lstPlaylist.Items[ActiveSong.Index].Selected = false;
                }
                lstPlaylist.Items[select].Selected = true;
                lstPlaylist.Items[select].Focused = true;
                lstPlaylist.EnsureVisible(select);
            }
            catch (Exception ex)
            {
                Log("Error finding search term '" + search + "':");
                Log(ex.Message);
            }
        }
        
        private void showPracticeSections_Click(object sender, EventArgs e)
        {
            Log("showPracticeSections_Click");
            UpdateDisplay(false);
        }
        
        private void lstPlaylist_KeyPress(object sender, KeyPressEventArgs e)
        {
            const string valid_keys = "abcdefghijklmnopqrstuvwxyz1234567890";
            var input = e.KeyChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant();
            if (!valid_keys.Contains(input)) return;
            try
            {
                GoToSearchTerm(e.KeyChar.ToString(CultureInfo.InvariantCulture).ToLowerInvariant(), false);
                e.Handled = true;
            }
            catch (Exception ex)
            {
                Log("Error handling lstPlaylist_KeyPress:");
                Log(ex.Message);
            }
        }

        private void lstPlaylist_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up && ModifierKeys.HasFlag(Keys.Alt))
            {
                MoveSelectionUp();
            }
            else if (e.KeyCode == Keys.Down && ModifierKeys.HasFlag(Keys.Alt))
            {
                MoveSelectionDown();
            }
        }

        public static void Shuffle<T>(IList<T> list)
        {
            var rng = new Random();
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = rng.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void randomizePlaylist_Click(object sender, EventArgs e)
        {
            Log("randomizePlaylist_Click");
            SortPlaylist(PlaylistSorting.Shuffle);
        }

        private void startInstaMix_Click(object sender, EventArgs e)
        {
            Log("startInstaMix_Click");
            EnableDisable(false);
            btnClear.PerformClick();
            SongMixer.RunWorkerAsync();
        }

        private void SongMixer_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Log("Song mixer finished");
            EnableDisable(true);
            ReloadPlaylist(Playlist, true, false, false);
            MarkAsModified();
            btnShuffle.Tag = "noshuffle";
            btnShuffle.BackColor = Color.LightGray;
            toolTip1.SetToolTip(btnShuffle, "Enable track shuffling");
            if (PlayingSong == null || ActiveSong != PlayingSong || btnPlayPause.Text == "PLAY")
            {
                lstPlaylist_MouseDoubleClick(null,null);
            }
            else
            {
                lstPlaylist.Items[0].Tag = 1; //played
                UpdateHighlights();
                GetNextSong();
            }
        }

        private void SongMixer_DoWork(object sender, DoWorkEventArgs e)
        {
            Log("Song mixer working");
            var MixSong = ActiveSong;
            const int minSongs = 25;
            //try to get at least 25 songs in the playlist
            //allow 25%, 38% and 50% discrepancy at max
            //don't go beyond 50% discrepancy even if we have less than 25 songs
            CreateSongMix(MixSong, 0.25);
            Log("Created mix with discrepancy factor: 0.25");
            if (Playlist.Count < minSongs)
            {
                CreateSongMix(MixSong, 0.38);
                Log("Created mix with discrepancy factor: 0.38");
            }
            if (Playlist.Count < minSongs)
            {
                CreateSongMix(MixSong, 0.50);
                Log("Created mix with discrepancy factor: 0.50");
            }
            Playlist.Remove(MixSong);
            Shuffle(Playlist);
            var backup = Playlist[0];
            Playlist[0] = MixSong;
            Playlist.Add(backup);
        }

        private void CreateSongMix(Song MixSong, double factor)
        {
            var maxBPM = MixSong.BPM * (1.00 + factor);
            var minBPM = MixSong.BPM * (1.00 - factor);
            var maxLength = MixSong.Length * (1.00 + factor);
            var minLength = MixSong.Length * (1.00 - factor);
            var genre = MixSong.Genre.ToLowerInvariant();
            Playlist = new List<Song>();
            foreach (var song in from song in StaticPlaylist where !(song.BPM < minBPM) && !(song.BPM > maxBPM) where !(song.Length < minLength) && !(song.Length > maxLength) where song.Genre.ToLowerInvariant() == genre select song)
            {
                Playlist.Add(song);
            }
        }

        private void LoadRecent(int playlist)
        {
            Log("Loading recent playlist #" + playlist + ": " + RecentPlaylists[playlist]);
            if (Text.Contains("*"))
            {
                if (MessageBox.Show("You have unsaved changes on the current playlist\nAre you sure you want to do that?",
                        AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            }
            StartNew(false);
            PrepareToLoadPlaylist(RecentPlaylists[playlist]);
        }

        private void recent1_Click(object sender, EventArgs e)
        {
            LoadRecent(0);
        }

        private void recent2_Click(object sender, EventArgs e)
        {
            LoadRecent(1);
        }

        private void recent3_Click(object sender, EventArgs e)
        {
            LoadRecent(2);
        }

        private void recent4_Click(object sender, EventArgs e)
        {
            LoadRecent(3);
        }

        private void recent5_Click(object sender, EventArgs e)
        {
            LoadRecent(4);
        }
        
        private string GetJumpMessage(double time)
        {
            var message = "Jump to: " + Parser.GetSongDuration(time);
            if (MIDITools.PracticeSessions.Any())
            {
                message = message + " " + GetCurrentSection(time);
            }
            return message;
        }
        
        private void DrawSpectrum(Control sender, Graphics graphics)
        {
            Spectrum.ChannelIsMixerSource = false;
            //if (displayAudioSpectrum.Checked && (MediaPlayer.playState == WMPPlayState.wmppsPlaying || (MediaPlayer.playState == WMPPlayState.wmppsPaused && displayBackgroundVideo.Checked) || VideoIsPlaying)) return;
            try
            {
                var width = displayAudioSpectrum.Checked && openSideWindow.Checked ? picVisuals.Width : picPreview.Width;
                switch (SpectrumID)
                {
                    // line spectrum (width = resolution)
                    default:
                        SpectrumID = 0;
                        Spectrum.CreateSpectrumLine(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartRed, Color.Black, 2, 2, false, false, false);
                        break;
                    // normal spectrum (width = resolution)
                    case 1:
                        Spectrum.CreateSpectrum(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartRed, Color.Black, false, false, false);
                        break;
                    // line spectrum (full resolution)
                    case 2:
                        Spectrum.CreateSpectrumLine(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartBlue, ChartOrange, Color.Black, width / 15, 4, false, true, false);
                        break;
                    // ellipse spectrum (width = resolution)
                    case 3:
                        Spectrum.CreateSpectrumEllipse(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartRed, Color.Black, 1, 2, false, false, false);
                        break;
                    // peak spectrum (width = resolution)
                    case 4:
                        Spectrum.CreateSpectrumLinePeak(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartYellow, ChartOrange, Color.Black, 2, 1, 2, 10, false, false, false);
                        break;
                    // peak spectrum (full resolution)
                    case 5:
                        Spectrum.CreateSpectrumLinePeak(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartBlue, ChartOrange, Color.Black, width / 15, 5, 3, 5, false, true, false);
                        break;
                    // WaveForm
                    case 6:
                        Spectrum.CreateWaveForm(BassMixer, graphics, new Rectangle(sender.Location, sender.Size), ChartGreen, ChartRed, ChartYellow, Color.Black, 1, true, false, false);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log("Error drawing audio spectrum:" + ex.Message);
            }
        }

        private void displayAudioSpectrum_Click(object sender, EventArgs e)
        {
            updateDisplayType(sender);
        }

        private void displayAlbumArt_Click(object sender, EventArgs e)
        {
            updateDisplayType(sender);
            toolTip1.SetToolTip(picPreview, "Click to change spectrum style");
        }

        private void displayMIDIChartVisuals_Click(object sender, EventArgs e)
        {
            updateDisplayType(sender);
        }

        private void ChangeDisplay()
        {
            ClearVisuals();
            Log(File.Exists(CurrentSongArt) ? "Album art found, loading" : "No album art found");
            if (!displayAlbumArt.Checked && File.Exists(CurrentSongArt))
            {
                picPreview.Image = Tools.NemoLoadImage(CurrentSongArt);
                picPreview.Cursor = Cursors.Hand;
                toolTip1.SetToolTip(picPreview, "Click to view album art");
            }
            else
            {
                picPreview.Image = Resources.noart;
                picPreview.Cursor = Cursors.Default;
                toolTip1.SetToolTip(picPreview, "No album art available");
            }
        }

        private void audioTracks_Click(object sender, EventArgs e)
        {
            isChoosingStems = true;
            var selector = new AudioSelector(this);
            selector.Show();
            Log("Displayed audio selector");
        }

        private void showMIDIVisuals_Click(object sender, EventArgs e)
        {
            var selector = new MIDISelector(this);
            selector.Show();
            Log("Displayed MIDI display setting selector");
        }

        private void DrawLyricsScrolling(List<Lyric> lyrics, Control label, Color color, Graphics graphics)
        {
            if (!openSideWindow.Checked || PlayingSong == null || btnPlayPause.Tag.ToString() == "play" || !doScrollingLyrics) return;
            var time = GetCorrectedTime();
            label.Text = "";            
            using (var pen = new SolidBrush(MediaPlayer.Visible ? picVisuals.BackColor : LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            for (var i = 0; i < lyrics.Count(); i++)
            {
                if (lyrics[i].LyricStart < time) continue;
                if (lyrics[i].LyricStart > time + PlaybackWindow) return;
                var left = (int)(((lyrics[i].LyricStart - time) / PlaybackWindow) * label.Width);
                TextRenderer.DrawText(graphics, ProcessLine(lyrics[i].LyricText, true), label.Font, new Point(left, 0), color);
            }
        }

        private void DrawLyricsKaraoke(IEnumerable<LyricPhrase> phrases, IEnumerable<Lyric> lyrics, Control label, Color color, Graphics graphics)
        {
            var time = GetCorrectedTime();
            label.Text = "";            
            using (var pen = new SolidBrush(MediaPlayer.Visible ? picVisuals.BackColor : LabelBackgroundColor))
            {
                graphics.FillRectangle(pen, label.ClientRectangle);
            }
            LyricPhrase line = null;
            foreach (var lyric in phrases.TakeWhile(lyric => lyric.PhraseStart <= time).Where(lyric => lyric.PhraseEnd >= time))
            {
                line = lyric;
            }
            if (line == null || string.IsNullOrEmpty(line.PhraseText)) return;
            var measure = TextRenderer.MeasureText(ProcessLine(line.PhraseText, doWholeWordsLyrics), label.Font);
            var left = (label.Width - measure.Width) / 2;
            TextRenderer.DrawText(graphics, ProcessLine(line.PhraseText, doWholeWordsLyrics),label.Font,new Point(left, 0), Color.White);
            var line2 = lyrics.Where(lyr => !(lyr.LyricStart < line.PhraseStart)).TakeWhile(lyr => !(lyr.LyricStart > time)).Aggregate("", (current, lyr) => current + " " + lyr.LyricText);
            if (string.IsNullOrEmpty(line2)) return;
            TextRenderer.DrawText(graphics, ProcessLine(line2, doWholeWordsLyrics), label.Font, new Point(left, 0), color);
        }

        private void lblHarm1_Paint(object sender, PaintEventArgs e)
        {
            if (!openSideWindow.Checked || PlayingSong == null || btnPlayPause.Tag.ToString() == "play" || (!doScrollingLyrics && !doKaraokeLyrics && !doStaticLyrics)) return;
            var harmony = (doScrollingLyrics && MIDITools.LyricsHarm2.Lyrics.Any()) || ((doKaraokeLyrics || doStaticLyrics) && MIDITools.PhrasesHarm2.Phrases.Any());
            var color = (harmony && doHarmonyLyrics) || doMIDIHarm1onVocals ? Harm1Color : Color.White;
            var phrases = harmony && doHarmonyLyrics ? MIDITools.PhrasesHarm1.Phrases : MIDITools.PhrasesVocals.Phrases;
            var lyrics = harmony && doHarmonyLyrics ? MIDITools.LyricsHarm1.Lyrics : MIDITools.LyricsVocals.Lyrics;
            if (doScrollingLyrics)
            {
                DrawLyricsScrolling(lyrics, lblHarm1, color, e.Graphics);
            }
            else if (doKaraokeLyrics)
            {
                DrawLyricsKaraoke(phrases, lyrics, lblHarm1, Harm1Color, e.Graphics);
            }
            else if (doStaticLyrics)
            {
                DrawLyricsStatic(phrases, lblHarm1, color, e.Graphics);
            }
        }

        private void lblHarm2_Paint(object sender, PaintEventArgs e)
        {
            if (!openSideWindow.Checked || PlayingSong == null || btnPlayPause.Tag.ToString() == "play" || (!doScrollingLyrics && !doKaraokeLyrics && !doStaticLyrics)) return;
            if (!MIDITools.LyricsHarm2.Lyrics.Any()) return;
            if (doScrollingLyrics)
            {
                DrawLyricsScrolling(MIDITools.LyricsHarm2.Lyrics, lblHarm2, Harm2Color, e.Graphics);
            }
            else if (doKaraokeLyrics && MIDITools.PhrasesHarm2.Phrases.Any())
            {
                DrawLyricsKaraoke(MIDITools.PhrasesHarm2.Phrases, MIDITools.LyricsHarm2.Lyrics, lblHarm2, Harm2Color, e.Graphics);
            }
            else if (doStaticLyrics && MIDITools.PhrasesHarm2.Phrases.Any())
            {
                DrawLyricsStatic(MIDITools.PhrasesHarm2.Phrases, lblHarm2, Harm2Color, e.Graphics);
            }
        }

        private void lblHarm3_Paint(object sender, PaintEventArgs e)
        {
            if (!openSideWindow.Checked || PlayingSong == null || btnPlayPause.Tag.ToString() == "play" || (!doScrollingLyrics && !doKaraokeLyrics && !doStaticLyrics)) return;
            if (!MIDITools.LyricsHarm3.Lyrics.Any()) return; 
            if (doScrollingLyrics)
            {
                DrawLyricsScrolling(MIDITools.LyricsHarm3.Lyrics, lblHarm3, Harm3Color, e.Graphics);
            }
            else if (doKaraokeLyrics && MIDITools.PhrasesHarm3.Phrases.Any())
            {
                DrawLyricsKaraoke(MIDITools.PhrasesHarm3.Phrases, MIDITools.LyricsHarm3.Lyrics, lblHarm3, Harm3Color, e.Graphics);
            }
            else if (doStaticLyrics && MIDITools.PhrasesHarm3.Phrases.Any())
            {
                DrawLyricsStatic(MIDITools.PhrasesHarm3.Phrases, lblHarm3, Harm3Color, e.Graphics);
            }
        }
        
        private void showLyrics_Click(object sender, EventArgs e)
        {
            var selector = new LyricSelector(this);
            selector.Show();
            Log("Displayed lyrics setting selector");
        }

        private static void Log(string line)
        {
            if (string.IsNullOrEmpty(line)) return;
            var logfile = Application.StartupPath + "\\log.txt";
            if (!File.Exists(logfile))
            {
                var sw = new StreamWriter(logfile, false);
                var vers = Assembly.GetExecutingAssembly().GetName().Version;
                var version = " v" + String.Format("{0}.{1}.{2}", vers.Major, vers.Minor, vers.Build);
                sw.WriteLine("//Log file for " + AppName);
                sw.WriteLine("//Created by " + AppName + version);
                sw.Dispose();
            }
            try
            {
                var writer = new StreamWriter(logfile, true);
                writer.WriteLine(GetCurrentTime() + "\t" + line);
                writer.Dispose();
            }
            catch (Exception)
            {}
        }

        private static string GetCurrentTime()
        {
            return DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();
        }
        
        private void panelVisuals_DoubleClick(object sender, EventArgs e)
        {
            WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
        }

        private void takeScreenshot_Click(object sender, EventArgs e)
        {
            if (!openSideWindow.Checked)
            {
                MessageBox.Show("No visuals to capture!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            if (Uploader.IsBusy)
            {
                MessageBox.Show("Slow down, the other image is still uploading!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
            var folder = Application.StartupPath + "\\Screenshots\\";
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            string xOut;
            if (PlayingSong == null)
            {
                xOut = folder + AppName + "_" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour +
                       DateTime.Now.Minute + DateTime.Now.Second + DateTime.Now.Millisecond + ".png";
            }
            else
            {
                var name = Tools.CleanString(PlayingSong.Name, true, true).Replace(" ", "");
                xOut = folder + AppName + "_" + name + "_" + PlaybackSeconds + ".png";
            }
            var location = PointToScreen(picVisuals.Location);
            try
            {
                using (var bitmap = new Bitmap(picVisuals.Width, picVisuals.Height))
                {
                    var g = Graphics.FromImage(bitmap);
                    g.CopyFromScreen(location, new Point(0, 0), picVisuals.Size, CopyPixelOperation.SourceCopy);
                    var myEncoder = System.Drawing.Imaging.Encoder.Quality;
                    var myEncoderParameters = new EncoderParameters(1);
                    myEncoderParameters.Param[0] = new EncoderParameter(myEncoder, 100L);
                    var myImageCodecInfo = Tools.GetEncoderInfo("image/png");
                    bitmap.Save(xOut, myImageCodecInfo, myEncoderParameters);
                }
            }
            catch (Exception ex)
            {
                Log("Error capture visuals:");
                Log(ex.Message);
                MessageBox.Show("Error capture screenshot of visuals:\n" + ex.Message + "\nTry again", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            if (!uploadScreenshots.Checked) return;
            ImgToUpload = xOut;
            Uploader.RunWorkerAsync();
        }

        private void Uploader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (string.IsNullOrEmpty(ImgURL))
            {
                MessageBox.Show("Failed to upload to Imgur, please try again", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                Clipboard.SetText(ImgURL);
                if (MessageBox.Show("Uploaded to Imgur successfully\nClick OK to open link in browser", AppName, MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Asterisk) != DialogResult.OK) return;
                Process.Start(ImgURL);
            }
        }

        private void Uploader_DoWork(object sender, DoWorkEventArgs e)
        {
            ImgURL = Tools.UploadToImgur(ImgToUpload);
        }

        private void viewSongDetails_Click(object sender, EventArgs e)
        {
            if (ActiveSong == null)
            {
                MessageBox.Show("No song is selected, no details to show", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                var details = "Selected song details:\n\nLocation:\n" + ActiveSong.Location + FormatInfoLine("Index", ActiveSong.Index, 26) + 
                    FormatInfoLine("Artist", ActiveSong.Artist, 26) + FormatInfoLine("Title", ActiveSong.Name, 28) + FormatInfoLine("Album", ActiveSong.Album, 24) + 
                    FormatInfoLine("Track Number", ActiveSong.Track, 10) + FormatInfoLine("Year", ActiveSong.Year, 28) + FormatInfoLine("Genre", ActiveSong.Genre, 25) + 
                    FormatInfoLine("Length", FormatTime(ActiveSong.Length / 1000L), 23) + 
                    FormatInfoLine("Charter", string.IsNullOrEmpty(ActiveSong.Charter.Trim()) ? "Unknown" : ActiveSong.Charter, 22) + 
                    FormatInfoLine("Internal Name", ActiveSong.InternalName, 11) + 
                    FormatInfoLine("Rhythm on Keys?", ActiveSong.isRhythmOnKeys ? "Yes" : "No", 5) + FormatInfoLine("Rhythm on Bass?", ActiveSong.isRhythmOnBass ? "Yes" : "No", 5) + 
                    FormatInfoLine("Audio Delay", ActiveSong.PSDelay == 0 ? "None" : ActiveSong.PSDelay.ToString(CultureInfo.InvariantCulture) + " ms", 14) + 
                    FormatInfoLine("Channels - Drums", ActiveSong.ChannelsDrums, 4) + FormatInfoLine("Channels - Bass", ActiveSong.ChannelsBass, 8) + 
                    FormatInfoLine("Channels - Guitar", ActiveSong.ChannelsGuitar, 5) + FormatInfoLine("Channels - Keys", ActiveSong.ChannelsKeys, 8) + 
                    FormatInfoLine("Channels - Vocals", ActiveSong.ChannelsVocals, 5) + FormatInfoLine("Channels - Backing", ActiveSong.ChannelsBacking, 2) + 
                    FormatInfoLine("Channels - Crowd", ActiveSong.ChannelsCrowd, 4);
                if (ActiveSong == PlayingSong)
                {
                    var instruments = "";
                    var solos = "";
                    var rangeVocals = "";
                    var rangeHarmonies = "";
                    var rangeProKeys = "";
                    if (MIDITools.MIDI_Chart.Drums.ChartedNotes.Count > 0)
                    {
                        instruments += "D ";
                    }
                    if (MIDITools.MIDI_Chart.Bass.ChartedNotes.Count > 0)
                    {
                        instruments += "B ";
                    }
                    if (MIDITools.MIDI_Chart.Guitar.ChartedNotes.Count > 0)
                    {
                        instruments += "G ";
                    }
                    if (MIDITools.MIDI_Chart.Keys.ChartedNotes.Count > 0)
                    {
                        instruments += "K ";
                    }
                    if (MIDITools.MIDI_Chart.ProKeys.ChartedNotes.Count > 0)
                    {
                        instruments += "PK ";
                        rangeProKeys = FormatInfoLine("Range - Pro Keys", MIDITools.MIDI_Chart.ProKeys.NoteRange.Count, 6);
                    }
                    if (MIDITools.MIDI_Chart.Vocals.ChartedNotes.Count > 0)
                    {
                        instruments += "V ";
                        rangeVocals = FormatInfoLine("Range - Vocals", MIDITools.MIDI_Chart.Vocals.NoteRange.Count, 10);
                    }
                    if (MIDITools.MIDI_Chart.Harm1.ChartedNotes.Count > 0)
                    {
                        instruments += "H1 ";
                        rangeHarmonies = FormatInfoLine("Range - Harmonies", MIDITools.MIDI_Chart.Harm1.NoteRange.Count, 2);
                    }
                    if (MIDITools.MIDI_Chart.Harm2.ChartedNotes.Count > 0)
                    {
                        instruments += "H2 ";
                    }
                    if (MIDITools.MIDI_Chart.Harm3.ChartedNotes.Count > 0)
                    {
                        instruments += "H3 ";
                    }
                    if (MIDITools.MIDI_Chart.Drums.Solos.Count > 0)
                    {
                        solos += "D ";
                    }
                    if (MIDITools.MIDI_Chart.Bass.Solos.Count > 0)
                    {
                        solos += "B ";
                    }
                    if (MIDITools.MIDI_Chart.Guitar.Solos.Count > 0)
                    {
                        solos += "G ";
                    }
                    if (MIDITools.MIDI_Chart.Keys.Solos.Count > 0)
                    {
                        solos += "K ";
                    }
                    if (MIDITools.MIDI_Chart.ProKeys.Solos.Count > 0)
                    {
                        solos += "PK ";
                    }
                    details = details + FormatInfoLine("Average BPM", MIDITools.MIDI_Chart.AverageBPM.ToString(CultureInfo.InvariantCulture), 12) + 
                        FormatInfoLine("Uses disco flip?", MIDITools.MIDI_Chart.DiscoFlips.Any() ? "Yes" : "No", 9) + 
                        FormatInfoLine("Has Pro Keys?", PlayingSong.hasProKeys && MIDITools.MIDI_Chart.ProKeys.ChartedNotes.Count > 0 ? "Yes" : "No", 11) + 
                        FormatInfoLine("Instrument Charts", instruments.Trim(), 4) + FormatInfoLine("Instrument Solos", string.IsNullOrEmpty(solos.Trim()) ? "None" : solos.Trim(), 6) + 
                        rangeVocals + rangeHarmonies + rangeProKeys + FormatInfoLine("Practice Sessions", MIDITools.PracticeSessions.Count, 6);
                }
                MessageBox.Show(details + "\nAttenuation:\n" + ActiveSong.AttenuationValues.Trim() + "\nPanning:\n" + ActiveSong.PanningValues.Trim(), AppName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }

        private static string FormatTime(double time)
        {
            if (time >= 3600.0)
            {
                var num1 = (int)(time / 3600.0);
                var num2 = (int)(time - num1 * 3600);
                var num3 = (int)(time - num2 * 60);
                return (string)(object)num1 + (object)":" + (string)(num2 < 10 ? (object)"0" : (object)"") + (string)(object)num2 + ":" + (string)(num3 < 10 ? (object)"0" : (object)"") + (string)(object)num3;
            }
            if (time < 60.0)
            {
                return "0:" + (time < 10.0 ? "0" : "") + (int)time;
            }
            var num4 = (int)(time / 60.0);
            var num5 = (int)(time - num4 * 60);
            return string.Concat(new object[] {num4, ":", num5 < 10 ? "0" : "", num5});
        }

        private static string FormatInfoLine(string field, int data, int spacer)
        {
            return FormatInfoLine(field, data.ToString(CultureInfo.InvariantCulture), spacer);
        }

        private static string FormatInfoLine(string field, string data, int spacers)
        {
            var str = "";
            for (var index = 0; index < spacers; ++index)
            {
                str += " ";
            }
            return "\n" + field + ": " + str + " " + data;
        }

        private void picRandom_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || lstPlaylist.Items.Count <= 1 || songExtractor.IsBusy || songPreparer.IsBusy) return;
            DoShuffleSongs();
        }
        
        private void DoShuffleSongs()
        {
            var num1 = ShuffleSongs(true);
            if (num1 < 0 || num1 > lstPlaylist.Items.Count - 1)
            {
                MessageBox.Show("There was an error selecting a song at random, try again", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            else
            {
                NextSongIndex = num1;
                foreach (ListViewItem listViewItem in lstPlaylist.SelectedItems)
                {
                    listViewItem.Selected = false;
                }
                if (NextSongIndex > lstPlaylist.Items.Count - 1)
                {
                    NextSongIndex = 0;
                    DeleteUsedFiles(false);
                }
                lstPlaylist.Items[NextSongIndex].Selected = true;
                lstPlaylist.Items[NextSongIndex].Focused = true;
                lstPlaylist.EnsureVisible(NextSongIndex);
                lstPlaylist_MouseDoubleClick(null, null);
            }
        }

        private void updater_DoWork(object sender, DoWorkEventArgs e)
        {
            var path = Application.StartupPath + "\\bin\\update.txt";
            Tools.DeleteFile(path);
            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;// (SecurityProtocolType)3072; //TLS 1.2 for .NET 4.0
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile("https://nemosnautilus.com/cplayer/update.txt", path);
                }
                catch (Exception)
                { }
            }
        }

        private static string GetAppVersion()
        {
            var vers = Assembly.GetExecutingAssembly().GetName().Version;
            return "v" + String.Format("{0}.{1}.{2}", vers.Major, vers.Minor, vers.Build);
        }

        private void updater_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            var path = Application.StartupPath + "\\bin\\update.txt";
            if (!File.Exists(path))
            {
                if (showUpdateMessage)
                {
                    MessageBox.Show("Unable to check for updates", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                return;
            }
            var thisVersion = GetAppVersion();
            var newVersion = "v";
            string newName;
            string releaseDate;
            string link;
            var changeLog = new List<string>();
            var sr = new StreamReader(path);
            try
            {
                var line = sr.ReadLine();
                if (line.ToLowerInvariant().Contains("html"))
                {
                    sr.Dispose();
                    if (showUpdateMessage)
                    {
                        MessageBox.Show("Unable to check for updates", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    }
                    return;
                }
                newName = Tools.GetConfigString(line);
                newVersion += Tools.GetConfigString(sr.ReadLine());
                releaseDate = Tools.GetConfigString(sr.ReadLine());
                link = Tools.GetConfigString(sr.ReadLine());
                sr.ReadLine();//ignore Change Log header
                while (sr.Peek() >= 0)
                {
                    changeLog.Add(sr.ReadLine());
                }
            }
            catch (Exception ex)
            {
                if (showUpdateMessage)
                {
                    MessageBox.Show("Error parsing update file:\n" + ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                sr.Dispose();
                return;
            }
            sr.Dispose();
            Tools.DeleteFile(path);
            if (thisVersion.Equals(newVersion))
            {
                if (showUpdateMessage)
                {
                    MessageBox.Show("You have the latest version", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            var newInt = Convert.ToInt16(newVersion.Replace("v", "").Replace(".", "").Trim());
            var thisInt = Convert.ToInt16(thisVersion.Replace("v", "").Replace(".", "").Trim());
            if (newInt <= thisInt)
            {
                if (showUpdateMessage)
                {
                    MessageBox.Show("You have a newer version (" + thisVersion + ") than what's on the server (" + newVersion + ")\nNo update needed!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return;
            }
            var updaterForm = new Updater();
            updaterForm.SetInfo(AppName, thisVersion, newName, newVersion, releaseDate, link, changeLog);
            updaterForm.ShowDialog();
        }

        private void checkForUpdates_Click(object sender, EventArgs e)
        {
            showUpdateMessage = true;
            updater.RunWorkerAsync();
        }

        private void viewChangeLog_Click(object sender, EventArgs e)
        {
            const string changelog = "cplayer_changelog.txt";
            if (!File.Exists(Application.StartupPath + "\\" + changelog))
            {
                MessageBox.Show("Changelog file is missing!", AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Process.Start(Application.StartupPath + "\\" + changelog);
        }

        private void sortPlaylistByModifiedDate_Click(object sender, EventArgs e)
        {
            SortPlaylist(PlaylistSorting.ByModifiedDate);
        }

        private void displayKaraokeMode_Click(object sender, EventArgs e)
        {
            updateDisplayType(sender);
        }

        private void picVisuals_Paint(object sender, PaintEventArgs e)
        {
            if (!PlaybackTimer.Enabled || !openSideWindow.Checked || PlayingSong == null || WindowState == FormWindowState.Minimized 
                || (MediaPlayer.playState == WMPPlayState.wmppsPaused && displayBackgroundVideo.Checked) || VideoIsPlaying) return;
            UpdateTextQuality(e.Graphics);
            if (displayAudioSpectrum.Checked)
            {
                DrawSpectrum(picVisuals, e.Graphics);
                return;
            }
            if (displayMIDIChartVisuals.Checked && chartFull.Checked && DrewFullChart)
            {
                var percent = (GetCorrectedTime()/((double) PlayingSong.Length/1000));
                var width = ((int) (picVisuals.Width*percent)) + 1;
                var chart = CopyChartSection(ChartBitmap, new Rectangle(new Point(0, 0), new Size(width, picVisuals.Height)));
                e.Graphics.DrawImage(chart, 0, 0, width, picVisuals.Height);
                chart.Dispose();
                return;
            }
            if (displayKaraokeMode.Checked && MIDITools.PhrasesVocals.Phrases.Any() && MIDITools.LyricsVocals.Lyrics.Any())
            {                
                KaraokeOverlay.Visible = false;
                DoKaraokeMode(e.Graphics, MIDITools.PhrasesVocals.Phrases, MIDITools.LyricsVocals.Lyrics);        
                return;
            }
            if (!displayMIDIChartVisuals.Checked || (!chartSnippet.Checked && DrewFullChart)) return;
            DrawMIDIFile(e.Graphics);
            DrewFullChart = true;
        }

        private void picPreview_Paint(object sender, PaintEventArgs e)
        {
            if (displayAlbumArt.Checked || (!File.Exists(CurrentSongArt) && !displayAudioSpectrum.Checked))
            {
                DrawSpectrum(picPreview, e.Graphics);
            }
        }

        private void GetIntroOutroSilencePS()
        {
            IntroSilenceNext = 0.0;
            OutroSilenceNext = 0.0;
            if (!skipIntroOutroSilence.Checked || NextSong == null) return;
            var OGGs = Directory.GetFiles(Path.GetDirectoryName(NextSong.Location), "*.ogg", SearchOption.TopDirectoryOnly);
            if (!OGGs.Any()) return;
            List<int> NextSongStreams;
            int mixer;
            if (!PrepMixerPS(OGGs, out mixer, out NextSongStreams)) goto ReleaseTempStreams;
            foreach (var stream in NextSongStreams.Where(stream => stream != 0))
            {
                double newIntroSilence;
                double newOutroSilence;
                ProcessStreamForSilence(stream, out newIntroSilence, out newOutroSilence);
                if (IntroSilenceNext == 0.0 || newIntroSilence < IntroSilenceNext) //we only want earliest instance of sound in all streams
                {
                    IntroSilenceNext = newIntroSilence;
                }
                if (newOutroSilence > OutroSilenceNext) //we only want latest instance of silence in all streams
                {
                    OutroSilenceNext = newOutroSilence;
                }
            }
            ReleaseTempStreams:
            foreach (var stream in NextSongStreams)
            {
                Bass.BASS_StreamFree(stream);
            }
            Bass.BASS_StreamFree(mixer);
        }
                
        private void GetIntroOutroSilence()
        {
            IntroSilenceNext = 0.0;
            OutroSilenceNext = 0.0;
            if (!skipIntroOutroSilence.Checked || yarg.Checked || powerGig.Checked || bandFuse.Checked || fortNite.Checked || guitarHero.Checked) return;
            if (nautilus.NextSongOggData == null || nautilus.NextSongOggData.Length == 0) return;
            var stream = Bass.BASS_StreamCreateFile(nautilus.GetOggStreamIntPtr(true), 0L, nautilus.NextSongOggData.Length, BASSFlag.BASS_STREAM_DECODE | BASSFlag.BASS_SAMPLE_FLOAT);
            if (stream == 0)
            {
                nautilus.ReleaseStreamHandle(true);
                return;
            }
            double newIntroSilence;
            double newOutroSilence;
            ProcessStreamForSilence(stream, out newIntroSilence, out newOutroSilence);
            nautilus.ReleaseStreamHandle(true);
            if (IntroSilenceNext == 0.0 || newIntroSilence < IntroSilenceNext)
            {
                IntroSilenceNext = newIntroSilence;
            }
            if (newOutroSilence > OutroSilenceNext)
            {
                OutroSilenceNext = newOutroSilence;
            }
        }

        private void ProcessStreamForSilence(int stream, out double intro, out double outro)
        {
            var buffer = new float[50000];
            long count = 0;
            intro = 0.0;
            outro = 0.0;
            var length = Bass.BASS_ChannelGetLength(stream);
            try
            {
                //detect start silence
                int b;
                do
                {
                    //decode some data
                    b = BassMix.BASS_Mixer_ChannelGetData(stream, buffer, 40000);
                    if (b <= 0) break;
                    //bytes -> samples
                    b /= 4;
                    //count silent samples
                    int a;
                    for (a = 0; a < b && Math.Abs(buffer[a]) <= SilenceThreshold; a++) { }
                    //add number of silent bytes
                    count += a * 4;
                    //if sound has begun...
                    if (a >= b) continue;
                    //move back to a quieter sample (to avoid "click")
                    for (; a > SilenceThreshold / 4 && Math.Abs(buffer[a]) > SilenceThreshold / 4; a--, count -= 4) { }
                    break;
                } while (b > 0);
                intro = Bass.BASS_ChannelBytes2Seconds(stream, count);
                
                //detect end silence
                var pos = length;
                while (pos > count)
                {
                    //step back a bit
                    pos = pos < 200000 ? 0 : pos - 200000;
                    Bass.BASS_ChannelSetPosition(stream, pos);
                    //decode some data
                    var d = BassMix.BASS_Mixer_ChannelGetData(stream, buffer, 200000);
                    if (d <= 0) break;
                    //bytes -> samples
                    d /= 4;
                    //count silent samples
                    int c;
                    for (c = d; c > 0 && Math.Abs(buffer[c - 1]) <= SilenceThreshold / 2; c--) { }
                    //if sound has begun...
                    if (c <= 0) continue;
                    //silence begins here
                    count = pos + c * 4;
                    break;
                }
                outro = Bass.BASS_ChannelBytes2Seconds(stream, count);
            }
            catch (Exception ex)
            {
                Log("Error calculating silence: " + ex.Message);
            }
        }
        
        private void updateDisplayType(object sender)
        {
            Log(((ToolStripMenuItem) sender).Name + "_Click");
            displayAlbumArt.Checked = false;
            displayAudioSpectrum.Checked = false;
            displayMIDIChartVisuals.Checked = false;
            displayKaraokeMode.Checked = false;
            ((ToolStripMenuItem) sender).Checked = true;
            ChangeDisplay();
            UpdateDisplay(false);
        }

        private void MediaPlayer_ClickEvent(object sender, _WMPOCXEvents_ClickEvent e)
        {
            if (e.nButton == 2)
            {
                VisualsContextMenu.Show(MousePosition);
            }
        }

        private void displayBackgroundVideo_Click(object sender, EventArgs e)
        {
            ChangeDisplay();
            UpdateDisplay(false);
            if (displayBackgroundVideo.Checked && !string.IsNullOrEmpty(MediaPlayer.URL) && File.Exists(MediaPlayer.URL))
            {
                StartVideoPlayback();
            }
            else if (!displayBackgroundVideo.Checked)
            {
                StopVideoPlayback(false);
            }
            playBGVideos.Checked = displayBackgroundVideo.Checked;
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            var enabled = !string.IsNullOrEmpty(txtSearch.Text.Trim()) && txtSearch.Text != "Type to search playlist...";
            txtSearch.ForeColor = enabled ? EnabledButtonColor : DisabledButtonColor;
            if (enabled) return;
            btnSearch.ForeColor = DisabledButtonColor;
            btnGoTo.ForeColor = DisabledButtonColor;
            btnClear.ForeColor = DisabledButtonColor;
        }

        private void rebuildPlaylistMetadata_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("This might take a while...are you sure you want to do this now?",
                AppName, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            
            DoClickStop();
            btnClear.PerformClick();
            var rebuilder = new Rebuilder(this, StaticPlaylist);
            rebuilder.ShowDialog();
            if (rebuilder.UserCanceled)
            {
                MessageBox.Show("Rebuilding was canceled, no changes to apply", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                rebuilder.Dispose();
                return;
            }
            if (rebuilder.RebuiltPlaylist.Count == 0)
            {
                MessageBox.Show("Rebuilt playlist contains 0 items, nothing to do", AppName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                rebuilder.Dispose();
                return;

            }
            ClearAll();
            StaticPlaylist = rebuilder.RebuiltPlaylist;
            Playlist = StaticPlaylist;
            rebuilder.Dispose();
            MessageBox.Show("Rebuilding completed successfully...reloading playlist now...", AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ReloadPlaylist(Playlist);
            UpdateHighlights();
            MarkAsModified();
        }

        private void playBGVideos_Click(object sender, EventArgs e)
        {
            displayBackgroundVideo.Checked = playBGVideos.Checked;
            displayBackgroundVideo_Click(sender, e);
        }

        private void frmMain_Move(object sender, EventArgs e)
        {
            UpdateOverlayPosition();
        }

        private void gifTmr_Tick(object sender, EventArgs e)
        {
            if (GIFOverlay == null) return;
            var visible = MonitorApplicationFocus();
            if (visible)
            {
                GIFOverlay.Start();
            }
            else
            {
                GIFOverlay.Stop();
            }
            if (!GIFOverlay.Visible) return;
            UpdateOverlayPosition();
        }
    }

    public class ActiveWord
    {
        public string Text { get; set; }
        public double WordStart { get; set; }
        public double WordEnd { get; set; }

        public ActiveWord(string text, double wordStart, double wordEnd)
        {
            Text = text;
            WordStart = wordStart;
            WordEnd = wordEnd;
        }
    }      

    public class Song
    {
        public string Name { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public long Length { get; set; }
        public string Location { get; set; }
        public string InternalName { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public int Track { get; set; }
        public int Index { get; set; }
        public string PanningValues { get; set; }
        public string AttenuationValues { get; set; }
        public string Charter { get; set; }
        public int ChannelsDrums { get; set; }
        public int ChannelsBass { get; set; }
        public int ChannelsGuitar { get; set; }
        public int ChannelsKeys { get; set; }
        public int ChannelsVocals { get; set; }
        public int ChannelsCrowd { get; set; }
        public int ChannelsBacking { get; set; }
        public bool AddToPlaylist { get; set; }
        public int DTAIndex { get; set; }
        public double BPM { get; set; }
        public bool isRhythmOnBass { get; set; }
        public bool isRhythmOnKeys { get; set; }
        public bool hasProKeys { get; set; }
        public int PSDelay { get; set; }
        public int ChannelsBassStart { get; set; }
        public int ChannelsDrumsStart { get; set; }
        public int ChannelsGuitarStart { get; set; }
        public int ChannelsKeysStart { get; set; }
        public int ChannelsVocalsStart { get; set; }
        public int ChannelsCrowdStart { get; set; }
        public int ChannelsTotal { get; set; }
        public string yargPath { get; set; }
    }
}
