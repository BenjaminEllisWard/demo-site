﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace Web.Helpers
{
    public static class EntryPointHelper
    {
        static Dictionary<string, string> _assets;

        static EntryPointHelper()
        {
            _assets = GetWebpackAssetsJson();
        }

        public static string PathFor(string filePath)
        {
            return VirtualPathUtility.ToAbsolute(filePath);
        }

        public static IHtmlString ReactEntryPoint(string entryPoint)
        {
            var output = new List<string>();
            output.Add(ScriptWithCachebustingHash("~/Scripts/react/polyfill"));
            output.Add(ScriptWithCachebustingHash("~/Scripts/react/common"));
            output.Add(ScriptWithCachebustingHash(entryPoint));
            return new HtmlString(string.Join("\n", output));
        }

        public static string ScriptWithCachebustingHash(string path)
        {
            var fixedPath = path.Replace(".tsx", "").ToLower();

            if (!_assets.ContainsKey(fixedPath))
            {
                throw new InvalidOperationException(string.Format("Couldn't find {0}", fixedPath));
            }
            var cachebustedPath = _assets[fixedPath];
            var absolutePath = PathFor(cachebustedPath);

            return string.Format(@"<script src=""{0}""></script>", absolutePath);
        }

        public static Dictionary<string, string> GetWebpackAssetsJson()
        {
            JObject o1 = JObject.Parse(File.ReadAllText(string.Format("{0}\\webpack-assets.json", AppDomain.CurrentDomain.BaseDirectory)));
            var assets = new Dictionary<string, string>();
            foreach (var x in o1)
            {
                string key = x.Key;
                string value = x.Value["js"].Value<string>();
                string keyFixed = key.Replace("./src/Web/", "~/").ToLower();
                string valueFixed = value.Replace("./src/Web/", "~/").ToLower();
                assets.Add(keyFixed, valueFixed);
            }

            return assets;
        }
    }
}