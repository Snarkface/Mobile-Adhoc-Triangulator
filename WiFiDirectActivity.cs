using Android.App;
using Android.Content;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Provider;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;

namespace Mobile_Adhoc_Triangulator
{
    /**
     * An activity that uses WiFi Direct APIs to discover and connect with available
     * devices. WiFi Direct APIs are asynchronous and rely on callback mechanism
     * using interfaces to notify the application of operation success or failure.
     * The application should also register a BroadcastReceiver for notification of
     * WiFi state related events.
     */
    public class WiFiDirectActivity : Activity, WifiP2pManager.IChannelListener, DeviceListFragment.IDeviceActionListener
    {
        public static readonly String TAG = "wifidirectdemo";
        private WifiP2pManager manager;
        private bool isWifiP2pEnabled = false;
        private bool retryChannel = false;
        private readonly IntentFilter intentFilter = new IntentFilter();
        private WifiP2pManager.Channel channel;
        private BroadcastReceiver receiver = null;

        /**
         * @param isWifiP2pEnabled the isWifiP2pEnabled to set
         */
        public void SetIsWifiP2pEnabled(bool isWifiP2pEnabled)
        {
            this.isWifiP2pEnabled = isWifiP2pEnabled;
        }

        public new void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            // add necessary intent values to be matched.
            intentFilter.AddAction(WifiP2pManager.WifiP2pStateChangedAction);
            intentFilter.AddAction(WifiP2pManager.WifiP2pPeersChangedAction);
            intentFilter.AddAction(WifiP2pManager.WifiP2pConnectionChangedAction);
            intentFilter.AddAction(WifiP2pManager.WifiP2pThisDeviceChangedAction);
            manager = (WifiP2pManager)GetSystemService(Context.WifiP2pService);
            channel = manager.Initialize(this, MainLooper, null);
        }

        /** register the BroadcastReceiver with the intent values to be matched */
        public new void OnResume()
        {
            base.OnResume();
            receiver = new WiFiDirectBroadcastReceiver(manager, channel, this);
            RegisterReceiver(receiver, intentFilter);
        }

        public new void OnPause()
        {
            base.OnPause();
            UnregisterReceiver(receiver);
        }

        /**
         * Remove all peers and clear all fields. This is called on
         * BroadcastReceiver receiving a state change event.
         */
        public void ResetData()
        {
            DeviceListFragment fragmentList = (DeviceListFragment)FragmentManager
                    .FindFragmentById(Resource.Id.frag_list);
            DeviceDetailFragment fragmentDetails = (DeviceDetailFragment)FragmentManager
                    .FindFragmentById(Resource.Id.frag_detail);
            if (fragmentList != null)
            {
                fragmentList.ClearPeers();
            }
            if (fragmentDetails != null)
            {
                fragmentDetails.ResetViews();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater inflater = MenuInflater;
            inflater.Inflate(Resource.Menu.action_items, menu);
            return true;
        }

        /*
         * (non-Javadoc)
         * @see android.app.Activity#onOptionsItemSelected(android.view.MenuItem)
         */
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.atn_direct_enable:
                    if (manager != null && channel != null)
                    {
                        // Since this is the system wireless settings activity, it's
                        // not going to send us a result. We will be notified by
                        // WiFiDeviceBroadcastReceiver instead.
                        StartActivity(new Intent(Settings.ActionWirelessSettings));
                    }
                    else
                    {
                        Log.Error(TAG, "channel or manager is null");
                    }
                    return true;
                case Resource.Id.atn_direct_discover:
                    if (!isWifiP2pEnabled)
                    {
                        Toast.MakeText(this, Resource.String.p2p_off_warning,
                                ToastLength.Short).Show();
                        return true;
                    }
                    DeviceListFragment fragment = (DeviceListFragment)FragmentManager
                            .FindFragmentById(Resource.Id.frag_list);
                    fragment.OnInitiateDiscovery();
                    /*manager.DiscoverPeers(channel, new WifiP2pManager.ActionListener() {
                        @Override
                            public void onSuccess()
                        {
                            Toast.makeText(WiFiDirectActivity.this, "Discovery Initiated",
                                    Toast.LENGTH_SHORT).show();
                        }
                        @Override
                            public void onFailure(int reasonCode)
                        {
                            Toast.makeText(WiFiDirectActivity.this, "Discovery Failed : " + reasonCode,
                                    Toast.LENGTH_SHORT).show();
                        }
                    });*/
                    return true;
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

        public void ShowDetails(WifiP2pDevice device)
        {
            DeviceDetailFragment fragment = (DeviceDetailFragment)FragmentManager
                    .FindFragmentById(Resource.Id.frag_detail);
            fragment.ShowDetails(device);
        }

        public void Connect(WifiP2pConfig config)
        {
            /*manager.Connect(channel, config, new ActionListener() {
                    @Override
                    public void onSuccess()
            {
                // WiFiDirectBroadcastReceiver will notify us. Ignore for now.
            }
            @Override
                    public void onFailure(int reason)
            {
                Toast.makeText(WiFiDirectActivity.this, "Connect failed. Retry.",
                        Toast.LENGTH_SHORT).show();
            }
        });*/
        }


        public void Disconnect()
        {
            DeviceDetailFragment fragment = (DeviceDetailFragment)FragmentManager
                    .FindFragmentById(Resource.Id.frag_detail);
            fragment.ResetViews();
            /*manager.removeGroup(channel, new ActionListener() {
                    @Override
                    public void onFailure(int reasonCode)
            {
                Log.d(TAG, "Disconnect failed. Reason :" + reasonCode);
            }
            @Override
                    public void onSuccess()
            {
                fragment.getView().setVisibility(View.GONE);
            }
        });*/
        }

        public void OnChannelDisconnected()
        {
            // we will try once more
            if (manager != null && !retryChannel)
            {
                Toast.MakeText(this, "Channel lost. Trying again", ToastLength.Long).Show();
                ResetData();
                retryChannel = true;
                manager.Initialize(this, MainLooper, this);
            }
            else
            {
                Toast.MakeText(this,
                        "Severe! Channel is probably lost premanently. Try Disable/Re-Enable P2P.",
                        ToastLength.Long).Show();
            }
        }

        public void CancelDisconnect()
        {
            /*
             * A cancel abort request by user. Disconnect i.e. removeGroup if
             * already connected. Else, request WifiP2pManager to abort the ongoing
             * request
             */
            if (manager != null)
            {
                DeviceListFragment fragment = (DeviceListFragment)FragmentManager
                        .FindFragmentById(Resource.Id.frag_list);
                if (fragment.GetDevice() == null
                        || fragment.GetDevice().Status == WifiP2pDeviceState.Connected)
                {
                    Disconnect();
                }
                /*
                else if (fragment.GetDevice().Status == WifiP2pDeviceState.Available
                      || fragment.GetDevice().Status == WifiP2pDeviceState.Invited)
                {
                    manager.CancelConnect(channel, new ActionListener() {
                            @Override
                            public void onSuccess()
                    {
                        Toast.makeText(WiFiDirectActivity.this, "Aborting connection",
                                Toast.LENGTH_SHORT).show();
                    }
                    @Override
                            public void onFailure(int reasonCode)
                    {
                        Toast.makeText(WiFiDirectActivity.this,
                                "Connect abort request failed. Reason Code: " + reasonCode,
                                Toast.LENGTH_SHORT).show();
                    }
                });*/
            }
        }
    }
}