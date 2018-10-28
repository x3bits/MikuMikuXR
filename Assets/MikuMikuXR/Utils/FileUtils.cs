using System.IO;
using System.Text;
using SharpCompress.Common;
using SharpCompress.Reader;

namespace MikuMikuXR.Utils
{
    public static class FileUtils
    {
        public static void WriteTextFile(string text, string path, Encoding encoding)
        {
            var parent = Directory.GetParent(path);
            Directory.CreateDirectory(parent.FullName);
            File.WriteAllText(path, text, encoding);
        }
        
        public static void ExtractZipBytesToFolder(string path, byte[] bytes, string tempPath) {
            File.WriteAllBytes (tempPath, bytes);
            ExtractFileToFolder (tempPath, path);
        }
		
        public static void ExtractFileToFolder(string filePath, string dstPath) {
            using (Stream stream = File.OpenRead(filePath))
            {
                var reader = ReaderFactory.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        reader.WriteEntryToDirectory(dstPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }
    }
}