using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TitleButtonController : MonoBehaviour
{
    public GameObject firstSelectedButton;
    public GameObject cursor; // カーソルオブジェクト

    void Start()
    {
        // 最初のボタンを選択
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        UpdateCursorPosition(firstSelectedButton);
        
    }

    void Update()
    {
        // 現在選択されているボタンを取得
        GameObject selectedButton = EventSystem.current.currentSelectedGameObject;
        if (selectedButton != null)
        {
            UpdateCursorPosition(selectedButton);
            
        }
    }

    void UpdateCursorPosition(GameObject selectedButton)
    {
        // カーソルの位置を選択されたボタンの左側に更新
        RectTransform buttonRectTransform = selectedButton.GetComponent<RectTransform>();
        RectTransform cursorRectTransform = cursor.GetComponent<RectTransform>();

        // ボタンの左側にカーソルを配置
        Vector3 buttonPosition = buttonRectTransform.position;
        cursorRectTransform.position = new Vector3(buttonPosition.x - buttonRectTransform.rect.width / 2 - cursorRectTransform.rect.width / 2, buttonPosition.y, buttonPosition.z);
    }

    //選択したボタンの点滅

}
