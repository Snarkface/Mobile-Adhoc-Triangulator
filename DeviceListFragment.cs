using Android.App;
using Android.Content;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mobile_Adhoc_Triangulator
{
    /**
     * A ListFragment that displays available peers on discovery and requests the
     * parent activity to handle user interaction events
     */
    public class DeviceListFragment : ListFragment, WifiP2pManager.IPeerListListener
    {
        private List<WifiP2pDevice> peers = new List<WifiP2pDevice>();
        private ProgressDialog progressDialog = null;
        private View mContentView = null;
        private WifiP2pDevice device;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
            ListAdapter = new WiFiPeerListAdapter(Context, Resource.Layout.row_devices, peers, Activity);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            mContentView = inflater.Inflate(Resource.Layout.device_list, null);
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
        public override void OnListItemClick(ListView l, View v, int position, long id)
        {
            WifiP2pDevice device = (WifiP2pDevice)ListAdapter.GetItem(position);
            ((IDeviceActionListener)Activity).ShowDetails(device);
        }

        /**
         * Array adapter for ListFragment that maintains WifiP2pDevice list.
         */
        private class WiFiPeerListAdapter : ArrayAdapter<WifiP2pDevice>
        {
            private List<WifiP2pDevice> items;
            private Activity Activity;

            /**
             * @param context
             * @param textViewResourceId
             * @param objects
             */
            public WiFiPeerListAdapter(Context context, int textViewResourceId,
                            List<WifiP2pDevice> objects, Activity Activity) : base(context, textViewResourceId, objects)
            {
                this.items = objects;
                this.Activity = Activity;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View v = convertView;
                if (v == null)
                {
                    LayoutInflater vi = (LayoutInflater)Activity.GetSystemService(
                            Context.LayoutInflaterService);
                    v = vi.Inflate(Resource.Layout.row_devices, null);
                }
                WifiP2pDevice device = items.ElementAt(position);
                if (device != null)
                {
                    TextView top = (TextView)v.FindViewById(Resource.Id.device_name);
                    TextView bottom = (TextView)v.FindViewById(Resource.Id.device_details);
                    if (top != null)
                    {
                        top.Text = device.DeviceName;
                    }
                    if (bottom != null)
                    {
                        bottom.Text = GetDeviceStatus(device.Status);
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
        public void UpdateThisDevice(WifiP2pDevice device)
        {
            this.device = device;
            TextView view = (TextView)mContentView.FindViewById(Resource.Id.my_name);
            view.Text = device.DeviceName;
            view = (TextView)mContentView.FindViewById(Resource.Id.my_status);
            view.Text = GetDeviceStatus(device.Status);
        }

        public void OnPeersAvailable(WifiP2pDeviceList peerList)
        {
            if (progressDialog != null && progressDialog.IsShowing)
            {
                progressDialog.Dismiss();
            }
            peers.Clear();
            peers.AddRange(peerList.DeviceList);
            ((WiFiPeerListAdapter)ListAdapter).NotifyDataSetChanged();
            if (peers.Count == 0)
            {
                //Log.Debug(WiFiDirectActivity.TAG, "No devices found");
                return;
            }
        }

        public void ClearPeers()
        {
            peers.Clear();
            ((WiFiPeerListAdapter)ListAdapter).NotifyDataSetChanged();
        }

        /**
         *
         */
        public void OnInitiateDiscovery()
        {
            if (progressDialog != null && progressDialog.IsShowing)
            {
                progressDialog.Dismiss();
            }
            progressDialog = ProgressDialog.Show(Activity, "Press back to cancel", "finding peers", true,
                true, new DialogInterfaceOnCancelListener());
        }

        private class DialogInterfaceOnCancelListener : Java.Lang.Object, IDialogInterfaceOnCancelListener
        {
            public void OnCancel(IDialogInterface dialog)
            {
            }
        }

        /**
         * An interface-callback for the activity to listen to fragment interaction
         * events.
         */
        public interface IDeviceActionListener
        {
            void ShowDetails(WifiP2pDevice device);
            void CancelDisconnect();
            void Connect(WifiP2pConfig config);
            void Disconnect();
        }
    }
}
