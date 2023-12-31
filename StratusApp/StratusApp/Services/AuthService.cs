﻿using StratusApp.Models;
using StratusApp.Services.MongoDBServices;
using Utils.DTO;
using MongoDB.Driver;
using BCrypt.Net;
using System;
using System.Security.Cryptography;
using System.Text;
using StratusApp.Settings;

namespace StratusApp.Services
{
    public class AuthService
    {
        private readonly MongoDBService _mongoDatabase;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthService(MongoDBService mongoDatabase, AppSettings appSettings, IHttpContextAccessor httpContextAccessor)
        {
            _mongoDatabase = mongoDatabase;
            EncryptionHelpers.EncryptionHelper.SetSettings(appSettings.EncryptionSettings);
            _httpContextAccessor = httpContextAccessor;
        }

        internal Task<string> IsUserExists(StratusUser user)
        {
            bool isUserExists = false;
            try
            {
                var dbUsers = _mongoDatabase.GetCollectionAsList<StratusUser>(eCollectionName.Users).Result;
                string hashedPassword = EncryptionHelpers.EncryptionHelper.Encrypt(user.Password);
                if (dbUsers.Any(dbUser => dbUser.Username != null && dbUser.Username.Equals(user.Username)))
                { 
                    return Task.FromResult("Username already exists");
                } 
                else if( dbUsers.Any(dbUser => dbUser.Password.Equals(hashedPassword)))
                {
                    return Task.FromResult("Password already exists");
                }
                else if(dbUsers.Any(dbUser => dbUser.Email.Equals(user.Email))) 
                {
                    return Task.FromResult("Email already exists");
                }
                else
                {
                    return Task.FromResult("User doesn't exists");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
        public Task<bool> IsUserRegistered(string email, string password)
        {
            bool registered = false;
            try
            {
                var dbUsers = _mongoDatabase.GetCollectionAsList<StratusUser>(eCollectionName.Users).Result;
                string hashedPassword = EncryptionHelpers.EncryptionHelper.Encrypt(password);
                if (dbUsers.Any(dbUser => dbUser.Email != null && dbUser.Email.Equals(email)) && dbUsers.Any(dbUser => dbUser.Password.Equals(hashedPassword)))
                {
                    registered = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            return Task.FromResult(registered);
        }
        public void decryptAccessKey(StratusUser user)
        {
            user.AccessKey = EncryptionHelpers.EncryptionHelper.Decrypt(user.AccessKey);
        }
        public void decryptSecretKey(StratusUser user)
        {
            user.SecretKey = EncryptionHelpers.EncryptionHelper.Decrypt(user.SecretKey);
        }
        public void encryptPassword(StratusUser user)
        {
            user.Password = EncryptionHelpers.EncryptionHelper.Encrypt(user.Password);
        }
        public void encryptAccessKey(StratusUser user)
        {
            user.AccessKey = EncryptionHelpers.EncryptionHelper.Encrypt(user.AccessKey);
        }
        public void encryptSecretKey(StratusUser user)
        {
            user.SecretKey = EncryptionHelpers.EncryptionHelper.Encrypt(user.SecretKey);
        }
        public void InsertUserToDB(StratusUser user)
        {
            //check that the decryption works

            encryptPassword(user);
            encryptAccessKey(user);
            encryptSecretKey(user);
            _mongoDatabase.InsertDocument<StratusUser>(eCollectionName.Users, user);
            decryptAccessKey(user);
            decryptSecretKey(user);
        }

        public async Task<StratusUser> GetUserFromDB(string email)
        {
            var dbUsers = await _mongoDatabase.GetCollectionAsList<StratusUser>(eCollectionName.Users);
            var user = dbUsers.Find(dbUser => dbUser.Email.Equals(email));
            decryptSecretKey(user);
            decryptAccessKey(user);
            return user; 
        }

        internal async Task<string> StoreUserDBEmailInSession(StratusUser user)
        {
            var dbUsers = await _mongoDatabase.GetCollectionAsList<StratusUser>(eCollectionName.Users);
            try
            {
                //var userDBEmail = dbUsers.Find(dbUser => dbUser.Email.Equals(user.Email)).Email;
                _httpContextAccessor.HttpContext.Session.SetString("Stratus", user.Email);
                return user.Email;
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }

        internal void LogOutFromStratusService()
        {
            _httpContextAccessor.HttpContext.Session.Remove("Stratus");
        }
    }
}
