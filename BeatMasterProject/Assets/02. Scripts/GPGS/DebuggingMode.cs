using UnityEngine;
using UnityEngine.SceneManagement;

public class DebuggingMode : MonoBehaviour
{
    public static DebuggingMode Instance { get; private set; }
    private bool _isAutoPlay;

    private float _deltaTime = 0.0f;

    private NormalGame _normal;
    private Store _store;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name.Contains("Stage"))
        {
            _normal = FindObjectOfType<NormalGame>();
            _isAutoPlay = _normal.isAutoPlay;
        }
        else if (SceneManager.GetActiveScene().name.Contains("Title"))
        {
            _store = FindObjectOfType<MenuTitleButton>().transform.GetChild(4).GetComponent<Store>();
        }
    }


    private void Update()
    {
        _deltaTime += (Time.deltaTime - _deltaTime) * 0.1f;
    }
    private void OnGUI()
    {
        GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, Vector3.one * 3);

        if (SceneManager.GetActiveScene().name.Contains("Stage") && GUILayout.Button(_isAutoPlay ? "AutoPlay" : "Play"))
        {
            _isAutoPlay = !_isAutoPlay;
            _normal.isAutoPlay = _isAutoPlay;
        }
        else if (SceneManager.GetActiveScene().name.Equals("Title") && GUILayout.Button("Coins"))
        {
            DataCenter.Instance.UpdatePlayerItemInDebuggingMode(500);
            if (_store.enabled)
            {
                _store.UpdatePlayersDataInScene();
            }
        }

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(80, 0, w, h * 2 / 120);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 120;
        style.normal.textColor = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        float msec = _deltaTime * 1000.0f;
        float fps = 1.0f / _deltaTime;
        string text = string.Format("{0:0.} fps ({1:0.0} ms) ", fps, msec);
        GUI.Label(rect, text, style);
    }
}
