INSERT INTO people.friends (id, name, age) values 
    (1, 'Jay', 30 ),
    (2, 'Andy', 39),
    (3, 'Eve', NULL ),
    (4, 'Anne', NULL);

INSERT INTO games.games (id, name, year) values 
    (1, 'wizard', 1984),
    (2, 'carcassonne', 2000),
    (3, 'istanbul', 2014),
    (4, 'terraforming mars', 2016),
    (5, 'splendor', 2014),
    (6, 'patchwork', 2014);

INSERT INTO games.ratings (id, game_id, rating, date) values 
    (1, 1, 7.0, '2024-02-22'),
    (2, 2, 7.4, '2024-02-22'),
    (3, 3, 7.5, '2024-02-22'), 
    (4, 4, 8.4, '2024-02-22'),
    (5, 5, 7.4, '2024-02-22'),
    (6, 6, 7.6, '2024-02-22');

INSERT INTO people.game_owners (id, owner_id, game_id) values 
    (1, 1, 1), 
    (2, 1, 2),
    (3, 2, 3),
    (4, 4, 4),
    (5, 3, 5),
    (6, 2, 5),
    (7, 3, 6),
    (8, 4, 6);




