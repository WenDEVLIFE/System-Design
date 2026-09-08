using System;
using System.IO;

namespace System_Design
{
    internal static class RememberMe
    {
        private static readonly string StorageDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "StudentManagement");

        private static readonly string StorageFile = Path.Combine(StorageDirectory, "remember.dat");

        public static void Save(string username)
        {
            try
            {
                Directory.CreateDirectory(StorageDirectory);
                File.WriteAllText(StorageFile, username ?? string.Empty);
            }
            catch (Exception)
            {
                // Best-effort persistence: a settings file must never break the login flow.
            }
        }

        public static string Load()
        {
            try
            {
                if (!File.Exists(StorageFile))
                {
                    return null;
                }

                string value = File.ReadAllText(StorageFile);
                return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Clear()
        {
            try
            {
                if (File.Exists(StorageFile))
                {
                    File.Delete(StorageFile);
                }
            }
            catch (Exception)
            {
            }
        }
    }
}