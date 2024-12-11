using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;
using NAudio.Midi;
using Encoder = System.Drawing.Imaging.Encoder;

namespace cPlayer
{
    public class NemoTools
    {
        public int TextureSize = 512; //default value
        public bool KeepDDS = false;
        
        /// <summary>
        /// Will safely try to move, and if fails, copy/delete a file
        /// </summary>
        /// <param name="input">Full starting path of the file</param>
        /// <param name="output">Full destination path of the file</param>
        public bool MoveFile(string input, string output)
        {
            if (string.IsNullOrEmpty(input) || string.IsNullOrEmpty(output)) return false;
            if (!File.Exists(input)) return false;
            
            try
            {
                DeleteFile(output);
                File.Move(input, output);
            }
            catch (Exception)
            {
                try
                {
                    File.Copy(input, output);
                    DeleteFile(input);
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return File.Exists(output);
        }

        /// <summary>
        /// Simple function to safely delete folders
        /// </summary>
        /// <param name="folder">Full path of folder to be deleted</param>
        /// <param name="delete_contents">Whether to delete folders that are not empty</param>
        public void DeleteFolder(string folder, bool delete_contents)
        {
            if (!Directory.Exists(folder)) return;
            try
            {
                if (delete_contents)
                {
                    Directory.Delete(folder, true);
                    return;
                }
                if (!Directory.GetFiles(folder).Any())
                {
                    Directory.Delete(folder);
                }
            }
            catch (Exception)
            {}
        }

        /// <summary>
        /// Simple function to safely delete files
        /// </summary>
        /// <param name="file">Full path of file to be deleted</param>
        public void DeleteFile(string file)
        {
            if (string.IsNullOrEmpty(file) || !File.Exists(file)) return;
            try
            {
                File.Delete(file);
            }
            catch (Exception)
            { }
        }

        /// <summary>
        /// Simple function to safely delete folders
        /// </summary>
        /// <param name="folder">Full path of folder to be deleted</param>
        public void DeleteFolder(string folder)
        {
            if (!Directory.Exists(folder)) return;
            DeleteFolder(folder, false);
        }

        private static byte[] BuildDDSHeader(string format, int width, int height)
        {
            var dds = new byte[] //512x512 DXT5 
                {
                    0x44, 0x44, 0x53, 0x20, 0x7C, 0x00, 0x00, 0x00, 0x07, 0x10, 0x0A, 0x00, 0x00, 0x02, 0x00, 0x00,
                    0x00, 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0A, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x4E, 0x45, 0x4D, 0x4F, 0x00, 0x00, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00,
                    0x04, 0x00, 0x00, 0x00, 0x44, 0x58, 0x54, 0x35, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x00, 0x00,
                    0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
                };

            switch (format.ToLowerInvariant())
            {
                case "dxt1":
                    dds[87] = 0x31;
                    break;
                case "dxt3":
                    dds[87] = 0x33;
                    break;
                case "normal":
                    dds[84] = 0x41;
                    dds[85] = 0x54;
                    dds[86] = 0x49;
                    dds[87] = 0x32;
                    break;
            }

            switch (height)
            {
                case 8:
                    dds[12] = 0x08;
                    dds[13] = 0x00;
                    break;
                case 16:
                    dds[12] = 0x10;
                    dds[13] = 0x00;
                    break;
                case 32:
                    dds[12] = 0x20;
                    dds[13] = 0x00;
                    break;
                case 64:
                    dds[12] = 0x40;
                    dds[13] = 0x00;
                    break;
                case 128:
                    dds[12] = 0x80;
                    dds[13] = 0x00;
                    break;
                case 256:
                    dds[13] = 0x01;
                    break;
                case 1024:
                    dds[13] = 0x04;
                    break;
                case 2048:
                    dds[13] = 0x08;
                    break;
            }

            switch (width)
            {
                case 8:
                    dds[16] = 0x08;
                    dds[17] = 0x00;
                    break;
                case 16:
                    dds[16] = 0x10;
                    dds[17] = 0x00;
                    break;
                case 32:
                    dds[16] = 0x20;
                    dds[17] = 0x00;
                    break;
                case 64:
                    dds[16] = 0x40;
                    dds[17] = 0x00;
                    break;
                case 128:
                    dds[16] = 0x80;
                    dds[17] = 0x00;
                    break;
                case 256:
                    dds[17] = 0x01;
                    break;
                case 1024:
                    dds[17] = 0x04;
                    break;
                case 2048:
                    dds[17] = 0x08;
                    break;
            }

            if (width == height)
            {
                switch (width)
                {
                    case 8:
                        dds[0x1C] = 0x00; //no mipmaps at this size
                        break;
                    case 16:
                        dds[0x1C] = 0x05;
                        break;
                    case 32:
                        dds[0x1C] = 0x06;
                        break;
                    case 64:
                        dds[0x1C] = 0x07;
                        break;
                    case 128:
                        dds[0x1C] = 0x08;
                        break;
                    case 256:
                        dds[0x1C] = 0x09;
                        break;
                    case 1024:
                        dds[0x1C] = 0x0B;
                        break;
                    case 2048:
                        dds[0x1C] = 0x0C;
                        break;
                }
            }
            return dds;
        }

        /// <summary>
        /// Figure out right DDS header to go with HMX texture
        /// </summary>
        /// <param name="full_header">First 16 bytes of the png_xbox/png_ps3 file</param>
        /// <param name="short_header">Bytes 5-16 of the png_xbox/png_ps3 file</param>
        /// <returns></returns>
        private static byte[] GetDDSHeader(byte[] full_header, byte[] short_header)
        {
            //official album art header, most likely to be the one being requested
            var header = BuildDDSHeader("dxt1", 256, 256);

            var headers = Directory.GetFiles(Application.StartupPath + "\\bin\\headers\\", "*.header");
            foreach (var head in headers)
            {
                var header_bytes = File.ReadAllBytes(head);
                if (!full_header.SequenceEqual(header_bytes) && !short_header.SequenceEqual(header_bytes)) continue;

                var head_name = Path.GetFileNameWithoutExtension(head).ToLowerInvariant();
                var format = "dxt5";
                if (head_name.Contains("dxt1"))
                {
                    format = "dxt1";
                }
                else if (head_name.Contains("normal"))
                {
                    format = "normal";
                }

                var index1 = head_name.IndexOf("_", StringComparison.Ordinal) + 1;
                var index2 = head_name.IndexOf("x", StringComparison.Ordinal);
                var width = Convert.ToInt16(head_name.Substring(index1, index2 - index1));
                index1 = head_name.IndexOf("_", index2, StringComparison.Ordinal);
                index2++;
                var height = Convert.ToInt16(head_name.Substring(index2, index1 - index2));

                header = BuildDDSHeader(format, width, height);
                break;
            }
            return header;
        }

        public bool XMASH(string xma)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "xmash.exe"))
            {
                MessageBox.Show("xmash.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var newPath = Path.GetDirectoryName(xma) + "\\xmash.exe";
            if (!File.Exists(newPath))
            {
                File.Copy(path + "xmash.exe", newPath, true);
            }

            var arg = " \"" + Path.GetFileName(xma) + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = newPath,
                Arguments = arg,
                WorkingDirectory = Path.GetDirectoryName(newPath)
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();

            var xmas = Directory.GetFiles(Path.GetDirectoryName(xma), "*.xma");
            return xmas.Count() > 1;
        }

        public bool toWAV(string xma)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "towav.exe"))
            {
                MessageBox.Show("towav.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            var newPath = Path.GetDirectoryName(xma) + "\\towav.exe";
            if (!File.Exists(newPath))
            {
                File.Copy(path + "towav.exe", newPath, true);
            }

            var arg = " \"" + Path.GetFileName(xma) + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = newPath,
                Arguments = arg,
                WorkingDirectory = Path.GetDirectoryName(newPath)
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();
            var wav = xma.Substring(0, xma.Length - 3) + "wav";
            return File.Exists(wav);
        }

        public bool ConvertBandFuse(string argument, string input, string output)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "songfuse.exe"))
            {
                MessageBox.Show("songfuse.exe is missing from the bin directory and I can't continue without it", "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            DeleteFile(output);
            var arg = argument + " \"" + input + "\" \"" + output + "\"";
            var app = new ProcessStartInfo
            {
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                FileName = path + "songfuse.exe",
                Arguments = arg,
                WorkingDirectory = path
            };
            var process = Process.Start(app);
            do
            {
                //
            } while (!process.HasExited);
            process.Dispose();
            return File.Exists(output);
        }


        /// <summary>
        /// Converts png_xbox files to usable format
        /// </summary>
        /// <param name="rb_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <param name="delete_original">True: delete | False: keep (default)</param>
        /// <returns></returns>
        public bool ConvertRBImage(string rb_image, string output_path, string format, bool delete_original)
        {
            var ddsfile = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path) + ".dds";
            var tgafile = ddsfile.Replace(".dds", ".tga");
            
            var nv_tool = Application.StartupPath + "\\bin\\nvdecompress.exe";
            if (!File.Exists(nv_tool))
            {
                MessageBox.Show("nvdecompress.exe is missing and is required\nProcess aborted", "Nemo Tools",MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            try
            {
                DeleteFile(ddsfile);
                DeleteFile(tgafile);

                //read raw file bytes
                var ddsbytes = File.ReadAllBytes(rb_image);

                var buffer = new byte[4];
                var swap = new byte[4];

                //get filesize / 4 for number of times to loop
                //32 is the size of the HMX header to skip
                var loop = (ddsbytes.Length - 32) / 4;

                //skip the HMX header
                var input = new MemoryStream(ddsbytes, 32, ddsbytes.Length - 32);

                //grab HMX header to compare against known headers
                var full_header = new byte[16];
                var file_header = new MemoryStream(ddsbytes, 0, 16);
                file_header.Read(full_header, 0, 16);
                file_header.Dispose();

                //some games have a bunch of headers for the same files, so let's skip the varying portion and just
                //grab the part that tells us the dimensions and image format
                var short_header = new byte[11];
                file_header = new MemoryStream(ddsbytes, 5, 11);
                file_header.Read(short_header, 0, 11);
                file_header.Dispose();

                //create dds file
                var output = new FileStream(ddsfile, FileMode.Create);
                var header = GetDDSHeader(full_header, short_header);
                output.Write(header, 0, header.Length);

                //here we go
                for (var x = 0; x <= loop; x++)
                {
                    input.Read(buffer, 0, 4);

                    //PS3 images are not byte swapped, just DDS images with HMX header on top
                    if (rb_image.EndsWith("_ps3", StringComparison.Ordinal))
                    {
                        swap = buffer;
                    }
                    else
                    {
                        //XBOX images are byte swapped, so we gotta return it
                        swap[0] = buffer[1];
                        swap[1] = buffer[0];
                        swap[2] = buffer[3];
                        swap[3] = buffer[2];
                    }
                    output.Write(swap, 0, 4);
                }
                input.Dispose();
                output.Dispose();

                //read raw dds bytes
                ddsbytes = File.ReadAllBytes(ddsfile);

                //grab relevant part of dds header
                var header_stream = new MemoryStream(ddsbytes, 0, 32);
                var size = new byte[32];
                header_stream.Read(size, 0, 32);
                header_stream.Dispose();

                //default to 256x256
                var width = 256;

                //get dds dimensions from header
                switch (size[17]) //width byte
                {
                    case 0x00:
                        switch (size[16])
                        {
                            case 0x08:
                                width = 8;
                                break;
                            case 0x10:
                                width = 16;
                                break;
                            case 0x20:
                                width = 32;
                                break;
                            case 0x40:
                                width = 64;
                                break;
                            case 0x80:
                                width = 128;
                                break;
                        }
                        break;
                    case 0x02:
                        width = 512;
                        break;
                    case 0x04:
                        width = 1024;
                        break;
                    case 0x08:
                        width = 2048;
                        break;
                }
                TextureSize = width;

                var arg = "\"" + ddsfile + "\"";
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = nv_tool,
                    Arguments = arg,
                    WorkingDirectory = Application.StartupPath + "\\bin\\"
                };
                var process = Process.Start(startInfo);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
                
                if (!ResizeImage(tgafile, TextureSize, format, output_path))
                {
                    DeleteFile(tgafile);
                    return false;
                }
                DeleteFile(ddsfile);
                DeleteFile(tgafile);
                if (delete_original)
                {
                    DeleteFile(rb_image);
                }
                Application.DoEvents();
                return true;
            }
            catch (Exception)
            {
                if (!rb_image.EndsWith(".dds", StringComparison.Ordinal))
                {
                    DeleteFile(ddsfile);
                }
                return false;
            }
        }

        /// <summary>
        /// Converts png_wii files to usable format
        /// </summary>
        /// <param name="wii_image">Full path to the png_xbox / png_ps3 / dds file</param>
        /// <param name="output_path">Full path you'd like to save the converted image</param>
        /// <param name="format">Allowed formats: BMP | JPG | PNG (default)</param>
        /// <param name="delete_original">True: delete | False: keep (default)</param>
        /// <returns></returns>
        public bool ConvertWiiImage(string wii_image, string output_path, string format, bool delete_original)
        {
            var tplfile = Path.GetDirectoryName(wii_image) + "\\" + Path.GetFileNameWithoutExtension(wii_image) + ".tpl";
            var pngfile = tplfile.Replace(".tpl", ".png");
            var Headers = new ImageHeaders();
            
            TextureSize = 128;
            DeleteFile(pngfile);

            try
            {
                if (tplfile != wii_image)
                {
                    DeleteFile(tplfile);

                    var binaryReader = new BinaryReader(File.OpenRead(wii_image));
                    var binaryWriter = new BinaryWriter(new FileStream(tplfile, FileMode.Create));

                    var wii_header = new byte[32];
                    binaryReader.Read(wii_header, 0, 32);

                    byte[] tpl_header;
                    if (wii_header.SequenceEqual(Headers.wii_128x256))
                    {
                        tpl_header = Headers.tpl_128x256;
                        TextureSize = 256;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x128_rgba32))
                    {
                        tpl_header = Headers.tpl_128x128_rgba32;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_256x256) ||
                             wii_header.SequenceEqual(Headers.wii_256x256_B) ||
                             wii_header.SequenceEqual(Headers.wii_256x256_c8))
                    {
                        tpl_header = Headers.tpl_256x256;
                        TextureSize = 256;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_128x128))
                    {
                        tpl_header = Headers.tpl_128x128;
                    }
                    else if (wii_header.SequenceEqual(Headers.wii_64x64))
                    {
                        TextureSize = 64;
                        tpl_header = Headers.tpl_64x64;
                    }
                    else
                    {
                        MessageBox.Show("File " + Path.GetFileName(wii_image) +
                            " has a header I don't recognize, so I can't convert it",
                            "Nemo Tools", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    binaryWriter.Write(tpl_header);

                    var buffer = new byte[64];
                    int num;
                    do
                    {
                        num = binaryReader.Read(buffer, 0, 64);
                        if (num > 0)
                            binaryWriter.Write(buffer);
                    } while (num > 0);
                    binaryWriter.Dispose();
                    binaryReader.Dispose();
                }

                //this is so image quality is higher than the default
                var myEncoder = Encoder.Quality;
                var myEncoderParameters = new EncoderParameters(1);
                var myEncoderParameter = new EncoderParameter(myEncoder, 100L);
                myEncoderParameters.Param[0] = myEncoderParameter;

                var img = TPL.ConvertFromTPL(tplfile);
                img.Save(pngfile, GetEncoderInfo("image/png"), myEncoderParameters);
                img.Dispose();

                if (!File.Exists(pngfile))
                {
                    if (tplfile != wii_image)
                    {
                        DeleteFile(tplfile);
                    }
                    return false;
                }
                if (!format.ToLowerInvariant().Contains("png"))
                {
                    var image = NemoLoadImage(pngfile);
                    if (!ResizeImage(pngfile, image.Width, format, output_path))
                    {
                        image.Dispose();
                        DeleteFile(pngfile);
                        return false;
                    }
                    image.Dispose();
                }

                if (tplfile != wii_image && !KeepDDS)
                {
                    DeleteFile(tplfile);
                }
                if (!format.ToLowerInvariant().Contains("png"))
                {
                    DeleteFile(pngfile);
                }
                if (delete_original)
                {
                    DeleteFile(wii_image);
                }
                Application.DoEvents();
                return true;
            }
            catch (Exception)
            {
                if (tplfile != wii_image)
                {
                    DeleteFile(tplfile);
                }
                DeleteFile(pngfile);
                return false;
            }
        }

        /// <summary>
        /// Use to resize images up or down or convert across BMP/JPG/PNG/TIF
        /// </summary>
        /// <param name="image_path">Full file path to source image</param>
        /// <param name="image_size">Integer for image size, can be smaller or bigger than source image</param>
        /// <param name="format">Format to save the image in: BMP | JPG | TIF | PNG (default)</param>
        /// <param name="output_path">Full file path to output image</param>
        /// <returns></returns>
        [HandleProcessCorruptedStateExceptions]
        public bool ResizeImage(string image_path, int image_size, string format, string output_path)
        {
            try
            {
                var newimage = Path.GetDirectoryName(output_path) + "\\" + Path.GetFileNameWithoutExtension(output_path);

                Il.ilInit();
                Ilu.iluInit();

                var imageId = new int[10];

                // Generate the main image name to use
                Il.ilGenImages(1, imageId);

                // Bind this image name
                Il.ilBindImage(imageId[0]);

                // Loads the image into the imageId
                if (!Il.ilLoadImage(image_path))
                {
                    return false;
                }
                // Enable overwriting destination file
                Il.ilEnable(Il.IL_FILE_OVERWRITE);

                var height = image_size;
                var width = image_size;

                //assume we're downscaling, this is better filter
                const int scaler = Ilu.ILU_BILINEAR;

                //resize image
                Ilu.iluImageParameter(Ilu.ILU_FILTER, scaler);
                Ilu.iluScale(width, height, 1);

                if (format.ToLowerInvariant().Contains("bmp"))
                {
                    //disable compression
                    Il.ilSetInteger(Il.IL_BMP_RLE, 0);
                    newimage = newimage + ".bmp";
                }
                else if (format.ToLowerInvariant().Contains("jpg") || format.ToLowerInvariant().Contains("jpeg"))
                {
                    Il.ilSetInteger(Il.IL_JPG_QUALITY, 99);
                    newimage = newimage + ".jpg";
                }
                else if (format.ToLowerInvariant().Contains("tif"))
                {
                    newimage = newimage + ".tif";
                }
                else if (format.ToLowerInvariant().Contains("tga"))
                {
                    Il.ilSetInteger(Il.IL_TGA_RLE, 0);
                    newimage = newimage + ".tga";
                }
                else
                {
                    Il.ilSetInteger(Il.IL_PNG_INTERLACE, 0);
                    newimage = newimage + ".png";
                }

                if (!Il.ilSaveImage(newimage))
                {
                    return false;
                }

                // Done with the imageId, so let's delete it
                Il.ilDeleteImages(1, imageId);
                Application.DoEvents();
                return File.Exists(newimage);
            }
            catch (AccessViolationException)
            {}
            catch (Exception)
            {}
            return false;
        }

        [HandleProcessCorruptedStateExceptions]
        public void CreateBlurredArt(string raw_input, string output)
        {
            if (!File.Exists(raw_input)) return;
            DeleteFile(output);

            //verify that the alleged png file is actually png and not jpg with changed extension
            //this was observed with GH to PS conversions
            var png = new byte[] {0x89, 0x50, 0x4E, 0x47};
            var header = new byte[png.Length];
            using (var br = new BinaryReader(File.Open(raw_input, FileMode.Open, FileAccess.Read)))
            {
                header = br.ReadBytes(header.Length);
            }
            var input = raw_input;
            var ext = Path.GetExtension(raw_input);
            if (!header.SequenceEqual(png) && ext.ToLowerInvariant() == ".png")
            {
                //it's not PNG, assume it's JPG
                input = raw_input.Replace(ext, ".jpg");
                DeleteFile(input);
                File.Copy(raw_input, input);
            }

            try
            {
                Il.ilInit();
                Ilu.iluInit();

                var img = Image.FromFile(input);
                var orig_width = img.Width;
                img.Dispose();

                var imageId = new int[2];
                Il.ilGenImages(2, imageId);
                Il.ilBindImage(imageId[0]);
                if (!Il.ilLoadImage(input))
                {
                    Il.ilDeleteImages(2, imageId);
                    return;
                }

                var scaler = Ilu.ILU_BILINEAR;
                if (orig_width > 512)
                {
                    Ilu.iluImageParameter(Ilu.ILU_FILTER, scaler);
                    Ilu.iluScale(512, 512, 1);
                    orig_width = 512;
                }

                scaler = Ilu.ILU_SCALE_TRIANGLE;
                Ilu.iluImageParameter(Ilu.ILU_FILTER, scaler);

                Il.ilBindImage(imageId[1]);
                if (!Il.ilLoadImage(input))
                {
                    Il.ilDeleteImages(2, imageId);
                    return;
                }

                Il.ilEnable(Il.IL_FILE_OVERWRITE);
                Il.ilSetInteger(Il.IL_PNG_INTERLACE, 0);

                const int width = 590;
                const int height = 654;

                //resize image
                Ilu.iluScale(width, height, 1);
                Ilu.iluBlurGaussian(25);
                Il.ilOverlayImage(imageId[0], (width - orig_width)/2, (height - orig_width)/2, 0);
                Il.ilSaveImage(output);
                Il.ilDeleteImages(2, imageId);
                if (input != raw_input)
                {
                    //DeleteFile(input);
                }
            }
            catch (AccessViolationException)
            {}
            catch (Exception)
            {}
        }

        /// <summary>
        /// Returns string with correctly formatted characters
        /// </summary>
        /// <param name="raw_line">Raw line from songs.dta file</param>
        /// <returns></returns>
        public string FixBadChars(string raw_line)
        {
            var line = raw_line.Replace("Ã¡", "á");
            line = line.Replace("Ã©", "é");
            line = line.Replace("Ã¨", "è");
            line = line.Replace("ÃŠ", "Ê");
            line = line.Replace("Ã¬", "ì");
            line = line.Replace("Ã­­­", "í");
            line = line.Replace("Ã¯", "ï");
            line = line.Replace("Ã–", "Ö");
            line = line.Replace("Ã¶", "ö");
            line = line.Replace("Ã³", "ó");
            line = line.Replace("Ã²", "ò");
            line = line.Replace("Ãœ", "Ü");
            line = line.Replace("Ã¼", "ü");
            line = line.Replace("Ã¹", "ù");
            line = line.Replace("Ãº", "ú");
            line = line.Replace("Ã¿", "ÿ");
            line = line.Replace("Ã±", "ñ");
            line = line.Replace("ï¿½", "");
            line = line.Replace("�", "");
            line = line.Replace("E½", "");
            return line;
        }
        
        /// <summary>
        /// Returns byte array in hex value
        /// </summary>
        /// <param name="xIn">String value to be converted to hex</param>
        /// <returns></returns>
        public byte[] ToHex(string xIn)
        {
            for (var i = 0; i < (xIn.Length % 2); i++)
                xIn = "0" + xIn;
            var xReturn = new List<byte>();
            for (var i = 0; i < (xIn.Length / 2); i++)
                xReturn.Add(Convert.ToByte(xIn.Substring(i * 2, 2), 16));
            return xReturn.ToArray();
        }
        
        /// <summary>
        /// Returns clean Track Name from midi event string
        /// </summary>
        /// <param name="raw_event">The raw ToString value of the midi event</param>
        /// <returns></returns>
        public string GetMidiTrackName(string raw_event)
        {
            var name = raw_event;
            name = name.Substring(2, name.Length - 2); //remove track number
            name = name.Replace("SequenceTrackName", "");
            return name.Trim();
        }
        
        /// <summary>
        /// Returns cleaned string for file names, etc
        /// </summary>
        /// <param name="raw_string">Raw string from the songs.dta file</param>
        /// <param name="removeDash">Whether to remove dashes from the string</param>
        /// <param name="DashForSlash">Whether to replace slashes with dashes</param>
        /// <returns></returns>
        public string CleanString(string raw_string, bool removeDash, bool DashForSlash = false)
        {
            var mystring = raw_string;

            //remove forbidden characters if present
            mystring = mystring.Replace("\"", "");
            mystring = mystring.Replace(">", " ");
            mystring = mystring.Replace("<", " ");
            mystring = mystring.Replace(":", " ");
            mystring = mystring.Replace("|", " ");
            mystring = mystring.Replace("?", " ");
            mystring = mystring.Replace("*", " ");
            mystring = mystring.Replace("'", "");
            mystring = mystring.Replace("&#8217;", "'"); //Don't Speak
            mystring = mystring.Replace("   ", "");
            mystring = mystring.Replace("  ", "");
            mystring = mystring.Replace(" ", "");
            mystring = FixBadChars(mystring).Trim();

            if (removeDash)
            {
                if (mystring.Substring(0, 1) == "-") //if starts with -
                {
                    mystring = mystring.Substring(1, mystring.Length - 1);
                }
                if (mystring.Substring(mystring.Length - 1, 1) == "-") //if ends with -
                {
                    mystring = mystring.Substring(0, mystring.Length - 1);
                }

                mystring = mystring.Trim();
            }

            if (mystring.EndsWith(".", StringComparison.Ordinal)) //can't have files like Y.M.C.A.
            {
                mystring = mystring.Substring(0, mystring.Length - 1);
            }

            mystring = mystring.Replace("\\", DashForSlash && mystring != "AC/DC" ? "-" : (mystring != "AC/DC" ? " " : ""));
            mystring = mystring.Replace("/", DashForSlash && mystring != "AC/DC" ? "-" : (mystring != "AC/DC" ? " " : ""));

            return mystring;
        }

        public MidiFile NemoLoadMIDI(string midi_in)
        {
            //NAudio is limited in its ability to read some non-standard MIDIs
            //before this step was added, 3 different errors would prevent this program from reading
            //MIDIs with those situations
            //thanks raynebc we can fix them first and load the fixed MIDIs
            var midishrink = Application.StartupPath + "\\bin\\midishrink.exe";
            if (!File.Exists(midishrink)) return null;
            var midi_out = Application.StartupPath + "\\bin\\temp.mid";
            DeleteFile(midi_out);
            MidiFile MIDI;
            try
            {
                MIDI = new MidiFile(midi_in, false);
            }
            catch (Exception)
            {
                var folder = Path.GetDirectoryName(midi_in) ?? Environment.CurrentDirectory;
                var startInfo = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    FileName = Application.StartupPath + "\\bin\\midishrink.exe",
                    Arguments = "\"" + midi_in + "\" \"" + midi_out + "\"",
                    WorkingDirectory = folder
                };
                var start = (DateTime.Now.Minute * 60) + DateTime.Now.Second;
                var process = Process.Start(startInfo);
                do
                {
                    //this code checks for possible memory leak in midishrink
                    //typical usage outputs a fixed file in 1 second or less, at 15 seconds there's a problem
                    if ((DateTime.Now.Minute * 60) + DateTime.Now.Second - start < 15) continue;
                    break;

                } while (!process.HasExited);
                if (!process.HasExited)
                {
                    process.Kill();
                    process.Dispose();
                }
                if (File.Exists(midi_out))
                {
                    try
                    {
                        MIDI = new MidiFile(midi_out, false);
                    }
                    catch (Exception)
                    {
                        MIDI = null;
                    }
                }
                else
                {
                    MIDI = null;
                }
            }
            DeleteFile(midi_out);  //the file created in the loop is useless, delete it
            return MIDI;
        }

        /// <summary>
        /// Use to quickly grab value on right side of = in C3 options/fix files
        /// </summary>
        /// <param name="raw_line">Raw line from the c3 file</param>
        /// <returns></returns>
        public string GetConfigString(string raw_line)
        {
            var line = raw_line;
            var index = line.IndexOf("=", StringComparison.Ordinal) + 1;
            try
            {
                line = line.Substring(index, line.Length - index);
            }
            catch (Exception)
            {
                line = "";
            }
            return line.Trim();
        }

        public ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

        /// <summary>
        /// Loads image and unlocks file for uses elsewhere. USE THIS!
        /// </summary>
        /// <param name="file">Full path to the image file</param>
        /// <returns></returns>
        public Image NemoLoadImage(string file)
        {
            if (!File.Exists(file))
            {
                return null;
            }
            Image img = null;

            if (Path.GetExtension(file) == ".dds")
            {
                if (ConvertRBImage(file, file.Replace(".dds", ".png"), "png", false))
                {
                    file = file.Replace(".dds", ".png");
                }
            }
            try
            {
                using (var bmpTemp = new Bitmap(file))
                {
                    img = new Bitmap(bmpTemp);
                }
            }
            catch (Exception)
            {}
            return img;
        }

        /// <summary>
        /// Returns line with featured artist normalized as 'ft.'
        /// </summary>
        /// <param name="line">Line to normalize</param>
        /// <returns></returns>
        public string FixFeaturedArtist(string line)
        {
            if (string.IsNullOrEmpty(line)) return "";

            var adjusted = line;

            adjusted = adjusted.Replace("Featuring", "ft.");
            adjusted = adjusted.Replace("featuring", "ft.");
            adjusted = adjusted.Replace("feat.", "ft.");
            adjusted = adjusted.Replace("Feat.", "ft.");
            adjusted = adjusted.Replace(" ft ", " ft. ");
            adjusted = adjusted.Replace(" FT ", " ft. ");
            adjusted = adjusted.Replace("Ft. ", "ft. ");
            adjusted = adjusted.Replace("FEAT. ", "ft. ");
            adjusted = adjusted.Replace(" FEAT ", " ft. ");

            if (adjusted.StartsWith("ft ", StringComparison.Ordinal))
            {
                adjusted = "ft. " + adjusted.Substring(3, adjusted.Length - 3);
            }

            return FixBadChars(adjusted);
        }

        /// <summary>
        /// Loads and formats help file for display on the HelpForm
        /// </summary>
        /// <param name="file">Name of the file, path assumed to be \bin\help/</param>
        /// <returns></returns>
        public string ReadHelpFile(string file)
        {
            var message = "";
            var helpfile = Application.StartupPath + "\\bin\\help\\" + file;
            if (File.Exists(helpfile))
            {
                var sr = new StreamReader(helpfile);
                while (sr.Peek() > 0)
                {
                    var line = sr.ReadLine();
                    message = message == "" ? line : message + "\r\n" + line;
                }
                sr.Dispose();
            }
            else
            {
                message = "Could not find help file, please redownload this program and DO NOT delete any files";
            }

            return message;
        }

        public Color LightenColor(Color color)
        {
            var correctionFactor = (float) 0.20;

            var red = (float)color.R;
            var green = (float)color.G;
            var blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }
            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        private const string ClientId = "0833089fd629761"; //from imgur, specific to this program, do not use elsewhere
        public string UploadToImgur(string image)
        {
            var link = "";
            try
            {
                using (var w = new WebClient())
                {
                    var values = new NameValueCollection
                        {
                            {"image", Convert.ToBase64String(File.ReadAllBytes(image))}
                        };
                    w.Headers.Add("Authorization", "Client-ID " + ClientId);
                    var response = w.UploadValues("https://api.imgur.com/3/upload.xml", values);
                    var sr = new StreamReader(new MemoryStream(response), Encoding.Default);
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        if (line == null || !line.Contains("link")) continue;
                        //get substring starting at http
                        line = line.Substring(line.IndexOf(":", StringComparison.Ordinal) - 5, line.Length - line.IndexOf(":", StringComparison.Ordinal));
                        //split string starting at </link
                        line = line.Substring(0, line.IndexOf("<", StringComparison.Ordinal));
                        link = line;
                    }
                    sr.Dispose();
                }
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                link = "";
                if (error.Contains("429"))
                {
                    error = "Error Code 429: Rate limiting\nThis most likely means you've uploaded too many images\nPlease wait a couple of hours and try again";
                }
                else if (error.Contains("500"))
                {
                    error = "Error Code 500: Unexpected internal error\nThis means something is broken with the imgur service\nPlease try again later";
                }
                if (MessageBox.Show("Sorry, there was an error in uploading the file!\nThe error says:\n" + error + "\nTry again?", "Error",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
                {
                    UploadToImgur(image);
                }
            }
            return link;
        }

        public bool ExtractPKG(string pkg, string folder, out string klic)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "pkgripper.exe"))
            {
                MessageBox.Show("pkgripper.exe is missing from the bin directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                klic = "";
                return false;
            }
            try
            {
                var arg = " -o \"" + folder + "\" \"" + pkg + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    FileName = path + "pkgripper.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting package:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                klic = "";
                return false;
            }

            var success = false;
            if (Directory.Exists(folder))
            {
                var dta = Directory.GetFiles(folder, "*.dta", SearchOption.AllDirectories);
                success = dta.Count() > 0;
            }

            folder += "\\USRDIR\\";
            var int_folder = Directory.GetDirectories(folder);
            var folder_value = Path.GetFileName(int_folder[0]);
            var unhashed_klic = "Ih38rtW1ng3r" + folder_value + "10025250";
            klic = CreateMD5(unhashed_klic);
            return success;
        }

        public string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                return BitConverter.ToString(hashBytes).Replace("-", "");
            }
        }

        public bool DecryptEdat(string edat, string midi, string klic = "")
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "edattool.exe"))
            {
                MessageBox.Show("edattool.exe is missing from the bin directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            const string c3_klic = "0B72B62DABA8CAFDA3352FF979C6D5C2";
            if (string.IsNullOrEmpty(klic))
            {
                klic = c3_klic;
            }
            try
            {
                var arg = " decrypt -custom:" + klic + " \"" + edat + "\" \"" + midi + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    FileName = path + "edattool.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error decrypting EDAT file:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return File.Exists(midi);
        }
    
    public bool ExtractPsArc(string inFile, string outFolder, string rsFolder)
        {
            var path = Application.StartupPath + "\\bin\\rs2014\\";
            if (!File.Exists(path + "packer.exe"))
            {
                MessageBox.Show("packer.exe is missing from the bin\\rs2014 directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                var arg = " -u -v=RS2014 -f=PC -d=false -i=\"" + inFile + "\" -o=\"" + outFolder + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    FileName = path + "packer.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting package:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            //Thread.Sleep(5000);
            return Directory.Exists(rsFolder);
        }

        public bool ExtractSNG(string inFile, string outFolder)
        {
            var path = Application.StartupPath + "\\bin\\";
            if (!File.Exists(path + "SngCli.exe"))
            {
                MessageBox.Show("SngCli.exe is missing from the bin directory and I can't continue without it", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            try
            {
                var arg = " decode -i \"" + inFile + "\" -o \"" + outFolder + "\"";
                var app = new ProcessStartInfo
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = false,
                    RedirectStandardError = false,
                    RedirectStandardInput = false,
                    UseShellExecute = false,
                    FileName = path + "SngCli.exe",
                    Arguments = arg,
                    WorkingDirectory = path
                };
                var process = Process.Start(app);
                do
                {
                    //
                } while (!process.HasExited);
                process.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error extracting package:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return Directory.Exists(outFolder) && File.Exists(outFolder + "\\song.ini");
        }

        #region GHWT:DE Stuff - Most of the code by AddyMills
        private static byte[] FSB4 = new byte[] { 0x46, 0x53, 0x42, 0x34 };

        public bool fsbIsEncrypted(string fsb)
        {
            if (!File.Exists(fsb)) return false;
            using (var ms = new MemoryStream(File.ReadAllBytes(fsb)))
            {
                using (var br = new BinaryReader(ms))
                {
                    var fsb4 = br.ReadBytes(4);
                    return !fsb4.SequenceEqual(FSB4);
                }
            }
        }

        /// <summary>
        /// Removes the first character from the file name if it starts with "adlc".
        /// </summary>
        /// <param name="fileName">The file name to be renamed.</param>
        /// <returns>The renamed file name.</returns>
        public static string FileRenamer(string fileName)
        {
            if (fileName.StartsWith("adlc"))
            {
                // Remove the first character
                fileName = fileName.Substring(1);
            }
            return fileName;
        }

        /// <summary>
        /// Flips the bits of the given audio.
        /// </summary>
        /// <param name="audio">The audio to flip the bits of.</param>
        /// <returns>The audio with flipped bits.</returns>
        public static byte[] FlipBits(byte[] audio)
        {
            byte[] result = new byte[audio.Length];
            for (int i = 0; i < audio.Length; i++)
            {
                result[i] = binaryReverse[audio[i]];
            }
            return result;
        }

        /// <summary>
        /// Performs the XOR operation between the audio and the key.
        /// </summary>
        /// <param name="audio">The audio to perform the XOR operation on.</param>
        /// <param name="key">The key to use for the XOR operation.</param>
        /// <returns>The result of the XOR operation.</returns>
        public static byte[] XorProcess(byte[] audio, byte[] key)
        {
            // Calculate the number of repetitions needed to match or exceed the length of the audio.
            int repetitions = 1 + (audio.Length / key.Length);

            // Create an array to hold the extended key.
            byte[] extendedKey = new byte[audio.Length];

            // Fill the extendedKey array with repeated copies of the key.
            for (int i = 0; i < repetitions; i++)
            {
                Array.Copy(key, 0, extendedKey, i * key.Length, Math.Min(key.Length, extendedKey.Length - (i * key.Length)));
            }

            // Create an array to hold the result of the XOR operation.
            byte[] result = new byte[audio.Length];

            // Perform the XOR operation between each byte of the audio and the extended key.
            for (int i = 0; i < audio.Length; i++)
            {
                result[i] = (byte)(audio[i] ^ extendedKey[i]);
            }

            return result;
        }

        /// <summary>
        /// Generates an FSB encryption key based on the provided string.
        /// </summary>
        /// <param name="toGen">The string used to generate the key.</param>
        /// <returns>The generated FSB key.</returns>
        public static byte[] GenerateFsbKey(string toGen)
        {
            uint xor = 0xffffffff;
            string encStr = "";
            const int cycle = 32;
            List<byte> key = new List<byte>();

            for (int i = 0; i < cycle; i++)
            {
                char ch = toGen[i % toGen.Length];
                uint crc = QBKeyUInt(new string(ch, 1));
                xor ^= crc;

                int index = (int)(xor % toGen.Length);
                encStr += toGen[index];
            }

            for (int i = 0; i < cycle - 1; i++)
            {
                char ch = encStr[i];
                uint crc = QBKeyUInt(new string(ch, 1));
                xor ^= crc;

                int c = i & 0x03;
                xor >>= c;

                uint z = 0; // Set to 0
                for (int x = 0; x < 32 - c; x++)
                {
                    z += (uint)(1 << x);
                }

                xor &= z;

                byte checkByte = (byte)(xor & 0xFF); // Equivalent to Python's hex(xor)[-2:],16

                if (checkByte == 0)
                {
                    break;
                }

                key.Add(checkByte);
            }

            return key.ToArray();
        }

        public static uint QBKeyUInt(string textBytes)
        {
            return ConvertHexToUInt(QBKey(textBytes));
        }
        private static uint ConvertHexToUInt(string hexString)
        {
            // Ensure the string starts with '0x' and is not longer than 10 characters.
            if (hexString.StartsWith("0x") && hexString.Length <= 10)
            {
                return Convert.ToUInt32(hexString, 16);
            }
            throw new ArgumentException("Invalid hex string: " + hexString);
        }

        public static string QBKey(string text)
        {
            if (text.StartsWith("0x") && text.Length <= 10)
            {
                return text;
            }
            text = text.ToLower().Replace("/", "\\");
            byte[] textBytes = Encoding.UTF8.GetBytes(text);

            return GenQBKey(textBytes);
        }

        public static string GenQBKey(byte[] textBytes)
        {
            uint crc = 0xffffffff;

            foreach (var b in textBytes)
            {
                uint numA = (crc ^ b) & 0xFF;
                crc = CRC32Table[numA] ^ crc >> 8 & 0x00ffffff;
            }

            uint finalCRC = ~crc;
            long value = -finalCRC - 1;
            string result = (value & 0xffffffff).ToString("x8");

            // Pad to 8 characters
            result = result.PadLeft(8, '0');
            result = "0x" + result;
            return result;
        }

        /// <summary>
        /// Decrypts the given audio using the provided key.
        /// </summary>
        /// <param name="audio">The audio to decrypt.</param>
        /// <param name="key">The key to use for decryption.</param>
        /// <returns>The decrypted audio.</returns>
        public static byte[] DecryptFsb4(byte[] audio, byte[] key)
        {
            var decrypted = FlipBits(audio);
            decrypted = XorProcess(decrypted, key);

            return decrypted;
        }

        /// <summary>
        /// Decrypts the given audio file.
        /// </summary>
        /// <param name="audio">The audio file to decrypt.</param>
        /// <param name="filename">The name of the audio file.</param>
        /// <returns>The decrypted audio.</returns>
        /// <exception cref="NotImplementedException">Thrown when the file type is not supported.</exception>
        public static byte[] DecryptFile(byte[] audio, string filename = "")
        {
            // Remove the extension and convert to lowercase. Sometimes there are two extensions which this hopefully covers.
            string noExt = FileRenamer(Path.GetFileNameWithoutExtension(filename).ToLower()).Replace(".fsb", "");
            byte[] key = GenerateFsbKey(noExt);
            return DecryptFsb4(audio, key);
        }

        /// <summary>
        /// Decrypts the audio file from the specified file path.
        /// </summary>
        /// <param name="filePath">The path of the audio file.</param>
        /// <returns>The decrypted audio.</returns>
        public byte[] DecryptFSBFile(string filePath)
        {
            var audio = File.ReadAllBytes(filePath);
            return DecryptFile(audio, filePath);
        }

        public static Dictionary<byte, byte> binaryReverse = new Dictionary<byte, byte>
        {
            {0, 0},
            {1, 128},
            {2, 64},
            {3, 192},
            {4, 32},
            {5, 160},
            {6, 96},
            {7, 224},
            {8, 16},
            {9, 144},
            {10, 80},
            {11, 208},
            {12, 48},
            {13, 176},
            {14, 112},
            {15, 240},
            {16, 8},
            {17, 136},
            {18, 72},
            {19, 200},
            {20, 40},
            {21, 168},
            {22, 104},
            {23, 232},
            {24, 24},
            {25, 152},
            {26, 88},
            {27, 216},
            {28, 56},
            {29, 184},
            {30, 120},
            {31, 248},
            {32, 4},
            {33, 132},
            {34, 68},
            {35, 196},
            {36, 36},
            {37, 164},
            {38, 100},
            {39, 228},
            {40, 20},
            {41, 148},
            {42, 84},
            {43, 212},
            {44, 52},
            {45, 180},
            {46, 116},
            {47, 244},
            {48, 12},
            {49, 140},
            {50, 76},
            {51, 204},
            {52, 44},
            {53, 172},
            {54, 108},
            {55, 236},
            {56, 28},
            {57, 156},
            {58, 92},
            {59, 220},
            {60, 60},
            {61, 188},
            {62, 124},
            {63, 252},
            {64, 2},
            {65, 130},
            {66, 66},
            {67, 194},
            {68, 34},
            {69, 162},
            {70, 98},
            {71, 226},
            {72, 18},
            {73, 146},
            {74, 82},
            {75, 210},
            {76, 50},
            {77, 178},
            {78, 114},
            {79, 242},
            {80, 10},
            {81, 138},
            {82, 74},
            {83, 202},
            {84, 42},
            {85, 170},
            {86, 106},
            {87, 234},
            {88, 26},
            {89, 154},
            {90, 90},
            {91, 218},
            {92, 58},
            {93, 186},
            {94, 122},
            {95, 250},
            {96, 6},
            {97, 134},
            {98, 70},
            {99, 198},
            {100, 38},
            {101, 166},
            {102, 102},
            {103, 230},
            {104, 22},
            {105, 150},
            {106, 86},
            {107, 214},
            {108, 54},
            {109, 182},
            {110, 118},
            {111, 246},
            {112, 14},
            {113, 142},
            {114, 78},
            {115, 206},
            {116, 46},
            {117, 174},
            {118, 110},
            {119, 238},
            {120, 30},
            {121, 158},
            {122, 94},
            {123, 222},
            {124, 62},
            {125, 190},
            {126, 126},
            {127, 254},
            {128, 1},
            {129, 129},
            {130, 65},
            {131, 193},
            {132, 33},
            {133, 161},
            {134, 97},
            {135, 225},
            {136, 17},
            {137, 145},
            {138, 81},
            {139, 209},
            {140, 49},
            {141, 177},
            {142, 113},
            {143, 241},
            {144, 9},
            {145, 137},
            {146, 73},
            {147, 201},
            {148, 41},
            {149, 169},
            {150, 105},
            {151, 233},
            {152, 25},
            {153, 153},
            {154, 89},
            {155, 217},
            {156, 57},
            {157, 185},
            {158, 121},
            {159, 249},
            {160, 5},
            {161, 133},
            {162, 69},
            {163, 197},
            {164, 37},
            {165, 165},
            {166, 101},
            {167, 229},
            {168, 21},
            {169, 149},
            {170, 85},
            {171, 213},
            {172, 53},
            {173, 181},
            {174, 117},
            {175, 245},
            {176, 13},
            {177, 141},
            {178, 77},
            {179, 205},
            {180, 45},
            {181, 173},
            {182, 109},
            {183, 237},
            {184, 29},
            {185, 157},
            {186, 93},
            {187, 221},
            {188, 61},
            {189, 189},
            {190, 125},
            {191, 253},
            {192, 3},
            {193, 131},
            {194, 67},
            {195, 195},
            {196, 35},
            {197, 163},
            {198, 99},
            {199, 227},
            {200, 19},
            {201, 147},
            {202, 83},
            {203, 211},
            {204, 51},
            {205, 179},
            {206, 115},
            {207, 243},
            {208, 11},
            {209, 139},
            {210, 75},
            {211, 203},
            {212, 43},
            {213, 171},
            {214, 107},
            {215, 235},
            {216, 27},
            {217, 155},
            {218, 91},
            {219, 219},
            {220, 59},
            {221, 187},
            {222, 123},
            {223, 251},
            {224, 7},
            {225, 135},
            {226, 71},
            {227, 199},
            {228, 39},
            {229, 167},
            {230, 103},
            {231, 231},
            {232, 23},
            {233, 151},
            {234, 87},
            {235, 215},
            {236, 55},
            {237, 183},
            {238, 119},
            {239, 247},
            {240, 15},
            {241, 143},
            {242, 79},
            {243, 207},
            {244, 47},
            {245, 175},
            {246, 111},
            {247, 239},
            {248, 31},
            {249, 159},
            {250, 95},
            {251, 223},
            {252, 63},
            {253, 191},
            {254, 127},
            {255, 255}
        };


        private static readonly uint[] CRC32Table = new uint[]{
    0x00000000, 0x77073096, 0xee0e612c, 0x990951ba,
    0x076dc419, 0x706af48f, 0xe963a535, 0x9e6495a3,
    0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
    0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
    0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de,
    0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
    0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec,
    0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
    0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
    0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b,
    0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940,
    0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
    0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116,
    0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f,
    0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
    0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
    0x76dc4190, 0x01db7106, 0x98d220bc, 0xefd5102a,
    0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433,
    0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818,
    0x7f6a0dbb, 0x086d3d2d, 0x91646c97, 0xe6635c01,
    0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e,
    0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457,
    0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c,
    0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
    0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2,
    0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb,
    0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0,
    0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
    0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086,
    0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f,
    0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4,
    0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
    0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a,
    0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683,
    0xe3630b12, 0x94643b84, 0x0d6d6a3e, 0x7a6a5aa8,
    0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
    0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe,
    0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7,
    0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc,
    0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
    0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252,
    0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b,
    0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60,
    0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
    0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
    0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f,
    0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04,
    0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
    0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a,
    0x9c0906a9, 0xeb0e363f, 0x72076785, 0x05005713,
    0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38,
    0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21,
    0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e,
    0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777,
    0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c,
    0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
    0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,
    0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db,
    0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0,
    0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
    0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6,
    0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf,
    0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
    0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
    };

        #endregion
    }


    public class FolderPicker
{
    private readonly List<string> _resultPaths = new List<string>();
    private readonly List<string> _resultNames = new List<string>();

    public IReadOnlyList<string> ResultPaths => _resultPaths;
    public IReadOnlyList<string> ResultNames => _resultNames;
    public string ResultPath => ResultPaths.FirstOrDefault();
    public string ResultName => ResultNames.FirstOrDefault();
    public virtual string InputPath { get; set; }
    public virtual bool ForceFileSystem { get; set; }
    public virtual bool Multiselect { get; set; }
    public virtual string Title { get; set; }
    public virtual string OkButtonLabel { get; set; }
    public virtual string FileNameLabel { get; set; }

    protected virtual int SetOptions(int options)
    {
        if (ForceFileSystem)
        {
            options |= (int)FOS.FOS_FORCEFILESYSTEM;
        }

        if (Multiselect)
        {
            options |= (int)FOS.FOS_ALLOWMULTISELECT;
        }
        return options;
    }

    // for all .NET
    public virtual bool? ShowDialog(IntPtr owner, bool throwOnError = false)
    {
        var dialog = (IFileOpenDialog)new FileOpenDialog();
        if (!string.IsNullOrEmpty(InputPath))
        {
            if (CheckHr(SHCreateItemFromParsingName(InputPath, null, typeof(IShellItem).GUID, out var item), throwOnError) != 0)
                return null;

            dialog.SetFolder(item);
        }

        var options = FOS.FOS_PICKFOLDERS;
        options = (FOS)SetOptions((int)options);
        dialog.SetOptions(options);

        if (Title != null)
        {
            dialog.SetTitle(Title);
        }

        if (OkButtonLabel != null)
        {
            dialog.SetOkButtonLabel(OkButtonLabel);
        }

        if (FileNameLabel != null)
        {
            dialog.SetFileName(FileNameLabel);
        }

        if (owner == IntPtr.Zero)
        {
            owner = Process.GetCurrentProcess().MainWindowHandle;
            if (owner == IntPtr.Zero)
            {
                owner = GetDesktopWindow();
            }
        }

        var hr = dialog.Show(owner);
        if (hr == ERROR_CANCELLED)
            return null;

        if (CheckHr(hr, throwOnError) != 0)
            return null;

        if (CheckHr(dialog.GetResults(out var items), throwOnError) != 0)
            return null;

        items.GetCount(out var count);
        for (var i = 0; i < count; i++)
        {
            items.GetItemAt(i, out var item);
            CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEPARSING, out var path), throwOnError);
            CheckHr(item.GetDisplayName(SIGDN.SIGDN_DESKTOPABSOLUTEEDITING, out var name), throwOnError);
            if (path != null || name != null)
            {
                _resultPaths.Add(path);
                _resultNames.Add(name);
            }
        }
        return true;
    }

    private static int CheckHr(int hr, bool throwOnError)
    {
        if (hr != 0 && throwOnError) Marshal.ThrowExceptionForHR(hr);
        return hr;
    }

    [DllImport("shell32")]
    private static extern int SHCreateItemFromParsingName([MarshalAs(UnmanagedType.LPWStr)] string pszPath, IBindCtx pbc, [MarshalAs(UnmanagedType.LPStruct)] Guid riid, out IShellItem ppv);

    [DllImport("user32")]
    private static extern IntPtr GetDesktopWindow();

#pragma warning disable IDE1006 // Naming Styles
    private const int ERROR_CANCELLED = unchecked((int)0x800704C7);
#pragma warning restore IDE1006 // Naming Styles

    [ComImport, Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")] // CLSID_FileOpenDialog
    private class FileOpenDialog { }

    [ComImport, Guid("d57c7288-d4ad-4768-be02-9d969532d960"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IFileOpenDialog
    {
        [PreserveSig] int Show(IntPtr parent); // IModalWindow
        [PreserveSig] int SetFileTypes();  // not fully defined
        [PreserveSig] int SetFileTypeIndex(int iFileType);
        [PreserveSig] int GetFileTypeIndex(out int piFileType);
        [PreserveSig] int Advise(); // not fully defined
        [PreserveSig] int Unadvise();
        [PreserveSig] int SetOptions(FOS fos);
        [PreserveSig] int GetOptions(out FOS pfos);
        [PreserveSig] int SetDefaultFolder(IShellItem psi);
        [PreserveSig] int SetFolder(IShellItem psi);
        [PreserveSig] int GetFolder(out IShellItem ppsi);
        [PreserveSig] int GetCurrentSelection(out IShellItem ppsi);
        [PreserveSig] int SetFileName([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        [PreserveSig] int GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
        [PreserveSig] int SetTitle([MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
        [PreserveSig] int SetOkButtonLabel([MarshalAs(UnmanagedType.LPWStr)] string pszText);
        [PreserveSig] int SetFileNameLabel([MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
        [PreserveSig] int GetResult(out IShellItem ppsi);
        [PreserveSig] int AddPlace(IShellItem psi, int alignment);
        [PreserveSig] int SetDefaultExtension([MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
        [PreserveSig] int Close(int hr);
        [PreserveSig] int SetClientGuid();  // not fully defined
        [PreserveSig] int ClearClientData();
        [PreserveSig] int SetFilter([MarshalAs(UnmanagedType.IUnknown)] object pFilter);
        [PreserveSig] int GetResults(out IShellItemArray ppenum);
        [PreserveSig] int GetSelectedItems([MarshalAs(UnmanagedType.IUnknown)] out object ppsai);
    }

    [ComImport, Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItem
    {
        [PreserveSig] int BindToHandler(); // not fully defined
        [PreserveSig] int GetParent(); // not fully defined
        [PreserveSig] int GetDisplayName(SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
        [PreserveSig] int GetAttributes();  // not fully defined
        [PreserveSig] int Compare();  // not fully defined
    }

    [ComImport, Guid("b63ea76d-1f85-456f-a19c-48159efa858b"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IShellItemArray
    {
        [PreserveSig] int BindToHandler();  // not fully defined
        [PreserveSig] int GetPropertyStore();  // not fully defined
        [PreserveSig] int GetPropertyDescriptionList();  // not fully defined
        [PreserveSig] int GetAttributes();  // not fully defined
        [PreserveSig] int GetCount(out int pdwNumItems);
        [PreserveSig] int GetItemAt(int dwIndex, out IShellItem ppsi);
        [PreserveSig] int EnumItems();  // not fully defined
    }

#pragma warning disable CA1712 // Do not prefix enum values with type name
    private enum SIGDN : uint
    {
        SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
        SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
        SIGDN_FILESYSPATH = 0x80058000,
        SIGDN_NORMALDISPLAY = 0,
        SIGDN_PARENTRELATIVE = 0x80080001,
        SIGDN_PARENTRELATIVEEDITING = 0x80031001,
        SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
        SIGDN_PARENTRELATIVEPARSING = 0x80018001,
        SIGDN_URL = 0x80068000
    }

    [Flags]
    private enum FOS
    {
        FOS_OVERWRITEPROMPT = 0x2,
        FOS_STRICTFILETYPES = 0x4,
        FOS_NOCHANGEDIR = 0x8,
        FOS_PICKFOLDERS = 0x20,
        FOS_FORCEFILESYSTEM = 0x40,
        FOS_ALLNONSTORAGEITEMS = 0x80,
        FOS_NOVALIDATE = 0x100,
        FOS_ALLOWMULTISELECT = 0x200,
        FOS_PATHMUSTEXIST = 0x800,
        FOS_FILEMUSTEXIST = 0x1000,
        FOS_CREATEPROMPT = 0x2000,
        FOS_SHAREAWARE = 0x4000,
        FOS_NOREADONLYRETURN = 0x8000,
        FOS_NOTESTFILECREATE = 0x10000,
        FOS_HIDEMRUPLACES = 0x20000,
        FOS_HIDEPINNEDPLACES = 0x40000,
        FOS_NODEREFERENCELINKS = 0x100000,
        FOS_OKBUTTONNEEDSINTERACTION = 0x200000,
        FOS_DONTADDTORECENT = 0x2000000,
        FOS_FORCESHOWHIDDEN = 0x10000000,
        FOS_DEFAULTNOMINIMODE = 0x20000000,
        FOS_FORCEPREVIEWPANEON = 0x40000000,
        FOS_SUPPORTSTREAMABLEITEMS = unchecked((int)0x80000000)
    }
#pragma warning restore CA1712 // Do not prefix enum values with type name
}
}
