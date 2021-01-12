CREATE TABLE players (
    id        BIGINT PRIMARY KEY ASC ON CONFLICT FAIL,
    score     BIGINT,
    playcount BIGINT,
    rank      INT
);

CREATE TABLE beatmaps (
    hash              TEXT PRIMARY KEY
                             NOT NULL
        UNIQUE,
    name              TEXT   NOT NULL,
    circlesize        FLOAT  NOT NULL,
    approachrate      FLOAT  NOT NULL,
    hpdrainrate       FLOAT  NOT NULL,
    overalldifficulty FLOAT  NOT NULL,
    mode              INT    NOT NULL
        DEFAULT (0),
    totalscore        BIGINT NOT NULL
        DEFAULT (0),
    maxcombo          INT    NOT NULL,
    starrating        FLOAT  NOT NULL
);