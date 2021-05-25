using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace NETCoreBase.Common.Helpers
{
    public static class FileHelper
    {
        public static void CopyFile(string fromPath, string toPath, bool isDeleteSourceFile, bool isOverWriteFile)
        {
            bool flag = false;
            try
            {
                File.Copy(fromPath, toPath, isOverWriteFile);
                flag = true;

                if (flag & isDeleteSourceFile)
                {
                    File.Delete(fromPath);
                }
            }
            catch (DirectoryNotFoundException directoryNotFoundException)
            {
                Console.WriteLine(directoryNotFoundException.Message);
            }
            catch (IOException oException)
            {
                Console.WriteLine(oException.Message);
            }
        }

        public static string DeleteDirectory(string directoryPath, bool isDeleteAllFile)
        {
            string message;
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, isDeleteAllFile);
                }
                message = "";
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Console.WriteLine(exception.Message);
                message = exception.Message;
            }
            return message;
        }

        public static void DeleteFile(string filePath)
        {
            File.Delete(filePath);
        }

        public static string IsDirectoryExists(string directoryPath, bool isCreateDirectory)
        {
            string message = "";
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    if (isCreateDirectory)
                    {
                        Directory.CreateDirectory(directoryPath);
                    }
                }
            }
            catch (Exception exception1)
            {
                Exception exception = exception1;
                Console.WriteLine(exception.Message);
                message = exception.Message;
            }
            return message;
        }

        public static bool IsFileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static void ZipExtractToDirectory(string zipPath, string toPath)
        {
            ZipFile.ExtractToDirectory(zipPath, toPath);
        }

        public static void ZipExtractToDirectoryByGz(string zipPath)
        {
            FileInfo fileInfo = new FileInfo(zipPath);
            using (FileStream fileStream = fileInfo.OpenRead())
            {
                string fullName = fileInfo.FullName;
                string str = fullName.Remove(fullName.Length - fileInfo.Extension.Length);
                using (FileStream fileStream1 = File.Create(str))
                {
                    using (GZipStream gZipStream = new GZipStream(fileStream, CompressionMode.Decompress))
                    {
                        gZipStream.CopyTo(fileStream1);
                        Console.WriteLine(string.Concat("Decompressed: ", fileInfo.Name));
                    }
                }
            }
        }
    }
}