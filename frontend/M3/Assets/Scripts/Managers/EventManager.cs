public static class EventManager
{
    public static StateChangeEvent StateChange = new StateChangeEvent();

    public static MatchEvent Match = new MatchEvent();
    public static InsertDoneEvent InsertDone = new InsertDoneEvent();
    public static PointChangeEvent PointChange = new PointChangeEvent();
    public static MyTurnEvent MyTurn = new MyTurnEvent();
    public static TimeOverEvent TimerOver = new TimeOverEvent();

    // Networking (opcode at the end)
    public static PlayerAcceptedEvent PlayerAccepted = new PlayerAcceptedEvent();           //1
    public static DisconnectEvent Disconect = new DisconnectEvent();                        //2
    public static PlayAgainEvent playAgain = new PlayAgainEvent();                          //5 sender: Player
    public static OpponentPlayAgainEvent OponentPlayAgain = new OpponentPlayAgainEvent();   //5
    public static PlayerReadyEvent PlayerReady = new PlayerReadyEvent();                    //10 sender: Player
    public static BoardConfigEvent BoardConfig = new BoardConfigEvent();                    //11
    public static PlayerHasTurn PlayerHasTurn = new PlayerHasTurn();                        //20
    public static MoveEvent Move = new MoveEvent();                                         //30 sender: Player
    public static OpponentMoveEvent OpponentMove = new OpponentMoveEvent();                 //30
    public static GameOverEvent GameOver = new GameOverEvent();                             //40
    public static OpponentDisconnectEvent OpponentDisconect = new OpponentDisconnectEvent();//300
    public static InvalidRequestEvent InvalidRequest = new InvalidRequestEvent();           //311
    public static InvalidMoveEvent InvalidMove = new InvalidMoveEvent();                    //320
}
