using System.Collections.Generic;
using Android.Net.Wifi.P2p;
using Android.Util;


namespace Mobile_Adhoc_Triangulator
{
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