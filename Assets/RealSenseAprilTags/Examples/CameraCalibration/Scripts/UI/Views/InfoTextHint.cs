using TMPro;
using UnityEngine;

namespace Babilinapps.RealSenseAprilTags.Examples.Views
{
    public class InfoTextHint : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _infoText;

        private bool _showing;


        public void SetText(string text)
        {
            if (!_showing)
            {
                gameObject.SetActive(true);
            }

            _infoText.text = text;
        }

        public void Hide()
        {
            if (_showing)
            {
                gameObject.SetActive(false);
            }
        }

        private void OnEnable()
        {
            _showing = true;
        }

        private void OnDisable()
        {
            _showing = false;
        }
    }
}