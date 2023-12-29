using DeviceInformation.Models;
using DeviceInformation.ViewModels.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using X.PagedList;
using X.PagedList.Mvc;
using static System.Collections.Specialized.BitVector32;

namespace DeviceInformation.Controllers
{
    [Authorize]
    public class DevicesController : Controller
    {
        private readonly DeviceDbContext db = new DeviceDbContext();
        // GET: Devices
        //public ActionResult Index(int page=1)
        //{
           
        //    return View(db.Devices.OrderBy(d => d.DeviceId).ToPagedList(page, 5));
        //}
        public async Task<ActionResult> Index(int page = 1)
        {
            var data = await db.Devices.OrderBy(a => a.DeviceId).ToPagedListAsync(page, 5);
            return View(data);
        }
        public ActionResult Create()
        {
            ViewBag.CommonSpecs = db.CommonSpecifications.Select(c => c.CommonSpecificationName).ToList();
            var d = new DeviceInputModel();
            d.Specifications.Add(new Specification());
            return View(d);
        }
        [HttpPost]
        public ActionResult Create(DeviceInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Specifications.Add(new Specification());
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                var index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Specifications.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var device = new Device
                    {
                        DeviceName = model.DeviceName,
                        DeviceType = model.DeviceType,
                        ReleaseDate = model.ReleaseDate,
                        Price = model.Price,
                        Discontued = model.Discontued
                        

                    };
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), fileName);
                    model.Picture.SaveAs(savePath);
                    device.Picture = fileName;
                   
                    foreach (var sp in model.Specifications)
                    {
                        device.Specifications.Add(new Specification { Name= sp.Name, Value=sp.Value});
                        if (!db.CommonSpecifications.Any(cs => sp.Name.ToLower() == cs.CommonSpecificationName))
                        {
                            db.CommonSpecifications.Add(new CommonSpecification { CommonSpecificationName = sp.Name });
                        }
                    }
                    db.Devices.Add(device);
                    
                    db.SaveChanges();
                    model = new DeviceInputModel();
                    model.Specifications.Add(new Specification());
                }
            }
            ViewData["Act"] = act;
            
            Thread.Sleep(500);
            return PartialView("_CreatePartial", model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.CommonSpecs = db.CommonSpecifications.Select(c => c.CommonSpecificationName).ToList();
            var d = db.Devices.FirstOrDefault(x => x.DeviceId == id);
            if (d == null) return new HttpNotFoundResult();
            var model = new DeviceEditModel
            {
                DeviceId = d.DeviceId,
                DeviceName = d.DeviceName,
                DeviceType = d.DeviceType,
                Price = d.Price,
                ReleaseDate = d.ReleaseDate,
                Discontued = d.Discontued,
                
               
            };
            ViewData["CurrentPic"] = d.Picture;
            model.Specifications = d.Specifications.ToList();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(DeviceEditModel model, string act = "")
        {
            var device = db.Devices.FirstOrDefault(x => x.DeviceId == model.DeviceId);
            if (device == null) return new HttpNotFoundResult();
            if (act == "add")
            {
                model.Specifications.Add(new Specification());
                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act.StartsWith("remove"))
            {
                var index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Specifications.RemoveAt(index);

                foreach (var v in ModelState.Values)
                {
                    v.Errors.Clear();
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    device.DeviceId = model.DeviceId;
                    device.DeviceName = model.DeviceName;
                    device.DeviceType = model.DeviceType;
                    device.ReleaseDate = model.ReleaseDate;
                    device.Price = model.Price;
                    device.Discontued = model.Discontued;
                    if(model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Pictures"), fileName);
                        model.Picture.SaveAs(savePath);
                        device.Picture = fileName;
                    }
                    var olds = db.Specifications.Where(x => x.DeviceId == model.DeviceId).ToList();
                        
                    for(var i=0; i < olds.Count; i++)
                    {
                        db.Specifications.Remove(olds[i]);
                        device.Specifications.Remove(olds[i]);
                    }
                        
                    foreach(var sp in model.Specifications)
                    {
                        device.Specifications.Add(new Specification { Name = sp.Name, Value = sp.Value });
                    }
                   
                   

                   

                    db.SaveChanges();
                    
                   
                }
            }
            ViewData["Act"] = act;

            ViewData["CurrentPic"] = device.Picture;
            model.Specifications = device.Specifications.ToList();
            return PartialView("_EditPartial", model);
        }
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var d = db.Devices.FirstOrDefault(x=> x.DeviceId == id);
            if (d == null) return new HttpNotFoundResult();
            db.Devices.Remove(d);
            db.SaveChanges();
            return Json(new {success=true, id});
        }
    }
    
}
