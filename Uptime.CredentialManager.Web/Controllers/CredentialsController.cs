using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Uptime.CredentialManager.Web.Authorization;
using Uptime.CredentialManager.Web.Models;
using Uptime.CredentialManager.Web.ViewModels;

namespace Uptime.CredentialManager.Web.Controllers
{
    public class CredentialsController : Controller
    {
        private readonly CredentialManagerDbContext _db;

        public CredentialsController(CredentialManagerDbContext db)
        {
            _db = db;
        }

        private async Task<SelectList> GetUsersAsync(Expression<Func<User, bool>> filter)
        {
            var users = await _db.Users
                .Include(x => x.UserCredentials)
                .Where(filter)
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
                .ToListAsync();

            users.Insert(0, new SelectListItem()
            {
                Text = "--- select user ---",
                Value = null
            });

            return new SelectList(users, nameof(SelectListItem.Value), nameof(SelectListItem.Text));
        }

        private bool IsCredentialUnderUser(Credential credential, User user)
        {
            return user.UserCredentials.Any(x => x.CredentialId == credential.Id);
        }

        private bool IsUserUnderCredential(User user, Credential credential)
        {
            return credential.UserCredentials.Any(x => x.UserId == user.Id);
        }

        private bool CredentialExists(Guid id)
        {
            return _db.Credentials.Any(e => e.Id == id);
        }

        private async Task<IEnumerable<Credential>> SearchAsync(string term)
        {
            var pattern = $"{term}";

            return await _db.Credentials.Include(x => x.UserCredentials)
                                            .ThenInclude(x => x.User)
                                            .Where(x => x.Description.Contains(pattern))
                                            .ToListAsync();
        }

        [HttpGet("[controller]/[action]")]
        public async Task<IActionResult> Index()
        {            
                var identity = User.Identity as ClaimsIdentity;
                var username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
                
                var user = await _db.Users
                    .Include(x => x.UserCredentials)
                    .ThenInclude(x => x.Credential)
                    .FirstOrDefaultAsync(m => m.Name == username);

                var credentialsUnderUser = await _db.Credentials
                    .Include(x => x.UserCredentials)
                    .ThenInclude(x => x.User)
                    .Where(x => IsCredentialUnderUser(x, user))
                    .ToListAsync();

                return View(credentialsUnderUser);           
         }

        [HttpGet("[controller]/[action]")]
        public async Task<IActionResult> Details(Guid id)
        {          
            var credential = await _db.Credentials
                .Include(x => x.UserCredentials)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (credential == null)
            {
                return NotFound();
            }

            var identity = User.Identity as ClaimsIdentity;
            var username = identity.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;

            if (!credential.UserCredentials.Any(x => x.User.Name == username))
            {
                return Unauthorized();
            }                

            var credentialVM = new CredentialEditViewModel();
            {
                credentialVM.Id = credential.Id;
                credentialVM.Description = credential.Description;
                credentialVM.Value = credential.Value;
                credentialVM.UserList = credential.UserCredentials
                    .Select(x => new UserViewModel
                    {
                        UserId = x.UserId,
                        UserName = x.User.Name
                    })
                    .ToList();
            };

            return View(credentialVM);
        }

        [HttpGet("[controller]/[action]")]
        [Authorize(IsAdminPolicy.Name)]
        public async Task<IActionResult> Create()
        {
            var users = await GetUsersAsync(x => true);

            var credentialVM = new CredentialEditViewModel
            {
                Users = users,
            };
            
            return View(credentialVM);
        }

        [HttpPost("[controller]/[action]")]
        [ValidateAntiForgeryToken]
        [Authorize(IsAdminPolicy.Name)]
        public async Task<IActionResult> Create(CredentialEditViewModel credentialVM)
        {
            if (!ModelState.IsValid)
            {
                return View(credentialVM);
            }

            var credential = new Credential
            {
                Description = credentialVM.Description,
                Value = credentialVM.Value,
                UserCredentials = new List<UserCredential>(),
            };

            foreach (var userId in credentialVM.SelectedUser)
            {
                credential.UserCredentials.Add(new UserCredential
                {
                    UserId = userId
                });
            }

            _db.Add(credential);
            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/[action]")]
        [Authorize(IsAdminPolicy.Name)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _db.Credentials.Include(x => x.UserCredentials)
                                                      .ThenInclude(x => x.User)
                                                      .FirstOrDefaultAsync(m => m.Id == id);           
                                               
            if (credential == null)
            {
                return NotFound();
            }
            
            var unusedUsers = await _db.Users
                                            .Where(x => !IsUserUnderCredential(x, credential))
                                            .OrderBy(x => x.Name)
                                            .Select(x =>
                                                new SelectListItem
                                                {
                                                    Text = x.Name,
                                                    Value = x.Id.ToString()
                                                })
                                            .ToListAsync();

            var credentialTipp = new SelectListItem()
            {
                Text = "--- select user ---",
                Value = null
            };

            unusedUsers.Insert(0, credentialTipp);
            var dropdown = new SelectList(unusedUsers, "Value", "Text");
            
            var credentialVM = new CredentialEditViewModel();
            {
                credentialVM.Id = credential.Id;
                credentialVM.Description= credential.Description;
                credentialVM.Value = credential.Value;
                credentialVM.UserList = credential.UserCredentials.Select(x => new UserViewModel
                {
                    UserId = x.UserId,
                    UserName = x.User.Name
                }).ToList();

                credentialVM.Users = dropdown;
            };
           
            return View(credentialVM);
        }

        [HttpPost("[controller]/[action]")]
        [Authorize(IsAdminPolicy.Name)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CredentialEditViewModel credentialVM)
        {
           if (ModelState.IsValid)
            {
                try
                {
                    var credential = await _db.Credentials.Include(x => x.UserCredentials).FirstOrDefaultAsync(m => m.Id == credentialVM.Id);
                    {
                        credential.Description = credentialVM.Description;
                        credential.Value = credentialVM.Value;

                        foreach (Guid userId in credentialVM.SelectedUser)
                        {
                            var user = _db.Find<User>(userId);
                            credential.UserCredentials.Add(new UserCredential { User = user, Credential = credential });
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CredentialExists(credentialVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", new { id = credentialVM.Id });                
            }
            return View(credentialVM);
        }

        [HttpGet("[controller]/[action]")]
        [Authorize(IsAdminPolicy.Name)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var credential = await _db.Credentials
                .FirstOrDefaultAsync(m => m.Id == id);
            if (credential == null)
            {
                return NotFound();
            }

            return View(credential);
        }

        [Authorize(IsAdminPolicy.Name)]
        [HttpPost("[controller]/Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var credential = await _db.Credentials.FindAsync(id);
            _db.Credentials.Remove(credential);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("[controller]/[action]")]
        [Authorize(IsAdminPolicy.Name)]
        public async Task<IActionResult> Remove(Guid userId, Guid credentialId)
        {

            var credential = await _db.Credentials.Include(x => x.UserCredentials)
                                          .ThenInclude(x => x.User)
                                          .FirstOrDefaultAsync(m => m.Id == credentialId);

            var userCredential = credential.UserCredentials.FirstOrDefault(x => x.UserId == userId);
            credential.UserCredentials.Remove(userCredential);
            await _db.SaveChangesAsync();
            return RedirectToAction("Edit", new { id = credentialId });
        }                     

        [HttpGet("/")]
        public IActionResult Search()
        {
            var credential = new List<Credential>();                                      
            return View(credential);
        }

        [HttpPost("[controller]/[action]")]
        public async Task<IActionResult> Search(string term)
        {
            var credential = await SearchAsync(term);
            return View(credential);            
        }
    }
}
