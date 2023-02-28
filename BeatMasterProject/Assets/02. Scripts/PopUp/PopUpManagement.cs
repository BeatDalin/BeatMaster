using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManagement : MonoBehaviour
{
    [SerializeField] private Button _exitBtn;

    private void Start()
    {
        AddListener();
    }

    public void AddListener()
    {
        _exitBtn.onClick.AddListener(() =>
        {
            //UIManager.instance.popUpStack.Pop();
            
            _exitBtn.transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete += () =>
            {
                _exitBtn.transform.DORewind();
                
                gameObject.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete += () =>
                {
                    gameObject.SetActive(false);
                    
                };
            };
        });
    }
    
    
    
}
