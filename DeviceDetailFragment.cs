using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Net.Wifi.P2p;
using Android.OS;
using Android.Util;
using Android.Views;
using Android.Widget;
using System.IO;
using System.Threading.Tasks;

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
            mContentView = inflater.Inflate(Resource.Layout.device_detail, null);
            mContentView.FindViewById(Resource.Id.btn_connect).SetOnClickListener(new ViewOnClickListnerConnect(Activity, device, progressDialog));
            mContentView.FindViewById(Resource.Id.btn_disconnect).SetOnClickListener(new ViewOnClickListnerDisconnect(Activity));
            mContentView.FindViewById(Resource.Id.btn_start_client).SetOnClickListener(new ViewOnClickListnerImage(Activity));
            return mContentView;
        }

        public void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            // User has picked an image. Transfer it to group owner i.e peer using
            // FileTransferService.
            Android.Net.Uri uri = data.Data;
            TextView statusText = (TextView)mContentView.FindViewById(Resource.Id.status_text);
            statusText.Text = "Sending: " + uri;
            Log.Debug(WiFiDirectActivity.TAG, "Intent----------- " + uri);
            Intent serviceIntent = new Intent(this.Activity, typeof(FileTransferService));
            serviceIntent.SetAction(FileTransferService.ACTION_SEND_FILE);
            serviceIntent.PutExtra(FileTransferService.EXTRAS_FILE_PATH, uri.ToString());
            serviceIntent.PutExtra(FileTransferService.EXTRAS_GROUP_OWNER_ADDRESS,
                        info.GroupOwnerAddress.HostAddress);
            serviceIntent.PutExtra(FileTransferService.EXTRAS_GROUP_OWNER_PORT, 8988);
            Activity.StartService(serviceIntent);
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
            TextView view = (TextView)mContentView.FindViewById(Resource.Id.group_owner);
            view.Text = Resources.GetString(Resource.String.group_owner_text)
                    + ((info.IsGroupOwner == true) ? Resources.GetString(Resource.String.yes)
                            : Resources.GetString(Resource.String.no));
            // InetAddress from WifiP2pInfo struct.
            view = (TextView)mContentView.FindViewById(Resource.Id.device_info);
            view.Text = "Group Owner IP - " + info.GroupOwnerAddress.HostAddress;
            // After the group negotiation, we assign the group owner as the file
            // server. The file server is single threaded, single connection server
            // socket.
            if (info.GroupFormed && info.IsGroupOwner)
            {
                TextView statusText = (TextView)mContentView.FindViewById(Resource.Id.status_text);
                new Task(() => { FileServerAsyncTask(Activity, statusText); }).Start();
            }
            else if (info.GroupFormed)
            {
                // The other device acts as the client. In this case, we enable the
                // get file button.
                mContentView.FindViewById(Resource.Id.btn_start_client).Visibility = ViewStates.Visible;
                ((TextView)mContentView.FindViewById(Resource.Id.status_text)).Text = this.Resources.GetString(Resource.String.client_text);
            }
            // hide the connect button
            mContentView.FindViewById(Resource.Id.btn_connect).Visibility = ViewStates.Gone;
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
            TextView view = (TextView)mContentView.FindViewById(Resource.Id.device_address);
            view.Text = device.DeviceAddress;
            view = (TextView)mContentView.FindViewById(Resource.Id.device_info);
            view.Text = device.ToString();
        }

        /**
         * Clears the UI fields after a disconnect or direct mode disable operation.
         */
        public void ResetViews()
        {
            mContentView.FindViewById(Resource.Id.btn_connect).Visibility = ViewStates.Visible;
            TextView view = (TextView)mContentView.FindViewById(Resource.Id.device_address);
            view.SetText(Resource.String.empty);
            view = (TextView)mContentView.FindViewById(Resource.Id.device_info);
            view.SetText(Resource.String.empty);
            view = (TextView)mContentView.FindViewById(Resource.Id.group_owner);
            view.SetText(Resource.String.empty);
            view = (TextView)mContentView.FindViewById(Resource.Id.status_text);
            view.SetText(Resource.String.empty);
            mContentView.FindViewById(Resource.Id.btn_start_client).Visibility = ViewStates.Gone;
            View.Visibility = ViewStates.Gone;
        }

        private class ViewOnClickListnerConnect : Java.Lang.Object, View.IOnClickListener
        {
            Activity activity;
            private WifiP2pDevice device;
            private ProgressDialog progressDialog;

            public ViewOnClickListnerConnect(Activity activity, WifiP2pDevice device, ProgressDialog progressDialog) : base()
            {
                this.activity = activity;
                this.device = device;
                this.progressDialog = progressDialog;
            }

            public void OnClick(View v)
            {
                WifiP2pConfig config = new WifiP2pConfig
                {
                    DeviceAddress = device.DeviceAddress
                };
                config.Wps.Setup = WpsInfo.Pbc;
                if (progressDialog != null && progressDialog.IsShowing)
                {
                    progressDialog.Dismiss();
                }
                progressDialog = ProgressDialog.Show(activity, "Press back to cancel",
                        "Connecting to :" + device.DeviceAddress, true, true
                        //                        new DialogInterface.OnCancelListener() {
                        //
                        //                            @Override
                        //                            public void onCancel(DialogInterface dialog) {
                        //                                ((DeviceActionListener) getActivity()).cancelDisconnect();
                        //                            }
                        //                        }
                        );
                ((DeviceListFragment.IDeviceActionListener)activity).Connect(config);
            }
        }

        private class ViewOnClickListnerDisconnect : Java.Lang.Object, View.IOnClickListener
        {
            Activity activity;

            public ViewOnClickListnerDisconnect(Activity activity) : base()
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                ((DeviceListFragment.IDeviceActionListener)activity).Disconnect();
            }
        }

        private class ViewOnClickListnerImage : Java.Lang.Object, View.IOnClickListener
        {
            Activity activity;

            public ViewOnClickListnerImage(Activity activity) : base()
            {
                this.activity = activity;
            }

            public void OnClick(View v)
            {
                // Allow user to pick an image from Gallery or other
                // registered apps
                Intent intent = new Intent(Intent.ActionGetContent);
                intent.SetType("image/*");
                activity.StartActivityForResult(intent, CHOOSE_FILE_RESULT_CODE);
            }
        }

        /**
         *  Temporary example of implemantation
         *  Will be reimplemnted in clean C#
         */
        public void FileServerAsyncTask(Context context, TextView statusText)
        {
            statusText.Text = "Opening a server socket";
            string result = null;
            try
            {
                LocalServerSocket serverSocket = new LocalServerSocket("8898");
                Log.Debug(WiFiDirectActivity.TAG, "Server: Socket opened");
                LocalSocket client = serverSocket.Accept();
                client.Connect(new LocalSocketAddress(""));
                Log.Debug(WiFiDirectActivity.TAG, "Server: connection done");
                FileInfo f = new FileInfo(Environment.ExternalStorageDirectory + "/"
                        + context.PackageName + "/wifip2pshared-" + Java.Lang.JavaSystem.CurrentTimeMillis()
                        + ".jpg");
                DirectoryInfo dirs = f.Directory;
                if (!dirs.Exists)
                    dirs.Create();
                f.Create();
                Log.Debug(WiFiDirectActivity.TAG, "server: copying files " + f.ToString());
                Stream inputstream = client.InputStream;
                CopyFile(inputstream, f.OpenRead());
                serverSocket.Close();
                result = f.FullName;
            }
            catch (IOException e)
            {
                Log.Debug(WiFiDirectActivity.TAG, e.Message);
            }
            finally
            {
                if (result != null)
                {
                    statusText.Text = "File copied - " + result;
                    Intent intent = new Intent();
                    intent.SetAction(Intent.ActionView);
                    intent.SetDataAndType(Uri.Parse("file://" + result), "image/*");
                    context.StartActivity(intent);
                }
            }
        }

        public static bool CopyFile(Stream inputStream, Stream outp)
        {
            byte[] buf = new byte[1024];
            int len;
            try
            {
                while ((len = inputStream.Read(buf, 0, 1024)) != -1)
                {
                    outp.Write(buf, 0, len);
                }
                outp.Close();
                inputStream.Close();
            }
            catch (IOException e)
            {
                Log.Debug(WiFiDirectActivity.TAG, e.ToString());
                return false;
            }
            return true;
        }
    }
}