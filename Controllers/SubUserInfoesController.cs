using BookStation.Data;
using BookStation.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStation.Controllers
{
    public class SubUserInfoesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SubUserInfoesController(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: SubUserInfoes
        public async Task<IActionResult> Index()
        {
            return _context.SubUserInfos != null ?
                        View(await _context.SubUserInfos.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.SubUserInfos'  is null.");
        }

        // GET: SubUserInfoes/Details/5
        public async Task<IActionResult> Details(int id)
        {
            if (id == null || _context.SubUserInfos == null)
            {
                return NotFound();
            }

            var subUserInfo = await _context.SubUserInfos
                .FirstOrDefaultAsync(m => m.infoID == id);
            if (subUserInfo == null)
            {
                return NotFound();
            }

            return View(subUserInfo);
        }

        // GET: SubUserInfoes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SubUserInfoes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("infoID,UserID,AvatarID")] SubUserInfo subUserInfo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subUserInfo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subUserInfo);
        }

        // GET: SubUserInfoes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.SubUserInfos == null)
            {
                return NotFound();
            }

            var subUserInfo = await _context.SubUserInfos.FindAsync(id);
            if (subUserInfo == null)
            {
                return NotFound();
            }
            return View(subUserInfo);
        }

        // POST: SubUserInfoes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("infoID,UserID,AvatarID")] SubUserInfo subUserInfo)
        {
            if (id != subUserInfo.infoID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subUserInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubUserInfoExists(subUserInfo.infoID))
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
            return View(subUserInfo);
        }

        // GET: SubUserInfoes/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null || _context.SubUserInfos == null)
            {
                return NotFound();
            }

            var subUserInfo = await _context.SubUserInfos
                .FirstOrDefaultAsync(m => m.infoID == id);
            if (subUserInfo == null)
            {
                return NotFound();
            }

            return View(subUserInfo);
        }

        // POST: SubUserInfoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.SubUserInfos == null)
            {
                return Problem("Entity set 'ApplicationDbContext.SubUserInfos'  is null.");
            }
            var subUserInfo = await _context.SubUserInfos.FindAsync(id);
            if (subUserInfo != null)
            {
                _context.SubUserInfos.Remove(subUserInfo);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SubUserInfoExists(int id)
        {
            return (_context.SubUserInfos?.Any(e => e.infoID == id)).GetValueOrDefault();
        }

        // Custom method for uploading avatar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> uploadAvatar([Bind("infoID,UserID,FileUpload")] SubUserInfo subUserInfo)
        {
            Console.WriteLine("^^^^^^^^^^^^^info=" + subUserInfo.infoID + " user:" + subUserInfo.UserID);
            IdentityUser user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            Console.WriteLine("######## upload");
            if (user.Id != subUserInfo.UserID)
            {
                Console.WriteLine("$$$$$$$$ user not found");
                return NotFound();
            }


                try
                {
                    // Get old sub user info data of user from database 
                    SubUserInfo subUserInfoOldData = _context.SubUserInfos.AsNoTracking().FirstOrDefault
                        (e => e.UserID == subUserInfo.UserID);

                    var pathAvaImg = _webHostEnvironment.WebRootPath + UserAvatar.avatarFolder;

                    // if old avatar is default image, we should not delete it
                    if (!UserAvatar.avatarsArray.Contains(subUserInfoOldData?.AvatarName))
                    {
                        deleteAvatarImage(pathAvaImg, subUserInfoOldData?.AvatarName);
                    }

                    // save new avatar image
                    var savedFileName = await saveAvatarImage(subUserInfo.UserID, subUserInfo.FileUpload, pathAvaImg);

                    // update data into database
                    subUserInfo.infoID = subUserInfoOldData.infoID;
                    subUserInfo.Path = pathAvaImg;
                    subUserInfo.AvatarName = savedFileName;
                    Console.WriteLine("&&&&& changed");
                    _context.Update(subUserInfo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubUserInfoExists(subUserInfo.infoID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            Console.WriteLine("$$$$$$$$ error");
            return RedirectToAction("Index", "Personal");
           
        }


        private void deleteAvatarImage(String path, String filePath)
        {
            string fullPath = Path.Combine(path, filePath);

            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }

        private async Task<String> saveAvatarImage(String userId, IFormFile formFile, String pathAvaImg)
        {
            var tempFilePath = Path.GetTempFileName();
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }

            var savedFileName = userId + getExtensionFile(formFile);
            var finalFilePath = Path.Combine(pathAvaImg, savedFileName);

            System.IO.File.Move(tempFilePath, finalFilePath);
            return savedFileName;
        }

        private String getExtensionFile(IFormFile formFile)
        {
            int lastDotIndex = formFile.FileName.LastIndexOf('.');
            return (lastDotIndex >= 0 ? formFile.FileName.Substring(lastDotIndex) : string.Empty);
        }
    }
}
