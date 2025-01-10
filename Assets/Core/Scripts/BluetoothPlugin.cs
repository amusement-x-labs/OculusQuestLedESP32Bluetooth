using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BluetoothPlugin : MonoBehaviour
{
    public event Action<string> OnStatusChanged;
    public bool IsConnected => _isConnected;

    private static AndroidJavaObject bluetoothHandler;
    private bool _isConnected = false;

    private void Start()
    {
        try
        {
            using (var pluginClass = new AndroidJavaClass("com.amusementxlabs.bluetooth.BluetoothHandler"))
            {
                bluetoothHandler = pluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin Start: " + e.ToString());
        }
    }

    public string GetPairedDevices()
    {
        if (bluetoothHandler == null)
        {
            Debug.LogError("BluetoothPlugin GetPairedDevices error: bluetoothHandler is null");
            return string.Empty;
        }

        try
        {
            return bluetoothHandler.Call<string>("getPairedDevices");
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin GetPairedDevices: " + e.ToString());
        }
        return  string.Empty;
    }

    public void Connect(string macAddress)
    {
        if (bluetoothHandler == null)
        {
            Debug.LogError("BluetoothPlugin Connect error: bluetoothHandler is null");
            return;
        }

        if (_isConnected)
        {
            Debug.LogWarning("Connection is already set. Disconnect to establish new connection.");
            return;
        }

        try
        {
            _isConnected = bluetoothHandler.Call<bool>("connect", macAddress);

            if(_isConnected)
                OnStatusChanged?.Invoke("Connected");
            else
                OnStatusChanged?.Invoke("Disconnected");
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin Connect: " + e.ToString());
        }
    }

    public void SendData(string data)
    {
        if (bluetoothHandler == null)
        {
            Debug.LogError("BluetoothPlugin SendData error: bluetoothHandler is null");
            return;
        }

        try
        {
            bluetoothHandler.Call("sendData", data);
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin SendData: " + e.ToString());
        }
    }

    public string ReceiveData()
    {
        if (bluetoothHandler == null)
        {
            Debug.LogError("BluetoothPlugin ReceiveData error: bluetoothHandler is null");
            return string.Empty;
        }

        try
        {
            string data = bluetoothHandler.Call<string>("getReceivedData");
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin ReceiveData: " + e.ToString());
        }
        return string.Empty;
    }

    public void Disconnect()
    {
        if (bluetoothHandler == null)
        {
            Debug.LogError("BluetoothPlugin Disconnect error: bluetoothHandler is null");
            return;
        }

        if (!_isConnected)
        {
            Debug.LogWarning("Connection is not set. Connect is required to make disconnect.");
            return;
        }

        try
        {
            bluetoothHandler.Call("disconnect");
            _isConnected = false;
            OnStatusChanged?.Invoke("Disconnected");
        }
        catch (Exception e)
        {
            Debug.LogError("BluetoothPlugin Disconnect: " + e.ToString());
        }
    }
}