using System;
using System.IO;
using System.Linq;
namespace ConsoleIPTVEditor
{
    class Program
    {
        private static string[] _settings;
        static void Main(string[] args)
        {
            GetSettings();
            while (EditFile(GetFileFromUser())) ;
        }
        private static bool EditFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
            var data = M3U.Parser(path);
            while (true)
            {
                Console.Clear();
                Console.WriteLine(
                    "Choose action\n\n" +
                    "[1] Add channel\n" +
                    "[2] Remove channel\n" +
                    "[3] Edit Channel\n" +
                    "[4] Swap channels\n" +
                    "[5] Get channel from other file\n\n" +
                    "[B] Back to choose file");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        data = AddChanel(data);
                        break;
                    case ConsoleKey.D2:
                        data = RemoveChannel(data);
                        break;
                    case ConsoleKey.D3:
                        data = EditChannel(data);
                        break;
                    case ConsoleKey.D4:
                        data = SwapChannels(data);
                        break;
                    case ConsoleKey.D5:
                        data = GetChannelFromOtherFile(data);
                        break;
                    case ConsoleKey.B:
                        return true;
                    case ConsoleKey.C:
                        return true;
                }
                Save(path, data);
            }
        }
        private static M3U AddChanel(M3U m3U)
        {
            M3U8 m3U8 = new M3U8();
            string temp;
            do
            {
                Console.Clear();
                Console.Write("Please enter a channel`s name: ");
                temp = Console.ReadLine();
            } while (string.IsNullOrEmpty(temp));
            m3U8.Params.Add("#EXTINF:-1 ," + temp);
            temp = null;
            do
            {
                Console.Clear();
                Console.WriteLine("Enter url m3u8: ");
                temp = Console.ReadLine();
            } while (string.IsNullOrEmpty(temp));
            m3U8.M3U8Url = temp;
            var newM3U = m3U;
            newM3U.Channels.Add(m3U8);
            return newM3U;
        }
        private static M3U RemoveChannel(M3U m3U)
        {
            int result = ChooseChannel(m3U, "delete");
            if (result > -1)
                m3U.Channels.RemoveAt(result);
            return m3U;
        }
        private static M3U EditChannel(M3U m3U)
        {
            int result = ChooseChannel(m3U, "edit");
            if (result < 0)
                return m3U;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("What need to edit?\n\n[1] Name\n[2] Url\n\n[B] Back");
                string newName;
                string newUrl;
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        do
                        {
                            Console.Clear();
                            Console.Write("Enter new name: ");
                            newName = Console.ReadLine();
                        } while (string.IsNullOrEmpty(newName));
                        m3U.Channels[result].Params[m3U.Channels[result].Params.FindIndex(0, m3U.Channels[result].Params.Count, a => a.Contains("#EXTINF:-1"))] = $"#EXTINF:-1 ,{newName}";
                        return m3U;
                    case ConsoleKey.D2:
                        do
                        {
                            Console.Clear();
                            Console.Write("Enter new name: ");
                            newUrl = Console.ReadLine();
                        } while (string.IsNullOrEmpty(newUrl));
                        m3U.Channels[result].M3U8Url = newUrl;
                        return m3U;
                    case ConsoleKey.B:
                        return m3U;
                }
            }
        }
        private static M3U SwapChannels(M3U m3U)
        {
            int result1 = ChooseChannel(m3U, "swap channels (First)");
            if (result1 < 0)
                return m3U;
            int result2 = ChooseChannel(m3U, "swap channels (Second)");
            if (result2 < 0)
                return m3U;
            var temp = m3U.Channels[result1];
            m3U.Channels[result1] = m3U.Channels[result2];
            m3U.Channels[result2] = temp;
            return m3U;
        }
        private static M3U GetChannelFromOtherFile(M3U m3U)
        {
            var file = GetFileFromUser();
            if (string.IsNullOrEmpty(file))
                return m3U;
            M3U m3u = M3U.Parser(file);
            m3U.Channels.Add(m3u.Channels[ChooseChannel(m3u, "copy in main file")]);
            return m3U;
        }
        private static int ChooseChannel(M3U m3U, string textmsg)
        {
            int pages = m3U.Channels.Count / 9;
            int currentPage = 0;
            int choice = -1;
            while (choice == -1)
            {
                Console.Clear();
                Console.WriteLine($"Choose channel to {textmsg}\n[Page {currentPage + 1}/{pages + 1}]\n");
                for (int i = (currentPage) * 9, j = 1; i < m3U.Channels.Count && i < 9 * (currentPage + 1); i++, j++)
                {
                    Console.WriteLine(@$"[{j}] [{m3U.Channels[i].Params.
                        Where(a => a.Contains("#EXTINF:-1")).
                        ToArray()[0].Split(',')[1].Replace("\r", "")}]");
                }
                Console.WriteLine();
                if (pages != currentPage && currentPage * 9 != m3U.Channels.Count)
                    Console.WriteLine("[N] Next");
                if (currentPage != 0 && pages > 0)
                    Console.WriteLine("[P] Previus");
                Console.WriteLine("[C] Close");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        if (0 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 0 + (currentPage * 9);
                        break;
                    case ConsoleKey.D2:
                        if (1 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 1 + (currentPage * 9);
                        break;
                    case ConsoleKey.D3:
                        if (2 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 2 + (currentPage * 9);
                        break;
                    case ConsoleKey.D4:
                        if (3 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 3 + (currentPage * 9);
                        break;
                    case ConsoleKey.D5:
                        if (4 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 4 + (currentPage * 9);
                        break;
                    case ConsoleKey.D6:
                        if (5 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 5 + (currentPage * 9);
                        break;
                    case ConsoleKey.D7:
                        if (6 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 6 + (currentPage * 9);
                        break;
                    case ConsoleKey.D8:
                        if (7 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 7 + (currentPage * 9);
                        break;
                    case ConsoleKey.D9:
                        if (8 + (currentPage * 9) < m3U.Channels.Count)
                            choice = 8 + (currentPage * 9);
                        break;
                    case ConsoleKey.N:
                        if (currentPage < pages)
                            currentPage += 1;
                        break;
                    case ConsoleKey.P:
                        if (currentPage != 0)
                            currentPage -= 1;
                        break;
                    case ConsoleKey.C:
                        return -1;
                }
            }
            return choice;
        }
        private static string GetFileFromUser()
        {
            string[] filesInPath = Directory.GetFiles(_settings[0], "*m3u");
            int pages = filesInPath.Length / 9;
            int currentPage = 0;
            int choice = -1;
            while (choice == -1)
            {
                Console.Clear();
                Console.WriteLine($"Choose file\n[Page {currentPage + 1}/{pages + 1}]\n");

                for (int i = (currentPage) * 9, j = 1; i < filesInPath.Length && i < 9 * (currentPage + 1); i++, j++)
                {
                    Console.WriteLine($"[{j}] [{Path.GetFileName(filesInPath[i])}]");
                }
                Console.WriteLine();
                if (pages != currentPage && currentPage * 9 != filesInPath.Length)
                    Console.WriteLine("[N] Next");
                if (currentPage != 0 && pages > 0)
                    Console.WriteLine("[P] Previus");
                    Console.WriteLine("[C] Close");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D1:
                        if (0 + (currentPage * 9) < filesInPath.Length)
                            choice = 0 + (currentPage * 9);
                        break;
                    case ConsoleKey.D2:
                        if (1 + (currentPage * 9) < filesInPath.Length)
                            choice = 1 + (currentPage * 9);
                        break;
                    case ConsoleKey.D3:
                        if (2 + (currentPage * 9) < filesInPath.Length)
                            choice = 2 + (currentPage * 9);
                        break;
                    case ConsoleKey.D4:
                        if (3 + (currentPage * 9) < filesInPath.Length)
                            choice = 3 + (currentPage * 9);
                        break;
                    case ConsoleKey.D5:
                        if (4 + (currentPage * 9) < filesInPath.Length)
                            choice = 4 + (currentPage * 9);
                        break;
                    case ConsoleKey.D6:
                        if (5 + (currentPage * 9) < filesInPath.Length)
                            choice = 5 + (currentPage * 9);
                        break;
                    case ConsoleKey.D7:
                        if (6 + (currentPage * 9) < filesInPath.Length)
                            choice = 6 + (currentPage * 9);
                        break;
                    case ConsoleKey.D8:
                        if (7 + (currentPage * 9) < filesInPath.Length)
                            choice = 7 + (currentPage * 9);
                        break;
                    case ConsoleKey.D9:
                        if (8 + (currentPage * 9) < filesInPath.Length)
                            choice = 8 + (currentPage * 9);
                        break;
                    case ConsoleKey.N:
                        if (currentPage < pages)
                            currentPage += 1;
                        break;
                    case ConsoleKey.P:
                        if (currentPage != 0)
                            currentPage -= 1;
                        break;
                    case ConsoleKey.C:
                        return "";
                }
            }
            Console.Clear();
            return filesInPath[choice];
        }
        private static void Save(string path, M3U m3U)
        {
            string text = null;
            foreach (var Channels in m3U.Channels)
            {
                foreach (var Params in Channels.Params)
                {
                    text += Params + "\n";
                }
                text += Channels.M3U8Url + "\n";
            }

            using (StreamWriter w = new StreamWriter(path))
            {
                w.Write(text);
            }
        }
        static void GetSettings()
        {
            using (StreamReader r = new StreamReader("settings.cfg"))
            {
                _settings = r.ReadToEnd().Split('\n');
            }
        }
    }
}
