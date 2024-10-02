#region my
//https://github.com/aws-samples/megafrograce-gamelift-realtime-servers-sample/blob/3482a572048576f416560d802640097117dae896/MegaFrogRace/Assets/Scripts/RTSClient.cs#L396
#endregion

using Amazon;
using Amazon.CognitoIdentity;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Amazon.Runtime;
using Aws.GameLift.Realtime;
using Aws.GameLift.Realtime.Event;
using Aws.GameLift.Realtime.Types;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NetworkingManager : MonoBehaviour
{
    public static NetworkingManager Instance { get; private set; } //???????
    public RealTimeClient _realTimeClient;
    public Button InvokeButton = null;
    public string CognitoIdentityRegion = RegionEndpoint.EUCentral1.SystemName;
    public string LambdaRegion = RegionEndpoint.EUCentral1.SystemName;
    private QueryStringParameters queryStringParameters;
    private Aws.GameLift.Realtime.Client _client;
    private const string DEFAULT_ENDPOINT = "127.0.0.1"; // ???
    private const int DEFAULT_TCP_PORT = 3001; //???
    private const int DEFAULT_UDP_PORT = 8921; // ???
    private Byte[] connectionPayload = new byte[64];

    private NetworkingManager()
    {

    }

    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    class QueryStringParameters
    {
        public string game { get; set; }
    }

    private void Start()
    {
        UnityInitializer.AttachToGameObject(this.gameObject);
        //InvokeButton.onClick.AddListener(() => { ConnectToGameLiftServer(); });
    }

    public void ConnectToGameLiftServer()
    {
        queryStringParameters = new QueryStringParameters { game = "M3" };
        Debug.Log("Reaching out to client service Lambda function");

        AWSConfigs.AWSRegion = "EUCentral1";
        AWSConfigs.HttpClient = AWSConfigs.HttpClientOption.UnityWebRequest;
        // paste this in from the Amazon Cognito Identity Pool console

        CognitoAWSCredentials credentials = new CognitoAWSCredentials(
            // die der GameLift-Instanz ist arn:aws:gamelift:eu-central-1:289499586032:fleet/fleet-2f620a33-1536-4732-afb9-a1d1fe7f390e
            "eu-central-1:2f620a33-1536-4732-afb9-a1d1fe7f390e",
            RegionEndpoint.EUCentral1); // Region is EUCentral1

        AmazonLambdaClient client = new AmazonLambdaClient(credentials, RegionEndpoint.EUCentral1);
        InvokeRequest request = new InvokeRequest
        {
            //FunctionName = "InvokeAsync",//name of lambda function //Dem Lambda-Skript muss mittels Query-String das Spiel mitgeteilt werden ("game" = "SSPS" oder "M3")
            FunctionName = "arn:aws:lambda:eu-central-1:289499586032:function:matchmaker",
            InvocationType = InvocationType.RequestResponse,
            Payload = JsonConvert.SerializeObject(queryStringParameters)


            /*
            * aws lambda invoke --function-name arn:aws:lambda:eu-central-1:289499586032:function:matchmaker --payload
            * '{"queryStringParameters":{"game":"SSPS"}}' server_response.json
            */
            //Payload = //The JSON that you want to provide to your Lambda function as input.

        };

        //create gameLift session
        client.InvokeAsync(request, (response) =>
         {
             if (response.Exception == null)
             {
                 if (response.Response.StatusCode == 200)
                 {
                     var payload = Encoding.ASCII.GetString(response.Response.Payload.ToArray()) + "\n";
                     var playerSessionObj = JsonUtility.FromJson<PlayerSessionObject>(payload);

                     if (playerSessionObj.FleetId == null)
                     {
                         Debug.Log($"Error in Lambda: {payload}");
                     }
                     else
                     {
                        //connect
                        JoinMatch(playerSessionObj.IpAddress, playerSessionObj.Port, playerSessionObj.PlayerSessionId);
                     }
                 }
             }
         });
    }
    public void JoinMatch(string playerSessionDns, string playerSessionPort, string playerSessionId)
    {
        //call realTimeClient
        int localPort = FindAvailableUDPPort(DEFAULT_UDP_PORT, DEFAULT_UDP_PORT + 20);
        _realTimeClient = new RealTimeClient(playerSessionDns, Int32.Parse(playerSessionPort), localPort, ConnectionType.RT_OVER_WS_UDP_UNSECURED, playerSessionId, connectionPayload);
    }

    private int FindAvailableUDPPort(int firstPort, int lastPort)
    {
        var UDPEndPoints = IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners();
        List<int> usedPorts = new List<int>();
        usedPorts.AddRange(from n in UDPEndPoints where n.Port >= firstPort && n.Port <= lastPort select n.Port);
        usedPorts.Sort();
        for (int testPort = firstPort; testPort <= lastPort; ++testPort)

        {
            if (!usedPorts.Contains(testPort))
            {
                return testPort;
            }
        }
        return -1;
    }
}
