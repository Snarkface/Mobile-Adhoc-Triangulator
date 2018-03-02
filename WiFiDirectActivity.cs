using Android.App;
using Android.Content;
using Android.OS;
using Android.Net.Wifi.P2p;

namespace Mobile_Adhoc_Triangulator
{
    public class WiFiDirectActivity : Activity
    {
        private IntentFilter intentFilter = new IntentFilter();
        private WiFiDirectBroadcastReceiver receiver;
        private WifiP2pManager mManager;
        private WifiP2pManager.Channel mChannel;
        bool isWifiP2pEnabled;

        public new void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            // Indicates a change in the Wi-Fi P2P status.
            intentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);

            // Indicates a change in the list of available peers.
            intentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);

            // Indicates the state of Wi-Fi P2P connectivity has changed.
            intentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);

            // Indicates this device's details have changed.
            intentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);

            mManager = (WifiP2pManager) GetSystemService(Context.WifiP2pService);
            mChannel = mManager.Initialize(this, MainLooper, null);
        }

        public new void OnResume()
        {
            base.OnResume();
            receiver = new WiFiDirectBroadcastReceiver(mManager, mChannel, this);
            RegisterReceiver(receiver, intentFilter);
        }

        public new void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(receiver);
        }

        private void RegisterReceiver(WiFiDirectBroadcastReceiver receiver, IntentFilter intentFilter)
        {
            // Need to Register Receiver
        }

        private void UnregisterReceiver(WiFiDirectBroadcastReceiver receiver, IntentFilter intentFilter)
        {
            // Need to Unregister Receiver
        }

        public void SetIsWifiP2pEnabled(bool isWifiP2pEnabled)
        {
            this.isWifiP2pEnabled = isWifiP2pEnabled;
        }
    }
}