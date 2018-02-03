using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;


using Android.Net.Wifi.P2p;
using Android.Util;

namespace Mobile_Adhoc_Triangulator
{
    public class WiFiDirectActivity : Activity
    {

        WiFiDirectBroadcastReceiver receiver;
        WifiP2pManager mManager;
        WifiP2pManager.Channel mChannel;
        bool isWifiP2pEnabled;
        //PeerListListner peerListListener;

        private IntentFilter intentFilter = new IntentFilter();
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

    public class WiFiDirectBroadcastReceiver : BroadcastReceiver
    {
        private WifiP2pManager mManager;
        private WifiP2pManager.Channel mChannel;
        private WiFiDirectActivity activity;
        private PeerListListener peerListListener;

        public WiFiDirectBroadcastReceiver(WifiP2pManager mManager, WifiP2pManager.Channel mChannel, WiFiDirectActivity activity) : base()
        {
            this.mManager = mManager;
            this.mChannel = mChannel;
            this.activity = activity;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (WifiP2pManager.WifiP2pStateChangedAction.Equals(action))
            {
                // Determine if Wifi P2P mode is enabled or not, alert
                // the Activity.
                int state = intent.GetIntExtra(WifiP2pManager.ExtraWifiState, -1);
                if (state == int.Parse(WifiP2pManager.WifiP2pStateEnabled.ToString()))
                {
                    activity.SetIsWifiP2pEnabled(true);
                }
                else
                {
                    activity.SetIsWifiP2pEnabled(false);
                }
            }
            else if (WifiP2pManager.WifiP2pPeersChangedAction.Equals(action))
            {

                // Request available peers from the wifi p2p manager. This is an
                // asynchronous call and the calling activity is notified with a
                // callback on PeerListListener.onPeersAvailable()
                if (mManager != null)
                {
                    mManager.RequestPeers(mChannel, peerListListener);
                }
                Log.Debug("WiFiDirectActivity", "P2P peers changed");

            }
            else if (WifiP2pManager.WifiP2pConnectionChangedAction.Equals(action))
            {

                // Connection state changed! We should probably do something about
                // that.

            }
            else if (WifiP2pManager.WifiP2pThisDeviceChangedAction.Equals(action))
            {
                /*
                DeviceListFragment fragment = (DeviceListFragment)activity.getFragmentManager()
                        .findFragmentById(R.id.frag_list);
                fragment.updateThisDevice((WifiP2pDevice)intent.getParcelableExtra(
                        WifiP2pManager.EXTRA_WIFI_P2P_DEVICE));
                */
            }
        }

    public class PeerListListener : Java.Lang.Object, WifiP2pManager.IPeerListListener
    {

        private List<WifiP2pDevice> peers = new List<WifiP2pDevice>();

        public void OnPeersAvailable(WifiP2pDeviceList peerList)
        {

            List<WifiP2pDevice> refreshedPeers = (List<WifiP2pDevice>)peerList.DeviceList;
            if (!refreshedPeers.Equals(peers))
            {
                peers.Clear();
                peers.AddRange(refreshedPeers);

                // Perform any other updates needed based on the new list of
                // peers connected to the Wi-Fi P2P network.
            }

            if (peers.Count == 0)
            {
                Log.Debug("WiFiDirectActivity", "No devices found");
                return;
            }

        }
    }
}