// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
//
// public class Hit : MonoBehaviour
// {
//     [SerializeField] private MonsterPooling _monsterPooling;
//
//     private void OnTriggerEnter2D(Collider2D col)
//     {
//         if (col.name.Equals("Monster(Clone)"))
//         {
//             Debug.Log("몬스터 때림");
//             
//             if (col.gameObject.GetComponent<Monster>().isGainCoin)
//             {
//                 col.transform.GetChild(0).GetComponent<Animator>().SetBool("Hit", true);
//                 col.gameObject.GetComponent<Monster>().isGainCoin = false;
//                 StartCoroutine(CheckAnimation(col.gameObject));
//             }
//             else
//             {
//                 col.gameObject.SetActive(false);
//             }
//             _monsterPooling._disableMonsterList.Enqueue(col.gameObject);
//         }
//     }
//
//     IEnumerator CheckAnimation(GameObject obj)
//     {
//         while (obj.transform.GetChild(0).GetComponent<Animation>().IsPlaying("Coin2"))
//         {
//             yield return null;
//         }
//
//         obj.transform.GetChild(0).GetComponent<Animator>().enabled = false;
//         obj.transform.GetChild(0).gameObject.SetActive(false);
//         obj.SetActive(false);
//     }
// }
