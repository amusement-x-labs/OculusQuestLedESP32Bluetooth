using System;
using System.Collections.Generic;
using Oculus.Interaction;
using TMPro;
using UnityEngine;

namespace Core.Scripts
{
    public class ControlMenuHandler : MonoBehaviour
    {
        [SerializeField] private PointableUnityEventWrapper connectButton;
        [SerializeField] private PointableUnityEventWrapper disconnectButton;
        [SerializeField] private PointableUnityEventWrapper pairButton;
        [Space(20)]
        [SerializeField] private PointableUnityEventWrapper prevMacButton;
        [SerializeField] private PointableUnityEventWrapper nextMacButton;
        [SerializeField] private TextMeshPro selectedMac;
        [Space(20)]
        [SerializeField] private BluetoothPlugin bluetoothPlugin;

        private string _macAddress = string.Empty;

        private List<KeyValuePair<string, string>> _deviceMap = new List<KeyValuePair<string, string>>(); // Name-MAC list
        private int currentIndex = 0;

        private void Start()
        {
            connectButton.WhenRelease.AddListener(ConnectHandler);
            disconnectButton.WhenRelease.AddListener(DisconnectHandler);
            pairButton.WhenRelease.AddListener(PairHandler);

            prevMacButton.WhenRelease.AddListener(PrevMacHandler);
            nextMacButton.WhenRelease.AddListener(NextMacHandler);

            UpdateCurrentDeviceUI();
        }

        private void OnDestroy()
        {
            connectButton.WhenRelease.RemoveListener(ConnectHandler);
            disconnectButton.WhenRelease.RemoveListener(DisconnectHandler);
            pairButton.WhenRelease.RemoveListener(PairHandler);

            prevMacButton.WhenRelease.RemoveListener(PrevMacHandler);
            nextMacButton.WhenRelease.RemoveListener(NextMacHandler);
        }

        private void ConnectHandler(PointerEvent arg)
        {
            if (_macAddress == String.Empty)
            {
                Debug.LogWarning("No MAC address is set");
                return;
            }

            if (bluetoothPlugin.IsConnected)
            {
                Debug.LogWarning("Connection is already set. Disconnect previous connection");
                return;
            }

            bluetoothPlugin.Connect(_macAddress);
        }

        private void DisconnectHandler(PointerEvent arg)
        {
            if (!bluetoothPlugin.IsConnected)
            {
                Debug.LogWarning("Connection is not set");
                return;
            }

            bluetoothPlugin.Disconnect();
        }

        private void PairHandler(PointerEvent arg)
        {
            string pairedDevices = bluetoothPlugin.GetPairedDevices();

            if (string.IsNullOrEmpty(pairedDevices))
            {
                Debug.LogWarning("No paired devices found.");
                return;
            }

            string[] devices = pairedDevices.Split(';');
            foreach (string device in devices)
            {
                if (!string.IsNullOrEmpty(device))
                {
                    string[] deviceInfo = device.Split(new[] { "::" }, StringSplitOptions.None);
                    if (deviceInfo.Length == 2)
                    {
                        string name = deviceInfo[0];
                        string macAddress = deviceInfo[1];
                        _deviceMap.Add(new KeyValuePair<string, string>(name, macAddress));
                    }
                }
            }

            UpdateCurrentDeviceUI();
        }

        private void NextMacHandler(PointerEvent arg)
        {
            if (_deviceMap.Count == 0) return;

            currentIndex = (currentIndex + 1) % _deviceMap.Count;
            UpdateCurrentDeviceUI();
        }
        private void PrevMacHandler(PointerEvent arg)
        {
            if (_deviceMap.Count == 0) return;

            currentIndex = (currentIndex - 1 + _deviceMap.Count) % _deviceMap.Count;
            UpdateCurrentDeviceUI();
        }

        private void UpdateCurrentDeviceUI()
        {
            if (_deviceMap.Count == 0 || currentIndex < 0 || currentIndex >= _deviceMap.Count)
            {
                selectedMac.text = "No devices";
                return;
            }

            var currentDevice = _deviceMap[currentIndex];
            //selectedMac.text = $"{currentDevice.Key} ({currentDevice.Value})";
            selectedMac.text = currentDevice.Key;
            _macAddress = currentDevice.Value;
        }
    }
}
