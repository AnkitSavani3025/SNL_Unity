using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_HeartBeatManager : MonoBehaviour
    {
        internal int pingMissedCounter;
        internal long pingTime;
        internal long pongTime;

        internal bool isInternetDisconnected = false;
        public static bool isInternetPopupOpened;
        public static bool isPongReceived;
        public bool checkInternetWithAPI;
        public bool checkInternetWithHeartBeat;
        public bool needToCheckInternet;

        private int badDelayCounter;
        private int errorCounter;

        public int PING_INTERVAL = 1;
        public int firstRequestDelay = 2;
        public int totalErrorCounter = 6;
        public int totalbadDelayCounter = 6;

        private Coroutine pingPongWithAPICorotine;
        private Coroutine pingPongWithHeartBeatCorotine;

        public void CallSendPingInvoke()
        {
            pingMissedCounter = 0;
            ResetPingRepeating();
            InvokeRepeating(nameof(SendPing), 1f, SNL_3PLUS_MGP_GameManager.instance.networkPingTime);
        }

        public void ResetPingRepeating()
        {
            pingMissedCounter = 0;
            CancelInvoke(nameof(SendPing));
            CancelInvoke(nameof(CheckPingPongCount));
        }

        internal void SendPing()
        {
            pingTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            isPongReceived = false;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("Send Ping Time : " + pingTime);
            }

            SNL_3PLUS_MGP_GameManager.instance.eventManager.SendHeartBeatEvent();
        }

        internal void CheckPingPongCount()
        {
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("NetworkPingTime \t" + SNL_3PLUS_MGP_GameManager.instance.networkPingTime +
                          " \t MaxPingCounter \t" + SNL_3PLUS_MGP_GameManager.instance.maxPingCounter);
                Debug.Log("missPingNetwork \t" + pingMissedCounter + " \t PingCounter \t" + pingMissedCounter);
            }

            pingMissedCounter++; //1 ...2...3...4..5..6
            if (!SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable())
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("showing no network indicator");
                }

                SNL_3PLUS_MGP_GameManager.instance.ShowNoInternetPanel();
                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(4);
            }

            if (pingMissedCounter >= SNL_3PLUS_MGP_GameManager.instance.maxPingCounter)
            {
                ResetPingRepeating();
                SNL_3PLUS_MGP_GameManager.instance.socketHandler.CloseSocketForceFully();
                // No Internet popup should be displayed here
                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(4);
            }
        }

        internal void OnReceiveHB(JSONObject data)
        {
            isPongReceived = true;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("HeartBeat --> " + data);
            }

            pongTime = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
            long timeDuration = pongTime - pingTime;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
            {
                Debug.Log("Time Duration --> " + timeDuration);
            }

            if (errorCounter > 0 && errorCounter < totalErrorCounter) return;
            if (timeDuration >= SNL_3PLUS_MGP_GameManager.instance.pongTimer[0] &&
                timeDuration < SNL_3PLUS_MGP_GameManager.instance.pongTimer[1])
            {
                badDelayCounter = (badDelayCounter > 0) ? errorCounter-- : 0;
                //Fast Network
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("FAST NETWORK");
                }

                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(0);
            }
            else if (timeDuration > SNL_3PLUS_MGP_GameManager.instance.pongTimer[1] &&
                     timeDuration < SNL_3PLUS_MGP_GameManager.instance.pongTimer[2])
            {
                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(1);
                badDelayCounter = (badDelayCounter > 0) ? errorCounter-- : 0;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("AVERAGE NETWORK");
                }
            }
            else if (timeDuration > SNL_3PLUS_MGP_GameManager.instance.pongTimer[2] &&
                     timeDuration < SNL_3PLUS_MGP_GameManager.instance.pongTimer[3])
            {
                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(2);
                badDelayCounter++;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("LOW NETWORK");
                }
            }
            else if (timeDuration > SNL_3PLUS_MGP_GameManager.instance.pongTimer[3])
            {
                SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(3);
                badDelayCounter++;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("VERY LOW NETWORK");
                }
            }

            pingMissedCounter = 0;
            CancelInvoke(nameof(CheckPingPongCount));
        }

        public void CheckInternetWithPINGPONG()
        {
            badDelayCounter = errorCounter = 0;

            needToCheckInternet = true;
            StopCheckInternet();

            if (checkInternetWithHeartBeat)
                pingPongWithHeartBeatCorotine = StartCoroutine(GetRequestWithHeartBeat());
            else
                Debug.LogError(" Please Select Any METHOD for Internet Checking");
        }

        public void StopCheckInternet()
        {
            if (pingPongWithAPICorotine != null)
                StopCoroutine(pingPongWithAPICorotine);
            if (pingPongWithHeartBeatCorotine != null)
                StopCoroutine(pingPongWithHeartBeatCorotine);
        }

        #region Check Ping and TimeOut

        IEnumerator GetRequestWithHeartBeat()
        {
            yield return new WaitForSecondsRealtime(firstRequestDelay);
        //2-3

        SB:
            if (needToCheckInternet)
            {
                //Debug.Log(" Start Check ping Pong ");

                if (SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable())
                {
                    SendPing();

                    yield return new WaitForSecondsRealtime(PING_INTERVAL); //2
                    if (badDelayCounter > totalbadDelayCounter || errorCounter > totalErrorCounter)
                    {
                        //3>6 || 4>3
                        Debug.Log(" Auto Reconnect badDelayCounter || " + badDelayCounter + "|| errorCounter  " +
                                  errorCounter);
                        StopCheckInternet();
                        SNL_3PLUS_MGP_GameManager.instance.ResetGame();
                    }
                    else
                    {
                        if (isPongReceived)
                        {
                            errorCounter = (errorCounter > 3) ? errorCounter-- : 0;
                        }
                        else
                        {
                            errorCounter++;
                            Debug.Log("<Color=red>Ping server call missing:: </Color>" + errorCounter +
                                      "SocketState:: " + SNL_3PLUS_MGP_GameManager.instance.socketHandler.socketState +
                                      " IsInternet Available: " + SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable());
                            SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(errorCounter);
                        }

                        goto SB;
                    }
                }
                else
                {
                    SNL_3PLUS_MGP_GameManager.instance.ShowNoInternetPanel();
                }
            }
            else
            {
                Debug.LogError(" NO Need to Check Connection || needToCheckInternet => " + needToCheckInternet);
            }
        }

        #endregion
    }
}