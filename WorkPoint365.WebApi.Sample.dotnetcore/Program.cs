using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkPoint365.WebAPI.Model;
using WorkPoint365.WebAPI.Model.SharePoint;
using WorkPoint365.WebAPI.Proxy.NETStandard;

namespace WorkPoint365.WebApi.Sample.dotnetcore
{
    class Program
    {
        public static string ClientID = "{ClientId}"; 
        public static string ClientSecret = "{ClientSecret}";
        public static string WorkPointUrl = "{WorkPointUrl}";
        public static string TenantID = "{TenantID}";    

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
            WorkPointAPI workPointAPI = new WorkPointAPI(WorkPointAPI.Mode.Integration, TenantID, WorkPointUrl, ClientID, ClientSecret);
            var value = await workPointAPI.GetBusinessModules();

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
