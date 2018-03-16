using System;
using System.IO;
using System.Net.Sockets;
using Android.App;
using Android.Content;
using Android.Util;

namespace Mobile_Adhoc_Triangulator
{
    public class FileTransferService : IntentService
    {
        private static readonly int SOCKET_TIMEOUT = 5000;
        public static readonly string ACTION_SEND_FILE = "com.example.android.wifidirect.SEND_FILE";
        public static readonly string EXTRAS_FILE_PATH = "file_url";
        public static readonly string EXTRAS_GROUP_OWNER_ADDRESS = "go_host";
        public static readonly string EXTRAS_GROUP_OWNER_PORT = "go_port";
        public FileTransferService(String name) : base(name)
        {
        }
        public FileTransferService() : base(name: "FileTransferService")
        {
        }

        protected override void OnHandleIntent(Intent intent)
        {
            Context context = ApplicationContext;
            if (intent.Action.Equals(ACTION_SEND_FILE))
            {
                string fileUri = intent.Extras.GetString(EXTRAS_FILE_PATH);
                string host = intent.Extras.GetString(EXTRAS_GROUP_OWNER_ADDRESS);
                Socket socket2 = new Socket(SocketType.Stream, ProtocolType.IP);
                new NetworkStream(socket2);
                int port = intent.Extras.GetInt(EXTRAS_GROUP_OWNER_PORT);
                try
                {
                    Log.Debug(WiFiDirectActivity.TAG, "Opening client socket - ");
                    socket2.Bind(null);
                    socket2.Connect(host, port);
                    socket2.ReceiveTimeout = SOCKET_TIMEOUT;
                    socket2.SendTimeout = SOCKET_TIMEOUT;
                    Log.Debug(WiFiDirectActivity.TAG, "Client socket - " + socket2.Connected);
                    NetworkStream stream = new NetworkStream(socket2);
                    ContentResolver cr = context.ContentResolver;
                    NetworkStream inputStream = null;
                    try
                    {
                        inputStream = (NetworkStream)cr.OpenInputStream(Android.Net.Uri.Parse(fileUri));
                    }
                    catch (FileNotFoundException e)
                    {
                        Log.Debug(WiFiDirectActivity.TAG, e.ToString());
                    }
                    DeviceDetailFragment.CopyFile(inputStream, stream);
                    Log.Debug(WiFiDirectActivity.TAG, "Client: Data written");
                }
                catch (IOException e)
                {
                    Log.Error(WiFiDirectActivity.TAG, e.Message);
                }
                finally
                {
                    if (socket2 != null)
                    {
                        if (socket2.Connected)
                        {
                            try
                            {
                                socket2.Close();
                            }
                            catch (IOException e)
                            {
                                // Give up
                                Console.WriteLine(e.StackTrace);
                            }
                        }
                    }
                }
            }
        }
    }
}