﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ZenithDataLib;
using ZenithSociety.Models;

namespace ZenithSociety.Controllers
{
    public class EventsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        [Authorize]
        public ActionResult Index()
        {
            var events = db.Events.Include(e => e.Activity);
            return View(events.ToList());
        }

        // GET: Events/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

		// GET: Events/Create
		[Authorize(Roles ="Admin")]
		public ActionResult Create()
        {
            ViewBag.ActivityCategory = new SelectList(db.Activities, "ActivityCategoryId", "ActivityDescription");
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles ="Admin")]
		public ActionResult Create([Bind(Include = "EventId,StartDate,EndDate,EnteredByUsername,ActivityCategory,CreationDate,IsActive")] Event @event)
        {
            if (ModelState.IsValid)
            {
				var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
				var userManager = new UserManager<ApplicationUser>(store);
				var user = userManager.FindById(User.Identity.GetUserId());
				@event.EnteredByUsername = user.UserName;
                db.Events.Add(@event);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ActivityCategory = new SelectList(db.Activities, "ActivityCategoryId", "ActivityDescription", @event.ActivityCategory);
            return View(@event);
        }

		// GET: Events/Edit/5
		[Authorize(Roles ="Admin")]
		public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            ViewBag.ActivityCategory = new SelectList(db.Activities, "ActivityCategoryId", "ActivityDescription", @event.ActivityCategory);
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles ="Admin")]
		public ActionResult Edit([Bind(Include = "EventId,StartDate,EndDate,EnteredByUsername,ActivityCategory,CreationDate,IsActive")] Event @event)
        {
            if (ModelState.IsValid)
            {
                db.Entry(@event).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ActivityCategory = new SelectList(db.Activities, "ActivityCategoryId", "ActivityDescription", @event.ActivityCategory);
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Event @event = db.Events.Find(id);
            if (@event == null)
            {
                return HttpNotFound();
            }
            return View(@event);
        }

        // POST: Events/Delete/5
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult DeleteConfirmed(int id)
        {
            Event @event = db.Events.Find(id);
            db.Events.Remove(@event);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
