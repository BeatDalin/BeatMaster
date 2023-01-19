using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingPopUp : MonoBehaviour
{
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        _exitBtn.onClick.AddListener((() =>
        {
            UIManager.instance.popUpStack.Pop();
            
            gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete += () =>
            {
                Destroy(gameObject);
            };
        }));
    }
}
