using System.Collections.Generic;
using System.IO;
namespace ConsoleIPTVEditor
{
    class M3U
    {
        public List<M3U8> Channels { get; set; }
        public M3U(List<M3U8> channels = null)
        {
            Channels = channels ?? new List<M3U8>();
        }
        public static M3U Parser(string path)
        {
            string[] data;
            using (StreamReader r = new StreamReader(path))
            {
                data = r.ReadToEnd().Split('\n');
            }
            M3U m3U = new M3U();
            M3U8 m3u8 = new M3U8();
            foreach (var item in data)
            {
                if (item.StartsWith('#'))
                {
                    m3u8.Params.Add(item);
                }
                else if (!string.IsNullOrEmpty(item))
                {
                    m3u8.M3U8Url = item;
                    m3U.Channels.Add(m3u8);
                    m3u8 = new M3U8();
                }
            }
            return m3U;
        }
    }
}
