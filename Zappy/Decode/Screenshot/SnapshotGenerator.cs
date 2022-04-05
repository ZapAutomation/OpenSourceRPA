using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;

namespace Zappy.Decode.Screenshot
{
    internal class SnapshotGenerator
    {
        private static SnapshotGenerator instance;
        private ZappyTaskImageInfo lastImageInfo;
        private static readonly object lockObject = new object();
        private Task snapShotTask;
        private CancellationTokenSource sourceToken;
        private CancellationToken token;

        private SnapshotGenerator()
        {
            SnapShotProvider = new DefaultSnapShotProvider();
        }

        private ZappyTaskImageInfo GetLastAsyncTaskOutput()
        {
            object lockObject = SnapshotGenerator.lockObject;
            lock (lockObject)
            {
                if (snapShotTask != null)
                {
                    snapShotTask.Wait();
                    snapShotTask.Dispose();
                    snapShotTask = null;
                    return lastImageInfo;
                }
                return null;
            }
        }

        internal void StartSnapshotOfScreenTask(Screen activeScreen)
        {
            return;
            object lockObject = SnapshotGenerator.lockObject;
            lock (lockObject)
            {
                if (snapShotTask != null)
                {
                    try
                    {
                                                                        sourceToken.Cancel();
                        sourceToken.Dispose();
                                            }
                    catch (Exception exception)
                    {
                        object[] args = { exception };
                        CrapyLogger.log.ErrorFormat("SnapshotGenerator.StartSnapshotOfScreenTask: Error {0}", args);
                    }
                }
                sourceToken = new CancellationTokenSource();
                this.token = sourceToken.Token;
                snapShotTask = Task.Factory.StartNew(delegate
                {
                    CancellationToken token = this.token;
                    ZappyTaskImageInfo info = TakeSnapshotOfScreenInternal(activeScreen);
                    if (!token.IsCancellationRequested)
                    {
                        lastImageInfo = info;
                    }
                    else
                    {
                        
                    }
                }, this.token);
            }
        }

        internal ZappyTaskImageInfo TakeSnapshotOfScreen(Screen activeScreen)
        {
            object lockObject = SnapshotGenerator.lockObject;
            lock (lockObject)
            {
                ZappyTaskImageInfo lastAsyncTaskOutput = GetLastAsyncTaskOutput();
                if (lastAsyncTaskOutput == null)
                {
                    lastAsyncTaskOutput = TakeSnapshotOfScreenInternal(activeScreen);
                    lastImageInfo = lastAsyncTaskOutput;
                }
                return lastAsyncTaskOutput;
            }
        }

        private ZappyTaskImageInfo TakeSnapshotOfScreenInternal(Screen screen)
        {
                                    return SnapShotProvider.TakeAndSaveSnapShotAsync(screen);
                    }

        internal static SnapshotGenerator Instance
        {
            get
            {
                if (instance == null)
                {
                    object lockObject = SnapshotGenerator.lockObject;
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new SnapshotGenerator();
                        }
                    }
                }
                return instance;
            }
        }

        internal ZappyTaskImageInfo LastSnapshot
        {
            get
            {
                object lockObject = SnapshotGenerator.lockObject;
                lock (lockObject)
                {
                    return lastImageInfo;
                }
            }
        }

        internal ISnapShotProvider SnapShotProvider { get; set; }
    }
}