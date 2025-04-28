using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP
{
    public enum TokenColor
    {
        Yellow,
        Blue,
        Green,
        Red
    }

    public enum GamePlayStatus
    {
        None,
        PlayerMovement,
        PassTurn
    }

    public enum GameProgressStatus
    {
        FTUE,
        WAITINGFORPLAYER,
        GAMEPLAY,
        WINNINGSCREEN,
        KICKOUT
    }

    public enum TokenRotateSide
    {
        None,
        Left,
        Right
    }

    public class SNL_3PLUS_MGP_GameManager : MonoBehaviour
    {
        [SerializeField] internal bool gameRunOnSDK;

        public bool isGameLogsEnable;

        public bool isMode;
        public Text one, two;
        public GameObject snl2, snl23;
        public GameObject snl4, snl43;

        public static SNL_3PLUS_MGP_GameManager instance;

        public GameProgressStatus gameProgressStatus;

        public SNL_3PLUS_MGP_SocketHandler socketHandler;
        public SNL_3PLUS_MGP_SocketEventReceiver socketEventReceiver;
        public SNL_3PLUS_MGP_EventManager eventManager;
        public SNL_3PLUS_MGP_HeartBeatManager heartBeatManager;
        public SNL_3PLUS_MGP_ToastMessageManager toastMessageManager;
        [HideInInspector] public MyInternetReachability myInternetReachability;
        public SNL_3PLUS_MGP_RejoinController rejoinController;
        public SNL_3PLUS_MGP_LoadingController loadingController;
        public SNL_3PLUS_MGP_FTUEManager ftueManager;

        public UserDetail userDetail;

        public GameObject tokenPrefab;
        public GameObject noInternetPanel;
        public Text gameStartTimerText;
        public GameObject gamePlayScreen;
        public GameObject twoPlayerProfiles;
        public GameObject fourPlayerProfiles;
        public Text tabeIdText;

        public Sprite extraTimerImage;

        [Serializable]
        public class Players
        {
            [Serializable]
            public class Player
            {
                public List<SNL_3PLUS_MGP_PlayerController> players = new List<SNL_3PLUS_MGP_PlayerController>();
            }

            public Player twoPlayer;
            public Player threePlayer;
            public Player fourPlayer;
        }

        public Players players;
        public List<Transform> wayPoints = new List<Transform>();
        public List<Sprite> tokenImages = new List<Sprite>();
        public List<SNL_3PLUS_MGP_Token> allTokensOnBoard = new List<SNL_3PLUS_MGP_Token>();

        internal int totalPlayerCount;
        internal float baseTurnTimerValue;
        internal int extraTurnTimerValue;
        internal int thisPlayerSeatIndex;
        internal int diceValue;
        internal int currentTurnSeatIndex;
        internal int totalMoves;
        internal int totalTurnTaken;
        internal int gameBoardNumber;
        public int networkPingTime;
        public int maxPingCounter;

        internal bool isExtraTurn;

        public List<int> pongTimer = new List<int>();

        public Text movesLeftText;

        public GamePlayStatus gamePlayStatus;

        public static event Action<string, SNL_3PLUS_MGP_Token> OnAddToken;
        public static event Action<SNL_3PLUS_MGP_Token> OnRemoveToken;
        public static event Action<bool> OnTokenAnimation;
        public static event Action OnResetTurn;
        public static event Action ResetBoxPropertyOnRejoin;
        public static event Action SetTokenLayer;

        public GameObject winnerScreen;



        [Serializable]
        public class WinnerProfiles
        {
            [Serializable]
            public class Profile
            {
                public Image profilePic;
                public Text userNameText;
                public Text winAmountText;
                public Text scoreText;
                public GameObject crown;
            }

            public GameObject twoPlayerProfilesGameObject;
            public GameObject fourPlayerProfilesGameObject;
            public List<Profile> twoPlayerProfiles = new List<Profile>();
            public List<Profile> fourPlayerProfiles = new List<Profile>();
        }

        public WinnerProfiles winnerProfiles;

        public ParticleSystem killEffect;
        public ParticleSystem winEffect;

        public Image twoPlayerHeader, twoPlayerBG, fourPlayerHeader, fourPlayerBG;
        public Sprite winHeader, lossHeader, drawHeader, winBG, lossBG;

        [Serializable]
        public class WinAnimation
        {
            public GameObject result;
            public GameObject win;
            public GameObject lose;
            public GameObject tied;
            public GameObject bg;
        }

        public WinAnimation winAnimation;

        [Serializable]
        public class SettingReferences
        {
            [Serializable]
            public class ButtonReference
            {
                public Image switchImage;
                public Image icon;
                public Sprite onIcon;
                public Sprite offIcon;
                public Transform ball;
                public Text statusText;
                public Transform onPosition;
                public Transform offPosition;
                public GameObject onText;
                public GameObject offText;
                public Button button;
            }

            public GameObject settingScreen;
            public Transform settingPanel;
            public ButtonReference soundButtonReference;
            public ButtonReference vibrateButtonReference;
            public ButtonReference musicButtonReference;
            public Sprite switchOnImage;
            public Sprite switchOffImage;
            public GameObject help;
            public Transform helpBGPanel;
            public Button leaveYesButton;
            public Button exitYesButton;
        }

        public SettingReferences settingReferences;
        public Sprite liveRed;
        public Sprite liveGrey;

        [Serializable]
        public class TrailColors
        {
            public Color blueColor;
            public Color yellowColor;
            public Color greenColor;
            public Color redColor;
        }

        public TrailColors trailColors;

        public GameObject exit;
        public Transform exitPanel;

        [Serializable]
        public class Sounds
        {
            public AudioClip tokenEnterSafePlace;
            public AudioClip tokenEnterHome;
            public AudioClip tokenMovement;
            public AudioClip tokenKill;
            public AudioClip turnPass;
            public AudioClip ladderClimb;
            public AudioClip snakeBite;
            public AudioClip win;
            public AudioClip loss;

            public AudioSource timeOutSource;
            public AudioSource gameAudioSource;
            public AudioSource backgroundSource;
        }

        public Sounds sounds;

        public GameObject playerCountSelectionScreen;
        public List<GameObject> boards = new List<GameObject>();

        public Image killEffectBackground;
        public Color greenColor;
        public Color redColor;

        public GameObject ftueScreen;

        [Serializable]
        public class Alert
        {
            public GameObject alertPopup;
            public Transform alertPopupBG;
            public Text alertMessageText;
        }

        public Alert alertPopup;

        [Serializable]
        public class NetWorkImages
        {
            public List<GameObject> netWorkImages;
            public GameObject closeImage;
        }

        public NetWorkImages netWorkImages;

        public List<Button> buttons = new List<Button>();
        public Sprite defaultProfilePic;
        public List<Sprite> profilePics = new List<Sprite>();

        public Button backButton;
        private void OnEnable()
        {
            instance = this;
        }
        private void Awake()
        {
        }

        private void Start()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            SetSoundButton();
            SetVibrateButton();
            SetMusicButton();

            Screen.sleepTimeout = SleepTimeout.NeverSleep;
        }

        internal void SetBoard(int boardNumber)
        {
            for (var i = 0; i < boards.Count; i++)
            {
                boards[i].SetActive(i == boardNumber);
            }
        }

        internal void ShowTableId(string tableId)
        {
            tabeIdText.text = "#" + tableId.Substring(tableId.Length - 8);
        }

        internal void SetPlayerSeatIndex()
        {
            switch (totalPlayerCount)
            {
                case 2:
                    for (var i = 0; i < players.twoPlayer.players.Count; i++)
                    {
                        var val = thisPlayerSeatIndex + i;
                        if (val > players.twoPlayer.players.Count)
                        {
                            val = 1;
                        }

                        players.twoPlayer.players[i].seatIndex = val;
                    }

                    break;
                case 3:
                    for (var i = 0; i < players.threePlayer.players.Count; i++)
                    {
                        var val = thisPlayerSeatIndex + i;
                        if (val > players.threePlayer.players.Count)
                        {
                            val = 1;
                        }

                        players.threePlayer.players[i].seatIndex = val;
                    }

                    break;
                case 4:
                    for (var i = 0; i < players.fourPlayer.players.Count; i++)
                    {
                        var val = thisPlayerSeatIndex + i;
                        if (val > totalPlayerCount)
                        {
                            val = val - totalPlayerCount;
                        }

                        players.fourPlayer.players[i].seatIndex = val;
                    }

                    break;
            }
        }

        internal SNL_3PLUS_MGP_PlayerController ReturnPlayerFromSeatIndex(int seatIndex)
        {
            return totalPlayerCount switch
            {
                2 => players.twoPlayer.players.Single(player => player.seatIndex == seatIndex),
                3 => players.threePlayer.players.Single(player => player.seatIndex == seatIndex),
                4 => players.fourPlayer.players.Single(player => player.seatIndex == seatIndex),
                _ => null
            };
        }

        internal void SetPlayerProfiles()
        {
            switch (totalPlayerCount)
            {
                case 2:
                    twoPlayerProfiles.SetActive(true);
                    fourPlayerProfiles.SetActive(false);
                    break;
                case 4:
                    twoPlayerProfiles.SetActive(false);
                    fourPlayerProfiles.SetActive(true);
                    break;
            }
        }

        internal void SetPlayerWinProfiles()
        {
            switch (totalPlayerCount)
            {
                case 2:
                    winnerProfiles.twoPlayerProfilesGameObject.SetActive(true);
                    winnerProfiles.fourPlayerProfilesGameObject.SetActive(false);
                    break;
                case 4:
                    winnerProfiles.twoPlayerProfilesGameObject.SetActive(false);
                    winnerProfiles.fourPlayerProfilesGameObject.SetActive(true);
                    break;
            }
        }

        internal void AddTokenToBox(string boxName, SNL_3PLUS_MGP_Token token)
        {
            OnAddToken?.Invoke(boxName, token);
        }

        internal void RemoveTokenFromBox(SNL_3PLUS_MGP_Token token)
        {
            OnRemoveToken?.Invoke(token);
        }

        internal void AnimateTokens(bool isAnimate)
        {
            OnTokenAnimation?.Invoke(isAnimate);
        }

        internal void SetTokenLayers()
        {
            SetTokenLayer?.Invoke();
        }

        internal void ResetTurn()
        {
            gamePlayStatus = GamePlayStatus.None;
            OnResetTurn?.Invoke();
            AnimateTokens(false);
            StopTimeOutCountSound();
        }

        internal void SendTokenToHome(int killedPlayerSeatIndex, int killedTokenId)
        {
            var player = ReturnPlayerFromSeatIndex(killedPlayerSeatIndex);
            var token = player.allTokensForThisPlayer[killedTokenId - 1];
            token.TokenGoHomeAnimation(() => { player.SetTokenAtHomePosition(killedTokenId - 1); });
        }

        internal void MoveTokenToPosition(int tokenEndPosition, int tokenIndex, int turnSeatIndex,
            UnityAction callBack)
        {
            var player = ReturnPlayerFromSeatIndex(turnSeatIndex);
            var token = player.allTokensForThisPlayer[tokenIndex - 1];
            token.MoveToken(tokenEndPosition, turnSeatIndex, callBack);
        }

        internal void SwallowToken(int tokenEndPosition, int tokenIndex, int turnSeatIndex, UnityAction callBack)
        {
            var player = ReturnPlayerFromSeatIndex(turnSeatIndex);
            var token = player.allTokensForThisPlayer[tokenIndex - 1];
            token.SwallowToken(tokenEndPosition, callBack);
        }

        internal void DisplayMovesLeft()
        {
            var movesLeft = totalMoves - totalTurnTaken;
            movesLeftText.text = movesLeft.ToString();
        }

        internal void DestroyAllTokensOnBoard()
        {
            if (allTokensOnBoard.Count <= 0) return;
            foreach (var token in allTokensOnBoard)
            {
                Destroy(token.gameObject);
            }

            allTokensOnBoard.Clear();
        }

        internal bool IsInternetAvailable()
        {
            // return myInternetReachability.IsInternetAvailable;
            return Application.internetReachability != NetworkReachability.NotReachable;
        }

        internal void ShowNoInternetPanel()
        {
            if (noInternetPanel.activeInHierarchy) return;
            noInternetPanel.SetActive(true);
        }

        internal void HideNoInternetPanel()
        {
            noInternetPanel.SetActive(false);
        }

        internal void SetDataWhenReconnect(List<RejoinTableModel.Player> playersData)
        {
            ResetBoxPropertyOnRejoin?.Invoke();
            SetTokenPositionsOnReconnect(playersData);
        }

        private void SetTokenPositionsOnReconnect(List<RejoinTableModel.Player> playersData)
        {
            foreach (var player in playersData)
            {
                if (!player.isLeave)
                {
                    var pawnsData = player.pawnsArray;
                    var playerController = ReturnPlayerFromSeatIndex(player.seatIndex);
                    for (var i = 0; i < pawnsData.Count; i++)
                    {
                        var token = ReturnTokenFromPlayer(playerController, pawnsData[i].pawnNumber);
                        token.SetTokenPositionOnReconnect(pawnsData[i].pawnPosition);
                    }
                }
            }
        }

        private SNL_3PLUS_MGP_Token ReturnTokenFromPlayer(SNL_3PLUS_MGP_PlayerController player, int tokenIndex)
        {
            return player.allTokensForThisPlayer.Where(token => token.tokenIndex == tokenIndex).Single();
        }

        internal void RemovePlayerWhenDisconnect(SNL_3PLUS_MGP_PlayerController player)
        {
            foreach (var token in player.allTokensForThisPlayer)
            {
                RemoveTokenFromBox(token);
                allTokensOnBoard.Remove(token);
                Destroy(token.gameObject);
            }

            player.allTokensForThisPlayer.Clear();
            if (isGameLogsEnable)
            {
                Debug.Log("Token Count For SeatIndex --> " + player.seatIndex + " Are " +
                          player.allTokensForThisPlayer.Count);
            }
        }

        internal void ShowResultAnimation(bool isWin, bool isDraw, UnityAction callBack)
        {
            winAnimation.result.SetActive(true);
            winAnimation.bg.transform.localScale = Vector3.zero;
            if (isWin)
            {
                if (isDraw)
                {
                    winAnimation.win.SetActive(false);
                    winAnimation.lose.SetActive(false);
                    winAnimation.tied.SetActive(true);

                    twoPlayerBG.sprite = lossBG;
                    fourPlayerBG.sprite = lossBG;
                    twoPlayerHeader.sprite = drawHeader;
                    fourPlayerHeader.sprite = drawHeader;
                }
                else
                {
                    winAnimation.win.SetActive(true);
                    winAnimation.lose.SetActive(false);
                    winAnimation.tied.SetActive(false);

                    twoPlayerBG.sprite = winBG;
                    fourPlayerBG.sprite = winBG;
                    twoPlayerHeader.sprite = winHeader;
                    fourPlayerHeader.sprite = winHeader;
                }
            }
            else
            {
                winAnimation.win.SetActive(false);
                winAnimation.lose.SetActive(true);
                winAnimation.tied.SetActive(false);

                twoPlayerBG.sprite = lossBG;
                fourPlayerBG.sprite = lossBG;
                twoPlayerHeader.sprite = lossHeader;
                fourPlayerHeader.sprite = lossHeader;
            }

            winAnimation.bg.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                winAnimation.bg.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.Linear).SetDelay(2).OnComplete(
                    () =>
                    {
                        winAnimation.result.SetActive(false);
                        callBack.Invoke();
                    });
            });
        }

        public void OnAlertOKButtonClick()
        {
            //SceneManager.LoadScene(0);
            if (SNL_3PLUS_MGP_GameManager.instance.gameRunOnSDK)
                MGPSDK.MGPGameManager.instance.OnClickQuite();
            else
                Application.Quit();
        }

        internal void ShowAlertPopup(string message)
        {
            alertPopup.alertPopup.SetActive(true);
            alertPopup.alertPopupBG.localScale = Vector3.zero;
            alertPopup.alertMessageText.text = message;
            alertPopup.alertPopupBG.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        #region Settings

        public void OnSettingButtonClick()
        {
            settingReferences.settingScreen.SetActive(true);
            settingReferences.settingPanel.localScale = Vector3.zero;
            settingReferences.settingPanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void OnSettingCloseButtonClick()
        {
            settingReferences.settingPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                settingReferences.settingScreen.SetActive(false);
            });
        }

        private void SetSoundButton()
        {
            if (PlayerPrefs.GetInt("Sound").Equals(0))
            {
                //   settingReferences.soundButtonReference.ball.position =
                //      settingReferences.soundButtonReference.onPosition.position;
                settingReferences.soundButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                // settingReferences.soundButtonReference.statusText.text = "Sound On";
                //settingReferences.soundButtonReference.icon.sprite = settingReferences.soundButtonReference.onIcon;
                //settingReferences.soundButtonReference.onText.SetActive(true);
                //settingReferences.soundButtonReference.offText.SetActive(false);
            }
            else
            {
                //    settingReferences.soundButtonReference.ball.position =
                //       settingReferences.soundButtonReference.offPosition.position;
                settingReferences.soundButtonReference.switchImage.sprite = settingReferences.switchOffImage;
                //    settingReferences.soundButtonReference.statusText.text = "Sound Off";
                //    settingReferences.soundButtonReference.icon.sprite = settingReferences.soundButtonReference.offIcon;
                //    settingReferences.soundButtonReference.onText.SetActive(false);
                //    settingReferences.soundButtonReference.offText.SetActive(true);
            }
        }

        public void OnSoundButtonClick()
        {
            if (PlayerPrefs.GetInt("Sound").Equals(0))
            {
                PlayerPrefs.SetInt("Sound", 1);
                //settingReferences.soundButtonReference.ball
                //    .DOMove(settingReferences.soundButtonReference.offPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.soundButtonReference.switchImage.sprite =
                //                settingReferences.switchOffImage;
                //            settingReferences.soundButtonReference.statusText.text = "Sound Off";
                //            settingReferences.soundButtonReference.icon.sprite =
                //                settingReferences.soundButtonReference.offIcon;
                //            settingReferences.soundButtonReference.onText.SetActive(false);
                //            settingReferences.soundButtonReference.offText.SetActive(true);
                //        });
                settingReferences.soundButtonReference.switchImage.sprite =
                               settingReferences.switchOffImage;
            }
            else
            {
                PlayerPrefs.SetInt("Sound", 0);
                //settingReferences.soundButtonReference.ball
                //    .DOMove(settingReferences.soundButtonReference.onPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.soundButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                //            settingReferences.soundButtonReference.statusText.text = "Sound On";
                //            settingReferences.soundButtonReference.icon.sprite =
                //                settingReferences.soundButtonReference.onIcon;
                //            settingReferences.soundButtonReference.onText.SetActive(true);
                //            settingReferences.soundButtonReference.offText.SetActive(false);
                //        });
                settingReferences.soundButtonReference.switchImage.sprite = settingReferences.switchOnImage;
            }
        }

        private void SetVibrateButton()
        {
            if (PlayerPrefs.GetInt("Vibrate").Equals(0))
            {
                //  settingReferences.vibrateButtonReference.ball.transform.position =
                //      settingReferences.vibrateButtonReference.onPosition.position;
                settingReferences.vibrateButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                //  settingReferences.vibrateButtonReference.statusText.text = "Vibrate On";
                //  settingReferences.vibrateButtonReference.icon.sprite = settingReferences.vibrateButtonReference.onIcon;
                //  settingReferences.vibrateButtonReference.onText.SetActive(true);
                //  settingReferences.vibrateButtonReference.offText.SetActive(false);
            }
            else
            {
                //    settingReferences.vibrateButtonReference.ball.transform.position =
                //       settingReferences.vibrateButtonReference.offPosition.position;
                settingReferences.vibrateButtonReference.switchImage.sprite = settingReferences.switchOffImage;
                //   settingReferences.vibrateButtonReference.statusText.text = "Vibrate Off";
                //    settingReferences.vibrateButtonReference.icon.sprite = settingReferences.vibrateButtonReference.offIcon;
                //    settingReferences.vibrateButtonReference.onText.SetActive(false);
                //     settingReferences.vibrateButtonReference.offText.SetActive(true);
            }
        }

        public void OnVibrateButtonClick()
        {
            if (PlayerPrefs.GetInt("Vibrate").Equals(0))
            {
                PlayerPrefs.SetInt("Vibrate", 1);
                //settingReferences.vibrateButtonReference.ball
                //    .DOMove(settingReferences.vibrateButtonReference.offPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.vibrateButtonReference.switchImage.sprite =
                //                settingReferences.switchOffImage;
                //            settingReferences.vibrateButtonReference.statusText.text = "Vibrate Off";
                //            settingReferences.vibrateButtonReference.icon.sprite =
                //                settingReferences.vibrateButtonReference.offIcon;
                //            settingReferences.vibrateButtonReference.onText.SetActive(false);
                //            settingReferences.vibrateButtonReference.offText.SetActive(true);
                //        });
                settingReferences.vibrateButtonReference.switchImage.sprite =
                              settingReferences.switchOffImage;
            }
            else
            {
                PlayerPrefs.SetInt("Vibrate", 0);
                //settingReferences.vibrateButtonReference.ball
                //    .DOMove(settingReferences.vibrateButtonReference.onPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.vibrateButtonReference.switchImage.sprite =
                //                settingReferences.switchOnImage;
                //            settingReferences.vibrateButtonReference.statusText.text = "Vibrate On";
                //            settingReferences.vibrateButtonReference.icon.sprite =
                //                settingReferences.vibrateButtonReference.onIcon;
                //            settingReferences.vibrateButtonReference.onText.SetActive(true);
                //            settingReferences.vibrateButtonReference.offText.SetActive(false);
                //        });
                settingReferences.vibrateButtonReference.switchImage.sprite =
                               settingReferences.switchOnImage;

            }
        }

        private void SetMusicButton()
        {
            if (PlayerPrefs.GetInt("Music").Equals(0))
            {
                // settingReferences.musicButtonReference.ball.transform.position =
                //     settingReferences.musicButtonReference.onPosition.position;
                settingReferences.musicButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                //settingReferences.musicButtonReference.statusText.text = "Music On";
                //settingReferences.musicButtonReference.icon.sprite = settingReferences.musicButtonReference.onIcon;
                //settingReferences.musicButtonReference.onText.SetActive(true);
                //settingReferences.musicButtonReference.offText.SetActive(false);
            }
            else
            {
                //   settingReferences.musicButtonReference.ball.transform.position =
                //       settingReferences.musicButtonReference.offPosition.position;
                settingReferences.musicButtonReference.switchImage.sprite = settingReferences.switchOffImage;
                //settingReferences.musicButtonReference.statusText.text = "Music Off";
                //settingReferences.musicButtonReference.icon.sprite = settingReferences.musicButtonReference.offIcon;
                //settingReferences.musicButtonReference.onText.SetActive(false);
                //settingReferences.musicButtonReference.offText.SetActive(true);
            }
        }

        public void OnMusicButtonClick()
        {
            if (PlayerPrefs.GetInt("Music").Equals(0))
            {
                PlayerPrefs.SetInt("Music", 1);
                StopBackgroundSound();
                //settingReferences.musicButtonReference.ball
                //    .DOMove(settingReferences.musicButtonReference.offPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.musicButtonReference.switchImage.sprite =
                //                settingReferences.switchOffImage;
                //            settingReferences.musicButtonReference.statusText.text = "Music Off";
                //            settingReferences.musicButtonReference.icon.sprite =
                //                settingReferences.musicButtonReference.offIcon;
                //            settingReferences.musicButtonReference.onText.SetActive(false);
                //            settingReferences.musicButtonReference.offText.SetActive(true);
                //        });
                settingReferences.musicButtonReference.switchImage.sprite =
                              settingReferences.switchOffImage;
                //  settingReferences.musicButtonReference.statusText.text = "Music Off";
                //  settingReferences.musicButtonReference.icon.sprite =
                //     settingReferences.musicButtonReference.offIcon;
                //  settingReferences.musicButtonReference.onText.SetActive(false);
                //  settingReferences.musicButtonReference.offText.SetActive(true);
            }
            else
            {
                PlayerPrefs.SetInt("Music", 0);
                PlayBackgroundSound();
                //settingReferences.musicButtonReference.ball
                //    .DOMove(settingReferences.musicButtonReference.onPosition.position, 0.1f).SetEase(Ease.Linear)
                //    .OnComplete(
                //        () => {
                //            settingReferences.musicButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                //            settingReferences.musicButtonReference.statusText.text = "Music On";
                //            settingReferences.musicButtonReference.icon.sprite =
                //                settingReferences.musicButtonReference.onIcon;
                //            settingReferences.musicButtonReference.onText.SetActive(true);
                //            settingReferences.musicButtonReference.offText.SetActive(false);
                //        });

                settingReferences.musicButtonReference.switchImage.sprite = settingReferences.switchOnImage;
                //   settingReferences.musicButtonReference.statusText.text = "Music On";
                //  settingReferences.musicButtonReference.icon.sprite =
                //      settingReferences.musicButtonReference.onIcon;
                // settingReferences.musicButtonReference.onText.SetActive(true);
                //  settingReferences.musicButtonReference.offText.SetActive(false);
            }
        }

        public void OnHelpButtonClick()
        {
            settingReferences.help.SetActive(true);
            settingReferences.helpBGPanel.localScale = Vector3.zero;
            settingReferences.helpBGPanel.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        }

        public void OnHelpCloseButtonClick()
        {
            settingReferences.helpBGPanel.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack).OnComplete(() =>
            {
                settingReferences.help.SetActive(false);
            });
        }

        public void OnLeaveGameYesButtonClick()
        {
            exit.SetActive(true);
            settingReferences.settingPanel.localScale = Vector3.zero;
            exitPanel.localScale = Vector3.zero;
            exitPanel.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        }

        public void OnExitYesButtonClick()
        {
            exitPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                eventManager.SendLeaveTable();
                exit.SetActive(false);
                settingReferences.settingScreen.SetActive(false);
            });
        }

        public void OnExitNoButtonClick()
        {
            exitPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                exit.SetActive(false);
                settingReferences.settingScreen.SetActive(false);
            });
        }

        internal void SetSettingButtonsInteractable()
        {
            settingReferences.musicButtonReference.button.interactable = true;
            settingReferences.soundButtonReference.button.interactable = true;
            settingReferences.vibrateButtonReference.button.interactable = true;
        }

        #endregion

        #region Audio

        internal void PlayTimeOutCountSound()
        {
            if (PlayerPrefs.GetInt("Sound").Equals(0))
            {
                sounds.timeOutSource.Play();
            }
        }

        internal void StopTimeOutCountSound()
        {
            sounds.timeOutSource.Stop();
        }

        internal void PlaySound(AudioClip clip)
        {
            if (PlayerPrefs.GetInt("Sound").Equals(0))
            {
                sounds.gameAudioSource.PlayOneShot(clip);
            }
        }

        internal void PlayBackgroundSound()
        {
            if (PlayerPrefs.GetInt("Music").Equals(0))
            {
                sounds.backgroundSource.Play();
            }
        }

        internal void StopBackgroundSound()
        {
            sounds.backgroundSource.Stop();
        }

        #endregion

        internal void ShowNetwork(int count)
        {
            // if (isGameLogsEnable){
            Debug.Log("Showing Network Status" + count);
            // }

            netWorkImages.closeImage.SetActive(false);
            for (var i = 0; i < netWorkImages.netWorkImages.Count; i++)
            {
                netWorkImages.netWorkImages[i].SetActive(true);
            }

            for (var i = 0; i < count; i++)
            {
                netWorkImages.netWorkImages[i].SetActive(false);
            }

            if (count > 3)
            {
                netWorkImages.closeImage.SetActive(true);
            }
        }

        internal void ShowFTUE()
        {
            ftueScreen.SetActive(true);
            gamePlayScreen.SetActive(false);
            ftueManager.SetInitialStep();
        }

        internal void HideFTUE()
        {
            ftueScreen.SetActive(false);
            gamePlayScreen.SetActive(true);
            gameProgressStatus = GameProgressStatus.GAMEPLAY;
            if (gameRunOnSDK)
            {
                //  MGPSDK.MGPGameManager.instance.sdkConfig.data.lobbyData.IsFTUE = false;
            }

            eventManager.SendSignUp(totalPlayerCount);
        }

        internal void PlayKillEffect(bool isKilled)
        {
            if (isKilled)
            {
                killEffectBackground.gameObject.SetActive(true);
                killEffectBackground.color = redColor;
                killEffectBackground.DOColor(new Color(redColor.r, redColor.g, redColor.b, 0), 2).SetEase(Ease.Linear)
                    .OnComplete(() => { killEffectBackground.gameObject.SetActive(false); });
            }
            else
            {
                killEffectBackground.gameObject.SetActive(true);
                killEffectBackground.color = greenColor;
                killEffectBackground.DOColor(new Color(greenColor.r, greenColor.g, greenColor.b, 0), 2)
                    .SetEase(Ease.Linear)
                    .OnComplete(() => { killEffectBackground.gameObject.SetActive(false); });
            }
        }

        internal void MakeButtonInteractable()
        {
            for (var i = 0; i < buttons.Count; i++)
            {
                buttons[i].interactable = true;
            }
        }

        public void On2PlayerClick()
        {
            playerCountSelectionScreen.SetActive(false);
            totalPlayerCount = 2;
            if (PlayerPrefs.GetInt("FTUE").Equals(0))
            {
                gameProgressStatus = GameProgressStatus.FTUE;
                PlayerPrefs.SetInt("FTUE", 1);
                ShowFTUE();
            }
            else
            {
                gameProgressStatus = GameProgressStatus.GAMEPLAY;
                eventManager.SendSignUp(totalPlayerCount);
            }
        }

        public void On4PlayerClick()
        {
            playerCountSelectionScreen.SetActive(false);
            totalPlayerCount = 4;
            if (PlayerPrefs.GetInt("FTUE").Equals(0))
            {
                gameProgressStatus = GameProgressStatus.FTUE;
                PlayerPrefs.SetInt("FTUE", 1);
                ShowFTUE();
            }
            else
            {
                gameProgressStatus = GameProgressStatus.GAMEPLAY;
                eventManager.SendSignUp(totalPlayerCount);
            }
        }



        [Space(50)]
        [Header(" Server derive Comman popup => Anni ")]
        public GameObject serverCommanPopUp;

        public GameObject serverPopupHolder;
        public Text commanPopUpTitileText, commanPopUpMessageText;

        internal void ShowServerPopUp(string titile, string message)
        {
            Debug.Log(" Game manager  ||  Show Server popup  || Titile " + titile + " || message  || " + message);
            heartBeatManager.needToCheckInternet = false;
            serverPopupHolder.SetActive(true);
            serverCommanPopUp.SetActive(true);
            serverCommanPopUp.transform.localScale = Vector3.zero;
            serverCommanPopUp.transform.DOScale(1, 0.1f).SetEase(Ease.OutBounce);
            commanPopUpTitileText.text = titile;
            commanPopUpMessageText.text = message;
        }

        public void OnClickExitOnServerPopUp()
        {
            Debug.Log(" Game manager  ||  Show Server popup  || OnClickExitOnServerPopUp ");
            MGPSDK.MGPGameManager.instance.OnClickQuite();
        }

        internal void ResetGame()
        {
            socketHandler.socketManager.Socket.Disconnect();
            MakeButtonInteractable();
            loadingController.CloseLoadingScreen();
            HideNoInternetPanel();
            socketHandler.CreateSocket();
        }
    }

    [Serializable]
    public class UserDetail
    {
        public string userId;
        public string userName;
        public string userProfile;
        public int mobileNumber;
        public string tableId;
    }

    [Serializable]
    public class MyInternetReachability
    {
        private string url = "http://ping-test.net";
        private string result = "pong";

        public bool IsInternetAvailable => CheckForInternetAvailability();

        private bool CheckForInternetAvailability()
        {
            var fromUri = GetHtmlFromUrl(url);
            if (!String.IsNullOrEmpty(fromUri) && fromUri.Contains(result))
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("<Internet> CheckForInternetAvailability : Internet is available.");
                }

                return true;
            }
            else
            {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("<Internet> CheckForInternetAvailability : Internet not available.");
                }

                return false;
            }
        }

        private string GetHtmlFromUrl(string resource)
        {
            var html = string.Empty;
            var request = (HttpWebRequest)WebRequest.Create(resource);
            try
            {
                request.Timeout = 2000;
                using (var resp = (HttpWebResponse)request.GetResponse())
                {
                    var isSuccess = (int)resp.StatusCode < 299 && (int)resp.StatusCode >= 200;
                    return isSuccess ? result : "disconnect";
                }
            }
            catch (Exception e)
            {
                html = string.Empty;
            }

            return html;
        }
    }
}