using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_BoxProperty : MonoBehaviour {
        public Image trail;
        public Image blink;
        internal List<SNL_3PLUS_MGP_Token> tokensOnThisBox = new List<SNL_3PLUS_MGP_Token>();
        public List<SNL_3PLUS_MGP_Snake> snakes = new List<SNL_3PLUS_MGP_Snake>();

        public TokenRotateSide tokenRotateSide;

        private void OnEnable(){
            SNL_3PLUS_MGP_GameManager.OnAddToken += AddTokenToList;
            SNL_3PLUS_MGP_GameManager.OnRemoveToken += RemoveTokenFromList;
            SNL_3PLUS_MGP_GameManager.ResetBoxPropertyOnRejoin += ResetWhenReconnect;
        }

        private void OnDisable(){
            SNL_3PLUS_MGP_GameManager.OnAddToken -= AddTokenToList;
            SNL_3PLUS_MGP_GameManager.OnRemoveToken -= RemoveTokenFromList;
            SNL_3PLUS_MGP_GameManager.ResetBoxPropertyOnRejoin -= ResetWhenReconnect;
        }

        internal void AddTokenToThisBox(SNL_3PLUS_MGP_Token token){
            SNL_3PLUS_MGP_GameManager.instance.AddTokenToBox(name, token);
        }

        private void AddTokenToList(string boxName, SNL_3PLUS_MGP_Token token){
            if (!name.Equals(boxName)) return;
            if (!tokensOnThisBox.Contains(token)){
                if (IsSafeZoneGenerated(token)){
                    SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds
                        .tokenEnterSafePlace);
                }

                tokensOnThisBox.Add(token);
                token.transform.SetParent(transform);
                token.transform.position = Vector3.zero;
            }

            ResetTokenScaleAndPosition();
        }

        private void RemoveTokenFromList(SNL_3PLUS_MGP_Token token){
            if (token == null) return;
            for (var i = 0; i < tokensOnThisBox.Count; i++){
                if (tokensOnThisBox[i] != null){
                    if (tokensOnThisBox[i].name.Equals(token.name)){
                        tokensOnThisBox.Remove(tokensOnThisBox[i]);
                    }
                }
                else{
                    tokensOnThisBox.Remove(tokensOnThisBox[i]);
                }
            }

            ResetTokenScaleAndPosition();
        }

        private void ResetTokenScaleAndPosition(){
            if (tokensOnThisBox.Count == 0) return;
            var scaleValue = ReturnTokenScaleValue();
            foreach (var token in tokensOnThisBox){
                token.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);
            }

            SetTokenPositions();
        }

        private void ResetWhenReconnect(){
            tokensOnThisBox.Clear();
        }

        private bool IsSafeZoneGenerated(SNL_3PLUS_MGP_Token token){
            for (var i = 0; i < tokensOnThisBox.Count; i++){
                if (tokensOnThisBox[i].playerController.tokenColor == token.playerController.tokenColor){
                    return true;
                }
            }

            return false;
        }

        private float ReturnTokenScaleValue(){
            switch (tokensOnThisBox.Count){
                case 1:
                    return 1f;
                case 2:
                    return 0.9f;
                case 3:
                    return 0.8f;
                case 4:
                case 5:
                    return 0.7f;
                case 6:
                    return 0.65f;
                case 7:
                case 8:
                case 9:
                case 10:
                case 11:
                case 12:
                    return 0.45f;
                default:
                    return 1f;
            }
        }

        private void SetTokenPositions(){
            switch (tokensOnThisBox.Count){
                case 1:
                    SetSingleTokenPosition();
                    break;
                case 2:
                    SetTwoTokenPositions();
                    break;
                case 3:
                    SetThreeTokenPositions();
                    break;
                case 4:
                    SetFourTokenPositions();
                    break;
                case 5:
                    SetFiveTokenPositions();
                    break;
                case 6:
                    SetSixTokenPositions();
                    break;
                case 7:
                    SetSevenTokenPositions();
                    break;
                case 8:
                    SetEightTokenPositions();
                    break;
                case 9:
                    SetNineTokenPositions();
                    break;
                case 10:
                    SetTenTokenPositions();
                    break;
                case 11:
                    SetElevenTokenPositions();
                    break;
                case 12:
                    SetTwelveTokenPositions();
                    break;
            }
        }

        private void SetSingleTokenPosition(){
            tokensOnThisBox[0].transform.localPosition = Vector3.zero;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For Single Token");
            }
        }

        private void SetTwoTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-24f, 0, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(24f, 0, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 2 Tokens");
            }
        }

        private void SetThreeTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-29f, -22f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(29f, -22f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(0, 21f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 3 Tokens");
            }
        }

        private void SetFourTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-24f, 25f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(24f, 25f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(-24f, -25f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(24f, -25f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 4 Tokens");
            }
        }

        private void SetFiveTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-30f, 25f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(30f, 25f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(-30f, -25f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(30f, -25f, 0);
            tokensOnThisBox[4].transform.localPosition = Vector3.zero;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 5 Tokens");
            }
        }

        private void SetSixTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-34f, 25f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(0, 25f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(34f, 25f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(-34f, -25f, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(0, -25f, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(34, -25f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 6 Tokens");
            }
        }

        private void SetSevenTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-30f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(0, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(30f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = Vector3.zero;
            tokensOnThisBox[4].transform.localPosition = new Vector3(-30f, -32f, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(0, -32f, 0);
            tokensOnThisBox[6].transform.localPosition = new Vector3(30f, -32f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 7 Tokens");
            }
        }

        private void SetEightTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-30f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(0, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(30f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(-16f, 0, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(16f, 0, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(-30f, -32f, 0);
            tokensOnThisBox[6].transform.localPosition = new Vector3(0, -32f, 0);
            tokensOnThisBox[7].transform.localPosition = new Vector3(30f, -32f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 8 Tokens");
            }
        }

        private void SetNineTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-30f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(0, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(30f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(-30f, 0, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(30f, 0, 0);
            tokensOnThisBox[5].transform.localPosition = Vector3.zero;
            tokensOnThisBox[6].transform.localPosition = new Vector3(16f, 0, 0);
            tokensOnThisBox[7].transform.localPosition = new Vector3(-30f, -32f, 0);
            tokensOnThisBox[8].transform.localPosition = new Vector3(0, -32f, 0);
            tokensOnThisBox[9].transform.localPosition = new Vector3(30f, -32f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 9 Tokens");
            }
        }

        private void SetTenTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-36f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(-12f, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(12f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(36f, 32f, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(-24f, 0, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(24f, 0, 0);
            tokensOnThisBox[6].transform.localPosition = new Vector3(-36f, -32f, 0);
            tokensOnThisBox[7].transform.localPosition = new Vector3(-12f, -32f, 0);
            tokensOnThisBox[8].transform.localPosition = new Vector3(12f, -32f, 0);
            tokensOnThisBox[9].transform.localPosition = new Vector3(36f, -32f, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 10 Tokens");
            }
        }

        private void SetElevenTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-36f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(-12f, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(12f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(36f, 32f, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(-24f, 0, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(24f, 0, 0);
            tokensOnThisBox[6].transform.localPosition = new Vector3(-36f, -32f, 0);
            tokensOnThisBox[7].transform.localPosition = new Vector3(-12f, -32f, 0);
            tokensOnThisBox[8].transform.localPosition = new Vector3(12f, -32f, 0);
            tokensOnThisBox[9].transform.localPosition = new Vector3(36f, -32f, 0);
            tokensOnThisBox[10].transform.localPosition = Vector3.zero;
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 11 Tokens");
            }
        }

        private void SetTwelveTokenPositions(){
            tokensOnThisBox[0].transform.localPosition = new Vector3(-36f, 32f, 0);
            tokensOnThisBox[1].transform.localPosition = new Vector3(-12f, 32f, 0);
            tokensOnThisBox[2].transform.localPosition = new Vector3(12f, 32f, 0);
            tokensOnThisBox[3].transform.localPosition = new Vector3(36f, 32f, 0);
            tokensOnThisBox[4].transform.localPosition = new Vector3(-36f, -32f, 0);
            tokensOnThisBox[5].transform.localPosition = new Vector3(-12f, -32f, 0);
            tokensOnThisBox[6].transform.localPosition = new Vector3(12f, -32f, 0);
            tokensOnThisBox[7].transform.localPosition = new Vector3(36f, -32f, 0);
            tokensOnThisBox[8].transform.localPosition = new Vector3(-36f, 0, 0);
            tokensOnThisBox[9].transform.localPosition = new Vector3(-12f, 0, 0);
            tokensOnThisBox[10].transform.localPosition = new Vector3(12f, 0, 0);
            tokensOnThisBox[11].transform.localPosition = new Vector3(36f, 0, 0);
            if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                Debug.Log("Setting Token Position For 12 Tokens");
            }
        }
    }
}