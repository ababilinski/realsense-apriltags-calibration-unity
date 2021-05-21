using Babilinapps.RealSenseAprilTags.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

namespace Babilinapps.RealSenseAprilTags.Examples.Views
{
    public class CalibrateDeviceButton: MonoBehaviour
    {
        public string SerialNumber { get; private set; }
        public CalibratePressedEvent CalibratePressed;
        public Button UnityButton;
        public TextMeshProUGUI textMesh;
        public void Init(string sn,int idColor)
        {
      
            SerialNumber = sn;
            textMesh.text = SerialNumber;
            var colors = UnityButton.colors;
            var colorFromSn = ColorHelper.GetColorById(idColor);
            colors.disabledColor = colorFromSn;
            colors.normalColor = colorFromSn;
            UnityButton.colors = colors;
            textMesh.color = ColorHelper.PickTextColorBasedOnBgColor(colorFromSn);
        
        }

        public void Invoke()
        {
            CalibratePressed?.Invoke(SerialNumber);
        }

        [System.Serializable]
        public class CalibratePressedEvent : UnityEvent<string>{}

       
    }
}
