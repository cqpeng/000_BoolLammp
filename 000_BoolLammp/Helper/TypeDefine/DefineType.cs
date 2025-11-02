using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _000_BoolLammp.Helper.TypeDefine
{
    public enum Mode
    {
        [Description("单灯")]
        Single,
        [Description("单灯循环")]
        SingleLoop,
        [Description("转向灯")]
        DirectionShow, 
        [Description("中间向两边")]
        TowDirectionShow
    }
}
