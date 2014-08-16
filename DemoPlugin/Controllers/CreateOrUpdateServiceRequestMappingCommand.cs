using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Services.Client;
using System.Linq;
using DemoPlugin.ERPEntity;
using DemoPlugin.Services;
using MFR.Client.Context;
using MFR.Client.MFR.Client;
using Newtonsoft.Json.Linq;

namespace BSSPlugin.Command
{
    // Anzahl der Anfragen 2

    public class CreateOrUpdateServiceRequestMappingCommand
    {
        public CreateOrUpdateServiceRequestMappingCommand()
        {
        }

        public ITenant tenant { get; set; }

        public Settings synchSettings;

        //! \brief Is the job from cavea
        public Auftrag Auftrag { get; set; }

        public Einsatzort Einsatzort { get; set; }

        public Company MfrCustomer { get; set; }

        public Container TenantContainer { get; set; }

        public Benutzer Mitarbeiter { get; set; }

        protected List<ServiceObject> ServiceObjectsToAdd { get; set; }

        public void Execute()
        {
            TenantContainer = tenant.CreateApiContainer();

            ServiceRequest mfrServiceRequest = null;
            ServiceObjectsToAdd = new List<ServiceObject>();
            mfrServiceRequest = TenantContainer.ServiceRequests.Expand(j => j.Customer)
                                               .Expand(j => j.Steps)
                                               .Expand(j => j.Items)
                                               .Expand("Appointments/Contacts")
                                               .Expand(j => j.ServiceObjects)
                                               .Where(c => c.ExternalId == Auftrag.EindeutigeNummer)
                                               .ToList()
                                               .FirstOrDefault();

            if (mfrServiceRequest == null)
            {
                mfrServiceRequest = new ServiceRequest()
                    {
                        Name = "Auftragsbeispiel",
                        Id = 0,
                    };

                TenantContainer.AddToServiceRequests(mfrServiceRequest);
            }

            UpdateJobDetails(mfrServiceRequest);
            TenantContainer.SaveChanges(SaveChangesOptions.Batch);

            if (ServiceObjectsToAdd.Count != 0)
            {
                ServiceObjectsToAdd.ForEach(s => mfrServiceRequest.ServiceObjects.Add(s));
                TenantContainer.UpdateObject(mfrServiceRequest);
                TenantContainer.SaveChanges();
            }
        }

        private void UpdateJobDetails(ServiceRequest mfrServiceRequest)
        {
            mfrServiceRequest.Name = MfrCustomer.Name;
            mfrServiceRequest.CustomerId = MfrCustomer.Id;
            mfrServiceRequest.ExternalId = Auftrag.EindeutigeNummer;
            mfrServiceRequest.Description = "Aufgabenbeschreibung";

            TenantContainer.SaveChanges(); // create the job

            // service objekte hinzufügen

            var serviceObject = mfrServiceRequest.ServiceObjects.Where(s => s.ExternalId == Einsatzort.EindeutigeNummer).FirstOrDefault();
            if (serviceObject == null)
            {
                serviceObject = TenantContainer.ServiceObjects.Where(s => s.ExternalId == Einsatzort.EindeutigeNummer).FirstOrDefault();
                if (serviceObject == null)
                {
                    throw new InvalidOperationException("ServiceObject not found");
                }

                mfrServiceRequest.ServiceObjects.Add(serviceObject);
                TenantContainer.AddLink(mfrServiceRequest, "ServiceObjects", serviceObject);
            }

            // send a user notification and assign the job
            CreateOrUpdateAppointment(mfrServiceRequest);
        }

        private void CreateOrUpdateAppointment(ServiceRequest mfrServiceRequest)
        {
            if (mfrServiceRequest.ServiceObjects.Count == 0)
            {
                //("ERROR: Job " + mfrServiceRequest.ExternalId + "has no service objects. Appointment could not be created.");
                //mfrServiceRequest.Description = "ERROR: Job " + mfrServiceRequest.ExternalId + "has no service objects. Appointment could not be created.";
                TenantContainer.SaveChanges();
                return;
            }

            var mfrTechnicianContainer = GetTechniciansForAuftrag(Auftrag);

            var appointment = mfrServiceRequest.Appointments.FirstOrDefault() ?? new Appointment()
                {
                    Id = 0
                };

            foreach (
                var technician in
                    mfrTechnicianContainer.Where(technician => !appointment.ContactIds.Contains(technician.Id)))
            {
                appointment.ContactIds.Add(technician.Id);
            }

            appointment.StartDateTime = Auftrag.TerminBegin;
            appointment.EndDateTime = Auftrag.TerminEnde;

            if (appointment.StartDateTime.Value.AddMinutes(30) > appointment.EndDateTime.Value)
                appointment.EndDateTime = appointment.StartDateTime.Value.AddHours(1);

            if (appointment.Id == 0)
            {
                mfrServiceRequest.Appointments.Add(appointment);
                TenantContainer.AddRelatedObject(mfrServiceRequest, "Appointments", appointment);
                TenantContainer.SaveChanges();
            }
            else
            {
                TenantContainer.UpdateObject(appointment);
                TenantContainer.SaveChanges();
            }
        }

        public virtual List<Contact> GetTechniciansForAuftrag(Auftrag auftrag)
        {
            var technicians = new List<Contact>();
            var mfrTechnician = new CreateOrUpdateUserMappingCommand()
                                        {
                                            tenant = tenant,
                                            Benutzer = Mitarbeiter,
                                        }.Execute();

            technicians.Add(mfrTechnician);

            return technicians;
        }
    }
}