using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleLocalizator
{
    [Serializable]
    public struct TranslateText
    {
        public Language lang;
        [TextArea(3, 10)]
        public string text;

        public TranslateText(Language l, string s)
        {
            lang = l;
            text = s;
        }
    }
    [Serializable]
    public class TranslateString
    {
        #region Unity scene settings
        [SerializeField] List<TranslateText> translations = new List<TranslateText>();
        #endregion

        #region Data
        [SerializeField] Language _currentLanguage = Language.Russian;

        Language currentLanguage
        {
            get
            {
                return _currentLanguage;
            }
            set
            {
                _currentLanguage = value;
                RefreshString(GetCurrentText());
            }
        }
        #endregion

        #region Methods
        private void Reset()
        {
            if (translations == null || translations.Count <= 0)
            {
                if (translations == null)
                    translations = new List<TranslateText>();
                else translations.Clear();
                Language[] langs = (Language[])Enum.GetValues(typeof(Language));
                for (int i = 0; i < langs.Length; i++)
                {
                    translations.Add(new TranslateText(langs[i], "not translated"));
                }
            }
        }

        string GetCurrentText()
        {
            for (int i = 0; i < translations.Count; i++)
            {
                if (translations[i].lang == currentLanguage)
                {
                    return translations[i].text;
                }
            }
            return "not translated";
        }

        protected virtual void RefreshString(string str)
        {

        }

        public void OnValidate()
        {
            currentLanguage = _currentLanguage;
        }

        private void OnEnable()
        {
            OnLanguageRefresh();
            LanguageManager.onLanguageChanged += OnLanguageRefresh;
        }

        private void OnDisable()
        {
            LanguageManager.onLanguageChanged -= OnLanguageRefresh;
        }

        private void OnLanguageRefresh()
        {
            currentLanguage = LanguageManager.currentLang;
        }
        #endregion
    }
}


