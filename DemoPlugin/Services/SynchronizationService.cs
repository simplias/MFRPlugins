using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web.Configuration;
using BSSPlugin.Command;
using DemoPlugin.ERPEntity;
using DemoPlugin.Services;
using MFR.Client;
using MFR.Client.Api.Messages;
using MFR.Client.Context;
using MFR.Client.MFR.Client;
using MFR.Client.Messaging;
using MFR.Domain.Domain;
namespace BSSPlugin.Services
{
    public class SynchronizationService
    {

        // Todo Implement some tenant based settings
        static public Settings Settings;

        private readonly IPluginContext _context;

        public SynchronizationService(IPluginContext context)
        {
            _context = context;

            if (Settings == null)
            {
                Settings = new Settings()
                    {
                        Id = 32342,
                        Note = "some note"
                    };
            }
        }


        public void Start()
        {
            _context.OnTenantMessage(ProcessMessages);
        }

        private void ProcessMessages(ITenant tenant, TenantMessage message)
        {
            tenant.ClearApiContainer();
            //"Processing message " + message.Event + " for tenant " + tenant.Settings.TenantId;

            try
            {
                if (message.Event != EventType.ServiceRequestStateChangedEvent)
                    return;

                var serviceRequestStateChangedMessage = message.GetBody<ServiceRequestStateChangedMessage>();

                if (serviceRequestStateChangedMessage.NewStateEnum != ServiceRequestState.eIsWorkDone)
                    return;

                // here you can execute the code if the service request state has changed e.g. write the data back

            }
            catch (Exception e)
            {
            }

            // Processing done
        }

        public
             string TestConnection(ITenant tenant)
        {
            return "OK";
        }

        public void InvokeSynchronize(ITenant tenant)
        {
            //Starting synchronizing jobs for tenant {0}

            var settings = GetSettings(tenant);
            if (settings.IsSynchRunning == true)
            {
                // _log.Error("ERRO Synch in progress for tenant {0}");
                return;
            }

            settings.IsSynchRunning = true;
            UpdateSettings(tenant, settings);

            try
            {
                // get Auftrag
                var auftrag = new Auftrag();

                // get Benutzer for Auftrag
                var techniker = new Benutzer();

                // get Einsatzort for Auftrag
                var einsatzOrt = new Einsatzort();

                // get Kunde for Auftrag
                var kunde = new Unternehmen();

                // get Kontakt for Auftrag
                var kontakt = new Kontakt();

                new CreateOrUpdateUserMappingCommand()
                    {
                        Benutzer = techniker,
                        tenant = tenant,
                    }.Execute();

                var mfrCustomer = new CreateOrUpdateCompanyMappingCommand()
                {
                    Unternehmen = kunde,
                    Kontakt = kontakt,
                    tenant = tenant,
                }.Execute();

                new CreateOrUpdateServiceObjectMappingCommand()
                {
                    Einsatzort = einsatzOrt,
                    ParentCompany = mfrCustomer,
                    tenant = tenant,
                }.Execute();

                new CreateOrUpdateServiceRequestMappingCommand()
                {
                    Auftrag = auftrag,
                    Einsatzort = einsatzOrt,
                    MfrCustomer = mfrCustomer,
                    Mitarbeiter = techniker,
                    tenant = tenant,
                }.Execute();

            }
            catch (Exception e)
            {

                //_log.Info("Incremental sync for tenant " + e + " done");

                settings.IsSynchRunning = false;
                UpdateSettings(tenant, settings);

                return;
            }

            settings.IsSynchRunning = false;
            UpdateSettings(tenant, settings);
            //_log.Info("Incremental sync for tenant {0} done");
        }

        public Settings UpdateSettings(ITenant tenant, Settings syncSettings)
        {
            Settings = syncSettings;
            return Settings;
        }

        public Settings GetSettings(ITenant tenant)
        {
            return Settings;
        }
    }
}