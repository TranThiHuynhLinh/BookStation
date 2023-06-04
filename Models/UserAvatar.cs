using BookStation.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStation.Models
{
    public class UserAvatar : SubUserInfo
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public static string[] avatarsArray = { "AV1.jpg", "AV2.jpg", "AV3.jpg", "AV4.jpg", "AV5.jpg", "AV6.jpg" };
        
        public static string avatarFolder = "/img/avt_img";
        public UserAvatar(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        private String getRandomAvatar()
        {
            Random random = new Random();
            int randomNumber = random.Next(6);
            return avatarsArray[randomNumber];
        }

        public async Task<SubUserInfo> saveRandomAvatarForUser(string userId)
        {
            var pathAvaImg = _webHostEnvironment.WebRootPath + avatarFolder;
            SubUserInfo subUserInfo = new SubUserInfo();
            subUserInfo.UserID = userId;
            subUserInfo.Path = pathAvaImg;
            subUserInfo.AvatarName = getRandomAvatar();
            _context.Add(subUserInfo);
            await _context.SaveChangesAsync();
            return subUserInfo;
        }

        public String getAvatarOfUser(string userId)
        {
            return getSubUserInfos(userId).AvatarName;
        }

        public SubUserInfo getSubUserInfos(string userId)
        {
            var subUserInfos = (_context.SubUserInfos.Where(sub => sub.UserID == userId)).ToList();
            if (subUserInfos.Any())
            {
                return subUserInfos[0];
            }
            throw new Exception("No found SubUserInfo");
        }
        //public async Task<SubUserInfo> getSubUserInfos(string userId)
        //{
        //    var subUserInfo = await _context.SubUserInfos.FirstOrDefaultAsync(sub => sub.UserID == userId);
        //    return subUserInfo;
        //}
    }
}
