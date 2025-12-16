using UnityEditor.Rendering.Universal;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MazeGenerator : MonoBehaviour
{
    const float TILE_SIZE = 0.64f;

    enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT
    }

    enum TileType : byte
    {
        WALL = 0,
        GROUND = 1,
        EXIT = 2
    }

    [SerializeField] private Sprite groundTile;
    [SerializeField] private Sprite wallTile;
    [SerializeField] private Sprite targetTile;

    [Range(6, 24)]
    [SerializeField] private byte mazeSize = 8;

    private byte _limit;
    private System.Random _rand = new();
    private readonly Direction[] _directions = 
        new Direction[]{Direction.UP, Direction.DOWN, Direction.LEFT, Direction.RIGHT};

    private void Start()
    {
        _limit = (byte)(mazeSize * 2 + 1);
        byte[][] layout = new byte[_limit][];
        for(int i = 0; i < _limit; ++i)
        {
            layout[i] = new byte[_limit];
            for (int k = 0; k < _limit; k++)
            {
                layout[i][k] = (byte)TileType.WALL;
            }
        }

        Vector2Int start = new(1, 1);
        BacktrackingLayoutGenerator(start, _limit, layout);

        // Ensure spawn point generation
        layout[1][0] = (byte)TileType.WALL;
        layout[1][1] = (byte)TileType.GROUND;

        // Create exit
        layout[_limit - 2][_limit - 2] = (byte)TileType.GROUND;
        layout[_limit - 2][_limit - 1] = (byte)TileType.EXIT;

        layout = RandomizeLayoutPosition(layout);

        GenerateTiles(layout);
	}

    private void BacktrackingLayoutGenerator(Vector2Int pos, byte limit, byte[][] layout)
    {
        ShuffleDirections(_directions);
        layout[pos.x][pos.y] = (byte)TileType.GROUND;
        foreach(Direction dir in _directions)
        {
            Vector2Int nextPos = pos;
            switch(dir)
            {
                case Direction.UP:
                {
                    nextPos.y -= 2;
                    break;
                }
                case Direction.DOWN:
                {
                    nextPos.y += 2;
                    break;
                }
                case Direction.LEFT:
                {
                    nextPos.x -= 2;
					break;
                }
                case Direction.RIGHT:
                {
                    nextPos.x += 2;
					break;
				}
                default: break;
            }

            if (IsInside(nextPos) && layout[nextPos.x][nextPos.y] == (byte)TileType.WALL)
            {
                layout[(pos.x + nextPos.x) / 2][(pos.y + nextPos.y) / 2] = (byte)TileType.GROUND;
                BacktrackingLayoutGenerator(nextPos, limit, layout);
            }
        }
    }

    private void GenerateTiles(in byte[][] layout)
    {
        for (int x = 0; x < layout.Length; ++x)
        {
            for (int y = 0; y < layout[x].Length; ++y)
            {
                GameObject tileGO = new();
                tileGO.transform.SetParent(gameObject.transform);
                SpriteRenderer tileSprite = tileGO.AddComponent<SpriteRenderer>();
                if (layout[x][y] == (byte)TileType.WALL)
                {
                    BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(0.64f, 0.64f);
                    box.isTrigger = false;
                    tileSprite.sprite = wallTile;
                    tileGO.layer = LayerMask.NameToLayer("Wall");
                }
                else if (layout[x][y] == (byte)TileType.EXIT)
                {
                    BoxCollider2D box = tileGO.AddComponent<BoxCollider2D>();
                    box.size = new Vector2(0.64f, 0.64f);
                    box.isTrigger = true;
                    tileSprite.sprite = targetTile;
                }
                else
                {
                    tileSprite.sprite = groundTile;
                }
                tileGO.transform.position = new Vector3(x * TILE_SIZE, y * TILE_SIZE, 0);
            }
        }
    }

    private byte[][] RandomizeLayoutPosition(in byte[][] layout)
    {
        byte[][] newLayout = new byte[_limit][];
        for(int i = 0; i < _limit; ++i)
        {
            newLayout[i] = new byte[_limit];
        }
        
        byte randomOption = (byte)_rand.Next(0, 2);
        Debug.Log($"Layout randomization ID: {randomOption}");
        switch (randomOption)
        {
            case 0: return layout;
            case 1:
            {
                for(int i = 0; i < _limit; ++i)
                {
                    newLayout[i] = new byte[_limit];
                    for (int k = 0; k < _limit; k++)
                    {
                        newLayout[i][k] = layout[k][i];
                    }
                }
                break;
            }
            default: return layout;
        }

        return newLayout;
    }

    private void ShuffleDirections(Direction[] dirs)
    {
        for (int i = dirs.Length - 1; i > 0; i--)
        {
            int k = _rand.Next(i + 1);
            (dirs[i], dirs[k]) = (dirs[k], dirs[i]);
        }
    }

    private bool IsInside(Vector2Int v) => v.x > 0 && v.x < _limit - 1 && v.y > 0 && v.y < _limit - 1;
}
