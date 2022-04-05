using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Zappy.ActionMap.Enums;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;

namespace Zappy.ActionMap.TaskAction
{
    [DebuggerDisplay("Count = {Count}"),]
    public class ListQueueStack<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, IDisposable
    {
        private readonly AutoResetEvent dequeueEvent;
        private bool disposed;
        private bool endOfQueue;
        private List<T> listImplementation;
        private readonly object lockObject;
        private readonly AutoResetEvent queueEvent;

        public ListQueueStack()
        {
            listImplementation = new List<T>();
            lockObject = new object();
            queueEvent = new AutoResetEvent(false);
            dequeueEvent = new AutoResetEvent(false);
        }

        public void Add(T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                listImplementation.Add(element);
                queueEvent.Set();
            }
        }

        public void AddRange(IEnumerable<T> collection)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                listImplementation.AddRange(collection);
                queueEvent.Set();
            }
        }

        private void AssertValidIndex(int index)
        {
            if (index < 0)
                CrapyLogger.log.ErrorFormat("ListQueueStack: Invalid index.  Index cannot be < 0");
            else if (index >= listImplementation.Count)
                CrapyLogger.log.ErrorFormat("ListQueueStack: Invalid index.  Index cannot be >= Count");
        }

        private void AssertValidIndexAndCount(int index, int count, bool backward)
        {
            AssertValidIndex(index);
            if (count < 0)
                CrapyLogger.log.ErrorFormat("ListQueueStack: Invalid count.  Index cannot be < 0");
            int num = count - 1;
            if (backward)
            {
                num = -num;
            }
            AssertValidIndex(index + num);
        }

        public T BlockingDequeue()
        {
            while (!EndOfQueue)
            {
                object obj2 = this.lockObject;
                lock (obj2)
                {
                    if (listImplementation.Count > 0)
                    {
                        return RemoveAtInternal(0);
                    }
                }
                queueEvent.WaitOne(0x3e8, true);
            }
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (listImplementation.Count > 0)
                {
                    return RemoveAtInternal(0);
                }
            }
            return default(T);
        }

        public void Clear()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                listImplementation.Clear();
                endOfQueue = false;
                queueEvent.Reset();
                dequeueEvent.Set();
            }
        }

        public bool Contains(T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.Contains(element);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                listImplementation.CopyTo(array, arrayIndex);
            }
        }

        public T Dequeue()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                IfEmptyThrowException();
                return RemoveAtInternal(0);
            }
        }

        public void Dispose()
        {
            if (!disposed)
            {
                queueEvent.Close();
                dequeueEvent.Close();
                disposed = true;
                GC.SuppressFinalize(this);
            }
        }

        public void Enqueue(T element)
        {
            Push(element);
        }

        public T Find(Predicate<T> match)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.Find(match);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.GetEnumerator();
            }
        }

        private void IfEmptyThrowException()
        {
            if (listImplementation.Count == 0)
            {
                CrapyLogger.log.ErrorFormat("ListQueueStack: List is empty");
                throw new InvalidOperationException();
            }
        }

        public int IndexOf(T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.IndexOf(element);
            }
        }

        public int IndexOf(T element, int index)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndex(index);
                return listImplementation.IndexOf(element, index);
            }
        }

        public int IndexOf(T element, int index, int count)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndexAndCount(index, count, false);
                return listImplementation.IndexOf(element, index, count);
            }
        }

        public void Insert(int index, T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndex(index);
                listImplementation.Insert(index, element);
                queueEvent.Set();
            }
        }

        public void InsertRange(int index, IEnumerable<T> collection)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndex(index);
                listImplementation.InsertRange(index, collection);
                queueEvent.Set();
            }
        }

        public int LastIndexOf(T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.LastIndexOf(element);
            }
        }

        public int LastIndexOf(T element, int index)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndex(index);
                return listImplementation.LastIndexOf(element, index);
            }
        }

        public int LastIndexOf(T element, int index, int count)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndexAndCount(index, count, true);
                return listImplementation.LastIndexOf(element, index, count);
            }
        }

        public virtual T Peek() =>
            Peek(0);

        public virtual T Peek(int nth) =>
            this[Count - 1 - nth];

        public T PeekFront()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                IfEmptyThrowException();
                return listImplementation[0];
            }
        }

        public virtual T Pop() =>
            Pop(0);

        public virtual T Pop(int nth)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                int index = Count - 1 - nth;
                T local = this[index];
                RemoveAt(index);
                return local;
            }
        }

        [Conditional("DEBUG_AGGREGATOR")]
        private void PrintQueue()
        {
            if (listImplementation is List<ZappyTaskAction>)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append("\r\nPRINTING ACTION QUEUE\r\n");
                for (int i = listImplementation.Count - 1; i >= 0; i--)
                {
                    builder.Append(listImplementation[i] + "\r\n");
                }
                
            }
        }

        public virtual void Push(T element)
        {
            Add(element);
        }

        public bool Remove(T element)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                int index = listImplementation.IndexOf(element);
                if (index != -1)
                {
                    RemoveAtInternal(index);
                }
                return index != -1;
            }
        }

        public void RemoveAt(int index)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndex(index);
                RemoveAtInternal(index);
            }
        }

        private T RemoveAtInternal(int index)
        {
            T local = listImplementation[index];
            listImplementation.RemoveAt(index);
            queueEvent.Reset();
            dequeueEvent.Set();
            return local;
        }

        public void RemoveRange(int index, int count)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                AssertValidIndexAndCount(index, count, false);
                listImplementation.RemoveRange(index, count);
                queueEvent.Reset();
                dequeueEvent.Set();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.GetEnumerator();
            }
        }

        public T[] ToArray()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                return listImplementation.ToArray();
            }
        }

        public List<T> ToListAndClear()
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                List<T> listImplementation = this.listImplementation;
                this.listImplementation = new List<T>();
                queueEvent.Reset();
                dequeueEvent.Set();
                return listImplementation;
            }
        }

        public WaitResult WaitForElement() =>
            WaitForElement(-1);

        public WaitResult WaitForElement(int timeoutInMilliseconds)
        {
            object lockObject = this.lockObject;
            lock (lockObject)
            {
                if (listImplementation.Count > 0)
                {
                    return WaitResult.Success;
                }
                if (endOfQueue)
                {
                    return WaitResult.EndOfQueue;
                }
            }
            if (!queueEvent.WaitOne(timeoutInMilliseconds, false))
            {
                return WaitResult.Timeout;
            }
            object obj3 = this.lockObject;
            lock (obj3)
            {
                if (listImplementation.Count > 0)
                {
                    return WaitResult.Success;
                }
                return WaitResult.EndOfQueue;
            }
        }

        public WaitResult WaitForEmpty()
        {
            while (listImplementation.Count > 0)
            {
                dequeueEvent.WaitOne(0x3e8, true);
            }
            return WaitResult.Success;
        }

        public virtual int Count
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return listImplementation.Count;
                }
            }
        }

        public bool EndOfQueue
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    return endOfQueue;
                }
            }
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    endOfQueue = value;
                    if (endOfQueue)
                    {
                        queueEvent.Set();
                    }
                    else
                    {
                        queueEvent.Reset();
                    }
                    dequeueEvent.Set();
                }
            }
        }

        public bool IsReadOnly =>
            false;

        public T this[int index]
        {
            get
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    AssertValidIndex(index);
                    return listImplementation[index];
                }
            }
            set
            {
                object lockObject = this.lockObject;
                lock (lockObject)
                {
                    AssertValidIndex(index);
                    listImplementation[index] = value;
                }
            }
        }

        public virtual object SyncRoot =>
            lockObject;
    }
}