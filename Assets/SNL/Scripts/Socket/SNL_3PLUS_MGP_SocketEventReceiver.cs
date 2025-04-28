using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_SocketEventReceiver : MonoBehaviour
    {
        private char trimCharArray = '"';

        private float gameStartTimerEnd;

        internal void ReceiveData(JSONObject responseData)
        {
            var en = responseData.GetField("en").ToString().Trim(trimCharArray);
            var data = responseData.GetField("Data");
            if (!en.Equals("HEART_BEAT"))
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log($"<color=blue>{en} >> Event Emit From Server End Response >>> {responseData} </color>");
                }
            }

            switch (en)
            {
                case "SIGNUP":
                    HandleSignUp(data);
                    break;
                case "CREATE_TABLE":
                    HandleCreateTable(data);
                    break;
                case "JOIN_TABLE":
                    HandleJoinTable(data);
                    break;
                case "NEW_USER":
                    HandleNewUser(data);
                    break;
                case "TABLE_LOCK":
                    HandleTableLock();
                    break;
                case "GAME_START":
                    HandleGameStart(data);
                    break;
                case "TURN_INFO":
                    HandleTurnInfo(data);
                    break;
                case "EXTRA_TIME":
                    HandleExtraTimer(data);
                    break;
                case "TURN_MISSED":
                    HandleTurnMissed(data);
                    break;
                case "TURN_PASS":
                    HandleTurnPass(data);
                    break;
                case "MOVE":
                    HandleMoveToken(data);
                    break;
                case "REJOIN_TABLE":
                    HandleRejoinTable(data);
                    break;
                case "WINNER_DECLARE":
                    HandleWinnerDeclare(data);
                    break;
                case "LEAVE_TABLE":
                    HandleLeaveTable(data);
                    break;
                case "ALERT":
                    HandleAlert(data);
                    break;
                case "HEART_BEAT":
                    SNL_3PLUS_MGP_GameManager.instance.heartBeatManager.OnReceiveHB(data);
                    break;
                case "SHOW_POPUP":
                    ShowServerPopup(data);
                    break;
                default:
                    break;
            }
        }

        public ServerPopUpData serverPopUp = new ServerPopUpData();

        private void ShowServerPopup(JSONObject data)
        {
            serverPopUp = JsonUtility.FromJson<ServerPopUpData>(data.ToString());
            Debug.Log("  ||  ShowServerPopup   ||  " + data);
            SNL_3PLUS_MGP_GameManager.instance.ShowServerPopUp(serverPopUp.title, serverPopUp.message);
        }

        private void HandleSignUp(JSONObject data)
        {
            var signUpModel = new SignUpModel();
            signUpModel = JsonConvert.DeserializeObject<SignUpModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.userDetail.userId = signUpModel.userId;
            SNL_3PLUS_MGP_GameManager.instance.userDetail.mobileNumber = signUpModel.mobileNumber;
            SNL_3PLUS_MGP_GameManager.instance.userDetail.userName = signUpModel.userName;
            SNL_3PLUS_MGP_GameManager.instance.loadingController.CloseLoadingScreen();
            if (!SNL_3PLUS_MGP_GameManager.instance.rejoinController.isDisconnected)
            {
                SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.WAITFORPLAYER);
            }
            else
            {
                if (SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus == GameProgressStatus.FTUE)
                {
                    SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.WAITFORPLAYER);
                }
            }
        }

        private void HandleCreateTable(JSONObject data)
        {
            var createTableModel = new CreateTableModel();
            createTableModel = JsonConvert.DeserializeObject<CreateTableModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.WAITFORPLAYER);
            SNL_3PLUS_MGP_GameManager.instance.gameBoardNumber = createTableModel.gameBoardNo;
            SNL_3PLUS_MGP_GameManager.instance.SetBoard(createTableModel.gameBoardNo);
            SNL_3PLUS_MGP_GameManager.instance.isMode = createTableModel.mode;
            if (SNL_3PLUS_MGP_GameManager.instance.isMode)
            {
                Debug.Log("Enter==========================================================");
                SNL_3PLUS_MGP_GameManager.instance.one.text = "Next Move";
                SNL_3PLUS_MGP_GameManager.instance.two.text = "Next Move";
                SNL_3PLUS_MGP_GameManager.instance.snl23.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl2.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl43.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl4.SetActive(false);
            }
            else
            {
                Debug.Log("Exit==============================================================");

                SNL_3PLUS_MGP_GameManager.instance.snl23.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl2.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl43.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl4.SetActive(true);
            }

            foreach (var t in createTableModel.playersArray)
            {
                if (!t.userId.Equals(SNL_3PLUS_MGP_GameManager.instance.userDetail.userId)) continue;
                SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex = t.seatIndex;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("ThisPlayerSeatIndex -----> " + SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex);
                }

                break;
            }

            SNL_3PLUS_MGP_GameManager.instance.movesLeftText.text = "";
            SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken = 0;
            SNL_3PLUS_MGP_GameManager.instance.DestroyAllTokensOnBoard();
            SNL_3PLUS_MGP_GameManager.instance.userDetail.tableId = createTableModel.tableId;
            SNL_3PLUS_MGP_GameManager.instance.ShowTableId(createTableModel.tableId);
            SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount = createTableModel.maxPlayers;
            SNL_3PLUS_MGP_GameManager.instance.totalMoves = createTableModel.totalMoves;
            SNL_3PLUS_MGP_GameManager.instance.ResetTurn();
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerProfiles();
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerSeatIndex();
            SNL_3PLUS_MGP_GameManager.instance.baseTurnTimerValue = createTableModel.baseTurnTimerValue;
            SNL_3PLUS_MGP_GameManager.instance.extraTurnTimerValue = createTableModel.extraTurnTimerValue;
            foreach (var t in createTableModel.playersArray)
            {
                var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(t.seatIndex);
                player.HidePlayerLeftSign();
                player.SetPlayerInfo(t.userName, t.userProfile);
                player.EnablePlayer();
                player.GenerateTokens(SNL_3PLUS_MGP_GameManager.instance.tokenPrefab, t.pawnsArray.Count);
                player.UpdateScore(t.score, true);
                player.UpdateLives(t.timeOut);
            }
        }

        private void HandleJoinTable(JSONObject data)
        {
            var joinTableModel = new JoinTableModel();
            joinTableModel = JsonConvert.DeserializeObject<JoinTableModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.gameBoardNumber = joinTableModel.gameBoardNo;
            SNL_3PLUS_MGP_GameManager.instance.SetBoard(joinTableModel.gameBoardNo);
            SNL_3PLUS_MGP_GameManager.instance.isMode = joinTableModel.mode;
            if (SNL_3PLUS_MGP_GameManager.instance.isMode)
            {
                Debug.Log("Enter==========================================================");
                SNL_3PLUS_MGP_GameManager.instance.one.text = "Next Move";
                SNL_3PLUS_MGP_GameManager.instance.two.text = "Next Move";
                SNL_3PLUS_MGP_GameManager.instance.snl23.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl2.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl43.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl4.SetActive(false);
            }
            else
            {
                Debug.Log("Exit==============================================================");

                SNL_3PLUS_MGP_GameManager.instance.snl23.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl2.SetActive(true);
                SNL_3PLUS_MGP_GameManager.instance.snl43.SetActive(false);
                SNL_3PLUS_MGP_GameManager.instance.snl4.SetActive(true);
            }
            foreach (var t in joinTableModel.playersArray)
            {
                if (!t.userId.Equals(SNL_3PLUS_MGP_GameManager.instance.userDetail.userId)) continue;
                SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex = t.seatIndex;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("ThisPlayerSeatIndex -----> " + SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex);
                }

                break;
            }

            SNL_3PLUS_MGP_GameManager.instance.movesLeftText.text = "";
            SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken = 0;
            SNL_3PLUS_MGP_GameManager.instance.DestroyAllTokensOnBoard();
            SNL_3PLUS_MGP_GameManager.instance.userDetail.tableId = joinTableModel.tableId;
            SNL_3PLUS_MGP_GameManager.instance.ShowTableId(joinTableModel.tableId);
            SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount = joinTableModel.maxPlayers;
            SNL_3PLUS_MGP_GameManager.instance.totalMoves = joinTableModel.totalMoves;
            SNL_3PLUS_MGP_GameManager.instance.ResetTurn();
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerProfiles();
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerSeatIndex();
            SNL_3PLUS_MGP_GameManager.instance.baseTurnTimerValue = joinTableModel.baseTurnTimerValue;
            SNL_3PLUS_MGP_GameManager.instance.extraTurnTimerValue = joinTableModel.extraTurnTimerValue;
            foreach (var t in joinTableModel.playersArray)
            {
                var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(t.seatIndex);
                player.HidePlayerLeftSign();
                player.SetPlayerInfo(t.userName, t.userProfile);
                player.EnablePlayer();
                player.GenerateTokens(SNL_3PLUS_MGP_GameManager.instance.tokenPrefab, t.pawnsArray.Count);
                player.UpdateScore(t.score, true);
                player.UpdateLives(t.timeOut);
            }
        }

        private void HandleNewUser(JSONObject data)
        {
            var newUserModel = new NewUserModel();
            newUserModel = JsonConvert.DeserializeObject<NewUserModel>(data.ToString());
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(newUserModel.seatIndex);

            player.HidePlayerLeftSign();
            player.SetPlayerInfo(newUserModel.userName, newUserModel.userProfile);
            player.EnablePlayer();
            player.GenerateTokens(SNL_3PLUS_MGP_GameManager.instance.tokenPrefab, newUserModel.pawnsArray.Count);
            player.UpdateScore(newUserModel.score, true);
            player.UpdateLives(newUserModel.timeOut);
        }

        private void HandleTableLock()
        {
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.exitYesButton.interactable = false;
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.leaveYesButton.interactable = false;
            SNL_3PLUS_MGP_GameManager.instance.alertPopup.alertPopup.SetActive(false);
            SNL_3PLUS_MGP_GameManager.instance.OnSettingCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.OnHelpCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.OnExitNoButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.backButton.interactable = false;
        }

        private void HandleGameStart(JSONObject data)
        {
            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.HideToastMessage(ToastMessage.WAITFORPLAYER);
            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.GAMESTARTCOUNTDOWN);
            var timerValue = int.Parse(data.GetField("timer").ToString());
            gameStartTimerEnd = Time.realtimeSinceStartup + timerValue;
            StartCoroutine(StartCountDown(timerValue));
            SNL_3PLUS_MGP_GameManager.instance.SetSettingButtonsInteractable();
            SNL_3PLUS_MGP_GameManager.instance.PlayBackgroundSound();
        }

        private IEnumerator StartCountDown(int timerValue)
        {
            var remainTime = timerValue;
            while (gameStartTimerEnd - Time.realtimeSinceStartup > 0)
            {
                SNL_3PLUS_MGP_GameManager.instance.gameStartTimerText.text = remainTime.ToString();
                yield return new WaitForSecondsRealtime(1);
                remainTime = Mathf.CeilToInt(gameStartTimerEnd - Time.realtimeSinceStartup);
            }

            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.HideToastMessage(ToastMessage.GAMESTARTCOUNTDOWN);
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.leaveYesButton.interactable = true;
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.exitYesButton.interactable = true;
        }

        private void HandleTurnInfo(JSONObject data)
        {
            SNL_3PLUS_MGP_GameManager.instance.backButton.interactable = true;
            var turnInfoModel = new TurnInfoModel();
            turnInfoModel = JsonConvert.DeserializeObject<TurnInfoModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.baseTurnTimerValue = turnInfoModel.totalTime;
            SNL_3PLUS_MGP_GameManager.instance.ResetTurn();
            SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(turnInfoModel.currentTurn);
            player.ScaleUpProfile();
            player.PlayEffect();
            player.StartPlayerTurn(turnInfoModel.diseNumber, turnInfoModel.remainingTime, turnInfoModel.diseNumberArray);
            SNL_3PLUS_MGP_GameManager.instance.diceValue = turnInfoModel.diseNumber;
            SNL_3PLUS_MGP_GameManager.instance.currentTurnSeatIndex = turnInfoModel.currentTurn;
            SNL_3PLUS_MGP_GameManager.instance.isExtraTurn = turnInfoModel.isExtraTurn;
            if (SNL_3PLUS_MGP_GameManager.instance.isExtraTurn)
            {
                SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.EXTRAMOVE);
            }

            if (turnInfoModel.currentTurn.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
            {
                player.AnimateDiceValueText();
                var movesLeft = SNL_3PLUS_MGP_GameManager.instance.totalMoves -
                                SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken;
                if (movesLeft.Equals(1) && !turnInfoModel.isExtraTurn)
                {
                    SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowToastMessage(ToastMessage.LASTMOVE);
                }

                SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.turnPass);
                SNL_3PLUS_MGP_GameManager.instance.gamePlayStatus = GamePlayStatus.PlayerMovement;
                player.ActiveTokenMovement(turnInfoModel.diseNumber);
                SNL_3PLUS_MGP_GameManager.instance.AnimateTokens(true);
                SNL_3PLUS_MGP_GameManager.instance.SetTokenLayers();
            }
        }

        private void HandleTurnMissed(JSONObject data)
        {
            var turnMissedModel = new TurnMissedModel();
            turnMissedModel = JsonConvert.DeserializeObject<TurnMissedModel>(data.ToString());
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(turnMissedModel.seatIndex);
            player.UpdateLives(turnMissedModel.timeOut);
            if (turnMissedModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
            {
                if (!SNL_3PLUS_MGP_GameManager.instance.isExtraTurn)
                {
                    SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken++;
                    SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
                }
            }
        }

        private void HandleTurnPass(JSONObject data)
        {
            var turnPassModel = new TurnPassModel();
            turnPassModel = JsonConvert.DeserializeObject<TurnPassModel>(data.ToString());
            if (turnPassModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex) &&
                !SNL_3PLUS_MGP_GameManager.instance.isExtraTurn)
            {
                SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken++;
                SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
            }
        }

        private void HandleMoveToken(JSONObject data)
        {
            var moveTokenModel = new MoveTokenModel();
            moveTokenModel = JsonConvert.DeserializeObject<MoveTokenModel>(data.ToString());
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(moveTokenModel.seatIndex);
            var token = player.allTokensForThisPlayer[moveTokenModel.pawn - 1];
            if (SNL_3PLUS_MGP_GameManager.instance.currentTurnSeatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance
                    .thisPlayerSeatIndex) &&
                !SNL_3PLUS_MGP_GameManager.instance.isExtraTurn)
            {
                SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken++;
            }

            SNL_3PLUS_MGP_GameManager.instance.AnimateTokens(false);
            token.StartTokenMovementAnimation(moveTokenModel);
            foreach (var userScore in moveTokenModel.allUserScore)
            {
                if (userScore.seatIndex != -1)
                {
                    SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(userScore.seatIndex)
                        .UpdateScore(userScore.score, false);
                }
            }
        }
        private void HandleExtraTimer(JSONObject data)
        {
            var extraTimerInfoModel = new ExtraTimerInfoModel();
            extraTimerInfoModel = JsonConvert.DeserializeObject<ExtraTimerInfoModel>(data.ToString());
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(extraTimerInfoModel.currentTurn);
            SNL_3PLUS_MGP_GameManager.instance.diceValue = extraTimerInfoModel.diseNumber;
            SNL_3PLUS_MGP_GameManager.instance.baseTurnTimerValue = extraTimerInfoModel.totalTime;
            SNL_3PLUS_MGP_GameManager.instance.currentTurnSeatIndex = extraTimerInfoModel.currentTurn;
            SNL_3PLUS_MGP_GameManager.instance.ResetTurn();
            SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
            player.ScaleUpProfile();
            player.PlayEffect();
            player.SetExtraTimeImage(true);
            player.StartPlayerTurn(SNL_3PLUS_MGP_GameManager.instance.diceValue, extraTimerInfoModel.remainingTime, extraTimerInfoModel.diseNumberArray);
            if (extraTimerInfoModel.currentTurn.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
            {
                player.AnimateDiceValueText();
                SNL_3PLUS_MGP_GameManager.instance.gamePlayStatus = GamePlayStatus.PlayerMovement;
                player.AnimateDiceValueText();
                player.ActiveTokenMovement(SNL_3PLUS_MGP_GameManager.instance.diceValue);
                SNL_3PLUS_MGP_GameManager.instance.AnimateTokens(true);
                SNL_3PLUS_MGP_GameManager.instance.SetTokenLayers();
            }
        }

        private void HandleWinnerDeclare(JSONObject data)
        {
            var winnerData = new WinnerDeclareModel();
            winnerData = JsonConvert.DeserializeObject<WinnerDeclareModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.eventManager.SendWinConfirmationEvent();
            SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus = GameProgressStatus.WINNINGSCREEN;
            SNL_3PLUS_MGP_GameManager.instance.StopBackgroundSound();
            SNL_3PLUS_MGP_GameManager.instance.ResetTurn();
            SNL_3PLUS_MGP_GameManager.instance.OnSettingCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.OnHelpCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.OnExitNoButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.movesLeftText.text = "0";
            for (var i = 0; i < winnerData.WinningArray.Count; i++)
            {
                if (winnerData.WinningArray[i].seatIndex
                    .Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
                {
                    SNL_3PLUS_MGP_GameManager.instance.ShowResultAnimation(winnerData.WinningArray[i].isWinner,
                        winnerData.isDraw,
                        () =>
                        {
                            SNL_3PLUS_MGP_GameManager.instance.DestroyAllTokensOnBoard();
                            StartCoroutine(ShowResultScreen(winnerData));
                        });
                }
            }
        }

        private IEnumerator ShowResultScreen(WinnerDeclareModel data)
        {
            yield return new WaitForSeconds(0.2f);
            SNL_3PLUS_MGP_GameManager.instance.winnerScreen.SetActive(true);
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerWinProfiles();
            if (data.isDraw)
            {
                switch (SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount)
                {
                    case 2:
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[0].crown.SetActive(false);
                        break;
                    case 4:
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[0].crown.SetActive(false);
                        break;
                }
            }

            for (var i = 0; i < data.WinningArray.Count; i++)
            {
                switch (SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount)
                {
                    case 2:
                        var profilePic2 = SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[i]
                            .profilePic;
                        profilePic2.GetComponent<SNL_3PLUS_MGP_ProfileDownLoader>().GetDisplayImage(data.WinningArray[i].userProfile);

                        //SNL_3PLUS_MGP_ProfileDownLoader.intance.GetDisplayImage(data.WinningArray[i].userProfile,
                        //    (Sprite sprite) => {
                        //        if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                        //            Debug.Log(" Profile pic downloaded ");
                        //        }

                        //        profilePic2.sprite = sprite;
                        //    });
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[i].scoreText.text =
                            data.WinningArray[i].score.ToString();
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[i].userNameText.text =
                            data.WinningArray[i].userName;
                        if (data.WinningArray[i].isWinner)
                        {
                            SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[i].winAmountText.text =
                                data.winAmount;
                            if (data.WinningArray[i].seatIndex
                                .Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
                            {
                                if (!data.isDraw)
                                {
                                    SNL_3PLUS_MGP_GameManager.instance.winEffect.Play();
                                }
                                SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.win);
                            }
                        }
                        else
                        {
                            SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.twoPlayerProfiles[i].winAmountText.text =
                                data.lossAmount.ToString();
                            SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.loss);
                        }

                        break;
                    case 4:
                        var profilePic4 = SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[i]
                            .profilePic;
                        profilePic4.GetComponent<SNL_3PLUS_MGP_ProfileDownLoader>().GetDisplayImage(data.WinningArray[i].userProfile);
                        //SNL_3PLUS_MGP_ProfileDownLoader.intance.GetDisplayImage(data.WinningArray[i].userProfile,
                        //    (Sprite sprite) => { profilePic4.sprite = sprite; });
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[i].scoreText.text =
                            data.WinningArray[i].score.ToString();
                        SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[i].userNameText.text =
                            data.WinningArray[i].userName;
                        if (data.WinningArray[i].isWinner)
                        {
                            SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[i].winAmountText.text =
                                data.winAmount;
                            if (data.WinningArray[i].seatIndex
                                .Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
                            {
                                if (!data.isDraw)
                                {
                                    SNL_3PLUS_MGP_GameManager.instance.winEffect.Play();
                                }
                                SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.win);
                            }
                        }
                        else
                        {
                            SNL_3PLUS_MGP_GameManager.instance.winnerProfiles.fourPlayerProfiles[i].winAmountText.text =
                                data.lossAmount.ToString();
                            SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.loss);
                        }

                        break;
                }
            }
        }

        private void HandleLeaveTable(JSONObject data)
        {
            var leaveTableModel = new LeaveTableModel();
            leaveTableModel = JsonConvert.DeserializeObject<LeaveTableModel>(data.ToString());
            if (leaveTableModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
            {
                if (leaveTableModel.playerChooseToExit)
                {
                    //SceneManager.LoadScene(0);
                    if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
                        MGPSDK.MGPGameManager.instance.OnClickQuite();
                    else
                        Application.Quit();
                    return;
                }
            }

            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(leaveTableModel.seatIndex);
            player.ShowPlayerLeftSign();
            player.RemovePlayerData();
            SNL_3PLUS_MGP_GameManager.instance.RemovePlayerWhenDisconnect(player);
        }

        private void HandleRejoinTable(JSONObject data)
        {
            var rejoinData = new RejoinTableModel();
            rejoinData = JsonConvert.DeserializeObject<RejoinTableModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.PlayBackgroundSound();
            SNL_3PLUS_MGP_GameManager.instance.DestroyAllTokensOnBoard();
            foreach (var t in rejoinData.playersArray)
            {
                if (!t.userId.Equals(SNL_3PLUS_MGP_GameManager.instance.userDetail.userId)) continue;
                SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex = t.seatIndex;
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("ThisPlayerSeatIndex -----> " + SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex);
                }

                break;
            }

            SNL_3PLUS_MGP_GameManager.instance.loadingController.CloseLoadingScreen();
            SNL_3PLUS_MGP_GameManager.instance.userDetail.tableId = rejoinData.tableId;
            SNL_3PLUS_MGP_GameManager.instance.ShowTableId(rejoinData.tableId);
            SNL_3PLUS_MGP_GameManager.instance.gameBoardNumber = rejoinData.gameBoardNo;
            SNL_3PLUS_MGP_GameManager.instance.totalPlayerCount = rejoinData.maxPlayers;
            SNL_3PLUS_MGP_GameManager.instance.totalMoves = rejoinData.totalMoves;
            SNL_3PLUS_MGP_GameManager.instance.SetBoard(rejoinData.gameBoardNo);
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerProfiles();
            SNL_3PLUS_MGP_GameManager.instance.SetPlayerSeatIndex();
            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.HideToastMessage(ToastMessage.WAITFORPLAYER);
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.leaveYesButton.interactable = true;
            SNL_3PLUS_MGP_GameManager.instance.settingReferences.exitYesButton.interactable = true;
            SNL_3PLUS_MGP_GameManager.instance.rejoinController.isDisconnected = false;
            SNL_3PLUS_MGP_GameManager.instance.backButton.interactable = true;
            foreach (var player in rejoinData.playersArray)
            {
                var seatIndex = player.seatIndex;
                var playerController = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(seatIndex);
                playerController.SetPlayerInfo(player.userName, player.userProfile);
                playerController.EnablePlayer();
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("IsLeave --> " + player.isLeave + " SeatIndex --> " + player.seatIndex);
                }

                if (player.isLeave)
                {
                    playerController.UpdateLives(3);
                    playerController.UpdateScore(0, true);
                    playerController.ShowPlayerLeftSign();
                }
                else
                {
                    playerController.GenerateTokens(SNL_3PLUS_MGP_GameManager.instance.tokenPrefab,
                        player.pawnsArray.Count);
                    playerController.UpdateScore(player.score, true);
                    playerController.UpdateLives(player.timeOut);
                }

                if (player.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex))
                {
                    SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken =
                        SNL_3PLUS_MGP_GameManager.instance.totalMoves - player.moveLeft;
                    SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
                }
            }

            SNL_3PLUS_MGP_GameManager.instance.SetDataWhenReconnect(rejoinData.playersArray);
        }

        private void HandleAlert(JSONObject data)
        {
            SNL_3PLUS_MGP_GameManager.instance.eventManager.SendWinConfirmationEvent();
            var alert = new AlertModel();
            alert = JsonConvert.DeserializeObject<AlertModel>(data.ToString());
            SNL_3PLUS_MGP_GameManager.instance.gameProgressStatus = GameProgressStatus.KICKOUT;
            SNL_3PLUS_MGP_GameManager.instance.OnSettingCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.OnHelpCloseButtonClick();
            SNL_3PLUS_MGP_GameManager.instance.ShowAlertPopup(alert.Message);
        }
    }

    public class SignUpModel
    {
        public string userId;
        public string userName;
        public string userProfile;
        public int mobileNumber;
        public int chips;
        public int token;
        public string socketId;
        public string tableId;
    }

    public class CreateTableModel
    {
        public string tableId;
        public int bootValue;
        public int currentTurn;
        public float turnStartAt;
        public int maxPlayers;
        public int diceNumber;
        public int baseTurnTimerValue;
        public int extraTurnTimerValue;
        public int totalMoves;
        public int gameBoardNo;
        public List<Player> playersArray = new List<Player>();

        public class Player
        {
            public string userId;
            public string userName;
            public int seatIndex;
            public int timeOut;
            public int score;
            public int extraTimer;
            public int moveLeft;
            public bool isLeave;
            public string userProfile;
            public List<Pawns> pawnsArray = new List<Pawns>();

            public class Pawns
            {
                public int pawnNumber;
                public int pawnPosition;
            }
        }
        public bool mode;
    }

    public class JoinTableModel
    {
        public string tableId;
        public int bootValue;
        public int currentTurn;
        public float turnStartAt;
        public int maxPlayers;
        public int diceNumber;
        public int baseTurnTimerValue;
        public int extraTurnTimerValue;
        public int totalMoves;
        public int gameBoardNo;
        public List<Player> playersArray = new List<Player>();

        public class Player
        {
            public string userId;
            public string userName;
            public int seatIndex;
            public int timeOut;
            public int score;
            public int extraTimer;
            public int moveLeft;
            public bool isLeave;
            public string userProfile;
            public List<Pawns> pawnsArray = new List<Pawns>();

            public class Pawns
            {
                public int pawnNumber;
                public int pawnPosition;
            }
        }
        public bool mode;
    }

    public class NewUserModel
    {
        public string userId;
        public string userName;
        public int seatIndex;
        public int timeOut;
        public int score;
        public int extraTimer;
        public int moveLeft;
        public bool isLeave;
        public string userProfile;
        public List<Pawns> pawnsArray = new List<Pawns>();

        public class Pawns
        {
            public int pawnNumber;
            public int pawnPosition;
        }
    }

    public class TableLockModel
    {
        public string tableId;
    }

    public class TurnInfoModel
    {
        public int currentTurn;
        public int totalTime;
        public float remainingTime;
        public int diseNumber;
        public bool isExtraTurn;
        public List<int> diseNumberArray;
    }

    public class MoveTokenModel
    {
        public int seatIndex;
        public int pawn;
        public int userMoveLeft;
        public int score;
        public int pawnPosition;
        public bool isLadder;
        public bool isSnake;
        public bool isKill;
        public int killSeatIndex;
        public int killPawnNumber;
        public int killScore;
        public int bonusPoint;
        public int scoreDifference;
        public List<Scores> allUserScore = new List<Scores>();

        public class Scores
        {
            public int score;
            public int seatIndex;
        }
    }

    public class ExtraTimerInfoModel
    {
        public int currentTurn;
        public int totalTime;
        public int diseNumber;
        public float remainingTime;
        public List<int> diseNumberArray;
    }

    public class TurnMissedModel
    {
        public int seatIndex;
        public int userMoveLeft;
        public int score;
        public int timeOut;
        public List<UserScores> allUserScore = new List<UserScores>();

        public class UserScores
        {
            public int score;
            public int seatIndex;
        }
    }

    public class TurnPassModel
    {
        public int seatIndex;
    }

    public class AlertModel
    {
        public int seatIndex;
        public string Message;
    }

    public class WinnerDeclareModel
    {
        public class Player
        {
            public string userId;
            public string userName;
            public int seatIndex;
            public int score;
            public bool isWinner;
            public bool isLeave;
            public string userProfile;
        }

        public string winAmount;
        public int lossAmount;
        public bool isDraw;
        public List<Player> WinningArray = new List<Player>();
    }

    public class LeaveTableModel
    {
        public int seatIndex;
        public string userId;
        public bool playerChooseToExit;
    }

    public class RejoinTableModel
    {
        public string tableId;
        public int bootValue;
        public int currentTurn;
        public float turnStartAt;
        public int maxPlayers;
        public int diseNumber;
        public int baseTurnTimerValue;
        public int extraTurnTimerValue;
        public int totalMoves;
        public int gameBoardNo;
        public List<Player> playersArray = new List<Player>();

        public class Player
        {
            public string userId;
            public string userName;
            public string userProfile;
            public int seatIndex;
            public int timeOut;
            public int score;
            public int extraTimer;
            public int moveLeft;
            public bool isLeave;
            public List<Pawns> pawnsArray = new List<Pawns>();

            public class Pawns
            {
                public int pawnNumber;
                public int pawnPosition;
            }
        }
    }

    [System.Serializable]
    public class ServerPopUpData
    {
        public bool isPopup;
        public string popupType;
        public string title;
        public string message;
        public int buttonCounts;
        public List<string> button_text;
        public List<string> button_color;
        public List<string> button_methods;
    }

    [System.Serializable]
    public class ServerPopUp
    {
        public string en;
        public ServerPopUpData Data;
    }
}