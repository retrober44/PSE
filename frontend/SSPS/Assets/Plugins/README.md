# GameLift Realtime C# Client SDK
## Documentation
You can find the official GameLift documentation [here](https://aws.amazon.com/documentation/gamelift/).
## Building the SDK
### Minimum Requirements:
* Visual Studio 2015 or later (requires C#6 support)
* Visual Studio for Mac (to be confirmed)
* Xamarin Studio 5.8.1 or later
* Mono develop 7.x or later
* Dotnet Core SDK 2.1 or later
 
The package contains the solution: 
* GameLiftRealtimeClientSdkNet45.sln for .Net framework 4.5

To build, simply load up the solution in one of the supported IDEs, restore the Nuget packages and build it from there.

## Building the Native TLS library.
In order to leverage the TLS feature of this SDK, you will need to build the GameLiftRealtimeNative library.
From the root directory:
On Windows:
```
cd Native
cmake ..
msbuild ALL_BUILD.vcxproj /p:Configuration=Release
```
On MacOS:
```
cd Native
cmake ..
make
```

This will produce GameLiftRealtimeNative libraries under Native/Release. 
On Windows, copy those files over to the working directory of your game executable.

If using Mono on MacOS, add the LibGameLiftRealtimeNative.dylib to one of the following environment variables, depending on how you have your variables set up:
* DYLD_FRAMEWORK_PATH
* DYLD_LIBRARY_PATH
* DYLD_FALLBACK_FRAMEWORK_PATH

## Unity
### Add GameLift libraries to Unity Editor
The 4.5 solution is compatible with Unity. Once you build the solution to generate libraries, add them to your Unity project.
In the Unity Editor, import the following libraries produced by the build:
* GameLiftRealtimeClientSdkNet45.dll
* Google.Protobuf.dll
* log4net.dll
* SuperSocket.ClientEngine.dll
* WebSocket4Net.dll

Make sure to put all the DLLs in the Assets/Plugins directory.

### Check Scripting Runtime Version setting
Make sure you set the Scripting Runtime Version to the .Net solution you're using. 
Otherwise, Unity will throw errors when importing the DLLs.  
From the Unity editor, go to:  
File->Build Settings->Player Settings. Under Other Settings->Configuration->Scripting Runtime Version

At this point, you should be ready to start playing with the SDK!

## Initial Integration Example

```c#
using System;
using System.Text;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime.Types;

namespace Example
{
    /**
     * An example client that wraps the GameLift Realtime client SDK
     * 
     * You can redirect logging from the SDK by setting up the LogHandler as such:
     * ClientLogger.LogHandler = (x) => Console.WriteLine(x);
     *
     */
    class RealTimeClient
    {
        public Aws.GameLift.Realtime.Client Client { get; private set; }
        public bool OnCloseReceived { get; private set; }
        // An opcode defined by client and your server script that represents a custom message type
        private const int MY_TEST_OP_CODE = 10;

        /// <summary>
        /// Initialize a client for GameLift Realtime and connects to a player session.
        /// </summary>
        /// <param name="endpoint">The endpoint for the GameLift Realtime server to connect to</param>
        /// <param name="tcpPort">The TCP port for the GameLift Realtime server</param>
        /// <param name="localUdpPort">Local Udp listen port to use</param>
        /// <param name="playerSessionId">The player session Id in use - from CreatePlayerSession</param>
        /// <param name="connectionPayload"></param>
        public RealTimeClient(string endpoint, int tcpPort, int localUdpPort, string playerSessionId, byte[] connectionPayload)
        {
            this.OnCloseReceived = false;

            ClientConfiguration clientConfiguration = ClientConfiguration.Default();

            Client = new Aws.GameLift.Realtime.Client(clientConfiguration);
            Client.ConnectionOpen += new EventHandler(OnOpenEvent);
            Client.ConnectionClose += new EventHandler(OnCloseEvent);
            Client.GroupMembershipUpdated += new EventHandler<GroupMembershipEventArgs>(OnGroupMembershipUpdate);
            Client.DataReceived += new EventHandler<DataReceivedEventArgs>(OnDataReceived);

            ConnectionToken token = new ConnectionToken(playerSessionId, connectionPayload);
            Client.Connect(endpoint, tcpPort, localUdpPort, token);
        }

        public void Disconnect()
        {
            if (Client.Connected)
            {
                Client.Disconnect();
            }
        }

        public bool IsConnected()
        {
            return Client.Connected;
        }

        /// <summary>
        /// Example of sending to a custom message to the server.
        /// 
        /// Server could be replaced by known peer Id etc.
        /// </summary>
        /// <param name="intent">Choice of delivery intent ie Reliable, Fast etc. </param>
        /// <param name="payload">Custom payload to send with message</param>
        public void SendMessage(DeliveryIntent intent, string payload)
        {
            Client.SendMessage(Client.NewMessage(MY_TEST_OP_CODE)
                .WithDeliveryIntent(intent)
                .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
                .WithPayload(StringToBytes(payload)));
        }

        /**
         * Handle connection open events
         */
        public void OnOpenEvent(object sender, EventArgs e)
        {
        }

        /**
         * Handle connection close events
         */
        public void OnCloseEvent(object sender, EventArgs e)
        {
            OnCloseReceived = true;
        }

        /**
         * Handle Group membership update events 
         */
        public void OnGroupMembershipUpdate(object sender, GroupMembershipEventArgs e)
        {
        }

        /**
         *  Handle data received from the Realtime server 
         */
        public virtual void OnDataReceived(object sender, DataReceivedEventArgs e)
        {
            switch (e.OpCode)
            {
                // handle message based on OpCode
                default:
                    break;
            }
        }

        /**
         * Helper method to simplify task of sending/receiving payloads.
         */
        public static byte[] StringToBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /**
         * Helper method to simplify task of sending/receiving payloads.
         */
        public static string BytesToString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}
```

## Troubleshooting
Building the SDK uses Nuget to manage packages, if you run into issues with the missing dependent DLLs, check that
have a valid package source in your NuGet manager settings or similar, for example (```https://api.nuget.org/v3/index.json```)