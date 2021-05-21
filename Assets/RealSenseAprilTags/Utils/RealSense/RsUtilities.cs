using System.Collections;
using System.Collections.Generic;
using Intel.RealSense;
using UnityEngine;

public static class RsUtilities 
{
    public static string GetPrettyDeviceName(Device device)
    {
        return GetDeviceName(device) + " " + GetDeviceSerialNumber(device);
    }

    public static string GetDeviceSerialNumber(Device device)
    {
        string sn = "########";
        if (device.Info.Supports(CameraInfo.SerialNumber))
            sn = device.Info.GetInfo(CameraInfo.SerialNumber);

        return sn;
    }

    public static string GetDeviceName(Device device)
    {
        // Each device provides some information on itself, such as name:
        string name = "Unknown Device";
        if (device.Info.Supports(CameraInfo.Name))
        {
            name = device.Info.GetInfo(CameraInfo.Name);
        }

        return name;
    }
}
