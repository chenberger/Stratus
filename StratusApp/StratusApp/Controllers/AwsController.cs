﻿using Microsoft.AspNetCore.Mvc;
using AwsClient = CloudApiClient.CloudApiClient;
using StratusApp.Models;
using StratusApp.Models.Responses;
using Amazon.CloudWatch.Model;
using Amazon.EC2.Model;
using Microsoft.AspNetCore.Cors;
using CloudApiClient.AwsServices.AwsUtils;
using StratusApp.Data;
using Utils.DTO;
using StratusApp.Services;
using Utils.Enums;
using Amazon;

namespace StratusApp.Controllers
{
    //[EnableCors()]
    [EnableCors("AllowAnyOrigin")]
    public class AwsController : Controller
    {
        private readonly AwsService _awsService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        
        public AwsController(AwsService awsService, IHttpContextAccessor httpContextAccessor) 
        {
            _awsService = awsService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet("GetUserInstanceData")]
        public async Task<ActionResult<StratusResponse<List<AwsInstanceDetails>>>> GetUserInstanceData()
        {
            var userInstanceDataStartusResp = new StratusResponse<List<AwsInstanceDetails>>();

            userInstanceDataStartusResp.Data = await _awsService.GetBasicAwsInstancesDetails();

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetInstanceCPUStatistics")]
        public async Task<ActionResult<StratusResponse<List<Datapoint>>>> GetInstanceCPUStatistics(string instanceId)
        {
            var userInstanceDataStartusResp = new StratusResponse<List<Datapoint>>();
            userInstanceDataStartusResp.Data = await _awsService.GetInstanceCPUStatistics(instanceId);

            return Ok(userInstanceDataStartusResp);
        }

        [HttpGet("GetUserInstanceCpuUsageDataOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetUserAwsInstanceCpuUsageDataOverTime(string instanceId, string filterTime = "Month")
        {
            var userInstanceCpuUsageDataOverTimeStartusResp = new StratusResponse<List<CpuUsageData>>();
            userInstanceCpuUsageDataOverTimeStartusResp.Data = await _awsService.GetInstanceCpuUsageOverTime(instanceId, filterTime);

            return Ok(userInstanceCpuUsageDataOverTimeStartusResp);
        }

        [HttpGet("GetInstanceFromAWS")]
        public async Task<ActionResult<StratusResponse<List<Instance>>>> GetInstanceFromAWS()
        {
            var userInstanceStartusResp = new StratusResponse<List<Instance>>();

            userInstanceStartusResp.Data = await _awsService.GetInstances();

            return Ok(userInstanceStartusResp);
        }
        
        [HttpGet("GetMoreFittedInstancesFromAWS")]
        public async Task<ActionResult<List<AwsInstanceDetails>>> GetMoreFittedInstancesFromAWS(string instanceId)
        {
            var instancesListResponse = new StratusResponse<List<AwsInstanceDetails>>();
            instancesListResponse.Data = await _awsService.GetMoreFittedInstances(instanceId);

            return Ok(instancesListResponse);
        }

        [HttpGet("GetInstanceVolumes")]
        public async Task<ActionResult<StratusResponse<List<Volume>>>> GetInstanceVolumes(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<List<Volume>>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceVolumes(instanceId);

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceTotalVolumesSize")]
        public async Task<ActionResult<StratusResponse<int>>> GetInstanceTotalVolumesSize(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<int>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceTotalVolumesSize(instanceId);

            return Ok(instanceVolumeResponse);
        }
        [HttpGet("GetCurrentInstanceVolumesUsage")]
        public async Task<ActionResult<StratusResponse<double>>> GetCurrentInstanceVolumesUsage(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<double>();

            instanceVolumeResponse.Data = await _awsService.GetCurrentInstanceVolumesUsage(instanceId);

            return Ok(instanceVolumeResponse);
        }
        [HttpGet("GetInstanceOperatingSystem")]
        public async Task<ActionResult<StratusResponse<string>>> GetInstanceOperatingSystem(string instanceId)
        {
            var instanceVolumeResponse = new StratusResponse<string>();

            instanceVolumeResponse.Data = await _awsService.GetInstanceOperatingSystem(instanceId);

            return Ok(instanceVolumeResponse);
        }

        [HttpGet("GetInstanceBasicDetails")]
        public async Task<ActionResult<InstanceDetails>> GetInstanceBasicDetails(string instanceId)
        {
            var instanceBasicDetailsResponse = new StratusResponse<InstanceDetails>();

            instanceBasicDetailsResponse.Data = await _awsService.GetInstanceBasicDetails(instanceId);

            return Ok(instanceBasicDetailsResponse);
        }
        [HttpGet("getAlternativeMachinesWithScraper")]
        public async Task<ActionResult<StratusResponse<List<AlternativeInstance>>>> getAlternativeMachinesWithScraper()
        {
            var instanceBasicDetailsResponse = new StratusResponse<List<AlternativeInstance>>();

            instanceBasicDetailsResponse.Data = await _awsService.ScrapeInstancesDetailsIntoDB();

            return Ok(instanceBasicDetailsResponse);
        }
        [HttpGet("StoreAWSCredentialsInSession")]
        public async Task<ActionResult<StratusResponse<bool>>> StoreAWSCredentialsInSession(string accessKey, string secretKey, string region)
        {
            var instanceBasicDetailsResponse = new StratusResponse<bool>();

            instanceBasicDetailsResponse.Data = _awsService.StoreAWSCredentialsInSession(accessKey, secretKey, region);

            return Ok(instanceBasicDetailsResponse);
        }
        [HttpGet("GetAWSCredentialsFromSession")]
        public async Task<ActionResult<StratusResponse<Dictionary<eAWSCredentials, string>>>> GetAWSCredentialsFromSession()
        {
            var instanceBasicDetailsResponse = new StratusResponse<Dictionary<eAWSCredentials, string>>();

            instanceBasicDetailsResponse.Data = _awsService.GetAWSCredentialsFromSession();

            return Ok(instanceBasicDetailsResponse);
        }
    }
}
