using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using cPlayer.x360;

namespace cPlayer
{
    public partial class Rebuilder : Form
    {
        private readonly List<Song> CurrentPlaylist;
        public List<Song> RebuiltPlaylist; 
        private readonly frmMain xParent;
        public bool UserCanceled;
        private readonly DTAParser Parser;

        public Rebuilder(frmMain parent, List<Song> playlist)
        {
            InitializeComponent();
            xParent = parent;
            ControlBox = false;
            Parser = new DTAParser();
            CurrentPlaylist = playlist;
            RebuiltPlaylist = new List<Song>();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            UserCanceled = true;
        }

        private void Rebuilder_Shown(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            Close();
        }

        private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            var count = 0;
            foreach (var playlistSong in CurrentPlaylist)
            {
                if (UserCanceled) return;
                count++;
                lblCurrent.Invoke(new MethodInvoker(() => lblCurrent.Text = "Processing song " + count + " of " + CurrentPlaylist.Count));
                lblCurrent.Invoke(new MethodInvoker(() => lblCurrent.Refresh()));
                if (!File.Exists(playlistSong.Location)) continue;
                if ((Path.GetExtension(playlistSong.Location).ToLowerInvariant()).Equals(".dta"))
                {
                    Parser.ReadDTA(File.ReadAllBytes(playlistSong.Location));
                }
                else if ((Path.GetExtension(playlistSong.Location).ToLowerInvariant()).Equals(".ini"))
                {
                    Parser.ReadINIFile(playlistSong.Location);
                }
                else if (VariousFunctions.ReadFileType(playlistSong.Location) == XboxFileType.STFS)
                {
                    var xPackage = new STFSPackage(playlistSong.Location);
                    if (!xPackage.ParseSuccess) continue;
                    Parser.ReadDTA(xPackage);
                    xPackage.CloseIO();
                }
                else
                {
                    continue;
                }
                if (UserCanceled || !Parser.Songs.Any()) return;
                var index = 0;
                if (Parser.Songs.Count > 1)
                {
                    for (index = 0; index < Parser.Songs.Count; index++)
                    {
                        var song = Parser.Songs[index];
                        if (String.Equals(song.Artist, playlistSong.Artist, StringComparison.InvariantCultureIgnoreCase) &&
                            String.Equals(song.Name, playlistSong.Name, StringComparison.InvariantCultureIgnoreCase)) break;
                        if (song.InternalName == playlistSong.InternalName) break;
                    }
                }
                var dtaSong = Parser.Songs[index];
                var newSong = new Song
                {
                    Name = xParent.CleanArtistSong(dtaSong.Name),
                    Artist = xParent.CleanArtistSong(dtaSong.Artist),
                    Location = playlistSong.Location,
                    Length = dtaSong.Length > 0 ? dtaSong.Length : playlistSong.Length,
                    InternalName = dtaSong.InternalName,
                    Album = dtaSong.Album,
                    Year = dtaSong.YearReleased,
                    Track = dtaSong.TrackNumber,
                    Genre = Parser.doGenre(dtaSong.RawGenre),
                    Index = -1,
                    AddToPlaylist = true,
                    AttenuationValues = dtaSong.AttenuationValues.Replace("\t", ""),
                    PanningValues = dtaSong.PanningValues.Replace("\t", ""),
                    Charter = dtaSong.ChartAuthor,
                    ChannelsDrums = dtaSong.ChannelsDrums,
                    ChannelsBass = dtaSong.ChannelsBass,
                    ChannelsGuitar = dtaSong.ChannelsGuitar,
                    ChannelsKeys = dtaSong.ChannelsKeys,
                    ChannelsVocals = dtaSong.ChannelsVocals,
                    ChannelsCrowd = dtaSong.ChannelsCrowd,
                    ChannelsBacking = dtaSong.ChannelsBacking(),
                    DTAIndex = index,
                    isRhythmOnBass = dtaSong.RhythmBass,
                    isRhythmOnKeys = dtaSong.RhythmKeys || (dtaSong.Name.Contains("Rhythm Version") && !dtaSong.RhythmBass),
                    hasProKeys = dtaSong.ProKeysDiff > 0,
                    PSDelay = dtaSong.PSDelay
                };
                RebuiltPlaylist.Add(newSong);
            }
        }
    }
}
