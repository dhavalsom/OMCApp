using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OMCWebApp.Controllers
{
    public class PatientProblemsController : Controller
    {
        // GET: PatientProblems
        public ActionResult Index()
        {
            return View();
        }

        // GET: PatientProblems/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: PatientProblems/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PatientProblems/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientProblems/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: PatientProblems/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: PatientProblems/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: PatientProblems/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
