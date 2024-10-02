#region my
///documentation:
///Client API reference: https://docs.aws.amazon.com/gamelift/latest/developerguide/realtime-sdk-csharp-ref-actions.html
///realtime servers interface reference: https://docs.aws.amazon.com/gamelift/latest/developerguide/realtime-script-objects.html
///realtime server callbacks reference: https://docs.aws.amazon.com/gamelift/latest/developerguide/realtime-script-callbacks.html
///Original realTimeServerScript.js from Amazon: https://docs.aws.amazon.com/gamelift/latest/developerguide/realtime-script.html
#endregion

using System;
using System.Text;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime.Types;
using Newtonsoft.Json.Linq;

public class RealTimeClient
{
    public Aws.GameLift.Realtime.Client Client { get; private set; }

    #region test OPCode
    // An opcode defined by client and your server script that represents a custom message type
    private const int MY_TEST_OP_CODE = 10;
    #endregion

    /// Initialize a client for GameLift Realtime and connect to a player session.
    /// <param name="endpoint">The DNS name that is assigned to Realtime server</param>
    /// <param name="remoteTcpPort">A TCP port for the Realtime server</param>
    /// <param name="listeningUdpPort">A local port for listening to UDP traffic</param>
    /// <param name="connectionType">Type of connection to establish between client and the Realtime server</param>
    /// <param name="playerSessionId">The player session ID that is assigned to the game client for a game session </param>
    /// <param name="connectionPayload">Developer-defined data to be used during client connection, such as for player authentication</param>
    public RealTimeClient(string endpoint, int remoteTcpPort, int listeningUdpPort, ConnectionType connectionType,
                 string playerSessionId, byte[] connectionPayload)
    {
        // Create a client configuration to specify a secure or unsecure connection type
        // Best practice is to set up a secure connection using the connection type RT_OVER_WSS_DTLS_TLS12.
        ClientConfiguration clientConfiguration = new ClientConfiguration()
        {
            // C# notation to set the field ConnectionType in the new instance of ClientConfiguration
            ConnectionType = connectionType
        };

        // Create a Realtime client with the client configuration            
        Client = new Client(clientConfiguration);

        // Initialize event handlers for the Realtime client
        Client.ConnectionOpen += OnOpenEvent;
        Client.ConnectionClose += OnCloseEvent;
        Client.GroupMembershipUpdated += OnGroupMembershipUpdate;
        Client.DataReceived += OnDataReceived;

        // Create a connection token to authenticate the client with the Realtime server
        // Player session IDs can be retrieved using AWS SDK for GameLift
        ConnectionToken connectionToken = new ConnectionToken(playerSessionId, connectionPayload);

        // Initiate a connection with the Realtime server with the given connection information
        Client.Connect(endpoint, remoteTcpPort, listeningUdpPort, connectionToken);
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

    //standard message
    public void SendMessage(DeliveryIntent intent, string payload)
    {
        Client.SendMessage(Client.NewMessage(MY_TEST_OP_CODE)
            .WithDeliveryIntent(intent)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(payload)));
    }


    //new SendMessage with OPCODE
    public void SendStringToServer(DeliveryIntent intent, int opcode, string payload)
    {
        //listen to moveEvemt
        //payload: JSON-Message with matching OP-Code
        Client.SendMessage(Client.NewMessage(opcode)
            .WithDeliveryIntent(intent)
            .WithTargetPlayer(Constants.PLAYER_ID_SERVER)
            .WithPayload(StringToBytes(payload)));
    }

    public virtual void OnDataReceived(object sender, DataReceivedEventArgs e)
    {
        // handle message based on OpCode
        #region M3 OPCODES
        /*{
                   "board":
                   [
                       [1,2,3,4,5],
                       [1,2,3,4,5],
                       [1,2,3,4,5],
                       [1,2,3,4,5],
                       [1,2,3,4,5]
                   ],
                   "wheel": [1,2,3],

                   "player": "player1ID"
               }*/

        #region Event Opcodes
        /*
        public static PlayerReadyEvent playerready = new PlayerReadyEvent();    //10
        public static BoardConfigEvent boardConfig = new BoardConfigEvent();    //11
        public static PlayerHasTurn playerHasTurn = new PlayerHasTurn();        //20
        public static MoveEvent move = new MoveEvent();                         //30
        public static OpponentMoveEvent opponentMove = new OpponentMoveEvent(); //30
        public static GameOverEvent gameOver = new GameOverEvent();             //40
        public static DisconnectEvent disconect = new DisconnectEvent();        //300
        */
        #endregion

        #region Server Opcodes
        /*
        Sender: Server ->accepted player                //1
        Sender: Server ->acknowledge player disconnect  //2
        Sender: player ->player wants to play again     //5
        Sender: Server ->opponent wants to play again   //5
        Sender: Server ->opponent left please leave     //300
        */

        #endregion

        #endregion

        string result = BytesToString(e.Data);
        
        switch (e.OpCode)
        {
            case 1:  //player has been accepted
                break;
            case 2:  //player has disconnected
                break;
            case 5:  //opponent wants to play again
                break;
            case 10: //both players connected
                //EventManager.playerready.Invoke(); //triggers event
                break;
            case 11: //both players ready
                boardConfig(result);
                break;
            case 20: //player x has turn
                //string arg = JObject.Parse(result)["playerId"].ToString();//gets playerID
                //EventManager.playerHasTurn.Invoke(arg);
                break;
            case 30: //its my turn
                //prevMove: X (e.g. “L1”) newCard: Y (numbers 0 - 6)
                //OpponentMoveEventData(MatchFieldEnums.Row row, MatchFieldEnums.Slot slot, ItemEnums.Item newItem, GameObject item = null)
                break;
            case 40: //game ends
                //string arg = JObject.Parse(result)["playerId"].ToString();//gets playerID
                //EventManager.gameOver.Invoke(arg);
                break;
            case 300: //opponent disconnected please leave session
                //string message = JObject.Parse(result)["playerId"].ToString();//gets message
                //EventManager.disconect.Invoke(message);
                break;
            case 311: //player didnt send the correct data
                break;
            case 320: //turn was incorrect(game rules)
                break;
            default:
                break;
        }
    }

    #region connection open event
    /**
     * Handle connection open events
     */
    public void OnOpenEvent(object sender, EventArgs e)
    {
    }
    #endregion

    #region connection close event
    /**
     * Handle connection close events
     */
    public void OnCloseEvent(object sender, EventArgs e)
    {

    }
    #endregion

    /**
     * Handle Group membership update events 
     */
    public void OnGroupMembershipUpdate(object sender, GroupMembershipEventArgs e)
    {
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

    //testing method
    public ItemEnums.Item GetItem(int number)
    {
        switch (number)
        {
            case 0:
                return ItemEnums.Item.apple;
            case 1:
                return ItemEnums.Item.banana;
            case 2:
                return ItemEnums.Item.grape;
            case 3:
                return ItemEnums.Item.orange;
            case 4:
                return ItemEnums.Item.cherry;
            case 5:
                return ItemEnums.Item.pineapple;
            case 6:
                return ItemEnums.Item.watermelon;
            default:
                return ItemEnums.Item.empty;
        }
    }
    private void boardConfig(String result) //OPCODE 11
    {
        //nach .Parse(result) mit selectToken probieren falls errors entstehen
        int[,] boardArr = JObject.Parse(result)["board"]?.ToObject<int[,]>();
        int[] wheelArr = JObject.Parse(result)["wheel"]?.ToObject<int[]>();

        ItemEnums.Item[,] boardArray = new ItemEnums.Item[5, 5];
        ItemEnums.Item[] wheelArray = new ItemEnums.Item[wheelArr.Length];

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                boardArray[i, j] = GetItem(boardArr[i, j]);
            }
        }

        for (int i = 0; i < wheelArr.Length; i++)
        {
            wheelArray[i] = GetItem(wheelArr[i]);
        }

        BoardConfigEventData data = new BoardConfigEventData(boardArray, wheelArray);
        //EventManager.boardConfig.Invoke(data);
    }
}