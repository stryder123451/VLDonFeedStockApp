using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Plugin.FirebasePushNotification;
using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using VLDonFeedStockApp.Models;
using VLDonFeedStockApp.Services;
using VLDonFeedStockApp.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace VLDonFeedStockApp.ViewModels
{
    [QueryProperty(nameof(Id), nameof(Id))]
    [QueryProperty(nameof(Address), nameof(Address))]
    public class DetailOrderViewModel : BaseViewModel
    {
        private int _id;
        private Workers _user;
        private FileResult _uploadedFile;
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
        private bool _isLoading;
        private bool _stateLoading;
        private GridLength _loadingRow;
        private GridLength _loadingItems;
        private Color _likeColor;
        private Color _dislikeColor;
        //private bool _poddonPhotoLoaded;
        //private bool _plenkaPhotoLoaded;
        //private bool _cartonPhotoLoaded;

        public string _address;
        public ObservableCollection<Workers> Users { get; }
        public ObservableCollection<AttachedFiles> RelatedFiles { get; }
  
        public Command LoadOrdersCommand { get; }
        public Command UpdateOrder { get; }
        public Command ChoosePhoto { get; }
        public Command<ImageSource> ShowPhoto { get; }
        public Command<ImageSource> ShowPhotoPlenka { get; }
        public Command<ImageSource> ShowPhotoCarton { get; }
        public Command MakePhoto { get; }
        public Command MakeVideo { get; }
        public Command UpdateState { get; }
        public Command BackCommand { get; }
        public Command<AttachedFiles> DownloadFile { get; }
        public Command SetLike { get; }
        public Command SetDislike { get; }
        public ObservableCollection<Request> Requests { get; }

        public DetailOrderViewModel()
        {
            Requests = new ObservableCollection<Request>();
            Users = new ObservableCollection<Workers>();
            RelatedFiles = new ObservableCollection<AttachedFiles>();
            Title = $"Данные о заявке";
            alertService = DependencyService.Resolve<IAlertService>();
            LoadOrdersCommand = new Command(async () => await GetUserData());
            UpdateOrder = new Command(OnEditClicked);
            ChoosePhoto = new Command(AttachFileMethod);
            //ShowPhoto = new Command<ImageSource>(OnItemSelectedPoddon);
            //ShowPhotoPlenka = new Command<ImageSource>(OnItemSelectedPlenka);
            //ShowPhotoCarton = new Command<ImageSource>(OnItemSelectedCarton);
            MakePhoto = new Command(AttachPhotoMethod);
            MakeVideo = new Command(AttachVideoMethod);
            UpdateState = new Command(OnStateEditClicked);
            BackCommand = new Command(OnCancel);
            DownloadFile = new Command<AttachedFiles>(OnItemSelected);
            SetLike = new Command(Like);
            SetDislike = new Command(Dislike);
        }

        private async void Dislike(object obj)
        {
            Request.Mark = await SetMark("dislike");
            CheckLikes();
        }

        private async void Like(object obj)
        {
            Request.Mark = await SetMark("like");
            CheckLikes();
        }

        public async Task<string> SetMark(string mark)
        {
            HttpClient _tokenclient = new HttpClient();
            _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
            var _responseToken = await _tokenclient.GetAsync($"{GlobalSettings.HostUrl}api/order/setmark/{mark}/{Request.Address}/{DateTime.Now.ToString("MMMM", new System.Globalization.CultureInfo("en-US"))}/{Request.Id}/{User.Login}");
            if (_responseToken.IsSuccessStatusCode)
            {
               // alertService.ShowToast($"{Request.Mark}", 1);
                return _responseToken.Content.ReadAsStringAsync().Result;
            }
            else
            {
                alertService.ShowToast($"Ошибка...", 1);
                return null;
            }
        }


        private async void AttachVideoMethod(object obj)
        {
            await AttachVideoMethodAsync();
            //await UploadPhotoToServer(UploadedFile, "file");
        }

        private async void AttachPhotoMethod(object obj)
        {
            UploadedFile = await AttachPhotoMethodAsync();
            await UploadPhotoToServer(UploadedFile, "file");
        }

        private async Task<FileResult> AttachPhotoMethodAsync()
        {
            var file = await MediaPicker.CapturePhotoAsync();

            if (file == null)
            {
                return null;
            }
            else
            {
                return file;
            }
        }

        //async Task LoadVideoAsync(FileResult photo)
        //{
        //    // canceled
        //    if (photo == null)
        //    {
        //        UploadedFile = null;
        //        return;
        //    }
        //    // save the file into local storage
        //    var newFile = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        //    using (var stream = await photo.OpenReadAsync())
        //    using (var newStream = File.OpenWrite(newFile))
        //        await stream.CopyToAsync(newStream);

        //    UploadedFile = newFile;
        //}

        private async Task AttachVideoMethodAsync()
        {


            var file = await MediaPicker.CaptureVideoAsync();

            if (file == null)
            {
                await alertService.ShowMessage("Error", "Something wrong 0_o");
            }
            else
            {

                UploadedFile = await AttachFile();
                await UploadPhotoToServer(UploadedFile, "file");
            }


        }
        

        

        private async void AttachFileMethod(object obj)
        {
           UploadedFile = await AttachFile();
           await UploadPhotoToServer(UploadedFile, "file");
        }

        private async Task<FileResult> AttachFile()
        {
            var file = await FilePicker.PickAsync();

            if (file == null)
            {
                return null;
            }
            else
            {
                return file;
            }
            
            //Task upload = await UploadPhotoToServer(file, "file");

            //var result = await FilePicker.PickAsync(new PickOptions()
            //{
            //    PickerTitle = "Выберите файлы...",
            //});
            //if (result != null)
            //{
            //    var stream = await result.OpenReadAsync();
            //    if (stream != null)
            //    {
            //        var file = ImageSource.FromStream(() => stream);
            //        UploadPhotoToServer(file,"file")
            //           //PoddonPhoto = ImageSource.FromStream(() => stream);

            //        //var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png";
            //        //if (File.Exists(path))
            //        //{
            //        //    File.Delete(path);
            //        //}
            //        //File.Copy(AttachedPhotoPoddon.FullPath, path);
            //    }
            //}
        }

        

        private async void OnItemSelected(AttachedFiles obj)
        {
            IsLoading = true;
            LoadingState = false;
            LoadingItems = 0;
            LoadingRow = new GridLength(1, GridUnitType.Star);
            if (obj != null)
            {
                var res = File.Exists(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{obj.Name}");
                if (res)
                {
                    await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{obj.Name}") });
                    IsLoading = false;
                    LoadingState = true;
                    LoadingRow = 0;
                    LoadingItems = new GridLength(1, GridUnitType.Star);
                }
                else
                {
                   
                    await GetFile(obj);
                }
                // await alertService.ShowMessage("Прикрепленный файл",obj.Name);
            }
        }

        private async Task GetFile(AttachedFiles obj)
        {
            try
            {
                HttpClient _tokenclient = new HttpClient();
                _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                var _responseToken = await _tokenclient.GetAsync($"{GlobalSettings.HostUrl}api/order/{Request.Id}/{obj.Name}/get");
                using (var fs = new FileStream(
                DependencyService.Resolve<IFileService>().GetRootPath() + $"/{obj.Name}",
                FileMode.CreateNew))
                {
                    await _responseToken.Content.CopyToAsync(fs);
                    alertService.ShowToast($"{obj.Name} загружен...",1f);
                    IsLoading = false;
                    LoadingState = true;
                    LoadingRow = 0;
                    LoadingItems = new GridLength(1, GridUnitType.Star);
                }

            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Фото", "Фотография заказа повреждена...");
                IsLoading = false;
                LoadingState = true;
                LoadingRow = 0;
                LoadingItems = new GridLength(1, GridUnitType.Star);

            }
        }

        public Color LikeColor
        {
            get => _likeColor;
            set => SetProperty(ref _likeColor, value);
        }

        public Color DislikeColor
        {
            get => _dislikeColor;
            set => SetProperty(ref _dislikeColor, value);
        }


        public FileResult UploadedFile
        {
            get => _uploadedFile;
            set => SetProperty(ref _uploadedFile, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set=> SetProperty(ref _isLoading, value);
        }
        public bool LoadingState
        {
            get => _stateLoading;
            set => SetProperty(ref _stateLoading, value);
        }
        public GridLength LoadingRow
        {
            get => _loadingRow;
            set => SetProperty(ref _loadingRow, value);
        }
        public GridLength LoadingItems
        {
            get => _loadingItems;
            set => SetProperty(ref _loadingItems, value);
        }
        private void OnCancel(object obj)
        {
            BackToOrder();
        }

        private async void BackToOrder()
        {
            await Shell.Current.GoToAsync($"..");
        }

        //private async void OnItemSelectedPoddon(ImageSource obj)
        //{
        //    var res = File.Exists(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png");
        //    if (res == false)
        //    {
        //        alertService.ShowToast("Фотография отсутствует ...", 1);
        //    }
        //    else
        //    {
        //        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png") });
        //    }
        //}

        //private async void OnItemSelectedPlenka(ImageSource obj)
        //{

        //    var res = File.Exists(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png");
        //    if (res == false)
        //    {
        //        alertService.ShowToast("Фотография отсутствует ...", 1);
        //    }
        //    else
        //    {
        //        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png") });
        //    }
            
            
        //}
        //private async void OnItemSelectedCarton(ImageSource obj)
        //{
        //    var res = File.Exists(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png");
        //    if (res == false)
        //    {
        //        alertService.ShowToast("Фотография отсутствует ...", 1);
        //    }
        //    else
        //    {
        //        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png") });
        //    }
        //}

        private void OnEditClicked(object obj)
        {
            try
            {
                UpdateConfirm();
               // await UpdateIndicationsAsync(Request);
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message, 1);
            }


        }
        //public ImageSource PoddonPhoto
        //{
        //    get => _poddonOrderPhoto;
        //    set => SetProperty(ref _poddonOrderPhoto, value);
        //}
        //public ImageSource PlenkaPhoto
        //{
        //    get => _plenkaOrderPhoto;
        //    set => SetProperty(ref _plenkaOrderPhoto, value);
        //}
        //public ImageSource CartonPhoto
        //{
        //    get => _cartonOrderPhoto;
        //    set => SetProperty(ref _cartonOrderPhoto, value);
        //}


        //public FileResult AttachedPhotoPoddon
        //{
        //    get => _attachedPhotoPoddon;
        //    set => SetProperty(ref _attachedPhotoPoddon, value);
        //}
        //public FileResult AttachedPhotoPlenka
        //{
        //    get => _attachedPhotoPlenka;
        //    set => SetProperty(ref _attachedPhotoPlenka, value);
        //}
        //public FileResult AttachedPhotoCarton
        //{
        //    get => _attachedPhotoCarton;
        //    set => SetProperty(ref _attachedPhotoCarton, value);
        //}

        //public bool PoddonPhotoLoaded
        //{
        //    get => _poddonPhotoLoaded;
        //    set => SetProperty(ref _poddonPhotoLoaded, value);
        //}

        //public bool PlenkaPhotoLoaded
        //{
        //    get => _plenkaPhotoLoaded;
        //    set => SetProperty(ref _plenkaPhotoLoaded, value);
        //}

        //public bool CartonPhotoLoaded
        //{
        //    get => _cartonPhotoLoaded;
        //    set => SetProperty(ref _cartonPhotoLoaded, value);
        //}

        public string Address
        {
            get => _address;
            set => SetProperty(ref _address, value);    
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
                
                switch (Request.RuState)
                {
                    case "Создан":
                        UpdateStateConfirm("вывезен");
                        break;
                    case "Вывезен":
                        UpdateStateConfirm("взвешен");
                        break;
                    case "Взвешен":
                        UpdateStateConfirm("завершен");
                        break;
                    case "Завершен":
                        await alertService.ShowMessage("Статус", "Заказ уже закрыт!!!");
                        break;
                }
              

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
                RelatedFiles.Clear();
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
                Request _jsonResults = await CheckDataForRequest();
                //
                await CheckPrices();
                //
                Requests.Add(_jsonResults);
                Request = Requests[0];
                CheckLikes();

                GetMaterialsInfo();
                CheckUserRights();

                await CheckListOfAttachedFiles();

                CanEdit = AllowEdit(Request, User);
                //


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

        private async Task<Request> CheckDataForRequest()
        {
            HttpClient _tokenclient = new HttpClient();
            _tokenclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
            var _responseToken = await _tokenclient.GetStringAsync($"{GlobalSettings.HostUrl}api/order/get/{Id}");
            var _jsonResults = JsonConvert.DeserializeObject<Request>(_responseToken);
            _jsonResults.RuState = RuState(_jsonResults.State);
            return _jsonResults;
        }

        private async Task CheckPrices()
        {
            HttpClient _tokenClientPrice = new HttpClient();
            _tokenClientPrice.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
            var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/price/{User.Organization}/{Address}");
            var _jsonResultsPrice = JsonConvert.DeserializeObject<Prices>(_responseTokenPrice);
            Prices = _jsonResultsPrice;
        }

        private void CheckLikes()
        {
            
            switch (Request.Mark)
            {
                case "like":
                    LikeColor = Color.AntiqueWhite;
                    DislikeColor = Color.Transparent;
                    break;
                case "dislike":
                    LikeColor = Color.Transparent;
                    DislikeColor = Color.AntiqueWhite;
                    break;
                default:
                    LikeColor = Color.Transparent;
                    DislikeColor = Color.Transparent;
                    break;
            }
        }

        private async void CheckPhotos()
        {
            
            await CheckListOfAttachedFiles();
          //  await CheckPhotoCarton();
          // await CheckPhotoPlenka();
          // await CheckPhotoPoddon();
        }

        private async Task<List<AttachedFiles>> CheckListOfAttachedFiles()
        {
            
            try
            {
                IsLoading = true;
                LoadingState = false;
                LoadingItems = 0;
                LoadingRow = new GridLength(1, GridUnitType.Star);

                RelatedFiles.Clear();
                HttpClient _tokenClientPrice = new HttpClient();
                _tokenClientPrice.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                var _responseTokenPrice = await _tokenClientPrice.GetStringAsync($"{GlobalSettings.HostUrl}api/order/{Request.Id}/getFiles");
                var res = JsonConvert.DeserializeObject<List<AttachedFiles>>(_responseTokenPrice);
                foreach (var attachedFile in res)
                {
                    RelatedFiles.Add(attachedFile);
                }
                LoadingRow = 0;
                LoadingItems = new GridLength(1, GridUnitType.Star);
                IsLoading = false;
                LoadingState = true;
                return null;
                
            }
            catch (Exception ex)
            {
                alertService.ShowToast(ex.Message,1);
                LoadingRow = 0;
                LoadingItems = new GridLength(1, GridUnitType.Star);
                return null;
            }
        }

        //

        //private async Task CheckPhotoCarton()
        //{
        //    if (Request.AttachedPhotoCarton != null)
        //    {

        //        try
        //        {
        //            CartonPhotoLoaded = true;

        //            WebClient _cartonPhotoClient = new WebClient();
        //            _cartonPhotoClient.Headers.Add(HttpRequestHeader.Authorization, User.Token);
        //            //_cartonPhotoClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Users[0].Token);
        //            _cartonPhotoClient.DownloadFileAsync(new Uri($"{GlobalSettings.HostUrl}api/order/{Request.Id}/{Request.Address}/Carton"),
        //            DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png");
        //            _cartonPhotoClient.DownloadFileCompleted += _cartonPhotoClient_DownloadFileCompleted;


        //        }
        //        catch (Exception ex)
        //        {
        //            await alertService.ShowMessage("Фото", "Фотография заказа повреждена...");
        //            CartonPhotoLoaded = false;

        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        private async void _cartonPhotoClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            await alertService.ShowMessage("Файл", "Загружено!");
            //CartonPhotoLoaded = false;
            ////CartonPhoto = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png";
            ////CartonPhoto = new UriImageSource()
            ////{
            ////    Uri = new Uri(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png"),
            ////    CachingEnabled = false
            ////};
            //await CheckPhotoPlenka();
        }

        //
        //private async Task CheckPhotoPlenka()
        //{
        //    if (Request.AttachedPhotoPlenka != null)
        //    {

        //        try
        //        {
        //            PlenkaPhotoLoaded = true;

        //            WebClient _plenkaPhotoClient = new WebClient();
        //            _plenkaPhotoClient.Headers.Add(HttpRequestHeader.Authorization, User.Token);
        //            _plenkaPhotoClient.DownloadFileAsync(new Uri($"{GlobalSettings.HostUrl}api/order/{Request.Id}/{Request.Address}/Plenka"),
        //            DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png");
        //            _plenkaPhotoClient.DownloadFileCompleted += _plenkaPhotoClient_DownloadFileCompleted;


        //        }
        //        catch (Exception ex)
        //        {
        //            await alertService.ShowMessage("Фото", "Фотография заказа повреждена...");
        //            PlenkaPhotoLoaded = false;

        //        }
        //    }
        //    else
        //    {

        //    }
        //}

        //private async void _plenkaPhotoClient_DownloadFileCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    PlenkaPhotoLoaded = false;
        //   // PlenkaPhoto = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png";
        //    //PlenkaPhoto = new UriImageSource()
        //    //{
        //    //    Uri = new  Uri(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png"),
        //    //    CachingEnabled = false,

        //    //};

        //    await CheckPhotoPoddon();
        //}


        //
        //private async Task CheckPhotoPoddon()
        //{
        //    if (Request.AttachedPhotoPoddon != null)
        //    {

        //        try
        //        {
        //            PoddonPhotoLoaded = true;
                    
        //            WebClient _tokenPhoto = new WebClient();
        //            _tokenPhoto.Headers.Add(HttpRequestHeader.Authorization, User.Token);
        //            _tokenPhoto.DownloadFileAsync(new Uri($"{GlobalSettings.HostUrl}api/order/{Request.Id}/{Request.Address}/Poddon"),
        //            DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png");
        //            _tokenPhoto.DownloadFileCompleted += _tokenPhoto_DownloadFileCompletedAsync;
                    
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            await alertService.ShowMessage("Фото", "Фотография заказа повреждена...");
        //            PoddonPhotoLoaded = false;
                    
        //        }
        //    }
        //    else
        //    {
                
        //    }
        //}

        //private void _tokenPhoto_DownloadFileCompletedAsync(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        //{
        //    //await alertService.ShowMessage("Фото", "Загружено");
        //    PoddonPhotoLoaded =false;
        //    //PoddonPhoto = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png";
        //    //PoddonPhoto = new UriImageSource()
        //    //{
        //    //    Uri = new Uri(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png"),
        //    //    CachingEnabled = true,
        //    //    CacheValidity = TimeSpan.FromDays(1),
        //    //};
        //}

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


        private async void UpdateConfirm()
        {
            var action = await alertService.ConfirmDialog("Статус", "Вы хотите изменить данные заказа?", "Да", "Нет");
            if (!action)
            {
                await alertService.ShowMessage("Данные заказа", "Ну бывает...");
            }
            else
            {
                if (Request.State == "actual" && User.Role == "admin")
                {
                    await alertService.ShowMessage("Данные заказа", "Директор не может изменять заказ после вывоза!!!");
                }
                else
                {
                   Request = await UpdateIndicationsAsync(Request);
                }
            }
        }
        private async void UpdateStateConfirm(string message)
        {
            var action = await alertService.ConfirmDialog("Статус", $"Вы хотите изменить статус заказа на {message}?", "Да", "Нет");
            if (!action)
            {
                await alertService.ShowMessage("Статус заказа", "Ну бывает...");
            }
            else
            {
                if (Request.State == "actual" && User.Role == "admin")
                {
                    await alertService.ShowMessage("Статус заказа", "Директор не может взвешивать заказ!!!");
                }
                else
                {
                   Request = await UpdateStateAsync(Request);
                }
            }
        }

        //public async void TakePhotoPlenka()
        //{
        //    if (IsPlenka)
        //    {

        //        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
        //        {
        //            Title = "Выберите фотографию Пленки"
        //        });
        //        if (result != null)
        //        {
        //            var stream = await result.OpenReadAsync();
        //            if (stream != null)
        //            {
        //                try
        //                {
        //                    //PlenkaPhoto = ImageSource.FromStream(() => stream);
        //                    //AttachedPhotoPlenka = result;
        //                    var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png";
        //                    if (File.Exists(path))
        //                    {
        //                        File.Delete(path);
        //                    }
        //                    File.Copy(AttachedPhotoPlenka.FullPath, path);
        //                }
        //                catch (Exception ex)
        //                {
        //                    alertService.ShowToast(ex.Message, 1);
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        alertService.ShowToast("Такой позиции нет в заказе...", 1);
        //    }
        //}

        //public async void CreatePhotoPlenka()
        //{
        //    if (IsPlenka)
        //    {
        //        try
        //        {
        //            var result = await MediaPicker.CapturePhotoAsync();
        //            if (result != null)
        //            {
        //                var stream = await result.OpenReadAsync();
        //                if (stream != null)
        //                {
        //                    try
        //                    {
        //                        PlenkaPhoto = ImageSource.FromStream(() => stream);
        //                        AttachedPhotoPlenka = result;
        //                        var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png";
        //                        if (File.Exists(path))
        //                        {
        //                            File.Delete(path);
        //                        }
        //                        File.Copy(AttachedPhotoPlenka.FullPath, path);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        alertService.ShowToast(ex.Message, 1);
        //                    }
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await alertService.ShowMessage("Фото", ex.Message);
        //        }
        //    }
        //    else
        //    {
        //        alertService.ShowToast("Такой позиции нет в заказе...", 1);
        //    }
        //}




        //public async void TakePhotoCarton()
        //{
        //    if (IsCarton)
        //    {
        //        var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
        //        {
        //            Title = "Выберите фотографию Картона"
        //        });
        //        if (result != null)
        //        {
        //            var stream = await result.OpenReadAsync();
        //            if (stream != null)
        //            {
        //                CartonPhoto = ImageSource.FromStream(() => stream);
        //                AttachedPhotoCarton = result;
        //                var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png";
        //                if (File.Exists(path))
        //                {
        //                    File.Delete(path);
        //                }
        //                File.Copy(AttachedPhotoCarton.FullPath, path);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        alertService.ShowToast("Такой позиции нет в заказе...", 1);
        //    }
        //}

       // public async void CreatePhotoCarton()
       // {
       //     if (IsCarton)
       //     {
       //         try
       //         {
       //             var result = await MediaPicker.CapturePhotoAsync();
       //             if (result != null)
       //             {
       //                 var stream = await result.OpenReadAsync();
       //                 if (stream != null)
       //                 {
       //                     CartonPhoto = ImageSource.FromStream(() => stream);
       //                     AttachedPhotoCarton = result;
       //                     var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png";
       //                     if (File.Exists(path))
       //                     {
       //                         File.Delete(path);
       //                     }
       //                     File.Copy(AttachedPhotoCarton.FullPath, path);
       //                 }
       //             }
       //         }
       //         catch (Exception ex)
       //         {
       //             await alertService.ShowMessage("Фото", ex.Message);
       //         }
       //     }
       //     else
       //     {
       //         alertService.ShowToast("Такой позиции нет в заказе...", 1);
       //     }
       // }



       // public async void TakePhotoPoddon()
       // {
       //     if (IsPoddon)
       //     {
       //         var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions()
       //         {
       //             Title = "Выберите фотографию Поддонов"
       //         });
       //         if (result != null)
       //         {
       //             var stream = await result.OpenReadAsync();
       //             if (stream != null)
       //             {
       //                 PoddonPhoto = ImageSource.FromStream(() => stream);
       //                 AttachedPhotoPoddon = result;
       //                 var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png";
       //                 if (File.Exists(path))
       //                 {
       //                     File.Delete(path);
       //                 }
       //                 File.Copy(AttachedPhotoPoddon.FullPath, path);
       //             }
       //         }
       //     }
       //     else
       //     {
       //         alertService.ShowToast("Такой позиции нет в заказе...", 1);
       //     }
       // }

       //public async void CreatePhotoPoddon()
       // {
       //     if (IsPoddon)
       //     {
       //         try
       //         {
       //             var result = await MediaPicker.CapturePhotoAsync();
       //             if (result != null)
       //             {
       //                 var stream = await result.OpenReadAsync();
       //                 if (stream != null)
       //                 {
       //                     PoddonPhoto = ImageSource.FromStream(() => stream);
       //                     AttachedPhotoPoddon = result;
       //                     var path = DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png";
       //                     if (File.Exists(path))
       //                     {
       //                         File.Delete(path);
       //                     }
       //                     File.Copy(AttachedPhotoPoddon.FullPath, path);
       //                 }
       //             }
       //         }
       //         catch (Exception ex)
       //         {
       //             await alertService.ShowMessage("Фото", ex.Message);
       //         }
       //     }
       //     else
       //     {
       //         alertService.ShowToast("Такой позиции нет в заказе...", 1);
       //     }
       // }

        internal void OnAppear()
        {
            IsBusy = true;
        }

        public async Task<Request> UpdateIndicationsAsync(Request indications)
        {
            try
            {

                if (indications.State.Length == "created".Length && (User.Role.Length == "admin".Length || User.Role.Length == "root".Length))
                {
                    //await UploadPhotos();
                    return await ChangeData(indications);
                }
                if (indications.State.Length == "actual".Length && (User.Role.Length == "employeе".Length || User.Role.Length == "root".Length))
                {
                    //await UploadPhotos();
                    return await ChangeData(indications);
                }
                if ((indications.State.Length == "weighted".Length || indications.State.Length == "finished".Length) && User.Role.Length == "root".Length)
                {
                    //await UploadPhotos();
                    return await ChangeData(indications);
                } 

                else
                {
                    alertService.ShowToast("У вас недостаточно прав!!!", 1);
                    return null;
                }

            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Message", ex.Message);
                return indications;
            }
        }

        //private async Task UploadPhotos()
        //{
        //    FilesValidator();
        //    //if (IsPoddon && File.Exists(AttachedPhotoPoddon.FullPath))
        //    //{
        //    //    await UploadPhotoToServer(AttachedPhotoPoddon, "Poddon");
        //    //}
        //    //if (IsCarton && File.Exists(AttachedPhotoCarton.FullPath))
        //    //{
        //    //    await UploadPhotoToServer(AttachedPhotoCarton, "Carton");
        //    //}
        //    //if (IsPlenka && File.Exists(AttachedPhotoPlenka.FullPath))
        //    //{
        //    //    await UploadPhotoToServer(AttachedPhotoPlenka, "Plenka");
        //    //}
        //}

        private void FilesValidator()
        {
            try
            {
                //if (AttachedPhotoPoddon == null)
                //{
                //    AttachedPhotoPoddon = new FileResult(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Poddon.png");
                //}
                //if (AttachedPhotoCarton == null)
                //{
                //    AttachedPhotoCarton = new FileResult(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Carton.png");
                //}
                //if (AttachedPhotoPlenka == null)
                //{
                //    AttachedPhotoPlenka = new FileResult(DependencyService.Resolve<IFileService>().GetRootPath() + $"/{Request.Id}_{Request.Address}_Plenka.png");
                //}
            }
            catch (Exception ex)
            {
                
            }
        }

        public async Task UploadPhotoToServer(FileResult file, string material)
        {
            //Path.Combine(FileSystem.CacheDirectory, photo.FileName);
            
            if (file != null && (File.Exists(file.FullPath) || File.Exists(Path.Combine(FileSystem.CacheDirectory, file.FileName))))
            {
                IsLoading = true;
                LoadingState = false;
                LoadingItems = 0;
                LoadingRow = new GridLength(1, GridUnitType.Star);
                var content = new MultipartFormDataContent();
                content.Add(new StreamContent(await file.OpenReadAsync()), "photos",file.FileName);
                var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                var res = await httpClient.PostAsync($"{GlobalSettings.HostUrl}api/order/UploadPhoto/{Request.Id}/{material}", content);
                if (res.IsSuccessStatusCode)
                {
                    await CheckListOfAttachedFiles();
                }
                else
                {
                    await alertService.ShowMessage("Загрузка", "Ошибка при загрузке...");
                    IsLoading = false;
                    LoadingState = true;
                    LoadingRow = 0;
                    LoadingItems = new GridLength(1, GridUnitType.Star);
                }
            }
            else
            {
                await alertService.ShowMessage("Загрузка", "Ошибка при загрузке...");
                IsLoading = false;
                LoadingState = true;
                LoadingRow = 0;
                LoadingItems = new GridLength(1, GridUnitType.Star);
            }
        }

      

        private async Task<Request> ChangeData(Request indications)
        {
            // await Task.Delay(1000);
            alertService.ShowToast("Изменение данных...", 1);
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
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                    var response = await client.PutAsync($"{GlobalSettings.HostUrl}api/order/{User.UserToken}",
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
                        indications = Request;
                        GetMaterialsInfo();
                        //CheckPhotos();
                    //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                      return indications;
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
            if (!String.IsNullOrEmpty(request.WeightData) && !String.IsNullOrEmpty(request.TakenData))
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
            try
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
                    
                   await ChangeState(indications);
                   return Request;
                    //Task result = ChangeData(indications).ContinueWith(async x => indications = await ChangeState(indications));
                    
                    //var task = ChangeData(indications);
                    //alertService.ShowToast("Изменение данных...", 1);
                    //task.Wait();
                    //alertService.ShowToast("Изменение статуса...", 1);
                    //return await ChangeState(indications);
                }
                if ((indications.State.Length == "actual".Length) && (User.Role.Length == "admin".Length || User.Role.Length == "employee".Length || User.Role.Length == "root".Length))
                {
                   await ChangeState(indications);
                    return Request;
                }
                if ((indications.State.Length == "weighted".Length) && (User.Role.Length == "employee".Length || User.Role.Length == "root".Length))
                {
                   await ChangeState(indications);
                    return Request;
                }
                else
                {
                    alertService.ShowToast("Недостаточно прав!!!", 1);
                    return indications;

                }
            }
            catch (Exception ex)
            {
                await alertService.ShowMessage("Error",ex.StackTrace);
                await alertService.ShowMessage("Error",ex.Data.ToString());
                await alertService.ShowMessage("Error",ex.Message);
                return indications;

            }
           
        }

        private async Task<Request> ChangeState(Request indications)
        {

            //await Task.Delay(1000);
            alertService.ShowToast("Изменение статуса...", 1);
            if (CheckAmount(indications.State, IsPlenka, Plenka, PlenkaAmount) && CheckAmount(indications.State, IsPoddon, Poddon, PoddonAmount)
                && CheckAmount(indications.State, IsCarton, Carton, CartonAmount))
            {
                alertService.ShowToast("Идет обновление... Пожалуйста, подождите...", 1);
                IsBusy = true;
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", User.Token);
                var response = await client.PostAsync($"{GlobalSettings.HostUrl}api/order/update_state",
                new StringContent(System.Text.Json.JsonSerializer.Serialize(indications),
                Encoding.UTF8, "application/json"));
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    alertService.ShowToast("Ошибка при обновлении... Попробуйте позже...", 1);
                    return null;
                }
                else
                {
                    var res = JsonConvert.DeserializeObject<Result>(response.Content.ReadAsStringAsync().Result.ToString());
                    
                    var jsonRes = new Request()
                    {
                        Id = res.Id,
                         Address = res.Address,
                          Data = res.Data,
                           Description = res.Description,
                            Materials = res.Materials,
                             Name = res.Name,
                              Organization = res.Organization,
                               RelatedOperator = res.RelatedOperator,
                                RelatedOrganizationId = res.RelatedOrganizationId,
                                 RuState = RuState(res.State),
                                  State = res.State,
                                   TakenData = res.TakenData,
                                    FinishedData = res.FinishedData,
                                     LastModified = res.LastModified,
                                      Price = res.Price,
                                       WeightData = res.WeightData,
                                        WhoClosed = res.WhoClosed,
                                         WhoTook = res.WhoTook,
                                          WhoWeighed = res.WhoWeighed,
                                           OldCartonPrice = res.OldCartonPrice,
                                            OldPlenkaPrice = res.OldPlenkaPrice,
                                             OldPoddonPrice = res.OldPoddonPrice,
                                              Mark = res.Mark 
                    };

                   
                     
                     Request = jsonRes;
                   
                    CheckUserRights();
                    //await Shell.Current.GoToAsync($"//{nameof(LightIndicationsPage)}");
                    return Request;
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
