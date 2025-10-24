namespace MyPath;

public partial class TimePickerPage : ContentPage
{
    public TimeSpan SelectedTime { get; private set; }

    public TimePickerPage(TimeSpan currentTime)
    {
        InitializeComponent();
        SelectedTime = currentTime;

        InitializePickers();
        UpdateSelectedTimeDisplay();

        // Подписываемся на изменения
        HoursPicker.SelectedIndexChanged += OnTimeChanged;
        MinutesPicker.SelectedIndexChanged += OnTimeChanged;

        OkButton.Clicked += OnOkClicked;
        CancelButton.Clicked += OnCancelClicked;
    }

    private void InitializePickers()
    {
        // Заполняем часы (0-23)
        for (int i = 0; i < 24; i++)
        {
            HoursPicker.Items.Add(i.ToString("00"));
        }

        // Заполняем минуты (0-59)
        for (int i = 0; i < 60; i++)
        {
            MinutesPicker.Items.Add(i.ToString("00"));
        }

        // Устанавливаем текущее время
        HoursPicker.SelectedIndex = SelectedTime.Hours;
        MinutesPicker.SelectedIndex = SelectedTime.Minutes;
    }

    private void OnTimeChanged(object sender, EventArgs e)
    {
        if (HoursPicker.SelectedIndex != -1 && MinutesPicker.SelectedIndex != -1)
        {
            var hours = int.Parse(HoursPicker.Items[HoursPicker.SelectedIndex]);
            var minutes = int.Parse(MinutesPicker.Items[MinutesPicker.SelectedIndex]);
            SelectedTime = new TimeSpan(hours, minutes, 0);
            UpdateSelectedTimeDisplay();
        }
    }

    private void UpdateSelectedTimeDisplay()
    {
        SelectedTimeLabel.Text = SelectedTime.ToString(@"hh\:mm");
    }

    private async void OnOkClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        SelectedTime = TimeSpan.Zero; // Отмена выбора
        await Navigation.PopModalAsync();
    }
}