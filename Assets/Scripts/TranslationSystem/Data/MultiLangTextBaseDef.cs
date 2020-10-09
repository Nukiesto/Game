/*SimpleLocalizator plugin
 * Developed by NightLord189 (nldevelop.com)*/

using UnityEngine;

namespace SimpleLocalizator {
	public abstract class MultiLangTextBaseDef : MultiLangComponent{
		#region Unity scene settings
		[SerializeField] TranslateString _translateString;
		[SerializeField] bool toUpper=false;
		#endregion

		#region Interface
		public TranslateString TranslateString {
			get {
				return _translateString;
			}
			set {
				_translateString = value;
			    Refresh();
			}
		}

		public void SetLabelId(TranslateString value)
		{
			TranslateString = value;
		}
        #endregion

        #region Methods
        protected override void Refresh()
        {
			string str = "";
            for (int i = 0; i < TranslateString.translations.Count; i++)
            {
				if (TranslateString.translations[i].lang == currentLanguage) {
					str = TranslateString.translations[i].text;
				}
			}
		    if (toUpper)
                str = str.ToUpper();
		    VisualizeString(str);
		}

	    protected abstract void VisualizeString(string str);

	    #endregion
	}
}