using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_FTUEManager : MonoBehaviour {
        internal int currentStep;
        public List<GameObject> steps = new List<GameObject>();
        public List<Image> pagination = new List<Image>();

        public Sprite deselectImage;
        public Sprite selectImage;

        public List<Canvas> scoreBgs = new List<Canvas>();
        public GameObject scoreBoarder;

        public GameObject blueScoreBoarder;
        public List<Canvas> tiles = new List<Canvas>();
        public List<Canvas> tokens = new List<Canvas>();
        public List<Canvas> tokensProfile = new List<Canvas>();
        public GameObject tokenBoarder;
        public List<Canvas> tilesNumber = new List<Canvas>();

        public Canvas moveCanvas;
        public GameObject moveBoarder;

        public List<Canvas> step5Tiles = new List<Canvas>();
        public List<Canvas> step5Tokens = new List<Canvas>();
        public List<Canvas> step5Texts = new List<Canvas>();

        public Canvas timerCanvas;
        public GameObject timerBoarder;

        public Canvas livesCanvas;
        public GameObject livesBoarder;

        public Canvas movesLeftCanvas;
        public GameObject movesLeftBoarder;

        public GameObject skipButton;
        public Text nextButtonText;

        internal void SetInitialStep(){
            skipButton.SetActive(true);
            nextButtonText.text = "NEXT";
            currentStep = 0;
            HideSteps();
            SetStep();
            SetPagination();
        }

        public void NextStep(){
            currentStep++;
            HideSteps();
            switch (currentStep){
                case 0:
                    break;
                case 1:
                    ShowStep1();
                    break;
                case 2:
                    ShowStep2();
                    break;
                case 3:
                    ShowStep3();
                    break;
                case 4:
                    ShowStep4();
                    break;
                case 5:
                    ShowStep5();
                    break;
                case 6:
                    ShowStep6();
                    break;
                case 7:
                    ShowStep7();
                    break;
                case 8:
                    ShowStep8();
                    break;
                default:
                    SNL_3PLUS_MGP_GameManager.instance.HideFTUE();
                    break;
            }

            SetStep();
            SetPagination();
        }

        private void SetStep(){
            for (var i = 0; i < steps.Count; i++){
                if (currentStep.Equals(i)){
                    steps[i].SetActive(true);
                }
                else{
                    steps[i].SetActive(false);
                }
            }
        }

        private void SetPagination(){
            for (var i = 0; i < pagination.Count; i++){
                if (currentStep.Equals(i)){
                    pagination[i].sprite = selectImage;
                }
                else{
                    pagination[i].sprite = deselectImage;
                }
            }
        }

        private void ShowStep1(){
            for (var i = 0; i < scoreBgs.Count; i++){
                scoreBgs[i].sortingOrder = 5;
            }

            scoreBoarder.SetActive(true);
        }

        private void HideStep1(){
            for (var i = 0; i < scoreBgs.Count; i++){
                scoreBgs[i].sortingOrder = 1;
            }

            scoreBoarder.SetActive(false);
        }

        private void ShowStep2(){
            scoreBgs[0].sortingOrder = 5;
            blueScoreBoarder.SetActive(true);
            for (var i = 0; i < tiles.Count; i++){
                tiles[i].sortingOrder = 5;
            }

            for (var i = 0; i < tokens.Count; i++){
                tokens[i].sortingOrder = 6;
            }

            for (var i = 0; i < tilesNumber.Count; i++){
                tilesNumber[i].sortingOrder = 6;
            }
        }

        private void HideStep2(){
            scoreBgs[0].sortingOrder = 1;
            blueScoreBoarder.SetActive(false);
            for (var i = 0; i < tiles.Count; i++){
                tiles[i].sortingOrder = 1;
            }

            for (var i = 0; i < tokens.Count; i++){
                tokens[i].sortingOrder = 0;
            }

            for (var i = 0; i < tilesNumber.Count; i++){
                tilesNumber[i].sortingOrder = 2;
            }
        }

        private void ShowStep3(){
            tokenBoarder.SetActive(true);
            for (var i = 0; i < tokensProfile.Count; i++){
                tokensProfile[i].sortingOrder = 5;
            }
        }

        private void HideStep3(){
            tokenBoarder.SetActive(false);
            for (var i = 0; i < tokensProfile.Count; i++){
                tokensProfile[i].sortingOrder = 3;
            }
        }

        private void ShowStep4(){
            moveCanvas.sortingOrder = 5;
            moveBoarder.SetActive(true);
        }

        private void HideStep4(){
            moveCanvas.sortingOrder = 3;
            moveBoarder.SetActive(false);
        }

        private void ShowStep5(){
            for (var i = 0; i < step5Tiles.Count; i++){
                step5Tiles[i].sortingOrder = 5;
            }

            for (var i = 0; i < step5Tokens.Count; i++){
                step5Tokens[i].sortingOrder = 6;
            }

            for (var i = 0; i < step5Texts.Count; i++){
                step5Texts[i].sortingOrder = 6;
            }
        }

        private void HideStep5(){
            for (var i = 0; i < step5Tiles.Count; i++){
                step5Tiles[i].sortingOrder = 1;
            }

            for (var i = 0; i < step5Tokens.Count; i++){
                step5Tokens[i].sortingOrder = 0;
            }

            for (var i = 0; i < step5Texts.Count; i++){
                step5Texts[i].sortingOrder = 2;
            }
        }

        private void ShowStep6(){
            timerCanvas.sortingOrder = 5;
            timerBoarder.SetActive(true);
        }

        private void HideStep6(){
            timerCanvas.sortingOrder = 3;
            timerBoarder.SetActive(false);
        }

        private void ShowStep7(){
            livesCanvas.sortingOrder = 5;
            livesBoarder.SetActive(true);
        }

        private void HideStep7(){
            livesCanvas.sortingOrder = 3;
            livesBoarder.SetActive(false);
        }

        private void ShowStep8(){
            movesLeftCanvas.sortingOrder = 5;
            movesLeftBoarder.SetActive(true);
            skipButton.SetActive(false);
            nextButtonText.text = "PLAY";
        }

        private void HideStep8(){
            movesLeftCanvas.sortingOrder = 3;
            movesLeftBoarder.SetActive(false);
        }

        private void HideSteps(){
            HideStep1();
            HideStep2();
            HideStep3();
            HideStep4();
            HideStep5();
            HideStep6();
            HideStep7();
            HideStep8();
        }

        public void OnSkipButtonClick(){
            SNL_3PLUS_MGP_GameManager.instance.HideFTUE();
        }
    }
}