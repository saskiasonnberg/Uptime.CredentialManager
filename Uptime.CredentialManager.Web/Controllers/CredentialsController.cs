using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Uptime.CredentialManager.Web.Models;

namespace Uptime.CredentialManager.Web.Controllers
{
    public class CredentialsController : Controller
    {
        private readonly UptimeCredentialManagerWebContext _context;

        public CredentialsController(UptimeCredentialManagerWebContext context)
        {
            _context = context;
        }

        // GET: Credentials
        public async Task<IActionResult> Index()
        {
            return View(await _context.Credential.ToListAsync());
        }

        // GET: Credentials/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        // GET: Credentials/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Credentials/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Value")] Credential credential)
        {
            if (ModelState.IsValid)
            {
                credential.Id = Guid.NewGuid();
                _context.Add(credential);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(credential);
        }

        // GET: Credentials/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential.FindAsync(id);
            if (credential == null)
            {
                return NotFound();
            }
            return View(credential);
        }

        // POST: Credentials/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Description,Value")] Credential credential)
        {
            if (id != credential.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(credential);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credential.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(credential);
        }

        // GET: Credentials/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _context.Credential
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        // POST: Credentials/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var credential = await _context.Credential.FindAsync(id);
            _context.Credential.Remove(credential);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CredentialExists(Guid id)
        {
            return _context.Credential.Any(e => e.Id == id);
        }
    }
}
