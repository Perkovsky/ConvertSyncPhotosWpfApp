using System.Xml.Serialization;
using System.IO;

namespace ConvertSyncPhotosWpfApp
{
    /// <summary>
    /// Class that contains settings
    /// </summary>
    public class SettingsFields
    {
        // initially, a non-existent directory is specified so as not to start the watcher until the real directory is specified
        public string WatcherDirectory = @"c:\276E9339-502A-441E-8784-9CACD6CD5145\";
        public string ConvertDirectory = @"c:\CFCF4FD5-F252-4E3F-AEAB-62D785D4357A\";
        public bool Logger = true;
    }

    /// <summary>
    /// The class is designed to read service settings. Only use in manual write setting into XML.
    /// </summary>
    public class Settings
    {
        private readonly string FILE_NAME = string.Format(@"{0}\settings.xml", FileConverting.GetCurrentDirectory());
        private SettingsFields fields;
        public SettingsFields Fields { get { return fields; } }

        public Settings()
        {
            fields = new SettingsFields();
            ReadXml();
        }

        // only uses if no XML
        private void WriteXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SettingsFields));
            TextWriter writer = new StreamWriter(FILE_NAME);
            serializer.Serialize(writer, fields);
            writer.Close();
        }

        public void ReadXml()
        {
            if (File.Exists(FILE_NAME))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SettingsFields));
                TextReader reader = new StreamReader(FILE_NAME);
                fields = serializer.Deserialize(reader) as SettingsFields;
                reader.Close();
            }
            else
            {
                WriteXml();
            }
        }
    }
}
