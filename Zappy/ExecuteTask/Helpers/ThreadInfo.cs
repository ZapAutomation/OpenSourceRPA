using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Zappy.ExecuteTask.Helpers.Interface;

namespace Zappy.ExecuteTask.Helpers
{
    [ComVisible(true)]
    internal class ThreadInfo : IThreadInfo
    {
        private Dictionary<System.Diagnostics.ThreadState, ThreadState> threadStateToIntDictionary = new Dictionary<System.Diagnostics.ThreadState, ThreadState>();
        private Dictionary<System.Diagnostics.ThreadWaitReason, ThreadWaitReason> threadWaitReasonToIntDictionary = new Dictionary<System.Diagnostics.ThreadWaitReason, ThreadWaitReason>();

        public ThreadInfo()
        {
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Initialized] = ThreadState.Initialized;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Ready] = ThreadState.Ready;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Running] = ThreadState.Running;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Standby] = ThreadState.Standby;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Terminated] = ThreadState.Terminated;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Wait] = ThreadState.Waiting;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Transition] = ThreadState.Transition;
            this.threadStateToIntDictionary[System.Diagnostics.ThreadState.Unknown] = ThreadState.UnknownState;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.Executive] = ThreadWaitReason.ComponentOfTheWindowsNTExecutive1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.FreePage] = ThreadWaitReason.PageToBeFreed1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.PageIn] = ThreadWaitReason.PageToBeMappedOrCopied1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.SystemAllocation] = ThreadWaitReason.SpaceToBeAllocatedInThePagedOrNonpagedPool1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.ExecutionDelay] = ThreadWaitReason.ExecutionDelayToBeresolved1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.Suspended] = ThreadWaitReason.Suspended1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.UserRequest] = ThreadWaitReason.UserRequest1;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.EventPairHigh] = ThreadWaitReason.EentPairHigh;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.EventPairLow] = ThreadWaitReason.EventPairLow;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.LpcReceive] = ThreadWaitReason.LPCReceiveNotice;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.LpcReply] = ThreadWaitReason.LPCReplyNotice;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.VirtualMemory] = ThreadWaitReason.VirtualMemoryToBeAllocated;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.PageOut] = ThreadWaitReason.PageToBewrittenToDisk;
            this.threadWaitReasonToIntDictionary[System.Diagnostics.ThreadWaitReason.Unknown] = ThreadWaitReason.Reserved;
        }

        public int GetThreadWaitReason(int processId, int threadId, out int threadStateRetVal)
        {
            Process processById = null;
            try
            {
                processById = Process.GetProcessById(processId);
            }
            catch (Win32Exception)
            {
            }
            catch (ArgumentException)
            {
            }
            System.Diagnostics.ThreadWaitReason unknown = System.Diagnostics.ThreadWaitReason.Unknown;
            System.Diagnostics.ThreadState threadState = System.Diagnostics.ThreadState.Unknown;
            if (processById != null)
            {
                foreach (ProcessThread thread in processById.Threads)
                {
                    if (thread.Id == threadId)
                    {
                        threadState = thread.ThreadState;
                        if (threadState == System.Diagnostics.ThreadState.Wait)
                        {
                            unknown = thread.WaitReason;
                            object[] args = new object[] { processId, threadId, threadState, unknown };
                            
                        }
                        break;
                    }
                }
            }
            else
            {
                object[] objArray2 = new object[] { processId };
                
            }
            threadStateRetVal = (int)this.threadStateToIntDictionary[threadState];
            return (int)this.threadWaitReasonToIntDictionary[unknown];
        }
    }
}