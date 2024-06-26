CREATE MATERIALIZED VIEW "games"."games_with_info" 
AS SELECT 
    g.name,
    r.geek_rating, 
    r.my_rating,
    f.name as owner_name
FROM games.games g 
    JOIN games.ratings r ON (g.id = r.game_id)
    LEFT JOIN people.game_owners o ON (g.id = o.game_id)
    LEFT JOIN people.friends f ON (f.id = o.owner_id)
ORDER BY geek_rating DESC, owner_name
;