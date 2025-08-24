-- SELECT get_chart_json_by_id(id)
-- FROM chart

-- DELETE FROM way;
-- ALTER SEQUENCE public.way_id_seq RESTART WITH 1;
-- ALTER SEQUENCE public.way_item_id_seq RESTART WITH 1;
-- ALTER SEQUENCE public.way_item_station_id_seq RESTART WITH 1;
-- ALTER SEQUENCE public.way_item_railway_id_seq RESTART WITH 1;
-- ALTER SEQUENCE public.way_item_transition_id_seq RESTART WITH 1;

SELECT add_route_json(1, 3, 
'{
  "Title": "Новый маршрут",
  "Duration": "03:49:10",
  "RouteItems": [
    {
      "$type": "station",
      "Title": "Нахабино",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Нахабино",
      "ToStationTitle": "Аникеевка"
    },
    {
      "$type": "station",
      "Title": "Аникеевка",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Аникеевка",
      "ToStationTitle": "Опалиха"
    },
    {
      "$type": "station",
      "Title": "Опалиха",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Опалиха",
      "ToStationTitle": "Красногорская"
    },
    {
      "$type": "station",
      "Title": "Красногорская",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Красногорская",
      "ToStationTitle": "Павшино"
    },
    {
      "$type": "station",
      "Title": "Павшино",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Павшино",
      "ToStationTitle": "Пенягино"
    },
    {
      "$type": "station",
      "Title": "Пенягино",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Пенягино",
      "ToStationTitle": "Волоколамская"
    },
    {
      "$type": "station",
      "Title": "Волоколамская",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Волоколамская",
      "ToStationTitle": "Трикотажная"
    },
    {
      "$type": "station",
      "Title": "Трикотажная",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Трикотажная",
      "ToStationTitle": "Тушинская"
    },
    {
      "$type": "station",
      "Title": "Тушинская",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Тушинская",
      "ToStationTitle": "Щукинская"
    },
    {
      "$type": "station",
      "Title": "Щукинская",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Щукинская",
      "ToStationTitle": "Стрешнево"
    },
    {
      "$type": "station",
      "Title": "Стрешнево",
      "BranchTitle": "МЦД-2"
    },
    {
      "$type": "transition",
      "FromBranchTitle": "МЦД-2",
      "FromStationTitle": "Стрешнево",
      "ToBranchTitle": "Замоскворецкая линия",
      "ToStationTitle": "Войковская"
    },
    {
      "$type": "station",
      "Title": "Войковская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Войковская",
      "ToStationTitle": "Сокол"
    },
    {
      "$type": "station",
      "Title": "Сокол",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Сокол",
      "ToStationTitle": "Аэропорт"
    },
    {
      "$type": "station",
      "Title": "Аэропорт",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Аэропорт",
      "ToStationTitle": "Динамо"
    },
    {
      "$type": "station",
      "Title": "Динамо",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Динамо",
      "ToStationTitle": "Белорусская"
    },
    {
      "$type": "station",
      "Title": "Белорусская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Белорусская",
      "ToStationTitle": "Маяковская"
    },
    {
      "$type": "station",
      "Title": "Маяковская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Маяковская",
      "ToStationTitle": "Тверская"
    },
    {
      "$type": "station",
      "Title": "Тверская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Тверская",
      "ToStationTitle": "Театральная"
    },
    {
      "$type": "station",
      "Title": "Театральная",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Театральная",
      "ToStationTitle": "Новокузнецкая"
    },
    {
      "$type": "station",
      "Title": "Новокузнецкая",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Новокузнецкая",
      "ToStationTitle": "Павелецкая"
    },
    {
      "$type": "station",
      "Title": "Павелецкая",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Павелецкая",
      "ToStationTitle": "Автозаводская"
    },
    {
      "$type": "station",
      "Title": "Автозаводская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Автозаводская",
      "ToStationTitle": "Технопарк"
    },
    {
      "$type": "station",
      "Title": "Технопарк",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Технопарк",
      "ToStationTitle": "Коломенская"
    },
    {
      "$type": "station",
      "Title": "Коломенская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Коломенская",
      "ToStationTitle": "Каширская"
    },
    {
      "$type": "station",
      "Title": "Каширская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Каширская",
      "ToStationTitle": "Кантемировская"
    },
    {
      "$type": "station",
      "Title": "Кантемировская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Кантемировская",
      "ToStationTitle": "Царицыно"
    },
    {
      "$type": "station",
      "Title": "Царицыно",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Царицыно",
      "ToStationTitle": "Орехово"
    },
    {
      "$type": "station",
      "Title": "Орехово",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Орехово",
      "ToStationTitle": "Домодедовская"
    },
    {
      "$type": "station",
      "Title": "Домодедовская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Домодедовская",
      "ToStationTitle": "Красногвардейская"
    },
    {
      "$type": "station",
      "Title": "Красногвардейская",
      "BranchTitle": "Замоскворецкая линия"
    },
    {
      "$type": "railway",
      "FromStationTitle": "Красногвардейская",
      "ToStationTitle": "Алма-Атинская"
    },
    {
      "$type": "station",
      "Title": "Алма-Атинская",
      "BranchTitle": "Замоскворецкая линия"
    }
  ]
}'::jsonb);