using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using MemDumpHost.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MemDumpHost.Services
{
    public sealed class Startup
    {
        private readonly DumpEndpointSettings settings;
        private readonly ProcessDumper dumper;

        public Startup(IHostingEnvironment env, DumpEndpointSettings settings, ProcessDumper dumper)
        {
            this.settings = settings;
            this.dumper = dumper;
            
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        private Dictionary<Guid, string> dumpsToDownload;

        public void Configure(IApplicationBuilder app)
        {
            var path = settings.PathGuid.HasValue ? $"/dump/{settings.PathGuid.Value}" : "/dump";

            var enableDownloads = settings.DumpDownloadRegex != null;

            app.Map(path, c =>
            {
                if (enableDownloads)
                {
                    dumpsToDownload = new Dictionary<Guid, string>();

                    c.Map("/download", cc =>
                    {
                        cc.Run(async ctx =>
                        {
                            if (Guid.TryParse(ctx.Request.Path.Value.TrimStart('/'), out var downloadId) && dumpsToDownload.TryGetValue(downloadId, out var pathToFile))
                            {
                                var filename = Path.GetFileName(pathToFile);
                                ctx.Response.Headers.Add("Content-Disposition", $@"attachment; filename=""{WebUtility.UrlEncode(filename)}""");
                                await ctx.Response.SendFileAsync(pathToFile);
                                return;
                            }

                            ctx.Response.StatusCode = 404;
                            await ctx.Response.WriteAsync($"{ctx.Request.Path} not found");
                        });
                    });
                }

                c.Run(async ctx =>
                {
                    try
                    {
                        var result = dumper.DumpSelf(settings.DumpArgs, settings.DumpDirectory);

                        if (enableDownloads)
                        {
                            var downloadMatches = settings.DumpDownloadRegex.Matches(result);
                            var downloads = downloadMatches.OfType<Match>().Where(x => x.Success).Select(x => x.Groups["filename"].Value.Trim());
                            
                            foreach (var d in downloads)
                            {
                                var id = Guid.NewGuid();
                                dumpsToDownload.Add(id, d);
                                result = result.Replace(d, $@"<a href=""{path}/download/{id}"">{d}</a>");
                            }
                        }

                        ctx.Response.ContentType = "text/html";
                        await ctx.Response.WriteAsync($"<html><body><pre>{result}<pre></body></html");
                    }
                    catch (Exception e)
                    {
                        ctx.Response.StatusCode = 500;
                        await ctx.Response.WriteAsync(e.ToString());
                    }
                });
            });
        }
    }
}