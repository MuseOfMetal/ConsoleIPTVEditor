using System.Collections.Generic;
namespace ConsoleIPTVEditor
{
    class M3U8
    {
        public List<string> Params { get; set; }
        public string M3U8Url { get; set; }
        public M3U8()
        {
            Params = new List<string>();
        }
    }
}
