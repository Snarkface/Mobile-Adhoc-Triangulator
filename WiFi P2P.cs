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


using Android.Net.Wifi;
using Android.Net.Wifi.P2p;
using static Android.Net.Wifi.P2p.WifiP2pManager;

namespace Mobile_Adhoc_Triangulator
{
    /**
    * A BroadcastReceiver that notifies of important Wi-Fi p2p events.
    */
    public class WiFiDirectBroadcastReceiver : BroadcastReceiver
    {
        private WifiP2pManager mManager;
        private Channel mChannel;

        public WiFiDirectBroadcastReceiver(WifiP2pManager manager, Channel channel) : base()
        {
            this.mManager = manager;
            this.mChannel = channel;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            String action = intent.Action;

            if (WifiP2pManager.WifiP2pStateChangedAction.Equals(action))
            {
                // Check to see if Wi-Fi is enabled and notify appropriate activity
            }
            else if (WifiP2pManager.WifiP2pPeersChangedAction.Equals(action))
            {
                // Call WifiP2pManager.requestPeers() to get a list of current peers
            }
            else if (WifiP2pManager.WifiP2pConnectionChangedAction.Equals(action))
            {
                // Respond to new connection or disconnections
            }
            else if (WifiP2pManager.WifiP2pThisDeviceChangedAction.Equals(action))
            {
                // Respond to this device's wifi state changing
            }
        }
    }
}