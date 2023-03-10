﻿using Microsoft.AspNetCore.Mvc;
using AwsClient = CloudApiClient.CloudApiClient;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Amazon.CloudWatch.Model;

namespace StratusApp.Controllers
{
    public class AwsController : Controller
    {

        private readonly AwsClient _awsClient;

        public AwsController() 
        {
            _awsClient = new AwsClient();
        }

        /*[HttpGet("GetAllAwsPackages")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetAllAwsPackages()
        {

        }*/

        [HttpGet("GetUserInstanceData")]
        public async Task<ActionResult<StratusResponse<StratusUser>>> GetUserAwsInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<Datapoint>>();

            userInstanceDataStartusResp.Data = await _awsClient.GetInstanceData();
            
            return Ok(userInstanceDataStartusResp);
        }
    }
}
