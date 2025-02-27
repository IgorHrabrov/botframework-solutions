﻿namespace RestaurantBooking.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using RestaurantBooking.Services;

    public class UrlResolver : IUrlResolver
    {
        public UrlResolver(IHttpContextAccessor httpContextAccessor, BotSettings settings)
        {
            var httpContext = httpContextAccessor.HttpContext;
            ServerUrl = httpContext.Request.Scheme + "://" + httpContext.Request.Host.Value;
        }

        public string ServerUrl { get; }

        public string GetImageUrl(string imagePath)
        {
            return GetImageByCulture(imagePath);
        }

        private string GetImageByCulture(string imagePath)
        {
            var currentCulture = CultureInfo.CurrentUICulture.Name.Split("-");
            var neutralCulture = currentCulture[0].ToLower();
            string specificCulture = null;

            if (currentCulture.ElementAtOrDefault(1) != null)
            {
                specificCulture = currentCulture[1];
            }

            return GetImagePath(imagePath, neutralCulture, specificCulture);
        }

        private string GetImagePath(string imagePath, string neutralCulture, string specificCulture)
        {
            return $"{ServerUrl}/assets/en/images/{imagePath}";
        }
    }
}
