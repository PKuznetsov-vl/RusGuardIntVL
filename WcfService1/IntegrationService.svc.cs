
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
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
    public class ErrorData
    {
        public string Message { get; set; }
    }


    // ПРИМЕЧАНИЕ. Команду "Переименовать" в меню "Рефакторинг" можно использовать для одновременного изменения имени класса "Service1" в коде, SVC-файле и файле конфигурации.
    // ПРИМЕЧАНИЕ. Чтобы запустить клиент проверки WCF для тестирования службы, выберите элементы Service1.svc или Service1.svc.cs в обозревателе решений и начните отладку.
    public class Service1 : IService1
    {
        ErrorData errorData = new ErrorData();

        private static string ServiceUrl = App_Code.Initialize.AppInitialize();

        private static readonly ChannelContainer<ILNetworkService> NetworkService = new ChannelContainer<ILNetworkService>(
            "Admin",
            "",
            WCFBindingFactory.CreateUnlimitedBasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential));

        private static readonly ChannelContainer<ILNetworkConfigurationService> NetworkCnfgService = new ChannelContainer<ILNetworkConfigurationService>(
            "Admin",
            "",
            WCFBindingFactory.CreateUnlimitedBasicHttpBinding(BasicHttpSecurityMode.TransportWithMessageCredential));
        #region


        #endregion
        public void Validate()
        { ServicePointManager.ServerCertificateValidationCallback = (a, b, c, d) => true; }

        public Guid GuidSerialize(string StringID)
        {
            Guid ID = Guid.Parse(StringID);
            return ID;
        }

        public int[] ArraySerialize(string Arr)
        {
            int[] IntArr = Arr.Split(new[] { ',' },
                StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x)).ToArray();
            return IntArr;
        }
        #region Получить уровни доступа

        public string ReturnGetAcsAccessLevels1()
        {
            // var res = GetAcsAccessLevels();
            return string.Format(App_Code.Initialize.AppInitialize().ToString());

        }
        public Stream ReturnGetAcsAccessLevels()
        {
            try
            {
                Validate();
                var res = GetAcsAccessLevels();
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }
        }

        public Stream ReturnGetAcsEmployeeGroups()
        {
            try
            {
                Validate();
                var res = GetAcsEmployeeGroups();
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));

            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }
        public static AcsAccessLevelSlimInfo[] GetAcsAccessLevels()
        {
            //Следует учитывать, что у уровней доступа есть свойство IsRemoved которое означает, что хотя он и существует в БД, 
            //но помечен как удаленный и все операции с ним будут приводить к выбрасыванию исключения. 
            //В нашем случае такие уровни доступа просто не обрабатываем.
            var res = NetworkService.InvokeFunc(_ => _.GetAcsAccessLevelsSlimInfo(), ServiceUrl).Where(_ => !_.IsRemoved).ToArray();
            return res;
        }
 

        /// Получить все группы сотрудников.
        /// </summary>
        /// <returns></returns>
        private static AcsEmployeeGroup[] GetAcsEmployeeGroups()
        {
            //Следует учитывать, что у групп сотрудников есть свойство IsRemoved которое означает, что хотя она и существует в БД, 
            //но помечена как удаленная и все операции с ней будут приводить к выбрасыванию исключения. 
            //В нашем случае такие группы просто не обрабатываем.
            return NetworkService.InvokeFunc(_ => _.GetAcsEmployeeGroups(), ServiceUrl).Where(_ => !_.IsRemoved).ToArray();
        }

        public Stream ReturnGetGuestEmployeeGroup(string id)
        {
            try
            {
                Validate();
                Guid IdGroup = GuidSerialize(id);
                var res = GetGuestEmployeeGroup(IdGroup);
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }

        /// <summary>
        /// Пример получения группы сотрудников для посетителей
        /// </summary>
        /// <returns></returns>
        public static AcsEmployeeGroup GetGuestEmployeeGroup(Guid id)
        {
            AcsEmployeeGroup guestEmployeeGroup = NetworkService.InvokeFunc(_ => _.GetAcsEmployeeGroup(id), ServiceUrl);

            try
            {
                if (guestEmployeeGroup == null)
                {
                    guestEmployeeGroup = NetworkCnfgService.InvokeFunc(_ => _.AddAcsEmployeeGroup(null, "Посетители", ""), ServiceUrl);
                }

                return guestEmployeeGroup;

            }
            //Наименование создаваемой группы равно null или является пустой строкой
            catch (FaultException<ArgumentException>)
            {
                throw;
            }
            //Наименование создаваемой группы содержит более 255 символов
            catch (FaultException<ArgumentOutOfRangeException>)
            {
                throw;
            }
        }

        /// Получить краткую информацию о сотрудниках в группе. Данные каждого сотрудника содержат назначенные ему уровни доступа, ключи, табельный номер
        /// </summary>
        /// <param name="groupId"></param>
        /// 
        public Stream ReturnGetAcsEmployeesInGroup(string groupId)
        {
            try
            {
                Guid ID = GuidSerialize(groupId);
                Validate();
                var res = GetAcsEmployeesInGroup(ID);
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));

            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }

        private static AcsEmployeeSlim[] GetAcsEmployeesInGroup(Guid groupId)
        {
            try
            {
                // 1. Следует учитывать, что у сотрудников есть свойство IsRemoved которое означает, что хотя он и существует в БД, 
                //но помечен как удаленный и все операции с ним будут приводить к выбрасыванию исключения. 
                //В нашем случае такие группы просто не обрабатываем.
                // 2. Сведения о сотрудниках содержат только минимальное необходимые данный, для получения расширенной информации по сотруднику
                //необходимо воспользоваться методом GetAcsEmployee

                return NetworkService.InvokeFunc(_ => _.GetAcsEmployeesByGroup(groupId), ServiceUrl).Where(_ => !_.IsRemoved).ToArray();
            }
            //Исключение возникает в случае, если группа сотрудников отсутствует в БД или ее свойство IsRemoved установлено в true
            catch (FaultException<DataNotFoundException>)
            {
                throw;
            }
        }

        /// <summary>
        /// Получить полные данные о сотрудниках по табельным номерам
        /// </summary>
        /// <returns></returns>
        public static AcsEmployeeFull[] GetAcsEmployeesByTableNumbers(int[] tableNumbers)
        {
            //полная информация содержит паспортные данные, состояние сотрудника(удален), сведения о блокировании - разблокировании сотрудника и т.д
            return NetworkService.InvokeFunc(_ => _.GetAcsEmployeesByTableNumbers(tableNumbers), ServiceUrl);
        }

        public Stream ReturnGetAcsEmployeesByTableNumbers(string tableNumbers)
        {
            try
            {
                Validate();
                int[] IntTableNumbers = ArraySerialize(tableNumbers);
                //int[] IntTableNumbers = tableNumbers.Select(i => (int)i).ToArray();
                var res = GetAcsEmployeesByTableNumbers(IntTableNumbers);
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));

            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }

        /// <summary>
        /// Получить полные данные о сотруднике
        /// </summary>
        /// <returns></returns>
        public static AcsEmployeeFull GetAcsEmployee(Guid id)
        {
            //полная информация содержит паспортные данные, состояние сотрудника(удален), сведения о блокировании - разблокировании сотрудника и т.д
            return NetworkService.InvokeFunc(_ => _.GetAcsEmployee(id), ServiceUrl);
        }
        public static AcsKeyInfo[] GetAcsKeysForEmployee(Guid id)
        {

            return NetworkService.InvokeFunc(_ => _.GetAcsKeysForEmployee(id), ServiceUrl).ToArray();
        }

        public Stream ReturnGetAcsEmployee(string Id)
        {
            try
            {
                Guid ID = GuidSerialize(Id);
                Validate();
                var res = GetAcsEmployee(ID);
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
    System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }
        public Stream ReturnEmployeeKeys(string Id)
        {
            try
            {
                Guid ID = GuidSerialize(Id);
                Validate();
                var key = GetAcsKeysForEmployee(ID);
                string outputKey = JsonConvert.SerializeObject(key, Formatting.Indented);
                return new MemoryStream(Encoding.UTF8.GetBytes(outputKey));
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
System.Net.HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }

        }
        /// <summary>
        /// Получение всех уровней доступа 
        /// </summary>
        /// <returns></returns>
        /// 
        public static AcsAccessLevelSlimInfo[] GetAcsAccessLevelsSlimInfo()
        {

            return NetworkService.InvokeFunc(_ => _.GetAcsAccessLevelsSlimInfo(), ServiceUrl).ToArray();
        }
        public Stream ReturnGetAcsAccessLevelsSlimInfo()
        {
            try
            {
                Validate();
                var res = GetAcsAccessLevelsSlimInfo();
                string output = JsonConvert.SerializeObject(res);
                return new MemoryStream(Encoding.UTF8.GetBytes(output));
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
HttpStatusCode.BadRequest;
                errorData.Message = Exp.Message;
                string ErrorJson = JsonConvert.SerializeObject(errorData);
                return new MemoryStream(Encoding.UTF8.GetBytes(ErrorJson));
            }
        }
        /// <summary>
        /// Добавить фотографию сотруднику, удалить фотографию у сотрудника
        /// </summary>
        /// <param name="employeeId"></param>
        /// <param name="photoNumber"></param>
        /// <param name="data"></param>
        public static void SetAcsEmployeePhoto(Guid employeeId, int photoNumber, byte[] data)
        {

            try
            {
                //photoNumber - параметр, указывающий номер(поизицию фотографии), нумерация начинается с еденицы.
                //В настройках сервисов есть ограничение на объем передаваемых данных в параметре data, 
                //data не должно превышать 4 Mb
                //Для удаления фотографии с позицией, указанной в photoNumber, в параметре data следует указать null
                NetworkCnfgService.InvokeAction(_ => _.SetAcsEmployeePhoto(employeeId, photoNumber, data), ServiceUrl);

            }
            //сотрудник, переданная в параметре employeeId отсутствует в БД или помечен как удаленный(IsRemoved = true).
            catch (FaultException<DataNotFoundException>)
            {
                throw;
            }

        }
        /// <summary>
        /// Получение фото
        /// </summary>
        /// <returns></returns>
        /// 
        public static byte[] GetPhotoEmp(Guid employeeId, int photoNumber)
        {

            return NetworkService.InvokeFunc(_ => _.GetAcsEmployeePhoto(employeeId, photoNumber), ServiceUrl);
        }
        public string GetSetAcsEmployeePhoto(string employeeId, int photoNumber)
        {
            try
            {
                Validate();
                Guid ID = GuidSerialize(employeeId);
                var res = GetPhotoEmp(ID, photoNumber);
                string v = System.Convert.ToBase64String(res);
                return v;
            }
            catch (Exception Exp)
            {
                WebOperationContext.Current.OutgoingResponse.StatusCode =
 HttpStatusCode.BadRequest;
                return Exp.Message; }
        }
        #endregion

        #region Блокировка - разблокировка сотрудников

        /*
         * 1. Сотрудника можно заблокировать/разблокировать (т.е. запретить/разрешить ему перемещаться по объекту). При этом все ключи, которые ему были назначены
         * ранее, так и остаются за ним закреплены и попытка задать эти ключи другому сотруднику вызовет исключение (см. ниже пример назначения ключа сотруднику)
         * 2. Узнать состояние блокировки сотрудника можно, воспользовавшись методом GetAcsEmployee
         */

        /// <summary>
        /// Заблокировать или разблокировать сотрудников.
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="isLocked"></param>
        public static void LockAcsEmployee(Guid[] ids, bool isLocked)
        {
            try
            {
                NetworkCnfgService.InvokeAction(_ => _.LockAcsEmployee(ids, isLocked), ServiceUrl);

            }
            //параметр ids пустой или null
            catch (FaultException<ArgumentException>)
            {
                throw;
            }
            //По крайне мере один из сотрудников, чей идентификатор передан в ids отсутствует в базе
            //или IsRemoved = true
            catch (FaultException<DataNotFoundException>)
            {
                throw;
            }
        }

        public void PostLockAcsEmployee(string employeeId, bool isLocked)
        {

            Validate();
            Guid ID = GuidSerialize(employeeId);
            Guid[] ArrID = new Guid[] { ID };
            LockAcsEmployee(ArrID, isLocked);

        }
        #endregion
        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            else
            { composite.StringValue = "True"; }
            string EmpId = composite.StringValue;
            bool isLocked = composite.BoolValue;
            PostLockAcsEmployee(EmpId, isLocked);
            return composite;
        }
    }
}
