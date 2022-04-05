using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Zappy.ZappyActions.Triggers.Helpers
{
    
        public class FlagEnumUIEditor : UITypeEditor
    {
        class FlagCheckedListBoxItem
        {
            public FlagCheckedListBoxItem(int v, string c)
            {
                value = v;
                caption = c;
            }

            public override string ToString()
            {
                return caption;
            }

                        public bool IsFlag
            {
                get
                {
                    return ((value & (value - 1)) == 0);
                }
            }

                        public bool IsMemberFlag(FlagCheckedListBoxItem composite)
            {
                return (IsFlag && ((value & composite.value) == value));
            }

            public int value;
            public string caption;
        }
        class FlagCheckedListBox : CheckedListBox
        {
            private System.ComponentModel.Container components = null;

            public FlagCheckedListBox()
            {
                                InitializeComponent();

                
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (components != null)
                        components.Dispose();
                }
                base.Dispose(disposing);
            }

            #region Component Designer generated code
            private void InitializeComponent()
            {
                                                                this.CheckOnClick = true;

            }
            #endregion

                        public FlagCheckedListBoxItem Add(int v, string c)
            {
                FlagCheckedListBoxItem item = new FlagCheckedListBoxItem(v, c);
                Items.Add(item);
                return item;
            }

            public FlagCheckedListBoxItem Add(FlagCheckedListBoxItem item)
            {
                Items.Add(item);
                return item;
            }

            protected override void OnItemCheck(ItemCheckEventArgs e)
            {
                base.OnItemCheck(e);

                if (isUpdatingCheckStates)
                    return;

                                FlagCheckedListBoxItem item = Items[e.Index] as FlagCheckedListBoxItem;
                                UpdateCheckedItems(item, e.NewValue);
            }

                        protected void UpdateCheckedItems(int value)
            {

                isUpdatingCheckStates = true;

                                for (int i = 0; i < Items.Count; i++)
                {
                    FlagCheckedListBoxItem item = Items[i] as FlagCheckedListBoxItem;

                    if (item.value == 0)
                    {
                        SetItemChecked(i, value == 0);
                    }
                    else
                    {

                                                if ((item.value & value) == item.value && item.value != 0)
                            SetItemChecked(i, true);
                                                else
                            SetItemChecked(i, false);
                    }
                }

                isUpdatingCheckStates = false;

            }

                                                protected void UpdateCheckedItems(FlagCheckedListBoxItem composite, CheckState cs)
            {

                                if (composite.value == 0)
                    UpdateCheckedItems(0);


                                int sum = 0;
                for (int i = 0; i < Items.Count; i++)
                {
                    FlagCheckedListBoxItem item = Items[i] as FlagCheckedListBoxItem;

                                        if (GetItemChecked(i))
                        sum |= item.value;
                }

                                if (cs == CheckState.Unchecked)
                    sum = sum & (~composite.value);
                                else
                    sum |= composite.value;

                                UpdateCheckedItems(sum);

            }

            private bool isUpdatingCheckStates = false;

                        public int GetCurrentValue()
            {
                int sum = 0;

                for (int i = 0; i < Items.Count; i++)
                {
                    FlagCheckedListBoxItem item = Items[i] as FlagCheckedListBoxItem;

                    if (GetItemChecked(i))
                        sum |= item.value;
                }

                return sum;
            }

            Type enumType;
            Enum enumValue;

                        private void FillEnumMembers()
            {
                foreach (string name in Enum.GetNames(enumType))
                {
                    object val = Enum.Parse(enumType, name);
                    int intVal = (int)Convert.ChangeType(val, typeof(int));

                    Add(intVal, name);
                }
            }

                        private void ApplyEnumValue()
            {
                int intVal = (int)Convert.ChangeType(enumValue, typeof(int));
                UpdateCheckedItems(intVal);

            }

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Enum EnumValue
            {
                get
                {
                    object e = Enum.ToObject(enumType, GetCurrentValue());
                    return (Enum)e;
                }
                set
                {

                    Items.Clear();
                    enumValue = value;                     enumType = value.GetType();                     FillEnumMembers();                     ApplyEnumValue(); 
                }
            }


        }

                private FlagCheckedListBox flagEnumCB;

        public FlagEnumUIEditor()
        {
            flagEnumCB = new FlagCheckedListBox();
            flagEnumCB.BorderStyle = BorderStyle.None;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context != null
                && context.Instance != null
                && provider != null)
            {

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {

                    Enum e = (Enum)Convert.ChangeType(value, context.PropertyDescriptor.PropertyType);
                    flagEnumCB.EnumValue = e;
                    edSvc.DropDownControl(flagEnumCB);
                    return flagEnumCB.EnumValue;

                }
            }
            return null;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }


    }
}

