using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using website_performance.Models;
using website_performance.Models.PerformanceViewModels;
using website_performance.Services;

namespace website_performance.Controllers
{
    public class PerformanceController : InitController
    {
        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            var webSiteDbModel = DbContext.Websites
                .Select(x => new WebsiteViewModel
                {
                    Id = x.Id,
                    WebSite = x.Website
                })
                .ToList();

            var viewModel = new SearchViewModel { StatusMessage = StatusMessage };

            return View("Index", new Tuple<List<WebsiteViewModel>, SearchViewModel>(webSiteDbModel, viewModel));
        }

        [HttpGet("{websiteId}")]
        public IActionResult SiteMap([FromRoute] Guid websiteId)
        {
            var siteMapDbModel = DbContext.Sitemaps
                .Where(x => x.WebsiteId == websiteId)
                .OrderByDescending(x => x.Responce)
                .Select(x => new SitemapViewModel
                {
                    Sitemap = x.Sitemap,
                    Responce = x.Responce
                })
                .ToList();

            return View("Sitemap", siteMapDbModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchViewModel viewModel)
        {
            if (viewModel.Url == null)
            {
                StatusMessage = "Error! Fill in the blank field.";
                return RedirectToAction(nameof(Index));
            }

            var request = WebRequest.Create(viewModel.Url);
            try
            {
                await request.GetResponseAsync();
            }
            catch
            {
                StatusMessage = "Error! Request failed send.";
                return RedirectToAction(nameof(Index));
            }

            var service = new SitemapBuilderService();
            var data = await service.LoadAsync(viewModel.Url);

            var perfDb = await DbContext.Websites.AddAsync(new WebsiteDbModel
            {
                Id = Guid.NewGuid(),
                Website = viewModel.Url
            });

            await DbContext.Sitemaps.AddRangeAsync(data.Select(x => new SitemapDbModel
            {
                Id = Guid.NewGuid(),
                WebsiteId = perfDb.Entity.Id,
                Sitemap = x.Url,
                Responce = x.TimeExecution
            }));

            await DbContext.SaveChangesAsync();

            StatusMessage = "Success! Request sent.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            foreach (var wsEntity in DbContext.Websites)
                DbContext.Websites.Remove(wsEntity);

            foreach (var smEntity in DbContext.Sitemaps)
                DbContext.Sitemaps.Remove(smEntity);

            await DbContext.SaveChangesAsync();

            StatusMessage = $"Success! Clear all story.";

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
