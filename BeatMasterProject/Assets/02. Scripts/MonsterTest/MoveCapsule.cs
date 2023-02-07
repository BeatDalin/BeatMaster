// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using UnityEngine;
//
// public class MoveCapsule : MonoBehaviour
// {
//     public float moveSpeed = 0f;
//
//     [SerializeField] private BoxCollider2D _boxCollider;
//
//     [SerializeField] private MonsterPooling _monsterPooling;
//
//     [SerializeField] private Transform[] _checkPoint;
//
//     [SerializeField] private SpriteRenderer _spriteRenderer;
//
//     public Transform rewindPos;
//
//     private bool isPressed;
//
//     public int _checkPointIdx = 0;
//     // Start is called before the first frame update
//     void Start()
//     {
//         rewindPos = _checkPoint[0];
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         Move();
//         CheckPoint();
//         InputKey();
//     }
//
//     private void Move()
//     {
//         transform.position += Vector3.right * Time.deltaTime * moveSpeed;
//     }
//
//     private void CheckPoint()
//     {
//         if (transform.position.x >= _checkPoint[_checkPointIdx].position.x) //플레이어x값이 다음체크포인트보다 크다면 체크포인트를 넘은것으로 간주
//         {
//             rewindPos.position = _checkPoint[_checkPointIdx].position;
//             _checkPointIdx++;
//             //_monsterPooling.ResetPool(); //체크포인트를 넘겼으므로 저장된 몬스터들은 오브젝트풀로 돌려도됨
//         }
//     }
//
//     private void InputKey()
//     {
//         if (Input.GetKey(KeyCode.Space))
//         {
//             _boxCollider.enabled = true;
//         }
//         else
//         {
//             _boxCollider.enabled = false;
//         }
//
//         if (Input.GetKeyDown(KeyCode.R)) //R키누르면 마지막 체크포인트 위치로 이동 및 몬스터 다시 켜주기
//         {
//             transform.position = rewindPos.position;
//             _monsterPooling.ReArrange();
//         }
//     }
//
//     private void OnCollisionEnter2D(Collision2D col)
//     {
//         if (col.gameObject.name.Equals("Monster(Clone)"))
//         {
//             Debug.Log("몬스터한테 맞음");
//
//             _spriteRenderer.DOFade(0, 0.1f).onComplete += () =>
//             {
//                 _spriteRenderer.DOFade(1, 0.1f);
//             };
//
//             col.gameObject.SetActive(false);
//             _monsterPooling.disableMonsterQueue.Enqueue(col.gameObject);
//         }
//     }
// }
