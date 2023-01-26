using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCreator : MonoBehaviour
{
    // Obstacle Tiles 3개 - 체크 포인트, 몬스터, 가시
    // Height Tiles 3(5)개 - 노트타일, 왼쪽 경사, 오른쪽 경사, (뒤 두개는 선택사항) 왼쪽 경사 아래 타일, 오른쪽 경사 아래 타일
    // 위 선택은 타일 없을 시 지우면 됨
    [SerializeField] private Tile[] _obstacleTiles;
    [SerializeField] private Tile[] _heightTiles;
    [SerializeField] private Tilemap _tileMap;
    // 타일 벡터
    private Vector3Int _instVec = Vector3Int.zero;
    // 이벤트 오브젝트 벡터
    private Vector3Int _obstacleVec;
    // 점프 이벤트 리스트
    private List<Vector3Int> _tileList = new List<Vector3Int>();

    public enum HeightType
    {
        Normal,
        Up,
        Down
    }
    
    // # : 체크 포인트
    // . : 일반 타일
    // ^ : 점프 타일(스파이크)
    // 1,2 : 피격횟수가 1,2인 적
    // + : 다음 타일 y좌표가 올라가는 점프 타일
    // - : 다음 타일 y좌표가 내려가는 점프 타일
    /* 타일 string 작성 규칙
    1. 마지막 타일은 몬스터 타일 만들지 말기
    * 임시는 일단 신경쓰지 않아도 됨
    (임시) 2번 쳐야하는 몬스터라면 밀리는 것 계산해서 배치
    (임시) 점프 타일이면 이동하는 것 생각해서 배치
    */
    private string _obstacles = ".... .... .... ...." +
                                ".... .... .... ....";

    private string _height = ".... .... .... ...." +
                             ".... .... .... ....";

    /*private string _obstacles = "#... .... ..1. ..1. ..1. ..1." +
                                "#.^. ..^. ..^. ..^^" +
                                "#.-- ..11 ..-- ..1." +
                                ".... .... .... ....";

    private string _height = ".... .... .... .... .... ...." +
                             ".... .... .... ...." +
                             ".... .... .... ...." +
                             ".c.. .c.. .C.. CCCC";*/
    
    private void Start()
    {
        _obstacles = _obstacles.Replace(" ", string.Empty);
        _height = _height.Replace(" ", string.Empty);
        _instVec.x = -1;
        MakeObstacles();
    }
    
    // 노트 생성 (이벤트 오브젝트)
    private void MakeObstacles()
    {
        for (int i = 0; i < _obstacles.Length; i++)
        {
            _instVec.x++;

            switch (_obstacles[i])
            {
                case '#':
                    _obstacleVec = _instVec;
                    _obstacleVec.y++;
                    _tileMap.SetTile(_obstacleVec, _obstacleTiles[0]);
                    MakeTiles(_height[i]);
                    break;
                case '.':
                    MakeTiles(_height[i]);
                    break;
                case '1':
                    _obstacleVec = _instVec;
                    _obstacleVec.x++;
                    _obstacleVec.y++;
                    MakeTiles(_height[i]);
                    _tileMap.SetTile(_obstacleVec, _obstacleTiles[1]);
                    break;
                case '^':
                    MakeTiles(_height[i]);
                    _tileList.Add(_instVec);
                    MakeTiles(HeightType.Normal);
                    break;
                case '+':
                    MakeTiles(_height[i]);
                    _tileList.Add(_instVec);
                    MakeTiles(HeightType.Up);
                    break;
                case '-':
                    MakeTiles(_height[i]);
                    _tileList.Add(_instVec);
                    MakeTiles(HeightType.Down);
                    break;
            }
        }
    }

    
    private void MakeTiles(HeightType type)
    {
        _instVec.x++;

        switch (type)
        {
            case HeightType.Up:
                _instVec.y++;
                _tileMap.SetTile(_instVec, _obstacleTiles[2]);
                break;
            case HeightType.Down:
                _instVec.y--;
                _tileMap.SetTile(_instVec, _obstacleTiles[2]);
                break;
            case HeightType.Normal:
                _tileMap.SetTile(_instVec, _obstacleTiles[2]);
                break;
        }
    }

    private void MakeTiles(char heightChar)
    {
        switch (heightChar)
        {
            case '.':
                _tileMap.SetTile(_instVec, _heightTiles[0]);
                break;
            case 'C':
                _tileMap.SetTile(_instVec, _heightTiles[3]);
                _instVec.y++;
                _tileMap.SetTile(_instVec, _heightTiles[1]);
                break;
            case 'c':
                _tileMap.SetTile(_instVec, _heightTiles[2]);
                _instVec.y--;
                _tileMap.SetTile(_instVec, _heightTiles[4]);
                break;
        }
    }
}
