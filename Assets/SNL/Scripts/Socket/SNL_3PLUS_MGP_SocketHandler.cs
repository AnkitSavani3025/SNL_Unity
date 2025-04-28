using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.Extensions;
using BestHTTP.SocketIO3;
using MGPSDK;
using UnityEngine;

namespace SNL_3PLUS_MGP
{
    public enum SocketState
    {
        None,
        Close,
        Connect,
        Open,
        Running,
        Error,
        Disconnect
    }

    public class SNL_3PLUS_MGP_SocketHandler : MonoBehaviour
    {
        internal SocketManager socketManager;
        public SocketState socketState;
        public string socketURL;
        internal string userId;

        private void Awake()
        {
            socketState = SocketState.None;
        }

        internal bool isFirstInteraction;

        public void Start()
        {
            if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
            {
                if (!SNL_3PLUS_MGP_GameManager.instance.playerCountSelectionScreen.activeInHierarchy)
                {
                    isFirstInteraction = true;
                    SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus = GameProgressStatus.GAMEPLAY;
                }

                if (int.Parse(MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber) != 0)
                    socketURL = MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.hostURL + ":" +
                                MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber;
                else // No Need to Assign port number if directly code run on server
                    socketURL = MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails
                        .hostURL; // + ":" + MGPSDK.MGPGameManager.instance.sdkConfig.data.socketDetails.portNumber;

                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log(" SocketHandler  || Start || ISFTUE => " +
                              MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE);
                }

                if (MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE)
                {
                    SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus = GameProgressStatus.FTUE;
                    SNL_3PLUS_MGP_GameManager.instance.ShowFTUE();
                }
            }

            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log(" SocketHandler  || Start  || socketURL  " + socketURL);
            }

            HttpManagerSettings();
            CreateSocket();
        }

        private void HttpManagerSettings()
        {
            // HTTPManager.HTTP2Settings.WebSocketOverHTTP2Settings.EnableWebSocketOverHTTP2 = true;
            // HTTPManager.HTTP2Settings.WebSocketOverHTTP2Settings.EnableImplementationFallback = true;
            HTTPManager.MaxConnectionPerServer = 10;
            HTTPManager.KeepAliveDefaultValue = false;
            HTTPManager.IsCachingDisabled = true;
            HTTPManager.MaxConnectionIdleTime = TimeSpan.FromSeconds(120);
            HTTPManager.IsCookiesEnabled = false;
            HTTPManager.CookieJarSize = 1048576;
            HTTPManager.EnablePrivateBrowsing = true;
            HTTPManager.ConnectTimeout = TimeSpan.FromSeconds(120);
            HTTPManager.RequestTimeout = TimeSpan.FromSeconds(120);
            //  HTTPManager.UseAlternateSSLDefaultValue = true;
            // HTTPManager.HTTP2Settings.MaxConcurrentStreams = 512;
            HTTPManager.UserAgent = string.Empty;
        }

        public void CreateSocket()
        {
            if (socketManager != null)
            {
                if (socketManager.Socket.IsOpen)
                {
                    socketManager.Socket.Disconnect();
                }

                socketManager = null;
            }

            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("creating socket from here>> ...>>>>>>>>>>");
            }

            if (!socketURL.Contains("socket.io"))
                socketURL = socketURL + "/socket.io/";
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("Connecting Socket to URL " + socketURL);
            }

            socketState = SocketState.None;
            socketManager = new SocketManager(new Uri(socketURL), SetSocketOption());
            if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
            {
                if (!MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE)
                {
                    SNL_3PLUS_MGP_GameManager.instance.loadingController.OpenLoadingScreen("Please Wait...");
                }
            }

            SNL_3PLUS_MGP_GameManager.instance.rejoinController.StartCheckAndReconnectFunction();
            //SNL_3PLUS_MGP_GameManager.instance.heartBeatManager.CallSendPingInvoke();
            RegisterEvents();
            SNL_3PLUS_MGP_GameManager.instance.heartBeatManager.CheckInternetWithPINGPONG();
        }

        public SocketOptions SetSocketOption()
        {
            SocketOptions socketOptions = new SocketOptions();
            socketOptions.ConnectWith = BestHTTP.SocketIO3.Transports.TransportTypes.WebSocket;
            socketOptions.Reconnection = true;
            socketOptions.ReconnectionAttempts = int.MaxValue;
            socketOptions.ReconnectionDelay = TimeSpan.FromMilliseconds(1000);
            socketOptions.ReconnectionDelayMax = TimeSpan.FromMilliseconds(5000);
            socketOptions.RandomizationFactor = 0.5f;
            socketOptions.Timeout = TimeSpan.FromMilliseconds(10000);
            socketOptions.AutoConnect = true;
            socketOptions.QueryParamsOnlyForHandshake = true;
            if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("Auth Token  || Recived from backend " +
                              MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken);
                }

                socketOptions.Auth = (manager, socket) =>
                    new { token = MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken };
            }

            return socketOptions;
        }

        private void RegisterEvents()
        {
            OnSocketConnect();
            OnSocketDisconnect();
            OnSocketError();
            socketManager.Socket.On<string>("res", (res) =>
            {
                var data = res;
                if (data == null) return;
                socketState = SocketState.Running;
                var receiveJson = new JSONObject(data.Trim('"'));
                SNL_3PLUS_MGP_GameManager.instance.socketEventReceiver.ReceiveData(receiveJson);
            });
        }

        private void OnSocketConnect()
        {
            socketManager.Socket.On(SocketIOEventTypes.Connect, () =>
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("OnSocketConnected() : Socket Status onconnect: " + socketManager.Socket.IsOpen +
                              "\t socketId " +
                              socketManager.Socket.Id);
                }

                if (socketManager.Socket.IsOpen)
                {
                    socketState = SocketState.Connect;
                    SNL_3PLUS_MGP_GameManager.instance.MakeButtonInteractable();
                    SNL_3PLUS_MGP_GameManager.instance.loadingController.CloseLoadingScreen();
                    SNL_3PLUS_MGP_GameManager.instance.HideNoInternetPanel();
                    if (SNL_3PLUS_MGP_GameManager.instance.rejoinController.isDisconnected ||
                        isFirstInteraction && SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
                    {
                        isFirstInteraction = false;
                        switch (SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus)
                        {
                            case GameProgressStatus.FTUE:
                                SNL_3PLUS_MGP_GameManager.instance.rejoinController.isDisconnected = false;
                                break;
                            case GameProgressStatus.GAMEPLAY:
                                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                                {
                                    Debug.Log("TotalPlayerCount --> " +
                                              SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount);
                                }

                                SNL_3PLUS_MGP_GameManager.instance.loadingController
                                    .OpenLoadingScreen("Please Wait...");
                                SNL_3PLUS_MGP_GameManager.instance.eventManager.SendSignUp(SNL_3PLUS_MGP_GameManager
                                    .instance.totalPlayerCount);
                                break;
                            case GameProgressStatus.WINNINGSCREEN:
                                break;
                            case GameProgressStatus.KICKOUT:
                                break;
                        }
                    }
                }
            });
        }

        private void OnSocketDisconnect()
        {
            socketManager.Socket.On(SocketIOEventTypes.Disconnect, () =>
            {
                if (!SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable())
                {
                    SNL_3PLUS_MGP_GameManager.instance.loadingController.CloseLoadingScreen();
                    SNL_3PLUS_MGP_GameManager.instance.ShowNoInternetPanel();
                    SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(4);
                }

                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("Socket DISCONNECTED: " + socketManager.Socket.IsOpen + " Socket ID : " +
                              socketManager.Socket.Id);
                }

                socketState = SocketState.Disconnect;
                if (SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable())
                {
                    SNL_3PLUS_MGP_GameManager.instance.loadingController.OpenLoadingScreen("Reconnecting...");
                }

                SNL_3PLUS_MGP_GameManager.instance.rejoinController.isDisconnected = true;
            });
        }

        private void OnSocketError()
        {
            socketManager.Socket.On<CustomError>(SocketIOEventTypes.Error,
                (errordata) => { Debug.Log("OnSocketError Error Message: " + errordata.message); });
        }

        public void SendEventData(JSONObject jsonData, string eventName)
        {
            try
            {
                if (!eventName.Equals("HEART_BEAT"))
                {
                    if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                    {
                        Debug.Log("CLIENT SENDING EVENT: " + eventName + "\t DATA: " + jsonData);
                    }
                }

                socketManager.Socket.Emit("req", jsonData.ToString());
            }
            catch (Exception ex)
            {
                Debug.LogError("Exception SendData Methods -> " + ex.ToString());
            }
        }

        internal void CloseSocketForceFully()
        {
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("CLIENT CLOSING SOCKET FORCEFULLY");
            }

            if (socketManager.Socket.IsOpen)
                (socketManager as IManager).Close(false);
        }

        internal void ConnectSocket()
        {
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("Open Socket Manually");
            }

            socketManager.Open();
            SNL_3PLUS_MGP_GameManager.instance.heartBeatManager.CheckInternetWithPINGPONG();
        }

        internal void CloseAndRemoveSocket()
        {
            socketManager.Close();
        }
    }

    public class CustomError : Error
    {
        public ErrorData data;

        public override string ToString()
        {
            return $"[CustomError {message}, {data?.code}, {data?.content}]";
        }
    }

    public class ErrorData
    {
        public int code;
        public string content;
    }
}