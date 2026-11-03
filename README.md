# PickDiceCool

Приложение для учета аренды настольных игр.

## Автор

Шурупов Константин 11/2-РПО-25/1

## О проекте

Тема - прокат настольных игр. Мое место называется PickDiceCool. Это маленькое место, где можно взять настольную игру на нужное количество дней. Приложение нужно для учета клиентов, игр и аренды. Фишка места в том, что здесь есть разного рода игры и подобрать игру по количеству игроков легко и быстро.


## База

```mermaid
erDiagram
    Clients {
        int Id PK
        string FullName
        string Phone
        bool IsStudent
        string FavoriteGenre
    }

    Games {
        int Id PK
        string Title
        string Genre
        int PlayerCount
        bool IsPartyGame
    }

    Rentals {
        int Id PK
        int ClientId FK
        int GameId FK
        DateTime StartDate
        DateTime EndDate
        int PriceRub
        bool IsReturned
    }

    Clients ||--o{ Rentals : has
    Games ||--o{ Rentals : has
```