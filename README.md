# mosze-jatekfejlesztes

SZE MOSZE 2025/26-2 játékfejlesztési projekt

## Aetheria - Rövid áttekintés

Az Aetheria egy 2D akció-platformer kalandjáték, amelyben a játékos célja, hogy egy veszélyekkel teli torony szintjein felfelé haladva megmentse az elrabolt királylányt.

A játék fantasy világban játszódik, ahol a torony minden szintje új kihívásokat tartogat:

- mozgó platformok
- csapdák és terepakadályok
- különböző viselkedésű ellenfelek
- fokozatosan növekvő nehézség

## Főbb játékelemek

- gyors, pontos időzítést igénylő mozgás
- ugrás és haladás platformokon keresztül
- ellenfél-interakciók és sebződés
- több pályatípus: ellenséges, puzzle és boss pálya

## Irányítás

| Billentyű | Funkció       |
| :-------- | :------------ |
| A         | Balra mozgás  |
| D         | Jobbra mozgás |
| Space     | Ugrás         |
| Esc       | Szünet menü   |

## Projekt futtatása

- A repository klónozása
- Unity Hub-ban: Add Project -> from disk -> a klónozott mappa kiválasztása
- A projekt megnyitása után a Scenes mappában dupla kattintás a GameScene-re
- Play gombbal futtatható a játék
- Frissítéshez: git pull

### Rendszerarchitektúra

A játék az alábbi főbb komponensekből és Unity rendszerekből épül fel:

```mermaid
graph TD
    %% Felhasználói interakció
    Input[Játékos Input <br> Billentyűzet / Esc] --> PC
    Input --> UIM

    %% Fő játéklogika (C# Scriptek)
    subgraph Játéklogika [Saját C# Scriptek]
        GM[Game Manager <br> Szintek, Játékállapot]
        PC[Player Controller <br> Mozgás, Ugrás, Életpont]
        EC[Enemy AI <br> Járőrözés, Boss harc]
    end

    %% Unity beépített rendszerek a 15. pont alapján
    subgraph Unity Engine Rendszerek
        Phys[Fizikai Rendszer <br> Rigidbody 2D, Collider 2D]
        Anim[Animációs Rendszer <br> Animator]
        UI[Unity UI <br> Canvas, Text, Button]
    end

    %% Adatkezelés a 16. pont alapján
    subgraph Adatkezelés
        Save[Mentési Rendszer <br> PlayerPrefs]
    end

    %% Kapcsolatok
    GM -->|Pontszám, Állapot mentése| Save
    GM -->|Pályák betöltése| PC
    GM -->|Pályák betöltése| EC

    PC <-->|Ütközés érzékelése, Sebzés| EC
    PC -->|Erőhatások, Pozíció| Phys
    EC -->|Erőhatások, Pozíció| Phys

    PC -->|Állapotváltás trigger| Anim
    EC -->|Állapotváltás trigger| Anim

    PC -->|Életpont küldése| UIM[UI Manager]
    GM -->|Idő, Menü állapot küldése| UIM
    UIM -->|Megjelenítés| UI
```
