const util = require('util');

/**
 * Subset of the GameSession data type.
 * @typedef {Object} GameSession
 * @property {timestamp} CreationTime - Time stamp indicating when this data object was created. Format is a number
 * expressed in Unix time as milliseconds (for example "1469498468.057").
 * @property {Array} GameProperties - Set of custom properties for a game session, formatted as key:value pairs.
 * @property {string} GameSessionData - Set of custom game session properties, formatted as a single string value.
 * @property {string} Name - A descriptive label that is associated with a game session. Session names do not need
 * to be unique.
 * @see [AWS documentation]{@link https://docs.aws.amazon.com/gamelift/latest/apireference/API_GameSession.html} for a
 * complete list of all properties.
 */

/**
 * GameMessage data type.
 * @typedef {Object} GameMessage
 * @property {int} opCode - Opcode
 * @property {string} payload - Payload/content of the message
 * @property {string} sender - Sender (sending Player) of the message.
 * @property {boolean} reliable - Flag indicating that the message was sent via TCP and not UDP.
 */

/**
 * Subset of the Player data type.
 * @typedef {Object} Player
 * @property {string} peerId
 * @see [AWS documentation]{@link https://docs.aws.amazon.com/gamelift/latest/apireference/API_Player.html} for a
 * complete list of all properties.
 */

/**
 * Game mode of this script instance.
 * @type {'SSPS'|'M3'}
 */
let GAME;
const maxPlayers = 2;
let players = [];
let playerReady = Array(maxPlayers).fill(false);
let playerSet = Array(maxPlayers).fill(false);
let logicalPlayerIDs = {};
let session = null;
let sessionTimeoutTimer = null;
const SESSION_TIMEOUT = 60 * 1000;  // 1 minute to wait for players to join

const sessionStates = {
    INIT: 0, READY: 1, SET: 2, GO: 3, GAME_RUNS: 4, GAME_OVER:5, REMATCH_OFFER: 6
}
let currentSessionState = sessionStates.INIT;

// server op codes (messages the server sends)
const SERVER_OP_CODES = {
    PLAYER_ACCEPTED: 0,
    ACK_PLAYER_DISCONNECT: 1,
    OPPONENT_WANTS_REMATCH: 5,
    GET_READY: 10,
    FIRST_TURN: 20,
    GAME_OVER: 40,
    OPPONENT_DISCONNECTED: 300,
    BAD_REQUEST: 400,
    INTERNAL_SERVER_ERROR: 500
}
// client op codes (messages the clients send)
const CLIENT_OP_CODES = {
    REQUESTING_REMATCH: 5,
    I_AM_READY: 10,
    MY_SPECIAL_PAWNS: 12
}

///////////////////////////////////////////////////////////////////////////////
// Utility functions
///////////////////////////////////////////////////////////////////////////////

/**
 * Sends a string message with an opcode to the specified recipients.
 * @param {Array<string>|string} peerIds - Recipient(s)
 * @param {int} opCode - Opcode
 * @param {string} stringToSend="" - String to send. Note that the strings will be Base64 encoded, so they can't contain
 * colon, comma or double quote.
 */
function sendStringToClient(peerIds, opCode, stringToSend = "") {

    peerIds = [].concat(peerIds); // making sure its an array
    log(GAME + " SendStringToClient: peerIds = " + peerIds.toString() + " opCode = " + opCode + " stringToSend = " + stringToSend);

    let gameMessage = session.newTextGameMessage(opCode, session.getServerId(), stringToSend);
    let peerArrayLen = peerIds.length;

    for (let index = 0; index < peerArrayLen; ++index) {
        log(GAME + " SendStringToClient: " + gameMessage.toString() + " " + peerIds[index].toString());
        session.sendReliableMessage(gameMessage, peerIds[index]);
    }
}

const logTypes = {INFO:'info', WARNING:'warning', WARN:'warn', ERROR:'error'}
function log(msg, type = logTypes.INFO) {

    let l = session.getLogger()
    switch (type) {
        case logTypes.INFO:  l.info(msg);  break;
        case logTypes.WARNING:
        case logTypes.WARN:  l.warn(msg);  break;
        case logTypes.ERROR: l.error(msg); break;
    }

}

///////////////////////////////////////////////////////////////////////////////
// Game code, logic is outsourced
///////////////////////////////////////////////////////////////////////////////


function askForReadiness() {
    currentSessionState = sessionStates.READY;

    players.forEach(player => sendStringToClient(
        player,
        SERVER_OP_CODES.GET_READY,
        logicalPlayerIDs[ player ]
    ));
}

/**
 * Relay the given messages to one or both players.
 * @param {[GameMessage|null,GameMessage|null]} gameMessages Two fielded array.
 * First field contains the message to the first player, the second field the message for the second player.
 *
 * @todo check syntax after implementing processMessage()
 */
function relayMessagesToClients(gameMessages) {
    if (gameMessages[0] !== null && gameMessages[0] === gameMessages[1])
        sendStringToClient(players, gameMessages[0].opCode, gameMessages[0].payload);
    else
        gameMessages.forEach((msg, index) =>
            sendStringToClient(players[index], msg.opCode, msg.payload))
}

function stopGame() {

    log("stopGame() – clearing game session");
    currentSessionState = sessionStates.GAME_OVER;
    // clear readiness
    playerReady.fill(false);
}

function stopSession() {
    if(session != null)
    {
        // processEnding will stop this instance of the game running
        // and will tell the game session to terminate
        session.processEnding().then(function(outcome) {
            log("Completed process ending with: " + outcome);
            process.exit(0);
        });
    }
}

/**
 * Processes the received game message through the game logic and triggers reply messages if expected.
 * @param {GameMessage} gameMessage - GameMessage object to process.
 *
 * @todo implement
 * @returns {null}
 */
function processMessage(gameMessage) {
    log("Ready do use game logic of " + GAME)
    // todo: import endpoint of the GameLogic scripts
    // todo: check syntax in processManager() that uses this function's return value.
    // return GameLogic.onMessage(logicalPlayerIDs[gameMessage.sender], gameMessage)
    log("Function processMessage(gameMessage) not yet implemented.", logTypes.WARNING)
    return null
}

function broadcastFirstTurn(firstPlayerNr) {
    players.forEach(player => sendStringToClient(
        player,
        SERVER_OP_CODES.FIRST_TURN,
        firstPlayerNr.toString()
    ));
}

/**
 * Saves the scores of the player into a database.
 * @param {int} scorePlayer0 End score of player 1.
 * @param {int} scorePlayer1 End score of player 2.
 *
 * @todo implement database interface
 */
function saveScoresToDatabase(scorePlayer0, scorePlayer1) {
    log("The called function saveScoresToDatabase() is not implemented yet.", logTypes.WARNING);
}

///////////////////////////////////////////////////////////////////////////////
// App callbacks
///////////////////////////////////////////////////////////////////////////////

/**
 * Called when game server is initialized.
 * @param {GameSession} rtSession
 */
function init(rtSession) {

    session = rtSession;
    GAME = session.Name;
    log("init(rtSession): ");
    log(util.inspect(rtSession));
}

/**
 * Processes incoming game messages.
 * @param {GameMessage} gameMessage - GameMessage object to process
 */
function onMessage(gameMessage) {

    log(GAME + " onMessage(gameMessage): ");
    log(util.inspect(gameMessage));
    if (!gameMessage.reliable) log("Message has been delivered via UDP.", logTypes.WARNING)

    // sender 0 is server
    if (gameMessage.sender != 0) {
        let logicalSender = logicalPlayerIDs[gameMessage.sender];
        const opCode = gameMessage.opCode; // guaranteed to be set

        switch (currentSessionState) {
            case sessionStates.INIT:
                sendStringToClient(gameMessage.sender, SERVER_OP_CODES.BAD_REQUEST,
                    "Bad robot! You send a message before the server asked you to.");
                log("Invalid message by a client with op-code " + opCode);
                break;

            case sessionStates.READY:
                if (opCode === CLIENT_OP_CODES.READY) {
                    playerReady[logicalSender] = true;
                    let gameLogicResponse = processMessage(gameMessage);

                    if (playerReady.includes(false) && (gameLogicResponse === null || gameLogicResponse === [])) {
                        // nothing to do
                    }
                    else if (! playerReady.includes(false) && gameLogicResponse !== null) {
                        // < both player ready >
                        currentSessionState = sessionStates.SET
                        relayMessagesToClients(gameLogicResponse);
                    } else {
                        // this spot should be unreachable
                        log("Hitting unreachable code.", logTypes.ERROR);
                    }
                }
                else {
                    sendStringToClient(gameMessage.sender, SERVER_OP_CODES.BAD_REQUEST,
                        "Bad robot! The server asked whether you're ready, therefore expecting op-code 10. Instead you sent op-code"+gameMessage.opCode+".");
                }
                break;

            case sessionStates.SET:
                // timeout check?
                if (opCode === CLIENT_OP_CODES.MY_SPECIAL_PAWNS)
                    playerSet[logicalSender] = true;
                    if (!playerSet.includes(false)) {
                        // < both player set >
                        relayMessagesToClients(processMessage(gameMessage));
                        currentSessionState = sessionStates.GO;
                        // == Go! ==
                        broadcastFirstTurn(1);
                        currentSessionState = sessionStates.GAME_RUNS;
                    }
                else {
                    sendStringToClient(gameMessage.sender, SERVER_OP_CODES.BAD_REQUEST,
                        "Bad robot! The server asked you to choose your special pawns, expecting op-code 12. Instead you sent op-code"+gameMessage.opCode+".");
                    log("Received bad request from a client.", logTypes.WARNING);
                }
                break;

            case sessionStates.GO:
                sendStringToClient(gameMessage.sender, SERVER_OP_CODES.BAD_REQUEST,
                    "Shut up bad robot! The server did not ask you to talk to him.");
                log("Received bad request from a client.", logTypes.WARNING);
                break;

            case sessionStates.GAME_RUNS:
                let gameLogicResponse = processMessage(gameMessage);
                relayMessagesToClients(gameLogicResponse);

                if (gameLogicResponse !== null && gameLogicResponse[0].opCode === SERVER_OP_CODES.GAME_OVER) {
                    // < game over >
                    stopGame();
                    // todo: read actual scores from the GameLogic's response
                    saveScoresToDatabase(0, 0);
                }
                break;

            case sessionStates.GAME_OVER:
                if (opCode === CLIENT_OP_CODES.REQUESTING_REMATCH) {
                    playerReady[logicalSender] = true;
                    if (playerReady[0] && playerReady[1]) {
                        // continue with state READY
                        askForReadiness();
                    }
                    // else wait for the second player
                }
                else {
                    sendStringToClient(gameMessage.sender, SERVER_OP_CODES.BAD_REQUEST,
                        "Bad robot! The server offers a rematch and accepts only op-code 5. Instead you sent op-code"+gameMessage.opCode+".");
                    log("Received bad request from a client.", logTypes.WARNING);
                }
                break;

            default:
                // should be unreachable
                log("The server's variable sessionState has a invalid content.", logTypes.ERROR);
                sendStringToClient(gameMessage.sender, SERVER_OP_CODES.INTERNAL_SERVER_ERROR,
                    "Internal Server Error");
        }
    }
}

/**
 * Is called when a player has passed initial validation.
 * @param {{peerId: int, payload: string}} connectMessage - ignored
 * @return {boolean} Whether the player may connect.
  */
function onPlayerConnect(connectMessage) {

    /* Not needed anymore as lambda-script and init-function set game property
    const correctGame = ["SSPS", "M3"].includes(connectMessage.payload)
    log(''+correctGame.peerId + " onPlayerConnect: " + connectMessage,
        correctGame? logTypes.INFO : logTypes.ERROR)
    if (correctGame)
        GAME = connectMessage.payload
    else
        return false // refuse connection
    */

    log("onPlayerConnect: " + connectMessage)
    // once a player connects it's fine to let the game session keep going
    // it will be killed once any client disconnects
    if(sessionTimeoutTimer != null)
    {
        clearTimeout(sessionTimeoutTimer);
        sessionTimeoutTimer = null;
    }

    return players.length < maxPlayers;
}

/**
 * onPlayerAccepted is called when a player has connected and not rejected
 * by onPlayerConnect. At this point it's possible to broadcast to the player
 * @param {Player} player - Player object of the accepted player
 */
function onPlayerAccepted(player) {
    const peerId = player.peerId;

    log(GAME + " onPlayerAccepted: player.peerId = " + peerId);
    // store the ID. Note that the index the player is assigned will be sent
    // to the client and determines if they are "player 0" or "player 1" independent
    // of the peerId
    players.push(peerId);
    log(GAME + " onPlayerAccepted: new contents of players array = " + players.toString());

    let logicalID = players.length - 1;
    log(GAME + " onPlayerAccepted: logical ID = " + logicalID);

    logicalPlayerIDs[peerId] = logicalID;
    log(GAME + " onPlayerAccepted: logicalPlayerIDs array = " + logicalPlayerIDs.toString());

    sendStringToClient(peerId, SERVER_OP_CODES.PLAYER_ACCEPTED, logicalID.toString());

    if (players.length === maxPlayers) askForReadiness();
}

/**
 * On Player Disconnect is called when a player has left or been forcibly terminated
 * Is only called players that actually connect to the server and not those rejected by validation
 * This is called before the player is removed from the player list
 * @param {string} peerId
 */
function onPlayerDisconnect(peerId) {

    log("onPlayerDisconnect: Player #" + logicalPlayerIDs[peerId] + " (" + peerId + ") disconnected.");
    stopGame();
    const opponentPeerId = (logicalPlayerIDs[peerId] === 0)? players[1] : players[0];
    sendStringToClient(opponentPeerId, SERVER_OP_CODES.OPPONENT_DISCONNECTED,
        "Your opponent left the game.");
    stopSession();
}

/**
 * On Process Started is called when the process has begun and we need to perform any
 * bootstrapping.  This is where the developer should insert any necessary code to prepare
 * the process to be able to host a game session.
 * Return true if the process has been appropriately prepared and it is okay to invoke the
 * GameLift ProcessReady() call.
 * @return {boolean}
 */
function onProcessStarted() {

    log("Starting process...");
    log("Ready to host games...");
    return true;
}

/**
 * On Start Game Session is called when GameLift creates a game session that runs this server script.
 * A Game Session is one instance of your game actually running. Each instance will have its
 * own instance of this script.
 * @param gameSession
 */
function onStartGameSession(gameSession) {

    log(GAME + " onStartGameSession");
    // The game session is started by the client service Lambda function
    // If no player joins, we want to kill the game session after
    // a certain period of time so it doesn't hang around forever taking up
    // a game instance.
    sessionTimeoutTimer = setTimeout(stopSession, SESSION_TIMEOUT);
}

exports.ssExports = {
    configuration: {},
    init: init,
    onMessage: onMessage,
    onPlayerConnect: onPlayerConnect,
    onPlayerDisconnect: onPlayerDisconnect,
    onProcessStarted: onProcessStarted,
    onPlayerAccepted: onPlayerAccepted,
    onStartGameSession: onStartGameSession
};