using html;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Zappy.ActionMap.Enums;
using Zappy.ActionMap.HelperClasses;
using Zappy.ActionMap.TaskTechnology;
using Zappy.ActionMap.ZappyTaskUtil;
using Zappy.Decode.Helper;
using Zappy.Decode.Hooks.Keyboard;
using Zappy.Decode.Mssa;
using Zappy.ExecuteTask.Extension.HtmlControls;
using Zappy.ExecuteTask.Helpers;
using Zappy.ExecuteTask.TaskExecutor;
using Zappy.Properties;
using HtmlMedia = Zappy.ExecuteTask.Helpers.HtmlMedia;

namespace Zappy.ExecuteTask.Extension
{
    internal class HtmlPropertyProvider : PropertyProviderBase
    {
        private const string Area = "Area";
        private const string Change = "change";
        private const string Div = "Div";
        private const string Editable = "Editable";
        private const string Enabled = "Enabled";
        private const string HeaderCell = "HeaderCell";
        private const string HTMLEventsString = "HTMLEvents";
        private const string IFrame = "IFrame";
        private const string Input = "Input";
        private const string InputButton = "InputButton";
        private const string OnChange = "onchange";
        private const string OnClick = "onclick";
        private const string Progress = "Progress";
        private const string ProgressBar = "ProgressBar";
        private const string Slider = "Slider";
        private const string Span = "Span";
        private const string TextArea = "TextArea";
        private const string TextAreaString = "TEXTAREA";
        private const string TH = "TH";

        public HtmlPropertyProvider()
        {
            commonProperties = InitializeCommonProperties();
            controlTypeToPropertiesMap = this.InitializePropertiesMap();
            controlTypeToPropertyNamesClassMap = this.InitializePropertyNameToClassMap();
            technologyName = "Web";
            specializedClassNamePrefix = "Html";
                    }

        protected void EnsureVisible(ZappyTaskControl uiControl)
        {
            try
            {
                uiControl.EnsureClickable();
            }
            catch (Exception)
            {
                            }
        }

        protected void FireOnChange(object domNode)
        {
            IHTMLElement element = domNode as IHTMLElement;
            if (element != null)
            {
                                                                                                                                                                                                                                                                            }
        }

        public override string[] GetPredefinedSearchProperties(Type specializedClass)
        {
            List<string> list = new List<string>();
            if (!string.Equals("HtmlControl", specializedClass.Name, StringComparison.Ordinal))
            {
                list.Add(ZappyTaskControl.PropertyNames.ControlType);
                if (!string.Equals("HtmlCustom", specializedClass.Name, StringComparison.Ordinal))
                {
                    list.Add(HtmlControl.PropertyNames.TagName);
                }
            }
            return list.ToArray();
        }

        protected override string GetPropertyForAction(string controlType, ZappyTaskAction action) =>
            ALUtility.GetHtmlPropertyForAction(controlType, action);

        protected override string[] GetPropertyForControlState(string controlType, ControlStates uiState,
            out bool[] stateValues)
        {
            bool[] flagArray1 = new bool[] { true };
            stateValues = flagArray1;
            string[] strArray = new string[0];
            if (ControlType.RadioButton.NameEquals(controlType) &&
                ((uiState & ControlStates.Checked) == ControlStates.Checked))
            {
                strArray = new string[] { HtmlRadioButton.PropertyNames.Selected };
            }
            if (ControlType.CheckBox.NameEquals(controlType))
            {
                if ((uiState & ControlStates.Checked) == ControlStates.Checked)
                {
                    strArray = new string[] { HtmlCheckBox.PropertyNames.Checked };
                }
                else if ((uiState & (ControlStates.None | ControlStates.Normal)) ==
                         (ControlStates.None | ControlStates.Normal))
                {
                    strArray = new string[] { HtmlCheckBox.PropertyNames.Checked };
                    stateValues[0] = false;
                }
            }
            if (strArray.Length == 0)
            {
                stateValues = new bool[0];
            }
            return strArray;
        }

        protected override object GetPropertyValueInternal(ZappyTaskControl uiTaskControl, string propertyName)
        {
            if (!(uiTaskControl.ControlType == ControlType.List) ||
                (!string.Equals(propertyName, HtmlList.PropertyNames.SelectedItemsAsString,
                     StringComparison.OrdinalIgnoreCase) && !string.Equals(propertyName,
                     ZappyTaskControl.PropertyNames.Value, StringComparison.OrdinalIgnoreCase)))
            {
                return uiTaskControl.TechnologyElement.GetPropertyValue(propertyName);
            }
            string[] propertyValue =
                this.GetPropertyValue(uiTaskControl, HtmlList.PropertyNames.SelectedItems) as string[];
            CommaListBuilder builder = new CommaListBuilder();
            if (propertyValue != null)
            {
                builder.AddRange(propertyValue);
            }
            return builder.ToString();
        }

        public override Type GetSpecializedClass(ZappyTaskControl uiControl)
        {
            string propertyValue = null;
            if (uiControl.SearchProperties.Contains("TagName"))
            {
                propertyValue = uiControl.SearchProperties["TagName"];
            }
            else if (uiControl.IsBound)
            {
                try
                {
                    propertyValue =
                        uiControl.TechnologyElement.GetPropertyValue(HtmlControl.PropertyNames.TagName) as string;
                }
                catch (ZappyTaskException)
                {
                }
            }
            string str2 = null;
            if (!string.IsNullOrEmpty(propertyValue))
            {
                if (string.Equals(propertyValue, "Div", StringComparison.OrdinalIgnoreCase))
                {
                    if (ControlType.Edit.NameEquals(uiControl.ControlType.Name))
                    {
                        str2 = "EditableDiv";
                    }
                    else
                    {
                        str2 = "Div";
                    }
                }
                else if (string.Equals(propertyValue, "Span", StringComparison.OrdinalIgnoreCase))
                {
                    if (ControlType.Edit.NameEquals(uiControl.ControlType.Name))
                    {
                        str2 = "EditableSpan";
                    }
                    else
                    {
                        str2 = "Span";
                    }
                }
                else if (string.Equals(propertyValue, "TextArea", StringComparison.OrdinalIgnoreCase))
                {
                    str2 = "TextArea";
                }
                else if (string.Equals(propertyValue, "IFrame", StringComparison.OrdinalIgnoreCase))
                {
                    str2 = "IFrame";
                }
                else if (string.Equals(propertyValue, "Input", StringComparison.OrdinalIgnoreCase))
                {
                    if (ControlType.Button.NameEquals(uiControl.ControlType.Name))
                    {
                        str2 = "InputButton";
                    }
                    else if (ControlType.Slider.NameEquals(uiControl.ControlType.Name))
                    {
                        str2 = "Slider";
                    }
                }
                else if (string.Equals(propertyValue, "TH", StringComparison.OrdinalIgnoreCase))
                {
                    str2 = "HeaderCell";
                }
                else if (string.Equals(propertyValue, "Progress", StringComparison.OrdinalIgnoreCase))
                {
                    str2 = "ProgressBar";
                }
                else if (ControlType.Hyperlink.NameEquals(uiControl.ControlType.Name) &&
                         string.Equals(propertyValue, "Area", StringComparison.OrdinalIgnoreCase))
                {
                    str2 = "Area" + ControlType.Hyperlink.Name;
                }
            }
            if (str2 != null)
            {
                object[] args = new object[] { specializedClassesNamespace, specializedClassNamePrefix + str2 };
                return Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}.{1}", args));
            }
            return base.GetSpecializedClass(uiControl);
        }

        protected static Dictionary<string, ZappyTaskPropertyDescriptor> InitializeCommonProperties()
        {
            ZappyTaskPropertyDescriptor descriptor = new ZappyTaskPropertyDescriptor(typeof(string),
                ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable |
                ZappyTaskPropertyAttributes.Readable);
            return new Dictionary<string, ZappyTaskPropertyDescriptor>(StringComparer.OrdinalIgnoreCase)
            {
                {
                    HtmlControl.PropertyNames.Class,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.HelpText,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    HtmlControl.PropertyNames.Id,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.InnerText,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.TagInstance,
                    new ZappyTaskPropertyDescriptor(typeof(int),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Searchable |
                        ZappyTaskPropertyAttributes.Readable)
                },
                {
                    HtmlControl.PropertyNames.TagName,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.Title,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.Type,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.ValueAttribute,
                    new ZappyTaskPropertyDescriptor(typeof(string),
                        ZappyTaskPropertyAttributes.CommonToTechnology | ZappyTaskPropertyAttributes.Readable)
                },
                {
                    HtmlControl.PropertyNames.AccessKey,
                    descriptor
                },
                {
                    HtmlControl.PropertyNames.ControlDefinition,
                    descriptor
                }
            };
        }

        protected Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>> InitializePropertiesMap()
        {
            Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>> dictionary =
                new Dictionary<ControlType, Dictionary<string, ZappyTaskPropertyDescriptor>>();
            ZappyTaskPropertyAttributes attributes =
                ZappyTaskPropertyAttributes.Searchable | ZappyTaskPropertyAttributes.Readable;
            ZappyTaskPropertyAttributes attributes2 =
                ZappyTaskPropertyAttributes.Writable | ZappyTaskPropertyAttributes.Readable;
            Dictionary<string, ZappyTaskPropertyDescriptor> dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlDocument.PropertyNames.PageUrl,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlDocument.PropertyNames.FrameDocument,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        HtmlDocument.PropertyNames.RedirectingPage,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes)
                    },
                    {
                        HtmlDocument.PropertyNames.AbsolutePath,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.Document, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlTable.PropertyNames.Cells,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlTable.PropertyNames.Rows,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlTable.PropertyNames.RowCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlTable.PropertyNames.ColumnCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlTable.PropertyNames.CellCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Table, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlRow.PropertyNames.Cells,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlRow.PropertyNames.RowIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlRow.PropertyNames.CellCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Row, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlCell.PropertyNames.RowIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlCell.PropertyNames.ColumnIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    }
                };
            dictionary.Add(ControlType.Cell, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlEdit.PropertyNames.Password,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Writable)
                    },
                    {
                        HtmlEdit.PropertyNames.Text,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlEdit.PropertyNames.IsPassword,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlEdit.PropertyNames.DefaultText,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlEdit.PropertyNames.CopyPastedText,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlEdit.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlEdit.PropertyNames.ReadOnly,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlEdit.PropertyNames.MaxLength,
                        new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Edit, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlList.PropertyNames.ItemCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlList.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlList.PropertyNames.SelectedItems,
                        new ZappyTaskPropertyDescriptor(typeof(string[]),
                            attributes2 | ZappyTaskPropertyAttributes.NonAssertable)
                    },
                    {
                        HtmlList.PropertyNames.SelectedIndices,
                        new ZappyTaskPropertyDescriptor(typeof(int[]),
                            attributes2 | ZappyTaskPropertyAttributes.NonAssertable)
                    },
                    {
                        HtmlList.PropertyNames.SelectedItemsAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlList.PropertyNames.IsMultipleSelection,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlList.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlList.PropertyNames.Size,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    }
                };
            dictionary.Add(ControlType.List, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlButton.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Button, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlCheckBox.PropertyNames.Checked,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes2)
                    },
                    {
                        HtmlCheckBox.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlCheckBox.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.CheckBox, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlComboBox.PropertyNames.ItemCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlComboBox.PropertyNames.Items,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlComboBox.PropertyNames.SelectedItem,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlComboBox.PropertyNames.SelectedIndex,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes2)
                    },
                    {
                        HtmlComboBox.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlComboBox.PropertyNames.Size,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    }
                };
            dictionary.Add(ControlType.ComboBox, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlFileInput.PropertyNames.FileName,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlFileInput.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlFileInput.PropertyNames.ReadOnly,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.FileInput, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlHyperlink.PropertyNames.AbsolutePath,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlHyperlink.PropertyNames.Alt,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlHyperlink.PropertyNames.Href,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlHyperlink.PropertyNames.Target,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.Hyperlink, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlRadioButton.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool), attributes2)
                    },
                    {
                        HtmlRadioButton.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlRadioButton.PropertyNames.Group,
                        new ZappyTaskPropertyDescriptor(typeof(ZappyTaskControlCollection),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlRadioButton.PropertyNames.ItemCount,
                        new ZappyTaskPropertyDescriptor(typeof(int), attributes)
                    },
                    {
                        HtmlRadioButton.PropertyNames.LabeledBy,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.RadioButton, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlImage.PropertyNames.Alt,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlImage.PropertyNames.Src,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlImage.PropertyNames.AbsolutePath,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlImage.PropertyNames.LinkAbsolutePath,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlImage.PropertyNames.Href,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.Image, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlScrollBar.PropertyNames.Orientation,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.ScrollBar, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlLabel.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlLabel.PropertyNames.LabelFor,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    }
                };
            dictionary.Add(ControlType.Label, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlDiv.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Pane, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlListItem.PropertyNames.DisplayText,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlListItem.PropertyNames.Selected,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.ListItem, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlFrame.PropertyNames.PageUrl,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlFrame.PropertyNames.AbsolutePath,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlFrame.PropertyNames.Scrollable,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Frame, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlMedia.PropertyNames.AutoPlay,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.CurrentTime,
                        new ZappyTaskPropertyDescriptor(typeof(TimeSpan),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.CurrentSrc,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.CurrentTimeAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Controls,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Duration,
                        new ZappyTaskPropertyDescriptor(typeof(TimeSpan),
                            ZappyTaskPropertyAttributes.NonAssertable | ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.DurationAsString,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Ended,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Loop,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Muted,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Paused,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.PlaybackRate,
                        new ZappyTaskPropertyDescriptor(typeof(float), attributes2)
                    },
                    {
                        HtmlMedia.PropertyNames.Seeking,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.Src,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes)
                    },
                    {
                        HtmlMedia.PropertyNames.Volume,
                        new ZappyTaskPropertyDescriptor(typeof(float), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlMedia.PropertyNames.ReadyState,
                        new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Audio, dictionary2);
            dictionary2.Add(HtmlVideo.PropertyNames.Poster,
                new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable));
            dictionary2.Add(HtmlVideo.PropertyNames.VideoHeight,
                new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable));
            dictionary2.Add(HtmlVideo.PropertyNames.VideoWidth,
                new ZappyTaskPropertyDescriptor(typeof(int), ZappyTaskPropertyAttributes.Readable));
            dictionary.Add(ControlType.Video, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlSlider.PropertyNames.Min,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlSlider.PropertyNames.Max,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlSlider.PropertyNames.ValueAsNumber,
                        new ZappyTaskPropertyDescriptor(typeof(double), attributes2)
                    },
                    {
                        HtmlSlider.PropertyNames.Step,
                        new ZappyTaskPropertyDescriptor(typeof(string), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlSlider.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(string), attributes2)
                    },
                    {
                        HtmlSlider.PropertyNames.Required,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlSlider.PropertyNames.Disabled,
                        new ZappyTaskPropertyDescriptor(typeof(bool), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.Slider, dictionary2);
            dictionary2 =
                new Dictionary<string, ZappyTaskPropertyDescriptor>(commonProperties,
                    StringComparer.OrdinalIgnoreCase)
                {
                    {
                        HtmlProgressBar.PropertyNames.Max,
                        new ZappyTaskPropertyDescriptor(typeof(float), ZappyTaskPropertyAttributes.Readable)
                    },
                    {
                        HtmlProgressBar.PropertyNames.Value,
                        new ZappyTaskPropertyDescriptor(typeof(float), ZappyTaskPropertyAttributes.Readable)
                    }
                };
            dictionary.Add(ControlType.ProgressBar, dictionary2);
            return dictionary;
        }

        protected Dictionary<ControlType, Type> InitializePropertyNameToClassMap() =>
            new Dictionary<ControlType, Type>
            {
                {
                    ControlType.Document,
                    typeof(HtmlDocument.PropertyNames)
                },
                {
                    ControlType.Table,
                    typeof(HtmlTable.PropertyNames)
                },
                {
                    ControlType.Row,
                    typeof(HtmlRow.PropertyNames)
                },
                {
                    ControlType.Cell,
                    typeof(HtmlCell.PropertyNames)
                },
                {
                    ControlType.Edit,
                    typeof(HtmlEdit.PropertyNames)
                },
                {
                    ControlType.List,
                    typeof(HtmlList.PropertyNames)
                },
                {
                    ControlType.ListItem,
                    typeof(HtmlListItem.PropertyNames)
                },
                {
                    ControlType.Button,
                    typeof(HtmlButton.PropertyNames)
                },
                {
                    ControlType.CheckBox,
                    typeof(HtmlCheckBox.PropertyNames)
                },
                {
                    ControlType.ComboBox,
                    typeof(HtmlComboBox.PropertyNames)
                },
                {
                    ControlType.FileInput,
                    typeof(HtmlFileInput.PropertyNames)
                },
                {
                    ControlType.Hyperlink,
                    typeof(HtmlHyperlink.PropertyNames)
                },
                {
                    ControlType.Frame,
                    typeof(HtmlFrame.PropertyNames)
                },
                {
                    ControlType.RadioButton,
                    typeof(HtmlRadioButton.PropertyNames)
                },
                {
                    ControlType.Image,
                    typeof(HtmlImage.PropertyNames)
                },
                {
                    ControlType.ScrollBar,
                    typeof(HtmlScrollBar.PropertyNames)
                },
                {
                    ControlType.Label,
                    typeof(HtmlLabel.PropertyNames)
                },
                {
                    ControlType.Pane,
                    typeof(HtmlDiv.PropertyNames)
                },
                {
                    ControlType.Video,
                    typeof(HtmlVideo.PropertyNames)
                },
                {
                    ControlType.Audio,
                    typeof(HtmlMedia.PropertyNames)
                },
                {
                    ControlType.Slider,
                    typeof(HtmlSlider.PropertyNames)
                },
                {
                    ControlType.ProgressBar,
                    typeof(HtmlProgressBar.PropertyNames)
                }
            };

        internal static bool IsFirefoxBrowserName(string browserName) =>
            browserName.StartsWith("Firefox", StringComparison.OrdinalIgnoreCase);

        protected void SetMediaProperty(ZappyTaskControl uiControl, string propertyName, object value)
        {
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                    }

        public override void SetPropertyValue(ZappyTaskControl uiControl, string propertyName, object value)
        {
                        {
                try
                {
                    TaskActivityElement technologyElement = uiControl.TechnologyElement;
                    if (technologyElement != null)
                    {
                        if (HtmlEdit.PropertyNames.Password.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                        {
                            if (!technologyElement.IsPassword)
                            {
                                throw new ArgumentException(Resources.EncryptedText);
                            }
                            value = EncodeDecode.DecodeString(value as string);
                        }
                        technologyElement.SetPropertyValue(propertyName, value);
                        return;
                    }
                }
                catch (NotImplementedException)
                {
                }
            }
            bool isEncoded = HtmlEdit.PropertyNames.Password.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
            bool useCopyPaste =
                HtmlEdit.PropertyNames.CopyPastedText.Equals(propertyName, StringComparison.OrdinalIgnoreCase);
            ControlType controlType = uiControl.ControlType;
            if ((ControlType.Edit == controlType) &&
                (((HtmlEdit.PropertyNames.Text.Equals(propertyName, StringComparison.OrdinalIgnoreCase) ||
                   ZappyTaskControl.PropertyNames.Value.Equals(propertyName, StringComparison.OrdinalIgnoreCase)) |
                  isEncoded) | useCopyPaste))
            {
                string dataToEncode = ZappyTaskUtilities.ConvertToType<string>(value, false);
                if (HtmlEdit.PropertyNames.Text.Equals(propertyName, StringComparison.OrdinalIgnoreCase) &&
                    uiControl.TechnologyElement.IsPassword)
                {
                    dataToEncode = EncodeDecode.EncodeString(dataToEncode);
                    isEncoded = true;
                }
                if (isEncoded | useCopyPaste)
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiControl, dataToEncode, isEncoded,
                        useCopyPaste);
                }
                else if (!string.IsNullOrEmpty(dataToEncode) &&
                         (dataToEncode.Contains(Environment.NewLine) || dataToEncode.Contains("\n")))
                {
                    dataToEncode = dataToEncode.Replace(Environment.NewLine, "{ENTER}").Replace("\n", "{ENTER}");
                    uiControl.ScreenElement.SendKeysDeleteContent(dataToEncode);
                }
                else
                {
                    TechnologyElementPropertyProvider.SetValueAsEditBox(uiControl, dataToEncode, isEncoded,
                        useCopyPaste);
                }
            }
            else
            {
                bool flag6;
                List<string> list;
                int num4;
                if (!(ControlType.ComboBox == controlType) ||
                    (!HtmlComboBox.PropertyNames.SelectedItem.Equals(propertyName,
                         StringComparison.OrdinalIgnoreCase) &&
                     !ZappyTaskControl.PropertyNames.Value.Equals(propertyName, StringComparison.OrdinalIgnoreCase)))
                {
                    if ((ControlType.ComboBox == controlType) &&
                        HtmlComboBox.PropertyNames.SelectedIndex.Equals(propertyName,
                            StringComparison.OrdinalIgnoreCase))
                    {
                        int num2 = ZappyTaskUtilities.ConvertToType<int>(value);
                        if (!ZappyTaskUtilities.IsIE10OrHigher)
                        {
                            ZappyTaskControlCollection controls =
                                uiControl.GetProperty(HtmlComboBox.PropertyNames.Items) as ZappyTaskControlCollection;
                            if (((num2 >= 0) && (controls != null)) && (num2 < controls.Count))
                            {
                                ZappyTaskControl control = controls[num2];
                                uiControl.ScreenElement.SetValueAsComboBoxUsingQueryId(control.QueryId);
                                return;
                            }
                            object[] objArray1 = new object[]
                                {num2, uiControl.ControlType.Name, HtmlComboBox.PropertyNames.SelectedIndex};
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                                Resources.InvalidParameterValue, objArray1));
                        }
                        this.ThrowIfDisabled(uiControl);
                        this.EnsureVisible(uiControl);
                        IHTMLSelectElement nativeElement = uiControl.NativeElement as IHTMLSelectElement;
                        if ((num2 >= 0) && (num2 < nativeElement.length))
                        {
                            nativeElement.selectedIndex = num2;
                            this.FireOnChange(nativeElement);
                            return;
                        }
                        object[] args = new object[]
                            {num2, uiControl.ControlType.Name, HtmlComboBox.PropertyNames.SelectedIndex};
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Resources.InvalidParameterValue, args));
                    }
                    if (((ControlType.RadioButton == controlType) || (ControlType.CheckBox == controlType)) &&
                        ZappyTaskControl.PropertyNames.State.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        ControlStates state = ZappyTaskUtilities.ConvertToType<ControlStates>(value);
                        TechnologyElementPropertyProvider.SetState(uiControl, state);
                        return;
                    }
                    if ((ControlType.CheckBox == controlType) &&
                        HtmlCheckBox.PropertyNames.Checked.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        bool flag4 = ZappyTaskUtilities.ConvertToType<bool>(value);
                        TechnologyElementPropertyProvider.SetState(uiControl,
                            flag4 ? ControlStates.Checked : (ControlStates.None | ControlStates.Normal));
                        return;
                    }
                    if ((ControlType.RadioButton == controlType) &&
                        HtmlRadioButton.PropertyNames.Selected.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        if (ZappyTaskUtilities.ConvertToType<bool>(value))
                        {
                            TechnologyElementPropertyProvider.SetState(uiControl, ControlStates.Checked);
                            return;
                        }
                        object[] objArray3 = new object[] { bool.FalseString, ControlType.RadioButton };
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Resources.SetStateNotSupportedForControlTypeMessage, objArray3));
                    }
                    if ((ControlType.List == controlType) &&
                        HtmlList.PropertyNames.SelectedItems.Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        string[] values = ZappyTaskUtilities.ConvertToType<string[]>(value, false);
                        if (!ZappyTaskUtilities.IsIE10OrHigher)
                        {
                            TechnologyElementPropertyProvider.SetValueAsListBox(uiControl, values);
                            return;
                        }
                        this.ThrowIfDisabled(uiControl);
                        this.EnsureVisible(uiControl);
                        IHTMLSelectElement element5 = uiControl.NativeElement as IHTMLSelectElement;
                        IHTMLElementCollection elements2 = element5.tags("option") as IHTMLElementCollection;
                        HashSet<string> set = new HashSet<string>();
                        if (values != null)
                        {
                            foreach (string str3 in values)
                            {
                                set.Add(str3);
                            }
                        }
                        foreach (IHTMLOptionElement element6 in elements2)
                        {
                            if (set.Contains(element6.text))
                            {
                                element6.selected = true;
                                set.Remove(element6.text);
                            }
                            else
                            {
                                element6.selected = false;
                            }
                        }
                        if (set.Count != 0)
                        {
                            new CommaListBuilder().AddRange((IEnumerable<string>)set);
                                                                                }
                        return;
                    }
                    if (!(ControlType.List == controlType) ||
                        !HtmlList.PropertyNames.SelectedIndices.Equals(propertyName, StringComparison.OrdinalIgnoreCase)
                    )
                    {
                        if ((ControlType.List == controlType) &&
                            (HtmlList.PropertyNames.SelectedItemsAsString.Equals(propertyName,
                                 StringComparison.OrdinalIgnoreCase) ||
                             ZappyTaskControl.PropertyNames.Value.Equals(propertyName,
                                 StringComparison.OrdinalIgnoreCase)))
                        {
                            string str4 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                            string[] strArray3 = null;
                            if (!string.IsNullOrEmpty(str4))
                            {
                                strArray3 = CommaListBuilder.GetCommaSeparatedValues(str4).ToArray();
                            }
                            this.SetPropertyValue(uiControl, HtmlList.PropertyNames.SelectedItems, strArray3);
                            return;
                        }
                        if ((ControlType.FileInput == controlType) &&
                            (HtmlFileInput.PropertyNames.FileName.Equals(propertyName,
                                 StringComparison.OrdinalIgnoreCase) ||
                             ZappyTaskControl.PropertyNames.Value.Equals(propertyName,
                                 StringComparison.OrdinalIgnoreCase)))
                        {
                            if (!string.IsNullOrEmpty(value as string))
                            {
                                ThrowExceptionIfReadOnly();
                                this.SetValueOnFileInputControl(uiControl, value);
                                return;
                            }
                        }
                        else
                        {
                            if ((ControlType.Video == controlType) || (ControlType.Audio == controlType))
                            {
                                this.SetMediaProperty(uiControl, propertyName, value);
                                return;
                            }
                            if (ControlType.Slider == controlType)
                            {
                                this.SetSliderProperty(uiControl, propertyName, value);
                                return;
                            }
                            object[] objArray6 = new object[] { propertyName, uiControl.ControlType.Name };
                            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                                Resources.SetPropertyNotSupportedMessage, objArray6));
                        }
                        return;
                    }
                    int[] numArray = ZappyTaskUtilities.ConvertToType<int[]>(value, false);
                    if (ZappyTaskUtilities.IsIE10OrHigher)
                    {
                        this.ThrowIfDisabled(uiControl);
                        this.EnsureVisible(uiControl);
                        IHTMLSelectElement element7 = uiControl.NativeElement as IHTMLSelectElement;
                        IHTMLElementCollection elements3 = element7.tags("option") as IHTMLElementCollection;
                        HashSet<int> collection = new HashSet<int>();
                        if (numArray != null)
                        {
                            foreach (int num9 in numArray)
                            {
                                collection.Add(num9);
                            }
                        }
                        int item = 0;
                        foreach (IHTMLOptionElement element8 in elements3)
                        {
                            if (collection.Contains(item))
                            {
                                element8.selected = true;
                                collection.Remove(item);
                            }
                            else
                            {
                                element8.selected = false;
                            }
                            item++;
                        }
                        if (collection.Count == 0)
                        {
                            return;
                        }
                        CommaListBuilder builder2 = new CommaListBuilder();
                        builder2.AddRange(collection);
                        object[] objArray5 = new object[]
                            {builder2.ToString(), uiControl.ControlType.Name, HtmlList.PropertyNames.SelectedIndices};
                        throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                            Resources.InvalidParameterValue, objArray5));
                    }
                    ZappyTaskControlCollection property =
                        uiControl.GetProperty(HtmlComboBox.PropertyNames.Items) as ZappyTaskControlCollection;
                    flag6 = true;
                    list = new List<string>();
                    num4 = ((numArray != null) && (numArray.Length != 0)) ? numArray[0] : -1;
                    if (numArray != null)
                    {
                        if (property != null)
                        {
                            foreach (int num6 in numArray)
                            {
                                if ((num6 >= 0) && (num6 < property.Count))
                                {
                                    ZappyTaskControl control2 = property[num6];
                                    list.Add(control2.QueryId);
                                }
                                else
                                {
                                    flag6 = false;
                                    num4 = num6;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            flag6 = false;
                        }
                    }
                }
                else
                {
                    string str2 = ZappyTaskUtilities.ConvertToType<string>(value, false);
                    if (!ZappyTaskUtilities.IsIE10OrHigher)
                    {
                        TechnologyElementPropertyProvider.SetValueAsComboBox(uiControl, str2, false);
                        return;
                    }
                    this.ThrowIfDisabled(uiControl);
                    this.EnsureVisible(uiControl);
                    IHTMLSelectElement domNode = uiControl.NativeElement as IHTMLSelectElement;
                    IHTMLElementCollection elements = domNode.tags("option") as IHTMLElementCollection;
                    int num = 0;
                    bool flag3 = false;
                    foreach (IHTMLOptionElement element3 in elements)
                    {
                        if (string.Equals(element3.text, str2, StringComparison.Ordinal))
                        {
                            flag3 = true;
                            break;
                        }
                        num++;
                    }
                    if (!flag3)
                    {
                                                                    }
                    domNode.selectedIndex = num;
                    this.FireOnChange(domNode);
                    return;
                }
                if (flag6)
                {
                    uiControl.ScreenElement.SetValueAsListBox(list.ToArray(), true);
                }
                else
                {
                    object[] objArray4 = new object[]
                        {num4, uiControl.ControlType.Name, HtmlList.PropertyNames.SelectedIndices};
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture,
                        Resources.InvalidParameterValue, objArray4));
                }
            }
        }

        protected void SetSliderProperty(ZappyTaskControl uiControl, string propertyName, object value)
        {
            object[] args = new object[] { propertyName };
                        this.ThrowIfDisabled(uiControl);
            string s = ZappyTaskUtilities.ConvertToType<string>(value);
            float result = -1f;
            float num2 = 0f;
            float num3 = 100f;
            if (!float.TryParse(s, out result))
            {
                object[] objArray2 = new object[] { value, propertyName, UIControl.ControlType };
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.InvalidParameterValue,
                    objArray2));
            }
            bool flag = float.TryParse((string)uiControl.GetProperty(HtmlSlider.PropertyNames.Min), out num2);
            if (flag)
            {
                flag = float.TryParse((string)uiControl.GetProperty(HtmlSlider.PropertyNames.Max), out num3);
            }
            if (flag && ((result < num2) || (result > num3)))
            {
                object[] objArray3 = new object[] { result, uiControl.ControlType.Name, propertyName };
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture,
                    Resources.InvalidParameterValue, objArray3));
            }
            Mouse.Hover(uiControl);
            IHTMLInputElement nativeElement = uiControl.NativeElement as IHTMLInputElement;
            if (nativeElement != null)
            {
                nativeElement.value = s;
                this.FireOnChange(nativeElement);
            }
            else
            {
                                object[] objArray4 = new object[] { propertyName, uiControl.ControlType.Name };
                throw new Exception(string.Format(CultureInfo.CurrentCulture, Resources.SetPropertyFailed,
                    objArray4));
            }
        }

        private void SetValueOnFileInputControl(ZappyTaskControl uiControl, object value)
        {
            this.ThrowIfDisabled(uiControl);
            Point point = new Point(uiControl.BoundingRectangle.Width - 5, 5);
            try
            {
                uiControl.EnsureClickable(point);
            }
            catch (Exception)
            {
                if (!uiControl.TryGetClickablePoint(out point))
                {
                    throw;
                }
            }
            Mouse.Click(uiControl, point);
            string iEFileUploadDialogTitle = string.Empty;
                                                                        {
                                                                                iEFileUploadDialogTitle = LocalizedSystemStrings.Instance.FireFoxFileUploadDialogTitle;
            }
            ZappyTaskControl searchLimitContainer = new ZappyTaskControl
            {
                TechnologyName = "MSAA"
            };
            string[] nameValuePairs = new string[]
            {
                ZappyTaskControl.PropertyNames.Name, iEFileUploadDialogTitle, ZappyTaskControl.PropertyNames.ClassName,
                "#32770", ZappyTaskControl.PropertyNames.ControlType, ControlType.Window.Name
            };
            searchLimitContainer.SearchProperties.Add(nameValuePairs);
            searchLimitContainer.SearchConfigurations.Add(SearchConfiguration.VisibleOnly);
            ZappyTaskControl control2 = new ZappyTaskControl(searchLimitContainer)
            {
                TechnologyName = "MSAA"
            };
            string[] textArray2 = new string[]
            {
                ZappyTaskControl.PropertyNames.Name, LocalizedSystemStrings.Instance.FileUploadComboBoxName,
                ZappyTaskControl.PropertyNames.ControlType, ControlType.ComboBox.Name
            };
            control2.SearchProperties.Add(textArray2);
            control2.SearchConfigurations.Add(SearchConfiguration.VisibleOnly);
            control2.Find();
            TechnologyElementPropertyProvider.SetValueAsComboBox(control2, value as string, true);
            Keyboard.SendKeys("{Enter}");
        }

        protected void ThrowIfDisabled(ZappyTaskControl uiControl)
        {
            try
            {
                if (!((bool)uiControl.GetProperty("Enabled")))
                {
                                    }
            }
            catch (NotSupportedException)
            {
            }
        }
    }
}