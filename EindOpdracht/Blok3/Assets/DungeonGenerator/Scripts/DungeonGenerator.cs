using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tile {
    None = 0,
    Floor = 1,
}

public class DungeonGenerator : MonoBehaviour {

    public int width, height, numRooms;
    public Vector2 roomWidth, roomHeight;
    public float scaleFactor;
    private List<Room> allRooms = new List<Room>();
    private GameObject wall, door, floor;
    private Tile[,] dungeon;
    private List<GameObject> allGameObjectsInDungeon = new List<GameObject>();

    // Use this for initialization
    private void Start() {
        //if(allGameObjectsInDungeon == null || allGameObjectsInDungeon.Count == 0) {
        //   GenerateDungeon();
        //}
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ClearDungeon();
            GenerateDungeon();
        }
    }

    public void GenerateDungeon() {
        ClearDungeon();
        InitDungeon();
        GenerateRooms();
        GenerateHallWays();
        AddExtraRoomTiles();
        CreatePillarsInRooms();
        SpawnDungeon();
        PlacePlayerInDungeon();
    }

    private void PlacePlayerInDungeon() {
        //Put Player in Dungeon
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) {
            player.transform.position = new Vector3(allRooms[0].x, 0f, allRooms[0].y) * scaleFactor;
        }
    }

    private void InitDungeon() {
        wall = Resources.Load<GameObject>("Wall");
        door = Resources.Load<GameObject>("Door");
        floor = Resources.Load<GameObject>("Floor");
        dungeon = new Tile[width, height];
    }

    private void AddExtraRoomTiles() {
        //this function will resize the rooms so that we don't get super long doors when a hallway runs next to a room
        foreach (Room room in allRooms) {
            DoActionForPartOfGrid(room.x - 1, room.y - 1, room.w + 2, room.h + 2, (x, y) => {
                if (!IsPositionInsideRoom(x, y, room) && dungeon[x, y] == Tile.Floor) {
                    room.AddTile(x, y);
                }

            });
        }
    }

    private void CreatePillarsInRooms() {
        foreach (Room r in allRooms) {
            DoActionForPartOfGrid(r.x + 1, r.y + 1, r.w - 2, r.h - 2, (x, y) => {
                if (Random.value < 0.25f)
                    dungeon[x, y] = Tile.None;
            }
            );

        }
    }

    private void GenerateRooms() {
        int tries = 0;
        while (allRooms.Count < numRooms && tries < numRooms * 2) {
            int x = Random.Range(1, width - (int)roomWidth.y - 2);
            int y = Random.Range(1, height - (int)roomHeight.y - 2);

            //make sure a room always has an odd width and height (3x3, 5x5 etc), this leads to nicer hallway entries
            int roomSizeWidth = Random.Range((int)roomWidth.x, (int)roomWidth.y + 1);
            roomSizeWidth = roomSizeWidth % 2 == 0 ? roomSizeWidth - 1 : roomSizeWidth;
            int roomSizeHeight = Random.Range((int)roomHeight.x, (int)roomHeight.y + 1);
            roomSizeHeight = roomSizeHeight % 2 == 0 ? roomSizeHeight - 1 : roomSizeHeight;

            Room room = new Room(x, y, roomSizeWidth, roomSizeHeight);

            //Check For overlapping rooms
            if (CheckGridSpaceClear(room)) {
                DoActionForPartOfGrid(x, y, roomSizeWidth, roomSizeHeight, (i, j) => { dungeon[i, j] = Tile.Floor; });
                allRooms.Add(room);
            }
            tries++;
        }
    }

    private void GenerateHallWays() {
        //Generate a HallWay from each room to another;
        for (int i = 0; i < allRooms.Count; i++) {
            Room currentRoom = allRooms[i];
            Room connectedRoom = allRooms[(i + 1) % allRooms.Count];
            GeneratePathWay(currentRoom, connectedRoom);
        }
    }

    private void GeneratePathWay(Room r1, Room r2) {
        //Generate Horizontal PathWay
        Room LeftMostRoom = r1.GetMiddleOfRoom.x <= r2.GetMiddleOfRoom.x ? r1 : r2;
        Room RightMostRoom = LeftMostRoom == r1 ? r2 : r1;

        DoActionForPartOfGrid(
            (int)LeftMostRoom.GetMiddleOfRoom.x,
            (int)LeftMostRoom.GetMiddleOfRoom.y,
            (int)RightMostRoom.GetMiddleOfRoom.x - (int)LeftMostRoom.GetMiddleOfRoom.x + 1,
            1,
            (i, j) => {
                dungeon[i, j] = Tile.Floor;
            }
            );
        //Generate Vertical PathWay
        Room LowerRoom = r1.y <= r2.y ? r1 : r2;
        Room UpperRoom = LowerRoom == r1 ? r2 : r1;
        DoActionForPartOfGrid(
            (int)LeftMostRoom.GetMiddleOfRoom.x + (int)RightMostRoom.GetMiddleOfRoom.x - (int)LeftMostRoom.GetMiddleOfRoom.x,
            (int)LowerRoom.GetMiddleOfRoom.y,
            1,
            (int)UpperRoom.GetMiddleOfRoom.y - (int)LowerRoom.GetMiddleOfRoom.y + 1,
            (i, j) => {
                dungeon[i, j] = Tile.Floor;
            }
            );
    }

    private void SpawnDungeon() {
        //Spawn Floors for each Floor tile
        DoActionForPartOfGrid(0, 0, width, height,
            (x, y) => {
                if (dungeon[x, y] == Tile.Floor) {
                    SpawnObjectForDungeon(floor, new Vector3(x, 0, y), Quaternion.identity);
                }
            }
        );

        //Spawn Walls
        //Look at upper, lower, left and right neighbour, if it is a floor, spawn a wall
        DoActionForPartOfGrid(0, 0, width, height,
        (x, y) => {
            if (dungeon[x, y] == Tile.None) {
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && dungeon[i, j] == Tile.Floor) {
                            SpawnObjectForDungeon(wall,
                                new Vector3(x, 0, y),
                                Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)));
                        }
                    }
                    );
            }
        }
        );

        //Spawn Doors
        //Doors can only be spawned to connect a hallway with a room, so we don't have to check the whole grid, only the rooms
        foreach (Room r in allRooms) {
            DoActionForPartOfGrid(r.x, r.y, r.w, r.h,
                (x, y) => {
                    //Look at neighbours, if a neighbour is a floor and is not in the room, spawn a door
                    DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                        (i, j) => {

                            if ((x == i || y == j) && dungeon[x, y] == Tile.Floor && dungeon[i, j] == Tile.Floor && !IsPositionInsideRoom(i, j, r)) {
                                SpawnObjectForDungeon(door, new Vector3(x, 0, y), Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)));
                            }
                        }
                        );
                });

            foreach (Vector2 v in r.extraTiles) {
                int x = (int)v.x;
                int y = (int)v.y;
                DoActionForPartOfGrid(x - 1, y - 1, 3, 3,
                    (i, j) => {
                        if ((x == i || y == j) && dungeon[x, y] == Tile.Floor && dungeon[i, j] == Tile.Floor && !IsPositionInsideRoom(i, j, r)) {
                            SpawnObjectForDungeon(door, new Vector3(x, 0, y), Quaternion.LookRotation(new Vector3(i, 0, j) - new Vector3(x, 0, y)));
                        }
                    }
                    );
            }
        }
    }

    private int GetAmountNeighbours(int x, int y, Tile type, bool checkDiagonal = true) {
        //Count neighbours, exclude self
        int count = 0;
        DoActionForPartOfGrid(x - 1, y - 1, 3, 3, (i, j) => {
            if (((!checkDiagonal && (x == i || y == j)) || checkDiagonal) && !(x == i && y == j) && dungeon[i, j] == type) {
                count++;
            }
        }
        );
        return count;
    }

    bool IsPositionInsideRoom(int x, int y, Room r) {
        foreach (Vector2 v in r.extraTiles) {
            if (v.x == x && v.y == y) { return true; }
        }

        return (x >= r.x && x < r.x + r.w && y >= r.y && y < r.y + r.h);
    }

    private GameObject SpawnObjectForDungeon(GameObject obj, Vector3 position, Quaternion rotation) {
        //Use this to scale spawned objects
        GameObject g = Instantiate(obj, position * scaleFactor, rotation);
        g.transform.localScale *= scaleFactor;
        g.transform.SetParent(transform, true);
        allGameObjectsInDungeon.Add(g);
        return g;
    }

    public void ClearDungeon() {
        for (int i = 0; i < allGameObjectsInDungeon.Count; i++) {
            DestroyImmediate(allGameObjectsInDungeon[i]);
        }
        allGameObjectsInDungeon.Clear();
        allRooms.Clear();
        dungeon = new Tile[width, height];
    }

    private bool CheckGridSpaceClear(Room room) {
        //we do not want adjacent rooms, so we check for one space greater than the size of the room

        for (int x = room.x - 1; x < room.x + room.w + 1; x++) {
            for (int y = room.y - 1; y < room.y + room.h + 1; y++) {
                if (x < 0 || y < 0 || x >= width || y >= height) { return false; }

                if (dungeon[x, y] == Tile.Floor) {
                    return false;
                }
            }
        }

        return true;
    }

    private void DoActionForPartOfGrid(int x, int y, int w, int h, System.Action<int, int> action) {
        for (int xx = x; xx < x + w; xx++) {
            for (int yy = y; yy < y + h; yy++) {
                if (xx < 0 || yy < 0 || xx >= width || yy >= height) {
                    continue;
                }
                action.Invoke(xx, yy);
            }
        }
    }
}


public class Room {
    public int x, y, w, h;
    public List<Vector2> extraTiles;
    public Room(int x, int y, int w, int h) {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
        extraTiles = new List<Vector2>();
    }

    public Vector2 GetMiddleOfRoom {
        get { return new Vector2(x + w / 2, y + h / 2); }
    }

    public void AddTile(int x, int y) {
        extraTiles.Add(new Vector2(x, y));
    }
}