using System;
using System.IO;
using System.Linq;

namespace Donkey_Kong
{
    static class FileReader
    {
        /// <summary>
        /// Quickly finds all instances of specified information inside a text document
        /// </summary>
        public static string[] FindInfo(string aPath, string aName, char aSeperator)
        {
            if (File.Exists(aPath))
            {
                if (new FileInfo(aPath).Length > 0)
                {
                    string[] tempFoundInfo;
                    int tempInfoSize = 0;

                    string tempReadFile = File.ReadAllText(aPath);
                    string[] tempSplitText = tempReadFile.Split(aSeperator);
                    string[] tempFoundValues = new string[tempSplitText.Length];

                    for (int i = 0; i < tempSplitText.Length; i++)
                    {
                        if (tempSplitText[i] == aName)
                        {
                            tempInfoSize++;
                            tempFoundValues[i] = tempSplitText[i + 1];
                        }
                    }
                    tempFoundInfo = new string[tempInfoSize];
                    for (int i = 0; i < tempFoundValues.Length; i++)
                    {
                        if (tempFoundValues[i] != null)
                        {
                            tempFoundInfo[i] = tempFoundValues[i];
                        }
                    }
                    return tempFoundInfo;
                }
            }
            return new string[0];
        }
    }
}
