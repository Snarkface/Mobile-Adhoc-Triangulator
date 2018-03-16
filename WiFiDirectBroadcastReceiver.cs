using Android.Content;
using Android.Net;
using Android.Net.Wifi.P2p;
using Android.Util;
using System;

namespace Mobile_Adhoc_Triangulator
{
    /**
     * A BroadcastReceiver that notifies of important wifi p2p events.
     */
    [BroadcastReceiver]
    public class WiFiDirectBroadcastReceiver : BroadcastReceiver
    {
        private WifiP2pManager manager;
        private WifiP2pManager.Channel channel;
        private WiFiDirectActivity activity;

        /**
         * @param manager WifiP2pManager system service
         * @param channel Wifi p2p channel
         * @param activity activity associated with the receiver
         */
        public WiFiDirectBroadcastReceiver(WifiP2pManager manager, WifiP2pManager.Channel channel, WiFiDirectActivity activity) : base()
        {
            this.manager = manager;
            this.channel = channel;
            this.activity = activity;
        }

        /*
         * (non-Javadoc)
         * @see android.content.BroadcastReceiver#onReceive(android.content.Context,
         * android.content.Intent)
         */
        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;
            if (WifiP2pManager.WifiP2pStateChangedAction.Equals(action))
            {
                // UI update to indicate wifi p2p status.
                int state = intent.GetIntExtra(WifiP2pManager.ExtraWifiState, -1);
                if ((WifiP2pState)state == WifiP2pState.Enabled)
                {
                    // Wifi Direct mode is enabled
                    activity.SetIsWifiP2pEnabled(true);
                }
                else
                {
                    activity.SetIsWifiP2pEnabled(false);
                    activity.ResetData();
                }
                Log.Debug(WiFiDirectActivity.TAG, "P2P state changed - " + state);
            }
            else if (WifiP2pManager.WifiP2pPeersChangedAction.Equals(action))
            {
                // request available peers from the wifi p2p manager. This is an
                // asynchronous call and the calling activity is notified with a
                // callback on PeerListListener.onPeersAvailable()
                if (manager != null)
                {
                    manager.RequestPeers(channel, (WifiP2pManager.IPeerListListener)activity.FragmentManager
                            .FindFragmentById(Resource.Id.frag_list));
                }
                Log.Debug(WiFiDirectActivity.TAG, "P2P peers changed");
            }
            else if (WifiP2pManager.WifiP2pConnectionChangedAction.Equals(action))
            {
                if (manager == null)
                {
                    return;
                }
                NetworkInfo networkInfo = (NetworkInfo)intent
                        .GetParcelableExtra(WifiP2pManager.ExtraNetworkInfo);
                if (networkInfo.IsConnected)
                {
                    // we are connected with the other device, request connection
                    // info to find group owner IP
                    DeviceDetailFragment fragment = (DeviceDetailFragment)activity
                            .FragmentManager.FindFragmentById(Resource.Id.frag_detail);
                    manager.RequestConnectionInfo(channel, fragment);
                }
                else
                {
                    // It's a disconnect
                    activity.ResetData();
                }
            }
            else if (WifiP2pManager.WifiP2pThisDeviceChangedAction.Equals(action))
            {
                DeviceListFragment fragment = (DeviceListFragment)activity.FragmentManager
                        .FindFragmentById(Resource.Id.frag_list);
                fragment.UpdateThisDevice((WifiP2pDevice)intent.GetParcelableExtra(
                        WifiP2pManager.ExtraWifiP2pDevice));
            }
        }
    }
}