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
using Java.IO;
using Android.Net.Wifi;
using static Mobile_Adhoc_Triangulator.DeviceListFragment;

namespace Mobile_Adhoc_Triangulator
{
    class DeviceDetailFragment : Fragment, WifiP2pManager.IConnectionInfoListener
    {
        protected static readonly int CHOOSE_FILE_RESULT_CODE = 20;
        private View mContentView = null;
        private WifiP2pDevice device;
        private WifiP2pInfo info;
        ProgressDialog progressDialog = null;

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            //mContentView = inflater.Inflate(Resource.layout.device_detail, null);
            //mContentView.FindViewById(R.id.btn_connect).SetOnClickListener(new ViewOnClickListnerConnect(Activity, device, progressDialog));
            //mContentView.FindViewById(R.id.btn_disconnect).SetOnClickListener(new ViewOnClickListnerDisconnect(Activity));
            /*mContentView.findViewById(R.id.btn_start_client).setOnClickListener(
                new View.OnClickListener() {
                    @Override
                    public void onClick(View v)
    {
        // Allow user to pick an image from Gallery or other
        // registered apps
        Intent intent = new Intent(Intent.ACTION_GET_CONTENT);
        intent.setType("image/*");
        startActivityForResult(intent, CHOOSE_FILE_RESULT_CODE);
    }
});*/
            return mContentView;
        }

        public void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            // User has picked an image. Transfer it to group owner i.e peer using
            // FileTransferService.
            Android.Net.Uri uri = data.Data;
            //TextView statusText = (TextView)mContentView.FindViewById(R.id.status_text);
            //statusText.Text = "Sending: " + uri;
            //Log.Debug(WiFiDirectActivity.TAG, "Intent----------- " + uri);
            /*Intent serviceIntent = new Intent(this.Activity, FileTransferService.class);
                serviceIntent.setAction(FileTransferService.ACTION_SEND_FILE);
                serviceIntent.putExtra(FileTransferService.EXTRAS_FILE_PATH, uri.toString());
                serviceIntent.putExtra(FileTransferService.EXTRAS_GROUP_OWNER_ADDRESS,
                        info.groupOwnerAddress.getHostAddress());
                serviceIntent.putExtra(FileTransferService.EXTRAS_GROUP_OWNER_PORT, 8988);
                getActivity().startService(serviceIntent);*/
        }

        public void OnConnectionInfoAvailable(WifiP2pInfo info)
        {
            if (progressDialog != null && progressDialog.IsShowing)
            {
                progressDialog.Dismiss();
            }
            this.info = info;
            this.View.Visibility = ViewStates.Visible;
            // The owner IP is now known.
            /*TextView view = (TextView)mContentView.FindViewById(R.id.group_owner);
            view.Text = this.Resources.GetString(R.string.group_owner_text)
                    + ((info.IsGroupOwner == true) ? Resources.GetString(R.string.yes)
                            : this.Resources.GetString(R.string.no)));
            InetAddress from WifiP2pInfo struct.
            view = (TextView)mContentView.FindViewById(R.id.device_info);
            view.Text = "Group Owner IP - " + info.GroupOwnerAddress.HostAddress);
            // After the group negotiation, we assign the group owner as the file
            // server. The file server is single threaded, single connection server
            // socket.
            if (info.GroupFormed && info.IsGroupOwner)
            {
                new FileServerAsyncTask(this.Activity, mContentView.FindViewById(R.id.status_text))
                        .execute();
            }
            else if (info.GroupFormed)
            {
                // The other device acts as the client. In this case, we enable the
                // get file button.
                mContentView.FindViewById(R.id.btn_start_client).setVisibility(ViewStates.Visible);
                ((TextView)mContentView.FindViewById(R.id.status_text)).setText(this.Resources
                        .GetString(R.string.client_text));
            }
            // hide the connect button
            mContentView.FindViewById(R.id.btn_connect).setVisibility(ViewStates.Gone);*/
        }

        /**
         * Updates the UI with device data
         * 
         * @param device the device to be displayed
         */
        public void ShowDetails(WifiP2pDevice device)
        {
            this.device = device;
            View.Visibility = (ViewStates.Visible);
            //TextView view = (TextView)mContentView.FindViewById(R.id.device_address);
            //view.Text = device.DeviceAddress;
            //view = (TextView)mContentView.FindViewById(R.id.device_info);
            //view.Text = device.ToString();
        }

        /**
         * Clears the UI fields after a disconnect or direct mode disable operation.
         */
        public void ResetViews()
        {
            /*mContentView.FindViewById(R.id.btn_connect).setVisibility(ViewStates.Visible);
            TextView view = (TextView)mContentView.FindViewById(R.id.device_address);
            view.SetText(R.string.empty);
            view = (TextView)mContentView.FindViewById(R.id.device_info);
            view.SetText(R.string.empty);
            view = (TextView)mContentView.FindViewById(R.id.group_owner);
            view.SetText(R.string.empty);
            view = (TextView)mContentView.FindViewById(R.id.status_text);
            view.SetText(R.string.empty);
            mContentView.FindViewById(R.id.btn_start_client).setVisibility(ViewStates.Gone);*/
            ((View)GetView()).Visibility = ViewStates.Gone;
        }

        private object GetView()
        {
            throw new NotImplementedException();
        }

        private class ViewOnClickListnerConnect : Java.Lang.Object, View.IOnClickListener
        {
            Activity Activity;
            private WifiP2pDevice device;
            private ProgressDialog progressDialog;

            public ViewOnClickListnerConnect(Activity Activity, WifiP2pDevice device, ProgressDialog progressDialog) : base()
            {
                this.Activity = Activity;
                this.device = device;
                this.progressDialog = progressDialog;
            }

            public void OnClick(View v)
            {
                WifiP2pConfig config = new WifiP2pConfig();
                config.DeviceAddress = device.DeviceAddress;
                config.Wps.Setup = WpsInfo.Pbc;
                if (progressDialog != null && progressDialog.IsShowing)
                {
                    progressDialog.Dismiss();
                }
                progressDialog = ProgressDialog.Show(Activity, "Press back to cancel",
                        "Connecting to :" + device.DeviceAddress, true, true
                        //                        new DialogInterface.OnCancelListener() {
                        //
                        //                            @Override
                        //                            public void onCancel(DialogInterface dialog) {
                        //                                ((DeviceActionListener) getActivity()).cancelDisconnect();
                        //                            }
                        //                        }
                        );
                ((IDeviceActionListener)Activity).Connect(config);
            }
        }

        private class ViewOnClickListnerDisconnect : Java.Lang.Object, View.IOnClickListener
        {
            Activity Activity;

            public ViewOnClickListnerDisconnect(Activity Activity) : base()
            {
                this.Activity = Activity;
            }

            public void OnClick(View v)
            {
                ((IDeviceActionListener)Activity).Disconnect();
            }
        }
    }
}