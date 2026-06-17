using RvSdk.Module;
using TMPro;
using UnityEngine;

namespace RvSdk.Samples
{
    public class LanguageSwitcher : MonoBehaviour
    {
        public TextMeshProUGUI selectedLanguage;

        private void Update()
        {
            // NULL REFERENCE EXCEPTION
            if (TextService.Instance != null)
                selectedLanguage.text = TextService.Instance.CurrentLanguage;
            else
                Debug.LogError("TextService.Instance is null");
        }

        public void DoSelectLanguage(string language)
        {
            TextService.Instance.SetLanguage(language);

            selectedLanguage.text = language;
        }
    }
}