using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using PFS.Assets.Scripts.Models.SoundManagerModels;
using PFS.Assets.Scripts.Models;
using PFS.Assets.Scripts.Models.ScreenManagerModels;

namespace PFS.Assets.Scripts.Views.Popups
{
    public class UISettingsPopupView : BaseView
    {
        [Inject]
        public SoundManagerModel SoundManager { get; set; }

        public Button close;
        public Slider volume;
        public TextMeshProUGUI valueCount;
        public Slider checkMusic;

        public GameObject[] offText;
        public GameObject[] onText;

        public void LoadView()
        {
            checkMusic.value = Convert.ToInt32(SoundManager.isMusic);
            SetMusicText(SoundManager.isMusic);
            valueCount.text = string.Format("{0:0}%", SoundManager.musicVolume * 100);
            volume.value = SoundManager.musicVolume;

            checkMusic.onValueChanged.AddListener((value) =>
            {
                ChangeOptionsMusic(value == 1);
            });

            volume.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

            close.onClick.AddListener(() => Close());

        }

        public void RemoveView()
        {

        }


        public void ChangeOptionsMusic(bool value)
        {
            if (value == true)
            {
                Dispatcher.Dispatch(EventGlobal.E_SoundRemuteMusic);
            }
            else
            {
                Dispatcher.Dispatch(EventGlobal.E_SoundMuteMusic);
            }
            SetMusicText(value);
        }

        public void ValueChangeCheck()
        {
            Debug.Log(volume.value);
            SoundManager.musicVolume = volume.value;
            valueCount.text = string.Format("{0:0}%", SoundManager.musicVolume * 100);
            PlayerPrefsModel.MusicVolume = SoundManager.musicVolume;
            Dispatcher.Dispatch(EventGlobal.E_SoundUpdateMusicVolume, SoundManager.musicVolume);
        }

        private void SetMusicText(bool on)
        {
            offText[0].SetActive(on);
            offText[1].SetActive(!on);

            onText[0].SetActive(!on);
            onText[1].SetActive(on);
        }

        private void Close()
        {
            Dispatcher.Dispatch(EventGlobal.E_HideScreen, new ShowScreenModel { screenName = UIScreens.UISettingsPopup, showSwitchAnim = false });
            Dispatcher.Dispatch(EventGlobal.E_SoundClick);
        }
    }
}