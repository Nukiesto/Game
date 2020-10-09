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
        public List<TranslateText> translations = new List<TranslateText>();
    }
}


