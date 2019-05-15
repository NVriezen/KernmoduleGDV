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

public struct PlayerRespawnStruct : IMessageStruct
{
    public uint playerID;
    public float positionX;
    public float positionY;
    public float positionZ;
}


