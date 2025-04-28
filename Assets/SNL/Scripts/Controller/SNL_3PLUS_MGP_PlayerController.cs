using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP
{
    public class SNL_3PLUS_MGP_PlayerController : MonoBehaviour
    {
        public int seatIndex;
        public Text userNameText;
        public Image userProfilePic;
        public Slider turnSlider;
        public Slider timerEffectSlider;
        public Text diceValueText;
        public Text scoreText;
        public TextMeshProUGUI updatedScoreText;
        public GameObject extraTime;
        public GameObject loading;
        public GameObject boarder;
        public GameObject leftSign;
        public GameObject parentPayerData;

        public List<ParticleSystem> effects = new List<ParticleSystem>();

        public TokenColor tokenColor;
        public List<SNL_3PLUS_MGP_Token> allTokensForThisPlayer = new List<SNL_3PLUS_MGP_Token>();
        public List<Transform> tokenHomePositions = new List<Transform>();
        public List<Image> lives = new List<Image>();

        public Image timerEffectImage;
        public Image timerImage;

        public Sprite normalTimerImage;

        private Coroutine timerCoroutine;

        private Tween diceValueTween;


        public Text dice1, dice2, dice3;

        [Serializable]
        public class ScorePositions
        {
            public Transform upPosition;
            public Transform centerPosition;
            public Transform downPosition;
        }

        public ScorePositions scorePositions;

        private void OnEnable()
        {
            SNL_3PLUS_MGP_GameManager.OnResetTurn += ResetWhenTurnChange;
        }

        private void OnDisable()
        {
            SNL_3PLUS_MGP_GameManager.OnResetTurn -= ResetWhenTurnChange;
        }

        private void Update()
        {
            timerEffectImage.material.mainTextureOffset += new Vector2(-Time.deltaTime * 0.6f, 0);
        }

        internal void GenerateTokens(GameObject obj, int tokenCount)
        {
            allTokensForThisPlayer.Clear();
            for (var i = 0; i < tokenCount; i++)
            {
                var token = Instantiate(obj, transform).GetComponent<SNL_3PLUS_MGP_Token>();
                token.tokenIndex = i + 1;
                token.tokenCurrentPosition = 0;
                token.playerController = this;
                switch (tokenColor)
                {
                    case TokenColor.Yellow:
                        token.tokenImage.sprite = SNL_3PLUS_MGP_GameManager.instance.tokenImages[0];
                        token.name = "Y" + token.tokenIndex;
                        break;
                    case TokenColor.Blue:
                        token.tokenImage.sprite = SNL_3PLUS_MGP_GameManager.instance.tokenImages[3];
                        token.name = "B" + token.tokenIndex;
                        break;
                    case TokenColor.Green:
                        token.tokenImage.sprite = SNL_3PLUS_MGP_GameManager.instance.tokenImages[1];
                        token.name = "G" + token.tokenIndex;
                        break;
                    case TokenColor.Red:
                        token.tokenImage.sprite = SNL_3PLUS_MGP_GameManager.instance.tokenImages[2];
                        token.name = "R" + token.tokenIndex;
                        break;
                    default:
                        break;
                }

                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable)
                {
                    Debug.Log("Setting Token Scale For " + token.name + " Scale " + token.transform.localScale);
                }

                allTokensForThisPlayer.Add(token);
                SNL_3PLUS_MGP_GameManager.instance.allTokensOnBoard.Add(token);
                token.transform.SetParent(tokenHomePositions[token.tokenIndex - 1]);
                token.transform.localScale = Vector3.one;
                token.transform.localPosition = Vector3.zero;
                token.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                token.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(2);
            AnimateDiceValueText();
        }

        internal void AnimateDiceValueText()
        {
            diceValueTween = diceValueText.transform.DOScale(new Vector3(1.3f, 1.3f, 1.3f), 0.4f).SetEase(Ease.InSine)
                .SetLoops(-1, LoopType.Yoyo).OnKill(
                    () => { diceValueText.transform.localScale = Vector3.one; });
        }

        internal void SetPlayerInfo(string userName, string profilePic)
        {
            userNameText.text = userName;
            userProfilePic.GetComponent<SNL_3PLUS_MGP_ProfileDownLoader>().GetDisplayImage(profilePic);
            //SNL_3PLUS_MGP_ProfileDownLoader.intance.GetDisplayImage(profilePic, (Sprite sprite) => { userProfilePic.sprite = sprite; });
        }

        internal void StartPlayerTurn(int diceValue, float remainTime, List<int> dice)
        {
            timerCoroutine = StartCoroutine(StartTurnTimer(remainTime));
            SetDiceValue(diceValue, dice);
        }

        internal void SetDiceValue(int diceValue, List<int> dice)
        {
            loading.SetActive(false);
            diceValueText.text = diceValue.ToString();
            if (transform.parent.name == "1")
            {
                for (int i = 0; i < dice.Count; i++)
                {
                    if (i == 0)
                        dice1.text = dice[0].ToString();
                    if (i == 1)
                        dice2.text = dice[1].ToString();
                    if (i == 2)
                        dice3.text = dice[2].ToString();
                }
            }
        }

        private IEnumerator StartTurnTimer(float remainTime)
        {
            turnSlider.gameObject.SetActive(true);
            timerEffectSlider.gameObject.SetActive(true);
            var timerValue = (float)SNL_3PLUS_MGP_GameManager.instance.baseTurnTimerValue;
            turnSlider.maxValue = timerValue;
            timerEffectSlider.maxValue = timerValue;
            turnSlider.value = remainTime;
            timerEffectSlider.value = remainTime;

            var check = false;
            while (remainTime > 0)
            {
                remainTime -= Time.deltaTime;
                turnSlider.value = remainTime;
                timerEffectSlider.value = remainTime;
                if (!check && turnSlider.value < 5f)
                {
                    check = true;
                    if (SNL_3PLUS_MGP_GameManager.instance.currentTurnSeatIndex.Equals(SNL_3PLUS_MGP_GameManager
                            .instance.thisPlayerSeatIndex))
                    {
                        SNL_3PLUS_MGP_GameManager.instance.PlayTimeOutCountSound();
                    }
                }

                yield return new WaitForEndOfFrame();
            }

            SNL_3PLUS_MGP_GameManager.instance.StopTimeOutCountSound();
        }

        private void ResetWhenTurnChange()
        {
            if (timerCoroutine != null) StopCoroutine(timerCoroutine);
            if (diceValueTween != null)
            {
                diceValueTween.Kill();
                diceValueTween = null;
            }

            turnSlider.gameObject.SetActive(false);
            timerEffectSlider.gameObject.SetActive(false);
            loading.SetActive(true);
            diceValueText.text = "";
            if (transform.parent.name == "1")
            {
                dice1.text = "";
                dice2.text = "";
                dice3.text = "";
            }
            SetExtraTimeImage(false);
            ScaleDownProfile();
            StopProfileEffect();
            if (!boarder.activeInHierarchy)
            {
                boarder.SetActive(true);
            }
        }

        internal void EnablePlayer()
        {
            gameObject.SetActive(true);
        }

        internal void DisablePlayer()
        {
            gameObject.SetActive(false);
        }

        internal void UpdateScore(int score, bool isReconnected)
        {
            if (isReconnected)
            {
                AnimateScoreText(score);
                return;
            }

            var previousScore = int.Parse(scoreText.text);
            var scoreDifference = score - previousScore;
            if (score - previousScore != 0)
            {
                AnimateScoreText(score);
            }
        }

        internal void SetTokenAtHomePosition(int tokenId)
        {
            allTokensForThisPlayer[tokenId].transform.SetParent(tokenHomePositions[tokenId]);
            allTokensForThisPlayer[tokenId].transform.localScale = Vector3.one;
            allTokensForThisPlayer[tokenId].transform.DOMove(tokenHomePositions[tokenId].position, 0.4f)
                .SetEase(Ease.Linear).OnComplete(() =>
                {
                    allTokensForThisPlayer[tokenId].transform.SetParent(tokenHomePositions[tokenId]);
                    allTokensForThisPlayer[tokenId].transform.localPosition = Vector3.zero;
                    allTokensForThisPlayer[tokenId].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    allTokensForThisPlayer[tokenId].GetComponent<RectTransform>().sizeDelta = Vector2.zero;
                });
        }

        internal void ActiveTokenMovement(int diceValue)
        {
            foreach (var t in allTokensForThisPlayer)
            {
                t.isMovementAble = t.tokenCurrentPosition + diceValue <= 100;
            }
        }

        internal void SetExtraTimeImage(bool isActive)
        {
            extraTime.SetActive(isActive);
            timerImage.sprite = isActive ? SNL_3PLUS_MGP_GameManager.instance.extraTimerImage : normalTimerImage;
        }

        internal void ScaleUpProfile()
        {
            var scale = 1.1f;
            //  transform.DOScale(new Vector3(scale, scale, scale), 0.2f).SetEase(Ease.OutBack);
        }

        private void ScaleDownProfile()
        {
            transform.localScale = Vector3.one;
        }

        private int GetMovementPossibleTokenCount()
        {
            var count = 0;
            foreach (var t in allTokensForThisPlayer)
            {
                if (t.isMovementAble)
                {
                    count++;
                }
            }

            return count;
        }

        internal void UpdateLives(int turnMissedCount)
        {
            for (var i = 0; i < lives.Count; i++)
            {
                lives[i].sprite = SNL_3PLUS_MGP_GameManager.instance.liveRed;
            }

            for (var i = 0; i < turnMissedCount; i++)
            {
                lives[i].sprite = SNL_3PLUS_MGP_GameManager.instance.liveGrey;
            }
        }

        internal void RemovePlayerData()
        {
            scoreText.text = "0";
            diceValueText.text = "";
            userNameText.text = "";
            UpdateLives(3);
        }

        internal void PlayEffect()
        {
            PlayProfileEffect();
        }

        internal void ShowPlayerLeftSign()
        {
            parentPayerData.SetActive(false);
            leftSign.SetActive(true);
        }

        internal void HidePlayerLeftSign()
        {
            parentPayerData.SetActive(true);
            leftSign.SetActive(false);
        }

        private void PlayProfileEffect()
        {
            foreach (var t in effects)
            {
                t.Play();
            }
        }

        private void StopProfileEffect()
        {
            foreach (var t in effects)
            {
                t.Stop();
            }
        }

        internal void ResetProfile()
        {
            HidePlayerLeftSign();
            scoreText.text = "0";
            diceValueText.text = "";
            userNameText.text = "";
            UpdateLives(0);
        }

        internal void AnimateScoreText(int score)
        {
            scoreText.DOFade(0, 0.5f).SetEase(Ease.InBack);
            scoreText.transform.DOLocalMove(scorePositions.upPosition.localPosition, 0.5f).SetEase(Ease.InBack)
                .OnComplete(
                    () =>
                    {
                        scoreText.transform.localPosition = scorePositions.downPosition.localPosition;
                        scoreText.text = score.ToString();
                        scoreText.DOFade(1, 0.5f).SetEase(Ease.OutBack);
                        scoreText.transform.DOLocalMove(scorePositions.centerPosition.localPosition, 0.5f)
                            .SetEase(Ease.OutBack);
                    });
        }
    }
}