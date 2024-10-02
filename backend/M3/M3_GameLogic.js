/**
 * === Initial Content ===
 */

// Players etc.
let Players = [];
let Board = [];
let cardRotation = [];
let Points = [10, 20, 30, 40, 70, 100, 150];
const POINT_DIFFERENCE = 300;
const MOVE_LETTERS = 'LTR';
const MOVE_NUMBERS = '01234';
const BOARD_SIZE = 5;

class Player {
    constructor() {
        this.points = 0;
        this.ready = false;
    }
}

/**
 * === OP-Codes ===
 */
// Server OP-Codes
const PLAYER_NUMBER_DISTRIBUTION = 10;
const BOARD_CONFIG = 11;
const PLAYER_X_HAS_TURN = 20;
const PLAYER_GETS_TURN = 30;
const NEW_CARD = 31;
const GAME_OVER = 40;
const PLAYER_DISCONNECTS = 300;
const DATA_INCORRECT = 311;
const TURN_INCORRECT = 320;

// Client OP-Codes
const PLAYER_READY = 10;
const PLAYER_MOVE = 30;

/**
 * === GAME CODE ===
 */

/**
 * This function listens on incoming calls and handles them by opCode
 * @param playerId logical playerId (from 0 to 1)
 * @param gameMessage the message containing the opCode and the payload that is sent
 */
function evaluateMessage(playerId, gameMessage) {

    switch (gameMessage.opCode) {
        case PLAYER_READY:

            if (Players.length === 0) {
                for (let i = 0; i < 2; i++) {
                    let player = new Player();
                    Players.push(player);
                }
            }

            Players[playerId].ready = true;

            if (Board.length === 0 && Players.every(player => player.ready)) {
                initializeBoard();
                let messages = [
                    [],
                    []
                ];

                const initialConfig = {
                    board: Board,
                    wheel: cardRotation
                };

                const startingPlayer = Math.floor(Math.random() * Math.floor(2));

                messages[0].push(messageParser(BOARD_CONFIG, initialConfig));
                messages[0].push(messageParser(PLAYER_X_HAS_TURN, {playerId: startingPlayer}));
                messages[1].push(messageParser(BOARD_CONFIG, initialConfig));
                messages[1].push(messageParser(PLAYER_X_HAS_TURN, {playerId: startingPlayer}));

                return messages;
            }
            break;
        case PLAYER_MOVE:
            return playerMakesMove(playerId, gameMessage.payload);
        default:
            return messageParser(DATA_INCORRECT, {error: "No matching opCode"});
    }

}

/**
 * Fills the board with random numbers from 0 to 6. Also fills the first 3 items of the rotating selection.
 */
function initializeBoard() {

    while (true) {
        for (let i = 0; i < BOARD_SIZE; i++) {

            let row = [];

            for (let j = 0; j < BOARD_SIZE; j++) {
                row.push(Math.floor(Math.random() * Math.floor(Points.length)));
            }

            Board.push(row);
        }

        for (let i = 0; i < 3; i++) {
            cardRotation.push(Math.floor(Math.random() * Math.floor(Points.length)));
        }

        if (!checkInitialMatches()) {
            break;
        }

    }

    return Board;

}

/**
 * THis function checks for any matches at the initialization of the Board. If there is a match the board will be
 * cleared.
 */
function checkInitialMatches() {

    for (let i = 0; i < Board.length; i++) {
        if (horizontalCheck(999, i) || verticalCheck(999, i)) {
            Board = [];
            return true;
        }
    }

    return false;

}

/**
 * This function decides whether the player's move is valid and from which direction the new card has to be inserted.
 * @param playerId contains the id of the player that submitted their move
 * @param move contains the string that defines the move ("T2" means insert from top into column 2, "L4" means insert
 *             from the left into row 4
 * @returns either an error if the move was not valid or a message for the upcoming player with the needed data
 */
function playerMakesMove(playerId, move) {

    if (move === '') {
        let messages = [
            [],
            []
        ];
        messages[1 - playerId].push(messageParser(PLAYER_GETS_TURN, {prevMove: move, newCard: cardRotation[2]}));
        messages[playerId].push(messageParser(NEW_CARD, {newCard: cardRotation[2]}));

        return messages;
    }

    if (validateMoveFormat(move)) {
        switch (move[0]) {
            case 'L':
                leftMove(Number(move[1]));
                break;
            case 'T':
                topMove(Number(move[1]));
                break;
            case 'R':
                rightMove(Number(move[1]));
                break;
        }
        rotateWheel();
        checkMatches(playerId)
        let messages = [
            [],
            []
        ];
        messages[1 - playerId].push(messageParser(PLAYER_GETS_TURN, {prevMove: move, newCard: cardRotation[2]}));
        messages[playerId].push(messageParser(NEW_CARD, {newCard: cardRotation[2]}));
        const winnerMessage = isPlayerWinner(playerId);
        if (winnerMessage !== null) {
            messages[0].push(winnerMessage);
            messages[1].push(winnerMessage);
        }
        return messages;
    } else {
        let messages = [
            [],
            []
        ]
        return messages[playerId].push(messageParser(DATA_INCORRECT, {error: "Wrong format for action: move"}));
    }

}

/**
 * THis function validates the move string for the right format and data. It is expected to start with a letter of
 * "L, T, R" followed by a number from 0 to 6.
 * @param move contains the string the player sends
 * @returns {boolean} that confirms or declines the validation of the move
 */
function validateMoveFormat(move) {
    return MOVE_LETTERS.includes(move[0]) && MOVE_NUMBERS.includes(move[1]);
}

/**
 * This function calculates an insertion on the left side of the board. If the row is full every card is getting moved
 * one field to the right. If there are spaces between cards, they will be filled in this calculation.
 * @param row contains the row that the insertion has to happen
 */
function leftMove(row) {

    let prevItem = cardRotation[0]

    for (let column = 0; column < 5; column++) {
        const currentItem = prevItem;
        prevItem = Board[row][column];

        Board[row][column] = currentItem;

        if (row < 4) {
            if (Board[row + 1][column] === -1) {
                fallDown(row, column, currentItem);
                break;
            }
        }
        if (prevItem === -1) break;

    }

}

/**
 * This function calculates an insertion on the right side of the board. If the row is full every card is getting moved
 * one field to the left. If there are spaces between cards, they will be filled in this calculation.
 * @param row contains the row that the insertion has to happen
 */
function rightMove(row) {

    let prevItem = cardRotation[0];

    for (let column = 4; column >= 0; column--) {
        const currentItem = prevItem;
        prevItem = Board[row][column];

        Board[row][column] = currentItem;

        if (row < 4) {
            if (Board[row + 1][column] === -1) {
                fallDown(row, column, currentItem);
                break;
            }
        }
        if (prevItem === -1) break;

    }

}

/**
 * This function calculates an insertion on the top of the board. If the column is full every card is getting moved
 * one field to the bottom. If there is no card underneath the inserted card, it will drop down the next card or the
 * bottom of the Board.
 * @param column contains the column that the insertion has to happen
 */
function topMove(column) {

    let prevItem = cardRotation[0];

    if (Board[0][column] === -1) {
        fallDown(0, column, prevItem);
        return;
    }

    for (let row = 0; row < 5; row++) {
        const currentItem = prevItem;
        prevItem = Board[row][column];

        Board[row][column] = currentItem;
    }
}

/**
 * fallDown calculates a card that has to fall down onto another card or to the bottom of the Board.
 * @param row contains the row in which the card has been inserted. This is mainly used to save time and because the
 *            field above the card do not have to be looked at.
 * @param column contains the column in which the fall has to happen.
 * @param value contains the value of the card that has to fall down.
 */
function fallDown(row, column, value) {

    for (let i = row + 1; i < 5; i++) {
        if (Board[i][column] !== -1) {
            Board[row][column] = -1;
            Board[i - 1][column] = value;
            return;
        }
    }

    Board[row][column] = -1;
    Board[4][column] = value;
}

/**
 * This function moves all entries of the card wheel one to the left, to be updated. Also generates a new number
 * from 0 to 6 and puts them onto the back of the wheel.
 */
function rotateWheel() {
    for (let i = 0; i < 3; i++) {
        if (i === 2) {
            cardRotation[i] = Math.floor(Math.random() * Math.floor(7));
        } else {
            cardRotation[i] = cardRotation[i + 1];
        }
    }
}

/**
 * This function checks for matches on the board after a move has been executed and calls the point calculation for a
 * player.
 * @param playerId contains the logical playerId of the player that send the move
 */
function checkMatches(playerId) {

    let matchFound = true;

    while (matchFound) {
        for (let row = 0; row < Board.length; row++) {
            matchFound = horizontalCheck(playerId, row);
            if (matchFound) break;
        }

        if (!matchFound) {
            for (let column = 0; column < Board.length; column++) {
                matchFound = verticalCheck(playerId, column);
                if (matchFound) break;
            }
        }
    }

    return Board;
}

/**
 * This function checks for horizontal matches in a row by looking for the longest streak of the same card.
 * @param playerId contains the playerId of the player taking the turn
 * @param row contains the number of the row that has to be checked
 */
function horizontalCheck(playerId, row) {

    let highestStreak = 0;
    let highestStreakStart = 0;
    let streak = 0;
    let streakStart = 0;

    for (let i = 0; i < Board.length - 1; i++) {
        if (Board[row][i] === Board[row][i + 1] && Board[row][i] !== -1) {
            streak++;
        } else {
            streak = 0;
            streakStart = i + 1;
        }
        if (streak > highestStreak) {
            highestStreak = streak;
            highestStreakStart = streakStart;
        }
    }

    if (highestStreak >= 2) {
        if(playerId !== 999) {
            calculatePoints(playerId, highestStreak, Board[row][highestStreakStart]);
        }
        for (let i = highestStreakStart; i <= highestStreakStart + highestStreak; i++) {
            Board[row][i] = -1;
        }
        fallDownAfterRowMatch(row);
        return true;
    }

    return false;

}

/**
 * This function checks for vertical matches in a column by looking for the longest streak of the same card.
 * @param playerId contains the playerId of the player taking the turn
 * @param column contains the number of the column that has to be checked
 */
function verticalCheck(playerId, column) {

    let highestStreak = 0;
    let highestStreakStart = 0;
    let streak = 0;
    let streakStart = 0;

    for (let i = 0; i < Board.length - 1; i++) {
        if (Board[i][column] === Board[i + 1][column] && Board[i][column] !== -1) {
            streak++;
        } else {
            streak = 0;
            streakStart = i + 1;
        }
        if (streak > highestStreak) {
            highestStreak = streak;
            highestStreakStart = streakStart;
        }
    }

    if (highestStreak >= 2) {
        if (playerId !== 999) {
            calculatePoints(playerId, highestStreak, Board[highestStreakStart][column]);
        }
        for (let i = highestStreakStart; i <= highestStreakStart + highestStreak; i++) {
            Board[i][column] = -1;
        }
        fallDownAfterColumnMatch(column);
        return true;
    }

    return false;

}

/**
 * This function calculates the falling cards after a match has been found in a row.
 * @param row contains the row in which the match was found
 */
function fallDownAfterRowMatch(row) {
    for (let i = 0; i < Board.length; i++) {
        if (Board[row][i] === -1) {
            for (let j = row; j >= 0; j--) {
                if (j === 0) {
                    Board[j][i] = -1;
                } else {
                    Board[j][i] = Board[j - 1][i];
                }
            }
        }
    }
}

/**
 * This function calculates the falling cards after a match has been found in a column.
 * @param column contains the column in which the match was found
 */
function fallDownAfterColumnMatch(column) {
    let leftOverCards = [];

    for (let i = Board.length - 1; i >= 0; i--) {
        if (Board[i][column] !== -1) {
            leftOverCards.push(Board[i][column]);
        }
    }
    for (let i = Board.length - 1; i >= 0; i--) {
        if (i < Board.length - leftOverCards.length) {
            Board[i][column] = -1;
        } else {
            Board[i][column] = leftOverCards[Board.length - 1 - i];
        }
    }
}

/**
 * This function calculates the points that have to be added to one players points after cards have been matched.
 * @param playerId contains the playerId of the player that gets the points
 * @param streak contains the length of the streak of cards that have been matched
 * @param value contains the value of the cards have been matched
 */
function calculatePoints(playerId, streak, value) {
    switch (streak) {
        case 2:
            Players[playerId].points += Points[value];
            break;
        case 3:
            Players[playerId].points += 2 * Points[value];
            break;
        case 4:
            Players[playerId].points += 3 * Points[value];
            break;
    }
}

/**
 * This function decides whether the point difference is great enough to end the game.
 * @param playerId contains the playerId of the player that took the last turn
 * @returns {{}|null} is a winners message or null if there is no winner yet
 */
function isPlayerWinner(playerId) {

    if (Players[playerId].points - Players[1 - playerId].points >= POINT_DIFFERENCE) {
        return messageParser(GAME_OVER, {playerId: playerId});
    }

    return null;

}

/**
 * THis function parses the objects that have been calculated into a JSON message with an opCode.
 * @param opCode contains the opCode that has to be attached to the message for clarity and action handling
 * @param payload contains the actual data that has to be sent to the clients
 * @returns message as a completed JSON string for easy access afterwards
 */
function messageParser(opCode, payload) {
    let message = ({});

    message.opCode = opCode;
    message.payload = JSON.stringify(payload);

    return message;

}

exports.ssExports = {
    evaluateMessage: evaluateMessage
};

/**
 * === TEST FUNCTIONS ===
 */

/**
 * Initializes a hard coded Board and card wheel for testing purposes.
 */
function initializeBoardWithGaps() {
    Board = [
        [-1, -1, -1, -1, -1],
        [-1, -1, -1, -1, -1],
        [3, 3, -1, -1, 3],
        [2, 2, -1, 2, 2],
        [1, 1, 0, 1, 1]
    ]
    cardRotation = [4, 5, 6];
}

/**
 * This function generates a Board with a config that has some "almost" matches. Put the first card in slot L2 or T4
 * to generate a triple match of 4's and a quintuple match of 2's afterwards by using slot L2.
 */
function initializeBoardWithAlmostMatch() {
    Board = [
        [-1, -1, -1, -1, 2],
        [4, -1, -1, -1, 3],
        /*-->*/ [0, 4, -1, -1, 3],
        [4, 2, 4, -1, 4],
        [4, 2, 0, 2, 2]
    ]
    cardRotation = [4, 3, 6];
    Players[0].points = 100;
    Players[1].points = 260;
}