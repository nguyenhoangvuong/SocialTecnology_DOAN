using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FivePSocialNetwork.Models;
using FivePSocialNetwork.Models.Json;

namespace FivePSocialNetwork.Areas.Admin.Controllers
{
    public class AddressAdminController : Controller
    {
        FivePSocialNetWorkEntities db = new FivePSocialNetWorkEntities();
        // GET: Admin/AddressAdmin
        public ActionResult Provincial()
        {
            return View();
        }
        public JsonResult ProvincialJson()
        {
            List<Provincial> provincials = db.Provincials.Where(n => n.provincial_recycleBin == false).ToList();
            List<ProvincialAdmin> provincialAdmins = provincials.Select(n => new ProvincialAdmin
            {
                provincial_id = n.provincial_id,
                provincial_name = n.provincial_name,
                provincial_activate = n.provincial_activate,
                provincial_dateCreate = n.provincial_dateCreate.ToString(),
                provincial_dateEdit = n.provincial_dateEdit.ToString(),
                provincial_recycleBin = n.provincial_recycleBin

            }).OrderBy(n=>n.provincial_name).ToList();
            return Json(provincialAdmins, JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecycleBinProvincialJson()
        {
            List<Provincial> provincials = db.Provincials.Where(n => n.provincial_recycleBin == true).ToList();
            List<ProvincialAdmin> provincialAdmins = provincials.Select(n => new ProvincialAdmin
            {
                provincial_id = n.provincial_id,
                provincial_name = n.provincial_name,
                provincial_activate = n.provincial_activate,
                provincial_dateCreate = n.provincial_dateCreate.ToString(),
                provincial_dateEdit = n.provincial_dateEdit.ToString(),
                provincial_recycleBin = n.provincial_recycleBin

            }).ToList();
            return Json(provincialAdmins, JsonRequestBehavior.AllowGet);
        }
        //xóa tạm thời tỉnh
        public JsonResult RecycleBinProvincial(int? id)
        {
            Provincial provincial = db.Provincials.Find(id);
            provincial.provincial_recycleBin = !provincial.provincial_recycleBin;
            db.SaveChanges();
            return Json(provincial,JsonRequestBehavior.AllowGet);
        }
        //hoạt động tỉnh
        public JsonResult ActivateProvincail(int? id)
        {
            Provincial provincial = db.Provincials.Find(id);
            provincial.provincial_activate = !provincial.provincial_activate;
            db.SaveChanges();
            return Json(provincial, JsonRequestBehavior.AllowGet);
        }
        // thêm tỉnh
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult CreateProvincail([Bind(Include = "provincial_id,provincial_name,provincial_activate,provincial_dateCreate,provincial_dateEdit,provincial_recycleBin")] Provincial provincial)
        {
            provincial.provincial_activate = true;
            provincial.provincial_dateCreate = DateTime.Now;
            provincial.provincial_dateEdit = DateTime.Now;
            provincial.provincial_recycleBin = false;
            db.Provincials.Add(provincial);
            db.SaveChanges();
            return RedirectToAction("Provincial");
        }
        // sửa tỉnh
        public ActionResult EditProvincial(string provincial_name, int? provincial_id)
        {
            Provincial provincial = db.Provincials.Find(provincial_id);
            provincial.provincial_name = provincial_name;
            db.SaveChanges();
            return RedirectToAction("Provincial");
        }
        //--------------------------phần Huyện ---------------------------------------
        public ActionResult District()
        {
            return View();
        }
        // hiển thị danh sách huyện
        public JsonResult DistrictJson()
        {
            List<District> districts = db.Districts.Where(n => n.district_recycleBin == false).ToList();
            List<DistrictAdmin> districtAdmins = districts.Select(n => new DistrictAdmin
            {
                district_id = n.district_id,
                district_name = n.district_name,
                district_activate = n.district_activate,
                district_dateCreate = n.district_dateCreate.ToString(),
                district_dateEdit = n.district_dateEdit.ToString(),
                district_recycleBin = n.district_recycleBin,
                provincial_id = n.provincial_id,
                provincial_name = n.Provincial.provincial_name

            }).OrderBy(n=>n.provincial_name).ToList();
            return Json(districtAdmins, JsonRequestBehavior.AllowGet);
        }
        //Thêm huyện
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult CreateDistrict([Bind(Include = "district_id,district_name,district_activate,district_dateCreate,district_dateEdit,district_recycleBin,provincial_id")] District district)
        {
            district.district_activate = true;
            district.district_dateCreate = DateTime.Now;
            district.district_dateEdit = DateTime.Now;
            district.district_recycleBin = false;
            db.Districts.Add(district);
            db.SaveChanges();
            return RedirectToAction("District");
        }
        public JsonResult ActivateDistrict(int? id)
        {
            District district = db.Districts.Find(id);
            district.district_activate = !district.district_activate;
            db.SaveChanges();
            return Json(district,JsonRequestBehavior.AllowGet);
        }
        public JsonResult RecycleBinDistrict(int? id)
        {
            District district = db.Districts.Find(id);
            district.district_recycleBin = !district.district_recycleBin;
            db.SaveChanges();
            return Json(district, JsonRequestBehavior.AllowGet);
        }
        // danh sách đã xóa
        public JsonResult RecycleBinDistrictJson()
        {
            List<District> districts = db.Districts.Where(n => n.district_recycleBin == true).ToList();
            List<DistrictAdmin> districtAdmins = districts.Select(n => new DistrictAdmin
            {
                district_id = n.district_id,
                district_name = n.district_name,
                district_activate = n.district_activate,
                district_dateCreate = n.district_dateCreate.ToString(),
                district_dateEdit = n.district_dateEdit.ToString(),
                district_recycleBin = n.district_recycleBin,
                provincial_id = n.provincial_id,
                provincial_name = n.Provincial.provincial_name

            }).ToList();
            return Json(districtAdmins, JsonRequestBehavior.AllowGet);
        }
        public ActionResult EditDistrict(int? provincial_id,string district_name, int? district_id)
        {
            District district = db.Districts.Find(district_id);
            district.district_name = district_name;
            district.provincial_id = provincial_id;
            db.SaveChanges();
            return RedirectToAction("District");
        }
        //--------------------------phần  Xã ---------------------------------------

        public ActionResult Commune()
        {
            return View();
        }
        // hiển thị
        public JsonResult CommuneJson()
        {
            List<Commune> communes = db.Communes.Where(n => n.commune_recycleBin == false).ToList();
            List<ComuneAdmin> comuneAdmins = communes.Select(n => new ComuneAdmin
            {
                commune_id = n.commune_id,
                commune_name = n.commune_name,
                commune_activate = n.commune_activate,
                commune_dateCreate = n.commune_dateCreate.ToString(),
                commune_dateEdit = n.commune_dateEdit.ToString(),
                commune_recycleBin = n.commune_recycleBin,
                district_id = n.district_id,
                district_name = n.District.district_name

            }).ToList();
            return Json(comuneAdmins, JsonRequestBehavior.AllowGet);
        }
        //Thêm -----------
        public ActionResult CreateCommune([Bind(Include = "commune_id,commune_name,commune_activate,commune_dateCreate,commune_dateEdit,commune_recycleBin,district_id")] Commune commune, string dsxa)
        {
            //commune.commune_activate = true;
            //commune.commune_dateCreate = DateTime.Now;
            //commune.commune_dateEdit = DateTime.Now;
            //commune.commune_recycleBin = false;
            //db.Communes.Add(commune);
            string[] xa = dsxa.Split(',');
            foreach(var item in xa)
            {
                db.Communes.Add(new Commune
                {
                    commune_activate = true,
                    commune_recycleBin = false,
                    commune_dateCreate = DateTime.Now,
                    commune_dateEdit = DateTime.Now,
                    commune_name = item,
                    district_id = commune.district_id
                });
            }
            db.SaveChanges();
            return RedirectToAction("Commune");
        }
        // Sửa----------
        public ActionResult EditCommune(string commune_name, int? commune_id, int? district_id)
        {
            Commune commune = db.Communes.Find(commune_id);
            commune.commune_name = commune_name;
            commune.district_id = district_id;
            db.SaveChanges();
            return RedirectToAction("Commune");
        }
    }
}