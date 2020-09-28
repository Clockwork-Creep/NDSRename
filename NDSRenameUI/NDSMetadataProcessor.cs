using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace NDSRenameUI
{
    static class MetadataHeaderConstants
    {
        public const int HeaderLength = 512;
        public const int PartialStringLength = 12;
        public const int GameIdLength = 4;
    }

    class NDSMetaData
    {
        public string PartialName { set; get; }
        public string CurrentFileName{ set; get; }
        public string NewFileName{ set; get; }
        public string FileName { set; get; }
        public string GameTDBMapping { set; get; }
        public string GameId{ set; get; }
    }

    class NDSMetadataProcessor
    {
        private Dictionary<string, string> GamesTDB { set; get; }
        private NDSMetaData MetaData { set; get; }

        public NDSMetadataProcessor()
        {
            this.GamesTDB = new Dictionary<string, string>();
            this.MetaData = new NDSMetaData();
            this.LoadDatabase();
        }
       private bool LoadDatabase()
        {
            var tabDelimitedDb = Properties.Resources.GamesTDB;
            if (string.IsNullOrEmpty(tabDelimitedDb)) return false;
            string[] lines = tabDelimitedDb.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            foreach (var line in lines)
            {
                var values = line.Split('\t');
                if (values.Length != 2)
                {
                    return false;
                }
                if (this.GamesTDB.ContainsKey(values[0]) == false)
                {
                    this.GamesTDB.Add(values[0], values[1]);
                }
            }
            return true;
        }

        private string ReadPartialTitleName(int[] metadataBuffer)
        {
            string partialTitle = "";
            for (var i = 0; i < MetadataHeaderConstants.PartialStringLength; i++)
            {
                if (metadataBuffer[i] == 0)
                {
                    break;
                }
                partialTitle += (char)metadataBuffer[i];
            }
            return partialTitle;
        }

        private string ReadId(int[] metadataBuffer)
        {
            string id = "";
            var end = MetadataHeaderConstants.PartialStringLength + MetadataHeaderConstants.GameIdLength;
            for (var i = MetadataHeaderConstants.PartialStringLength; i < end; i++)
            {
                id += (char)metadataBuffer[i];
            }
            return id;
        }

        public NDSMetaData ProcessNDSFile(OpenFileDialog openFileDlg)
        {
            if (string.IsNullOrEmpty(openFileDlg.FileName))
            {
                throw new NDSFileException("File Path Missing");
            }

            if (Path.GetExtension(openFileDlg.FileName) != ".nds")
            {
                throw new NDSFileException("File must have nds extension");
            }
            var metadataBuffer = new int[MetadataHeaderConstants.HeaderLength];
            this.MetaData.CurrentFileName = Path.GetFileNameWithoutExtension(openFileDlg.FileName);
            this.MetaData.NewFileName = Path.GetFileNameWithoutExtension(openFileDlg.FileName);

            this.MetaData.FileName = openFileDlg.FileName;
            try
            {
                using (FileStream fileStream = File.OpenRead(openFileDlg.FileName))
                {
                    fileStream.Seek(0, SeekOrigin.Begin);
                    var endReached = false;
                    for (int i = 0; i < MetadataHeaderConstants.HeaderLength && !endReached; i++)
                    {
                        var nextByte = fileStream.ReadByte();
                        if (nextByte == -1)
                        {
                            endReached = true;
                        }
                        else
                        {
                            metadataBuffer[i] = nextByte;
                        }
                    }
                    if (endReached)
                    {
                        throw new NDSFileStreamException("Failed to read header");
                    }
                    this.MetaData.PartialName = this.ReadPartialTitleName(metadataBuffer);
                    if (string.IsNullOrEmpty(this.MetaData.PartialName) == false)
                    {
                        var id = this.ReadId(metadataBuffer);
                        this.MetaData.GameId = id;
                        string gameName = "";
                        if (string.IsNullOrEmpty(id) == false)
                        {
                            this.GamesTDB.TryGetValue(id, out gameName);
                        }
                        if (string.IsNullOrEmpty(gameName))
                        {
                            this.MetaData.GameTDBMapping = "<< No Mapping >>";
                        }
                        else
                        {
                            this.MetaData.GameTDBMapping = gameName;
                            this.MetaData.NewFileName = gameName;
                        }
                    }
                }
                return this.MetaData;
            }
            catch (ArgumentException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (UnauthorizedAccessException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (PathTooLongException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (DirectoryNotFoundException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (FileNotFoundException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (NotSupportedException e)
            {
                throw new NDSFileStreamException(e.Message);
            }
            catch (NDSFileStreamException e)
            {
                throw e;
            }
            throw new NDSFileStreamException("Unhandled Error");
        }

#if DEBUG
        private void DumpMetadataHeaderToString(OpenFileDialog openFileDlg)
        {
            var test = File.ReadAllBytes(openFileDlg.FileName);
            string dmp = "";
            for (var i = 0; i < MetadataHeaderConstants.HeaderLength; i++)
            {
                dmp += (char)test[i];
            }
        }
#endif
    }
}
