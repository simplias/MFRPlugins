using System;
using System.Collections.Generic;
using System.Linq;
using DemoPlugin.ERPEntity;
using MFR.Client.Context;
using MFR.Client.MFR.Client;
namespace DemoPlugin.Command
{
    public class CreateOrUpdateContactMappingCommand
    {
        public ITenant tenant { get; set; }

        public Company MFRParentCompany;

        public Kontakt Kontakt { get; set; }

        public Contact MFRContact { get; set; }

        public Container TenantContainer { get; set;  }

        public Contact Execute()
        {
            TenantContainer =  tenant.CreateApiContainer();

            // attaches the company object to the tenant containers context
            TenantContainer.AttachTo("Companies",MFRParentCompany);

            if (MFRContact == null)
            {
                // queries the contact with the EinduetigeNummer
                MFRContact = TenantContainer.Contacts.Where(c => c.ExternalId == Kontakt.EindeutigeNummer).ToList().FirstOrDefault();
            }
                

            UpdateContactDetails(MFRContact);

            return MFRContact;
        }

        private void UpdateContactDetails(Contact mfrContact)
        {
            if (mfrContact == null)
            {
                mfrContact = new Contact();
                mfrContact.Id = 0;
            }

            // we have nothing to do if the modification date has not changed
            if (mfrContact.Id != 0 && mfrContact.DateModified.AddMinutes(10) > Kontakt.LetzteAenderung &&
                mfrContact.DateModified.AddMinutes(-10) < Kontakt.LetzteAenderung)
                return;

            mfrContact.ExternalId = Kontakt.EindeutigeNummer;
            mfrContact.FirstName = Kontakt.Vorname;
            mfrContact.LastName = Kontakt.Nachname;
            mfrContact.JobTitle = Kontakt.Berufsbezeichung;
            mfrContact.Email = Kontakt.EMail;
            mfrContact.Fax = Kontakt.Fax;
            mfrContact.MobilePhone = Kontakt.Handy;
            mfrContact.Telephone = Kontakt.Telephone;

            if (MFRParentCompany != null)
                mfrContact.CompanyId = MFRParentCompany.Id;

            if (mfrContact.Id == 0)
            {
                MFRParentCompany.Contacts.Add(mfrContact);
                TenantContainer.AddRelatedObject(MFRParentCompany, "Contacts", mfrContact);
                TenantContainer.SaveChanges();
            }
            else
            {
                TenantContainer.UpdateObject(mfrContact);
                TenantContainer.SaveChanges();
            }
        }
    }
}