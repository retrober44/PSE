  
describe('test ssps', function () {

  beforeEach(function () {
    players = [];
  });

  it('test testBoart', function () {
    expect(players.length).toBe(0);
    testBoart();
    expect(players.length).toBe(2);
  });
  
  it('test positionPaser', function () {
    expect(positionPaser('a6')).toEqual(['a', 6]);
    expect(positionPaser('f0')).toEqual(['f', 0]);
  });

  it('test positionPaser failure', function () {
    expect(positionPaser('a7')).toEqual(311);
    expect(positionPaser('g0')).toEqual(311);
  });

  // fillBoard

  // checkRules

  // doTurn

  it('test swapMove', function () {
    expect(swapMove('up')).toBe('down');
    expect(swapMove('down')).toBe('up');
    expect(swapMove('left')).toBe('right');
    expect(swapMove('right')).toBe('left');
  });

  // duell

  // doStrike

  // playerWins

  // playerLoses

  it('test swapBoard', function () {
    expect(swapBoard('a')).toBe('f');
    expect(swapBoard('b')).toBe('e');
    expect(swapBoard('c')).toBe('d');
    expect(swapBoard('d')).toBe('c');
    expect(swapBoard('e')).toBe('b');
    expect(swapBoard('f')).toBe('a');
  });


  it('test messagePaser', function () {
    expect(messagePaser(INITIAL_GAMEBOARD, 'foobar')).toEqual({ opCode: 11, payload: '"foobar"' });
  });

  it('test onMessage', function () {
    expect(players.length).toBe(0);
    const gameMessage = { opCode: I_AM_REDY };

    expect(onMessage(0, gameMessage)).toBe(null);
    expect(players.length).toBe(2);
    expect(players[0].redy).toBe(true);

    // expect(onMessage(1, gameMessage)).toEqual({ opCode: 12, payload: '""' });
    // expect(players[1].redy).toBe(true);
  });
});