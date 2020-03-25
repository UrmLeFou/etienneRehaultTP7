using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BO;
using Dojo.Data;
using Dojo.Models;

namespace Dojo.Controllers
{
    public class SamouraisController : Controller
    {
        private DojoContext db = new DojoContext();

        // GET: Samourais
        public ActionResult Index()
        {
            return View(db.Samourais.ToList());
        }

        // GET: Samourais/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            return View(samourai);
        }

        // GET: Samourais/Create
        public ActionResult Create()
        {
            SamouraiVM vm = new SamouraiVM();
            List<int> armesIds = db.Samourais.Where(s => s.Arme != null).Select(s => s.Arme.Id).ToList();
            vm.Armes = db.Armes.Where(s => !armesIds.Contains(s.Id)).ToList();
            vm.ArtsMartiaux.Add(new ArtMartial() { Nom = "Petit scarabée" });
            vm.ArtsMartiaux.AddRange(db.ArtMartials.ToList());
            return View(vm);
        }

        // POST: Samourais/Create
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SamouraiVM vm)
        {
            if (ModelState.IsValid)
            {
                vm.Samourai.ArtMartiaux = db.ArtMartials.Where(am => vm.IdArtMartiaux.Contains(am.Id)).ToList();

                db.Samourais.Add(vm.Samourai);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            vm.Armes = db.Armes.ToList();
            vm.ArtsMartiaux = db.ArtMartials.ToList();
            return View(vm);
        }

        // GET: Samourais/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SamouraiVM vm = new SamouraiVM();
            vm.Samourai = db.Samourais.Find(id);
            if (vm.Samourai == null)
            {
                return HttpNotFound();
            }

            List<int> armesIds = db.Samourais.Where(s => s.Arme != null && s.Id != id).Select(s => s.Arme.Id).ToList();
            vm.Armes = db.Armes.Where(s => !armesIds.Contains(s.Id)).ToList();
            if (vm.Samourai.Arme != null)
            {
                vm.IdArme = vm.Samourai.Arme.Id;
            }

            vm.ArtsMartiaux.Add(new ArtMartial() { Nom = "Petit scarabée" });
            vm.ArtsMartiaux.AddRange(db.ArtMartials.ToList());
            vm.IdArtMartiaux = vm.Samourai.ArtMartiaux.Select(am => am.Id).ToList();

            return View(vm);
        }

        // POST: Samourais/Edit/5
        // Afin de déjouer les attaques par sur-validation, activez les propriétés spécifiques que vous voulez lier. Pour 
        // plus de détails, voir  https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SamouraiVM vm)
        {
            if (ModelState.IsValid)
            {
                var samouraiDb = db.Samourais.Find(vm.Samourai.Id);

                samouraiDb.Force = vm.Samourai.Force;
                samouraiDb.Nom = vm.Samourai.Nom;

                if (vm.IdArme != null)
                {
                    var ArmeProprio = db.Samourais.Where(s => s.Arme.Id == vm.IdArme).ToList();

                    Arme arme = null;
                    foreach (var item in ArmeProprio)
                    {
                        arme = item.Arme;
                        item.Arme = null;
                        db.Entry(item).State = EntityState.Modified;
                    }

                    if (arme == null)
                    {
                        samouraiDb.Arme = db.Armes.FirstOrDefault(a => a.Id == vm.IdArme);
                    }
                    else
                    {
                        samouraiDb.Arme = arme;
                    }
                }
                else
                {
                    samouraiDb.Arme = null;
                }
                foreach(var item in samouraiDb.ArtMartiaux)
                {
                    db.Entry(item).State = EntityState.Modified;
                }
                samouraiDb.ArtMartiaux = db.ArtMartials.Where(am => vm.IdArtMartiaux.Contains(am.Id)).ToList();
                db.Entry(samouraiDb).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            List<int> armesIds = db.Samourais.Where(s => s.Arme != null && s.Id != vm.Samourai.Id).Select(s => s.Arme.Id).ToList();
            vm.Armes = db.Armes.Where(s => !armesIds.Contains(s.Id)).ToList();
            return View(vm);
        }

        // GET: Samourais/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Samourai samourai = db.Samourais.Find(id);
            if (samourai == null)
            {
                return HttpNotFound();
            }
            return View(samourai);
        }

        // POST: Samourais/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Samourai samourai = db.Samourais.Find(id);
            db.Samourais.Remove(samourai);
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
