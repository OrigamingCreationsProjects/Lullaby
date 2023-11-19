using Systems.SoundSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Lullaby
{
    public class GameSettings : MonoBehaviour
    {
        [SerializeField] private Slider generalVolumeSlider;
        [SerializeField] private Slider musicVolumeSlider;
        
        public TMP_Dropdown quialityDropdown;
        
        [SerializeField] private float defaultGeneralVolume = 0.5f;
        [SerializeField] private float defaultMusicVolume = 0.5f;
        [SerializeField] private QualityLevels defaultQualityLevel = QualityLevels.Ultra;
        
        private string _generalVolumeKey = "GeneralVolume";
        private string _musicVolumeKey = "MusicVolume";
        private string _qualityLevelKey = "QualityLevel";

        private enum QualityLevels
        {
            VeryLow = 0,
            Low = 1,
            Medium = 2,
            High = 3,
            VeryHigh = 4,
            Ultra = 5,
        }
        
        private void Start()
        {
            //SoundManager
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (PlayerPrefs.HasKey(_generalVolumeKey))
            {
                generalVolumeSlider.value = PlayerPrefs.GetFloat(_generalVolumeKey);
                Debug.Log("Valor de GeneralVolume al cargar settigns: " + PlayerPrefs.GetFloat(_generalVolumeKey));
            }
            else
            {
                generalVolumeSlider.value = defaultGeneralVolume;
                PlayerPrefs.SetFloat(_generalVolumeKey, defaultGeneralVolume);
            }

            if (PlayerPrefs.HasKey(_musicVolumeKey))
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat(_musicVolumeKey);
            }
            else
            {
                musicVolumeSlider.value = defaultMusicVolume;
                PlayerPrefs.SetFloat(_musicVolumeKey, defaultMusicVolume);
            }
            
            if (PlayerPrefs.HasKey(_qualityLevelKey))
            {
                quialityDropdown.value = PlayerPrefs.GetInt(_qualityLevelKey);
                Debug.Log("Quality cogido" + PlayerPrefs.GetInt(_qualityLevelKey));
                ChangeQualitySettings();
            }
            else
            {
                quialityDropdown.value = (int)defaultQualityLevel;
                PlayerPrefs.SetInt(_qualityLevelKey, (int)defaultQualityLevel);
                Debug.Log("Quality seteado" + (int)defaultQualityLevel);
                ChangeQualitySettings();
            }
        }

        public void ResetDefaultValues()
        {
            PlayerPrefs.DeleteAll();
            LoadSettings();
        }
        
        public void SetGeneralVolumePref()
        {
            PlayerPrefs.SetFloat(_generalVolumeKey, generalVolumeSlider.value);
            SoundManager.instance.ChangeGeneralVolume(generalVolumeSlider.value);
            PlayerPrefs.Save();
            //Sonido
        }
        
        public void SetMusicVolumePref()
        {
            PlayerPrefs.SetFloat(_musicVolumeKey, musicVolumeSlider.value);
            SoundManager.instance.ChangeMusicVolume(musicVolumeSlider.value);
            PlayerPrefs.Save();
            //Sonido
        }
        

        public void ChangeQualitySettings()
        {
            QualitySettings.SetQualityLevel(quialityDropdown.value);
            PlayerPrefs.SetInt(_qualityLevelKey, quialityDropdown.value);
            Debug.Log("Nuevo Quality Level " + quialityDropdown.value);
            PlayerPrefs.Save();
        }
        
        public void ChangeLanguage(int languageIndex)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageIndex];
        }
    }
}