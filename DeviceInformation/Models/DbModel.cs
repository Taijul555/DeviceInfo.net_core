using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace DeviceInformation.Models
{
    public enum DeviceType { Mobile=1, Tab, DualPC, PocketPc}
    public class CommonSpecification
    {
        public int CommonSpecificationId { get; set; }
        public string CommonSpecificationName { get; set;}
    }
    public class Device
    {
        public int DeviceId { get; set; }
        [Required, StringLength(50), Display(Name ="Device name")]
        public string DeviceName { get; set; }
        [Required, Column(TypeName ="date"), Display(Name ="Release date"), DisplayFormat(DataFormatString ="{0:yyyy-MM-dd")]
        public DateTime ReleaseDate { get; set; } = DateTime.Today;
        [Required, Column(TypeName = "money"), DisplayFormat(DataFormatString = "{0:0.00")]
        public decimal Price { get; set; }
        [Required, EnumDataType(typeof(DeviceType))]
        public DeviceType DeviceType { get; set; }
        public bool Discontued { get; set; }
        [Required, StringLength(50)]
        public string Picture { get; set; }
        public virtual ICollection<Specification> Specifications { get; set; }= new List<Specification>();
    }
    public class Specification
    {
        public int SpecificationId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        [Required, ForeignKey(nameof(Device))]
        public int DeviceId { get; set; }
        public virtual Device Device { get; set; }
    }
    public class DeviceDbContext : DbContext
    {
        public DeviceDbContext()
        {
            Database.SetInitializer(new DbInitializer());
        }
        public DbSet<CommonSpecification> CommonSpecifications { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Specification> Specifications { get; set; }
    }
    public class DbInitializer:DropCreateDatabaseIfModelChanges<DeviceDbContext>
    {
        protected override void Seed(DeviceDbContext db)
        {
            db.CommonSpecifications.AddRange(new CommonSpecification[]
            {
                new CommonSpecification{CommonSpecificationName="RAM"},
                new CommonSpecification{CommonSpecificationName="Storage"},
                new CommonSpecification{CommonSpecificationName="Battery"},
                new CommonSpecification{CommonSpecificationName="Processor"}
            });
            
            Device d = new Device { DeviceName = "Xiaomi M10", DeviceType = DeviceType.Mobile, Discontued = false, Price = 17000.00M, ReleaseDate = new DateTime(2022, 9, 12), Picture="d1.jpg" };
            d.Specifications.Add(new Specification { Name = "RAM", Value = "4GB" });
            d.Specifications.Add(new Specification { Name = "Storage", Value = "64GB" });
            db.Devices.Add(d);
            db.SaveChanges();
        }
    }
}