-- Creator:       MySQL Workbench 6.0.8/ExportSQLite plugin 2009.12.02
-- Author:        Thang
-- Caption:       New Model
-- Project:       Name of the project
-- Changed:       2014-10-28 16:26
-- Created:       2014-10-28 15:17
PRAGMA foreign_keys = OFF;

-- Schema: onepiece
BEGIN;
CREATE TABLE "op_user"(
  "id" INTEGER PRIMARY KEY NOT NULL,-- User id
  "user_name" VARCHAR(255) NOT NULL,-- User name, lay tu facebook
  "facebook_token" VARCHAR(255),
  "exp" INTEGER NOT NULL,
--   Điểm kinh nghiệm của user.
--   Điểm kinh nghiệm sẽ tăng lên sau mỗi lượt chơi
  "level_id" INTEGER NOT NULL,
--   Level của user. 
--   Level sẽ được tính theo điểm kinh nghiệm
  "lasted_health_res" DATETIME,
--   Thời gian lần cuối health được phục hồi
--   cứ mỗi 5 phút 1 health được phục hồi.
  "health" INTEGER NOT NULL,
--   Số health hiện tại.
--   Max là 5 health
  "updated_at" DATETIME,
  "created_at" DATETIME,
  "score" INTEGER,
  "high_score" INTEGER
);
CREATE TABLE "op_level"(
  "id" INTEGER PRIMARY KEY NOT NULL,
--   User level
--   Luu level cua user
  "level" INTEGER,-- Level
  "exp" INTEGER-- Số điểm kinh nghiệm cần có để đạt được level này.
);
CREATE TABLE "op_item"(
  "id" INTEGER PRIMARY KEY NOT NULL,
  "name" VARCHAR(45),
  "category" VARCHAR(45),
  "effect_name" VARCHAR(45),
  "consum_item" INTEGER,
--   item bị dùng
--   VD: 1 bom block = 1000 belly
  "cost" INTEGER
--   Giá của item
--   VD: 1 bom block = 1000 belly
);
CREATE TABLE "op_user_item"(
  "id" INTEGER PRIMARY KEY NOT NULL,
  "user_id" INTEGER,
  "item_id" INTEGER,
  "num" INTEGER
);
CREATE TABLE "level_character"(
  "id" INTEGER PRIMARY KEY NOT NULL,
  "level_id" INTEGER,-- level id
  "character_id" INTEGER
--   Ở level này sẽ hiển thị những character nào?
--   1 level có thể hiển thị nhiều character. cách hiển thị random (game logic)
--   Điêm càng cao -> càng hiển thị quái mạnh
);
COMMIT;
