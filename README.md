# PPO

## Название проекта

Приложение для поиска и планирования маршрутов в метрополитенах разных городов и мегаполисов.

## Описание идеи проекта

Создание приложения, обеспечивающее web-интерфейсное взаимодействие с базой данных, хранящей информацию о станциях, линиях, маршрутах, включая такие детали как загруженность станций, среденее время движения поездов между станциями и т.д., предоставляюещей многопользовательское взаимодействие, нацеленной на поиск и планирование оптимальных маршрутов в метрополитенах.

## Описание предметной области

Исследовать предметную область алгоритмов поиска оптимальных маршрутов.

## Анализ аналогичный решений

Сервис|Тип приложения|Авторизация|История|Цена
------|--------------|-----------|-------|----
Яндекс метро|Web|нет|нет|бесплатно
Яндекс метро|Мобильное (IOS, Android)|есть|есть|бесплатно
Google Maps|Мобильное (iOS, Android), Web|есть|есть|бесплатно
Metro|Мобильное (iOS, Android)|есть|есть|бесплатно
Moovit|Мобильное (iOS, Android), Web|есть|есть|бесплатно (с премиум-версией за $4,99/месяц)
Transit|Мобильное (iOS, Android)|есть|есть|Бесплатно (с премиум-версией за $4,99/месяц)

## Описание ролей

1. Незарегистрированный пользователь: доступ к схемам метрополитенов, поиск маршрутов.

2. Зарегистрированный пользователь: доступ к схемам метрополитенов, поиск маршрутов, сохранение найденных маршрутов.

3. Дежурный: доступ к схемам метрополитенов, поиск маршрутов, сохранение найденных маршрутов, обновление параметров веток, станций и переходов.

## Use-case диаграмма

![Use-Case - диаграмма](/images//charts/use-case.svg)

## ER диаграмма

![ER - диаграмма](/images//charts/ER.svg)

## Пользовательские сценарии

Регистрация/вход. Каждый из пользователей, за исключением незарегистрированных пользователей (для таких пользователей права выдаются автоматически), обязан авторизироваться, в противном случае он не получит прав.

Зарегистрированные пользователи, помимо возможности просмотра карты метрополитена и поиска маршрутов, могут сохранять и просматривать свою историю.

Дежурные имееют право изменять качественные атрибуты станций и переходов (загруженность, статус, время открытия и закрытия и т.д.), за которые они ответсвенны.

## Формализация ключевых бизнес-процессов

![BPMN - диаграмма](/images/charts/BPMN.svg)

## Описание типа приложения и выбранного технологического стека

Тип приложения - Web SPA

Технологический стек:

- Frontend __TypeScript__ + __React__

- Backend __C#__ +  __ASP.NET Core__

- База данных __PostgreSQL__



