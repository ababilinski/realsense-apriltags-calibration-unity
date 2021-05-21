using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Babilinapps.RealSenseAprilTags.Examples.Views
{
    public class CalibrationWindow : MonoBehaviour
    {
        public LayoutGroup ContentParent;

        [SerializeField] private CalibrateDeviceButton _calibrateDeviceButtonPrefab;
  
        [Header("Events")]
        public CalibrationButtonsCreatedEvent CalibrationButtonsCreated;
        public CalibrationButtonPressedEvent CalibrationButtonPressed;
        public void Init(List<string> devices)
        {
             var calibrateDeviceButtonBySerial = new Dictionary<string, CalibrateDeviceButton>();
            while (ContentParent.transform.childCount > 0)
            {
                DestroyImmediate(ContentParent.transform.GetChild(0).gameObject);
            }

            var index = 0;
            foreach (string sn in devices)
            {
                
                var label = Instantiate(_calibrateDeviceButtonPrefab, ContentParent.transform);
                label.Init(sn, index);
                calibrateDeviceButtonBySerial.Add(sn, label);
                label.CalibratePressed.AddListener(StartPredictPose);
                index++;
           
            }

            CalibrationButtonsCreated.Invoke(calibrateDeviceButtonBySerial);
        }

        void StartPredictPose(string serialNumber)
        {
            CalibrationButtonPressed.Invoke(serialNumber);
        }
        [System.Serializable]
        public class CalibrationButtonsCreatedEvent:UnityEvent<Dictionary<string, CalibrateDeviceButton>>{}

        [System.Serializable]
        public class CalibrationButtonPressedEvent : UnityEvent<string>
        {
        }
    }
}
