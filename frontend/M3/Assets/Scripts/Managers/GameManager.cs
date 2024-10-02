using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Player Me;
    public Player Opponent;
    private GameEnums.State _state;
    [SerializeField] Timer Timer;

    #region Singelton
    private static GameManager _instance;
    private GameManager() { }
    public static GameManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    #endregion

    void Start()
    {
        Me = new Player("Du");
        Me.MyTurn();
        Opponent = new Player("Gegner");
        HandleEvents();
    }

    private void Update()
    {
        
    }

    private void ChangeState(GameEnums.State newState)
    {
        GameEnums.State oldState = _state;
        _state = newState;
        EventManager.StateChange.Invoke(new StateChangeEventData(_state, oldState));
    }

    public GameEnums.State GetState()
    {
        return _state;
    }

    void HandleEvents()
    {
        EventManager.OpponentMove.AddListener(HandleOpponentMove);
        EventManager.InsertDone.AddListener(HandleInsertDone);
        EventManager.OpponentDisconect.AddListener(HandleOpponentDisconnect);
        EventManager.InvalidRequest.AddListener(HandleError);
        EventManager.InvalidMove.AddListener(HandleError);
        EventManager.Match.AddListener(HandleMatch);
        EventManager.Move.AddListener(HandleMove);
        EventManager.MyTurn.AddListener(HandleMyTurn);
    }


    #region Event Handler
    private void HandleMyTurn()
    {
        Me.MyTurn();
        ChangeState(GameEnums.State.myTurn);
        Timer.StartTimer();
    } 
    private void HandleMove(MoveEventData data)
    {
        Timer.StopTimer();
        ChangeState(GameEnums.State.pause);
    } 
    private void HandleOpponentMove(OpponentMoveEventData data)
    {
        ChangeState(GameEnums.State.pause);
    } 
    private void HandleError(string message)
    {
        Debug.LogError(message);
        ChangeState(GameEnums.State.error);
    } 
    private void HandleOpponentDisconnect(string message)
    {
        Debug.LogWarning(message);
        ChangeState(GameEnums.State.gameEnd);
    } 
    private void HandleMatch(MatchEventData data)
    {
        if (Me.IsMyTurn())
        {
            Me.AddPoints(data.points);
            EventManager.PointChange.Invoke(new PointChangeEventData(data.points, Me));
        }
        else if (Opponent.IsMyTurn())
        {
            Opponent.AddPoints(data.points);
            EventManager.PointChange.Invoke(new PointChangeEventData(data.points, Opponent));
        }
        else
        {
            ChangeState(GameEnums.State.error);
            Debug.LogError("Error: no one´s turn!!");
        }
    }
    private void HandleInsertDone()
    {
        if (Me.IsMyTurn())
        {
            Me.TurnOver();
            Opponent.MyTurn();
            ChangeState(GameEnums.State.opponentTurn);
        }
        else if (Opponent.IsMyTurn())
        {
            Opponent.TurnOver();
            EventManager.MyTurn.Invoke();
        }
        else
        {
            ChangeState(GameEnums.State.error);
            Debug.LogError("Error: no one´s turn!!");
        }
    }
    #endregion

}

