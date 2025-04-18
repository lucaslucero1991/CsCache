using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CSCache.Model
{
    public class DeviceImageSettings
    {
        public int Id { get; set; }
        public string ImageTypeCode { get; set; }
        public Nullable<int> DesktopWidth { get; set; }
        public Nullable<int> TabletWidth { get; set; }
        public Nullable<int> SmartphoneWidth { get; set; }
        public Nullable<int> CellphoneWidth { get; set; }
    }
}