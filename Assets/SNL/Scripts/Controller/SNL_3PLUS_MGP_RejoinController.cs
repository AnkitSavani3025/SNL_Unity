using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_RejoinController : MonoBehaviour {
        internal bool isDisconnected;

        private void Start(){
            Application.runInBackground = false;
        }

        private void OnApplicationPause(bool pauseStatus){
            if (pauseStatus){
                isDisconnected = true;
                SNL_3PLUS_MGP_GameManager.instance.socketHandler.CloseSocketForceFully();
                SNL_3PLUS_MGP_GameManager.instance.loadingController.OpenLoadingScreen("Reconnecting...");
            }
        }

        internal void StartCheckAndReconnectFunction(){
            CancelInvoke(nameof(CloseAndReconnectCheck));
            InvokeRepeating(nameof(CloseAndReconnectCheck), 1f, 1f);
        }

        private void CloseAndReconnectCheck(){
            StartCoroutine(CheckAndReconnectSocket());
        }

        private IEnumerator CheckAndReconnectSocket(){
            if (SNL_3PLUS_MGP_GameManager.instance.socketHandler.socketState != SocketState.Disconnect &&
                SNL_3PLUS_MGP_GameManager.instance.socketHandler.socketState != SocketState.Error &&
                SNL_3PLUS_MGP_GameManager.instance.socketHandler.socketState != SocketState.None){
                yield return new WaitForSecondsRealtime(1);
            }
            else{
                if (SNL_3PLUS_MGP_GameManager.instance.IsInternetAvailable()){
                    SNL_3PLUS_MGP_GameManager.instance.socketHandler.ConnectSocket();
                }
                else{
                    SNL_3PLUS_MGP_GameManager.instance.loadingController.CloseLoadingScreen();
                    SNL_3PLUS_MGP_GameManager.instance.ShowNoInternetPanel();
                    SNL_3PLUS_MGP_GameManager.instance.ShowNetwork(4);
                }

                yield return new WaitForSecondsRealtime(1);
            }
        }
    }
}