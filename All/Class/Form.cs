using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Reflection;
namespace All.Class
{
    public class Form
    {
        delegate object GetControlInfoHandle(System.Windows.Forms.Control control, string Info);
        delegate void SetControlInfoHandle(System.Windows.Forms.Control control, string Info, object value);

        public static object GetControlInfo(System.Windows.Forms.Control control, string Info)
        {
            object value = null;
            if (control.InvokeRequired)
            {
                value = control.Invoke(new GetControlInfoHandle(GetControlInfo), control, Info);
            }
            else
            {
                foreach (PropertyInfo pi in typeof(System.Windows.Forms.Control).GetProperties())
                {
                    if (pi.Name.ToUpper() == Info.ToUpper())
                    {
                        return pi.GetValue(control, null);
                    }
                }
                foreach (FieldInfo fi in typeof(System.Windows.Forms.Control).GetFields())
                {
                    if (fi.Name.ToUpper() == Info.ToUpper())
                    {
                        return fi.GetValue(control);
                    }
                }
            }
            return value;
        }


        public static void SetControlInfo(System.Windows.Forms.Control control, string Info, object value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new SetControlInfoHandle(SetControlInfo), control, Info, value);
            }
            else
            {
                foreach (PropertyInfo pi in typeof(System.Windows.Forms.Control).GetProperties())
                {
                    if (pi.Name.ToUpper() == Info.ToUpper())
                    {
                        pi.SetValue(control, value, null);
                        return;
                    }
                }
                foreach (FieldInfo fi in typeof(System.Windows.Forms.Control).GetFields())
                {
                    if (fi.Name.ToUpper() == Info.ToUpper())
                    {
                        fi.SetValue(control, value);
                        return;
                    }
                }
            }
        }
    }
}
