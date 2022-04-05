using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.ElementPicker.Helper;
using Zappy.ZappyTaskEditor.EditorPage.Forms;


namespace Zappy.ActionMap.ZappyTaskUtil
{
    public static class ActionTypeInfo
    {

        static ActionTypeInfo()
        {
            LoadActionTypeInfo();
        }

        public static void LoadActionTypeInfo()
        {
            try
            {
                Type type = typeof(IZappyAction);
                List<Type> list = new List<Type>();
                Type[] types = Assembly.GetExecutingAssembly().GetLoadableTypes().ToArray();
                for (int i = 0; i < types.Length; i++)
                {
                    var p = types[i];
                    if (type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface) list.Add(p);
                }

                                
                if (Directory.Exists(CrapyConstants.ProfileDllDirectory))
                {
                    foreach (string file in Directory.EnumerateFiles
                        (CrapyConstants.ProfileDllDirectory, "*.dll", SearchOption.AllDirectories))
                    {
                        try
                        {
                                                                                    Assembly assembly = Assembly.Load(File.ReadAllBytes(file));
                                                        CommonProgram.LoadExternalAssembly(assembly);
                                                        list.AddRange(assembly.GetLoadableTypes()
                                .Where(p => type.IsAssignableFrom(p) && !p.IsAbstract && !p.IsInterface));
                                                    }
                        catch (Exception ex)
                        {
                            CrapyLogger.log.Error("Err 148 external assembly " + ex);
                        }
                    }
                }

                SupportedActionTypes = list.ToArray();

                AllPropertyInfo = new Dictionary<Type, PropertyInfo[]>(SupportedActionTypes.Length);
                DynamicPropertyInfo = new Dictionary<Type, PropertyInfo[]>(SupportedActionTypes.Length);
                HelperFunctions.TypeToHumanizedString = new Dictionary<Type, string>(SupportedActionTypes.Length);
                CrapyConstants.TypeToActivityFormDisplayHelper = new Dictionary<Type, ActivityFormDisplayHelper>(SupportedActionTypes.Length);
                                TypeDescriptor.AddAttributes(typeof(IDynamicProperty), new EditorAttribute(typeof(DynamicPropertyPicker), typeof(UITypeEditor)));

                SupportedActionTypesSerializers = new XmlSerializer[SupportedActionTypes.Length];

                Serializers_ByClosingXMLTag =
                    new Dictionary<string, XmlSerializer>(SupportedActionTypes.Length, StringComparer.Ordinal);

                Serializers_ByTypeName =
                    new Dictionary<string, XmlSerializer>(SupportedActionTypes.Length, StringComparer.Ordinal);

                for (int i = 0; i < SupportedActionTypes.Length; i++)
                {
                    try
                    {
                        string _TypeName = "/" + SupportedActionTypes[i].Name + ">";
                        Serializers_ByTypeName[SupportedActionTypes[i].Name] = Serializers_ByClosingXMLTag[_TypeName] =
                            SupportedActionTypesSerializers[i] = null;
                                                Serializers_ByTypeName[SupportedActionTypes[i].Name] = Serializers_ByClosingXMLTag[_TypeName] =
                                                    SupportedActionTypesSerializers[i] = new XmlSerializer(SupportedActionTypes[i]);

                        PropertyInfo[] _Properties = null, _DynamicProperties = null;

                        GetPropertyInfos(SupportedActionTypes[i], out _Properties, out _DynamicProperties);
                        AllPropertyInfo[SupportedActionTypes[i]] = _Properties;
                        DynamicPropertyInfo[SupportedActionTypes[i]] = _DynamicProperties;

                                                string nodeText = SupportedActionTypes[i].Name.Humanize();
                        if (typeof(DecisionNodeAction).IsAssignableFrom(type))
                        {
                            nodeText += "?";
                        }
                        HelperFunctions.TypeToHumanizedString[SupportedActionTypes[i]] = nodeText;
                        ActivityFormDisplayHelper act = new ActivityFormDisplayHelper();
                        act.GroupKey = String.Join(".", SupportedActionTypes[i].FullName.Split('.').Reverse().Skip(1).Reverse());
                        act.GroupNodeText = act.GroupKey.Split('.').Last() + " Activities";
                        act.ComparisionString = SupportedActionTypes[i].Name.ToLower().Replace(" ", string.Empty);
                        act.MyAttribute = (DescriptionAttribute)Attribute.GetCustomAttribute(SupportedActionTypes[i], typeof(DescriptionAttribute));
                        CrapyConstants.TypeToActivityFormDisplayHelper[SupportedActionTypes[i]] = act;

                    }
                    catch (Exception ex)
                    {
                                                                        CrapyLogger.log.Error("Unable to Load action " + ex);
                    }

                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error("Err TypeLoader - Critical Program Should Exit " + ex);
            }
        }

        static void GetPropertyInfos(Type ActionType, out PropertyInfo[] _Properties, out PropertyInfo[] _DynamicProperties)
        {
            _Properties = ActionType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<PropertyInfo> _AllProperties = new List<PropertyInfo>(_Properties.Length);
            for (int i = 0; i < _Properties.Length; i++)
            {
                if (_Properties[i].GetGetMethod(false) != null &&
                    _Properties[i].GetSetMethod(false) != null &&
                    (!Attribute.IsDefined(_Properties[i], typeof(BrowsableAttribute)) ||
                     _Properties[i].GetCustomAttributes<BrowsableAttribute>().Contains(BrowsableAttribute.Yes)))
                    _AllProperties.Add(_Properties[i]);
            }
            _Properties = _AllProperties.ToArray();
            for (int i = 0; i < _Properties.Length; i++)
            {
                if (_Properties[i].PropertyType.IsGenericType && _Properties[i].PropertyType.GetGenericTypeDefinition() != typeof(DynamicProperty<>))
                    _AllProperties.Remove(_Properties[i]);
            }

            _DynamicProperties = _AllProperties.ToArray();
        }


        public static XmlSerializer GetSerializerForAction(string Action)
        {
            if (string.IsNullOrEmpty(Action))
                return null;
            Action = Action.Substring(
                Action.LastIndexOf(
                    '/'));             XmlSerializer _Xcer = null;
            Serializers_ByClosingXMLTag.TryGetValue(Action, out _Xcer);
            return _Xcer;
        }

        public static Dictionary<string, XmlSerializer> Serializers_ByClosingXMLTag;

        public static Dictionary<string, XmlSerializer> Serializers_ByTypeName;

        public static Type[] SupportedActionTypes;

        public static Dictionary<Type, PropertyInfo[]> AllPropertyInfo;

        public static Dictionary<Type, PropertyInfo[]> DynamicPropertyInfo;

        public static XmlSerializer[] SupportedActionTypesSerializers;

    }
}