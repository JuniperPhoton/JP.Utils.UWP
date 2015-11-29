using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace JP.CustomTriggers
{
    public class BoolTypeTrigger: StateTriggerBase
    {
        private bool _boolValue;
        public bool BoolValue
        {
            get
            {
                return _boolValue;
            }
            set
            {
                if(_boolValue!=value)
                {
                    _boolValue = value;
                    UpdateTrigger();
                }
            }
        }

        private bool _srcValue;
        public bool SrcValue
        {
            get
            {
                return _srcValue;
            }
            set
            {
                if (_srcValue != value)
                {
                    _srcValue = value;
                    UpdateTrigger();
                }
            }
        }

        private void UpdateTrigger()
        {
            if (BoolValue)
            {
                SetActive(SrcValue);
            }
            else
            {
                SetActive(!SrcValue);
            }
        }
    }
}
