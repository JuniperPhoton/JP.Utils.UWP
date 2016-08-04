using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JP.Utils.Control
{
    public class CalendarDatePickerEx: CalendarDatePicker
    {
        public DateTime SelectedDateTime
        {
            get { return (DateTime)GetValue(SelectedDateTimeProperty); }
            set { SetValue(SelectedDateTimeProperty, value); }
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register("SelectedDateTime", typeof(DateTime), typeof(CalendarDatePickerEx), new PropertyMetadata(DateTime.Now));

        public CalendarDatePickerEx():base()
        {
            DateChanged += CalendarDatePickerEx_DateChanged;
        }

        private void CalendarDatePickerEx_DateChanged(CalendarDatePicker sender, CalendarDatePickerDateChangedEventArgs args)
        {
            if(this.Date.HasValue)
            {
                this.SelectedDateTime = sender.Date.Value.DateTime;
                this.PlaceholderText= sender.Date.Value.DateTime.ToString("yyyy-MM-dd");
            }
        }
    }
}
