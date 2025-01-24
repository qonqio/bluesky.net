using System.Net.Http.Headers;

namespace Qonq.BlueSky.Helper
{
    internal class BlobConverter
    {

        public byte[] ConvertDataURIToByteArray(string dataUri)
        {
            var base64Data = dataUri;
            if (dataUri.StartsWith("data:image/png;base64,"))
            {
                base64Data = dataUri.Split(',')[1];
            }
            return Convert.FromBase64String(base64Data);
        }
    }
}
