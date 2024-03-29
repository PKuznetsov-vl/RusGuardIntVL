﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VVIInvestment.RusGuard.DAL.Entities.Entity;
using VVIInvestment.RusGuard.DAL.Entities.Entity.ACS.AccessLevels;
using VVIInvestment.RusGuard.DAL.Entities.Entity.ACS.AccessLevels.AcsAccesPoints;
using VVIInvestment.RusGuard.DAL.Entities.Entity.ACS.AcsKeys;
using VVIInvestment.RusGuard.DAL.Entities.Entity.ACS.Employees;
using VVIInvestment.RusGuard.DAL.Entities.Entity.Log;
using VVIInvestment.RusGuard.DAL.Entities.Exceptions;
using VVIInvestment.RusGuard.Net.Services;
using VVIInvestment.RusGuard.Net.Services.Entities;
using VVIInvestment.RusGuard.Net.Services.Wcf;

namespace WcfService1
{
    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени интерфейса "IService1" в коде и файле конфигурации.

    [ServiceContract]
    public interface IService1
    {

        [OperationContract]
        [WebGet(UriTemplate = "/Test", ResponseFormat = WebMessageFormat.Json)]

        string ReturnGetAcsAccessLevels1();
        [OperationContract]
        [WebGet(UriTemplate = "/GetAcsAccessLevels", ResponseFormat = WebMessageFormat.Json)]
        Stream ReturnGetAcsAccessLevels();
        [OperationContract]
        [WebGet(UriTemplate = "/GetAcsEmployeeGroups", ResponseFormat = WebMessageFormat.Json)]
        Stream ReturnGetAcsEmployeeGroups();
        [OperationContract]
        [WebGet(UriTemplate = "/GetGuestEmployeeGroup?GuidId={GuidId}", ResponseFormat = WebMessageFormat.Json)]
        Stream ReturnGetGuestEmployeeGroup(string GuidId);
        [OperationContract]
        [WebGet(UriTemplate = "/GetAcsEmployeesInGroup?GroupId={value}",ResponseFormat =WebMessageFormat.Json)]
        Stream ReturnGetAcsEmployeesInGroup(string value);
        [OperationContract]
        [WebGet(UriTemplate = "/GetAcsEmployeesByTableNumbers?TableNum={value}", ResponseFormat = WebMessageFormat.Json)]
        Stream ReturnGetAcsEmployeesByTableNumbers(string value);
        [OperationContract]

        [WebGet(UriTemplate = "/GetAcsEmployee?PersonGuidId={value}",ResponseFormat =WebMessageFormat.Json,BodyStyle =WebMessageBodyStyle.Bare)]
        Stream ReturnGetAcsEmployee(string value);
        [OperationContract]

        [WebGet(UriTemplate = "/GetEmployeeKeys?PersonGuidId={value}", ResponseFormat = WebMessageFormat.Json)]
        Stream ReturnEmployeeKeys(string value);
        [OperationContract]
        [WebGet(UriTemplate = "/GetPhotoEmployee?PersonGuidId={employeeId}&photoNumber={photoNumber}", ResponseFormat =WebMessageFormat.Json)     ]
        string GetSetAcsEmployeePhoto(string employeeId, int photoNumber);
        

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/LockAcsEmployee",
             RequestFormat = WebMessageFormat.Json,
             ResponseFormat = WebMessageFormat.Json,
             BodyStyle = WebMessageBodyStyle.Bare)]
        CompositeType GetDataUsingDataContract(CompositeType composite);
      

        #region
        /*    [OperationContract]
            [WebInvoke(Method = "POST", 
                UriTemplate = "/GetDataUsingDataContract", 
                ResponseFormat = WebMessageFormat.Json,
                RequestFormat =WebMessageFormat.Json, 
                BodyStyle = WebMessageBodyStyle.Bare)]

            CompositeType GetDataUsingDataContract(CompositeType composite);*/

        #endregion

    }

    // Используйте контракт данных, как показано в примере ниже, чтобы добавить составные типы к операциям служб.
    [DataContract]
    public class CompositeType
    {
        [DataMember]
        public string StringValue;
        public bool BoolValue;

    }
  
   

    }

