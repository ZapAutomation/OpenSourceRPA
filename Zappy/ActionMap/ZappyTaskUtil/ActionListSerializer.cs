using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Xml.Serialization;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.ActionMap.ScreenMaps;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.LogManager;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Helpers.Interface;
using Zappy.SharedInterface;
using Zappy.ZappyActions.AutomaticallyCreatedActions;

namespace Zappy.ActionMap.ZappyTaskUtil
{
    internal class ActionListSerializer
    {
        private static readonly object lockObject = new object();

        public static readonly XmlAttributeOverrides overrides = CreateOverrides();

        private static XmlSerializer serializerWithoutNamespace;

        private static XmlAttributeOverrides CreateOverrides()
        {
            try
            {
                XmlAttributeOverrides overrides = CreateOverridesFromTypeArray(ActionTypeInfo.SupportedActionTypes);
                return overrides;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                return null;
            }
        }

        public static XmlAttributeOverrides CreateOverridesFromTypeArray(Type[] SupportedActionTypes)
        {
            try
            {
                XmlAttributeOverrides overrides = new XmlAttributeOverrides();

                XmlAttributes attributes = new XmlAttributes();
                foreach (Type type in SupportedActionTypes)
                {
                    attributes.XmlElements.Add(new XmlElementAttribute(type));
                }

                overrides.Add(typeof(ActionList), "Activities", attributes);


                attributes = new XmlAttributes();
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(QueryCondition)));
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(AndCondition)));
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(FilterCondition)));
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(PropertyCondition)));
                overrides.Add(typeof(TaskActivityObject), "Condition", attributes);
                overrides.Add(typeof(QueryCondition), "ConditionsWrapper", attributes);
                attributes = new XmlAttributes
                {
                    XmlAttribute = new XmlAttributeAttribute()
                };
                overrides.Add(typeof(Point), "X", attributes);
                overrides.Add(typeof(Point), "Y", attributes);
                attributes = new XmlAttributes();
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(SettingGroup)));
                overrides.Add(typeof(ZappyTaskEnvironment), "Group", attributes);
                attributes = new XmlAttributes();
                attributes.XmlElements.Add(new XmlElementAttribute(typeof(Setting)));
                overrides.Add(typeof(SettingGroup), "Setting", attributes);

                return overrides;
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
                return null;
            }
        }

        public static ZappyTask Deserialize(Stream reader)
        {
            ZappyTask test2;
            
            try
            {
                ZappyTask task;
                if (reader.CanSeek)
                {
                                        reader.Seek(0L, SeekOrigin.Begin);
                }
                else
                {
                    CrapyLogger.log.Error("ActionListSerializer.Deserialize(Stream) - Could not validate input for VS2010 Beta 2 format.");
                }
                object lockObject = ActionListSerializer.lockObject;
                lock (lockObject)
                {
                    task = (ZappyTask)ZappyTaskSerializationHelper.Deserialize(GetSerializer(), reader);
                }
                                                                test2 = task;
            }
            catch (Exception exception)
            {
                CrapyLogger.log.Error(exception);
                throw new ZappyTaskException(exception.Message);
            }
            return test2;
        }

                                                                        
        private static void FixUIElementNameInActionList(List<IZappyAction> actionList, IDictionary<ITaskActivityElement, string> dictionary)
        {

        }

        private static void FixUIElementNameInZappyTask(ZappyTask testcase)
        {
                        {
                                                foreach (IZappyAction action in testcase.ExecuteActivities.Activities)
                {
                    if (action is ZappyTaskAction zaction)
                    {
                        FixZappyActionIdentifier(zaction);

                    }
                }
                                            }
        }


        internal static void FixZappyActionIdentifier(ZappyTaskAction zaction)
        {
            try
            {
                if (string.IsNullOrEmpty(zaction.TaskActivityIdentifier))
                {
                    if (zaction.WindowIdentifier == null)
                    {
                        zaction.WindowIdentifier = new ScreenIdentifier
                        {
                            Id = "Desktop"
                        };
                    }
                    zaction.WindowIdentifier.AddUIObjects(zaction.GetUIElementCollection());
                    TopLevelElement topLevelElementForString =
                        zaction.FixUIObjectName(zaction.WindowIdentifier.UniqueNameDictionary);
                    if (topLevelElementForString != null)
                        zaction.WindowIdentifier.externalAddTopLevelElement(topLevelElementForString);
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

                                                        
        public static XmlSerializer GetSerializer()
        {
                                    if (serializerWithoutNamespace == null)
            {
                serializerWithoutNamespace = new XmlSerializer(typeof(ZappyTask), overrides);
            }
            serializerWithoutNamespace.UnknownNode += UnknownNode;

            return serializerWithoutNamespace;
            
        }

        static void UnknownNode(object sender, XmlNodeEventArgs e)
        {
            string UnexpectedNodeMessage = "Unexpected node: " + e.LocalName + "as line " + e.LineNumber +
                ", column " + e.LinePosition;
            System.Windows.MessageBox.Show(UnexpectedNodeMessage);
        }

                
        public static void Serialize(Stream stream, ZappyTask testcase)
        {
            try
            {
                FixUIElementNameInZappyTask(testcase);
                object lockObject = ActionListSerializer.lockObject;
                lock (lockObject)
                {
                    GetSerializer().Serialize(stream, testcase);
                }
            }
            catch (Exception exception)
            {
                CrapyLogger.log.Error(exception);
                throw;
            }
        }

                                                                                        
                                                                                            }
}