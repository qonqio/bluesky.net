using System.Text;

namespace Qonq.BlueSky.Helper.Strings
{
    public class UnicodeString
    {
        public string Utf16 { get; set; }
        public byte[] Utf8 { get; set; }

        public UnicodeString(string utf16)
        {
            Utf16 = utf16;
            Utf8 = Encoding.UTF8.GetBytes(utf16);
        }

        // Helper to convert UTF-16 code-unit offsets to UTF-8 code-unit offsets
        public int Utf16IndexToUtf8Index(int i)
        {
            return Encoding.UTF8.GetByteCount(Utf16.Substring(0, i));
        }
    }
}
