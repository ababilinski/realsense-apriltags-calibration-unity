using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

#if REALSENSE
using Intel.RealSense;
#endif
namespace Babilinapps.RealSenseAprilTags.Examples.Views
{
    public class DeviceSelectorWindow : MonoBehaviour
    {
        public GameObject DeviceLabelPrefab;
        public LayoutGroup ContentParent;
        public Button StartButton;

        [Header("Events")]
        public SerialNumbersSelectedEvent SerialNumbersSelected;

        private readonly List<GameObject> _spawnedObject = new List<GameObject>();
        private readonly List<Toggle> _toggles = new List<Toggle>();
        private readonly List<string> _serialNumbers = new List<string>();
#if REALSENSE
        private Context _context;

        private ToggleGroup _toggleGroup;


        // Start is called before the first frame update
        void Awake()
        {
            StartButton.onClick.AddListener(ConfirmSelection);
        }

        void Start()
        {
            _context = new Context();
            _toggleGroup = gameObject.AddComponent<ToggleGroup>();
            GetDevices();
        }

        public void GetDevices()
        {
            DeviceList deviceList = _context.QueryDevices();
            if (deviceList.Count == 0)
            {
                Debug.LogWarning("No device connected, please connect a RealSense device");
            }


            UpdateDeviceLabels(deviceList);
        }

        private void UpdateDeviceLabels(DeviceList deviceList)
        {
            while (ContentParent.transform.childCount > 0)
            {
                DestroyImmediate(ContentParent.transform.GetChild(0).gameObject);
            }


            _spawnedObject.Clear();
            _toggles.Clear();
            _serialNumbers.Clear();

            for (int i = 0; i < deviceList.Count; i++)
            {
                var label = Instantiate(DeviceLabelPrefab, ContentParent.transform);
                RegisterToggle(label, RsUtilities.GetDeviceSerialNumber(deviceList[i]));
            }

            Canvas.ForceUpdateCanvases();
        }

        private void RegisterToggle(GameObject deviceLabel, string serialNumber)
        {
            var toggle = deviceLabel.gameObject.GetComponentInChildren<Toggle>();
            var text = deviceLabel.gameObject.GetComponentInChildren<TMP_Text>();

            text.text = serialNumber;

            _toggleGroup.RegisterToggle(toggle);

            _toggles.Add(toggle);
            _spawnedObject.Add(deviceLabel);
            _serialNumbers.Add(serialNumber);
        }

        private void ConfirmSelection()
        {
            List<string> activeSerialNumbers = new List<string>();
            for (int i = 0; i < _toggles.Count; i++)
            {
                if (_toggles[i].isOn)
                {
                    activeSerialNumbers.Add(_serialNumbers[i]);
                }
            }

            SerialNumbersSelected.Invoke(activeSerialNumbers);
        }

#endif

        [System.Serializable]
        public class SerialNumbersSelectedEvent : UnityEvent<List<string>>
        {
        }

    }
}