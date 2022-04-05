using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ZappyLogger.ColumnizaeLib;
using ZappyLogger.Controls.LogWindow;

namespace ZappyLogger.Classes.Filter
{
    public class FilterPipe
    {
        #region Fields

        private static readonly ILogger _logger = LogManager.GetCurrentClassLogger();

        private IList<int> lineMappingList = new List<int>();
        private StreamWriter writer;

        #endregion

        #region cTor

        public FilterPipe(FilterParams filterParams, LogWindow logWindow)
        {
            this.FilterParams = filterParams;
            this.LogWindow = logWindow;
            this.IsStopped = false;
            this.FileName = Path.GetTempFileName();

            _logger.Info("Created temp file: {0}", this.FileName);
        }

        #endregion

        #region Delegates

        public delegate void ClosedEventHandler(object sender, EventArgs e);

        #endregion

        #region Events

        public event ClosedEventHandler Closed;

        #endregion

        #region Properties

        public bool IsStopped { get; set; }

        public string FileName { get; }

        public FilterParams FilterParams { get; }

        public IList<int> LastLinesHistoryList { get; } = new List<int>();

        public LogWindow LogWindow { get; }

        public LogWindow OwnLogWindow { get; set; }

        #endregion

        #region Public methods

        public void OpenFile()
        {
            FileStream fStream = new FileStream(this.FileName, FileMode.Append, FileAccess.Write, FileShare.Read);
            this.writer = new StreamWriter(fStream, new UnicodeEncoding(false, false));
        }

        public void CloseFile()
        {
            if (this.writer != null)
            {
                this.writer.Close();
                this.writer = null;
            }
        }

        public bool WriteToPipe(ILogLine textLine, int orgLineNum)
        {
            try
            {
                lock (this.FileName)
                {
                    lock (this.lineMappingList)
                    {
                        try
                        {
                            this.writer.WriteLine(textLine.FullLine);
                            this.lineMappingList.Add(orgLineNum);
                            return true;
                        }
                        catch (IOException e)
                        {
                            _logger.Error(e, "writeToPipe()");
                            return false;
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                _logger.Error(ex, "writeToPipe(): file was closed");
                return false;
            }
        }

        public int GetOriginalLineNum(int lineNum)
        {
            lock (this.lineMappingList)
            {
                if (this.lineMappingList.Count > lineNum)
                {
                    return this.lineMappingList[lineNum];
                }
                else
                {
                    return -1;
                }
            }
        }

        public void ShiftLineNums(int offset)
        {
            _logger.Debug("FilterPipe.ShiftLineNums() offset={0}", offset);
            List<int> newList = new List<int>();
            lock (this.lineMappingList)
            {
                foreach (int lineNum in this.lineMappingList)
                {
                    int line = lineNum - offset;
                    if (line >= 0)
                    {
                        newList.Add(line);
                    }
                    else
                    {
                        newList.Add(-1);
                    }
                }
                this.lineMappingList = newList;
            }
        }

        public void ClearLineNums()
        {
            _logger.Debug("FilterPipe.ClearLineNums()");
            lock (this.lineMappingList)
            {
                for (int i = 0; i < this.lineMappingList.Count; ++i)
                {
                    this.lineMappingList[i] = -1;
                }
            }
        }

        public void ClearLineList()
        {
            lock (this.lineMappingList)
            {
                this.lineMappingList.Clear();
            }
        }

        public void RecreateTempFile()
        {
            lock (this.lineMappingList)
            {
                this.lineMappingList = new List<int>();
            }
            lock (this.FileName)
            {
                CloseFile();
                // trunc file
                FileStream fStream = new FileStream(this.FileName, FileMode.Truncate, FileAccess.Write, FileShare.Read);
                fStream.SetLength(0);
                fStream.Close();
            }
        }

        public void CloseAndDisconnect()
        {
            ClearLineList();
            OnClosed();
        }

        #endregion

        #region Private Methods

        private void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, new EventArgs());
            }
        }

        #endregion
    }
}