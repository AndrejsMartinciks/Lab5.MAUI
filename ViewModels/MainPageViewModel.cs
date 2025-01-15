using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using Lab5.MAUIData.Interfaces;
using Lab5.MAUIData.Models;

public class MainPageViewModel : ViewModelBase
{
    private readonly IDataRepository _dataRepository;

    public MainPageViewModel(IDataRepository dataRepository)
    {
        _dataRepository = dataRepository;

        // Инициализация команд
        LoadAuthorsCommand = new RelayCommand(async () => await LoadAuthorsAsync());
        AddAuthorCommand = new RelayCommand(async () => await AddSampleAuthorAsync());
        SelectAuthorCommand = new RelayCommand(async () => await ShowDetailsAsync());

        // Автоматическая загрузка данных при запуске
        _ = LoadAuthorsAsync();
    }

    // Свойство заголовка
    private string _title;
    public string Title
    {
        get => _title;
        set
        {
            _title = value;
            OnPropertyChanged();
        }
    }

    // Список авторов
    private ObservableCollection<Author> _authors;
    public ObservableCollection<Author> Authors
    {
        get => _authors;
        set
        {
            _authors = value;
            OnPropertyChanged();
        }
    }

    // Выбранный автор
    private Author _selectedAuthor;
    public Author SelectedAuthor
    {
        get => _selectedAuthor;
        set
        {
            _selectedAuthor = value;
            OnPropertyChanged();
        }
    }

    // Команды
    public ICommand LoadAuthorsCommand { get; }
    public ICommand AddAuthorCommand { get; }
    public ICommand SelectAuthorCommand { get; }

    // Загрузка авторов из репозитория
    private async Task LoadAuthorsAsync()
    {
        Title = "Loading authors...";
        var authors = await _dataRepository.GetAuthorsAsync();
        Authors = new ObservableCollection<Author>(authors);
        Title = $"Loaded {authors.Length} authors";
    }

    // Добавление тестового автора
    private async Task AddSampleAuthorAsync()
    {
        var author = new Author
        {
            Name = "Fyodor",
            Surname = "Dostoevsky",
            Books = new List<Book>
            {
                new Book { Title = "Crime and Punishment" },
                new Book { Title = "The Brothers Karamazov" }
            }
        };

        await _dataRepository.AddAuthorAsync(author);
        await LoadAuthorsAsync(); // Обновляем список авторов
    }

    // Показ деталей выбранного автора
    private async Task ShowDetailsAsync()
    {
        if (SelectedAuthor == null)
        {
            return; // Если автор не выбран, ничего не делаем
        }

        var navigationParameter = new Dictionary<string, object>
        {
            { "Author", SelectedAuthor }
        };

        await Shell.Current.GoToAsync("//BookPage", navigationParameter);
    }
}