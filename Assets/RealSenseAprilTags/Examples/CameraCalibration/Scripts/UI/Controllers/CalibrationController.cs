using System.Collections;
using System.Collections.Generic;
using Babilinapps.RealSenseAprilTags.Examples.Views;
using Babilinapps.RealSenseAprilTags.Utils.Math;
using UnityEngine;


namespace Babilinapps.RealSenseAprilTags.Examples{
public class CalibrationController : MonoBehaviour
{
    [SerializeField] private RsDeviceSpawner _rsDeviceSpawner;
	[Header("UI Screens")]
    [SerializeField] private DeviceSelectorWindow _deviceSelectorWindow;
	[SerializeField] private CalibrationWindow _calibrationWindow;
	[SerializeField] private InfoTextHint _infoTextHint;

	private readonly Dictionary<string, bool> _calibrationStateBySerial = new Dictionary<string, bool>();
	private Dictionary<string, CalibrateDeviceButton> _calibrateDeviceButtonBySerial;
	private Dictionary<string, RsCameraPoseEstimation> _rsDeviceBySerialNumber;


    // Start is called before the first frame update
    void Start()
    {
		_infoTextHint.Hide();
		_calibrationWindow.gameObject.SetActive(false);
		_deviceSelectorWindow.gameObject.SetActive(true);
        _deviceSelectorWindow.SerialNumbersSelected.AddListener(OnDevicesSelected);
		_rsDeviceSpawner.SpawnedDevices.AddListener(OnSpawnedDevices);
		_calibrationWindow.CalibrationButtonsCreated.AddListener(OnCalibrationButtonsCreated);
		_calibrationWindow.CalibrationButtonPressed.AddListener(StartPredictPose);
    }

    void OnDevicesSelected(List<string> devices)
    {
	
		_calibrationStateBySerial.Clear();
		StartCoroutine(SpawnDevices(devices));

	}
	//We use a coroutine To avoid freezing the app before the UI loads.
	private IEnumerator SpawnDevices(List<string> devices)
	{
		_infoTextHint.SetText("Spawning Devices...");
		_deviceSelectorWindow.gameObject.SetActive(false);
		yield return null;
		_calibrationWindow.Init(devices);
		yield return null;
		_rsDeviceSpawner.SpawnDevices(devices);
		_calibrationWindow.gameObject.SetActive(true);
		yield return null;
		_infoTextHint.Hide();

	}
	void OnCalibrationButtonsCreated(Dictionary<string, CalibrateDeviceButton> calibrateDeviceButtonBySerial)
	{
		_calibrateDeviceButtonBySerial = calibrateDeviceButtonBySerial;
	}

	void OnSpawnedDevices(Dictionary<string, RsCameraPoseEstimation> rsDeviceBySerialNumber)
	{
		_rsDeviceBySerialNumber = rsDeviceBySerialNumber;
	}

	void StartPredictPose(string serialNumber)
	{
#if REALSENSE
		StartCoroutine(PredictPose(serialNumber));
#endif
	}

#if REALSENSE
    IEnumerator PredictPose(string serialNumber)
	{

		

		var device = _rsDeviceBySerialNumber[serialNumber];
		_calibrateDeviceButtonBySerial[serialNumber].UnityButton.interactable = false;

		if (!_calibrationStateBySerial.ContainsKey(serialNumber))
		{
			_calibrationStateBySerial.Add(serialNumber,false);
		}

		
		device.MoveCamera = true;
		
		List<Vector3> positions = new List<Vector3>();
		List<Quaternion> rotations = new List<Quaternion>();
		float startTime = 0;
		float endTime = 5;
		int calibrationTicks = device.CalibrationTicks; //Only add tracked values
		while (device.CalibrationTicks < 100)
		{
			if (calibrationTicks != device.CalibrationTicks)
			{
				positions.Add(device.transform.position);
				rotations.Add(device.transform.rotation);
				calibrationTicks = device.CalibrationTicks;
				_calibrateDeviceButtonBySerial[serialNumber].textMesh.text = $"Calibrating... ({calibrationTicks})/({100})";
			}
			else
			{
				startTime += Time.deltaTime;
				if (startTime > endTime)
				{
					_calibrateDeviceButtonBySerial[serialNumber].textMesh.text = $"Calibrating... (marker not found)";
				}
			}

		
			

			yield return null;
		}

		device.MoveCamera = false;
		if (_calibrationStateBySerial[serialNumber])
		{
			device.transform.rotation = MathUtils.Average(device.transform.rotation, rotations.ToArray());
			device.transform.position = MathUtils.Average(device.transform.position, positions.ToArray());
		}
		else
		{
			device.transform.rotation = MathUtils.Average(rotations.ToArray());
			device.transform.position = MathUtils.Average(positions.ToArray());
			_calibrationStateBySerial[serialNumber] = true;
		}

		_calibrateDeviceButtonBySerial[serialNumber].textMesh.text = serialNumber;
		yield return new WaitForSeconds(0.2f);
		_calibrateDeviceButtonBySerial[serialNumber].UnityButton.interactable = true;
	}
#endif

}
}