using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    public class DetailOrderViewModel : BaseViewModel
    {
        private int _id;
        private Workers _user;
        private Prices _prices;
        private string _carton;
        private string _plenka;
        private string _poddon;
        private bool _isCarton;
        private bool _isPlenka;
        private bool _isPoddon;
        private bool _isCartonCanChanged;
        private bool _isPlenkaCanChanged;
        private bool _isPoddonCanChanged;
        private string _cartonAmount;
        private string _plenkaAmount;
        private string _poddonAmount;
        private IAlertService alertService;
        private bool _canUpdate;
        private bool _isTaken;
        private bool _isWeighted;
        private bool _isFinished;
        private Request _request;
        private bool _canEdit;
        public ObservableCollection<Workers> Users { get; }
        public Command LoadOrdersCommand { get; }
        public Command UpdateOrder { get; }
        public Command UpdateState { get; }

        public ObservableCollection<Request> Requests { get; }
        public DetailOrderViewModel()
        {
            Requests = new ObservableCollection<Request>();
            Users = new ObservableCollection<Workers>();
            Title = $"Данные о заявке";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadOrdersCommand = new Command(async () => await GetUserData());
            UpdateOrder = new Command(OnEditClicked);
            UpdateState = new Command(OnStateEditClicked);
        }
        private async void OnEditClicked(object obj)
        {
            try
            {
                await UpdateIndicationsAsync(Request);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }
        public bool CanEdit
        {
            get => _canEdit;
            set => SetProperty(ref _canEdit, value);
        }
        public string Plenka
        {
            get => _plenka;
            set => SetProperty(ref _plenka, value);
        }
        public string Poddon
        {
            get => _poddon;
            set => SetProperty(ref _poddon, value);
        }
        public string Carton
        {
            get => _carton;
            set => SetProperty(ref _carton, value);
        }
        //
        public string PlenkaAmount
        {
            get => _plenkaAmount;
            set => SetProperty(ref _plenkaAmount, value);
        }
        public string PoddonAmount
        {
            get => _poddonAmount;
            set => SetProperty(ref _poddonAmount, value);
        }
        public string CartonAmount
        {
            get => _cartonAmount;
            set => SetProperty(ref _cartonAmount, value);
        }
        //
        public bool IsCarton
        {
            get => _isCarton;
            set => SetProperty(ref _isCarton, value);
        }
        public bool IsPoddon
        {
            get => _isPoddon;
            set => SetProperty(ref _isPoddon, value);
        }
        public bool IsPlenka
        {
            get => _isPlenka;
            set => SetProperty(ref _isPlenka, value);
        }
        public bool IsCartonCanChanged
        {
            get => _isCartonCanChanged;
            set => SetProperty(ref _isCartonCanChanged, value);
        }
        public bool IsPoddonCanChanged
        {
            get => _isPoddonCanChanged;
            set => SetProperty(ref _isPoddonCanChanged, value);
        }
        public bool IsPlenkaCanChanged
        {
            get => _isPlenkaCanChanged;
            set => SetProperty(ref _isPlenkaCanChanged, value);
        }
        public bool CanUpdate
        {
            get => _canUpdate;
            set => SetProperty(ref _canUpdate, value);
        }
        public bool IsTaken
        {
            get => _isTaken;
            set => SetProperty(ref _isTaken, value);
        }
        public bool IsWeighted
        {
            get => _isWeighted;
            set => SetProperty(ref _isWeighted, value);
        }
        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value);
        }

        public Workers User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }
        public Prices Prices
        {
            get => _prices;
            set => SetProperty(ref _prices, value);
        }
        private async void OnStateEditClicked(object obj)
        {
            try
            {
                await UpdateStateAsync(Request);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }
        private async Task GetUserData()
        {
            try
            {
                Users.Clear();
                alertService.ShowToast("Загрузка...", 1f);

                var list = await App.Database.GetUsersAsync();
                if (list.Count > 0)
                {
                    foreach (var user in list)
                    {
                        Users.Add(user);
                    }
                    User = Users[0];
                }
                Requests.Clear();
                HttpClient _tokenclient = new HttpClient();
                var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/get/{Id}");
                var _jsonResults = JsonConvert.DeserializeObject<Request>(_responseToken);
                _jsonResults.RuState = RuState(_jsonResults.State);
                //
                HttpClient _tokenClientPrice = new HttpClient();
                var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price");
                var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
                Prices = _jsonResultsPrice;
                //
                Requests.Add(_jsonResults);
                Request = Requests[0];
                CanEdit = AllowEdit(Request,User);
                //
                
                GetMaterialsInfo();
                CheckUserRights();

                alertService.ShowToast("Данные получены...", 1f);

            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1f);
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void CheckUserRights()
        {
            if (User.Role == "root")
            {
                switch (Request.RuState)
                {
                    case "Создан":
                        IsTaken = true;
                        IsWeighted = false;
                        IsFinished = false;
                        break;
                    case "Вывезен":
                        IsTaken = false;
                        IsWeighted = true;
                        IsFinished = false;
                        break;
                    case "Взвешен":
                        IsTaken = false;
                        IsWeighted = false;
                        IsFinished = true;
                        break;
                }
            }
            if (User.Role == "admin")
            {
                switch (Request.RuState)
                {
                    case "Создан":
                        IsTaken = true;
                        IsWeighted = false;
                        IsFinished = false;
                        break;
                    case "Вывезен":
                        IsTaken = false;
                        IsWeighted = true;
                        IsFinished = false;
                        break;
                    case "Взвешен":
                        IsTaken = false;
                        IsWeighted = false;
                        IsFinished = false;
                        break;
                }
            }
            if (User.Role != "admin" && User.Role != "root")
            {
                switch (Request.RuState)
                {
                    case "Создан":
                        IsTaken = true;
                        IsWeighted = false;
                        IsFinished = false;
                        break;
                    case "Вывезен":
                        IsTaken = false;
                        IsWeighted = true;
                        IsFinished = false;
                        break;
                    case "Взвешен":
                        IsTaken = false;
                        IsWeighted = false;
                        IsFinished = false;
                        break;
                }
            }
        }

        private void GetMaterialsInfo()
        {
            IsPoddon = CheckMaterial(Request.Materials, "Поддоны");
            IsCarton = CheckMaterial(Request.Materials, "Картон");
            IsPlenka = CheckMaterial(Request.Materials, "Пленка");
            PlenkaAmount = CheckMaterialAmount(Request.Materials, 1);
            CartonAmount = CheckMaterialAmount(Request.Materials, 3);
            PoddonAmount = CheckMaterialAmount(Request.Materials, 5);
            if (IsPoddon)
            {
                Poddon = $"Поддоны:{PoddonAmount};";
            }
            if (IsCarton)
            {
                Carton = $"Картон:{CartonAmount};";
            }
            if (IsPlenka)
            {
                Plenka = $"Пленка:{PlenkaAmount};";
            }
        }


        bool CheckMaterial(string _data, string _pattern)
        {
            bool res = false;
            var _separatedData = _data.Split(':', ';');
            foreach (var x in _separatedData)
            {
                if (x == _pattern)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        string CheckMaterialAmount(string _data, int index)
        {
            var res = string.Empty;
            var _separatedData = _data.Split(':', ';');
            if (_separatedData.Count() == 7)
            {
                res = _separatedData[index];
            }
            return res;
        }

        string RuState(string data)
        {
            var res = string.Empty;
            switch (data)
            {
                case "created":
                    res = "Создан";

                    break;
                case "actual":
                    res = "Вывезен";

                    break;
                case "weighted":
                    res = "Взвешен";

                    break;
                case "finished":
                    res = "Завершен";

                    break;
            }
            return res;
        }

        public Request Request
        {
            get => _request;
            set => SetProperty(ref _request, value);
        }

        public int Id
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }





        internal void OnAppear()
        {
            IsBusy = true;
        }

        public async Task<Request> UpdateIndicationsAsync(Request indications)
        {
            if (indications.State.Length == "created".Length && (User.Role.Length == "admin".Length || User.Role.Length == "root".Length))
            {

                return await ChangeData(indications);
            }
            if (indications.State.Length == "actual".Length && (User.Role.Length == "employeе".Length || User.Role.Length == "root".Length))
            {
                return await ChangeData(indications);
            }
            if ((indications.State.Length == "weighted".Length || indications.State.Length == "finished".Length) && User.Role.Length == "root".Length)
            {
                return await ChangeData(indications);
            }
            else
            {
                alertService.ShowToast("У вас недостаточно прав!!!", 1);
                return null;
            }
        }


      

        private async Task<Request> ChangeData(Request indications)
        {
            if (indications.State == "created")
            {
                indications.WhoTook = User.Name;
                indications.TakenData = DateTime.Now.ToString().Split(' ')[0].ToString();
            }
            if (indications.State == "actual")
            {
                indications.WhoWeighed = User.Name;
                indications.WeightData = DateTime.Now.ToString().Split(' ')[0].ToString();
            }
            if (indications.State == "weighted")
            {
                indications.WhoClosed = User.Name;
                indications.FinishedData = DateTime.Now.ToString().Split(' ')[0].ToString();
            }

            if (indications.OldCartonPrice == null)
            {
                indications.OldCartonPrice = Prices.Carton;
            }
            if (indications.OldPlenkaPrice == null)
            {
                indications.OldPlenkaPrice = Prices.Plenka;
            }
            if (indications.OldPoddonPrice == null)
            {
                indications.OldPoddonPrice = Prices.Poddon;
            }




            if (CheckAmount(indications.State, IsPlenka, Plenka, PlenkaAmount) && CheckAmount(indications.State, IsPoddon, Poddon, PoddonAmount)
                && CheckAmount(indications.State, IsCarton, Carton, CartonAmount))
                {
                    
                    indications.LastModified = DateTime.Now.ToString().Split(' ')[0].ToString();

                    indications.Materials = $"{Validator(Plenka, PlenkaAmount)}{Validator(Carton, CartonAmount)}{Validator(Poddon, PoddonAmount)}";
                    alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
                    IsBusy = true;
                    HttpClient client = new HttpClient();
                    var response = await client.PutAsync($"{GlobalSettings.HostUrl}api/order/{User.Token}",
                    new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
                    Encoding.UTF8, "application/json"));
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                        return null;
                    }
                    else
                    {
                        var res = JsonConvert.DeserializeObject<Request>(response.Content.ReadAsStringAsync().Result);
                        res.RuState = RuState(res.State);
                        Request = res;
                        GetMaterialsInfo();

                        //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                        return null;
                    }
                }
            
            else
            {
                alertService.ShowToast("Активные материалы не заполнены!!!", 1f);
                return indications;
            }

        }

        bool AllowEdit(Request request, Workers workers)
        {
            if (request.WeightData != null && request.TakenData != null)
            {
                TimeSpan diff = TimeSpan.FromDays(float.Parse(request.WeightData.Split('.')[0]) - float.Parse(request.TakenData.Split('.')[0]));
                if (diff.Days <= 1 && workers.Role.Length=="employee".Length && request.State.Length=="weighted".Length)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public bool CheckAmount(string state,bool _isToggle,string _data, string _amount)
        {
           
            if (_isToggle)
            {
                if (!String.IsNullOrEmpty(_data) && !String.IsNullOrEmpty(_amount))
                {
                    return true;
                }
                else
                {
                    if (state.Length == "created".Length)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        public string Validator(string _data, string _amount)
        {
            if (String.IsNullOrEmpty(_data))
            {
                return ":;";
            }
            else
            {
              
                //return $"{_data}:{_amount};";
                return $"{_data}";
            }
        }
        public async Task<Request> UpdateStateAsync(Request indications)
        {

            if (indications.OldCartonPrice == null)
            {
                indications.OldCartonPrice = Prices.Carton;
            }
            if (indications.OldPlenkaPrice == null)
            {
                indications.OldPlenkaPrice = Prices.Plenka;
            }
            if (indications.OldPoddonPrice == null)
            {
                indications.OldPoddonPrice = Prices.Poddon;
            }
            indications.LastModified = DateTime.Now.ToString().Split(' ')[0].ToString();
            if (indications.State.Length == "created".Length)
            {
                await ChangeData(indications);
                return await ChangeState(indications);
            }
            if ((indications.State.Length == "actual".Length) && (User.Role.Length=="admin".Length||User.Role.Length == "employee".Length || User.Role.Length == "root".Length))
            {
                await ChangeData(indications);
                return await ChangeState(indications);
            }
            if ((indications.State.Length == "weighted".Length) && (User.Role.Length == "employee".Length || User.Role.Length == "root".Length))
            {
                await ChangeData(indications);
                return await ChangeState(indications);
            }
            else
            {
                alertService.ShowToast("Недостаточно прав!!!", 1);
                return indications;
            }

           
        }

        private async Task<Request> ChangeState(Request indications)
        {
            


            if (CheckAmount(indications.State, IsPlenka, Plenka, PlenkaAmount) && CheckAmount(indications.State, IsPoddon, Poddon, PoddonAmount)
                && CheckAmount(indications.State, IsCarton, Carton, CartonAmount))
            {
                alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
                IsBusy = true;
                HttpClient client = new HttpClient();
                var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/order/update_state/{indications.Id}",
                new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
                Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                    return null;
                }
                else
                {
                    var res = JsonConvert.DeserializeObject<Root>(response.Content.ReadAsStringAsync().Result);
                    
                    Request = new Request()
                    {
                        Id = res.Result.Id,
                         Address = res.Result.Address,
                          Data = res.Result.Data,
                           Description = res.Result.Description,
                            Materials = res.Result.Materials,
                             Name = res.Result.Name,
                              Organization = res.Result.Organization,
                               RelatedOperator = res.Result.RelatedOperator,
                                RelatedOrganizationId = res.Result.RelatedOrganizationId,
                                 RuState = RuState(res.Result.State),
                                  State = res.Result.State,
                                   TakenData = res.Result.TakenData,
                                    FinishedData = res.Result.FinishedData,
                                     LastModified = res.Result.LastModified,
                                      Price = res.Result.Price,
                                       WeightData = res.Result.WeightData,
                                        WhoClosed = res.Result.WhoClosed,
                                         WhoTook = res.Result.WhoTook,
                                          WhoWeighed = res.Result.WhoWeighed,
                                           OldCartonPrice = res.Result.OldCartonPrice,
                                            OldPlenkaPrice = res.Result.OldPlenkaPrice,
                                             OldPoddonPrice = res.Result.OldPoddonPrice,
                    };
                    
                   
                   
                    CheckUserRights();
                    //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                    return null;
                }
            }
            else
            {
                alertService.ShowToast("Активные материалы не заполнены!!!", 1);
                return indications;
            }
        }
    }
}
