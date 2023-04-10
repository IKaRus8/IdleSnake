using System;
using Extensions.Core;
using Managers.Interfaces;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class SoundManager : MonoBehaviour, ISoundManager
    {
        [SerializeField] 
        private AudioClip _boost;

        private AudioSource _audioSource;

        public bool IsSoundOn => !SoundOff;
        public bool IsMusicOn => Music;

        private bool SoundOff
        {
            get =>bool.Parse(PlayerPrefs.GetString("SoundOff", true.ToString()));
            set => PlayerPrefs.SetString("SoundOff", value.ToString());
        }

        private bool Music
        {
            get => bool.Parse(PlayerPrefs.GetString("Music", true.ToString()));
            set => PlayerPrefs.SetString("Music", value.ToString());
        }

        private void Awake()
        {
            _boost.CheckForNull();

            _audioSource = GetComponent<AudioSource>();
            _audioSource.CheckForNull();
        }

        private void Start()
        {
            ChangeSoundOff(SoundOff);
            ChangeMusic(Music);
        }

        public void ChangeSoundOff(bool value)
        {
            if (SoundOff != value)
            {
                SoundOff = value;
            }
        }

        public void ChangeMusic(bool isMusicOn)
        {
            if (Music != isMusicOn)
            {
                Music = isMusicOn;
            }

            if (isMusicOn)
            {
                _audioSource.Play();
            }
            else
            {
                _audioSource.Stop();
            }

            //_audioSource.mute = !sound;
        }

        public void PlayBoost()
        {
            if (SoundOff)
            {
               return;
            }
            
            _audioSource.PlayOneShot(_boost);
        }
    }
}
