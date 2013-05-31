using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Text;
using Posmotrim.Phone.Adapters;

namespace Posmotrim.TootFM.PhoneServices.Services.Stores
{
    public class SettingsStore : ISettingsStore
    {
        #region Fields

        private readonly IProtectData _protectDataAdapter;
        private const bool LocationServiceSettingDefault = true;
        private const string LocationServiceSettingKeyName = "LocationService";
        private const bool BackgroundTasksSettingDefault = false;
        private const string BackgroundTasksSettingKeyName = "BackgroundTasks";
        private const string FoursquareTokenSettingDefault = "";
        private const string FoursquareTokenSettingKeyName = "FoursquareTokenSetting";
        private const bool PushNotificationSettingDefault = false;
        private const string PushNotificationSettingKeyName = "PushNotification";
        private const string DeezerSettingDefault = "";
        private const string DeezerSettingKeyName = "DeezerTokenSetting";
        private const string CurrentSessionSettingDefault = "";
        private const string CurrentSessionSettingKeyName = "CurrentSessionSetting";
        private readonly IsolatedStorageSettings _isolatedStore;
        private UTF8Encoding _encoding;

        #endregion



        public SettingsStore(IProtectData protectDataAdapter)
        {
            this._protectDataAdapter = protectDataAdapter;
            _isolatedStore = IsolatedStorageSettings.ApplicationSettings;
            _encoding = new UTF8Encoding();
        }

        #region Properties

        public string DeezerToken
        {
            get
            {
                return DeezerTokenByteArray.Length ==
                    0 ? DeezerSettingDefault : _encoding.GetString(DeezerTokenByteArray, 0, DeezerTokenByteArray.Length);
            }
            set
            {
                DeezerTokenByteArray = _encoding.GetBytes(value);
            }
        }

      

        private byte[] DeezerTokenByteArray
        {
            get
            {
                byte[] encryptedValue = GetValueOrDefault(DeezerSettingKeyName, new byte[0]);
                if (encryptedValue.Length == 0)
                    return new byte[0];
                return _protectDataAdapter.Unprotect(encryptedValue, null);
            }
            set
            {
                byte[] encryptedValue = _protectDataAdapter.Protect(value, null);
                AddOrUpdateValue(DeezerSettingKeyName, encryptedValue);
            }
        }

        public string FoursquareToken
        {
            get
            {
                return FoursquareTokenByteArray.Length ==
                    0 ? FoursquareTokenSettingDefault : _encoding.GetString(FoursquareTokenByteArray, 0, FoursquareTokenByteArray.Length);
            }
            set
            {
                FoursquareTokenByteArray = _encoding.GetBytes(value);
                var handler = this.UserChanged;
                if (handler != null)
                {
                    UserChanged(this, null);
                }
            }
        }

        private byte[] FoursquareTokenByteArray
        {
            get
            {
                byte[] encryptedValue = GetValueOrDefault(FoursquareTokenSettingKeyName, new byte[0]);
                if (encryptedValue.Length == 0)
                    return new byte[0];
                return _protectDataAdapter.Unprotect(encryptedValue, null);
            }
            set
            {
                byte[] encryptedValue = _protectDataAdapter.Protect(value, null);
                AddOrUpdateValue(FoursquareTokenSettingKeyName, encryptedValue);
            }
        }

        public bool SubscribeToPushNotifications
        {
            get { return GetValueOrDefault(PushNotificationSettingKeyName, PushNotificationSettingDefault); }
            set { AddOrUpdateValue(PushNotificationSettingKeyName, value); }
        }

        public bool LocationServiceAllowed
        {
            get { return GetValueOrDefault(LocationServiceSettingKeyName, LocationServiceSettingDefault); }
            set { AddOrUpdateValue(LocationServiceSettingKeyName, value); }
        }

        public bool BackgroundTasksAllowed
        {
            get { return GetValueOrDefault(BackgroundTasksSettingKeyName, BackgroundTasksSettingDefault); }
            set { AddOrUpdateValue(BackgroundTasksSettingKeyName, value); }
        }

        public event EventHandler UserChanged;

        public string CurrentSessionToken
        {
            get { return this.GetValueOrDefault(CurrentSessionSettingKeyName, CurrentSessionSettingDefault); }
            set
            {
                this.AddOrUpdateValue(CurrentSessionSettingKeyName, value);
                var handler = this.UserChanged;
                if (handler != null)
                {
                    UserChanged(this, null);
                }
            }
        }

        #endregion




        #region Methods

        private void AddOrUpdateValue(string key, object value)
        {
            bool valueChanged = false;

            try
            {
                // if new value is different, set the new value.
                if (_isolatedStore[key] != value)
                {
                    _isolatedStore[key] = value;
                    valueChanged = true;
                }
            }
            catch (KeyNotFoundException)
            {
                _isolatedStore.Add(key, value);
                valueChanged = true;
            }
            catch (ArgumentException)
            {
                _isolatedStore.Add(key, value);
                valueChanged = true;
            }

            if (valueChanged)
            {
                Save();
            }
        }

        private T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            try
            {
                value = (T)_isolatedStore[key];
            }
            catch (KeyNotFoundException)
            {
                value = defaultValue;
            }
            catch (ArgumentException)
            {
                value = defaultValue;
            }

            return value;
        }

        private void Save()
        {
            _isolatedStore.Save();
        }

        #endregion
    }


}