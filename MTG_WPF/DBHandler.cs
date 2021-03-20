using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Sql;
using Microsoft.Data.Sqlite;
using Microsoft.Data;

namespace MTG_WPF
{
    class DBHandler
    {
        //Variables
        private const string dbPath= @"DB\mtg.db";
        SqliteConnection dbConnection;
        public DBHandler()
        {
            //Open Database connection.IntegerIf no DB exist, create new
            dbConnection = new SqliteConnection("Data Source=" + dbPath);
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_dynamic_cdecl() );
            dbConnection.Open();
        }

        public void InsertSets(List<CardSet> _setList)
        {
            //Insert set sql
            #region
            string setInsertSql = "INSERT INTO sets " +
                                    "( " +
                                        "set_id," +
                                        "set_code," +
                                        "mtgo_code," +
                                        "tcgplayer_id," +
                                        "name," +
                                        "set_type," +
                                        "released_at," +
                                        "block_code," +
                                        "block," +
                                        "parent_set_code," +
                                        "card_count," +
                                        "printed_size," +
                                        "digital," +
                                        "nonfoil_only," +
                                        "foil_only," +
                                        "scryfall_uri," +
                                        "uri," +
                                        "icon_svg_uri," +
                                        "search_uri" +
                                    " ) " +
                                    "VALUES ( " +
                                        "@set_id," +
                                        "@set_code," +
                                        "@mtgo_code," +
                                        "@tcgplayer_id," +
                                        "@name," +
                                        "@set_type," +
                                        "@released_at," +
                                        "@block," +
                                        "@block_code," +
                                        "@parent_set_code," +
                                        "@card_count," +
                                        "@digital," +
                                        "@printed_size," +
                                        "@nonfoil_only," +
                                        "@foil_only," +
                                        "@scryfall_uri," +
                                        "@uri," +
                                        "@icon_svg_uri," +
                                        "@search_uri" +
                                    " )";
            #endregion

            dbConnection.Open();

            using (SqliteTransaction transaction = dbConnection.BeginTransaction())
            using (SqliteCommand com_set = dbConnection.CreateCommand())
            {
                //Create Parameters
                #region
                com_set.Parameters.Add("@set_id",SqliteType.Text );
                com_set.Parameters.Add("@set_code",SqliteType.Text );
                com_set.Parameters.Add("@mtgo_code", SqliteType.Text);
                com_set.Parameters.Add("@tcgplayer_id",SqliteType.Text );
                com_set.Parameters.Add("@name",SqliteType.Text );
                com_set.Parameters.Add("@set_type",SqliteType.Text);
                com_set.Parameters.Add("@released_at",SqliteType.Text);
                com_set.Parameters.Add("@block",SqliteType.Text);
                com_set.Parameters.Add("@block_code",SqliteType.Text);
                com_set.Parameters.Add("@parent_set_code",SqliteType.Text);
                com_set.Parameters.Add("@card_count",SqliteType.Integer);
                com_set.Parameters.Add("@digital",SqliteType.Integer);
                com_set.Parameters.Add("@printed_size",SqliteType.Integer);
                com_set.Parameters.Add("@nonfoil_only",SqliteType.Integer);
                com_set.Parameters.Add("@foil_only",SqliteType.Integer);
                com_set.Parameters.Add("@scryfall_uri",SqliteType.Text);
                com_set.Parameters.Add("@uri",SqliteType.Text);
                com_set.Parameters.Add("@icon_svg_uri",SqliteType.Text);
                com_set.Parameters.Add("@search_uri", SqliteType.Text);
                #endregion
                com_set.CommandText = setInsertSql;
                com_set.Prepare();

                //Assign Values for each set
                if(_setList != null)
                {
                    foreach(CardSet set in _setList)
                    {
                        #region
                        com_set.Parameters["@set_id"].Value =(object)set.id ?? DBNull.Value;
                        com_set.Parameters["@set_code"].Value = (object)set.code ?? DBNull.Value;
                        com_set.Parameters["@mtgo_code"].Value = (object)set.mtgoCode ?? DBNull.Value;
                        com_set.Parameters["@tcgplayer_id"].Value = (object)set.tcgplayerId ?? DBNull.Value;
                        com_set.Parameters["@name"].Value = (object)set.setName ?? DBNull.Value;
                        com_set.Parameters["@set_type"].Value = (object)set.setType ?? DBNull.Value;
                        com_set.Parameters["@released_at"].Value = (object)set.releasedAt ?? DBNull.Value;
                        com_set.Parameters["@block"].Value = (object)set.block ?? DBNull.Value;
                        com_set.Parameters["@block_code"].Value = (object)set.blockCode ?? DBNull.Value;
                        com_set.Parameters["@parent_set_code"].Value = (object)set.parentSetCode ?? DBNull.Value;
                        com_set.Parameters["@card_count"].Value = (object)set.cardCount ?? 0;
                        com_set.Parameters["@digital"].Value = set.digital;
                        com_set.Parameters["@printed_size"].Value = set.printedSize;
                        com_set.Parameters["@nonfoil_only"].Value = set.nonfoilOnly;
                        com_set.Parameters["@foil_only"].Value = set.foilOnly;
                        com_set.Parameters["@scryfall_uri"].Value = (object)set.scryfallUri ?? DBNull.Value;
                        com_set.Parameters["@uri"].Value = (object)set.uri ?? DBNull.Value ?? DBNull.Value;
                        com_set.Parameters["@icon_svg_uri"].Value = (object)set.iconSVGUri ?? DBNull.Value;
                        com_set.Parameters["@search_uri"].Value = (object)set.searchUri ?? DBNull.Value;
                        #endregion
                        com_set.ExecuteNonQuery();
                    }
                }

                try
                {
                    transaction.Commit();
                }
                catch(SqliteException e)
                {
                    Console.WriteLine("Error encountered while inserting sets into DB: " + e.ErrorCode + " - " + e.Message);
                    transaction.Rollback();
                }
            }
        }

        public void InsertCards(List<CardScryfall> _cardList)
        {
            //Insert card sql
            #region
            string insertCardSql = "INSERT INTO cards ( " +
                      "object," +
                      "id," +
                      "oracle_id," +
                      "mtgo_id," +
                      "mtgo_foil_id," +
                      "tcgplayer_id," +
                      "cardmarket_id," +
                      "name," +
                      "lang," +
                      "released_at," +
                      "uri," +
                      "scryfall_uri," +
                      "layout," +
                      "highres_image," +
                      "image_status," +
                      "cmc," +
                      "type_line," +
                      "legalities_standard," +
                      "legalities_future," +
                      "legalities_historic," +
                      "legalities_gladiator," +
                      "legalities_pioneer," +
                      "legalities_modern," +
                      "legalities_legacy," +
                      "legalities_pauper," +
                      "legalities_vintage," +
                      "legalities_penny," +
                      "legalities_commander," +
                      "legalities_brawl," +
                      "legalities_duel," +
                      "legalities_oldschool," +
                      "legalities_premodern," +
                      "games_paper," +
                      "games_mtgo," +
                      "games_arena," +
                      "reserved," +
                      "foil," +
                      "nonfoil," +
                      "oversized," +
                      "promo," +
                      "reprint," +
                      "variation," +
                      "set_id," +
                      "set_name," +
                      "set_type," +
                      "set_uri," +
                      "set_search_uri," +
                      "scryfall_set_uri," +
                      "rulings_uri," +
                      "prints_search_uri," +
                      "collector_number," +
                      "digital," +
                      "rarity," +
                      "card_back_id," +
                      "artist," +
                      "border_color," +
                      "frame," +
                      "full_art," +
                      "textless," +
                      "booster," +
                      "story_spotlight," +
                      "edhrec_rank," +
                      "price_usd," +
                      "price_usd_foil," +
                      "price_eur," +
                      "price_eur_foil," +
                      "price_tix," +
                      "related_uris_gatherer," +
                      "related_uris_tcgplayer_decks," +
                      "related_uris_edhrec," +
                      "related_uris_mtgtop8," +
                      "purchase_uris_tcgplayer," +
                      "purchase_uris_cardmarket," +
                      "purchase_uris_cardhoarder" +
                  ") " +
                  "VALUES( " +
                      "@object," +
                      "@id," +
                      "@oracle_id," +
                      "@mtgo_id," +
                      "@mtgo_foil_id," +
                      "@tcgplayer_id," +
                      "@cardmarket_id," +
                      "@name," +
                      "@lang," +
                      "@released_at," +
                      "@uri," +
                      "@scryfall_uri," +
                      "@layout," +
                      "@highres_image," +
                      "@image_status," +
                      "@cmc," +
                      "@type_line," +
                      "@legalities_standard," +
                      "@legalities_future," +
                      "@legalities_historic," +
                      "@legalities_gladiator," +
                      "@legalities_pioneer," +
                      "@legalities_modern," +
                      "@legalities_legacy," +
                      "@legalities_pauper," +
                      "@legalities_vintage," +
                      "@legalities_penny," +
                      "@legalities_commander," +
                      "@legalities_brawl," +
                      "@legalities_duel," +
                      "@legalities_oldschool," +
                      "@legalities_premodern," +
                      "@games_paper," +
                      "@games_mtgo," +
                      "@games_arena," +
                      "@reserved," +
                      "@foil," +
                      "@nonfoil," +
                      "@oversized," +
                      "@promo," +
                      "@reprint," +
                      "@variation," +
                      "@set_id," +
                      "@set_name," +
                      "@set_type," +
                      "@set_uri," +
                      "@set_search_uri," +
                      "@scryfall_set_uri," +
                      "@rulings_uri," +
                      "@prints_search_uri," +
                      "@collector_number," +
                      "@digital," +
                      "@rarity," +
                      "@card_back_id," +
                      "@artist," +
                      "@border_color," +
                      "@frame," +
                      "@full_art," +
                      "@textless," +
                      "@booster," +
                      "@story_spotlight," +
                      "@edhrec_rank," +
                      "@price_usd," +
                      "@price_usd_foil," +
                      "@price_eur," +
                      "@price_eur_foil," +
                      "@price_tix," +
                      "@related_uris_gatherer," +
                      "@related_uris_tcgplayer_decks," +
                      "@related_uris_edhrec," +
                      "@related_uris_mtgtop8," +
                      "@purchase_uris_tcgplayer," +
                      "@purchase_uris_cardmarket," +
                      "@purchase_uris_cardhoarder" +
                  "); ";
            #endregion

            //Insert card color identities
            #region
            string insertCardColorIdentitiesSql = "INSERT INTO cards_color_identities ( " +
                                                       "dbid," +
                                                       "color_identity " +
                                                   ") " +
                                                   "VALUES( " +
                                                       "@dbid," +
                                                       "@color_identity" +
                                                   ");";
            #endregion

            //Insert card colors
            #region
            string insertCardColorsSql = "INSERT INTO cards_colors ( " +
                             "dbid," +
                             "color" +
                         ") " +
                         "VALUES( " +
                             "@dbid," +
                             "@color" +
                         ");";
            
            #endregion

            //Insert card face sql
            #region
            string insertCardFaceSql = "INSERT INTO cards_faces ( " +
                            "dbid, " +
                            "object, " +
                            "name, " +
                            "mana_cost, " +
                            "type_line, " +
                            "oracle_text, " +
                            "power, " +
                            "toughness, " +
                            "artist, " +
                            "artist_id, " +
                            "illustration_id, " +
                            "image_status, " +
                            "image_uris_small, " +
                            "image_uris_normal, " +
                            "image_uris_large, " +
                            "image_uris_png, " +
                            "image_uris_art_crop, " +
                            "image_uris_border_crop " +
                        ") " +
                        "VALUES( " +
                            "@dbid," +
                            "@object," +
                            "@name," +
                            "@mana_cost," +
                            "@type_line," +
                            "@oracle_text," +
                            "@power," +
                            "@toughness," +
                            "@artist," +
                            "@artist_id," +
                            "@illustration_id," +
                            "@image_status," +
                            "@image_uris_small," +
                            "@image_uris_normal," +
                            "@image_uris_large," +
                            "@image_uris_png," +
                            "@image_uris_art_crop," +
                            "@image_uris_border_crop" +
                        ");";

            #endregion

            //Insert card faces color indicators
            #region
            string insertCardFacesColorIndicatorsSql = "INSERT INTO cards_faces_color_indicators ( " +
                                             "face_id, " +
                                             "color_indicator " +
                                         ")" +
                                         "VALUES(" +
                                             "@face_id," +
                                             "@color_indicator " +
                                         ");";
            #endregion

            //Insert card faces colors
            #region
            string insertCardFacesColorsSql = "INSERT INTO cards_faces_colors ( " +
                                   "face_id," +
                                   "color " +
                               ") " +
                               "VALUES( " +
                                   "@face_id," +
                                   "@color" +
                               ");";
            #endregion

            //Insert card frame effects
            #region
            string insertCardFrameEffectsSql = "INSERT INTO cards_frame_effects( " +
                                    "dbid," +
                                    "effect" +
                                ") " +
                                "VALUES( " +
                                    "@dbid, " +
                                    "@effect" +
                                ");";
            #endregion

            //Insert card keywords
            #region
            string insertCardKeywordsSql = "INSERT INTO cards_keywords ( " +
                               "dbid," +
                               "keyword" +
                           ") " +
                           "VALUES( " +
                               "@dbid," +
                               "@keyword" +
                           ");";
            #endregion

            //Insert card multiverse
            #region
            string insertCardMultiversesSql = "INSERT INTO cards_multiverse_ids( " +
                                     "dbid, " +
                                     "multiverse_id " +
                                 ") " +
                                 "VALUES( " +
                                     "@dbid," +
                                     "@multiverse_id" +
                                 ");";
            #endregion

            if (_cardList != null)
            {
                dbConnection.Open();

                using (SqliteTransaction transaction = dbConnection.BeginTransaction())
                using (SqliteCommand com_card = dbConnection.CreateCommand())
                using (SqliteCommand com_card_iden = dbConnection.CreateCommand())
                using (SqliteCommand com_card_col = dbConnection.CreateCommand())
                using (SqliteCommand com_card_f = dbConnection.CreateCommand())
                using (SqliteCommand com_card_f_col_ind = dbConnection.CreateCommand())
                using (SqliteCommand com_card_f_col = dbConnection.CreateCommand())
                using (SqliteCommand com_card_fr_eff = dbConnection.CreateCommand())
                using (SqliteCommand com_card_key = dbConnection.CreateCommand())
                using (SqliteCommand com_card_multi = dbConnection.CreateCommand())
                {
                    //**********************PARAMETERS 
                    #region

                    //EXECUTE INSERT --> cards
                    #region
                    com_card.Parameters.Add("@object", SqliteType.Text);
                    com_card.Parameters.Add("@id", SqliteType.Text);
                    com_card.Parameters.Add("@oracle_id", SqliteType.Text);
                    com_card.Parameters.Add("@mtgo_id", SqliteType.Text);
                    com_card.Parameters.Add("@mtgo_foil_id", SqliteType.Integer);
                    com_card.Parameters.Add("@tcgplayer_id", SqliteType.Integer);
                    com_card.Parameters.Add("@cardmarket_id", SqliteType.Integer);
                    com_card.Parameters.Add("@name", SqliteType.Text);
                    com_card.Parameters.Add("@lang", SqliteType.Text);
                    com_card.Parameters.Add("@released_at", SqliteType.Text);
                    com_card.Parameters.Add("@uri", SqliteType.Text);
                    com_card.Parameters.Add("@scryfall_uri", SqliteType.Text);
                    com_card.Parameters.Add("@layout", SqliteType.Text);
                    com_card.Parameters.Add("@highres_image", SqliteType.Text);
                    com_card.Parameters.Add("@image_status", SqliteType.Text);
                    com_card.Parameters.Add("@cmc", SqliteType.Real);
                    com_card.Parameters.Add("@type_line", SqliteType.Text);
                    com_card.Parameters.Add("@legalities_standard", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_future", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_historic", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_gladiator", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_pioneer", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_modern", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_legacy", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_pauper", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_vintage", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_penny", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_commander", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_brawl", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_duel", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_oldschool", SqliteType.Integer);
                    com_card.Parameters.Add("@legalities_premodern", SqliteType.Integer);
                    com_card.Parameters.Add("@games_paper", SqliteType.Integer);
                    com_card.Parameters.Add("@games_mtgo", SqliteType.Integer);
                    com_card.Parameters.Add("@games_arena", SqliteType.Integer);
                    com_card.Parameters.Add("@reserved", SqliteType.Integer);
                    com_card.Parameters.Add("@foil", SqliteType.Integer);
                    com_card.Parameters.Add("@nonfoil", SqliteType.Integer);
                    com_card.Parameters.Add("@oversized", SqliteType.Integer);
                    com_card.Parameters.Add("@promo", SqliteType.Integer);
                    com_card.Parameters.Add("@reprint", SqliteType.Integer);
                    com_card.Parameters.Add("@variation", SqliteType.Integer);
                    com_card.Parameters.Add("@set_id", SqliteType.Text);
                    com_card.Parameters.Add("@set_name", SqliteType.Text);
                    com_card.Parameters.Add("@set_type", SqliteType.Text);
                    com_card.Parameters.Add("@set_uri", SqliteType.Text);
                    com_card.Parameters.Add("@set_search_uri", SqliteType.Text);
                    com_card.Parameters.Add("@scryfall_set_uri", SqliteType.Text);
                    com_card.Parameters.Add("@rulings_uri", SqliteType.Text);
                    com_card.Parameters.Add("@prints_search_uri", SqliteType.Text);
                    com_card.Parameters.Add("@collector_number", SqliteType.Text);
                    com_card.Parameters.Add("@digital", SqliteType.Integer);
                    com_card.Parameters.Add("@rarity", SqliteType.Text);
                    com_card.Parameters.Add("@card_back_id", SqliteType.Text);
                    com_card.Parameters.Add("@artist", SqliteType.Text);
                    com_card.Parameters.Add("@border_color", SqliteType.Text);
                    com_card.Parameters.Add("@frame", SqliteType.Text);
                    com_card.Parameters.Add("@full_art", SqliteType.Integer);
                    com_card.Parameters.Add("@textless", SqliteType.Integer);
                    com_card.Parameters.Add("@booster", SqliteType.Integer);
                    com_card.Parameters.Add("@story_spotlight", SqliteType.Integer);
                    com_card.Parameters.Add("@edhrec_rank", SqliteType.Integer);
                    com_card.Parameters.Add("@price_usd", SqliteType.Integer);
                    com_card.Parameters.Add("@price_usd_foil", SqliteType.Integer);
                    com_card.Parameters.Add("@price_eur", SqliteType.Integer);
                    com_card.Parameters.Add("@price_eur_foil", SqliteType.Integer);
                    com_card.Parameters.Add("@price_tix", SqliteType.Integer);
                    com_card.Parameters.Add("@related_uris_gatherer", SqliteType.Text);
                    com_card.Parameters.Add("@related_uris_tcgplayer_decks", SqliteType.Text);
                    com_card.Parameters.Add("@related_uris_edhrec", SqliteType.Text);
                    com_card.Parameters.Add("@related_uris_mtgtop8", SqliteType.Text);
                    com_card.Parameters.Add("@purchase_uris_tcgplayer", SqliteType.Text);
                    com_card.Parameters.Add("@purchase_uris_cardmarket", SqliteType.Text);
                    com_card.Parameters.Add("@purchase_uris_cardhoarder", SqliteType.Text);
                    #endregion
                    com_card.CommandText = insertCardSql;
                    com_card.Prepare();

                    //EXECUTE INSERT --> card col Identities
                    #region
                        com_card_iden.Parameters.Add("@dbid", SqliteType.Integer);
                        com_card_iden.Parameters.Add("@color_identity", SqliteType.Text);
                    #endregion
                    com_card_iden.CommandText = insertCardColorIdentitiesSql;
                    com_card_iden.Prepare();

                    //EXECUTE INSERT --> card colors
                    #region
                                com_card_col.Parameters.Add("@dbid", SqliteType.Integer );
                                com_card_col.Parameters.Add("@color", SqliteType.Text );
                    #endregion
                    com_card_col.CommandText = insertCardColorsSql;
                    com_card_col.Prepare();

                    //EXECUTE INSERT --> card faces
                    #region
                            com_card_f.Parameters.Add("@dbid", SqliteType.Integer );
                            com_card_f.Parameters.Add("@object", SqliteType.Text );
                            com_card_f.Parameters.Add("@name", SqliteType.Text);
                            com_card_f.Parameters.Add("@mana_cost", SqliteType.Text);
                            com_card_f.Parameters.Add("@type_line", SqliteType.Text);
                            com_card_f.Parameters.Add("@oracle_text", SqliteType.Text);
                            com_card_f.Parameters.Add("@power", SqliteType.Text);
                            com_card_f.Parameters.Add("@toughness", SqliteType.Text);
                            com_card_f.Parameters.Add("@artist", SqliteType.Text);
                            com_card_f.Parameters.Add("@artist_id", SqliteType.Text);
                            com_card_f.Parameters.Add("@illustration_id", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_status", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_small", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_normal", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_large", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_png", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_art_crop", SqliteType.Text);
                            com_card_f.Parameters.Add("@image_uris_border_crop", SqliteType.Text);
                    #endregion
                    com_card_f.CommandText = insertCardFaceSql;
                    com_card_f.Prepare();

                    //EXECUTE INSERT --> card faces col indicators
                    #region
                        com_card_f_col_ind.Parameters.Add("@face_id", SqliteType.Integer );
                        com_card_f_col_ind.Parameters.Add("@color_indicator ", SqliteType.Text );
                    #endregion
                    com_card_f_col_ind.CommandText = insertCardFacesColorIndicatorsSql;
                    com_card_f_col_ind.Prepare();

                    //EXECUTE INSERT --> card faces colors
                    #region
                        com_card_f_col.Parameters.Add("@face_id", SqliteType.Integer );
                        com_card_f_col.Parameters.Add("@color", SqliteType.Text );
                    #endregion
                    com_card_f_col.CommandText = insertCardFacesColorsSql;
                    com_card_f_col.Prepare();

                    //EXECUTE INSERT --> card frame effects
                    #region
                        com_card_fr_eff.Parameters.Add("@dbid ", SqliteType.Integer );
                        com_card_fr_eff.Parameters.Add("@effect", SqliteType.Text );
                    #endregion
                    com_card_fr_eff.CommandText = insertCardFrameEffectsSql;
                    com_card_fr_eff.Prepare();

                    //EXECUTE INSERT --> card keywords
                    #region
                        com_card_key.Parameters.Add("@dbid", SqliteType.Integer );
                        com_card_key.Parameters.Add("@keyword", SqliteType.Text );
                    #endregion
                    com_card_key.CommandText = insertCardKeywordsSql;
                    com_card_key.Prepare();

                    //EXECUTE INSERT --> card mulitverses
                    #region
                        com_card_multi.Parameters.Add("@dbid", SqliteType.Integer );
                        com_card_multi.Parameters.Add("@multiverse_id", SqliteType.Text );
                    #endregion
                    com_card_multi.CommandText = insertCardMultiversesSql;
                    com_card_multi.Prepare();

                    #endregion

                    //***********************EXECUTE INSERTS

                    foreach(CardScryfall _card in _cardList)
                    {
                        //EXECUTE INSERT --> cards
                        #region
                        com_card.Parameters["@object"].Value = (object)_card.objectName ?? DBNull.Value; 
                        com_card.Parameters["@id"].Value =  (object)_card.id ?? DBNull.Value; 
                        com_card.Parameters["@oracle_id"].Value = (object)_card.oracleId ?? DBNull.Value; 
                        com_card.Parameters["@mtgo_id"].Value = (object)_card.mtgoId ?? 0; 
                        com_card.Parameters["@mtgo_foil_id"].Value = (object)_card.mtgoFoilId ?? 0 ; 
                        com_card.Parameters["@tcgplayer_id"].Value = (object)_card.tcgplayerId ?? 0; 
                        com_card.Parameters["@cardmarket_id"].Value = (object)_card.cardmarketId ?? 0; 
                        com_card.Parameters["@name"].Value = (object)_card.cardName ?? DBNull.Value; 
                        com_card.Parameters["@lang"].Value = (object)_card.lang ?? DBNull.Value; 
                        com_card.Parameters["@released_at"].Value = (object)_card.releasedAt ?? DBNull.Value; 
                        com_card.Parameters["@uri"].Value = (object)_card.apiUri ?? DBNull.Value; 
                        com_card.Parameters["@scryfall_uri"].Value = (object)_card.scryfallSetUri ?? DBNull.Value; 
                        com_card.Parameters["@layout"].Value = (object)_card.layout ?? DBNull.Value; 
                        com_card.Parameters["@highres_image"].Value = (object)_card.highresImage ?? DBNull.Value; 
                        com_card.Parameters["@image_status"].Value = (object)_card.imageStatus ?? DBNull.Value; 
                        com_card.Parameters["@cmc"].Value = (object)_card.cmc ?? 0;
                        com_card.Parameters["@type_line"].Value = (object)_card.typeLine ?? DBNull.Value;
                        if (_card.legalities != null)
                        {
                            com_card.Parameters["@legalities_standard"].Value = _card.legalities.standard == "legal" ? true : false;
                            com_card.Parameters["@legalities_future"].Value = _card.legalities.future == "legal" ? true : false;
                            com_card.Parameters["@legalities_historic"].Value = _card.legalities.historic == "legal" ? true : false;
                            com_card.Parameters["@legalities_gladiator"].Value = _card.legalities.gladiator == "legal" ? true : false;
                            com_card.Parameters["@legalities_pioneer"].Value = _card.legalities.pioneer == "legal" ? true : false;
                            com_card.Parameters["@legalities_modern"].Value = _card.legalities.modern == "legal" ? true : false;
                            com_card.Parameters["@legalities_legacy"].Value = _card.legalities.legacy == "legal" ? true : false;
                            com_card.Parameters["@legalities_pauper"].Value = _card.legalities.pauper == "legal" ? true : false;
                            com_card.Parameters["@legalities_vintage"].Value = _card.legalities.vintage == "legal" ? true : false;
                            com_card.Parameters["@legalities_penny"].Value = _card.legalities.penny == "legal" ? true : false;
                            com_card.Parameters["@legalities_commander"].Value = _card.legalities.commander == "legal" ? true : false;
                            com_card.Parameters["@legalities_brawl"].Value = _card.legalities.brawl == "legal" ? true : false;
                            com_card.Parameters["@legalities_duel"].Value = _card.legalities.duel == "legal" ? true : false;
                            com_card.Parameters["@legalities_oldschool"].Value = _card.legalities.oldschool == "legal" ? true : false;
                            com_card.Parameters["@legalities_premodern"].Value = _card.legalities.premodern == "legal" ? true : false;
                        }
                        if(_card.games != null)
                        {
                            foreach(string game in _card.games)
                            {
                                com_card.Parameters["@games_paper"].Value = game == "paper" ? true : false ; 
                                com_card.Parameters["@games_mtgo"].Value = game == "mtgo" ? true : false;
                                com_card.Parameters["@games_arena"].Value = game == "arena" ? true : false;
                            }
                        }
                        com_card.Parameters["@reserved"].Value =  _card.reserved ; 
                        com_card.Parameters["@foil"].Value = _card.foil; 
                        com_card.Parameters["@nonfoil"].Value = _card.nonfoil; 
                        com_card.Parameters["@oversized"].Value = _card.oversized; 
                        com_card.Parameters["@promo"].Value = _card.promo; 
                        com_card.Parameters["@reprint"].Value = _card.reprint; 
                        com_card.Parameters["@variation"].Value = _card.variation; 
                        com_card.Parameters["@set_id"].Value =  (object)_card.set ?? DBNull.Value; 
                        com_card.Parameters["@set_name"].Value = (object)_card.setName ?? DBNull.Value; 
                        com_card.Parameters["@set_type"].Value = (object)_card.setType ?? DBNull.Value; 
                        com_card.Parameters["@set_uri"].Value = (object)_card.setUri ?? DBNull.Value; 
                        com_card.Parameters["@set_search_uri"].Value = (object)_card.setSearchUri ?? DBNull.Value; 
                        com_card.Parameters["@scryfall_set_uri"].Value = (object)_card.scryfallSetUri ?? DBNull.Value; 
                        com_card.Parameters["@rulings_uri"].Value = (object)_card.rulingsUri ?? DBNull.Value; 
                        com_card.Parameters["@prints_search_uri"].Value = (object)_card.printsSearchUri ?? DBNull.Value; 
                        com_card.Parameters["@collector_number"].Value = (object)_card.collectorNumber ?? DBNull.Value ; 
                        com_card.Parameters["@digital"].Value = _card.digital; 
                        com_card.Parameters["@rarity"].Value = (object)_card.rarity ?? DBNull.Value; 
                        com_card.Parameters["@card_back_id"].Value = (object)_card.cardBackId ?? DBNull.Value; 
                        com_card.Parameters["@artist"].Value = (object)_card.artist ?? DBNull.Value; 
                        com_card.Parameters["@border_color"].Value = (object)_card.borderColor ?? DBNull.Value; 
                        com_card.Parameters["@frame"].Value = (object)_card.frame ?? DBNull.Value; 
                        com_card.Parameters["@full_art"].Value = _card.full_art; 
                        com_card.Parameters["@textless"].Value = _card.textless; 
                        com_card.Parameters["@booster"].Value =  _card.booster; 
                        com_card.Parameters["@story_spotlight"].Value = _card.storySpotlight; 
                        com_card.Parameters["@edhrec_rank"].Value = (object)_card.edhrecRank ?? 0;
                        if (_card.prices != null)
                        {
                            com_card.Parameters["@price_usd"].Value = (object)_card.prices.usd ?? 0;
                            com_card.Parameters["@price_usd_foil"].Value = (object)_card.prices.usd_foil ?? 0;
                            com_card.Parameters["@price_eur"].Value = (object)_card.prices.eur ?? 0;
                            com_card.Parameters["@price_eur_foil"].Value = (object)_card.prices.eur_foil ?? 0;
                            com_card.Parameters["@price_tix"].Value = (object)_card.prices.tix ?? 0;
                        }
                        if(_card.relatedUris != null)
                        {
                            com_card.Parameters["@related_uris_gatherer"].Value =  (object)_card.relatedUris.gatherer ?? DBNull.Value; 
                            com_card.Parameters["@related_uris_tcgplayer_decks"].Value = (object)_card.relatedUris.tcgplayer_decks ?? DBNull.Value; 
                            com_card.Parameters["@related_uris_edhrec"].Value = (object)_card.relatedUris.edhrec ?? DBNull.Value; 
                            com_card.Parameters["@related_uris_mtgtop8"].Value = (object)_card.relatedUris.mtgtop8 ?? DBNull.Value; 
                        }
                        if(_card.purchaseUris != null)
                        {
                            com_card.Parameters["@purchase_uris_tcgplayer"].Value =  (object)_card.purchaseUris.tcgplayer ?? DBNull.Value;
                            com_card.Parameters["@purchase_uris_cardmarket"].Value = (object)_card.purchaseUris.cardmarket ?? DBNull.Value;
                            com_card.Parameters["@purchase_uris_cardhoarder"].Value = (object)_card.purchaseUris.cardhoarder ?? DBNull.Value;
                        }
                        
                    #endregion
                        com_card.ExecuteNonQuery();

                        int NextDBID = GetNextDBID(true);
                        //EXECUTE INSERT --> card col Identities
                        #region
                        if(_card.colorIdentity != null)
                        {
                            foreach(string identity in _card.colorIdentity)
                            {
                                com_card_iden.Parameters["@dbid"].Value = NextDBID;
                                com_card_iden.Parameters["@color_identity"].Value = identity;
                                com_card.ExecuteNonQuery();
                            }
                        }
                        #endregion

                        //EXECUTE INSERT --> card colors
                        #region
                        if(_card.colors != null)
                        {
                            foreach(string color in _card.colors)
                            {
                                com_card_col.Parameters["@dbid"].Value = NextDBID;
                                com_card_col.Parameters["@color"].Value = color;
                                com_card_col.ExecuteNonQuery();
                            }
                        }
                        #endregion

                        //EXECUTE INSERT --> card frame effects
                        #region
                        if (_card.frameEffects != null)
                        {
                            foreach (string effect in _card.frameEffects)
                            {
                                com_card_fr_eff.Parameters["@dbid "].Value =  NextDBID ;
                                com_card_fr_eff.Parameters["@effect"].Value = (object)effect ?? DBNull.Value;
                                com_card_fr_eff.ExecuteNonQuery();
                            }
                        }
                        #endregion


                        //EXECUTE INSERT --> card keywords
                        #region
                        if(_card.keywords != null)
                        {
                            foreach(string key in _card.keywords)
                            {
                                com_card_key.Parameters["@dbid"].Value = NextDBID;
                                com_card_key.Parameters["@keyword"].Value = (object)key ?? DBNull.Value;
                                com_card_key.ExecuteNonQuery();
                            }
                        }
                        #endregion
                        

                        //EXECUTE INSERT --> card mulitverses
                        #region
                        if(_card.multiverseIds != null)
                        {
                            foreach(int multi in _card.multiverseIds)
                            {
                                com_card_multi.Parameters["@dbid"].Value = NextDBID;
                                com_card_multi.Parameters["@multiverse_id"].Value = (object)multi ?? 0;
                                com_card_multi.ExecuteNonQuery();
                            }
                        }
                        #endregion
                        
                        //EXECUTE INSERT --> card faces
                        #region
                        foreach(CardFace face in _card.cardFaces)
                        {
                            com_card_f.Parameters["@dbid"].Value = NextDBID;
                            com_card_f.Parameters["@object"].Value = (object)face.objectName ?? DBNull.Value;
                            com_card_f.Parameters["@name"].Value =  (object)face.cardName ?? DBNull.Value;
                            com_card_f.Parameters["@mana_cost"].Value = (object)face.manaCost ?? DBNull.Value;
                            com_card_f.Parameters["@type_line"].Value = (object)face.typeLine ?? DBNull.Value;
                            com_card_f.Parameters["@oracle_text"].Value = (object)face.oracleText ?? DBNull.Value;
                            com_card_f.Parameters["@power"].Value = (object)face.power ?? DBNull.Value;
                            com_card_f.Parameters["@toughness"].Value = (object)face.toughness ?? DBNull.Value;
                            com_card_f.Parameters["@artist"].Value = (object)face.artist ?? DBNull.Value;
                            com_card_f.Parameters["@artist_id"].Value = (object)face.artistId ?? DBNull.Value;
                            com_card_f.Parameters["@illustration_id"].Value = (object)face.illustrationId ?? DBNull.Value ;
                            com_card_f.Parameters["@image_status"].Value = (object)face.imageStatus ?? DBNull.Value;
                            if(face.images != null)
                            {
                                com_card_f.Parameters["@image_uris_small"].Value =  (object)face.images.small ?? DBNull.Value;
                                com_card_f.Parameters["@image_uris_normal"].Value = (object)face.images.normal ?? DBNull.Value;
                                com_card_f.Parameters["@image_uris_large"].Value = (object)face.images.large ?? DBNull.Value;
                                com_card_f.Parameters["@image_uris_png"].Value = (object)face.images.png ?? DBNull.Value;
                                com_card_f.Parameters["@image_uris_art_crop"].Value = (object)face.images.art_crop ?? DBNull.Value;
                                com_card_f.Parameters["@image_uris_border_crop"].Value = (object)face.images.border_crop ?? DBNull.Value;
                            }

                            com_card_f.ExecuteNonQuery();

                            int nextFaceId = GetNextFaceId();
                            //EXECUTE INSERT --> card faces col indicators
                            #region
                            if(face.colorIndicators != null)
                            {
                                foreach(string indicator in face.colorIndicators)
                                {
                                    com_card_f_col_ind.Parameters["@face_id"].Value =  nextFaceId ;
                                    com_card_f_col_ind.Parameters["@color_indicator "].Value = (object)indicator ?? DBNull.Value;
                                    com_card_f_col_ind.ExecuteNonQuery();
                                }
                            }
                            #endregion


                            //EXECUTE INSERT --> card faces colors
                            #region
                            if(face.colors != null)
                            {
                                foreach(string color in face.colors)
                                {
                                    com_card_f_col.Parameters["@face_id"].Value = nextFaceId;
                                    com_card_f_col.Parameters["@color"].Value = (object)color ?? DBNull.Value;
                                    com_card_f_col.ExecuteNonQuery();
                                }
                            }
                            #endregion
                        }
                        
                        #endregion
                    }

                    try
                    {
                        transaction.Commit();
                    }
                    catch(SqliteException e)
                    {
                        Console.WriteLine("Error encountered during inserting cards to DB: " + e.ErrorCode + " - " + e.Message);
                        Console.WriteLine("Transaction has been rolled back");
                        transaction.Rollback();
                    }

                    dbConnection.Close();
                }
            }
        }

        private int GetNextDBID(bool _keepOpen = false)
        {
            int nextDBID = 0;
            if (dbConnection.State == System.Data.ConnectionState.Closed)
            {
                dbConnection.Open();
            }

            string sql = "SELECT MAX(dbid) FROM cards";

            SqliteCommand com = dbConnection.CreateCommand();
            com.CommandText = sql;
            nextDBID = int.Parse(com.ExecuteScalar().ToString()) +1;

            if(!_keepOpen)
            {
                dbConnection.Close();
            }
            return nextDBID;
        }

        private int GetNextFaceId(bool _keepOpen = false)
        {
            int nextFaceId = 0;
            if (dbConnection.State == System.Data.ConnectionState.Closed)
            {
                dbConnection.Open();
                string sql = "SELECT MAX(face_id) FROM cards_faces";

                SqliteCommand com = dbConnection.CreateCommand();
                com.CommandText = sql;
                nextFaceId = (int)com.ExecuteScalar()+1;

                if (!_keepOpen)
                {
                    dbConnection.Close();
                }
            }

            return nextFaceId;

        }

        ~DBHandler()
        {

        }
    }
}
