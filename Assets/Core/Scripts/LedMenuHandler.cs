using Oculus.Interaction;
using UnityEngine;

namespace Core.Scripts
{
    public class LedMenuHandler : MonoBehaviour
    {
        [SerializeField] private PointableUnityEventWrapper turnOnButton;
        [SerializeField] private PointableUnityEventWrapper turnOffButton;
        [Space(20)]
        [SerializeField] private MeshRenderer mesh;
        [Space(20)]
        [SerializeField] private Color onColor = Color.red;
        [SerializeField] private Color offColor = new Color(0x76, 0x76, 0x76);
        [Space(20)]
        [SerializeField] private BluetoothPlugin bluetoothPlugin;

        private void Start()
        {
            mesh.materials[0].color = offColor;

            turnOnButton.WhenRelease.AddListener(TurnOnHandler);
            turnOffButton.WhenRelease.AddListener(TurnOffHandler);
        }

        private void OnDestroy()
        {
            turnOnButton.WhenRelease.RemoveListener(TurnOnHandler);
            turnOffButton.WhenRelease.RemoveListener(TurnOffHandler);
        }

        private void Update()
        {
            // Poll data periodically
            // For demo purposes I don't want to add event-based system
            string receivedData = bluetoothPlugin.ReceiveData();
            if (!string.IsNullOrEmpty(receivedData))
            {
                MessageParser(receivedData);
            }
        }

        private void MessageParser(string message)
        {
            var receivedData= message.TrimEnd('\n', '\r', '\0'); ;
            
            if (string.Equals(receivedData, StaticConstants.LedOnStatus))
            {
                Debug.Log("LED STATUS CHANGED: ON");
                LedStatusChanged(true);
                return;
            }

            if (string.Equals(receivedData, StaticConstants.LedOffStatus))
            {
                Debug.Log("LED STATUS CHANGED: OFF");
                LedStatusChanged(false);
                return;
            }
        }

        private void TurnOnHandler(PointerEvent arg)
        {
            if (!bluetoothPlugin.IsConnected)
                return;

            string msg = StaticConstants.CommandLedOn + "\n";
            bluetoothPlugin.SendData(msg);
        }

        private void TurnOffHandler(PointerEvent arg)
        {
            if (!bluetoothPlugin.IsConnected)
                return;

            string msg = StaticConstants.CommandLedOff + "\n";
            bluetoothPlugin.SendData(msg);
        }

        private void LedStatusChanged(bool status)
        {
            if (mesh)
                mesh.materials[0].color = status == true ? onColor : offColor;
        }

    }
}
