using ECommerce.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECommerce.Controllers
{
    public class AdminController : Controller
    {
        dbemarketingEntities db = new dbemarketingEntities();
        // GET: Admin
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult login(tbl_admin avm)
        {
            tbl_admin ad = db.tbl_admin.Where(x => x.ad_username == avm.ad_username && x.ad_password == avm.ad_password).SingleOrDefault();
            if (ad != null)
            {
                Session["ad_id"] = ad.ad_id.ToString();
                return RedirectToAction("Create");
            }
            else { ViewBag.error = "Nume sau parola invalide"; }
            return View();
        }


        [HttpGet]
        public ActionResult Create()
        {
            if (Session["ad_id"] == null)
            {
                return RedirectToAction("login");
            }
            return View();
        }



        [HttpPost]
        public ActionResult Create(tbl_Category cvm,HttpPostedFileBase imgfile)
        {
            string path = Upload(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Imaginea nu poate fi incarcata....";
            }
            else
            {
                tbl_Category cat = new tbl_Category();
                cat.cat_name = cvm.cat_name;
                cat.cat_image = path;
                cat.cat_status = 1;
                cat.cat_fk_ad = Convert.ToInt32(Session["ad_id"].ToString());
                db.tbl_Category.Add(cat);
                db.SaveChanges();

                return RedirectToAction("Create");
            }
                return View();
        }

        public ActionResult ViewCategory(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_Category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_Category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }




        public string Upload(HttpPostedFileBase file)
            {
                Random r = new Random();
                string path = "-1";
                int random = r.Next();
                if (file != null && file.ContentLength > 0)
                {
                    string extension = Path.GetExtension(file.FileName);
                    if (extension.ToLower().Equals(".jpg") || extension.ToLower().Equals(".jpeg") || extension.ToLower().Equals(".png"))
                    {
                    try
                        {
                            path = Path.Combine(Server.MapPath("~/Content/upload"), random + Path.GetFileName(file.FileName));
                            file.SaveAs(path);
                            path = "~/Content/upload/" + random + Path.GetFileName(file.FileName);
                            //    ViewBag.Message = "File uploaded successfully";
                        }
                        catch (Exception ex)
                        {
                            path = "-1";
                        }
                    }
                    else
                    {
                        Response.Write("<script>alert('Numai jpg ,jpeg sau png sunt acceptate....'); </script>");
                    }
                }
                else
                {
                    Response.Write("<script>alert('Alegeti o poza'); </script>");
                    path = "-1";
                }
                return path;
            }
        }
}