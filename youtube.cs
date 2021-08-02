using System;
using YoutubeExplode;
using System.IO;
using System.Collections.Generic;
using YoutubeExplode.Videos.Streams;
using System.Threading.Tasks;

// ne kadar gereksiz uzun değil mi, en azından c# hızlı o yüzden şikayetçi değilim

namespace YTmp3
{
    class Program
    {
        public static List<string> LinkList = new List<string>();
        public static List<string> ffmpegNameList = new List<string>();

        async static Task download(string link)
        {
            YoutubeClient youtube = new YoutubeClient();
            var video = await youtube.Videos.GetAsync(link);
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(link);
            var streamf = streamManifest.GetAudioOnlyStreams().TryGetWithHighestBitrate();
            if (streamf == null)
            {
                Console.WriteLine($"{video.Title} için indirme bulunamadı");
            }
            else
            {
                await youtube.Videos.Streams.DownloadAsync(streamf, @"VideoCache\"+$"{video.Title}.{streamf.Container}");
                ffmpegNameList.Add(@"VideoCache\" + $"{video.Title}.{streamf.Container}");
            }
        }

        public static int ReadFile()
        {
            string line;
            int count = 0;
            StreamReader file = new StreamReader("Linkler.txt");
            while ((line = file.ReadLine()) != null)
            {
                LinkList.Add(line);
                count++;
            }
            file.Close();
            return count;
        }

        public static void downloadAll()
        {
            int count = 0;
            foreach(string link in LinkList)
            {
                download(link).Wait();
                Console.WriteLine($"{count}.müzik indiriliyor...");
                count++;
            }
        }

        public static void convertFFmpeg()
        {
            foreach(string vidName in ffmpegNameList)
            {
                string newext = vidName.Replace(".webm", ".mp3");
                string newerText = newext.Replace(@"VideoCache\", @"FinalMuzikler\");
                char qute = (char)34;
                string currentFolder = System.AppDomain.CurrentDomain.BaseDirectory;
                var prcs = System.Diagnostics.Process.Start(currentFolder+@"\ffmpeg.exe", $"-i {qute}{vidName}{qute} {qute}{newerText}{qute}");
                prcs.WaitForExit();
            }
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Youtubeden MP3 indiriciye hos geldiniz!");
            int lc = ReadFile();
            Console.WriteLine($"Dosya okundu... Toplamda {lc} müzik var");
            downloadAll();
            Console.WriteLine("Müzikler indirildi ;)");
            Console.WriteLine("Müzikler .mp3 formatına çevriliyor...");
            convertFFmpeg();
            Console.WriteLine("Basarili");
        }
    }
}
