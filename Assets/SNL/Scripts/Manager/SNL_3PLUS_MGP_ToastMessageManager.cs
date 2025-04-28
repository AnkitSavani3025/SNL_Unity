using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP {
    public enum ToastMessage {
        WAITFORPLAYER,
        GAMESTARTCOUNTDOWN,
        LASTMOVE,
        EXTRAMOVE,
        PLUS79POINTS
    }

    public class SNL_3PLUS_MGP_ToastMessageManager : MonoBehaviour {
        public GameObject waitForPlayerMessage;
        public GameObject gameStartCountDown;
        public GameObject lastMove;
        public GameObject extraMove;
        public GameObject plus50Points;
        public Text plus50PointsText;
        public Text plus79PointsText;
        public GameObject plus79Points;

        private Sequence extraMoveSequence;
        private Sequence bonusHomePointSequence;
        private Sequence scoreDifferenceSequence;
        private Sequence killPopupSequence;

        internal void ShowToastMessage(ToastMessage toastMessage){
            switch (toastMessage){
                case ToastMessage.WAITFORPLAYER:
                    ShowWaitForPlayerMessage();
                    break;
                case ToastMessage.GAMESTARTCOUNTDOWN:
                    gameStartCountDown.SetActive(true);
                    break;
                case ToastMessage.LASTMOVE:
                    lastMove.GetComponent<CanvasGroup>().alpha = 1;
                    lastMove.transform.localScale = Vector3.zero;
                    lastMove.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
                        lastMove.GetComponent<CanvasGroup>().DOFade(0, 1).SetEase(Ease.Linear).SetDelay(1.3f);
                    });
                    break;
                case ToastMessage.EXTRAMOVE:
                    if (extraMoveSequence != null){
                        extraMoveSequence.Kill();
                        extraMoveSequence = null;
                    }

                    if (scoreDifferenceSequence != null){
                        scoreDifferenceSequence.Kill();
                        scoreDifferenceSequence = null;
                    }

                    extraMove.GetComponent<CanvasGroup>().alpha = 1;
                    extraMove.transform.localScale = Vector3.zero;
                    var canvas = extraMove.GetComponent<CanvasGroup>();
                    extraMoveSequence = DOTween.Sequence();
                    extraMoveSequence.Append(extraMove.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack)
                        .SetDelay(0.1f));
                    extraMoveSequence.Append(canvas.DOFade(0, 1).SetEase(Ease.Linear)
                        .SetDelay(0.5f));
                    extraMoveSequence.Play().OnKill(() => {
                        extraMove.GetComponent<CanvasGroup>().alpha = 1;
                        extraMove.transform.localScale = Vector3.zero;
                    });

                    break;
                case ToastMessage.PLUS79POINTS:
                    if (bonusHomePointSequence != null){
                        bonusHomePointSequence.Kill();
                        bonusHomePointSequence = null;
                    }

                    plus79Points.GetComponent<CanvasGroup>().alpha = 1;
                    plus79Points.transform.localScale = Vector3.zero;
                    bonusHomePointSequence = DOTween.Sequence();
                    bonusHomePointSequence.Append(plus79Points.transform.DOScale(Vector3.one, 0.7f)
                        .SetEase(Ease.OutBack));
                    bonusHomePointSequence.Append(plus79Points.GetComponent<CanvasGroup>().DOFade(0, 1)
                        .SetEase(Ease.Linear)
                        .SetDelay(0.5f));
                    bonusHomePointSequence.Play().OnKill(() => {
                        plus79Points.GetComponent<CanvasGroup>().alpha = 1;
                        plus79Points.transform.localScale = Vector3.zero;
                    });
                    break;
            }
        }

        internal void HideToastMessage(ToastMessage toastMessage){
            switch (toastMessage){
                case ToastMessage.WAITFORPLAYER:
                    waitForPlayerMessage.SetActive(false);
                    break;
                case ToastMessage.GAMESTARTCOUNTDOWN:
                    gameStartCountDown.SetActive(false);
                    break;
            }
        }

        internal void ShowBonusPointsPopup(bool isKilled, int bonusPoint){
            plus50PointsText.text = isKilled ? "-" + bonusPoint + " POINTS" : "+" + bonusPoint + " POINTS";
            plus50Points.GetComponent<CanvasGroup>().alpha = 1;
            plus50Points.transform.localScale = Vector3.zero;
            plus50Points.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack).OnComplete(() => {
                plus50Points.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear).SetDelay(1f);
            });
        }

        internal void ShowScoreDifference(int score, bool isMinus){
            plus79PointsText.text = isMinus ? "-" + score + " POINTS" : "+" + score + " POINTS";
            plus79Points.GetComponent<CanvasGroup>().alpha = 1;
            plus79Points.transform.localScale = Vector3.zero;
            scoreDifferenceSequence = DOTween.Sequence();
            scoreDifferenceSequence.Append(plus79Points.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack));
            scoreDifferenceSequence.Append(plus79Points.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear)
                .SetDelay(1f));
            scoreDifferenceSequence.Play().OnKill(() => {
                plus79Points.GetComponent<CanvasGroup>().alpha = 1;
                plus79Points.transform.localScale = Vector3.zero;
            });
        }

        internal void ShowKillPopup(int score, bool isMinus){
            plus50PointsText.text = isMinus ? "-" + score + " POINTS" : "+" + score + " POINTS";
            plus50Points.GetComponent<CanvasGroup>().alpha = 1;
            plus50Points.transform.localScale = Vector3.zero;
            killPopupSequence = DOTween.Sequence();
            killPopupSequence.Append(plus50Points.transform.DOScale(Vector3.one, 0.7f).SetEase(Ease.OutBack));
            killPopupSequence.Append(plus50Points.GetComponent<CanvasGroup>().DOFade(0, 0.5f).SetEase(Ease.Linear)
                .SetDelay(1f));
            killPopupSequence.Play().OnKill(() => {
                plus50Points.GetComponent<CanvasGroup>().alpha = 1;
                plus50Points.transform.localScale = Vector3.zero;
            });
        }

        private void ShowWaitForPlayerMessage(){
            waitForPlayerMessage.SetActive(true);
            waitForPlayerMessage.transform.localScale = Vector3.zero;
            waitForPlayerMessage.transform.DOScale(Vector3.one, 0.75f).SetEase(Ease.InOutBack);
        }
    }
}