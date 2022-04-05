using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;

namespace Zappy.Graph
{
    public static class CrapyWriter
    {
        public static Dictionary<string, string> _ProcessInvocationMap;
        static string regexSearch;
        public static Regex r;

        static CrapyWriter()
        {
            _ProcessInvocationMap = new Dictionary<string, string>();
            if (File.Exists(CrapyConstants.CrappyIndex))
            {
                string[] _Lines = File.ReadAllLines(CrapyConstants.CrappyIndex);
                for (int i = 0; i < _Lines.Length; i++)
                {
                    try
                    {
                        string[] _DictInfo = _Lines[i].Split('^');
                        if (_DictInfo.Length > 1)
                            _ProcessInvocationMap[_DictInfo[0]] = _DictInfo[1];
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }

            regexSearch = new string(Path.GetInvalidFileNameChars()) +
                                 new string(Path.GetInvalidPathChars());
            r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));

        }




                                
                                
                        
                                                                                                                                                                                        
        
        public static void SaveWindowInformationRecordedTask(string _DirName, string fileName, string _ProcessFileName, string MainWindowName,
            ZappyTask _thisTask)
        {
            try
            {

                using (StreamWriter file =
                    new StreamWriter(CrapyConstants.CrappyIndex, true))
                {
                                        if (!_ProcessFileName.Equals("Zappy"))
                    {
                        file.WriteLine(_ProcessFileName + MainWindowName + "^" + _DirName);
                        _ProcessInvocationMap[_ProcessFileName + MainWindowName] = _DirName;
                    }
                }

                SaveToFile(_thisTask, fileName);
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(_DirName, ex);
            }

        }

        public static string Save(ZappyTask Task, string DirName, string FileName)
        {
            try
            {
                if (Task.ExecuteActivities.Count > 0)
                {
                    if (!Directory.Exists(DirName))
                        Directory.CreateDirectory(DirName);
                    string name = Path.Combine(DirName, FileName);
                    return SaveToFile(Task, name);
                }
            }
            catch (Exception e)
            {
                CrapyLogger.log.Error(e);
            }

            return "";
        }
                                                                                                                                                                                                                                                                                                
                
                                
                
        public static string SaveToFile(ZappyTask Task, string FileName)
        {
            try
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    Task.Save(FileName);
                    return FileName;
                }
            }
            catch (Exception e)
            {
                CrapyLogger.log.Error("Saving to File Failed", e);
            }

            return "";
        }

        public static void Close()
        {

        }

    }
}

