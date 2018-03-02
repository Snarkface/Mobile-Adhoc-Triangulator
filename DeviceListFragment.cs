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
    public class DeviceListFragment : ListFragment, WifiP2pManager.IPeerListListener
    {
        private List<WifiP2pDevice> peers = new List<WifiP2pDevice>();
        ProgressDialog progressDialog = null;
        View mContentView = null;
        private WifiP2pDevice device;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            //this.SetListAdapter(new WiFiPeerListAdapter(this.Activity, R.layout.row_devices, peers));
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //mContentView = inflater.Inflate(R.layout.device_list, null);
            return mContentView;
        }

        /**
         * @return this device
         */
        public WifiP2pDevice GetDevice()
        {
            return device;
        }

        private static String GetDeviceStatus(WifiP2pDeviceState deviceStatus)
        {
            //Log.Debug(WiFiDirectActivity.TAG, "Peer status :" + deviceStatus);
            switch (deviceStatus)
            {
                case WifiP2pDeviceState.Available:
                    return "Available";
                case WifiP2pDeviceState.Invited:
                    return "Invited";
                case WifiP2pDeviceState.Connected:
                    return "Connected";
                case WifiP2pDeviceState.Failed:
                    return "Failed";
                case WifiP2pDeviceState.Unavailable:
                    return "Unavailable";
                default:
                    return "Unknown";
            }
        }
        /**
         * Initiate a connection with the peer.
         */
        @Override
        public void onListItemClick(ListView l, View v, int position, long id)
        {
            WifiP2pDevice device = (WifiP2pDevice)getListAdapter().getItem(position);
            ((DeviceActionListener)getActivity()).showDetails(device);
        }

        /**
         * Array adapter for ListFragment that maintains WifiP2pDevice list.
         */
        private class WiFiPeerListAdapter extends ArrayAdapter<WifiP2pDevice> {
            private List<WifiP2pDevice> items;
        /**
         * @param context
         * @param textViewResourceId
         * @param objects
         */
        @Override
            public View getView(int position, View convertView, ViewGroup parent)
        {
            View v = convertView;
            if (v == null)
            {
                LayoutInflater vi = (LayoutInflater)getActivity().getSystemService(
                        Context.LAYOUT_INFLATER_SERVICE);
                v = vi.inflate(R.layout.row_devices, null);
            }
            WifiP2pDevice device = items.get(position);
            if (device != null)
            {
                TextView top = (TextView)v.findViewById(R.id.device_name);
                TextView bottom = (TextView)v.findViewById(R.id.device_details);
                if (top != null)
                {
                    top.setText(device.deviceName);
                }
                if (bottom != null)
                {
                    bottom.setText(getDeviceStatus(device.status));
                }
            }
            return v;
        }
    }
    /**
     * Update UI for this device.
     * 
     * @param device WifiP2pDevice object
     */
    public void updateThisDevice(WifiP2pDevice device)
    {
        this.device = device;
        TextView view = (TextView)mContentView.findViewById(R.id.my_name);
        view.setText(device.deviceName);
        view = (TextView)mContentView.findViewById(R.id.my_status);
        view.setText(getDeviceStatus(device.status));
    }

    public void onPeersAvailable(WifiP2pDeviceList peerList)
    {
        if (progressDialog != null && progressDialog.isShowing())
        {
            progressDialog.dismiss();
        }
        peers.clear();
        peers.addAll(peerList.getDeviceList());
        ((WiFiPeerListAdapter)getListAdapter()).notifyDataSetChanged();
        if (peers.size() == 0)
        {
            Log.d(WiFiDirectActivity.TAG, "No devices found");
            return;
        }
    }
    public void clearPeers()
    {
        peers.clear();
        ((WiFiPeerListAdapter)getListAdapter()).notifyDataSetChanged();
    }
    /**
     * 
     */
    public void onInitiateDiscovery()
    {
        if (progressDialog != null && progressDialog.isShowing())
        {
            progressDialog.dismiss();
        }
        progressDialog = ProgressDialog.show(getActivity(), "Press back to cancel", "finding peers", true,
                true, new DialogInterface.OnCancelListener() {
                        @Override
                        public void onCancel(DialogInterface dialog)
        {

        }
    }

    /**
     * An interface-callback for the activity to listen to fragment interaction
     * events.
     */
    public interface DeviceActionListener
    {
        void showDetails(WifiP2pDevice device);
        void cancelDisconnect();
        void connect(WifiP2pConfig config);
        void disconnect();
    }

    public WiFiPeerListAdapter(Context context, int textViewResourceId,
                List<WifiP2pDevice> objects)
    {
        super(context, textViewResourceId, objects);
        items = objects;
    }

}