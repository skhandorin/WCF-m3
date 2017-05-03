﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeoLib.Contracts;
using GeoLib.Data;
using System.Threading;
using System.ServiceModel;
using System.Windows.Forms;
using System.Transactions;
using System.Security.Principal;

namespace GeoLib.Services
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true
                   , InstanceContextMode = InstanceContextMode.PerSession
                   , UseSynchronizationContext = false
                   , ConcurrencyMode = ConcurrencyMode.Reentrant
                   , ReleaseServiceInstanceOnTransactionComplete = false
    )]
    public class GeoManager : IGeoService, IGeoServiceAdmin
    {
        public GeoManager()
        {
        }

        public GeoManager(IZipCodeRepository zipCodeRepository)
        {
            _ZipCodeRepository = zipCodeRepository;
        }

        public GeoManager(IStateRepository stateRepository)
        {
            _StateRepository = stateRepository;
        }

        public GeoManager(IZipCodeRepository zipCodeRepository, IStateRepository stateRepository)
        {
            _ZipCodeRepository = zipCodeRepository;
            _StateRepository = stateRepository;
        }

        IZipCodeRepository _ZipCodeRepository = null;
        IStateRepository _StateRepository = null;

        int _Counter = 0;

        public ZipCodeData GetZipInfo(string zip)
        {
            ZipCodeData zipCodeData = null;

            string hostIdentity = WindowsIdentity.GetCurrent().Name;
            string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name;
            string threadIdentity = Thread.CurrentPrincipal.Identity.Name;

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
            if (zipCodeEntity != null)
            {
                zipCodeData = new ZipCodeData()
                {
                    City = zipCodeEntity.City,
                    State = zipCodeEntity.State.Abbreviation,
                    ZipCode = zipCodeEntity.Zip
                };
            }
            else
            {
                //throw new ApplicationException($"Zip code {zip} not found.");
                //throw new FaultException($"Zip code {zip} not found.");

                //ApplicationException ex = new ApplicationException($"Zip code {zip} not found.");
                //throw new FaultException<ApplicationException>(ex, "Just another message");

                NotFoundData data = new NotFoundData()
                {
                    Message = $"Zip code {zip} not found.",
                    When = DateTime.Now.ToString(),
                    User = "Sergey"
                };
                throw new FaultException<NotFoundData>(data, "Just another message");
            }

            _Counter++;
            //Console.WriteLine($"Counter = {_Counter.ToString()}");
            //Thread.Sleep(10000);
            //MessageBox.Show($"{zip} = {zipCodeData.City}, {zipCodeData.State}", "Call Counter " + _Counter);

            return zipCodeData;
        }

        public IEnumerable<string> GetStates(bool primaryOnly)
        {
            List<string> stateData = new List<string>();

            IStateRepository stateRepository = _StateRepository ?? new StateRepository();

            IEnumerable<State> states = stateRepository.Get(primaryOnly);
            if (states != null)
            {
                foreach (State state in states)
                {
                    stateData.Add(state.Abbreviation);
                }
            }
            return stateData;
        }

        public IEnumerable<ZipCodeData> GetZips(string state)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            var zips = zipCodeRepository.GetByState(state);
            if (zips != null)
            {
                foreach (var zipCode in zips)
                {
                    zipCodeData.Add(new ZipCodeData()
                    {
                        City = zipCode.City,
                        State = zipCode.State.Abbreviation,
                        ZipCode = zipCode.Zip
                    });
                }
            }
            return zipCodeData;
        }

        public IEnumerable<ZipCodeData> GetZips(string zip, int range)
        {
            List<ZipCodeData> zipCodeData = new List<ZipCodeData>();

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zip);
            IEnumerable<ZipCode> zips = zipCodeRepository.GetZipsForRange(zipCodeEntity, range);
            if (zips != null)
            {
                foreach (var zipCode in zips)
                {
                    zipCodeData.Add(new ZipCodeData()
                    {
                        City = zipCode.City,
                        State = zipCode.State.Abbreviation,
                        ZipCode = zipCode.Zip
                    });
                }
            }
            return zipCodeData;
        }

        [OperationBehavior(TransactionScopeRequired = true)]
        public void UpdateZipCity(string zip, string city)
        {
            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            ZipCode zipEntity = zipCodeRepository.GetByZip(zip);
            if (zipEntity != null)
            {
                zipEntity.City = city;
                zipCodeRepository.Update(zipEntity);
            }
        }

        [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = true)]
        public int UpdateZipCity(IEnumerable<ZipCityData> zipCityData)
        {
            string hostIdentity = WindowsIdentity.GetCurrent().Name;
            string primaryIdentity = ServiceSecurityContext.Current.PrimaryIdentity.Name;
            string windowsIdentity = ServiceSecurityContext.Current.WindowsIdentity.Name;
            string threadIdentity = Thread.CurrentPrincipal.Identity.Name;

            IZipCodeRepository zipCodeRepository = _ZipCodeRepository ?? new ZipCodeRepository();

            //Dictionary<string, string> cityBatch = new Dictionary<string, string>();
            //foreach (ZipCityData zipCityItem in zipCityData)
            //{
            //    cityBatch.Add(zipCityItem.ZipCode, zipCityItem.City);
            //}
            //zipCodeRepository.UpdateCityBatch(cityBatch);
                
            int counter = 0;

            foreach (var zipCityItem in zipCityData)
            {
                counter++;

                ZipCode zipCodeEntity = zipCodeRepository.GetByZip(zipCityItem.ZipCode);
                zipCodeEntity.City = zipCityItem.City;
                ZipCode updateItem = zipCodeRepository.Update(zipCodeEntity);

                //if (counter == 3)
                //    throw new FaultException("Manually caused error.");

                IUpdateZipCallback callback = OperationContext.Current.GetCallbackChannel<IUpdateZipCallback>();
                if (callback != null)
                {
                    callback.ZipUpdated(zipCityItem);
                    Thread.Sleep(100);
                }
            }

            return counter;
        }

        public void OneWayExample()
        {
            MessageBox.Show("Made it to the server.");
        }
    }
}
