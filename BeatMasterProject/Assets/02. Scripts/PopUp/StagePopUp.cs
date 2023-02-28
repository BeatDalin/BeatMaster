using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class StagePopUp : MonoBehaviour
{
    [SerializeField] private Button _exitBtn;
    [SerializeField] private Button _stageBtn;

    private void Start()
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
        
        _stageBtn.onClick.AddListener(() =>
        {
            DOTween.Init();
            // Load LevelMenuScene
           SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        });
    }
}
