using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ModernWpf;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Windows.Media;
using System;
using MangaUpdates_Notifications.Native.Structs;

namespace MangaUpdates_Notifications.Models
{
    public partial class Settings : ObservableObject
    {
        public event EventHandler? SettingsChanged;
        [JsonIgnore] public string Json => JsonConvert.SerializeObject(this, Formatting.Indented);

        [ObservableProperty] private PersonalizationSettings _personalization = new();
        [ObservableProperty] private DiscordWebhookSettings _discordWebhook = new();
        [ObservableProperty] private DiscordBotSettings _discordBot = new();
        [ObservableProperty] private PushBulletSettings _pushBullet = new();

        [JsonProperty(Order = 1)] public WINDOWPLACEMENT? MainView = null;

        public Settings()
        {
            Personalization.PropertyChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            Personalization.Interval.PropertyChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            DiscordWebhook.PropertyChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            DiscordWebhook.Webhooks.ListChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            DiscordBot.PropertyChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            DiscordBot.Users.ListChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            DiscordBot.Channels.ListChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            PushBullet.PropertyChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
            PushBullet.Devices.ListChanged += (_, _) => SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        public partial class PersonalizationSettings : ObservableObject
        {
            [ObservableProperty] private ApplicationTheme _theme = ApplicationTheme.Dark;
            [ObservableProperty] private Color _accent = (Color)ColorConverter.ConvertFromString("#FF8C15");
            [ObservableProperty] private bool _windowsNotifications = true;
            [ObservableProperty] private TimeInterval _interval = new() { Hours = 1, Minutes = 0 };
            [ObservableProperty] private bool _checkUpdates = true;
        }

        public partial class TimeInterval : ObservableObject
        {
            [JsonIgnore] public int Milliseconds => (Hours * 60 + Minutes) * 60 * 1000;
            [ObservableProperty] private int _hours;
            [ObservableProperty] private int _minutes;

            partial void OnHoursChanged(int value)
            {
                if (value < 1)
                    Hours = 1;
            }
        }

        public partial class DiscordWebhookSettings : ObservableObject
        {
            [ObservableProperty] private bool _enabled;
            [ObservableProperty] [property: JsonConverter(typeof(Converters.StringWrapperListConverter<BindingList<StringWrapper>>))]
            private BindingList<StringWrapper> _webhooks = new();

            [RelayCommand] [property: JsonIgnore]
            private void AddWebhook()
            {
                Webhooks.Add(new StringWrapper { String = string.Empty });
            }

            [RelayCommand] [property: JsonIgnore]
            private void RemoveWebhook(StringWrapper webhook)
            {
                Webhooks.Remove(webhook);
            }
        }

        public partial class DiscordBotSettings : ObservableObject
        {
            [ObservableProperty] private bool _enabled;
            [ObservableProperty] private string _token = string.Empty;

            [ObservableProperty] [property: JsonConverter(typeof(Converters.StringWrapperListConverter<BindingList<StringWrapper>>))]
            private BindingList<StringWrapper> _users = new();

            [ObservableProperty] [property: JsonConverter(typeof(Converters.StringWrapperListConverter<BindingList<StringWrapper>>))]
            private BindingList<StringWrapper> _channels = new();

            [RelayCommand] [property: JsonIgnore]
            private void AddUser()
            {
                Users.Add(new StringWrapper { String = string.Empty });
            }

            [RelayCommand] [property: JsonIgnore]
            private void RemoveUser(StringWrapper user)
            {
                Users.Remove(user);
            }

            [RelayCommand] [property: JsonIgnore]
            private void AddChannel()
            {
                Channels.Add(new StringWrapper { String = string.Empty });
            }

            [RelayCommand] [property: JsonIgnore]
            private void RemoveChannel(StringWrapper channel)
            {
                Channels.Remove(channel);
            }
        }

        public partial class PushBulletSettings : ObservableObject
        {
            [ObservableProperty] private bool _enabled;
            [ObservableProperty] private bool _allDevices;
            [ObservableProperty] private string _token = string.Empty;

            [ObservableProperty] [property: JsonConverter(typeof(Converters.StringWrapperListConverter<BindingList<StringWrapper>>))]
            private BindingList<StringWrapper> _devices = new();

            [RelayCommand] [property: JsonIgnore]
            private void AddDevice()
            {
                Devices.Add(new StringWrapper { String = string.Empty });
            }

            [RelayCommand] [property: JsonIgnore]
            private void RemoveDevice(StringWrapper device)
            {
                Devices.Remove(device);
            }
        }
    }
}