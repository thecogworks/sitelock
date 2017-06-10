﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace Cogworks.SiteLock.Web.Configuration
{
    public interface ISiteLockConfiguration
    {
        List<string> GetLockedDomains();
        List<string> GetAllowedPaths();
        void AppendAllowedPath(string absolutePathLowered);
    }

    public class SiteLockConfiguration : ISiteLockConfiguration
    {
        private static string DomainsKey = typeof(SiteLockConfiguration) + "_domains";
        private static string AllowedPathsKey = typeof(SiteLockConfiguration) + "_allowedPaths";


        public List<string> GetLockedDomains()
        {
            var value = HttpRuntime.Cache[DomainsKey] as List<string>;

            if (value == null)
            {
                value = GetValues("lockedDomains", "domain");

                HttpRuntime.Cache.Insert(DomainsKey, value, null);
            }

            return value;
        }



        public List<string> GetAllowedPaths()
        {
            var value = HttpRuntime.Cache[AllowedPathsKey] as List<string>;

            if (value == null)
            {
                value = GetValues("allowedPaths", "path");

                HttpRuntime.Cache.Insert(AllowedPathsKey, value, null);
            }

            return value;
        }


        public void AppendAllowedPath(string absolutePathLowered)
        {
            var allowedPaths = GetAllowedPaths();

            var hasAbsolutePath = allowedPaths.Contains(absolutePathLowered);
            if (!hasAbsolutePath)
            {
                allowedPaths.Add(absolutePathLowered);
            }
        }


        private List<string> GetValues(string containerName, string elementName)
        {
            var doc = XDocument.Load(HostingEnvironment.MapPath("/config/SiteLock.config"));

            var value = doc.Root.Element(containerName).Elements(elementName).Select(x => x.Value.Trim()).ToList();

            return value;
        }
    }
}