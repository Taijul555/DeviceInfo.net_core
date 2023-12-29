using DeviceInformation.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace DeviceInformation.ViewModels.Input
{
    public class DeviceInputModel
    {
        public int DeviceId { get; set; }
        [Required, StringLength(50), Display(Name = "Device name")]
        public string DeviceName { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "Release date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd")]
        public DateTime ReleaseDate { get; set; }
        [Required, Column(TypeName = "money"), DisplayFormat(DataFormatString = "{0:0.00")]
        public decimal Price { get; set; }
        [Required, EnumDataType(typeof(DeviceType))]
        public DeviceType DeviceType { get; set; }
        [Required]
        public HttpPostedFileBase Picture { get; set; }
        public bool Discontued { get; set; }
       
        public virtual List<Specification> Specifications { get; set; } = new List<Specification>();
    }
}