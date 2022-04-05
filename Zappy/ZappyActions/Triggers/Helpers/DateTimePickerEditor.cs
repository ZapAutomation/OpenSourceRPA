using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Zappy.ZappyActions.Triggers.Helpers
{
    [Description("Editor Of DateTime Picker")]
    public class DateTimePickerEditor : UITypeEditor
    {

                IWindowsFormsEditorService editorService;
        DateTimePicker picker = new DateTimePicker();

        public DateTimePickerEditor()
        {
            picker.Format = DateTimePickerFormat.Custom;
            picker.CustomFormat = "HH:mm:ss";
            picker.ShowUpDown = true;
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                this.editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (this.editorService != null)
            {
                DateTime _TriggerFireTime_Parsed = DateTime.Now;
                if (value == null || string.IsNullOrEmpty(value.ToString()))
                    _TriggerFireTime_Parsed = DateTime.Now;
                else if (!DateTime.TryParseExact(value.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _TriggerFireTime_Parsed))
                    _TriggerFireTime_Parsed = DateTime.Now;


                picker.Value = _TriggerFireTime_Parsed;
                this.editorService.DropDownControl(picker);
                value = picker.Value.ToString("HH:mm:ss");
            }

            return value;
        }
    }

}

