using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SNL_3PLUS_MGP {
    public class SNL_3PLUS_MGP_GridResizer : MonoBehaviour {
        public GridLayoutGroup grid;

        void Start(){
            float width = grid.GetComponent<RectTransform>().rect.width - grid.padding.left;
            float height = grid.GetComponent<RectTransform>().rect.height - grid.padding.top;
            Debug.Log(width);
            float spacingX = grid.spacing.x;
            float spacingY = grid.spacing.y;
            Vector2 newSize = new Vector2((width / 10) - spacingX, (height / 10) - spacingY);
            grid.cellSize = newSize;
        }
    }
}