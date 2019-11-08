using HtmlAgilityPack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace website_performance.Services
{
    public class UrlContext
    {
        public string Url { get; set; }
        public TimeSpan TimeExecution { get; set; }
        public int Code { get; set; }
    }

    public class SitemapBuilderService
    {
        private Uri _baseUrl;
        private ConcurrentDictionary<string, UrlContext> _contexts;

        public async Task<List<UrlContext>> LoadAsync(string url)
        {
            _baseUrl = new Uri(url);
            _contexts = new ConcurrentDictionary<string, UrlContext>();

            await InternalLoadAsync(url);

            return _contexts.Values.ToList();
        }

        private async Task InternalLoadAsync(string url)
        {
            var context = new UrlContext
            {
                Url = url
            };

            if (!_contexts.TryAdd(url, context))
            {
                return;
            };

            var request = WebRequest.Create(url);
            WebResponse response = null;

            var sw = new Stopwatch();
            sw.Start();
            try
            {
                response = await request.GetResponseAsync();
                context.Code = 200;
            }
            catch
            {
                context.Code = 404;
            }
            sw.Stop();

            context.TimeExecution = sw.Elapsed;

            if (response != null)
            {
                var document = new HtmlDocument();
                document.Load(response.GetResponseStream());

                var tasks = document.DocumentNode
                    .SelectNodes($"//a/@href")
                    .Select(node => node.Attributes["href"]?.Value)
                    .Select(url => NormilizeUrl(url))
                    .Where(url => url != null)
                    .Select(url => InternalLoadAsync(url));

                await Task.WhenAll(tasks);
            }
        }

        private string NormilizeUrl(string url)
        {
            if (url != null)
            {
                if (url.StartsWith(_baseUrl.AbsoluteUri))
                {
                    return url;
                }

                if (url.StartsWith('/'))
                {
                    return new Uri(_baseUrl, url).AbsoluteUri;
                }
            }

            return null;
        }
    }
}
