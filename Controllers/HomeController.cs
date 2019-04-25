using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCAddressBook.ViewModel;
using System.Linq.Dynamic;

namespace MVCAddressBook.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        public ActionResult Index(int page =1, string sort="Name",string sortDir="asc", string search="")
        {
            int pageSize = 10;
            int totalRecord =0;
            if (page < 1)
                page = 1;
            int skip = (page * pageSize) - pageSize;
            var data = GetAddressDetails(search, sort, sortDir, skip,pageSize, out totalRecord);
            ViewBag.TotalRows = totalRecord;
            ViewBag.search = search;
            List<AddressModel> addBooks = new List<AddressModel>();
            //MyAddressBookEntities is datacontext
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                var v = (from a in dc.Addresses
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         select new AddressModel
                         {
                             AddressID = a.AddressID,
                             Name = a.Name,
                             Surname = a.Surname,
                             Address1 = a.Address1,
                             Address2 = a.Address2,
                             Postcode = a.Postcode,
                             Town = a.Town,
                             CountryID = a.CountryID,
                             Email = a.Email,
                             MobileNumber = a.MobileNumber,
                             ImagePath = a.ImagePath
                         }).ToList();
                addBooks = v;
            }


            return View(addBooks);
        }

        public List<Address> GetAddressDetails(string search, string sort, string sortDir, int skip, int pageSize, out int totalRecord)
        {
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                var v = (from a in dc.Addresses              
                         where
                             a.Name.Contains(search) ||
                             a.Surname.Contains(search) 
                          select a 
                         );
                totalRecord = v.Count();
                v = v.OrderBy(sort + " " + sortDir);
                if(pageSize>0)
                {
                    v = v.Skip(skip).Take(pageSize);
                }
                return v.ToList();
            }
        }
        public ActionResult Add()
        {
            //fetch country data
            List<Country> allCountry = new List<Country>();

            //MyAddressBookEntities is the DbContext
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();

            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName");

            return View();
        }

        //post method for add controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Address add, HttpPostedFileBase file)
        {
            //fetch Country
            #region
            List<Country> allCountry = new List<Country>();
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();

            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", add.CountryID);
            #endregion

            //validate file if selected
            #region
            if (file != null)
            {
                if (file.ContentLength > (512 * 100)) //File size 512KB
                {
                    ModelState.AddModelError("FileErrorMessage", "Size must within 512 KB");
                }
                string[] allowedType = new string[] { "image/png", "image/gif", "image/jpeg", "image/jpg" };
                bool isFileTypeValid = false;
                foreach (var i in allowedType)
                {
                    if (file.ContentType == i.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }
                if (!isFileTypeValid)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png .gif and .jpg file type allowed");
                }
            }
            #endregion

            //validate model & save to database
            #region
            if (ModelState.IsValid)
            {
                //save
                if (file != null)  //add image 
                {
                    string savePath = Server.MapPath("../Image/");
                    string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePath, fileName));
                    add.ImagePath = fileName;
                }

                using (MyAddressBookEntities dc = new MyAddressBookEntities())
                {
                    dc.Addresses.Add(add);
                    try
                    {
                        dc.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                    {
                        Exception raise = dbEx;
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                string message = string.Format("{0}:{1}",
                                    validationErrors.Entry.Entity.ToString(),
                                    validationError.ErrorMessage);
                                // raise a new exception nesting
                                // the current instance as InnerException
                                raise = new InvalidOperationException(message, raise);
                            }
                        }
                        throw raise;
                    }
                    
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(add);
            }
            #endregion
        }

        public ActionResult View(int id)
        {
            //Action is for show address book details of a selected contact
            
            Address ab = null;
            ab = GetAddress(id);
            return View(ab);
        }

        //function for fetch add book details from database by id
        private Address GetAddress(int id)
        {
            Address address = null;
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                var v = (from a in dc.Addresses
                         join b in dc.Countries on a.CountryID equals b.CountryID
                         where a.AddressID.Equals(id)
                         select new
                         {
                             a,
                             b.CountryName
                         }).FirstOrDefault();
                if (v != null)
                {
                    address = v.a;
                    address.CountryName = v.CountryName;

                }

            }
            return address;
        }

        
        public ActionResult Edit(int id)
        {
            //fetch Address
            Address add = null;
            add = GetAddress(id);
            if (add == null )
            {
                return HttpNotFound("Address Not Found");
            }

            //fetch Country
            List<Country> allCountry = new List<Country>();
            using(MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();
            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", add.CountryID);
            return View(add);
        }

        //post method for edit controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Address add, HttpPostedFileBase file)
        {
            //fecth Country for dropdownlist
            #region
            List<Country> allCountry = new List<Country>();
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                allCountry = dc.Countries.OrderBy(a => a.CountryName).ToList();

            }
            ViewBag.Country = new SelectList(allCountry, "CountryID", "CountryName", add.CountryID);
            #endregion
            //validate if file is selected
            #region
            if (file != null)
            {
                if (file.ContentLength > (512 * 100)) //File size 512KB
                {
                    ModelState.AddModelError("FileErrorMessage", "Size must within 512 KB");
                }
                string[] allowedType = new string[] { "image/png", "image/gif", "image/jpeg", "image/jpg" };
                bool isFileTypeValid = false;
                foreach (var i in allowedType)
                {
                    if (file.ContentType == i.ToString())
                    {
                        isFileTypeValid = true;
                        break;
                    }
                }
                if (!isFileTypeValid)
                {
                    ModelState.AddModelError("FileErrorMessage", "Only .png .gif and .jpg file type allowed");
                }
            }
            #endregion
            //update the Address
            #region
            if(ModelState.IsValid)
            {
                //update
                if(file != null)
                {
                    string savePath = Server.MapPath("~/image");
                    string filename = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    file.SaveAs(Path.Combine(savePath, filename));
                    add.ImagePath = filename;
                }

                using (MyAddressBookEntities dc = new MyAddressBookEntities())
                {
                    var v = dc.Addresses.Where(a => a.AddressID.Equals(add.AddressID)).FirstOrDefault();
                    if (v != null)
                    {
                        v.Name = add.Name;
                        v.Surname = add.Surname;
                        v.Address1 = add.Address1;
                        v.Address2 = add.Address2;
                        v.Postcode = add.Postcode;
                        v.Town = add.Town;
                        v.CountryID = add.CountryID;
                        v.Email = add.Email;
                        v.MobileNumber = add.MobileNumber;
                        if(file !=null)
                        {
                            v.ImagePath = add.ImagePath;
                        }
                    }
                    dc.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            else
            {
                return View(add);
            }
            #endregion
        }

        public ActionResult Delete(int id)
        {
            //fetch Address
            Address add = null;
            add = GetAddress(id); //function to get data
            return View(add);
        }

        //post method for delete controller
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]  //action name required
        public ActionResult DeleteConfirm(int id)
        {
            using (MyAddressBookEntities dc = new MyAddressBookEntities())
            {
                var address = dc.Addresses.Where(a => a.AddressID.Equals(id)).FirstOrDefault();
                if(address != null)
                {
                    dc.Addresses.Remove(address);
                    dc.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    return HttpNotFound("Address Not Found!");
                }
            }
                
        }
    }


}
