using System.Collections.Generic;
using Babilinapps.RealSenseAprilTags.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Babilinapps.RealSenseAprilTags.Examples
{
    public class RsDeviceSpawner : MonoBehaviour
    {
        public RsCameraPoseEstimation DevicePrefab;
        public Transform SpawnRoot;
        public Dictionary<string, RsCameraPoseEstimation> RsDeviceBySerialNumber = new Dictionary<string, RsCameraPoseEstimation>();

        [Header("Events")]
        public SpawnedDeviceEvent SpawnedDevice;

        public SpawnedDevicesEvent SpawnedDevices;

        // Start is called before the first frame update
        public void SpawnDevices(List<string> serialNumbers)
        {
#if REALSENSE
            SpawnRoot.gameObject.SetActive(false);

            for (int i = 0; i < serialNumbers.Count; i++)
            {
                if (RsDeviceBySerialNumber.ContainsKey(serialNumbers[i]))
                    continue;

                RsDevice spawnedDevice = null;
                var newPrefab = Instantiate(DevicePrefab, SpawnRoot, false);
                spawnedDevice = newPrefab.GetComponentInChildren<RsDevice>(true);

                spawnedDevice.DeviceConfiguration.RequestedSerialNumber = serialNumbers[i];
                RsDeviceBySerialNumber.Add(serialNumbers[i], newPrefab);
                newPrefab.MoveCamera = false;
                newPrefab.tagOverlayColor = ColorHelper.GetColorById(i);
                SpawnedDevice.Invoke(new KeyValuePair<string, RsCameraPoseEstimation>(serialNumbers[i], newPrefab));
            }

            SpawnRoot.gameObject.SetActive(true);
            SpawnedDevices.Invoke(RsDeviceBySerialNumber);
#endif

        }


        [System.Serializable]
        public class SpawnedDeviceEvent : UnityEvent<KeyValuePair<string, RsCameraPoseEstimation>>
        {
        }

        [System.Serializable]
        public class SpawnedDevicesEvent : UnityEvent<Dictionary<string, RsCameraPoseEstimation>>
        {
        }
    }

}