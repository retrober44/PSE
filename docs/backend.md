# So you have chosen backend.  
Good choice. Clearly, your thoughts are less cloudy than AWS.

## Proposed architecture
* [GameLift](https://aws.amazon.com/de/gamelift/) including GameLift Realtime server and FlexMatch.  
* [DynamoDB](https://aws.amazon.com/en/dynamodb/) to store score and leaderboards.  
* [Lambda](https://aws.amazon.com/en/lambda/) functions to do the rest and connect the resources, if necessary.  

## How to get started?
There is of course the [documentation](https://docs.aws.amazon.com/), where you could read about these services extensively to find out what they do - if you don't know already.
However, here is a quick explanation of what they do:  
GameLift is a service that will let us deploy very simple game servers - GameLift realtime servers - in the cloud.
This way, we don't have to worry about hardware or scaling up / down whenever we have no / lots of players, GameLift will handle it for us.
GameLift will make sure a server running our code is available for our players whenever they want to play.
FlexMatch, a "subservice" of GameLift, will handle matchmaking, allowing us to define rules to dictate when players will be matched together, so we can just
tell GameLift "here's a player who wants to play", and GameLift will find someone else, start a game server and have the two players connect to it.
Realtime server allows us to quickly create a working server with little serverside logic, which is enough for us, as our game - while being networked real-time - is turn-based, so it can be
expressed as a series of commands.  
[Docs for realtime server](https://docs.aws.amazon.com/gamelift/latest/developerguide/realtime-howitworks.html)

DynamoDB is a serverless nosql database, allowing us to quickly store and read data in a key/value format. The server could use it
to store gameplay information, such as scores, games won etc., so we could create statistics and leaderboards for players to comapre themselves.

Lambda allows us to run code serverlessly, so we can create a REST API to send commands to other services, such as GameLift, or read / write data to /from DynamoDB.
If you write Lambda functions, please use the Node.js runtime environment. 
I also recommend using the SAM CLI and/or the SAM template:  
https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/what-is-sam.html  
https://docs.aws.amazon.com/serverless-application-model/latest/developerguide/serverless-getting-started-hello-world.html

## Examples
Examples are worth the most. The AWS samples repository contains a sample for realtime server based networking in a Unity game: https://github.com/aws-samples/megafrograce-gamelift-realtime-servers-sample 


You can also find a ton of resources on AWS services by googling, obviously, which might even be more helpful than the official documentation.
