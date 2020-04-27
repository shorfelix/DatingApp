using System;
using System.Threading.Tasks;
using DatingApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class AuthRepostory : IAuthRepository
    { 
        private readonly DataContext _context;
        public AuthRepostory(DataContext context)
        {
            _context=context;
        }
        public async Task<User> Login(string username, string password)
        {
            var user =await _context.Users.FirstOrDefaultAsync(x=>x.Username==username);
            if(user==null){
                return null;
            }
            if(!VerifyPasswordHash(password,user.passworHash,user.passwordSalt))
                return null;

        return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passworHash, byte[] passwordSalt)
        {
           using( var hmac= new System.Security.Cryptography.HMACSHA512(passwordSalt)){
              
               var computedHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for(int i=0;i<computedHash.Length;i++){
                    if(computedHash[i]!=passworHash[i])
                    return false;

               }
               return true;
            }
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSolt;
            CreatePasswordHash(password, out passwordHash, out passwordSolt);
            user.passworHash=passwordHash;
            user.passwordSalt=passwordSolt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSolt)
        {
            using( var hmac= new System.Security.Cryptography.HMACSHA512()){
                passwordSolt=hmac.Key;
                passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
           
        }

        public async Task<bool> UserExists(string username)
        {
            if(await _context.Users.AnyAsync(x=>x.Username==username))
            return true;

            return false;
        }
    }
}