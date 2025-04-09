using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        private static IHttpClientFactory _httpClientFactory;

        static async Task Main(string[] args)
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            _httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();

            int id = await CreateListItemWithSite("Companies");

            var businessModules = await GetBusinessModules();

            var companyModule = businessModules.First(bm => bm.ListUrl == "Lists/Companies");

            ListItem listItem = await GetListItem(companyModule.Id, id);

            string siteRelativeUrl = listItem.FieldValues.First(fv => fv.InternalFieldName == "wpSite").TextValue;

            await QueueMasterSiteSyncJobForEntities(companyModule.Id, SynchronizationScopeEnum.BufferSites, workPointJobQueue: "Integration");
        }

        private static async Task<BusinessModuleOnline[]> GetBusinessModules()
        {
            WorkPointAPI workPointAPI = new WorkPointAPI(_httpClientFactory, Mode.Production, TenantID, WorkPointUrl, ClientID, ClientSecret);
            var value = await workPointAPI.GetBusinessModules();

            return value;
        }

        private static async Task<int> CreateListItemWithSite(string bmName)
        {
            List<FieldValue> fieldValues = new List<FieldValue>();
            fieldValues.Add(new FieldValue("Title", "My Company Title"));

            ListItemAPI listItemAPI = new ListItemAPI(_httpClientFactory, Mode.Production, TenantID, WorkPointUrl, ClientID, ClientSecret);
            listItemAPI.TimeOutInMilliseconds = 1000 * 60 * 5; // 5 minutes
            int value = await listItemAPI.Create(bmName, fieldValues.ToArray(), true);

            return value;
        }

        private static async Task<ListItem> GetListItem(Guid bmId, int itemId)
        {
            ListItemAPI listItemAPI = new ListItemAPI(_httpClientFactory, Mode.Production, TenantID, WorkPointUrl, ClientID, ClientSecret);
            var value = await listItemAPI.GetListItem(bmId, itemId);

            return value;
        }

		private static async Task<Guid?> QueueAggregationJobForEntity(Guid bmId, int itemId)
		{
			ListItemAPI listItemAPI = new ListItemAPI(_httpClientFactory, Mode.Production, TenantID, WorkPointUrl, ClientID, ClientSecret);
			var value = await listItemAPI.QueueAggregationJobForEntity(bmId, itemId);

			return value; 
		}

        private static async Task<Guid?> QueueMasterSiteSyncJobForEntities(Guid bmId, SynchronizationScopeEnum synchronizationScope, string camlQuery = null, Guid? viewId = null, string workPointJobQueue = null)
		{
            var queueMSSyncModel = new QueueMasterSiteEntitySynchronizationModel();

            queueMSSyncModel.ListScopes = new List<ListScope> { new ListScope("Documents") };
            queueMSSyncModel.SetWelcomePage = true;
            queueMSSyncModel.SyncWebParts = true;
            queueMSSyncModel.SyncFeatures = true;
            queueMSSyncModel.SyncNavigation = true;
            queueMSSyncModel.SyncNintexWorkflows = false;
            queueMSSyncModel.SyncNintexWorkflowsDeleteFromTargetSite = false;
            queueMSSyncModel.SyncTeamSettings = false;
            queueMSSyncModel.DeleteAddTeamTabs = false;
            queueMSSyncModel.SyncDefaultSiteCollectionSettings = true;
            queueMSSyncModel.SyncSiteColumns = true;
            queueMSSyncModel.SyncSiteContentTypes = true;
            queueMSSyncModel.SyncLanguageResources = true;
            queueMSSyncModel.SyncSiteCollectionTermSets = true;
            queueMSSyncModel.SyncSiteCollectionFeatures = true;
            queueMSSyncModel.SyncSiteCollectionApps = false;
            
            queueMSSyncModel.SynchronizationScope = synchronizationScope;
            
            if (synchronizationScope == SynchronizationScopeEnum.CAML)
                queueMSSyncModel.CamlQuery = camlQuery;
            else if (synchronizationScope == SynchronizationScopeEnum.View)
                queueMSSyncModel.ViewId = viewId;         

            WorkPointAPI workpointAPI = new WorkPointAPI(_httpClientFactory, Mode.Production, TenantID, WorkPointUrl, ClientID, ClientSecret);
            if (workPointJobQueue != null)
                workpointAPI.WorkPointJobQueue = workPointJobQueue; 
            
            var value = await workpointAPI.QueueMasterSiteSyncJobForEntities(bmId, queueMSSyncModel);
			return value; 
		}
    }
}
