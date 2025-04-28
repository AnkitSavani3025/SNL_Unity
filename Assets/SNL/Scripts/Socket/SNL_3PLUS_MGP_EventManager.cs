using System;
using System.Collections;
using System.Collections.Generic;
using BestHTTP.SocketIO3;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_EventManager : MonoBehaviour
    {
        private string userName;

        private void Start()
        {
            userName = "Arya" + Random.Range(0, 900);
        }

        private JSONObject SignUp(int playerCount)
        {
            var eventData = new JSONObject();
            var data = new JSONObject();

            if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
            {
                //eventData.AddField("en", "SIGNUP");
                //data.AddField("userId", SystemInfo.deviceUniqueIdentifier);
                //data.AddField("acessToken", SystemInfo.deviceUniqueIdentifier);
                //data.AddField("userName", userName);
                //data.AddField("userProfile", "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1668590057158.png");
                //data.AddField("mobileNumber", "123456789");
                //data.AddField("chips", 10);
                //data.AddField("token", "1");
                //data.AddField("bootValue", 1);
                //data.AddField("playerCount", playerCount);
                //data.AddField("isGameStartFresh", IsGameStartFresh());
                //eventData.AddField("data", data);
                //PlayerPrefs.SetInt("isGameStartFresh", 1);
                eventData.AddField("en", "SIGNUP");
                data.AddField("acessToken", MGPSDK.MGPGameManager.instance.sdkConfig.data.accessToken);
                data.AddField("userId", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.userID);
                data.AddField("userName", SystemInfo.deviceUniqueIdentifier.Substring(SystemInfo.deviceUniqueIdentifier.Length - 5));
                data.AddField("profilePic", MGPSDK.MGPGameManager.instance.sdkConfig.data.selfUserDetails.avatar);
                data.AddField("entryFee", float.Parse(MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.entryFee.ToString()));
                data.AddField("minPlayer", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.minPlayer);
                data.AddField("noOfPlayer", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.noOfPlayer);
                data.AddField("winningAmount", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.winningAmount.ToString());
                data.AddField("gameId", MGPSDK.MGPGameManager.instance.sdkConfig.data.gameData.gameId);
                data.AddField("lobbyId", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData._id);
                data.AddField("isFTUE", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE);
                data.AddField("gameModeId", MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.gameModeId);
                data.AddField("isBot", false);
                if (MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.gameModeName == "1 NUMBER")
                    data.AddField("mode", false);
                else
                    data.AddField("mode", true);

                data.AddField("latitude", MGPSDK.MGPGameManager.instance.sdkConfig.data.location.latitude.ToString());
                data.AddField("longitude", MGPSDK.MGPGameManager.instance.sdkConfig.data.location.longitude.ToString());

                data.AddField("token", "1");
                eventData.AddField("data", data);

            }
            else
            {
                eventData.AddField("en", "SIGNUP");
                data.AddField("userId", SystemInfo.deviceUniqueIdentifier);
                data.AddField("acessToken", SystemInfo.deviceUniqueIdentifier);
                data.AddField("userName", userName);
                data.AddField("userProfile", "https://artoon-game-platform.s3.amazonaws.com/mgp-3games/AvatarImages/avatarImage-1668590057158.png");
                data.AddField("mobileNumber", "123456789");
                data.AddField("chips", 10);
                data.AddField("token", "1");
                data.AddField("bootValue", 1);
                data.AddField("playerCount", playerCount);
                data.AddField("isGameStartFresh", IsGameStartFresh());
                eventData.AddField("data", data);
                PlayerPrefs.SetInt("isGameStartFresh", 1);
            }
            return eventData;
        }

        private bool IsGameStartFresh()
        {
            return PlayerPrefs.GetInt("isGameStartFresh") == 0;
        }

        internal void SendSignUp(int playerCount)
        {
            SNL_3PLUS_MGP_GameManager.instance.socketHandler.SendEventData(SignUp(playerCount), "SIGNUP");
        }

        internal void SendMoveToken(int tokenIndex)
        {
            var eventData = new JSONObject();
            var data = new JSONObject();
            eventData.AddField("en", "MOVE");
            data.AddField("userId", SNL_3PLUS_MGP_GameManager.instance.userDetail.userId);
            data.AddField("tableId", SNL_3PLUS_MGP_GameManager.instance.userDetail.tableId);
            data.AddField("seatIndex", SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex);
            data.AddField("pawn", tokenIndex);
            data.AddField("moveNumber", SNL_3PLUS_MGP_GameManager.instance.diceValue);
            eventData.AddField("data", data);
            SNL_3PLUS_MGP_GameManager.instance.socketHandler.SendEventData(eventData, "MOVE");
        }

        internal void SendLeaveTable()
        {
            var eventData = new JSONObject();
            var data = new JSONObject();
            eventData.AddField("en", "LEAVE_TABLE");
            data.AddField("userId", SNL_3PLUS_MGP_GameManager.instance.userDetail.userId);
            data.AddField("tableId", SNL_3PLUS_MGP_GameManager.instance.userDetail.tableId);
            data.AddField("playerChooseToExit", true);
            eventData.AddField("data", data);
            SNL_3PLUS_MGP_GameManager.instance.socketHandler.SendEventData(eventData, "LEAVE_TABLE");
        }

        internal void SendWinConfirmationEvent()
        {
            var eventData = new JSONObject();
            var data = new JSONObject();
            eventData.AddField("en", "WIN_CONFIRMATION");
            data.AddField("userId", SNL_3PLUS_MGP_GameManager.instance.userDetail.userId);
            eventData.AddField("data", data);
            SNL_3PLUS_MGP_GameManager.instance.socketHandler.SendEventData(eventData, "WIN_CONFIRMATION");
        }

        internal void SendHeartBeatEvent()
        {
            var eventData = new JSONObject();
            var data = new JSONObject();
            eventData.AddField("en", "HEART_BEAT");
            eventData.AddField("data", data);
            data.AddField("tableId", "cfdsff");
            SNL_3PLUS_MGP_GameManager.instance.socketHandler.SendEventData(eventData, "HEART_BEAT");
        }
    }

    public static class MetricsData
    {
        public static JSONObject GetRootObject()
        {
            JSONObject root = new JSONObject();
            JSONObject metrics = new JSONObject();
            metrics.AddField(MetricsKeys.RAMDOM_USERID_KEY, GetUID());
            long ctst = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            metrics.AddField(MetricsKeys.CURRENT_TIMESTAMP_KEY, ctst.ToString());
            metrics.AddField(MetricsKeys.CURRENT_TIMESTAMP_CURRENT_TIMESTAMP_SERVER_KEY, "");
            metrics.AddField(MetricsKeys.CURRENT_TIMESTAMP_SERVER_REPLY_KEY, "");
            metrics.AddField(MetricsKeys.CURRENT_TIMESTAMP_CLIENT_ACKNOWLEDGEMENT_KEY, "1.2");
            metrics.AddField(MetricsKeys.USER_ID_KEY, "");
            metrics.AddField(MetricsKeys.APK_VERSION_KEY, 101);
            metrics.AddField(MetricsKeys.TABLE_ID_KEY, "");
            root.AddField("metrics", metrics);
            return root;
        }

        static string GetUID()
        {
            Guid myuuid = Guid.NewGuid();
            string myuuidAsString = myuuid.ToString();
            return myuuidAsString;
        }
    }

    [Serializable]
    public class Root
    {
        public Metrics metrics;

        public Root()
        {
            Debug.Log("Root Default Constructor");
            metrics = new Metrics();
        }
    }

    [Serializable]
    public class Metrics
    {
        public string uuid;
        public string ctst;
        public string srct;
        public string srpt;
        public string crst;
        public string userId;
        public string apkVersion;
        public string tableId;

        public Metrics()
        {
            uuid = "";
            ctst = "";
            srct = "";
            srpt = "";
            crst = "";
            userId = "";
            apkVersion = "";
            tableId = "";
        }
    }

    public static class MetricsKeys
    {
        public const string RAMDOM_USERID_KEY = "uuid";
        public const string CURRENT_TIMESTAMP_KEY = "ctst";
        public const string CURRENT_TIMESTAMP_CURRENT_TIMESTAMP_SERVER_KEY = "srct";
        public const string CURRENT_TIMESTAMP_SERVER_REPLY_KEY = "srpt";
        public const string CURRENT_TIMESTAMP_CLIENT_ACKNOWLEDGEMENT_KEY = "crst";
        public const string USER_ID_KEY = "userId";
        public const string APK_VERSION_KEY = "apkVersion";
        public const string TABLE_ID_KEY = "tableId";
    }
}