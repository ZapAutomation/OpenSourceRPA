using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.Query;
using Zappy.Decode.Mssa;
using Zappy.Properties;

namespace Zappy.ExecuteTask.Helpers
{
    public sealed class PropertyExpressionCollection : Collection<PropertyExpression>, ICloneable, ICollection<PropertyExpression>,
        IEnumerable<PropertyExpression>, IEnumerable, INotifyCollectionChanged
    {
                public event NotifyCollectionChangedEventHandler CollectionChanged;

        public PropertyExpressionCollection()
        {
        }

        internal PropertyExpressionCollection(List<PropertyCondition> list)
        {
            if (list != null)
            {
                foreach (PropertyCondition condition in list)
                {
                    Add(new PropertyExpression(condition));
                }
            }
        }

        public void Add(PropertyExpression propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException("propertyExpression");
            }
            Remove(propertyExpression.PropertyName);
            propertyExpression.PropertyChanged += OnPropertyChanged;
            Items.Add(propertyExpression);
        }

        public void Add(params string[] nameValuePairs)
        {
            if (nameValuePairs.Length % 2 != 0)
            {
                throw new ArgumentException(Resources.OddNumberOfArguments, "nameValuePairs");
            }
            for (int i = 0; i < nameValuePairs.Length; i += 2)
            {
                Add(nameValuePairs[i], nameValuePairs[i + 1]);
            }
        }

        public void Add(string propertyName, string propertyValue)
        {
            Add(propertyName, propertyValue, PropertyExpressionOperator.EqualTo);
        }

        public void Add(string propertyName, string propertyValue, PropertyExpressionOperator conditionOperator)
        {
            Add(new PropertyExpression(propertyName, propertyValue, conditionOperator));
        }

        public void AddRange(params PropertyExpression[] propertyExpressions)
        {
            foreach (PropertyExpression expression in propertyExpressions)
            {
                Add(expression);
            }
        }

        public void AddRange(PropertyExpressionCollection collectionToAdd)
        {
            if (collectionToAdd != null && this != collectionToAdd)
            {
                foreach (PropertyExpression expression in collectionToAdd)
                {
                    Add(expression);
                }
            }
        }

        public object Clone()
        {
            PropertyExpressionCollection expressions = new PropertyExpressionCollection();
            foreach (PropertyExpression expression in Items)
            {
                expressions.Add((PropertyExpression)expression.Clone());
            }
            return expressions;
        }

        public bool Contains(PropertyExpression item) =>
            Items.Contains(item);

        public bool Contains(string propertyName) =>
            Find(propertyName) != null;

        public void CopyTo(PropertyExpression[] expressionArray, int arrayIndex)
        {
            Items.CopyTo(expressionArray, arrayIndex);
        }

        public IEnumerator<PropertyExpression> GetEnumerator()
        {
            return base.GetEnumerator();
                    }

        public override bool Equals(object other)
        {
            PropertyExpressionCollection expressions = other as PropertyExpressionCollection;
            if (expressions == null)
            {
                return false;
            }
            if (Count != expressions.Count)
            {
                return false;
            }
            foreach (PropertyExpression expression in this)
            {
                if (!expressions.Contains(expression))
                {
                    return false;
                }
            }
            return true;
        }

        public PropertyExpression Find(string propertyName)
        {
            foreach (PropertyExpression expression in Items)
            {
                if (string.Equals(expression.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    return expression;
                }
            }
            return null;
        }

        private void FireCollectionChanged(NotifyCollectionChangedEventArgs eventArgs)
        {
            CollectionChanged?.Invoke(this, eventArgs);
        }

                        public override int GetHashCode()
        {
            int num = 0;
            foreach (PropertyExpression expression in this)
            {
                num ^= expression.GetHashCode();
            }
            return num;
        }

        public static void GetProperties(IQueryCondition queryConditions,
            out PropertyExpressionCollection primaryProperties, out PropertyExpressionCollection secondaryProperties)
        {
            primaryProperties = new PropertyExpressionCollection();
            secondaryProperties = new PropertyExpressionCollection();
            GetProperties(queryConditions, primaryProperties, secondaryProperties, false);
        }

        private static void GetProperties(IQueryCondition queryConditions,
            PropertyExpressionCollection primaryProperties, PropertyExpressionCollection secondaryProperties,
            bool secondarySearchProperties)
        {
            if (queryConditions != null && queryConditions.Conditions != null)
            {
                foreach (IQueryCondition condition in queryConditions.Conditions)
                {
                    PropertyCondition propertyCondition = condition as PropertyCondition;
                    if (propertyCondition != null)
                    {
                        GetProperties(propertyCondition, primaryProperties, secondaryProperties,
                            secondarySearchProperties);
                    }
                    else if (condition is AndCondition)
                    {
                        GetProperties(condition, primaryProperties, secondaryProperties, secondarySearchProperties);
                    }
                    else if (condition is FilterCondition)
                    {
                        GetProperties(condition, primaryProperties, secondaryProperties, true);
                    }
                }
            }
        }

        private static void GetProperties(PropertyCondition propertyCondition,
            PropertyExpressionCollection primaryProperties, PropertyExpressionCollection secondaryProperties,
            bool secondarySearchProperties)
        {
            if (propertyCondition != null)
            {
                (secondarySearchProperties ? secondaryProperties : primaryProperties).Add(
                    new PropertyExpression(propertyCondition));
            }
        }

                                        
                                                
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            FireCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

                                                
                                                
        public bool Remove(PropertyExpression propertyExpression) =>
            Items.Contains(propertyExpression) && Remove(propertyExpression.PropertyName);

        public bool Remove(string propertyName)
        {
            PropertyExpression expression = Find(propertyName);
            if (expression != null)
            {
                expression.PropertyChanged -= OnPropertyChanged;
                Items.Remove(expression);
                return true;
            }
            return false;
        }

        internal PropertyCondition[] ToPropertyConditionArray()
        {
            List<PropertyCondition> list = new List<PropertyCondition>();
            foreach (PropertyExpression expression in Items)
            {
                list.Add(expression.ToPropertyCondition());
            }
            return list.ToArray();
        }

        public bool IsReadOnly =>
            false;

        public string this[string propertyName]
        {
            get
            {
                PropertyExpression expression = Find(propertyName);
                return expression?.PropertyValue;
            }
            set { Add(propertyName, value); }
        }

                        
                                
                                        
                                                                        
                                                                                                                                                                                                                                                                                        
                                        
                                                                                                                                        
                
                            }
}