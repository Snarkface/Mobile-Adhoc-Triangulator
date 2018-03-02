using System;

using Android.Content;
using Android.Net.Wifi.P2p;
using Android.Util;

namespace Mobile_Adhoc_Triangulator
{
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
            mManager.DiscoverPeers(mChannel, new WifiDirectActionListener());
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


    }

}