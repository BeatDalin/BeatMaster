using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColliderTest : MonoBehaviour
{
    [Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    
    [Header("Item")]
    [SerializeField] private GameObject _starObj;
    [SerializeField] private Transform _itemContainer;
    [SerializeField] private GameObject _checkPointObj;
    [SerializeField] private List<Animator> _checkPointAnim = new List<Animator>();

    [Header("Tile")]
    [SerializeField] private Tilemap _itemTilemap;
    [SerializeField] private List<Tile> _interactionTileList = new List<Tile>();
    [SerializeField] private List<AnimatedTile> _animatedTileList = new List<AnimatedTile>();
    private int _tileX = -1, _tileY;

    private static readonly int IsPlay = Animator.StringToHash("isPlay");


    public KoreographyTrack newTrack;
    void Awake()
    {
        CreateItemTile();
    }

    private void CreateItem(float tileX, float tileY)
    {
        var item = Instantiate(_starObj, new Vector3(tileX, tileY, 0), Quaternion.identity);
        item.transform.SetParent(_itemContainer);
    }

    private void CreateItemTile()
    {
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();

            int groundType = tileData[0];
            int groundYDelta = tileData[1];
            int actionType = tileData[2];
            int checkPoint = tileData[3];
            
            _tileX += 1;
            _tileY += groundYDelta;

            if (actionType != 0)
            {
                // USING tilemap -> item 처리 어떻게 할지 고민
                // var cell = new Vector3Int(_tileX + 1, _tileY + 2);
                // _itemTilemap.SetTile(cell, _animatedTileList[0]);

                CreateItem(_tileX + 1, _tileY + 2.5f);
                if (groundType != 3)
                {
                    if (i != _mapEventList.Count - 1 &&
                        _mapEventList[i + 1].GetTextValue().Split().Select(int.Parse).ToArray()[0] != 3)
                    {
                        // tile 위치 조건문 처리?
                        _itemTilemap.SetTile(new Vector3Int(_tileX + 1, _tileY, 0), _interactionTileList[0]);
                    }
                }
            }

            if (checkPoint != 0)
            {
                var effect = Instantiate(_checkPointObj, new Vector3Int(_tileX, _tileY+1, 0), Quaternion.identity);
                // effect.transform.SetParent(transform);
                _checkPointAnim.Add(effect.GetComponent<Animator>());
            }
        }
    }

    public void PlayCheckAnim(int idx)
    {
        Debug.Log("Change Anim State");
        _checkPointAnim[idx].SetTrigger(IsPlay);
    }
}
