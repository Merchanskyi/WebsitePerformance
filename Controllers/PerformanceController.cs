using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using website_performance.Models;
using website_performance.Models.PerformanceViewModels;

namespace website_performance.Controllers
{
    public class PerformanceController : InitController
    {
        [TempData]
        public string StatusMessage { get; set; }

        [HttpGet]
        public IActionResult Index()
        {
            var dbModel = DbContext.Performances
                .OrderByDescending(x => x.Responce)
                .Select(x => new PerformanceViewModel
                {
                    WebSite = x.Website,
                    Responce = x.Responce
                })
                .ToList();

            var viewModel = new SearchViewModel { StatusMessage = StatusMessage };

            return View("Index", new Tuple<List<PerformanceViewModel>, SearchViewModel>(dbModel, viewModel));
        }

        [HttpPost]
        public async Task<IActionResult> Index(SearchViewModel viewModel)
        {
            if (viewModel.Url == null)
            {
                StatusMessage = "Error! Fill in the blank field.";
                return RedirectToAction(nameof(Index));
            }

            HttpWebResponse response = null;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(viewModel.Url);
            request.Method = "HEAD";

            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch
            {
                StatusMessage = "Error! Request failed send.";
                return RedirectToAction(nameof(Index));
            }
            finally
            {
                if (response != null)
                    response.Close();
            }

            timer.Stop();

            await DbContext.Performances.AddAsync(new PerformanceDbModel
            {
                Id = Guid.NewGuid(),
                Website = viewModel.Url,
                Responce = timer.Elapsed
            });

            await DbContext.SaveChangesAsync();

            StatusMessage = "Success! Request sent.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Clear()
        {
            foreach (var entity in DbContext.Performances)
                DbContext.Performances.Remove(entity);

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
