﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using MongoDB.Bson;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class AuthController : Controller
    {
        private readonly MongoDBService _mongoDatabase;

        public AuthController(MongoDBService mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        [HttpGet("RegisterToStratusService")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> RegisterToStratusService(string username, string password, string accessKey,string secretKey)
        {
            var registerToStratusServiceResp = new StratusResponse<StratusUser>();
            await _mongoDatabase.InsertDocument("StratusDB", "Users");
            return Ok(registerToStratusServiceResp);
            //var user = new StratusUser()
            //{
            //    Username = username,
            //    Password = password
            //};
            //
            //var userExists = await _mongoDatabase.GetStratusUser(username);
            //
            //if (userExists == null)
            //{
            //    await _mongoDatabase.RegisterToStratusService(user);
            //
            //    registerToStratusServiceResp.Data = user;
            //
            //    return Ok(registerToStratusServiceResp);
            //}
            //else
            //{
            //    registerToStratusServiceResp.Error = "User already exists";
            //
            //    return BadRequest(registerToStratusServiceResp);
            //}
        }
        [HttpGet("GetDocumentByFilter")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetDocumentByFilter(string filter)
        {
            var getDocumentByFilterResp = new StratusResponse<StratusUser>();
            var filterdDocs = await _mongoDatabase.GetDocumentsByFilter("StratusDB", "Users", (documnet) => documnet.GetElement("email").ToString().Equals(filter));
            return Ok();
        }

    }
}