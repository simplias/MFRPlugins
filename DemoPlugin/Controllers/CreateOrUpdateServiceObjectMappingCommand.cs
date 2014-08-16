using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Dynamic;
using System.Linq;
using DemoPlugin.ERPEntity;
using DemoPlugin.Services;
using MFR.Client.MFR.Client;
using MFR.Client.Context;
using Newtonsoft.Json.Linq;

namespace BSSPlugin.Command
{
    // 2 Anfragen
    public class CreateOrUpdateServiceObjectMappingCommand
    {
        public ITenant tenant { get; set; }

        public Settings synchSettings;

        public Einsatzort Einsatzort { get; set; }

        public Company ParentCompany { get; set; }

        public List<Contact> serviceObjectContacts { get; set; }

        public Container TenantContainer { get; set; }

        public ServiceObject Execute()
        {
            TenantContainer = tenant.CreateApiContainer();

            ServiceObject mfrServiceObject = null;
            mfrServiceObject = TenantContainer.ServiceObjects
                .Expand(s => s.CustomValueSteps)
                .Where(c => c.MappingId == Einsatzort.EindeutigeNummer).ToList().FirstOrDefault();

            if (mfrServiceObject == null)
            {
                mfrServiceObject = new ServiceObject()
                    {
                        Id = 0,
                    };
            }

            UpdateServiceObject(mfrServiceObject);

            return mfrServiceObject;

            return null;
        }

        private void UpdateServiceObject(ServiceObject mfrServiceObject)
        {
            var hashCodeBefore = HashHelper.GetHashValueCode(mfrServiceObject);

            // we have nothing to do if the modification date has not changed
            if (mfrServiceObject.Id != 0 && mfrServiceObject.DateModified.AddMinutes(10) > Einsatzort.LetzteAenderung &&
                mfrServiceObject.DateModified.AddMinutes(-10) < Einsatzort.LetzteAenderung)
                return;

            mfrServiceObject.Name = Einsatzort.Name;
            mfrServiceObject.MappingId = Einsatzort.EindeutigeNummer;
            mfrServiceObject.ExternalId = Einsatzort.EindeutigeNummer;
            mfrServiceObject.CompanyId = ParentCompany.Id;

            if (mfrServiceObject.Location == null)
                mfrServiceObject.Location = new Location();

            mfrServiceObject.Location.AddressString = Einsatzort.Adresse ;
            mfrServiceObject.Location.Postal = Einsatzort.Adresse;
            mfrServiceObject.Location.City = Einsatzort.Stadt;

            mfrServiceObject.Note = Einsatzort.Beschreibung;

            if (mfrServiceObject.Id == 0)
            {
                TenantContainer.AddToServiceObjects(mfrServiceObject);
                TenantContainer.SaveChanges();
            }
            else if (hashCodeBefore != HashHelper.GetHashValueCode(mfrServiceObject))
            {
                TenantContainer.UpdateObject(mfrServiceObject);
                TenantContainer.SaveChanges();
            }

            var step = mfrServiceObject.CustomValueSteps.FirstOrDefault();
            if (step == null)
            {
                step = new Step();
                step.Id = 0;
            }

            hashCodeBefore = HashHelper.GetHashValueCode(step);

            if (step.Id == 0)
            {
                TenantContainer.AddToSteps(step);
                TenantContainer.SaveChanges();
            }
            else if (hashCodeBefore != HashHelper.GetHashValueCode(step))
            {
                TenantContainer.UpdateObject(step);
                TenantContainer.SaveChanges();
            }
        }

    }
}