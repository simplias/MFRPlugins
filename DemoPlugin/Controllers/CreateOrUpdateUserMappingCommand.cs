using System;
using System.Collections.Generic;
using System.Linq;
using DemoPlugin.ERPEntity;
using MFR.Client.MFR.Client;
using MFR.Client.Context;

namespace BSSPlugin.Command
{
    public class CreateOrUpdateUserMappingCommand
    {
        public ITenant tenant { get; set; }

        public Benutzer Benutzer { get; set; }

        public Container TenantContainer { get; set; }

        public Contact Execute()
        {
            TenantContainer = tenant.CreateApiContainer();

            var jobTitle = Benutzer.EindeutigeNummer;

            var contact = TenantContainer.Contacts.Where(c => c.JobTitle == jobTitle).FirstOrDefault();

            if(contact == null)
                throw new InvalidOperationException("Mitarbeiter mit JobTitle: " + jobTitle + " existiert nicht. Bitte legen Sie ihn in MFR an.");

            var hashMFRContact = contact.FirstName + contact.LastName + contact.MobilePhone + contact.Telephone;
            var hashCaveaContact = Benutzer.Vorname + Benutzer.Nachname;

            contact.FirstName = Benutzer.Vorname;
            contact.LastName = Benutzer.Nachname;

            if (hashMFRContact != hashCaveaContact)
            {
                TenantContainer.UpdateObject(contact);
                TenantContainer.SaveChanges();
            }
            return contact;
        }
    }
}