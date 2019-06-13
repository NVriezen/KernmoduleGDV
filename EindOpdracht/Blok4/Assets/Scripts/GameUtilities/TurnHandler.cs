using System.Collections.Generic;

public class TurnHandler
{
    public uint currentActivePlayer = 0;
    public uint totalTurnsPassed = 1;
    public List<uint> playersToSkip = new List<uint>();

    public void UpdatePlayerTurn(List<uint> playersList)
    {        
        int nextPlayerIndex = (playersList.IndexOf(currentActivePlayer) + 1) % playersList.Count;
        currentActivePlayer = playersList[nextPlayerIndex];
        if (nextPlayerIndex == 0)
        {
            totalTurnsPassed += 1;
        }
        if (playersToSkip.Contains(currentActivePlayer))
        {
            playersToSkip.Remove(currentActivePlayer);
            UpdatePlayerTurn(playersList);
        }
    }
}
