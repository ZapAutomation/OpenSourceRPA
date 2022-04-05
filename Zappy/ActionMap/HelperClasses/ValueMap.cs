using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;

namespace Zappy.ActionMap.HelperClasses
{
    public class ValueMap
    {
        private Collection<ActionParameter> parameterList;
        private Dictionary<string, ActionParameter> parameters;

        public ValueMap()
        {
            Initialize();
        }

        public void Add(ActionParameter parameter)
        {
            parameter.Name = GetStringInProperFormat(parameter.Name);
            if (!Contains(parameter.Name))
            {
                Parameters.Add(parameter.Name, parameter);
                ParameterList.Add(parameter);
            }
        }

        public void Clear()
        {
            Initialize();
        }

        public bool Contains(string parameterName) =>
            Parameters.ContainsKey(GetStringInProperFormat(parameterName));

        private static string GetStringInProperFormat(string parameterName) =>
            parameterName.ToString(CultureInfo.InvariantCulture).Trim();

        private void Initialize()
        {
            parameters = new Dictionary<string, ActionParameter>(StringComparer.OrdinalIgnoreCase);
            parameterList = new Collection<ActionParameter>();
        }

        public void PopulateParametersDictionaryAfterDeserialization()
        {
            Parameters.Clear();
            foreach (ActionParameter parameter in ParameterList)
            {
                Parameters.Add(parameter.Name, parameter);
            }
        }

        public bool Remove(string parameterName)
        {
            parameterName = GetStringInProperFormat(parameterName);
            if (Contains(parameterName))
            {
                ActionParameter parameter;
                Parameters.TryGetValue(parameterName, out parameter);
                ParameterList.Remove(parameter);
                Parameters.Remove(parameterName);
                return true;
            }
            return false;
        }

        public void SetParameterValue(string parameterName, object parameterValue)
        {
            ActionParameter parameter = null;
            if (Parameters.TryGetValue(GetStringInProperFormat(parameterName), out parameter))
            {
                parameter.Value = parameterValue;
            }
        }


        public bool TryGetParameterValue(string parameterName, out object parameterValue)
        {
            parameterValue = null;
            ActionParameter parameter = null;
            if (Parameters.TryGetValue(GetStringInProperFormat(parameterName), out parameter) && parameter.Value != null)
            {
                parameterValue = parameter.Value;
                return true;
            }
            return false;
        }

        public Collection<ActionParameter> ParameterList =>
            parameterList;

        private Dictionary<string, ActionParameter> Parameters =>
            parameters;
    }
}
