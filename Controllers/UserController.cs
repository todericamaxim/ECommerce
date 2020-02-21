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
    public class UserController : Controller
    {


        dbemarketingEntities db = new dbemarketingEntities();
        // GET: User
        public ActionResult Index(int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_Category.Where(x => x.cat_status == 1).OrderByDescending(x => x.cat_id).ToList();
            IPagedList<tbl_Category> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);

        }

        [HttpGet]
        public ActionResult Sign()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Sign(tbl_user user, HttpPostedFileBase imgfile)
        {
            string path = Upload(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Imaginea nu poate fi incarcata....";
            }

            else
            {
                tbl_user u = new tbl_user();
                u.u_name = user.u_name;
                u.u_password = user.u_password;
                u.u_email = user.u_email;
                u.u_image = path;
                u.u_contact = user.u_contact;
                db.tbl_user.Add(u);
                db.SaveChanges();

                return RedirectToAction("Login");
            }
            return View();

        }


        [HttpGet]
        public ActionResult login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult login(tbl_user avm)
        {
            tbl_user ad = db.tbl_user.Where(x => x.u_email == avm.u_email && x.u_password == avm.u_password).SingleOrDefault();
            if (ad != null)
            {
                Session["u_id"] = ad.u_id.ToString();
                return RedirectToAction("CreateP");
            }
            else
            {
                ViewBag.error = "Nume sau parola invalide";
            }

            return View();
        }

        [HttpGet]
        public ActionResult CreateP()
        {
            List<tbl_Category> categories = db.tbl_Category.ToList();
            ViewBag.categorylist = new SelectList(categories, "cat_id", "cat_name");
            return View();
        }

        [HttpPost]
        public ActionResult CreateP(tbl_product product, HttpPostedFileBase imgfile)
        {
            List<tbl_Category> li = db.tbl_Category.ToList();
            ViewBag.categorylist = new SelectList(li, "cat_id", "cat_name");


            string path = Upload(imgfile);
            if (path.Equals("-1"))
            {
                ViewBag.error = "Imaginea nu poate fi incarcata....";
            }
            else
            {
                tbl_product p = new tbl_product();
                p.pro_name = product.pro_name;
                p.pro_price = product.pro_price;
                p.pro_image = path;
                p.pro_fk_cat = product.pro_fk_cat;
                p.pro_des = product.pro_des;
                p.pro_fk_user = Convert.ToInt32(Session["u_id"].ToString());
                db.tbl_product.Add(p);
                db.SaveChanges();
                Response.Redirect("index");
            }
            return View();
        }



        public ActionResult Ads(int? id, int? page)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_fk_cat == id).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        [HttpPost]
        public ActionResult Ads(int? id, int? page, string search)
        {
            int pagesize = 9, pageindex = 1;
            pageindex = page.HasValue ? Convert.ToInt32(page) : 1;
            var list = db.tbl_product.Where(x => x.pro_name.Contains(search)).OrderByDescending(x => x.pro_id).ToList();
            IPagedList<tbl_product> stu = list.ToPagedList(pageindex, pagesize);
            return View(stu);
        }

        public ActionResult RepDes(int ?id)
        {
            RepDesmodel rp = new RepDesmodel();
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            rp.pro_id = p.pro_id;
            rp.pro_name = p.pro_name;
            rp.pro_image = p.pro_image;
            rp.pro_price = p.pro_price;
            tbl_Category cat = db.tbl_Category.Where(x=>x.cat_id==p.pro_fk_cat).SingleOrDefault();
            rp.cat_name = cat.cat_name;
            tbl_user u = db.tbl_user.Where(x => x.u_id == p.pro_fk_user).SingleOrDefault();
            rp.u_name = u.u_name;
            rp.u_image = u.u_image;
            rp.u_contact = u.u_contact;
            rp.pro_fk_user = u.u_id;

            return View(rp);
        }


        public ActionResult Iesire()
        {
            Session.RemoveAll();
            Session.Abandon();
            return RedirectToAction("Index");
                
        }

        public ActionResult Delete(int? id)
        {
            tbl_product p = db.tbl_product.Where(x => x.pro_id == id).SingleOrDefault();
            db.tbl_product.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
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
