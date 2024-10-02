// 7x6
// 12 units a 4 for ech weapon
// 2 spezial unnits
// first get spezail pawns than broadcast board
"use strict";

const allowedLetters = ["a", "b", "c", "d", "e", "f"];
const allowedPawns = ["scissors", "rock", "leaf", "trap", "king", "enemy"];


const Player = class {
    constructor() {

        this.ID = null;
        this.Board = {
            "a": new Array(7),
            "b": new Array(7),
            "c": new Array(7),
            "d": new Array(7),
            "e": new Array(7),
            "f": new Array(7),
        };
        this.redy = false;
        this.duelPos = new Array();
    }
};

let players = [];

// server op codes (messages server sends)
const INITIAL_GAMEBOARD = 11;
const ALL_PLAYERS_REDY_SPEZIAL_PAWNS = 12;
const PLAYER_HAS_TURN = 20;
const ITS_YOUR_TURN = 30;
const STRIKE_RESULTS = 35;
const DUELL = 36;
const GAME_OVER = 40;
const NOT_THE_CORRECT_DATA = 311;
const TURN_WAS_INCORRECT = 320;


// client op codes (messages client sends)
const I_AM_REDY = 10;
const SPEZIAL_PAWNS = 12;
const MY_TURN = 30;
const MY_STRIKE = 35; //nötig ????
const DUELL_WAPONE_CHOOSE = 36;

/**
 *
 * @param playerId the logical Id for the Player given by the GameManager
 * @param gameMessage the message with opcode and payload
 * @returns the defined response message with opcode and payload or null for no response
 */
function onMessage(playerId, gameMessage) {


    switch (gameMessage.opCode) {
        case I_AM_REDY:

            if (players.length === 0) {
                let player0 = new Player();
                let player1 = new Player();
                players.push(player0);
                players.push(player1);
            }

            players[playerId]["redy"] = true;
            // have both players signaled they are ready? If so, ready to go
            if (players[0].redy && players[1].redy) {
                let messagneArray = [];
                for (let i = 0; i < 2; i++) {
                    messagneArray.push(messagePaser(ALL_PLAYERS_REDY_SPEZIAL_PAWNS));
                }
                return messagneArray;
            } else {
                return null;
            }

            break;

        case SPEZIAL_PAWNS :
            return fillBoard(playerId, gameMessage["payload"]["flag"], gameMessage["payload"]["trap"]);
            break;

        case MY_TURN:
            return doTurn(playerId, gameMessage.payload);
            break;

        case MY_STRIKE:
            return doTurn(playerId, gameMessage.payload);
            break;

        case DUELL_WAPONE_CHOOSE:
            return duell(playerId, gameMessage.payload);
            break;

        default:
            return messagePaser(NOT_THE_CORRECT_DATA, "opcode does not exist");
    }
    ;

}

/**
 *
 * @param opCode the opcode for the Message
 * @param stringToSend the optional payload of the message
 * @returns {gameMessage} the message with opcode and payload
 */
function messagePaser(opCode, stringToSend = "") {
    let gameMessage = ({});
    gameMessage.opCode = opCode;


    gameMessage.payload = JSON.stringify(stringToSend);

    return (gameMessage);
}

/**
 *
 * @param position an array with the position of an figure
 *
 * this function turns the gamefield for the opponent
 */
function swapBoard(position) {
    switch (position) {

        case "a":
            position = "f";
            return position;
            break;

        case "b":
            position = "e";
            return position;
            break;

        case "c":
            position = "d";
            return position;
            break;

        case "d":
            position = "c";
            return position;
            break;

        case "e":
            position = "b";
            return position;
            break;

        case "f":
            position = "a";
            return position;
            break;

        default :
            break;
    }

}

/**
 *
 * @param playerId
 * @param position
 * @param move
 * @param enemyUnit
 * @param swapedPosition
 * @param swapedMove
 * @param playerUnit
 * @returns {[]}
 *
 * if the player loses it erase the figure from the board
 */
function playerLoses(playerId, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit) {

    let result = [];
    players[playerId]["Board"][position[0]][position[1]] = null;
    players[1- playerId]["Board"][swapedPosition[0]][swapedPosition[1]] = null;



    result[playerId] = messagePaser(STRIKE_RESULTS,{"pos": [position[0],position[1]],"move": move,"Unit": enemyUnit});
    result[1- playerId ] = messagePaser(STRIKE_RESULTS, {"pos": [swapedPosition[0],swapedPosition[1]],"move": swapedMove,"Unit" : playerUnit});

    return result;
}

/**
 *
 * @param playerId
 * @param enemyPawnPosition
 * @param newPosition
 * @param position
 * @param move
 * @param enemyUnit
 * @param swapedPosition
 * @param swapedMove
 * @param playerUnit
 * @returns {[]}
 *
 * If the player wins the strike this function changes the position on both boards
 */
function playerWins(playerId, enemyPawnPosition, newPosition, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit) {
    let result = [];
    players[1- playerId]["Board"][enemyPawnPosition[0]][enemyPawnPosition[1]] = "enemy";
    players[1- playerId]["Board"][swapedPosition[0]][swapedPosition[1]] = null;

    players[playerId]["Board"][newPosition[0]][newPosition[1]] = players[playerId]["Board"][position[0]][position[1]];
    players[playerId]["Board"][position[0]][position[1]] = null;


    result[playerId] = messagePaser(STRIKE_RESULTS,{"pos": [position[0],position[1]],"move": move,"Unit": enemyUnit});
    result[1- playerId ] = messagePaser(STRIKE_RESULTS, {"pos": [swapedPosition[0],swapedPosition[1]],"move": swapedMove,"Unit" : playerUnit});

    return result;
}

/**
 *
 * @param playerId id of the current player
 * @param newPosition target position
 * @param position current position
 * @param move current move
 * @returns {string|[]} array with the strike results for every player sorted by his id
 */
function doStrike(playerId, newPosition, position, move) {

    let enemyPawnPosition = [];
    enemyPawnPosition[0] = swapBoard(newPosition[0]);
    enemyPawnPosition[1] = 6 - newPosition[1];

    let swapedPosition = [];
    swapedPosition[0] = swapBoard(position[0]);
    swapedPosition[1] = 6 - position[1];


    let swapedMove = swapMove(move);

    let playerUnit = players[playerId]["Board"][position[0]][position[1]];
    let enemyUnit = players[1 - playerId ]["Board"][enemyPawnPosition[0]][enemyPawnPosition[1]];

    // is duel ?

    if (playerUnit === enemyUnit) {
        let result = [];
        players[playerId]["duelPos"] = position;
        players[playerId]["move"] = move;
        players[playerId]["duel"]=true;

        players[1-playerId]["duelPos"] = enemyPawnPosition;
        players[1-playerId]["move"] = "no move";
        players[1-playerId]["duel"]=true;

        result[playerId] = messagePaser(DUELL, {"pos":[position[0],position[1]],"move":move,"Unit":enemyUnit});
        result[1- playerId ] = messagePaser(DUELL, {"pos":[swapedPosition[0],swapedPosition[1]],"move":swapedMove,"Unit":playerUnit});

        return result;
    }


    if (enemyUnit === "king") {
        let result = [];

        result[playerId] = messagePaser(GAME_OVER, "You win");
        result[1-playerId] = messagePaser(GAME_OVER, "You lose");

        return result;
    }

    if (enemyUnit === "trap") {

        return playerLoses(playerId, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
    }


    // SSP logic
    if (playerUnit === "rock") {

        if (enemyUnit === "leaf") {

            return playerLoses(playerId, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
        if (enemyUnit === "scissors") {

            return playerWins(playerId, enemyPawnPosition, newPosition, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
    }

    if (playerUnit === "leaf") {

        if (enemyUnit === "scissors") {

            return playerLoses(playerId, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
        if (enemyUnit === "rock") {

            return playerWins(playerId, enemyPawnPosition, newPosition, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
    }
    if (playerUnit === "scissors") {

        if (enemyUnit === "rock") {

            return playerLoses(playerId, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
        if (enemyUnit === "leaf") {

            return playerWins(playerId, enemyPawnPosition, newPosition, position, move, enemyUnit, swapedPosition, swapedMove, playerUnit);
        }
    }
    // should never happen
    return "something went wrong"

}

/**
 *
 * @param playerID current player id
 * @param weapon the new wapon
 * @returns {gameMessage} new turn
 */
function duell(playerID, weapon) {

    if (allowedPawns.includes(weapon)) {
        let position = players[playerID]["duelPos"];

        players[playerID]["Board"][position[0]][position[1]] = weapon;
        players[playerID]["duel"]=false;

        if (!players[playerID]["duel"]  &&  !players[1-playerID]["duel"] ) {

            if (players[playerID]["move"] !== "no move") {
                let turn = players[playerID]["duelPos"][0] + players[playerID]["duelPos"][1] + players[playerID]["move"];
                return doTurn(playerID, turn);
            } else {
                let turn = players[1-playerID ]["duelPos"][0] + players[1-playerID]["duelPos"][1] + players[1-playerID]["move"];
                return doTurn(1-playerID, turn);
            }

        }


    }

}

/**
 *
 * @param move the move from the player
 * @returns {*} the move seen by the enemy
 */
function swapMove(move) {
    if (move === "up") {
        move = "down";
    }
    if (move === "right") {
        move = "left";
    }
    if (move === "left") {
        move = "right";
    }
    if(move ==="down"){
        move = "up";
    }
    return move;
}

/**
 *
 * @param playerId the id of the current player
 * @param turn his turn with position and move inside
 * @returns {gameMessage} the new position of the figure for the oponent
 */
function doTurn(playerId, turn) {


    let position = positionPaser(turn.slice(0, 2));

    let move = turn.slice(2);

    if (position === NOT_THE_CORRECT_DATA) {
        return messagePaser(NOT_THE_CORRECT_DATA);
    }

    let newPosition = checkRules(playerId, position, move);

    if (newPosition === TURN_WAS_INCORRECT) {
        return messagePaser(TURN_WAS_INCORRECT);
    }

    if (players[playerId]["Board"][newPosition[0]][newPosition[1]] === "enemy") {
        return doStrike(playerId, newPosition, position, move);
    }


    // change the position of the figure in the player board
    players[playerId]["Board"][newPosition[0]][newPosition[1]] = players[playerId]["Board"][position[0]][position[1]];
    players[playerId]["Board"][position[0]][position[1]] = null;


    // change the position to the look from the opponent

   let oldEnemyPosition = swapBoard(position[0]);
   let newEnemyPosition = swapBoard(newPosition[0]);

    // change position of the opponent board
    players[1- playerId ]["Board"][newEnemyPosition][6 - newPosition[1]] = players[playerId % 1]["Board"][oldEnemyPosition][6- position[1]];
    players[1- playerId ]["Board"][oldEnemyPosition][6 - position[1]] = null;

    // changes the move for the opponent
    move = swapMove(move);
   let enemyMove =[];
   enemyMove[0]= oldEnemyPosition;
   enemyMove[1]= 6- position[1];

    let newTurn = {"pos":[enemyMove[0],enemyMove[1]], "move": move};
    return messagePaser(ITS_YOUR_TURN, newTurn)

}

/**
 *
 * @param playerId the id of the current player
 * @param position the position of the figure
 * @param move the direction of the turn
 * @returns {[]|number} the new position of the figure if the move was correct
 */
function checkRules(playerId, position, move) {

    let newPosition = new Array();
    let row = position[0];
    switch (move) {
        case  "up":
            if (row === "f") {
                return TURN_WAS_INCORRECT;
            } else {

                let index = allowedLetters.indexOf(row) + 1;
                newPosition = [allowedLetters[index], position[1]];
            }
            break;

        case "right" :
            if (position[1] > 6) {
                return TURN_WAS_INCORRECT;
                break;
            } else {
                newPosition = [position[0], position[1] + 1];
            }
            break;

        case "left" :
            if (position[1] < 1) {
                return TURN_WAS_INCORRECT;
                break;
            } else {
                newPosition = [position[0], position[1] - 1];
                break;
            }

        case "down" :
            if (row === "a") {
                return TURN_WAS_INCORRECT;
            } else {
                let index = allowedLetters.indexOf(row) - 1;
                newPosition = [allowedLetters[index], position[1]];
            }
            break;

        default :
            return TURN_WAS_INCORRECT;
    }

    if (!players[playerId]["Board"][newPosition[0]][newPosition[1]] || players[playerId]["Board"][newPosition[0]][newPosition[1]] === "enemy") {
        return newPosition;
    } else {
        return TURN_WAS_INCORRECT;
    }

}


/**
 *
 * @param player the current player object for these game there are two different players
 * @param king the position for the special pawn king
 * @param trap the position of the special pawn trap
 *
 * the function creates the individual Board an game start for every player
 */

function fillBoard(playerId, king, trap) {


    let kingPos = positionPaser(king);
    let trapPos = positionPaser(trap);

    if (kingPos === NOT_THE_CORRECT_DATA) {
        return messagePaser(NOT_THE_CORRECT_DATA);
    }
    if (trapPos === NOT_THE_CORRECT_DATA) {
        return messagePaser(NOT_THE_CORRECT_DATA);
    }

    let pawns = [];

    for (let i = 0; i < 4; i++) {
        pawns.push("scissors", "rock", "leaf");
    }

    shuffleArray(pawns);

    players[playerId]["Board"][kingPos[0]][kingPos[1]] = "king";
    players[playerId]["Board"][trapPos[0]][trapPos[1]] = "trap";


    for (let i = 0; i < 7; i++) {

        players[playerId]["Board"]["e"][i] = "enemy";
        players[playerId]["Board"]["f"][i] = "enemy";

        if (players[playerId]["Board"]["a"][i] === "king" || players[playerId]["Board"]["a"][i] === "trap") {
        } else {
            players[playerId]["Board"]["a"][i] = pawns.pop();
        }
        if (players[playerId]["Board"]["b"][i] === "king" || players[playerId]["Board"]["b"][i] === "trap") {
        } else {
            players[playerId]["Board"]["b"][i] = pawns.pop();
        }
    }

    return messagePaser(INITIAL_GAMEBOARD, players[playerId]["Board"]);
}

/**
 *
 * @param move is a string with row and collum like a5
 * @returns {[string, number]} as an array with a number and a string for the position or if the data was not correct
 * an error opcode
 *
 * it Also checks if the given move has the correct data
 */
function positionPaser(move) {


    let row = move.slice(0, 1);
    let collum = parseInt(move.slice(1, 2));

    if (collum < 6 && collum >= 0 && allowedLetters.includes(row)) {

        let move = [row, collum];
        return move;
    } else {
        return NOT_THE_CORRECT_DATA;
    }

}


/**
 *
 * @param array contains all pawns
 *
 * shuffles the array
 */

function shuffleArray(array) {
    for (let i = array.length - 1; i > 0; i--) {
        const j = Math.floor(Math.random() * (i + 1));
        [array[i], array[j]] = [array[j], array[i]];
    }
}

/**
 *
 * a function just to test the strike
 */

function testBoart() {
    if (players.length === 0) {
        let player0 = new Player();
        let player1 = new Player();
        players.push(player0);
        players.push(player1);
    }

    players[0].Board["c"][1] = "rock";
    players[0].Board["d"][1] = "enemy";
    players[1].Board["c"][5] = "scissors";
    players[1].Board["d"][5] = "enemy";

    players[0].Board["c"][2] = "scissors";
    players[0].Board["d"][2] = "enemy";
    players[1].Board["c"][4] = "scissors";
    players[1].Board["d"][4] = "enemy";

    players[0].Board["c"][3] = "rock";
    players[0].Board["d"][3] = "enemy";
    players[1].Board["c"][3] = "king";
    players[1].Board["d"][3] = "enemy";

    players[0].Board["c"][0] = "rock";
    players[0].Board["d"][0] = "enemy";
    players[1].Board["c"][6] = "trap";
    players[1].Board["d"][6] = "enemy";


}

