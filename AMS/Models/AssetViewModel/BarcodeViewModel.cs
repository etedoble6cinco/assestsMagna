using System;
using System.Collections.Generic;

namespace AMS.Models.AssetViewModel
{
    public class BarcodeViewModel
    {
        public Int64 Id { get; set; }
        public string AssetName { get; set; }
        public string Barcode { get; set; }
        public List<BarcodeViewModel> listBarcodeViewModel { get; set; }
        public List<BarcodeViewModel> sublistBarcodeViewModel { get; set; }
    }
}
