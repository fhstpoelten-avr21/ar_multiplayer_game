public override void OnJoinedRoom()
{
    if (PhotonNetwork.CurrentRoom.PlayerCount == 2) 
    {
        // Starten Sie das Spiel, wenn 2 Spieler im Raum sind
        Debug.Log("2 Spieler im Raum, starte Spiel");
    }
}

