CREATE TABLE players (
    id        BIGINT PRIMARY KEY ASC ON CONFLICT FAIL,
    score     BIGINT,
    playcount BIGINT,
    rank      INT
);


