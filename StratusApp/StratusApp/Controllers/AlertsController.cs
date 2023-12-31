﻿using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StratusApp.Models;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using MonitoringClient;
using Utils.DTO;
using StratusApp.Services;
using DTO;
using StratusApp.Services.AlertsService;

namespace StratusApp.Controllers
{
    [EnableCors("AllowAnyOrigin")]
    public class AlertsController : Controller
    {
        private readonly AlertsService _alertsService;

        public AlertsController(AlertsService alertsService, EmailService emailService)
        {
            _alertsService = alertsService;
            _alertsService.EmailService = emailService;
        }

        [HttpGet("GetAlerts")]
        public async Task<ActionResult<StratusResponse<List<AlertData>>>> GetInstanceCpuUsage()
        {
            var alertResponse = new StratusResponse<List<AlertData>>();

            alertResponse.Data = await _alertsService.GetAlertsCollection(); //.Result;

            return Ok(alertResponse);
        }

        [HttpPost("SetConfigurations")]
        public async Task<ActionResult<StratusResponse<string>>> SetConfigurations([FromBody] AlertsConfigurations alertsConfigurations)
        {

            bool result = _alertsService.SetConfigurations(alertsConfigurations);

            if (result)
            {
                var response = new StratusResponse<string>
                {
                    Data = "Configurations saved successfully"
                };

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }            
        }

        [HttpPatch("RegisterToAlerts")]
        public async Task<ActionResult<StratusResponse<string>>> RegisterToAlerts()
        {
            await _alertsService.RegisterToAlerts();

            return Ok();
        }
    }
}
