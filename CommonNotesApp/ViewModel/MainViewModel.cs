
using CommonNotesApp.Data;
using CommonNotesApp.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CommonNotesApp.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DatabaseContext _db;
        public MainViewModel(DatabaseContext db)
        {
            _db = db;
            Items = new ObservableCollection<MainModel>();
        }
        [ObservableProperty]
        ObservableCollection<MainModel> items;


        [ObservableProperty]
        string item;
        [ObservableProperty]
        float price;

        [ObservableProperty]
        float total;

        public async Task LoadItemsAsync()
        {
            var Datas = await _db.GetAllAsync<Item>();
            if (Datas is not null && Datas.Any())
            {
                Items ??= new ObservableCollection<MainModel>();

                foreach (var data in Datas)
                {
                    Items.Add(new MainModel()
                    {
                        Item = data.Particular,
                        Price = data.Price,
                        Id = data.Id
                    });
                }
                Total = Items.Sum(a => a.Price);
            }
        }

        [RelayCommand]
        public async Task Delete(int id)
        {

            var Datas = await _db.GetFilteredAsync<Item>(a => a.Id == id);
            var Data = Datas.FirstOrDefault();
            if (Data != null)
            {
                if (await _db.DeleteItemAsync<Item>(Data))
                {
                    var data = Items.First(c => c.Id == id);

                    Items.Remove(data);
                    Total -= data.Price;
                }
                else
                {
                    await Shell.Current.DisplayAlert("Delete Error", "Fail to delete data.", "Ok");
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Delete Error", "Fail to delete data.", "Ok");
            }




        }

        [RelayCommand]
        public async Task Add()
        {
            if (String.IsNullOrEmpty(Item))
            {
                return;
            }
            var newItem = await _db.AddItemAsync<Item>(new Model.Item()
            {
                Particular = Item,
                Price = Price
            });

            Items.Add(new MainModel()
            {
                Item = Item,
                Price = Price,
                Id = newItem.Id
            });
            Total += Price;


            Item = string.Empty;
            Price = 0;
        }

    }
    public class MainModel
    {
        public string Display => $"{Item} (Rs. {Price})";

        public string Item { get; set; }
        public float Price { get; set; }
        public int Id { get; set; }
    }
}
