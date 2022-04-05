using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.LogManager;
using Zappy.Helpers;
using Zappy.SharedInterface;
using Zappy.SharedInterface.Helper;
using Zappy.ZappyActions.Core;
using Zappy.ZappyActions.Core.Helper;

namespace Zappy.ZappyTaskEditor.ExecutionHelpers
{
    public sealed class ZappyExecutionContext : IZappyExecutionContext
    {
        public Dictionary<string, object> ContextData { get; private set; }

        public Dictionary<string, Tuple<IDynamicProperty, Func<object>>> Variables { get; private set; }

        List<string> _DefaultContextDataItems;

        public ZappyExecutionContext(ZappyTask Task, Dictionary<string, object> InitContextData = null)
        {
            List<IZappyAction> Actions = Task.ExecuteActivities.Activities;

            Variables = new Dictionary<string, Tuple<IDynamicProperty, Func<object>>>();

            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i] is IVariableAction ivariableAction)
                {
                    VariableNodeHelper.UpdateContextWithVariables(ivariableAction, this);

                                                                                                }
            }

            if (InitContextData == null)
                ContextData = new Dictionary<string, object>();
            else
                ContextData = new Dictionary<string, object>(InitContextData);

            _DefaultContextDataItems = new List<string>(ContextData.Keys);

            ContextData[CrapyConstants.ZappyTaskKey] = Task;
        }

                internal Tuple<IDynamicProperty, Func<object>> ExpandVariableObjectValue(DynamicProperty<object> EvaluationExpression)
        {
            DynamicProperty<object> _ScriptOrRunTimeVariableOrStaticValue = EvaluationExpression;
            object _VariableEvaluationExpression = string.Empty;
            Func<object> _ExpansionFunc = null;

            if (_ScriptOrRunTimeVariableOrStaticValue.DymanicKeySpecified
                || _ScriptOrRunTimeVariableOrStaticValue.ValueSpecified
                || _ScriptOrRunTimeVariableOrStaticValue.RuntimeScriptSpecified)
            {
                if (_ScriptOrRunTimeVariableOrStaticValue.ValueSpecified)
                {
                    _VariableEvaluationExpression = _ScriptOrRunTimeVariableOrStaticValue.Value;
                    _ExpansionFunc = () => _VariableEvaluationExpression;

                }
                else if (_ScriptOrRunTimeVariableOrStaticValue.DymanicKeySpecified)
                {
                    _VariableEvaluationExpression = _ScriptOrRunTimeVariableOrStaticValue.DymanicKey;

                    if ((_VariableEvaluationExpression as string).StartsWith(SharedConstants.VariableNameBegin))
                        _ExpansionFunc = () => GetValueFromKey(_VariableEvaluationExpression as string);
                    else if ((_VariableEvaluationExpression as string).Contains(SharedConstants.ScriptCriteria))
                    {
                        Func<ZappyExecutionContext, object> _VariableExpression = ExpandDynamicExpression<ZappyExecutionContext, object>(_VariableEvaluationExpression as string);
                        _ExpansionFunc = () => _VariableExpression(this);
                    }
                }
                else if (_ScriptOrRunTimeVariableOrStaticValue.RuntimeScriptSpecified)
                {
                    Func<ZappyExecutionContext, object> _VariableExpression = ExpandDynamicExpression<ZappyExecutionContext, object>(_VariableEvaluationExpression as string);
                    _ExpansionFunc = () => _VariableExpression(this);
                }
            }

            return new Tuple<IDynamicProperty, Func<object>>(_ScriptOrRunTimeVariableOrStaticValue, _ExpansionFunc);
        }

        internal Tuple<IDynamicProperty, Func<object>> ExpandVariable(DynamicProperty<string> EvaluationExpression)
        {
            DynamicProperty<string> _ScriptOrRunTimeVariableOrStaticValue = EvaluationExpression;
            string _VariableEvaluationExpression = string.Empty;
            Func<object> _ExpansionFunc = null;

            if (_ScriptOrRunTimeVariableOrStaticValue.DymanicKeySpecified
                || _ScriptOrRunTimeVariableOrStaticValue.ValueSpecified
                || _ScriptOrRunTimeVariableOrStaticValue.RuntimeScriptSpecified)
            {
                if (_ScriptOrRunTimeVariableOrStaticValue.ValueSpecified)
                {
                    _VariableEvaluationExpression = _ScriptOrRunTimeVariableOrStaticValue.Value;
                    _ExpansionFunc = () => _VariableEvaluationExpression;

                }
                else if (_ScriptOrRunTimeVariableOrStaticValue.DymanicKeySpecified)
                {
                    _VariableEvaluationExpression = _ScriptOrRunTimeVariableOrStaticValue.DymanicKey;

                    if (_VariableEvaluationExpression.StartsWith(SharedConstants.VariableNameBegin))
                        _ExpansionFunc = () => GetValueFromKey(_VariableEvaluationExpression);
                    else if (_VariableEvaluationExpression.Contains(SharedConstants.ScriptCriteria))
                    {
                        Func<ZappyExecutionContext, object> _VariableExpression = ExpandDynamicExpression<ZappyExecutionContext, object>(_VariableEvaluationExpression);
                        _ExpansionFunc = () => _VariableExpression(this);
                    }
                }
                else if (_ScriptOrRunTimeVariableOrStaticValue.RuntimeScriptSpecified)
                {
                    Func<ZappyExecutionContext, object> _VariableExpression = ExpandDynamicExpression<ZappyExecutionContext, object>(_VariableEvaluationExpression);
                    _ExpansionFunc = () => _VariableExpression(this);
                }
            }

            return new Tuple<IDynamicProperty, Func<object>>(_ScriptOrRunTimeVariableOrStaticValue, _ExpansionFunc);
        }

        public static string CleanDynamicExpressionText(string FunctionBody)
        {
            int _Index = FunctionBody.IndexOf(SharedConstants.VariableNameBegin);
            if (_Index >= 0)
            {
                StringBuilder sb = new StringBuilder(FunctionBody, FunctionBody.Length + 150);
                sb.ExpandVariable("context.GetValueFromKey(\"" + SharedConstants.VariableNameBegin, CrapyConstants.VariableNameEnd + "\")");
                                                                return sb.ToString();
            }

            return FunctionBody;
        }

        static ScriptOptions _DefaultScriptOptions;
        static ScriptOptions GetDefaultScriptOptions()
        {
            if (_DefaultScriptOptions == null)
            {
                _DefaultScriptOptions = ScriptOptions.Default.WithImports("System");
                _DefaultScriptOptions = _DefaultScriptOptions.AddImports("System.Reflection.Metadata");
                Assembly[] _LoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (int i = 0; i < _LoadedAssemblies.Length; i++)
                {
                    try
                    {
                        if (!_LoadedAssemblies[i].IsDynamic)
                            _DefaultScriptOptions = _DefaultScriptOptions.AddReferences(_LoadedAssemblies[i]);
                    }
                    catch { }
                }
            }
            return _DefaultScriptOptions;
        }

        public static Func<TInParam, TResult> ExpandDynamicExpression<TInParam, TResult>(string FunctionBody)
        {
            FunctionBody = CleanDynamicExpressionText(FunctionBody);

            ScriptOptions options = GetDefaultScriptOptions();

            Task<Func<TInParam, TResult>> _Task =
                CSharpScript.EvaluateAsync<Func<TInParam, TResult>>(
                    FunctionBody, options);
            _Task.Wait();
            Func<TInParam, TResult> _VariableExpression = _Task.Result;
            return _VariableExpression;
        }

        public static Func<TInParam1, TInParam2, TResult> ExpandDynamicExpression<TInParam1, TInParam2, TResult>(string FunctionBody)
        {
            FunctionBody = CleanDynamicExpressionText(FunctionBody);

            ScriptOptions options = GetDefaultScriptOptions();

            Task<Func<TInParam1, TInParam2, TResult>> _Task =
                CSharpScript.EvaluateAsync<Func<TInParam1, TInParam2, TResult>>(
                    FunctionBody, options);
            _Task.Wait();
            Func<TInParam1, TInParam2, TResult> _VariableExpression = _Task.Result;
            return _VariableExpression;
        }

        public object GetValueFromKey(string Key)
        {
            object _returnObj = null;
            if (!ContextData.TryGetValue(Key, out _returnObj))
            {
                Tuple<IDynamicProperty, Func<object>> _Expression = null;
                if (Variables.TryGetValue(Key, out _Expression))
                    _returnObj = _Expression.Item2();
                else
                    _returnObj = null;
            }
            return _returnObj;

        }

        public void SetValue(string Key, object Value)
        {
            Key = SharedConstants.VariableNameBegin + Key + CrapyConstants.VariableNameEnd;
            ContextData[Key] = Value;
        }

        public void ThrowIfCancellationRequested()
        {
                    }

        public void SaveContext(IZappyAction Action)
        {
            ContextData[CrapyConstants.ZappyPreviousAction] = Action;

            PropertyInfo[] _Properties = ActionTypeInfo.AllPropertyInfo[Action.GetType()];

            for (int i = 0; i < _Properties.Length; i++)
            {
                try
                {
                    string _Key = GetKey(Action.SelfGuid, _Properties[i]);
                    object o = _Properties[i].GetValue(Action);
                    if (o is IDynamicProperty)
                        o = (o as IDynamicProperty).ObjectValue;
                    ContextData[_Key] = o;
                }
                catch (Exception)
                {
                                    }
            }
        }

        public void LoadContext(IZappyAction Action)
        {
            try
            {
                ContextData[CrapyConstants.ZappyCurrentAction] = Action;

                PropertyInfo[] _Properties = ActionTypeInfo.DynamicPropertyInfo[Action.GetType()];

                for (int i = 0; i < _Properties.Length; i++)
                {
                    try
                    {
                        IDynamicProperty _PropertyValue = _Properties[i].GetValue(Action) as IDynamicProperty;

                        if (_PropertyValue != null)
                        {
                            if (_PropertyValue.DymanicKeySpecified)
                            {
                                string _DynamicKey = _PropertyValue.DymanicKey;

                                if (_PropertyValue.DymanicKeySpecified && _DynamicKey.StartsWith(SharedConstants.VariableNameBegin) && _DynamicKey.EndsWith(CrapyConstants.VariableNameEnd))
                                {
                                    object _val = null;
                                    Tuple<IDynamicProperty, Func<object>> _ExpressionExpander = null;

                                    bool _SetVal = ContextData.TryGetValue(_DynamicKey, out _val);
                                    if (!_SetVal && Variables.TryGetValue(_DynamicKey, out _ExpressionExpander))
                                    {
                                        _val = _ExpressionExpander.Item2();
                                        if (_ExpressionExpander.Item1.EvaluateOnFirstUse)
                                            ContextData[_DynamicKey] = _val;
                                        _SetVal = true;
                                    }
                                    if (_SetVal)
                                        _PropertyValue.ObjectValue = _val;
                                }
                            }
                            else if (_PropertyValue.RuntimeScriptSpecified && !(Action is RunScriptActivity))
                            {
                                Func<ZappyExecutionContext, object> _VariableExpression = ExpandDynamicExpression<ZappyExecutionContext, object>(_PropertyValue.RuntimeScript);
                                Func<object> _Func = () => _VariableExpression(this);
                                _PropertyValue.ObjectValue = _Func();

                                if (_PropertyValue.EvaluateOnFirstUse)
                                {
                                    _PropertyValue.ResetFlags();
                                    _PropertyValue.ValueSpecified = true;
                                }
                                else
                                {
                                    _PropertyValue.DymanicKey = SharedConstants.VariableNameBegin + Guid.NewGuid().ToString() + "_RuntimeHandle" + CrapyConstants.VariableNameEnd;
                                    Variables[_PropertyValue.DymanicKey] = new Tuple<IDynamicProperty, Func<object>>(_PropertyValue, _Func);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CrapyLogger.log.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                CrapyLogger.log.Error(ex);
            }
        }

        public static string GetKey(Guid Guid, PropertyInfo Property)
        { return GetKey(Guid, Property.Name); }

        public static string GetKey(Guid Guid, string PropertyName)
        { return SharedConstants.VariableNameBegin + Guid.ToString() + ":" + PropertyName + CrapyConstants.VariableNameEnd; }

        public static string GetGuid(string DynamicKey)
        {
            return DynamicKey.Replace(SharedConstants.VariableNameBegin, "").Split(':')[0];
        }

        public static string GetKey(IZappyAction Action, PropertyInfo Property)
        {
            return GetKey(Action.SelfGuid, Property.Name);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void DestroyContext()
        {
            foreach (var context in ContextData)
            {
                if (_DefaultContextDataItems.Contains(context.Key))
                    continue;
                if (context.Value is IDisposable dcontext)
                    dcontext.Dispose();
            }
        }
    }
}
