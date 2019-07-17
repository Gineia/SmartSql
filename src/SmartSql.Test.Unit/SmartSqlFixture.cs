﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartSql.DbSession;
using SmartSql.Middlewares.Filters;
using Xunit;

namespace SmartSql.Test.Unit
{
    public class SmartSqlFixture : IDisposable
    {
        public const string GLOBAL_SMART_SQL = "GlobalSmartSql";
        public static int CtorCount = 0;

        public SmartSqlFixture()
        {
            LoggerFactory = new LoggerFactory(Enumerable.Empty<ILoggerProvider>(),
                new LoggerFilterOptions {MinLevel = LogLevel.Debug});
            var logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "SmartSql.log");
            LoggerFactory.AddFile(logPath, LogLevel.Trace);

            SmartSqlBuilder = new SmartSqlBuilder()
                .UseXmlConfig()
                .UseLoggerFactory(LoggerFactory)
                .UseAlias(GLOBAL_SMART_SQL)
                .AddFilter<TestPrepareStatementFilter>()
                .Build();
            DbSessionFactory = SmartSqlBuilder.DbSessionFactory;
            SqlMapper = SmartSqlBuilder.SqlMapper;
        }

        public SmartSqlBuilder SmartSqlBuilder { get; }
        public IDbSessionFactory DbSessionFactory { get; }
        public ISqlMapper SqlMapper { get; }
        public ILoggerFactory LoggerFactory { get; }

        public void Dispose()
        {
            SmartSqlBuilder.Dispose();
        }
    }

    [CollectionDefinition(SmartSqlFixture.GLOBAL_SMART_SQL)]
    public class SmartSqlCollection : ICollectionFixture<SmartSqlFixture>
    {
    }


    public class TestPrepareStatementFilter : IPrepareStatementFilter, ISetupSmartSql
    {
        private ILogger<TestPrepareStatementFilter> _logger;

        public void OnInvoking(ExecutionContext context)
        {
            _logger.LogDebug("TestPrepareStatementFilter.OnInvoking");
        }

        public void OnInvoked(ExecutionContext context)
        {
            _logger.LogDebug("TestPrepareStatementFilter.OnInvoked");
        }

        public Task OnInvokingAsync(ExecutionContext context)
        {
            _logger.LogDebug("TestPrepareStatementFilter.OnInvokingAsync");
            return Task.CompletedTask;
        }

        public Task OnInvokedAsync(ExecutionContext context)
        {
            _logger.LogDebug("TestPrepareStatementFilter.OnInvokedAsync");
            return Task.CompletedTask;
        }

        public void SetupSmartSql(SmartSqlBuilder smartSqlBuilder)
        {
            _logger = smartSqlBuilder.LoggerFactory.CreateLogger<TestPrepareStatementFilter>();
        }
    }
}