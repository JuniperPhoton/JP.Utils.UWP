using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JP.Utils.Framework
{
    public interface INavigable
    {
        /// <summary>
        /// OnNavigatedTo 时触发
        /// </summary>
        /// <param name="param"></param>
        void Activate(object param);

        /// <summary>
        /// OnNavigatedFrom 时触发
        /// </summary>
        /// <param name="param"></param>
        void Deactivate(object param);

        void OnLoaded();

        bool IsInView { get; set; }

        bool IsFirstActived { get; set; }
    }
}
