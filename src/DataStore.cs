using System;
using System.IO;
using System.Xml.Serialization;

namespace PersonalTodo
{
    public static class DataStore
    {
        private static readonly string DataDirectory =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "PersonalTodo");

        private static readonly string DataFile = Path.Combine(DataDirectory, "data.xml");

        public static AppData Load()
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            if (!File.Exists(DataFile))
                return new AppData();

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(AppData));
                using (FileStream stream = new FileStream(DataFile, FileMode.Open, FileAccess.Read))
                {
                    AppData data = (AppData)serializer.Deserialize(stream);
                    if (data == null)
                        return new AppData();
                    if (data.Items == null)
                        data.Items = new System.Collections.Generic.List<WorkItem>();
                    if (data.Folders == null)
                        data.Folders = new System.Collections.Generic.List<FolderBookmark>();
                    return data;
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    "XMLデータを読み込めませんでした。\r\n" + DataFile + "\r\n\r\n" + ex.Message, ex);
            }
        }

        public static void Save(AppData data)
        {
            if (!Directory.Exists(DataDirectory))
                Directory.CreateDirectory(DataDirectory);

            string tempFile = DataFile + ".tmp";
            XmlSerializer serializer = new XmlSerializer(typeof(AppData));

            using (FileStream stream = new FileStream(tempFile, FileMode.Create, FileAccess.Write))
            {
                serializer.Serialize(stream, data);
            }

            if (File.Exists(DataFile))
                File.Delete(DataFile);

            File.Move(tempFile, DataFile);
        }

        public static string GetDataFilePath()
        {
            return DataFile;
        }
    }
}