using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Zappy.Decode.LogManager;
using Zappy.SharedInterface;

namespace Zappy.ActionMap.ZappyTaskUtil
{


    public class ActionList : IXmlSerializable
    {
        private List<IZappyAction> actions = new List<IZappyAction>();

        private int currentIndex;
        private bool currentStateValid;

        public ActionList()
        {
            Reset();
        }

        internal void Add(IZappyAction action)
        {
            actions.Add(action);
        }

        public int AddRange(IEnumerable<IZappyAction> addActivities)
        {
            int num = 0;
            foreach (IZappyAction action in addActivities)
            {
                actions.Add(action);
                num++;
            }
            return num;
        }

        public void Delete(long id)
        {
            actions.RemoveAll(x => x.Id == id);
            Reset();
        }

        public int DeleteRange(long startId, long stopId)
        {
            int count = 0;
            int startIndex = actions.FindIndex(x => x.Id == startId);
            if (startIndex >= 0)
            {
                int num3 = actions.FindIndex(startIndex, x => x.Id == stopId);
                if (num3 >= startIndex)
                {
                    count = num3 - startIndex + 1;
                    actions.RemoveRange(startIndex, count);
                }
            }
            Reset();
            return count;
        }

        public bool Equals(ActionList actionList)
        {
            if (actionList == null)
            {
                return false;
            }
            if (this != actionList)
            {
                if (Count != actionList.Count)
                {
                    return false;
                }
                for (int i = 0; i < Count; i++)
                {
                    if (!Activities[i].Equals(actionList.Activities[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool Equals(object other)
        {
            ActionList actionList = other as ActionList;
            return Equals(actionList);
        }

        public IZappyAction Find(long id) =>
            actions.Find(x => x.Id == id);

        public int FindIndex(long id) =>
            actions.FindIndex(x => x.Id == id);

                
        public override int GetHashCode()
        {
            int num = 0;
            foreach (IZappyAction action in actions)
            {
                num ^= actions.GetHashCode();
            }
            return num;
        }

        public ICollection<IZappyAction> GetRange(long startId, long stopId)
        {
            int index = FindIndex(startId);
            int count = FindIndex(stopId) - index + 1;
            return actions.GetRange(index, count);
        }

        internal void Insert(int index, IZappyAction action)
        {
            actions.Insert(index, action);
        }

        private bool IsCurrentStateValid()
        {
            if (currentStateValid && (currentIndex < 0 || currentIndex >= actions.Count))
            {
                currentStateValid = false;
            }
            return currentStateValid;
        }

        internal bool MoveFirst()
        {
            if (IsCurrentStateValid())
            {
                currentIndex = 0;
                return true;
            }
            return false;
        }

        internal bool MoveLast()
        {
            if (IsCurrentStateValid())
            {
                currentIndex = actions.Count - 1;
                return true;
            }
            return false;
        }

        internal bool MoveNext()
        {
            currentIndex++;
            return IsCurrentStateValid();
        }

        internal bool MovePrevious()
        {
            currentIndex--;
            return IsCurrentStateValid();
        }

        internal bool MoveTo(int index)
        {
            currentIndex = index;
            return IsCurrentStateValid();
        }

        internal void Reset()
        {
            currentIndex = -1;
            currentStateValid = true;
        }

        public List<IZappyAction> Activities =>
            actions;

        public int Count =>
            actions.Count;

        internal IZappyAction Current
        {
            get
            {
                if (!IsCurrentStateValid())
                {
                    throw new InvalidOperationException();
                }
                return Activities[currentIndex];
            }
        }

        public IEnumerable<T> GetActivitiesOfType<T>() where T : IZappyAction
        {
            foreach (IZappyAction x in Activities)
            {
                if (x is T) yield return (T)x;
            }
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader inputStream)
        {
            if (actions == null)
                actions = new List<IZappyAction>();
            string parentNodeName = inputStream.Name;
            inputStream.Read();

                        XmlReader _SubtreeReader = inputStream;
            while (!_SubtreeReader.EOF)
            {
                if (ActionTypeInfo.Serializers_ByTypeName.TryGetValue(_SubtreeReader.Name, out XmlSerializer slzr))
                {
                    IZappyAction action = (IZappyAction)slzr.Deserialize(_SubtreeReader);
                    if (action == null && _SubtreeReader.Name.Equals("RunZappyTask"))
                    {
                        _SubtreeReader.Read();
                        continue;
                    }
                    actions.Add(action);
                }
                else if (string.IsNullOrEmpty(_SubtreeReader.Name))
                    _SubtreeReader.Read();
                else                     break;
            }
        }


        public void WriteXml(XmlWriter writer)
        {
            try
            {
                for (int index = 0; index < actions.Count; index++)
                {
                    ActionTypeInfo
                        .SupportedActionTypesSerializers[
                            Array.IndexOf(ActionTypeInfo.SupportedActionTypes, actions[index].GetType())].Serialize
                            (writer, actions[index]);
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                MessageBox.Show("Unable to save task due to ERR AL246");
            }
        }

                                                
                                                
    }
}