using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_Token : MonoBehaviour {
        public Image tokenImage;
        internal int tokenIndex;
        internal SNL_3PLUS_MGP_PlayerController playerController;
        public int tokenCurrentPosition;
        public bool isMovementAble;
        internal int moveStep = 0;

        public Transform tokenUI;
        public Animator scaleAnimator;
        public TrailRenderer trail;

        private Tween tokenAnimation;

        public Image blinkImage;
        public GameObject border;

        private void OnEnable(){
            SNL_3PLUS_MGP_GameManager.OnTokenAnimation += StartTokenAnimation;
            SNL_3PLUS_MGP_GameManager.SetTokenLayer += SetTokenLayer;
        }

        private void OnDisable(){
            SNL_3PLUS_MGP_GameManager.OnTokenAnimation -= StartTokenAnimation;
            SNL_3PLUS_MGP_GameManager.SetTokenLayer -= SetTokenLayer;
        }

        private void StartTokenAnimation(bool isAnimate){
            if (isAnimate){
                if (!isMovementAble) return;
                border.SetActive(true);
                var delay = Random.Range(0, 0.3f);
                var scale = transform.GetChild(0).localScale;
                tokenAnimation = transform.GetChild(0).DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.3f).SetEase(Ease.OutSine)
                    .SetLoops(-1, LoopType.Yoyo)
                    .SetDelay(delay).OnKill(() => {
                        transform.GetChild(0).localScale = scale;
                        border.SetActive(false);
                    });
            }
            else{
                tokenAnimation.Kill();
                tokenAnimation = null;
            }
        }

        private void SetTokenLayer(){
            var canvas = GetComponent<Canvas>();
            canvas.sortingOrder = isMovementAble ? 4 : 3;
        }

        public void OnTokenClick(){
            if (isMovementAble && SNL_3PLUS_MGP_GameManager.instance.gamePlayStatus == GamePlayStatus.PlayerMovement){
                SNL_3PLUS_MGP_GameManager.instance.eventManager.SendMoveToken(tokenIndex);
                SNL_3PLUS_MGP_GameManager.instance.gamePlayStatus = GamePlayStatus.PassTurn;
            }
        }

        internal void StartTokenMovementAnimation(MoveTokenModel moveTokenModel){
            moveStep = SNL_3PLUS_MGP_GameManager.instance.diceValue;
            //if (moveTokenModel.pawnPosition.Equals(tokenCurrentPosition)) return;
            StartCoroutine(TokenMovementAnimationSecond(moveTokenModel.seatIndex, () => {
                if (SNL_3PLUS_MGP_GameManager.instance.isGameLogsEnable){
                    Debug.Log("Token Movement Completed");
                }

                if (tokenCurrentPosition.Equals(100)){
                    SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.tokenEnterSafePlace);
                    if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                        SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowBonusPointsPopup(false, moveTokenModel.bonusPoint);
                    }
                }

                if (SNL_3PLUS_MGP_GameManager.instance.totalMoves - SNL_3PLUS_MGP_GameManager.instance.totalTurnTaken == 0){
                    SNL_3PLUS_MGP_GameManager.instance.DisplayMovesLeft();
                }

                if (moveTokenModel.isSnake || moveTokenModel.isLadder){
                    if (moveTokenModel.isSnake){
                        if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowScoreDifference(moveTokenModel.scoreDifference,
                                true);
                        }

                        SNL_3PLUS_MGP_GameManager.instance.SwallowToken(moveTokenModel.pawnPosition, moveTokenModel.pawn,
                            moveTokenModel.seatIndex,
                            () => {
                                if (moveTokenModel.isKill){
                                    if (moveTokenModel.killSeatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                                        if (moveTokenModel.killScore > 0){
                                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                                moveTokenModel.killScore,
                                                true);
                                        }

                                        SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(true);
                                    }
                                    else if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                                        if (moveTokenModel.killScore > 0){
                                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                                moveTokenModel.killScore,
                                                false);
                                        }

                                        SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(false);
                                    }

                                    SNL_3PLUS_MGP_GameManager.instance.SendTokenToHome(moveTokenModel.killSeatIndex,
                                        moveTokenModel.killPawnNumber);
                                }
                            });
                    }
                    else if (moveTokenModel.isLadder){
                        if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowScoreDifference(moveTokenModel.scoreDifference,
                                false);
                        }

                        SNL_3PLUS_MGP_GameManager.instance.MoveTokenToPosition(moveTokenModel.pawnPosition, moveTokenModel.pawn,
                            moveTokenModel.seatIndex, () => {
                                if (moveTokenModel.isKill){
                                    if (moveTokenModel.killSeatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                                        if (moveTokenModel.killScore > 0){
                                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                                moveTokenModel.killScore,
                                                true);
                                        }

                                        SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(true);
                                    }
                                    else if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                                        if (moveTokenModel.killScore > 0){
                                            SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                                moveTokenModel.killScore,
                                                false);
                                        }

                                        SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(false);
                                    }

                                    SNL_3PLUS_MGP_GameManager.instance.SendTokenToHome(moveTokenModel.killSeatIndex,
                                        moveTokenModel.killPawnNumber);
                                }
                            });
                    }
                }
                else{
                    if (moveTokenModel.isKill){
                        if (moveTokenModel.killSeatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                            if (moveTokenModel.killScore > 0){
                                SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                    moveTokenModel.killScore,
                                    true);
                            }

                            SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(true);
                        }
                        else if (moveTokenModel.seatIndex.Equals(SNL_3PLUS_MGP_GameManager.instance.thisPlayerSeatIndex)){
                            if (moveTokenModel.killScore > 0){
                                SNL_3PLUS_MGP_GameManager.instance.toastMessageManager.ShowKillPopup(
                                    moveTokenModel.killScore,
                                    false);
                            }

                            SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(false);
                        }

                        SNL_3PLUS_MGP_GameManager.instance.SendTokenToHome(moveTokenModel.killSeatIndex,
                            moveTokenModel.killPawnNumber);
                    }
                    else{
                        PlayJumpAnimation();
                    }
                }
            }));
        }

        private IEnumerator TokenMovementAnimationSecond(int seatIndex, UnityAction callBack){
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            transform.SetParent(SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition]);
            transform.localScale = Vector3.one;
            var val = 0;
            var scaleUp = true;
            Tweener moveTweener = null;
            var rotateVector = GetRotateVector(SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition]);
            transform.DORotate(rotateVector, 0.2f).SetEase(Ease.InBack);
            while (val < moveStep){
                var target = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition].position;
                if (tokenCurrentPosition - 1 >= 0){
                    var blink = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                        .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                        .blink;
                    blink.color = Color.white;
                    blink.DOFade(0, 0.5f).SetEase(Ease.OutSine);
                    var rotateVectorTemp = GetRotateVector(SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition]);
                    transform.rotation = Quaternion.Euler(rotateVectorTemp);
                }

                moveTweener ??= transform.DOMove(target, 0.12f).SetEase(Ease.Flash)
                    .OnComplete(() => { moveTweener = null; });

                var dist = Vector3.Distance(transform.position, target);
                if (dist <= 0.02f){
                    scaleUp = true;
                    val++;
                    tokenCurrentPosition++;
                    if (val == moveStep){
                        moveTweener.Kill(true);
                        var blink = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                            .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                            .blink;
                        blink.color = Color.white;
                        blink.DOFade(0, 0.5f).SetEase(Ease.OutSine);
                        SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                            .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                            .AddTokenToThisBox(this);
                        transform.DORotate(Vector3.zero, 0.15f).SetEase(Ease.OutBounce);
                    }

                    SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.tokenMovement);
                }

                yield return new WaitForEndOfFrame();
            }

            callBack.Invoke();
        }

        private Vector3 GetRotateVector(Transform wayPoint){
            var rotateSide = wayPoint.GetComponent<SNL_3PLUS_MGP_BoxProperty>().tokenRotateSide;
            switch (rotateSide){
                case TokenRotateSide.Left:
                    return new Vector3(0, 0, 20);
                case TokenRotateSide.Right:
                    return new Vector3(0, 0, -20);
                default:
                    return Vector3.zero;
            }
        }

        private void PlayJumpAnimation(){
            tokenUI.DOLocalMoveY(tokenUI.localPosition.y + 20, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f)
                .OnComplete(() => { tokenUI.DOLocalMoveY(tokenUI.localPosition.y - 20, 0.05f).SetEase(Ease.Linear); });
        }
        
        private IEnumerator TokenMovementAnimation(int seatIndex, UnityAction callBack){
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            transform.SetParent(SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition]);
            transform.localScale = Vector3.one;
            var val = 0;
            var scaleUp = true;
            Tweener moveTweener = null;
            while (val < moveStep){
                var target = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition].position;
                if (tokenCurrentPosition - 1 >= 0){
                    var trail = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                        .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                        .trail;
                    AnimateTrail(seatIndex, trail);
                }

                scaleAnimator.Play("scaleup");
                moveTweener ??= transform.DOMove(target, 0.2f).OnComplete(() => { moveTweener = null; });

                var dist = Vector3.Distance(transform.position, target);
                if (dist <= 0.02f){
                    scaleUp = true;
                    val++;
                    tokenCurrentPosition++;
                    if (val == moveStep){
                        moveTweener.Kill(true);
                        SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                            .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                            .AddTokenToThisBox(this);
                    }

                    SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.tokenMovement);
                }
                else if (dist <= 0.15f){
                    // transform.localScale =
                    //     Vector3.Lerp(transform.localScale, new Vector3(0.8f, 0.8f, 0.8f), Time.deltaTime * 9);
                }
                else{
                    if (scaleUp){
                        //transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        scaleUp = false;
                        //scaleAnimator.Play("scaleup");
                    }
                }

                yield return new WaitForSeconds(0f);
            }

            callBack.Invoke();
        }

        internal void TokenGoHomeAnimation(UnityAction callBack){
            SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.tokenKill);
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            var position = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].position;
            SNL_3PLUS_MGP_GameManager.instance.killEffect.transform.position = position;
            SNL_3PLUS_MGP_GameManager.instance.killEffect.Play();
            tokenCurrentPosition = 0;
            transform.localScale = Vector3.one;
            callBack.Invoke();
        }

        internal void MoveToken(int tokenPosition, int seatIndex, UnityAction callBack){
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            tokenCurrentPosition = tokenPosition;
            var target = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].position;
            BlinkToken();
            trail.gameObject.SetActive(true);
            SetTrailColor(seatIndex);
            SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.ladderClimb);
            transform.DOMove(target, 0.2f).SetEase(Ease.Linear).OnComplete(() => {
                trail.gameObject.SetActive(false);
                callBack.Invoke();
                PlayJumpAnimation();
                SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                    .AddTokenToThisBox(this);
            });
        }

        internal void SwallowToken(int tokenPosition, UnityAction callBack){
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            SNL_3PLUS_MGP_GameManager.instance.PlayKillEffect(true);
            var target = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].position;
            BlinkToken();
            var snake = SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]
                .GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                .snakes[SNL_3PLUS_MGP_GameManager.instance.gameBoardNumber];
            var snakeBlinkImage = snake.blink;
            BlinkSnake(snakeBlinkImage);
            var path = snake.wayPoints;
            var duration = snake.time;
            var pathPoints = new List<Vector3>();
            foreach (var point in path){
                pathPoints.Add(point.position);
            }

            SNL_3PLUS_MGP_GameManager.instance.PlaySound(SNL_3PLUS_MGP_GameManager.instance.sounds.snakeBite);
            tokenCurrentPosition = tokenPosition;
            transform.DOPath(pathPoints.ToArray(), duration, PathType.Linear, PathMode.TopDown2D).OnComplete(() => {
                callBack.Invoke();
                PlayJumpAnimation();
                SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                    .AddTokenToThisBox(this);
            });
        }

        private void BlinkToken(){
            blinkImage.DOKill();
            blinkImage.DOFade(0.7f, 0.2f).SetEase(Ease.OutSine).SetLoops(8, LoopType.Yoyo).OnKill(() => {
                blinkImage.color = new Color(255, 255, 255, 0);
            });
        }

        private void BlinkSnake(Image blink){
            blink.DOFade(0.5f, 0.2f).SetEase(Ease.OutSine).SetLoops(8, LoopType.Yoyo);
        }

        internal void SetTokenPositionOnReconnect(int tokenPosition){
            SNL_3PLUS_MGP_GameManager.instance.RemoveTokenFromBox(this);
            tokenCurrentPosition = tokenPosition;
            if (tokenPosition > 0){
                transform.SetParent(SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1]);
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
                SNL_3PLUS_MGP_GameManager.instance.wayPoints[tokenCurrentPosition - 1].GetComponent<SNL_3PLUS_MGP_BoxProperty>()
                    .AddTokenToThisBox(this);
            }
        }

        private void AnimateTrail(int seatIndex, Image trail){
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(seatIndex);
            switch (player.tokenColor){
                case TokenColor.Blue:
                    trail.color = SNL_3PLUS_MGP_GameManager.instance.trailColors.blueColor;
                    break;
                case TokenColor.Green:
                    trail.color = SNL_3PLUS_MGP_GameManager.instance.trailColors.greenColor;
                    break;
                case TokenColor.Yellow:
                    trail.color = SNL_3PLUS_MGP_GameManager.instance.trailColors.yellowColor;
                    break;
                case TokenColor.Red:
                    trail.color = SNL_3PLUS_MGP_GameManager.instance.trailColors.redColor;
                    break;
            }

            trail.DOFade(0, 1f).SetEase(Ease.OutSine);
        }

        private void SetTrailColor(int seatIndex){
            var player = SNL_3PLUS_MGP_GameManager.instance.ReturnPlayerFromSeatIndex(seatIndex);
            switch (player.tokenColor){
                case TokenColor.Blue:
                    trail.startColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.blueColor;
                    trail.endColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.blueColor;
                    break;
                case TokenColor.Green:
                    trail.startColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.greenColor;
                    trail.endColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.greenColor;
                    break;
                case TokenColor.Yellow:
                    trail.startColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.yellowColor;
                    trail.endColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.yellowColor;
                    break;
                case TokenColor.Red:
                    trail.startColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.redColor;
                    trail.endColor = SNL_3PLUS_MGP_GameManager.instance.trailColors.redColor;
                    break;
            }
        }
    }
}