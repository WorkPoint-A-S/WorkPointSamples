using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using WorkPoint.Integration.Common;
using WorkPoint365.WebAPI.Model;
using WorkPoint365.WebAPI.Model.SharePoint;
using WorkPoint365.WebAPI.Proxy;

namespace WorkPoint365.WebApi.Sample
{
    class Program
    {
        public static string ClientID = System.Configuration.ConfigurationManager.AppSettings["ClientID"];
        public static string ClientSecret = System.Configuration.ConfigurationManager.AppSettings["ClientSecret"];
        public static string WorkPointUrl = System.Configuration.ConfigurationManager.AppSettings["WorkPointUrl"];
        public static string TenantID = System.Configuration.ConfigurationManager.AppSettings["TenantID"];


        static async Task Main(string[] args)
        {
            int id = await CreateListItemWithSite("Companies");

            var businessModules = await GetBusinessModules();

            var companyModule = businessModules.First(bm => bm.ListUrl == "Lists/Companies");

            ListItem listItem = await GetListItem(companyModule.Id, id);

            string siteRelativeUrl = listItem.FieldValues.First(fv => fv.InternalFieldName == "wpSite").TextValue;
        }



        private static async Task<BusinessModuleOnline[]> GetBusinessModules()
        {
            ListItemAPI listItemAPI = new ListItemAPI(WorkPointAPI.Mode.Integration, TenantID, WorkPointUrl, ClientID, ClientSecret);
            var value = await listItemAPI.GetBusinessModules();

            return value;
        }


        private static async Task<int> CreateListItemWithSite(string bmName)
        {
            List<FieldValue> fieldValues = new List<FieldValue>();
            fieldValues.Add(new FieldValue("Title", "My Company Title"));


            ListItemAPI listItemAPI = new ListItemAPI(WorkPointAPI.Mode.Integration, TenantID, WorkPointUrl, ClientID, ClientSecret);
            listItemAPI.TimeOutInMilliseconds = 1000 * 60 * 5; // 5 minutes
            int value = await listItemAPI.Create(bmName, fieldValues.ToArray(), true);

            return value;
        }


        private static async Task<ListItem> GetListItem(Guid bmId, int itemId)
        {

            ListItemAPI listItemAPI = new ListItemAPI(WorkPointAPI.Mode.Integration, TenantID, WorkPointUrl, ClientID, ClientSecret);
            var value = await listItemAPI.GetListItem(bmId, itemId);

            return value;
        }


    }
}
