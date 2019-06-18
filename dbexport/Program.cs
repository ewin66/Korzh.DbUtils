﻿using System;
using System.IO;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

using McMaster.Extensions.CommandLineUtils;

using dbexport.DbExporters;

namespace dbexport
{
    [HelpOption]
    class Program
    {

        public static int Main(string[] args)
            => CommandLineApplication.Execute<Program>(args);

        [Required]
        [Option(ShortName = "c", LongName = "config", Description = "Configuration file name")]
        public string ConfigFileName { get; set; }

        [Option(ShortName = "f", LongName = "file", Description = "Output file name")]
        public string OutputFileName { get; set; }

        [Option(ShortName = "t", LongName = "type", Description = "Output file type")]
        public string OutputFileType { get; set; } 

        private void OnExecute()
        {


            var configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile(ConfigFileName)
                                    .Build();

            var assemblyInfo = typeof(Program).Assembly.GetName();
            var logger = new LoggerFactory()
                  .AddConsole(LogLevel.Information)
                  .CreateLogger(assemblyInfo.Name + ", v:" + assemblyInfo.Version);

            var dbExporterBuilder = new DbExporterBuilder(logger)
                                        .UseDbExporter(configuration.GetValue<string>("Database:Type") 
                                        ?? throw new Exception("Database type is not defined"))
                                        .SetConnectionString(configuration.GetValue<string>("Database:ConnectionString")
                                        ?? throw new Exception("Connection string is not defined"));


            var fileName = OutputFileName ?? "result";
            if (!string.IsNullOrEmpty(OutputFileType))
            {
                switch (OutputFileType.ToLowerInvariant())
                {
                    case "json":
                        dbExporterBuilder.UseJsonZipFileDbSaver(fileName);
                        break;
                    case "xml":
                        dbExporterBuilder.UseXmlZipFileDbSaver(fileName);
                        break;
                    default:
                        throw new Exception("Unknown output file type: " + OutputFileType);
                }
            }
            else
            {
                dbExporterBuilder.UseJsonZipFileDbSaver(fileName);
            }


            dbExporterBuilder
                .Build()
                .Export();
        }
    }
}
