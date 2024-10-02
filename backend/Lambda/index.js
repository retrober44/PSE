// Copyright 2019 Amazon.com, Inc. or its affiliates. All Rights Reserved.
// SPDX-License-Identifier: Apache-2.0
const uuid = require("uuid");
const AWS = require("aws-sdk");

const GameLift = new AWS.GameLift({region: 'eu-central-1'});
const FleetID = process.env.gameliftFleetId; // enviroment variable set in AWS

/**
 * The handler expects the game name, delivered via GET-params.
 * @param {'M3'|'SSPS'} event.queryStringParameters.game Name of requested game.
 * @param {*} [event.body] Stringified JSON payload. Will be ignored! A warning will be logged.
 * @returns {Promise<{body: string, statusCode: number}|*>}
 * @see {@link https://www.tsmean.com/articles/aws/the-ultimate-aws-lambda-tutorial-for-nodejs/|Lambda tutorial} for further attributes of 'event'.
 */
const handler = async (event) => {
    // URL query strings (e.g. foo->bar in https://blubb.amazonaws.com/helloWorld?foo=bar)
    // are accessible through `event.queryStringParameters`.
    // Arbitrary request payload (e.g. JSON data) can be accessed through `event.body`.
    const requestedGame = event.queryStringParameters.game
    if (! ['SSPS','M3'].includes(requestedGame)) {
        console.warn("Client request contained invalid parameter:")
        console.warn(event.queryStringParameters)
        return {
            statusCode: 400,
            body: JSON.stringify({
                message: "Invalid parameter."
            })
        };
    }
    let response;
    let gameSessions;

    // find any sessions that have available players
    await GameLift.searchGameSessions({
        FleetId: FleetID,
        FilterExpression: "hasAvailablePlayerSessions=true AND gameSessionName='" + requestedGame + "'"
        // it would be also possible to filter for gameProperty.Game instead of gameSessionName
    }).promise().then(data => {
        gameSessions = data.GameSessions;
    }).catch(err => {
        response = err;
    });

    // if the response object has any value at any point before the end of
    // the function that indicates a failure condition so return the response
    if(response != null) return response;

    // if there are no sessions, then we need to create a game session
    let selectedGameSession;
    if(gameSessions.length === 0)
    {
        console.log("No game session detected, creating a new one");
        await GameLift.createGameSession({
            MaximumPlayerSessionCount: 2,   // only two players allowed per game
            FleetId: FleetID,
            Name: requestedGame, // session names don't need to be unique
            GameProperties: [{Key: 'Game', Value: requestedGame}]
        }).promise().then(data => {
            selectedGameSession = data.GameSession;
        }).catch(err => {
           response = err; 
        });

        if(response != null) return response;
    }
    else
    {
        // we grab the first session we find and join it
        selectedGameSession = gameSessions[0];
        console.log("Game session exists, will join session ", selectedGameSession.GameSessionId);
    }
    
    // there isn't a logical way selectedGameSession could be null at this point
    // but it's worth checking for in case other logic is added
    if(selectedGameSession != null) 
    {
        // now we have a game session one way or the other, create a session for this player
        await GameLift.createPlayerSession({
            GameSessionId: selectedGameSession.GameSessionId ,
            PlayerId: uuid.v4()
        }).promise().then(data => {
            console.log("Created player session ID: ", data.PlayerSession.PlayerSessionId);
            response = data.PlayerSession;
        }).catch(err => {
           response = err; 
        });

    }
    else
    {
        response = {
          statusCode: 500,
          body: JSON.stringify({
              message: "Unable to find game session, check GameLift API status."
          })
        };
    }

    return response;
};

exports.handler = handler;
