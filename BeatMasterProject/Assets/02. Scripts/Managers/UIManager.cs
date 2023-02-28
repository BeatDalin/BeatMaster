using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

// UI Manager : 게이 씬 상에서 생길 여러가지 'UI 캔버스 프리팹'들의 생성과 삭제를 관리
// 팝업 UI 캔버스들은 렌더링 순서 또한 관리되어야 함
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // 고정 UI와 겹치지 않도록 10부터 시작하게 할 것.
    // 팝업 캔버스 UI들은 10, 11, 12 ....이렇게 렌더링 순서를 부여받음(늦게 생성될수록 가장 위에 그려지도록)
    private int _order = 10; // Sort order

    // 여러개가 생길 수 있으며, 가장 나중에 생성된 팝업부터 먼저 닫혀야 함 (팝업 캔버스 UI 오브젝트들을 스택으로 관리)
    private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); // 오브젝트 말고 컴포넌트 담음. 팝업 캔버스 UI들을 담는다.

    // 고정 UI : Sort order = 0으로 고정(가장 먼저 그려지고 밑에서 그려지게)
    // 스택으로 관리 X
    private UI_Scene _sceneUI = null; // 현재의 고정 캔버스 UI


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #region Root => Property

    // @UI_Root라는 이름의 오브젝트가 없다면 만들어서 리턴해주는 프로퍼티
    // Hierarchy상의 오브젝트들도 마치 폴더 안에 있는 것처럼 관련있는 것들끼리
    // 종류별로 이름을 구분한 빈 오브젝트의 자식으로 넣어 정리할 것이기 때문.
    // UI 오브젝트들은 이 @UI_Root 빈 오브젝트 아래에 생성되게 그룹화할 것이라서 필요
    public GameObject Root
    {
        get
        {
            GameObject root = GameObject.Find("@UI_Root");
            if (root == null)
            {
                root = new GameObject
                {
                    name = "@UI_Root"
                };
            }

            return root;
        }
    }

    #endregion


    #region SetCanvas : 오브젝트의 캔버스 세팅

    // 오브젝트의 캔버스 세팅 => SetCanvas
    public void SetCanvas(GameObject go, bool sort = true)
    {
        Canvas canvas = Util.GetOrAddComponent<Canvas>(go); // canvas에 Canvas 컴포넌트 가져옴(없으면 붙여서라도)
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.overrideSorting = true; // 캔버스 안에 캔버스 중첩일 경우 (부모 캔버스가 어떤 값을 가지던 나는 내 오더값을 가지려 할 때 true로)


        if (sort)
        {
            canvas.sortingOrder = _order; // 팝업 UI의 경우 canvas.sortingOrder를 _order로 세팅
            _order++; // _order를 증가
        }
        // sorting 요청 X 라는 소리는 팝업이 아닌 일반 고정 UI
        else
        {
            // 고정 UI의 경우 canvas.sortingOrder 값을 0으로 세팅
            canvas.sortingOrder = 0;
        }
    }

    #endregion


    #region ShowSceneUI : 고정 UI 캔버스 프리팹 생성

    // 고정 UI 캔버스 프리팹 생성 => ShowSceneUO
    // T는 UI_Scene 자식들만 가능
    public T ShowSceneUI<T>(string name = null) where T : UI_Scene
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        // 프리팹을 Instantiate로 생성하고 _sceneUI에 바인딩 후 리턴
        GameObject go = ResourceManager.instance.Instantiate($"UI/Scene/{name}");
        T sceneUI = Util.GetOrAddComponent<T>(go);
        _sceneUI = sceneUI;

        go.transform.SetParent(Root.transform); // Root 프로퍼티를 통해 @UI_Root의 자식으로 넣어줌

        return sceneUI;
    }

    #endregion


    #region ShowPopupUI : 팝업 UI 캔버스 프리팹 생성

    // 팝업 UI 캔버스 프리팹 생성 => ShowPopupUI
    // T는 UI_Popup 자식들만 가능
    public T ShowPopupUI<T>(string name = null) where T : UI_Popup
    {
        if (string.IsNullOrEmpty(name))
        {
            name = typeof(T).Name;
        }

        // 프리팹을 Instantiate로 생성하고 popup에 바인딩 후 리턴
        GameObject go = ResourceManager.instance.Instantiate($"UI/Popup/{name}");
        T popup = Util.GetOrAddComponent<T>(go);
        _popupStack.Push(popup);

        go.transform.SetParent(Root.transform); // Root 프로퍼티를 통해 @UI_Root의 자식으로 넣어줌

        return popup;
    }

    #endregion


    #region ClosePopup
    public void ClosePopupUI(UI_Popup popup)
    {
        // 스택이 비어있으면 삭제 불가
        if (_popupStack.Count == 0)
        {
            return;
        }

        // 스택의 가장 위에있는 Peek() 것만 삭제할 수 있기 때문에 popup이 Peek()가 아니면 삭제 불가
        if (_popupStack.Peek() != popup)
        {
            return;
        }
        
        ClosePopupUI();
    }


    public void ClosePopupUI()
    {
        if (_popupStack.Count == 0)
        {
            return;
        }

        UI_Popup popup = _popupStack.Pop();
        ResourceManager.instance.Destory(popup.gameObject);
        popup = null;
        _order--;   // order 줄이기
    }

    public void CloseAllPopupUI()
    {
        while (_popupStack.Count > 0)
        {
            ClosePopupUI();
        }
    }
    

    #endregion

    
    
    
    

    public void OpenPanel(GameObject panel)
    {
        if (!panel.activeSelf)
        {
            SoundManager.instance.PlaySFX("Touch");
            panel.SetActive(true);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
        }
        //popUpStack.Push(panel);
    }

    public void ClosePanel(GameObject panel)
    {
        // if (popUpStack.Count == 0)
        // {
        //     return;
        // }
        //GameObject g = popUpStack.Pop();
        SoundManager.instance.PlaySFX("Touch");
        panel.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.6f).onComplete += () =>
        {
            panel.SetActive(false);
        };
    }

}
