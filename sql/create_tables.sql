-- created with dbdiagram

CREATE TYPE "games"."rating" AS ENUM (
  'super',
  'good',
  'ok',
  'meh',
  'monopoly'
);


CREATE TABLE "people"."friends" (
  "id" int PRIMARY KEY,
  "name" varchar(255) NOT NULL,
  "age" int
);

CREATE TABLE "people"."game_owners" (
  "id" int PRIMARY KEY,
  "owner_id" int NOT NULL,
  "game_id" int NOT NULL
);

CREATE TABLE "games"."games" (
  "id" int PRIMARY KEY,
  "name" varchar(255) NOT NULL,
  "year" int
);

CREATE TABLE "games"."ratings" (
  "id" int PRIMARY KEY,
  "game_id" int NOT NULL,
  "geek_rating" decimal NOT NULL,
  "my_rating" "games"."rating" NOT NULL,
  "date" Date
);

ALTER TABLE "games"."ratings" ADD FOREIGN KEY ("game_id") REFERENCES "games"."games" ("id") ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE "people"."game_owners" ADD FOREIGN KEY ("game_id") REFERENCES "games"."games" ("id") ON DELETE CASCADE ON UPDATE CASCADE;

ALTER TABLE "people"."game_owners" ADD FOREIGN KEY ("owner_id") REFERENCES "people"."friends" ("id") ON DELETE CASCADE ON UPDATE CASCADE;
