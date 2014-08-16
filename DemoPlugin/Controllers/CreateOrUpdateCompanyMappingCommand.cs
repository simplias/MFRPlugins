using System;
using System.Collections.Generic;
using System.Linq;
using DemoPlugin.ERPEntity;
using MFR.Client.Context;
using MFR.Client.MFR.Client;

namespace BSSPlugin.Command
{
    public class CreateOrUpdateCompanyMappingCommand
    {
        public ITenant tenant { get; set; }

        public Unternehmen Unternehmen { get; set; }

        public Kontakt Kontakt { get; set; }

        public Company Execute()
        {

            var container = tenant.CreateApiContainer();

            Company mfrCompany = null;
            mfrCompany =
                container.Companies.Expand(c => c.Contacts)
                      .Where(c => c.MappingId == Unternehmen.EindeutigeNummer)
                      .ToList()
                      .FirstOrDefault();

            if (mfrCompany == null)
            {
                mfrCompany = new Company()
                    {
                        Name = Unternehmen.Name,
                        Id = 0,
                        MappingId = Unternehmen.EindeutigeNummer
                    };
            }

            UpdateCompanyDetails(container, mfrCompany);

            return mfrCompany;
        }

        private void UpdateCompanyDetails(Container container, Company mfrCompany)
        {
            mfrCompany.Name = Unternehmen.Name ;
            mfrCompany.MappingId = Unternehmen.EindeutigeNummer;
            mfrCompany.ExternalId = Unternehmen.EindeutigeNummer;
            mfrCompany.SupportTelephone = "";
            mfrCompany.SupportFax = "";
            mfrCompany.SupportMail = "";

            // we have nothing to do if the modification date has not changed
            if (mfrCompany.Id != 0 && mfrCompany.DateModified.AddMinutes(10) > Unternehmen.Letzte_Aenderung &&
                mfrCompany.DateModified.AddMinutes(-10) < Unternehmen.Letzte_Aenderung)
                return;

            if (mfrCompany.Location == null)
            {
                mfrCompany.Location = new Location();
            }

            mfrCompany.Location.AddressString = Unternehmen.Rechnung_Adresses;
            mfrCompany.Location.City = Unternehmen.Rechnung_Stadt;
            mfrCompany.Location.Postal = Unternehmen.Rechnung_Postleitzahl;

            mfrCompany.IsPhysicalPerson = false;

            if (mfrCompany.Id == 0)
                container.AddToCompanies(mfrCompany);
            else
                container.UpdateObject(mfrCompany);

            container.SaveChanges();

            // Kontakt zum Unternehmen hinzufügen 
            KontakHinzfuegen(container, mfrCompany);
        }

        private void KontakHinzfuegen(Container container, Company mfrCompany)
        {
            var mfrContact = mfrCompany.Contacts.Where(c => c.ExternalId == Unternehmen.EindeutigeNummer).FirstOrDefault();
            if (mfrContact == null)
            {
                mfrContact = new Contact();
                mfrContact.Id = 0;
            }

            mfrContact.ExternalId = Kontakt.EindeutigeNummer;
            mfrContact.FirstName = Kontakt.Vorname;
            mfrContact.LastName = Kontakt.Nachname;
            mfrContact.JobTitle = Kontakt.Berufsbezeichung;
            mfrContact.Email = Kontakt.EMail;
            mfrContact.Fax = Kontakt.Fax;
            mfrContact.MobilePhone = Kontakt.Handy;
            mfrContact.Telephone = Kontakt.Telephone;
            mfrContact.CompanyId = mfrCompany.Id;

            if (mfrContact.Id == 0)
            {
                mfrCompany.Contacts.Add(mfrContact);
                container.AddRelatedObject(mfrCompany, "Contacts", mfrContact);
                container.SaveChanges();
            }
            else
            {
                container.UpdateObject(mfrContact);
                container.SaveChanges();
            }
        }
    }
}