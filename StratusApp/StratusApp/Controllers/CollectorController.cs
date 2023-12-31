﻿using Amazon.CloudWatch.Model;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MonitoringClient;
using StratusApp.Models.Responses;
using StratusApp.Services.Collector;
using System.Collections.Generic;
using Utils.DTO;
using Utils.Utils;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class CollectorController : Controller
    {
        private readonly CollectorService _collectorService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CollectorController(CollectorService collectorService, IHttpContextAccessor contextAccessor)
        {
            _collectorService = collectorService;
            _httpContextAccessor = contextAccessor;
        }

        [HttpGet("GetNumberOfvCPU")]
        public async Task<ActionResult<StratusResponse<int>>> GetNumberOfvCPU(string instance)
        {
            var cpuUsageResponse = new StratusResponse<int>();

            cpuUsageResponse.Data = await _collectorService.GetNumberOfvCPU(instance);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetAvgCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<double>>> GetCpuUsageUtilization(string instance = "34.125.220.240", string timeFilter = "month")
        {
            if (instance is null) return BadRequest();

            var cpuUsageResponse = new StratusResponse<double>();

            cpuUsageResponse.Data = await _collectorService.GetAvgCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetMaxCpuUsageUtilization")]
        public async Task<ActionResult<StratusResponse<double>>> GetMaxCpuUsageUtilization(string instance = "34.125.220.240", string timeFilter = "month")
        {
            var cpuUsageResponse = new StratusResponse<double>();

            cpuUsageResponse.Data = await _collectorService.GetMaxCpuUsageUtilization(instance, timeFilter);

            return Ok(cpuUsageResponse);
        }

        [HttpGet("GetAvgCpuUtilizationByCpu")]
        public async Task<ActionResult<StratusResponse<List<SingleCpuUtilizationDTO>>>> GetAvgCpuUtilizationByCpu(string instance = "34.125.220.240", string timeFilter = "month")
        {
            try
            {
                var id = Request.Headers.Cookie;
                var freeMemorySizeResponse = new StratusResponse<List<SingleCpuUtilizationDTO>>();
                var email = HttpContext.Request.Cookies["userDBEmail"];
                freeMemorySizeResponse.Data = await _collectorService.GetAvgCpuUtilizationByCpu(instance, timeFilter);

                return Ok(freeMemorySizeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTotalDiskSizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetTotalDiskSizeInGB(string instance = "34.125.220.240")
        {
            try
            {
                var diskSizeResponse = new StratusResponse<double>();

                diskSizeResponse.Data = await _collectorService.GetTotalDiskSizeInGB(instance);

                return Ok(diskSizeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgFreeDiskSpaceInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgFreeDiskSpaceInGB(string instance, string timeFilter = "month")
        {
            try
            {
                var avgAvailableDiskSpaceResponse = new StratusResponse<double>();

                avgAvailableDiskSpaceResponse.Data = await _collectorService.GetAvgFreeDiskSpaceInGB(instance, timeFilter);

                return Ok(avgAvailableDiskSpaceResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgDiskSpaceUsageInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgDiskSpaceUsageInGB(string instance, string timeFilter = "month")
        {
            try
            {
                var avgDiskSpaceUsageResponse = new StratusResponse<double>();

                avgDiskSpaceUsageResponse.Data = await _collectorService.GetAvgDiskSpaceUsageInGB(instance, timeFilter);

                return Ok(avgDiskSpaceUsageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgDiskSpaceUsagePercentage")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgDiskSpaceUsagePercentage(string instance, string timeFilter = "month")
        {
            try
            {
                var avgDiskSpaceUsageResponse = new StratusResponse<double>();

                avgDiskSpaceUsageResponse.Data = await _collectorService.GetAvgDiskSpaceUsagePercentage(instance, timeFilter);

                return Ok(avgDiskSpaceUsageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetTotalMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetTotalMemorySizeInGB(string instance)
        {
            try
            {

                var totalMemorySizeResponse = new StratusResponse<double>();

                totalMemorySizeResponse.Data = await _collectorService.GetTotalMemorySizeInGB(instance);

                return Ok(totalMemorySizeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgFreeMemorySizeInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgFreeMemorySizeInGB(string instance, string timeFilter = "month")
        {
            try
            {
                var freeMemorySizeResponse = new StratusResponse<double>();

                freeMemorySizeResponse.Data = await _collectorService.GetAvgFreeMemorySizeInGB(instance, timeFilter);

                return Ok(freeMemorySizeResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgMemorySizeUsageInGB")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgMemorySizeUsageInGB(string instance, string timeFilter = "month")
        {
            try
            {
                var avgMemorySizeUsageResponse = new StratusResponse<double>();

                avgMemorySizeUsageResponse.Data = await _collectorService.GetAvgMemorySizeUsageInGB(instance, timeFilter);

                return Ok(avgMemorySizeUsageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAvgMemorySizeUsagePercentage")]
        public async Task<ActionResult<StratusResponse<double>>> GetAvgMemorySizeUsagePercentage(string instance, string timeFilter = "month")
        {
            try
            {
                var avgDiskSpaceUsageResponse = new StratusResponse<double>();

                avgDiskSpaceUsageResponse.Data = await _collectorService.GetAvgMemorySizeUsagePercentage(instance, timeFilter);

                return Ok(avgDiskSpaceUsageResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("RefreshAllUserResourcesDetails")]
        public async Task<ActionResult<StratusResponse<string>>> RefreshAllUserResourcesDetails()
        {
            try
            {
                string userEmail = SessionUtils.GetSessionId(_httpContextAccessor);

                var instances = await _collectorService.RefreshAllUserResourcesDetails(userEmail);
                if (instances != null)
                {
                    return Ok();
                }
                else throw new Exception();
            }
            catch (Exception ex)
            {
                return BadRequest(new StratusResponse<string>(false, ex.Message));
            }
        }

        [HttpGet("GetAllUserResourcesDetails")]
        public async Task<ActionResult<StratusResponse<List<AwsInstanceDetails>>>> GetAllUserResourcesDetails()
        {
            try
            {
                string userEmail = SessionUtils.GetSessionId(_httpContextAccessor);
                var instances = new StratusResponse<List<AwsInstanceDetails>>();

                instances.Data = await _collectorService.GetAllUserResourcesDetails(userEmail);

                return Ok(instances);
            }
            catch (Exception ex)
            {
                return BadRequest(new StratusResponse<string>(false, ex.Message));
            }
        }


        [HttpGet("GetAvgCpuUsageUtilizationOverTime")]
        public async Task<ActionResult<StratusResponse<List<CpuUsageData>>>> GetAvgCpuUsageUtilizationOverTime(string instance = "34.125.220.240", string timeFilter = "month")
        {
            try
            {
                var avgCpuOverTimeResponse = new StratusResponse<List<CpuUsageData>>();

                avgCpuOverTimeResponse.Data = await _collectorService.GetAvgCpuUsageUtilizationOverTime(instance, timeFilter);

                return Ok(avgCpuOverTimeResponse);

            }
            catch (Exception ex)
            {
                
                return BadRequest(
                    new StratusResponse<List<CpuUsageData>>(false ,ex.Message)
                    );
            }
        }     

    } 
}
