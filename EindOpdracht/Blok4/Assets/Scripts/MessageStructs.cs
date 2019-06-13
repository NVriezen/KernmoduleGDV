using System.Collections.Generic;

public struct PlayerConnectStruct : IMessageStruct
{
    public uint playerID;
}

public struct RequestOpponentStruct : IMessageStruct
{
    public uint playerID;
}

public struct AssignOpponentStruct : IMessageStruct
{
    public uint matchID;
}

public struct StartGameStruct : IMessageStruct
{
    public uint playingFieldID;
    public uint amountOfPlayers;
    public List<uint> playerInfoID;
    public List<uint> playerInfoCharacter;
}

public struct UIStateUpdateStruct : IMessageStruct
{
    //send Lives
    //send total turns
    public uint totalTurns;
    public uint amountOfPlayers;
    public List<uint> playerLives;
}

public struct PlayerPositionUpdateStruct: IMessageStruct
{
    public uint playerID;
    public uint amountOfData;
    public List<UnityEngine.Vector3> walkablePath;
}

public struct PlayerNamesStruct : IMessageStruct
{
    public List<string> playerNames;
}

public struct TurnUpdateStruct : IMessageStruct
{
    public uint totalTurns;
    public uint playerID;
}

public struct PlayerRespawnStruct : IMessageStruct
{
    public uint playerID;
    public float positionX;
    public float positionY;
    public float positionZ;
}

public struct BombSpawnStruct : IMessageStruct
{
    public uint bombID;
    public UnityEngine.Vector3 bombPosition;
}

public struct PlayerPlaceBombStruct : IMessageStruct
{
    public uint playerID;
}

public struct PlayerEndTurnStruct : IMessageStruct
{
    public uint playerID;
}

public struct WalkableFieldUpdateStruct : IMessageStruct
{
    public UnityEngine.Vector3 centerPosition;
    public uint listLength;
    public List<UnityEngine.Vector3> walkablePositions;
}

public struct PlayerMoveStruct : IMessageStruct
{
    public uint playerID;
    public UnityEngine.Vector3 newPosition;
}

public struct BombExplodeStruct : IMessageStruct
{
    public uint bombID;
    public uint amountOfData;
    public List<UnityEngine.Vector3> flamePositions;
}

public struct GameOverStruct : IMessageStruct
{
    public uint playerWon;
}

public struct CrateDestroyStruct : IMessageStruct
{
    public uint amountOfData;
    public List<UnityEngine.Vector3> cratesToDestroy;
}


