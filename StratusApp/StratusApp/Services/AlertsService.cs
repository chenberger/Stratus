﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using OpenQA.Selenium;
using StratusApp.Data;
using StratusApp.Models;
using StratusApp.Models.MongoDB;
using StratusApp.Models.Responses;
using StratusApp.Services.MongoDBServices;
using System.Reflection.PortableExecutable;
using System;
using System.Timers;
using Utils.DTO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.IO;
using DTO;
using Amazon.Runtime.Documents;
using MongoDB.Driver;
using System.Linq.Expressions;
using MonitoringClient.Prometheus;
using MonitoringClient.Prometheus.PrometheusApi;
using Utils.Enums;
using StratusApp.Services.Collector;

namespace StratusApp.Services
{
    public class AlertsService
    {
        private readonly MongoDBService _mongoDatabase;
        //private readonly PrometheusClient _prometheusClient;
        private readonly CollectorService _collectorService;
        private System.Timers.Timer _timer;
        private readonly Dictionary<eAlertType, int> _alertTypesConverter;
        private int _cpuPercentageThreshold = 70;
        private int _memoryPercentageThreshold = 70;
        private int _storagePercentageThreshold = 70;
        private double _intervalTimeToAlert = 1000 * 60;

        private const string INTERVAL_FILTER = "day";

        private readonly List<AlertData> _alerts = new List<AlertData>();

        public EmailService EmailService { get; internal set; }

        public AlertsService(MongoDBService mongoDatabase, CollectorService collectorService) 
        {
            _mongoDatabase = mongoDatabase;
            //_prometheusClient = new PrometheusClient();
            _collectorService = collectorService;

            _alertTypesConverter = new Dictionary<eAlertType, int>()
            {
                [eAlertType.CPU] = _cpuPercentageThreshold,
                [eAlertType.STORAGE] = _storagePercentageThreshold,
                [eAlertType.MEMORY] = _memoryPercentageThreshold,
            };

            InitTimer(_intervalTimeToAlert);
        }

        internal async Task<List<AlertData>> GetAlertsCollection()
        {
            var result = new List<AlertData>();
            //TODO get alerts ny user
            var alertsData =  _mongoDatabase.GetCollectionAsList<AlertData>(eCollectionName.Alerts).Result;            

            foreach (var alert in alertsData)
            {
                if (alert != null)
                {
                    result.Add(alert);
                }
            }

            return result;
        }    

        private void InitTimer(double interval)
        {
            // update table with new data and delete records that machine id that was terminated

            _timer = new System.Timers.Timer();
            _timer.Interval = interval; // should be confiugre by the user
            _timer.Elapsed += timer_Elapsed;
            _timer.Start();
        }

        private void timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            string machineId = "34.125.220.240";
            //foreach user:
            //foreach user instance:

            double avgCpuUsageUtilization = _collectorService.GetAvgCpuUsageUtilization(machineId, INTERVAL_FILTER).Result;
            double avgFreeDiskSpaceInGB = _collectorService.GetAvgFreeDiskSpaceInGB(machineId, INTERVAL_FILTER).Result;
            double avgFreeMemorySizeInGB = _collectorService.GetAvgFreeMemorySizeInGB(machineId, INTERVAL_FILTER).Result;

            _alerts.Clear();
            DetectAndInsertLowUsage(machineId, avgCpuUsageUtilization, eAlertType.CPU);
            DetectAndInsertLowUsage(machineId, avgFreeDiskSpaceInGB, eAlertType.STORAGE);
            DetectAndInsertLowUsage(machineId, avgFreeMemorySizeInGB, eAlertType.MEMORY);

           
            //Send mail to user
            //EmailService.SendAlertsEmailAsync("omer2541996@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("chen10.berger@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("amir.anckonina@gmail.com", _alerts);
            //EmailService.SendAlertsEmailAsync("hbinsky.mta@gmail.com", _alerts);

            InsertAlertsToDB();
        }

        private async void InsertAlertsToDB()
        {
            //insert with user id
            
            await _mongoDatabase.InsertMultipleDocuments(eCollectionName.AlertConfigurations, _alerts);
        }

        private void DetectAndInsertLowUsage(string machineId, double avgUsage, eAlertType eAlertType)
        {
            if (avgUsage <= _alertTypesConverter[eAlertType])
            {
                _alerts.Add(new AlertData()
                {
                    MachineId = machineId,
                    Type = eAlertType,
                    CreationTime = DateTime.Now,
                    PercentageUsage = avgUsage,
                });
            }
        }

        internal bool SetConfigurations(AlertsConfigurations alertsConfigurations)
        {
            bool result = false;

            try
            {
                SetIntervalTime(alertsConfigurations);
                SetThresholdValues(alertsConfigurations);
                var deleteResult = ResetAlertsConfigurationsCollection();

                if (deleteResult.IsAcknowledged)
                {
                    InsertAlertsConfigurationsToDB(alertsConfigurations);

                    result = true;
                }
            }
            catch { }

            return result;
        }

        private void SetThresholdValues(AlertsConfigurations alertsConfigurations)
        {
            _cpuPercentageThreshold = alertsConfigurations.CpuThreshold;
            _memoryPercentageThreshold = alertsConfigurations.MemoryThreshold;
            _storagePercentageThreshold = alertsConfigurations.DiskThreshold;
            _intervalTimeToAlert = alertsConfigurations.IntervalTimeMilisec;
            _timer.Interval = _intervalTimeToAlert;
        }

        private async void InsertAlertsConfigurationsToDB(AlertsConfigurations alertsConfigurations)
        {
            //insert for specific id
            
            await _mongoDatabase.InsertDocument(eCollectionName.AlertConfigurations, alertsConfigurations);
        }

        private DeleteResult ResetAlertsConfigurationsCollection()
        {
            // need to reset only for specific user !!

            return _mongoDatabase.DeleteDocuments(eCollectionName.AlertConfigurations, Builders<AlertsConfigurations>.Filter.Empty).Result;
        }

        private void SetIntervalTime(AlertsConfigurations alertsConfigurations)
        {
            switch(alertsConfigurations.IntervalPeriod)
            {
                case "hour":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * alertsConfigurations.IntervalPeriodValue; break;
                case "day":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * alertsConfigurations.IntervalPeriodValue; break;
                case "week":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * 7 * alertsConfigurations.IntervalPeriodValue; break;
                case "month":
                    alertsConfigurations.IntervalTimeMilisec = 1000 * 60 * 60 * 24 * 20 * alertsConfigurations.IntervalPeriodValue; break;
            }
        }

        ~AlertsService() { _timer.Stop(); }
    }
}
